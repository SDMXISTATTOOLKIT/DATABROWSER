import {Reducer} from "redux";
import {
    REQUEST_ERROR,
    REQUEST_START,
    REQUEST_SUCCESS
} from "../../middlewares/request/requestActions";
import {
    USER_CONFIG_FORM_HIDE,
    USER_CONFIG_FORM_SHOW,
    USERS_CONFIG_USER_CLEAR,
    USERS_CONFIG_USER_CREATE,
    USERS_CONFIG_USER_EDIT,
    USERS_CONFIG_USER_FETCH,
    USERS_CONFIG_USERS_CLEAR,
    USERS_CONFIG_USERS_FETCH
} from "./usersConfigActions";
import {
    USER_PASSWORD_CHANGE,
    USER_PASSWORD_SET,
    USER_REGISTER_MODAL_HIDE
} from '../user/userActions';

export type User = {};

export type UsersConfigState = {
    users: User[] | null,
    user: User | null,
    userErrors: string[] | null,
    userId: number | null,
    setPasswordToken: string | null
};

const usersConfigReducer: Reducer<UsersConfigState> = (
    state = {
        users: null,
        user: null,
        userErrors: null,
        userId: null,
        setPasswordToken: null
    },
    action
) => {
    switch (action.type) {
        case USER_CONFIG_FORM_SHOW:
            return {
                ...state,
                userId: action.payload.userId
            };
        case USER_CONFIG_FORM_HIDE:
            return {
                ...state,
                user: null,
                userId: null,
                userErrors: null
            };
        case USERS_CONFIG_USERS_CLEAR: {
            return {
                ...state,
                users: null
            };
        }
        case USERS_CONFIG_USER_CLEAR: {
            return {
                ...state,
                user: null
            };
        }
        case REQUEST_START: {
            switch (action.payload.label) {
                case USERS_CONFIG_USER_EDIT: {
                    return {
                        ...state,
                        user: null
                    };
                }
                default:
                    return state;
            }
        }
        case USER_REGISTER_MODAL_HIDE:
            return {
                ...state,
                userErrors: null
            };
        case REQUEST_SUCCESS: {
            switch (action.payload.label) {
                case USERS_CONFIG_USERS_FETCH: {
                    return {
                        ...state,
                        users: action.payload.response
                    };
                }
                case USERS_CONFIG_USER_FETCH: {
                    return {
                        ...state,
                        user: action.payload.response
                    }
                }
                case USERS_CONFIG_USER_CREATE:
                case USERS_CONFIG_USER_EDIT:
                    if (action.payload.response.haveError) {
                        return {
                            ...state,
                            userErrors: action.payload.response.errors
                        };
                    } else {
                        return {
                            ...state,
                            user: null,
                            userErrors: null,
                            userId: null
                        };
                    }
                case USER_PASSWORD_SET:
                case USER_PASSWORD_CHANGE:
                    return {
                        ...state,
                        userErrors: null
                    }
                default:
                    return state;
            }
        }
        case REQUEST_ERROR:
            switch (action.payload.label) {
                case USER_PASSWORD_SET:
                case USER_PASSWORD_CHANGE:
                    if (action.payload.response.haveError) {
                        return {
                            ...state,
                            userErrors: action.payload.response.errors
                        };
                    } else {
                        return state;
                    }
                default:
                    return state;
            }
        default:
            return state;
    }
};

export default usersConfigReducer;