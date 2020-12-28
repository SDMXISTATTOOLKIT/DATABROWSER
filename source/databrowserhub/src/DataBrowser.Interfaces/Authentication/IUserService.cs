using System.Collections.Generic;
using System.Threading.Tasks;
using DataBrowser.Interfaces.Constants;
using DataBrowser.Interfaces.Dto.Users;

namespace DataBrowser.Interfaces.Authentication
{
    public interface IUserService
    {
        Task<UserRegisterResult> RegisterAsync(UserRegisterDto userRegister);
        Task<UserAuthenticatedResult> GetTokenAsync(TokenRequestDto tokenRequest);
        Task<UserAuthenticatedResult> RefreshTokenAsync(string token);
        Task<bool> AddSingleNodePermissionAsync(int userId, int nodeId, PermissionType.NodePermission nodePermission);

        Task<bool> RemoveSingleNodePermissionAsync(int userId, int nodeId,
            PermissionType.NodePermission nodePermission);

        Task<bool> SetSingleNodePermissionAsync(int userId, int nodeId,
            List<PermissionType.NodePermission> nodePermissions);

        Task<bool> AddCrossNodePermissionAsync(int userId, PermissionType.NodePermission nodePermission);
        Task<bool> RemoveCrossNodePermissionAsync(int userId, PermissionType.NodePermission nodePermission);
        Task<bool> SetCrossNodePermissionAsync(int userId, List<PermissionType.NodePermission> nodePermissions);

        Task<bool> RemoveUserRefreshToken(int userId);
        Task<bool> RemoveAllNodePermissionAsync(int nodeId);
        Task<bool> SetRoles(int userId, List<UserAndGroup.Roles> roles);
        Task<bool> DisableUser(int userId);
        Task<UserDeleteDto> DeleteUser(int userId);
        Task<List<UserList>> GetUsers();
        Task<UserList> GetUser(int userId);
        Task<List<UserList>> GetUsersWithSingleNodePermission(int nodeId);
        Task<bool> UpdateUser(int userId, UserUpdateDto updateDataDto);
        Task<string> GetUsername(int userId);
        Task<bool> RecoveryPassword(string username);
        Task<UserRegisterResult> ResetPassword(string username, string resetToken, string newPassword);
        Task<UserRegisterResult> ChangePassword(int userId, string currentPassword, string newPassword);
        Task<int> GetLastErrorLogins(int userId);

    }
}