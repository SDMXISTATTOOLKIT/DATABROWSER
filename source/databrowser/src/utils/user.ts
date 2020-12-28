import {UserPermissions, UserRoles, UserState} from '../state/user/userReducer';

type UserChecker = (user: UserState, nodeId?: number) => boolean;
type UserNodeChecker = (user: UserState, nodeId: number) => boolean;

const getPermissionStr = (permission: UserPermissions, nodeId: number) => `${permission}_SingleNode_${nodeId}`;

export const canDisplayAppSettingsForm: UserChecker = user =>
    user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canCreateNode: UserChecker = user =>
    user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canSortNodes: UserChecker = user =>
    user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canDisplayNodesSettingsForm: UserChecker = user =>
    user && user.isAuthenticated && (
        user.roles.includes(UserRoles.Administrator) ||
        !!user.permissions.find(p => p.startsWith(`${UserPermissions.ManageCache}_SingleNode`)) ||
        !!user.permissions.find(p => p.startsWith(`${UserPermissions.ManageTemplate}_SingleNode`)) ||
        !!user.permissions.find(p => p.startsWith(`${UserPermissions.ManageConfig}_SingleNode`))
    );

export const canDisplayUsersSettingsForm: UserChecker = user =>
    user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canDisplayCacheSettingForm: UserNodeChecker = (user, nodeId) =>
    user && user.isAuthenticated && (
        user.roles.includes(UserRoles.Administrator) ||
        user.permissions.includes(getPermissionStr(UserPermissions.ManageCache, nodeId))
    );

export const canDisplayTemplatesSettingsForm: UserNodeChecker = (user, nodeId) =>
    user && user.isAuthenticated && (
        user.roles.includes(UserRoles.Administrator) ||
        user.permissions.includes(getPermissionStr(UserPermissions.ManageTemplate, nodeId))
    );

export const canSaveTemplate: UserNodeChecker = (user, nodeId) =>
    user && user.isAuthenticated && (
        user.roles.includes(UserRoles.Administrator) ||
        user.permissions.includes(getPermissionStr(UserPermissions.ManageTemplate, nodeId))
    );

export const canEditNode: UserNodeChecker = (user, nodeId) =>
    user && user.isAuthenticated && (
        user.roles.includes(UserRoles.Administrator) ||
        user.permissions.includes(getPermissionStr(UserPermissions.ManageConfig, nodeId))
    );

export const canDisplayUserViews: UserChecker = user =>
    user && user.isAuthenticated;

export const canShare: UserChecker = user =>
    user && user.isAuthenticated;

export const canSaveAsView: UserChecker = user =>
    user && user.isAuthenticated;

export const canClearServerCache: UserChecker = user =>
    user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canGetQueryLog: UserChecker = user =>
    user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canDisplayPermissionsSettingsForm: UserChecker = user =>
    user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canManagePersonalDashboards: UserChecker = user =>
    user && user.isAuthenticated && (
        user.roles.includes(UserRoles.Administrator) ||
        !!user.permissions.find(p => p.startsWith(`${UserPermissions.ManageConfig}_SingleNode`))
    );

export const canManageAppDashboards: UserChecker = user =>
    user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canManageNodeDashboards: UserNodeChecker = (user, nodeId) =>
  user && user.isAuthenticated && (
    user.roles.includes(UserRoles.Administrator) ||
    user.permissions.includes(getPermissionStr(UserPermissions.ManageConfig, nodeId))
  );

export const canDeleteNodes: UserChecker = user =>
  user && user.isAuthenticated && user.roles.includes(UserRoles.Administrator);

export const canViewTimesLog: UserNodeChecker = (user, nodeId) =>
  user && user.isAuthenticated && (
    user.roles.includes(UserRoles.Administrator) ||
    user.permissions.includes(getPermissionStr(UserPermissions.ManageConfig, nodeId))
  );

export const canViewTemplateOrAnnotationIcon: UserNodeChecker = (user, nodeId) =>
  user && user.isAuthenticated && (
    user.roles.includes(UserRoles.Administrator) ||
    user.permissions.includes(getPermissionStr(UserPermissions.ManageTemplate, nodeId))
  );