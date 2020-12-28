import {Reducer} from "redux";
import {REQUEST_ERROR, REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {
    USER_CHANGE_PASSWORD_FORM_HIDE,
    USER_CHANGE_PASSWORD_FORM_SHOW,
    USER_CLEAR,
    USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_CLEAR,
    USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_SET,
    USER_FETCH,
    USER_LOGIN_FORM_HIDE,
    USER_LOGIN_FORM_SHOW, USER_PASSWORD_CHANGE, USER_PASSWORD_SET,
    USER_REFRESH, USER_REGISTER_MODAL_HIDE, USER_REGISTER_MODAL_SHOW,
    USER_SET_PASSWORD_FORM_HIDE,
    USER_SET_PASSWORD_FORM_SHOW
} from './userActions';
import {INIT} from '../rootActions';
import {PERSISTENCE_ACTION_KEY} from '../../middlewares/persistence/middleware';
import {USER_ERRORS_INVALID_LOGIN} from '../../constants/getUserErrorsTranslations';
import {USERS_CONFIG_USER_CREATE} from '../usersConfig/usersConfigActions';

export enum UserRoles {
    Administrator = "Administrator",
    User = "User"
}

export enum UserPermissions {
    ManageCache = "ManageCache",
    ManageTemplate = "ManageTemplate",
    ManageConfig = "ManageConfig",
    ManageView = "ManageView"
}

export type UserState = {
    isAuthenticated: boolean,
    email: string | null,
    roles: UserRoles[],
    permissions: string[],
    token: string | null, // auth token
    refreshTokenExpiration: string | null, // refresh token expiration time (utc)
    tokenTTL: number | null // auth token expiration ttl (minutes)
    tokenReceivedTime: number | null // auth token received time (ms from unix epoch)
    isLoginDialogOpen: boolean,
    delayedRequest: object | null,
    message: string | null,
    isSetPasswordDialogOpen: boolean,
    isChangePasswordDialogOpen: boolean,
    isRegisterDialogOpen: boolean
};

const initialState = {
    isAuthenticated: false,
    email: null,
    roles: [],
    permissions: [],
    token: null,
    refreshTokenExpiration: null,
    tokenTTL: null,
    tokenReceivedTime: null,
    isLoginDialogOpen: false,
    delayedRequest: null,
    message: null,
    isSetPasswordDialogOpen: false,
    isChangePasswordDialogOpen: false,
    isRegisterDialogOpen: false
};

const userReducer: Reducer<UserState> = (
    state = initialState,
    action
) => {
    switch (action.type) {
        case INIT:
            return action.payload[PERSISTENCE_ACTION_KEY].user
                ? ({
                    ...initialState,
                    ...action.payload[PERSISTENCE_ACTION_KEY].user,
                    isLoginDialogOpen: false
                })
                : initialState;
        case USER_LOGIN_FORM_SHOW:
            return {
                ...state,
                isLoginDialogOpen: true,
                message: action.payload?.alert
            };
        case USER_LOGIN_FORM_HIDE:
            return {
                ...state,
                isLoginDialogOpen: false,
                message: undefined
            };
        case USER_SET_PASSWORD_FORM_SHOW:
            return {
                ...state,
                isLoginDialogOpen: false,
                isSetPasswordDialogOpen: true,
                setPasswordToken: action.payload.token
            };
        case USER_SET_PASSWORD_FORM_HIDE:
            return {
                ...state,
                isSetPasswordDialogOpen: false,
                message: undefined
            };
        case USER_CHANGE_PASSWORD_FORM_SHOW:
            return {
                ...state,
                isChangePasswordDialogOpen: true
            };
        case USER_CHANGE_PASSWORD_FORM_HIDE:
            return {
                ...state,
                isChangePasswordDialogOpen: false
            };
        case USER_REGISTER_MODAL_SHOW:
            return {
                ...state,
                isRegisterDialogOpen: true
            }
        case USER_REGISTER_MODAL_HIDE:
            return {
                ...state,
                isRegisterDialogOpen: false
            }
        case USER_CLEAR:
            return initialState;
        case REQUEST_SUCCESS: {
            switch (action.payload.label) {
                case USER_FETCH: {
                    const response = action.payload.response;
                    return {
                        ...response,
                        tokenReceivedTime: new Date().getTime()
                    };
                }
                case USER_REFRESH:
                    const response = action.payload.response;
                    return {
                        ...response,
                        tokenReceivedTime: new Date().getTime()
                    };
                case USER_PASSWORD_SET:
                    return {
                        ...state,
                        isSetPasswordDialogOpen: false
                    };
                case USER_PASSWORD_CHANGE:
                    return {
                        ...state,
                        isChangePasswordDialogOpen: false
                    };
                case USERS_CONFIG_USER_CREATE:
                    if (!action.payload.response.haveError && action.payload.extra?.isAnonymous) {
                        return {
                            ...state,
                            isRegisterDialogOpen: false,

                        }
                    } else {
                        return state;
                    }
                default:
                    return state;
            }
        }
        case USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_SET:
            return {
                ...state,
                delayedRequest: action.payload.request
            };
        case USER_DELAYED_REQUEST_WHEN_TOKEN_EXPIRED_CLEAR:
            return {
                ...state,
                delayedRequest: null
            };
        case REQUEST_ERROR: {
            switch (action.payload.label) {
                case USER_FETCH: {
                    if (action.payload.response.status === 401) {
                        return {
                            ...state,
                            message: USER_ERRORS_INVALID_LOGIN
                        };
                    }
                    return state;
                }
                default:
                    return state;
            }
        }
        default:
            return state;
    }
};

export default userReducer;