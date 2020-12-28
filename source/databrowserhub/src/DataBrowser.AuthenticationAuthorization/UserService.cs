using AutoMapper;
using DataBrowser.AC.Responses.Services;
using DataBrowser.AC.Utility;
using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.Nodes;
using DataBrowser.Domain.Entities.Users;
using DataBrowser.Domain.Entities.ViewTemplates;
using DataBrowser.Domain.Interfaces.Repositories;
using DataBrowser.Interfaces;
using DataBrowser.Interfaces.Authentication;
using DataBrowser.Interfaces.Configuration;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Dto.Users;
using DataBrowser.Interfaces.Mail;
using DataBrowser.Services.Interfaces;
using DataBrowser.Specifications.Dashboards;
using DataBrowser.Specifications.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataBrowser.AuthenticationAuthorization
{
    public class UserService : IUserService
    {
        private readonly AuthenticationConfig _authConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly IMediatorService _mediatorService;
        private readonly IRepository<Dashboard> _repositoryDashboard;
        private readonly IRepository<Node> _repositoryNode;
        private readonly IRepository<ViewTemplate> _repositoryViewTemplate;
        private readonly IRequestContext _requestContext;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRepository<UserAudit> _userAuditRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IEmailService _emailService;

        public UserService(ILogger<UserService> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptionsSnapshot<AuthenticationConfig> authConfig,
            IRepository<ApplicationUser> userRepository,
            IRepository<UserAudit> userAuditRepository,
            IHttpContextAccessor httpContextAccessor,
            IRequestContext requestContext,
            IMediatorService mediatorService,
            IRepository<Dashboard> repositoryDashboard,
            IRepository<Node> repositoryNode,
            IRepository<ViewTemplate> repositoryViewTemplate,
            IMapper mapper,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _authConfig = authConfig.Value;
            _userRepository = userRepository;
            _userAuditRepository = userAuditRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _requestContext = requestContext;
            _mediatorService = mediatorService;
            _repositoryDashboard = repositoryDashboard;
            _repositoryNode = repositoryNode;
            _repositoryViewTemplate = repositoryViewTemplate;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<UserRegisterResult> RegisterAsync(UserRegisterDto userRegister)
        {
            var user = new ApplicationUser
            {
                UserName = userRegister.Email,
                Email = userRegister.Email,
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                Organizzation = userRegister.Organization,
                Type = userRegister.Type
            };
            var result = await _userManager.CreateAsync(user, userRegister.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, UserAndGroup.DefaultRegisterRole.ToString());
                var claim = new Claim(PermissionType.GetPermissionNameForCroosNode(),
                    PermissionType.NodePermission.ManageView.ToString(), ClaimValueTypes.String);
                await _userManager.AddClaimAsync(user, claim);

                return new UserRegisterResult { UserId = user.Id, Username = user.UserName };
            }

            return new UserRegisterResult { HaveError = true, Errors = result.Errors.Select(i => !string.IsNullOrWhiteSpace(i.Code) ? i.Code : i.Description).ToList() };
        }

        public async Task<UserAuthenticatedResult> GetTokenAsync(TokenRequestDto tokenRequest)
        {
            UserAuthenticatedResult authenticationModel = null;

            if (tokenRequest.Email.Equals(UserAndGroup.ServiceUsername, StringComparison.InvariantCultureIgnoreCase))
            {
                var authenticationService = createAuthenticatedUserService(tokenRequest.Password);

                if (authenticationService == null)
                {
                    authenticationService = new UserAuthenticatedResult
                    {
                        IsAuthenticated = false,
                        Message = $"Incorrect Credentials for user {tokenRequest.Email}."
                    };
                }

                return authenticationService;
            }

            var user = await _userManager.FindByEmailAsync(tokenRequest.Email);
            if (user == null)
            {
                authenticationModel = new UserAuthenticatedResult
                {
                    IsAuthenticated = false,
                    Message = $"No Accounts Registered with {tokenRequest.Email}."
                };
                return authenticationModel;
            }

            var currentErrorLogin = await GetLastErrorLogins(user.Id);
            var maxTry = _authConfig.TryLoginMax > 0 ? _authConfig.TryLoginMax : Int32.MaxValue;
            if (currentErrorLogin > maxTry)
            {
                await Task.Delay(_authConfig.DelayLogin * (currentErrorLogin - _authConfig.TryLoginMax));
            }

            if (user.IsDisable)
            {
                auditUserLogin(UserAuditEventType.FailedLogin, user.Id,
                _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
                await _userRepository.UnitOfWork.SaveChangesAsync();

                authenticationModel = new UserAuthenticatedResult
                {
                    IsAuthenticated = false,
                    Message = $"Account Registered with {tokenRequest.Email} is disabled."
                };
                return authenticationModel;
            }

            if (await _userManager.CheckPasswordAsync(user, tokenRequest.Password))
            {
                authenticationModel = await createAuthenticatedUserAsync(user);

                if (_authConfig.EnableRefreshToken)
                {
                    if (user.RefreshTokens != null && user.RefreshTokens.Any(a => a.IsActive))
                    {
                        var activeRefreshToken = user.RefreshTokens.FirstOrDefault(a => a.IsActive);
                        authenticationModel.RefreshToken = activeRefreshToken.Token;
                        authenticationModel.RefreshTokenExpiration = activeRefreshToken.Expires;
                    }
                    else
                    {
                        var refreshToken = CreateRefreshToken();
                        authenticationModel.RefreshToken = refreshToken.Token;
                        authenticationModel.RefreshTokenExpiration = refreshToken.Expires;

                        user.AddToken(refreshToken);

                        _userRepository.Update(user);
                    }
                }

                auditUserLogin(UserAuditEventType.Login, user.Id,
                    _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
                await _userRepository.UnitOfWork.SaveChangesAsync();

                return authenticationModel;
            }

            authenticationModel = new UserAuthenticatedResult
            {
                IsAuthenticated = false,
                Message = $"Incorrect Credentials for user {user.Email}."
            };

            auditUserLogin(UserAuditEventType.FailedLogin, user.Id,
                _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString());
            await _userRepository.UnitOfWork.SaveChangesAsync();

            return authenticationModel;
        }

        public async Task<UserAuthenticatedResult> RefreshTokenAsync(string token)
        {
            if (!_authConfig.EnableRefreshToken)
            {
                return null;
            }

            var authenticationModel = new UserAuthenticatedResult();
            var users = await _userRepository.FindAsync(new UserByRefreshTokenSpecification(token));
            var user = users?.SingleOrDefault();
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Token did not match any users.";
                return authenticationModel;
            }

            if (user.IsDisable)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Token assign to a disabled user.";
                return authenticationModel;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = "Token Not Active.";
                return authenticationModel;
            }

            ////Revoke Current Refresh Token
            //refreshToken.Revoked = DateTime.UtcNow;

            ////Generate new Refresh Token and save to Database
            //var newRefreshToken = CreateRefreshToken();
            //user.AddToken(newRefreshToken);
            //_userRepository.Update(user);
            //await _userRepository.UnitOfWork.SaveChangesAsync();

            authenticationModel = await createAuthenticatedUserAsync(user);
            authenticationModel.RefreshToken = refreshToken.Token;
            authenticationModel.RefreshTokenExpiration = refreshToken.Expires;

            return authenticationModel;
        }

        public async Task<bool> RemoveUserRefreshToken(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            if (user.RefreshTokens != null)
            {
                var revokeDateTime = DateTime.UtcNow;
                foreach (var item in user.RefreshTokens)
                {
                    if (item.IsActive)
                    {
                        item.Revoked = revokeDateTime;
                    }
                }

                _userRepository.Update(user);
                await _userRepository.UnitOfWork.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> RemoveAllNodePermissionAsync(int nodeId)
        {
            _logger.LogDebug("START RemoveAllNodePermissionAsync");

            foreach (var user in _userManager.Users.ToList())
            {
                var claims = await _userManager.GetClaimsAsync(user);
                if (claims == null)
                {
                    continue;
                }

                await _userManager.RemoveClaimsAsync(user,
                    claims.Where(i => i.Type.Equals(PermissionType.GetPermissionNameForSingleNode(nodeId))));
            }

            _logger.LogDebug("END RemoveAllNodePermissionAsync");

            return true;
        }

        public async Task<bool> SetRoles(int userId, List<UserAndGroup.Roles> roles)
        {
            _logger.LogDebug("START AssignRole");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles.ToArray());

            if (roles != null)
            {
                var rolesString = roles.Select(i => i.ToString()).ToList();
                await _userManager.AddToRolesAsync(user, rolesString);
            }


            _logger.LogDebug("END AssignRole");

            return true;
        }

        public async Task<bool> DisableUser(int userId)
        {
            _logger.LogDebug("START DisableUser");

            if (_requestContext.LoggedUserId == userId)
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            user.IsDisable = true;
            await _userManager.UpdateAsync(user);

            _logger.LogDebug("END DisableUser");

            return true;
        }

        public async Task<List<UserList>> GetUsers()
        {
            var usersEntity = await _userManager.Users.Include("UserRoles.Role").ToListAsync();

            return usersEntity?.Select(item => new UserList
            {
                UserId = item.Id,
                Email = item.Email,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Username = item.UserName,
                Organization = item.Organizzation,
                Roles = item?.UserRoles?.Select(i => i?.Role.Name).Where(i => i != null).ToList(),
                Permission = null,
                IsActive = !item.IsDisable,
                Type = item.Type
            })?.ToList();
        }

        public async Task<List<UserList>> GetUsersWithSingleNodePermission(int nodeId)
        {
            var users = new List<UserList>();

            var usersEntity = await _userManager.Users.Include("UserRoles.Role")
                .Include("UserClaims")
                .ToListAsync();
            if (usersEntity != null)
            {
                foreach (var item in usersEntity)
                {
                    var userClaim = item.UserClaims.Select(i => new Claim(i.ClaimType, i.ClaimValue)).ToList();
                    users.Add(new UserList
                    {
                        UserId = item.Id,
                        Email = item.Email,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Username = item.UserName,
                        Organization = item.Organizzation,
                        Roles = item?.UserRoles?.Select(i => i?.Role.Name).Where(i => i != null).ToList(),
                        Permission = GetPermissionSingleNodeFromClaims(nodeId, userClaim),
                        IsActive = !item.IsDisable
                    });
                }
            }

            return users;
        }

        public async Task<UserList> GetUser(int userId)
        {
            var userEntity = await _userManager.Users
                .Include("UserRoles.Role")
                .Include("UserClaims")
                .Where(i => i.Id == userId)
                .FirstOrDefaultAsync();
            if (userEntity != null)
            {
                var userClaim = userEntity.UserClaims.Select(i => new Claim(i.ClaimType, i.ClaimValue)).ToList();
                return new UserList
                {
                    UserId = userEntity.Id,
                    Email = userEntity.Email,
                    FirstName = userEntity.FirstName,
                    LastName = userEntity.LastName,
                    Username = userEntity.UserName,
                    Organization = userEntity.Organizzation,
                    Roles = userEntity?.UserRoles?.Select(i => i?.Role.Name).Where(i => i != null).ToList(),
                    Permission = GetPermissionNodesFromClaims(userClaim),
                    IsActive = !userEntity.IsDisable,
                    Type = userEntity.Type
                };
            }

            return null;
        }

        public async Task<string> GetUsername(int userId)
        {
            var userEntity = await _userManager.Users
                .Where(i => i.Id == userId)
                .FirstOrDefaultAsync();
            if (userEntity != null)
            {
                return userEntity.UserName;
            }

            return null;
        }

        public async Task<UserDeleteDto> DeleteUser(int userId)
        {
            _logger.LogDebug("START DeleteUser");

            if (_requestContext.LoggedUserId == userId)
            {
                return new UserDeleteDto { CanDeleteUserItself = true };
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogDebug($"user {userId} not found");
                return new UserDeleteDto { NotFound = true };
            }

            var dashboards =
                await _repositoryDashboard.FindAsync(
                    new DashboardByUserIdSpecification(userId, withNodes: false, withViews: false));

            var userDeleteDto = new UserDeleteDto { Deleted = false };

            var dahboardDto = dashboards?.Where(i => i.HubFk.HasValue && i.HubFk.Value > 0 ||
                                                     i.Nodes != null && i.Nodes.Any())
                ?.Select(i => i.ConvertToDashboardDto(_mapper))?.ToList();
            if (dahboardDto != null && dahboardDto.Any())
            {
                userDeleteDto.Dashboards = new List<UserDeleteDto.EntityUseDto>();
                foreach (var dash in dahboardDto)
                {
                    var nodes = new List<NodeMinimalInfoDto>();
                    foreach (var item in dash.NodeIds)
                    {
                        var nodeEntity = await _repositoryNode.GetByIdAsync(item);
                        if (nodeEntity == null)
                        {
                            continue;
                        }

                        nodes.Add(nodeEntity.ConvertToNodeDataView(_mapper));
                    }

                    var views = new List<ViewTemplateDto>();
                    foreach (var item in dash.ViewIds)
                    {
                        var viewEntity = await _repositoryViewTemplate.GetByIdAsync(item);
                        if (viewEntity == null)
                        {
                            continue;
                        }

                        views.Add(viewEntity.ConvertToViewTemplateDto(_mapper));
                    }

                    var dashDto = new UserDeleteDto.EntityUseDto
                    {
                        Id = dash.DashboardId,
                        Title = dash.Title,
                        Nodes = nodes.Select(i => new UserDeleteDto.EntityUseDto { Id = i.NodeId, Title = i.Title })
                            .ToList(),
                        Views = views.Select(i => new UserDeleteDto.EntityUseDto
                        { Id = i.ViewTemplateId, Title = i.Title }).ToList()
                    };
                    userDeleteDto.Dashboards.Add(dashDto);
                }

                return userDeleteDto;
            }

            _logger.LogDebug("try to delete");
            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogInformation($"Delete user {userId} failed: {result.Errors.SelectMany(i => i.Code + ";")}");
                return new UserDeleteDto();
            }

            userDeleteDto.Deleted = true;

            _logger.LogDebug("call UserRemovedPublicEvent");

            await _mediatorService.Publish(new UserRemovedPublicEvent(userId));

            _logger.LogDebug("END DeleteUser");

            return new UserDeleteDto { Deleted = true };
        }

        public async Task<bool> UpdateUser(int userId, UserUpdateDto updateDataDto)
        {
            _logger.LogDebug("START UpdateUser");


            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            user.FirstName = updateDataDto.FirstName;
            user.LastName = updateDataDto.LastName;
            user.Organizzation = updateDataDto.Organization;
            user.IsDisable = !updateDataDto.IsActive;
            user.Type = updateDataDto.Type;
            await _userManager.UpdateAsync(user);

            _logger.LogDebug("END UpdateUser");

            return true;
        }

        public async Task<UserRegisterResult> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            _logger.LogDebug("START ChangePassword");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return null;
            }

            var resultOperation = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            var result = new UserRegisterResult { UserId = user.Id, Username = user.UserName };
            if (!resultOperation.Succeeded)
            {
                result.HaveError = true;
                result.Errors = resultOperation.Errors.Select(i => i.Code).ToList();
            }

            _logger.LogDebug("END ChangePassword");

            return result;
        }

        public async Task<bool> RecoveryPassword(string username)
        {
            _logger.LogDebug("START RecoveryPassword");

            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                _logger.LogDebug($"mail not found {username}");
                return false;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailService.RecoveryPasswordAsync(user.Email, resetToken);

            _logger.LogDebug("END RecoveryPassword");

            return true;
        }

        public async Task<UserRegisterResult> ResetPassword(string username, string resetToken, string newPassword)
        {
            _logger.LogDebug("START ResetPassword");

            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                _logger.LogDebug($"mail not found {username}");
                return null;
            }

            var resultOperation = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            var result = new UserRegisterResult { UserId = user.Id, Username = user.UserName };
            if (!resultOperation.Succeeded)
            {
                result.HaveError = true;
                result.Errors = resultOperation.Errors.Select(i => i.Code).ToList();
            }

            _logger.LogDebug("END ResetPassword");

            return result;
        }

        public async Task<bool> AddSingleNodePermissionAsync(int userId, int nodeId,
            PermissionType.NodePermission nodePermission)
        {
            return await addNodePermission(userId, nodeId, nodePermission);
        }

        public async Task<bool> RemoveSingleNodePermissionAsync(int userId, int nodeId,
            PermissionType.NodePermission nodePermission)
        {
            return await removeNodePermission(userId, nodeId, nodePermission);
        }

        public async Task<bool> SetSingleNodePermissionAsync(int userId, int nodeId,
            List<PermissionType.NodePermission> nodePermissions)
        {
            return await setNodePermission(userId, nodeId, nodePermissions);
        }

        public async Task<bool> AddCrossNodePermissionAsync(int userId, PermissionType.NodePermission nodePermission)
        {
            return await addNodePermission(userId, null, nodePermission);
        }

        public async Task<bool> RemoveCrossNodePermissionAsync(int userId, PermissionType.NodePermission nodePermission)
        {
            return await removeNodePermission(userId, null, nodePermission);
        }

        public async Task<bool> SetCrossNodePermissionAsync(int userId,
            List<PermissionType.NodePermission> nodePermissions)
        {
            return await setNodePermission(userId, null, nodePermissions);
        }


        private JwtSecurityToken CreateJwtToken(ApplicationUser user, IList<Claim> userClaims, IList<string> roles)
        {
            var roleClaims = new List<Claim>();
            for (var i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimValues.UserId, user.Id.ToString())
                }
                .Union(userClaims)
                .Union(roleClaims);
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfig.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                _authConfig.Issuer,
                _authConfig.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_authConfig.JwtTokenLifeTime),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomNumber);
                var cookieRefreshTokenLifeTime = _authConfig?.RefreshCookieOptions?.RefreshTokenLifeTimeOrDefaultValue;
                if (!cookieRefreshTokenLifeTime.HasValue)
                {
                    cookieRefreshTokenLifeTime = 14400;
                }

                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddMinutes(cookieRefreshTokenLifeTime.Value),
                    Created = DateTime.UtcNow
                };
            }
        }

        private async Task<UserAuthenticatedResult> createAuthenticatedUserAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var userAuthenticated = new UserAuthenticatedResult
            {
                IsAuthenticated = true
            };
            var jwtSecurityToken = CreateJwtToken(user, userClaims, roles);
            userAuthenticated.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            userAuthenticated.TokenTTL = _authConfig.JwtTokenLifeTime;
            userAuthenticated.Email = user.Email;
            userAuthenticated.UserName = user.UserName;
            userAuthenticated.Roles = roles.ToList();
            userAuthenticated.Permissions = GetPermissionNodesFromClaims(userClaims);
            userAuthenticated.Type = user.Type;

            return userAuthenticated;
        }

        private UserAuthenticatedResult createAuthenticatedUserService(string password)
        {
            if (string.IsNullOrWhiteSpace(_authConfig.UserServicesPassword))
            {
                return null;
            }

            if (!CryptUtility
                .SimpleDecryptWithPassword(_authConfig.UserServicesPassword, CryptUtility.PasswordUserService)
                .Equals(password))
            {
                return null;
            }

            var userClaims = new List<Claim> { new Claim(ClaimValues.ServiceUser, "1") };
            var roles = new List<string> { UserAndGroup.RoleService };

            var applicationUser = new ApplicationUser
            {
                Id = UserAndGroup.ServiceUserId,
                Email = UserAndGroup.ServiceUsername,
                UserName = UserAndGroup.ServiceUsername
            };

            var userAuthenticated = new UserAuthenticatedResult
            {
                IsAuthenticated = true
            };
            var jwtSecurityToken = CreateJwtToken(applicationUser, userClaims, roles);
            userAuthenticated.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            userAuthenticated.TokenTTL = _authConfig.JwtTokenLifeTime;
            userAuthenticated.Email = UserAndGroup.ServiceUsername;
            userAuthenticated.UserName = UserAndGroup.ServiceUsername;
            userAuthenticated.Roles = roles;
            userAuthenticated.Permissions = new List<string>();

            return userAuthenticated;
        }

        public List<string> GetPermissionNodesFromClaims(IList<Claim> claims)
        {
            return getPermissionFromClaims(null, claims);
        }

        public List<string> GetPermissionSingleNodeFromClaims(int nodeId, IList<Claim> claims)
        {
            return getPermissionFromClaims(nodeId, claims);
        }

        private List<string> getPermissionFromClaims(int? nodeId, IList<Claim> claims)
        {
            IEnumerable<Claim> filtered;
            if (nodeId.HasValue)
            {
                filtered = claims.Where(i =>
                    i.Type != null && i.Type.Equals(PermissionType.PermissionCroosNodeType,
                        StringComparison.InvariantCultureIgnoreCase) ||
                    i.Type != null && i.Type.Equals($"{PermissionType.PermissionSingleNodeType}{nodeId}",
                        StringComparison.InvariantCultureIgnoreCase));
            }
            else
            {
                filtered = claims.Where(i => i.Type != null && i.Type.StartsWith("Permission"));
            }

            return filtered.Select(i => PermissionType.ConvertClaimToPermissionViewModel(i)).ToList();
        }


        private void auditUserLogin(UserAuditEventType type, int userId, string ipAddress)
        {
            if (_authConfig.EnableAuditLogin)
            {
                var userAudit = UserAudit.CreateAuditEvent(userId.ToString(), type, ipAddress ?? "");
                _userAuditRepository.Add(userAudit);
            }
        }

        private async Task<bool> addNodePermission(int userId, int? nodeId,
            PermissionType.NodePermission nodePermission)
        {
            _logger.LogDebug("START addNodePermission");
            var user = await _userManager.Users
                .Include("UserClaims")
                .Where(i => i.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }

            _logger.LogDebug("user found");

            Claim claim;
            if (nodeId.HasValue)
            {
                claim = new Claim(PermissionType.GetPermissionNameForSingleNode(nodeId.Value),
                    nodePermission.ToString(), ClaimValueTypes.String);
            }
            else
            {
                claim = new Claim(PermissionType.GetPermissionNameForCroosNode(), nodePermission.ToString(),
                    ClaimValueTypes.String);
            }

            _logger.LogDebug($"claimType {claim.Type}\t claimValue {claim.Value}");

            var exist = user.UserClaims.Any(i =>
                i.ClaimType.Equals(claim.Type, StringComparison.InvariantCultureIgnoreCase) &&
                i.ClaimValue.Equals(claim.Value, StringComparison.InvariantCultureIgnoreCase));

            if (!exist)
            {
                _logger.LogDebug("try to add claim");
                await _userManager.AddClaimAsync(user, claim);
            }

            return true;
        }

        private async Task<bool> removeNodePermission(int userId, int? nodeId,
            PermissionType.NodePermission nodePermission)
        {
            _logger.LogDebug("START removeNodePermission");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            _logger.LogDebug("user found");

            Claim claim = null;
            if (nodeId.HasValue)
            {
                claim = new Claim(PermissionType.GetPermissionNameForSingleNode(nodeId.Value),
                    nodePermission.ToString(), ClaimValueTypes.String);
            }
            else
            {
                claim = new Claim(PermissionType.GetPermissionNameForCroosNode(), nodePermission.ToString(),
                    ClaimValueTypes.String);
            }

            _logger.LogDebug($"claimType {claim.Type}\t claimValue {claim.Value}");

            await _userManager.RemoveClaimAsync(user, claim);

            _logger.LogDebug("END removeNodePermission");

            return true;
        }

        private async Task<bool> setNodePermission(int userId, int? nodeId,
            List<PermissionType.NodePermission> nodePermissions)
        {
            _logger.LogDebug("START setNodePermission");

            var user = await _userManager.Users
                .Include("UserClaims")
                .Where(i => i.Id == userId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return false;
            }

            _logger.LogDebug("user found");
            string claimType;
            if (nodeId.HasValue)
            {
                claimType = PermissionType.GetPermissionNameForSingleNode(nodeId.Value);
            }
            else
            {
                claimType = PermissionType.GetPermissionNameForCroosNode();
            }

            _logger.LogDebug($"claimType {claimType}");

            var allClaim = user.UserClaims.Select(i => new { i.ClaimType, i.ClaimValue }).ToList();

            foreach (var claim in allClaim)
            {
                if (!claim.ClaimType.Equals(claimType, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                var found = false;
                foreach (var nodePermission in nodePermissions)
                {
                    if (claim.ClaimType.Equals(claimType, StringComparison.InvariantCultureIgnoreCase) &&
                        claim.ClaimValue.Equals(nodePermission.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        _logger.LogDebug(
                            $"[check for remove old] claim object exist type: {claim.ClaimType} value: {claim.ClaimValue}");
                        break;
                    }
                }

                if (!found)
                {
                    _logger.LogDebug(
                        $"[check for remove old] claim not exist type: {claim.ClaimType} value: {claim.ClaimValue}");
                    await _userManager.RemoveClaimAsync(user,
                        new Claim(claim.ClaimType, claim.ClaimValue, ClaimValueTypes.String));
                }
            }

            foreach (var nodePermission in nodePermissions)
            {
                var claimValue = nodePermission.ToString();
                if (allClaim.Any(i => i.ClaimType.Equals(claimType, StringComparison.InvariantCultureIgnoreCase) &&
                                      i.ClaimValue.Equals(claimValue, StringComparison.InvariantCultureIgnoreCase))
                )
                {
                    continue;
                }

                _logger.LogDebug($"try to add claim type: {claimType} value: {claimValue}");
                var addClaim = new Claim(claimType, claimValue, ClaimValueTypes.String);
                await _userManager.AddClaimAsync(user, addClaim);
            }

            _logger.LogDebug("END setNodePermission");
            return true;
        }

        public async Task<int> GetLastErrorLogins(int userId)
        {
            var lastLoginsAudit = await _userAuditRepository.FindAsync(new GetLastErrorLoginsSpecification(userId, _authConfig.TryLoginTime));
            int errorLogin = 0;
            foreach (var itemAudit in lastLoginsAudit)
            {
                if (itemAudit.AuditEvent == UserAuditEventType.FailedLogin)
                {
                    errorLogin++;
                }
                else
                {
                    break;
                }
            }

            return errorLogin;
        }
    }
}