import {errorRequest, REQUEST_INIT, RequestActionPayload, startRequest, successRequest} from './requestActions';
import axios, {AxiosResponse} from 'axios';
import {Middleware} from 'redux';
import {PayloadAction} from "@reduxjs/toolkit";

const requestMiddlewareFactory = (
  options: {
    onSuccess?: (requestAction: object, data?: any, extra?: any) => void,
    onManagedServerError?: (requestAction: RequestActionPayload, statusCode: number, managedServerError: any) => void,
    onGenericError?: (requestAction: RequestActionPayload, statusCode?: number) => void
  }
): Middleware =>
  ({dispatch, getState}) => next => action => {

    const handleSuccess = (requestAction: RequestActionPayload, response: AxiosResponse) => {

      if (options.onSuccess) {
        options.onSuccess(requestAction, response.data, {
          responseHeaders: response.headers
        });
      }

      dispatch(successRequest(requestAction, response.data, {
        responseHeaders: response.headers,
        status: response.status
      }));
    };

    const handleManagedServerError = (requestAction: RequestActionPayload, response: AxiosResponse) => {

      if (options.onManagedServerError && (!requestAction?.getHideErrorMessage || !requestAction.getHideErrorMessage(response?.status))) {
        options.onManagedServerError(requestAction, response.status, response.data);
      }

      dispatch(errorRequest(requestAction, response.status, response.data, {responseHeaders: response.headers}));
    };

    const handleGenericError = (requestAction: RequestActionPayload, response?: AxiosResponse) => {

      if (options.onGenericError && (!requestAction?.getHideErrorMessage || !requestAction.getHideErrorMessage(response?.status))) {
        options.onGenericError(requestAction, response?.status);
      }

      dispatch(errorRequest(requestAction, response?.status, response?.data, {responseHeaders: response?.headers}));
    };

    const handleError = (requestAction: RequestActionPayload, response?: AxiosResponse) => {

      if (response?.data) {

        if (response.data.usedBy !== undefined) {
          handleManagedServerError(requestAction, response);
        } else {
          handleGenericError(requestAction, response);
        }
      } else {
        handleGenericError(requestAction, response);
      }
    };

    const result = next(action);

    if (action.type !== REQUEST_INIT) {
      return result;
    }

    const requestAction: PayloadAction<RequestActionPayload> = action;

    dispatch(startRequest(requestAction.payload));

    let {
      method,
      url,
      data,
      baseURL,
      contentType,
      doNotTransformResponse
    } = requestAction.payload;

    const token = getState().user.token;

    axios.request({
      method,
      url: ((baseURL && baseURL.length > 0) ? baseURL : getState().config.baseURL) + url,
      data,
      headers: {
        "UserLang": getState().app.language,
        "Authorization": token ? `bearer ${token}` : undefined,
        "Content-Type": contentType || undefined
      },
      withCredentials: true,
      transformResponse: doNotTransformResponse ? res => res : undefined
    }).then((response: AxiosResponse) => {
      if (response.status.toString()[0] === "2") {
        handleSuccess(requestAction.payload, response);
      } else {
        handleError(requestAction.payload, response);
      }
    }).catch(error => {
      console.log(error);
      handleError(requestAction.payload, error.response);
    });

    return result;
  };

export default requestMiddlewareFactory;
