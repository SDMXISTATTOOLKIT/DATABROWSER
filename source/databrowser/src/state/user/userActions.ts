import {initRequest, RequestMethod} from '../../middlewares/request/requestActions';
import {
    getRefreshTokenUrl,
    getTokenUrl,
    getUserChangePasswordUrl,
    getUserResetPasswordUrl,
    getUserSendResetPasswordMailUrl
} from '../../serverApi/urls';

export const USER_FETCH = 'USER_FETCH';
export const USER_REFRESH = 'USER_REFRESH';
export const USER_CLEAR = 'USER_CLEAR';
export const USER_LOGIN_FORM_SHOW = 'USER_LOGIN_FORM_SHOW';
export const USER_LOGIN_FORM_HIDE = 'USER_LOGIN_FORM_HIDE';
export const USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_SET = 'USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_SET';
export const USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_CLEAR = 'USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_CLEAR';
export const USER_SET_PASSWORD_FORM_SHOW = 'USER_SET_PASSWORD_FORM_SHOW';
export const USER_SET_PASSWORD_FORM_HIDE = 'USER_SET_PASSWORD_FORM_HIDE';
export const USER_RESET_PASSWORD_EMAIL_SEND = 'USER_RESET_PASSWORD_EMAIL_SEND';
export const USER_PASSWORD_CHANGE = 'USER_PASSWORD_CHANGE';
export const USER_PASSWORD_SET = 'USER_PASSWORD_SET';
export const USER_CHANGE_PASSWORD_FORM_SHOW = 'USER_CHANGE_PASSWORD_FORM_SHOW';
export const USER_CHANGE_PASSWORD_FORM_HIDE = 'USER_CHANGE_PASSWORD_FORM_HIDE';
export const USER_REGISTER_MODAL_SHOW = 'USER_REGISTER_MODAL_SHOW';
export const USER_REGISTER_MODAL_HIDE = 'USER_REGISTER_MODAL_HIDE';

export const fetchUser = (email: string, password: string) => initRequest(
    USER_FETCH,
    getTokenUrl(),
    RequestMethod.POST,
    {
        Email: email,
        Password: btoa(password)
    },
    t => ({
        onStart: t("components.userSelect.messages.fetchUser.start")
    })
);

export const refreshUser = () => initRequest(
    USER_REFRESH,
    getRefreshTokenUrl(),
    RequestMethod.POST,
    null,
    t => ({
        onStart: t("middlewares.user.messages.refreshUser.start")
    })
);

export const clearUser = () => ({
    type: USER_CLEAR
});

export const showUserLoginForm = (alert: string) => ({
    type: USER_LOGIN_FORM_SHOW,
    payload: {
        alert
    }
});

export const hideUserLoginForm = () => ({
    type: USER_LOGIN_FORM_HIDE
});

export const setUserDelayedRequestWhenTokenExpired = (request: object) => ({
    type: USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_SET,
    payload: {
        request
    }
});

export const clearUserDelayedRequestWhenTokenExpired = () => ({
    type: USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_CLEAR
});

export const hideUserSetPasswordForm = () => ({
    type: USER_SET_PASSWORD_FORM_HIDE
});

export const sendUserResetPasswordEmail = (email: string) => initRequest(
    USER_RESET_PASSWORD_EMAIL_SEND,
    getUserSendResetPasswordMailUrl(),
    RequestMethod.POST,
    email,
    t => ({
        onStart: t("components.userRecoverPassword.messages.submit.start")
    }),
    undefined,
    undefined,
    undefined,
    undefined,
    "application/json"
);

export const changeUserPassword = (oldPassword: string, newPassword: string) => initRequest(
    USER_PASSWORD_CHANGE,
    getUserChangePasswordUrl(),
    RequestMethod.POST,
    {
        oldPassword: btoa(oldPassword),
        newPassword: btoa(newPassword)
    },
    t => ({
        onStart: t("components.userChangePasswordForm.messages.submit.start")
    })
);

export const setUserPassword = (email: string, token: string, password: string) => initRequest(
    USER_PASSWORD_SET,
    getUserResetPasswordUrl(),
    RequestMethod.POST,
    {
        username: email,
        token,
        password: btoa(password)
    },
    t => ({
        onStart: t("components.userSetPasswordForm.messages.submit.start")
    })
);

export const showUserSetPasswordForm = (token: string) => ({
    type: USER_SET_PASSWORD_FORM_SHOW,
    payload: {
        token
    }
});

export const showUserChangePasswordForm = () => ({
    type: USER_CHANGE_PASSWORD_FORM_SHOW
});

export const hideUserChangePasswordForm = () => ({
    type: USER_CHANGE_PASSWORD_FORM_HIDE
});

export const showUserRegisterModal = () => ({
    type: USER_REGISTER_MODAL_SHOW
});

export const hideUserRegisterModal = () => ({
    type: USER_REGISTER_MODAL_HIDE
});