using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DataBrowser.Domain.Serialization;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Dto.Users;
using DataBrowser.Query.Nodes;
using DataBrowser.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WSHUB.Models.Request;
using WSHUB.Models.Response;

namespace WSHUB.Controllers
{
    [ApiController]
    [Route("Users")]
    public class UsersController : ApiBaseController
    {
        private readonly AuthenticationConfig _authenticationConfig;
        private readonly ILogger<UsersController> _logger;
        private readonly IRequestContext _requestContext;
        private readonly IUserService _userService;

        public UsersController(ILogger<UsersController> logger,
            IMediatorService mediatorService,
            IUserService userService,
            IRequestContext requestContext,
            IOptionsSnapshot<AuthenticationConfig> authenticationConfig)
            : base(mediatorService)
        {
            _logger = logger;
            _userService = userService;
            _requestContext = requestContext;
            _authenticationConfig = authenticationConfig.Value;
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAuthenticatedResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("/Auth/Token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequest tokenRequest)
        {
            var result = await _userService.GetTokenAsync(new TokenRequestDto
            {
                Email = tokenRequest.Email,
                Password = Encoding.UTF8.GetString(Convert.FromBase64String(tokenRequest.Password))
            });

            if (!string.IsNullOrEmpty(result.RefreshToken))
                setRefreshTokenInCookie(result.RefreshToken);
            else
                Response.Cookies.Delete("refreshToken");

            if (!result.IsAuthenticated)
            {
                _logger.LogDebug($"401 error for: {result.Message}");
                return Unauthorized();
            }

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAuthenticatedResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("/Auth/RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _userService.RefreshTokenAsync(refreshToken);
            if (!string.IsNullOrEmpty(result?.RefreshToken))
            {
                //setRefreshTokenInCookie(result.RefreshToken);
            }
            else
            {
                Response.Cookies.Delete("refreshToken");
            }

            if (!result.IsAuthenticated)
            {
                _logger.LogDebug($"401 error for: {result.Message}");
                return Unauthorized();
            }
            return Ok(result);
        }

        private void setRefreshTokenInCookie(string refreshToken)
        {
            var cookieDomain = _authenticationConfig?.RefreshCookieOptions?.Domain;
            if (cookieDomain == null)
            {
                _logger.LogDebug("cookieDomain is null, start to auto generate");
                cookieDomain = Request.Host.HasValue ? Request.Host.Value : null;
            }

            _logger.LogDebug($"cookieDomain value is {cookieDomain}");

            var cookiePath = _authenticationConfig?.RefreshCookieOptions?.Path;
            if (string.IsNullOrWhiteSpace(cookiePath))
            {
                _logger.LogDebug("cookiePath value is null");
                cookiePath = Request.Path.Value.Replace("\\", "/", StringComparison.InvariantCultureIgnoreCase)
                    .TrimEnd('/');
                cookiePath = cookiePath.Remove(cookiePath.LastIndexOf('/') + 1);
                if (HttpContext.Request.PathBase.HasValue)
                {
                    _logger.LogDebug("cookiePath have base path");
                    var pathBase = HttpContext.Request.PathBase.Value
                        .Replace("\\", "/", StringComparison.InvariantCultureIgnoreCase).TrimEnd('/');
                    cookiePath = $"{pathBase}{cookiePath}";
                }
            }

            _logger.LogDebug($"cookiePath value is {cookiePath}");

            var cookieHttpOnly = _authenticationConfig?.RefreshCookieOptions?.HttpOnly;
            if (!cookieHttpOnly.HasValue) cookieHttpOnly = true;

            var cookieHttpSecure = _authenticationConfig?.RefreshCookieOptions?.Secure;
            if (!cookieHttpSecure.HasValue) cookieHttpSecure = true;

            var cookieRefreshTokenLifeTime =
                _authenticationConfig?.RefreshCookieOptions?.RefreshTokenLifeTimeOrDefaultValue;
            if (!cookieRefreshTokenLifeTime.HasValue) cookieRefreshTokenLifeTime = 14400;
            _logger.LogDebug($"cookieRefreshTokenLifeTime value is {cookieRefreshTokenLifeTime}");

            var cookieSameSiteString = _authenticationConfig?.RefreshCookieOptions?.SameSite;
            var cookieSameSite = SameSiteMode.Lax;
            if (cookieSameSiteString != null)
                switch (cookieSameSiteString.ToUpperInvariant())
                {
                    case "LAX":
                        cookieSameSite = SameSiteMode.Lax;
                        break;
                    case "NONE":
                        cookieSameSite = SameSiteMode.None;
                        break;
                    case "STRICT":
                        cookieSameSite = SameSiteMode.Strict;
                        break;
                    case "UNSPECIFIED":
                        cookieSameSite = SameSiteMode.Unspecified;
                        break;
                }

            _logger.LogDebug($"cookieSameSite value is {cookieSameSite}");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = cookieHttpOnly.Value,
                Expires = DateTime.UtcNow.AddDays(cookieRefreshTokenLifeTime.Value),
                SameSite = cookieSameSite,
                Secure = cookieHttpSecure.Value
            };

            if (!string.IsNullOrWhiteSpace(cookiePath)) cookieOptions.Path = cookiePath;
            if (!string.IsNullOrWhiteSpace(cookieDomain)) cookieOptions.Domain = cookieDomain;

            var cookieName = _authenticationConfig?.RefreshCookieOptions?.Name;
            if (string.IsNullOrWhiteSpace(cookieName)) cookieName = "refreshToken";
            _logger.LogDebug($"cookieName value is {cookieName}");
            Response.Cookies.Append(cookieName, refreshToken, cookieOptions);
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserRegisterResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("Register")]
        public async Task<ActionResult> RegisterAsync(UserRegisterRequest userRegisterRequest)
        {
            var userRegisterDto = new UserRegisterDto
            {
                FirstName = userRegisterRequest.FirstName,
                LastName = userRegisterRequest.LastName,
                Email = userRegisterRequest.Email,
                Password = userRegisterRequest.Password,
                //Password = Encoding.UTF8.GetString(Convert.FromBase64String(userRegisterRequest.Password)),
                Username = userRegisterRequest.Email,
                Organization = userRegisterRequest.Organization,
                Type = userRegisterRequest.Type
            };
            var result = await _userService.RegisterAsync(userRegisterDto);
            return Ok(result);
        }

        [HttpPost("{userId}/NodeCross/Permissions")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> SetCrossPermissions(int userId, int nodeId,
            [FromBody] List<PermissionType.NodePermission> nodePermissions)
        {
            var operationResult = await _userService.SetCrossNodePermissionAsync(userId, nodePermissions);

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = operationResult ? null : $"User {userId} not found";
            result.StatusCode = operationResult ? 204 : 404;
            return result;
        }

        [HttpPost("{userId}/RefreshToken/Remove")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> BlacklistRefreshToken(int userId)
        {
            var resultOperation = await _userService.RemoveUserRefreshToken(userId);

            var result = new ContentResult();
            result.ContentType = resultOperation ? "application/json" : "application/text";
            result.Content = resultOperation ? null : $"User {userId} not found";
            result.StatusCode = resultOperation ? 204 : 404;
            return result;
        }

        [HttpPost("{userId}/Roles")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> SetRoles(int userId, [FromBody] List<UserAndGroup.Roles> roles)
        {
            var operationResult = await _userService.SetRoles(userId, roles);

            var result = new ContentResult();
            result.ContentType = operationResult ? "application/json" : "application/text";
            result.Content = operationResult ? null : $"User {userId} not found";
            result.StatusCode = operationResult ? 204 : 404;
            return result;
        }

        [HttpPost("{userId}/Disable")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> DisableUser(int userId)
        {
            if (userId == _requestContext.LoggedUserId)
            {
                var resultBadRequest = new ContentResult();
                resultBadRequest.ContentType = "application/text";
                resultBadRequest.Content = $"User {userId} can't disable User {_requestContext.LoggedUserId}";
                resultBadRequest.StatusCode = 400;
                return resultBadRequest;
            }

            var operationResult = await _userService.DisableUser(userId);

            var result = new ContentResult();
            result.ContentType = operationResult ? "application/json" : "application/text";
            result.Content = operationResult ? null : $"User {userId} not found";
            result.StatusCode = operationResult ? 204 : 404;
            return result;
        }


        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserList>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> GetUserList()
        {
            var operationResult = await _userService.GetUsers();

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(operationResult);
            result.StatusCode = 200;
            return result;
        }

        [HttpGet("Permissions/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserList>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> GetUsersNodePermission()
        {
            var nodeRequest = await QueryAsync(new NodeByIdQuery(_requestContext.NodeId));
            if (nodeRequest == null)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found");
                var resultNotFound = new ContentResult();
                resultNotFound.ContentType = "application/text";
                resultNotFound.Content = "Node not found";
                resultNotFound.StatusCode = 404;
                return resultNotFound;
            }

            var operationResult = await _userService.GetUsersWithSingleNodePermission(_requestContext.NodeId);

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(operationResult);
            result.StatusCode = 200;
            return result;
        }

        [HttpPut("{userId}/Permissions/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> SetUserSinglePermissions(int userId, int nodeId,
            [FromBody] List<PermissionType.NodePermission> nodePermissions)
        {
            var nodeRequest = await QueryAsync(new NodeByIdQuery(_requestContext.NodeId));
            if (nodeRequest == null)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found");
                var resultNotFound = new ContentResult();
                resultNotFound.ContentType = "application/text";
                resultNotFound.Content = "Node not found";
                resultNotFound.StatusCode = 404;
                return resultNotFound;
            }

            var operationResult =
                await _userService.SetSingleNodePermissionAsync(userId, _requestContext.NodeId, nodePermissions);

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = operationResult ? null : $"User {userId} not found";
            result.StatusCode = operationResult ? 204 : 404;
            return result;
        }

        [HttpPut("Permissions/Nodes/{nodeId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EditUsersPermissionsModelView))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> SetMultiUserSinglePermissions(int nodeId,
            [FromBody] List<EditUsersPermissionsRequest> editUsersPermissionsRequests)
        {
            var nodeRequest = await QueryAsync(new NodeByIdQuery(_requestContext.NodeId));
            if (nodeRequest == null)
            {
                _logger.LogDebug($"Node {_requestContext.NodeId} not found");
                var resultNotFound = new ContentResult();
                resultNotFound.ContentType = "application/text";
                resultNotFound.Content = "Node not found";
                resultNotFound.StatusCode = 404;
                return resultNotFound;
            }

            var editUsersPermissionsResponse = new EditUsersPermissionsModelView();
            if (editUsersPermissionsRequests != null)
            {
                editUsersPermissionsResponse.ErrorsUserId = new List<int>();
                foreach (var item in editUsersPermissionsRequests)
                {
                    var operationResult = false;
                    try
                    {
                        operationResult = await _userService.SetSingleNodePermissionAsync(item.UserId,
                            _requestContext.NodeId, item.NodePermissions);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"SetMultiUserSinglePermissions Error: {ex.Message}");
                        operationResult = false;
                    }

                    if (!operationResult)
                    {
                        editUsersPermissionsResponse.HaveError = true;
                        editUsersPermissionsResponse.ErrorsUserId.Add(item.UserId);
                    }
                }
            }


            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(editUsersPermissionsResponse);
            result.StatusCode = 200;
            return result;
        }


        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserList))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> GetUserList(int userId)
        {
            var operationResult = await _userService.GetUser(userId);

            var result = new ContentResult();
            result.ContentType = "application/json";
            result.Content = DataBrowserJsonSerializer.SerializeObject(operationResult);
            result.StatusCode = operationResult != null ? 200 : 404;
            return result;
        }

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDeleteDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(DeleteGenericEntityModelView))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (userId == _requestContext.LoggedUserId)
            {
                var resultBadRequest = new ContentResult();
                resultBadRequest.ContentType = "application/text";
                resultBadRequest.Content = $"User {userId} can't delete User {_requestContext.LoggedUserId}";
                resultBadRequest.StatusCode = 400;
                return resultBadRequest;
            }

            var operationResult = await _userService.DeleteUser(userId);

            var result = new ContentResult();
            result.ContentType = !operationResult.NotFound ? "application/json" : "application/text";
            result.Content = !operationResult.NotFound
                ? DataBrowserJsonSerializer.SerializeObject(
                    DeleteGenericEntityModelView.ConvertFromUser(operationResult, _requestContext.UserLang))
                : $"User {userId} not found";
            if (operationResult.NotFound)
                result.StatusCode = 404;
            else if (operationResult.Dashboards != null &&
                     operationResult.Dashboards.Count > 0)
                result.StatusCode = 409;
            else
                result.StatusCode = 200;
            return result;
        }

        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = UserAndGroup.RoleAdministrator)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto userUpdateDto)
        {
            var operationResult = await _userService.UpdateUser(userId, userUpdateDto);

            var result = new ContentResult();
            result.ContentType = "application/text";
            result.Content = operationResult ? null : $"User {userId} not found";
            result.StatusCode = operationResult ? 204 : 404;
            return result;
        }

        [HttpPost("RecoveryPassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecoveryPassword()
        {
            string userName = null;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                userName = await reader.ReadToEndAsync();
            }
            var operationResult = await _userService.RecoveryPassword(userName);

            var result = new ContentResult();
            result.ContentType = operationResult ? "application/json" : "application/text";
            result.Content = operationResult ? null : $"User {userName} not found";
            result.StatusCode = operationResult ? 204 : 404;
            return result;
        }

        [HttpPost("ResetPassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(UserRegisterResult))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var operationResult = await _userService.ResetPassword(resetPasswordRequest.Username,
                                                                    resetPasswordRequest.Token,
                                                                    Encoding.UTF8.GetString(Convert.FromBase64String(resetPasswordRequest.Password)));

            if (operationResult == null)
            {
                var resultNotFound = new ContentResult();
                resultNotFound.ContentType = "application/text";
                resultNotFound.Content = $"User {resetPasswordRequest.Username} not found";
                resultNotFound.StatusCode = 404;
                return resultNotFound;
            }

            var result = new ContentResult();
            result.ContentType = !operationResult.HaveError ? "application/text" : "application/json";
            result.Content = !operationResult.HaveError ? null : DataBrowserJsonSerializer.SerializeObject(operationResult);
            result.StatusCode = !operationResult.HaveError ? 204 : 400;
            return result;
        }

        [HttpPost("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePassword)
        {
            var operationResult = await _userService.ChangePassword(_requestContext.LoggedUserId,
                                                                    Encoding.UTF8.GetString(Convert.FromBase64String(changePassword.OldPassword)),
                                                                    Encoding.UTF8.GetString(Convert.FromBase64String(changePassword.NewPassword)));

            if (operationResult == null)
            {
                var resultNotFound = new ContentResult();
                resultNotFound.ContentType = "application/text";
                resultNotFound.Content = $"User not found";
                resultNotFound.StatusCode = 404;
                return resultNotFound;
            }

            var result = new ContentResult();
            result.ContentType = !operationResult.HaveError ? "application/text" : "application/json";
            result.Content = !operationResult.HaveError ? null : DataBrowserJsonSerializer.SerializeObject(operationResult);
            result.StatusCode = !operationResult.HaveError ? 204 : 400;
            return result;
        }

    }
}