import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {
    getUserConfigGetUrl,
    getUserConfigPostUrl,
    getUserConfigPutUrl,
    getUserDeleteUrl,
    getUsersConfigGetUrl
} from "../../serverApi/urls";

export const USERS_CONFIG_USERS_FETCH = "usersConfig/fetchUsers";
export const USERS_CONFIG_USERS_CLEAR = "usersConfig/clearUsers";
export const USERS_CONFIG_USER_FETCH = "usersConfig/fetchUser";
export const USERS_CONFIG_USER_CLEAR = "usersConfig/clearUser";
export const USERS_CONFIG_USER_DELETE = "usersConfig/deleteUser";
export const USERS_CONFIG_USER_CREATE = "usersConfig/createUser";
export const USERS_CONFIG_USER_EDIT = "usersConfig/editUser";
export const USER_CONFIG_FORM_SHOW = "usersConfig/showForm";
export const USER_CONFIG_FORM_HIDE = "usersConfig/hideForm";

export const fetchUsersConfigUsers = () => initRequest(
    USERS_CONFIG_USERS_FETCH,
    getUsersConfigGetUrl(),
    undefined,
    undefined,
    t => ({
        onStart: t("scenes.usersConfig.fetchUsers.start")
    })
);

export const clearUsersConfigUsers = () => ({
    type: USERS_CONFIG_USERS_CLEAR
});

export const fetchUsersConfigUser = (userId: number) => initRequest(
    USERS_CONFIG_USER_FETCH,
    getUserConfigGetUrl(userId),
    undefined,
    undefined,
    t => ({
        onStart: t("scenes.usersConfig.fetchUser.start")
    })
);

export const clearUsersConfigUser = () => ({
    type: USERS_CONFIG_USER_CLEAR
});

export const deleteUsersConfigUser = (userId: number) => initRequest(
    USERS_CONFIG_USER_DELETE,
    getUserDeleteUrl(userId),
    RequestMethod.DELETE,
    undefined,
    t => ({
        onStart: t("scenes.usersConfig.deleteUser.start")
    }),
    {
        userId
    }
);

export const sendUsersConfigUserCreate = (user: any, isAnonymous: boolean) => initRequest(
    USERS_CONFIG_USER_CREATE,
    getUserConfigPostUrl(),
    RequestMethod.POST,
    user,
    t => ({
        onStart: t("scenes.usersConfig.sendUserCreate.start")
    }),
    {
        isAnonymous
    }
);

export const sendUsersConfigUserEdit = (user: any) => initRequest(
    USERS_CONFIG_USER_EDIT,
    getUserConfigPutUrl(user.userId),
    RequestMethod.PUT,
    user,
    t => ({
        onStart: t("scenes.usersConfig.sendUserEdit.start")
    })
);

export const showUserConfigForm = (userId: number) => ({
    type: USER_CONFIG_FORM_SHOW,
    payload: {
        userId
    }
});

export const hideUserConfigForm = () => ({
    type: USER_CONFIG_FORM_HIDE
});