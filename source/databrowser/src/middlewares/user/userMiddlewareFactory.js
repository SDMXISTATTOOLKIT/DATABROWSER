import {
  clearUser,
  USER_PASSWORD_CHANGE,
  USER_PASSWORD_SET,
  USER_REFRESH,
  USER_RESET_PASSWORD_EMAIL_SEND
} from "../../state/user/userActions";
import {REQUEST_INIT, REQUEST_SUCCESS} from "../request/requestActions";
import {HUB_FETCH} from "../../state/hub/hubActions";
import {NODE_FETCH} from "../../state/node/nodeActions";
import {USERS_CONFIG_USER_CREATE} from "../../state/usersConfig/usersConfigActions";

const userMiddlewareFactory = t => ({dispatch, getState}) => next => action => {

  const state = getState();

  let result;

  // if a request is starting but auth token is expired
  if (
    action.type === REQUEST_INIT &&
    action.payload.label !== HUB_FETCH &&
    action.payload.label !== NODE_FETCH &&
    action.payload.label !== USER_REFRESH &&
    state.user.isAuthenticated &&
    state.user.tokenReceivedTime !== null &&
    state.user.tokenTTL !== null &&
    new Date(new Date().getTime() - state.user.tokenReceivedTime).getMinutes()
    >= state.user.tokenTTL - 1
  ) {

    /*
      result = next(refreshUser()); // block current request dispatching, and replace it with token refresh request
      dispatch(setUserDelayedRequestWhenTokenExpired(action)); // delay current (request) action
    */

    result = next(({type: "NOP"}));
    if (window.error) {
      window.error.show(t("middlewares.user.sessionExpired"));
    }
    dispatch(clearUser());

  } else {

    result = next(action); // do nothing, continue action dispatching as normal

  }

  // if sent token is expired (in some cases it can happen, even if i checked expiration in the previous code)
  if (action.payload?.extra?.responseHeaders && action.payload.extra.responseHeaders["token-expired"]) {
    /*
      dispatch(clearUser()); // init user state
      dispatch(showUserLoginForm("Authentication is expired. Please log in again.")); // show login form with alert
     */

    if (window.error) {
      window.error.show(t("middlewares.user.sessionExpired"));
    }
    dispatch(clearUser());

  }

  // if token has been refreshed and delayed request is pending
  if (action.type === REQUEST_SUCCESS && action.payload?.label === USER_REFRESH && state.user.delayedRequest) {
    dispatch(state.user.delayedRequest); // dispatch pending (request) action
  }

  if (action.type === REQUEST_SUCCESS && (
    action.payload.label === USER_PASSWORD_CHANGE ||
    action.payload.label === USER_PASSWORD_SET
  )) {
    window.error.show(t("middlewares.user.passwordChangedLoginNecessary"), false);
  } else if (action.type === REQUEST_SUCCESS && action.payload.label === USER_RESET_PASSWORD_EMAIL_SEND) {
    window.error.show(t("middlewares.user.resetPasswordEmailSent"), false);
  } else if (action.type === REQUEST_SUCCESS && action.payload.label === USERS_CONFIG_USER_CREATE && !action.payload.response.haveError && action.payload.extra?.isAnonymous) {
    window.error.show(t("middlewares.user.userRegisterSuccess"), false);
  }

  return result;
};

export default userMiddlewareFactory;