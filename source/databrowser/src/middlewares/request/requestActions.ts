import {v4 as uuidv4} from 'uuid';

export const REQUEST_INIT = "request/init";
export const REQUEST_START = "request/start";
export const REQUEST_ERROR = "request/error";
export const REQUEST_SUCCESS = "request/success";

export enum RequestMethod {
  GET = "GET",
  POST = "POST",
  PUT = "PUT",
  DELETE = "DELETE"
}

type RequestMessages = (t: (str: string, options?: any) => string) => {
  onStart?: string,
  onSuccess?: string,
  onError?: string
};

export interface RequestActionPayload {
  label: string,
  url: string,
  method: RequestMethod,
  data?: any,
  messages?: RequestMessages,
  extra?: any,
  baseURL?: string,
  getHideErrorMessage?: any,
  hideSpinner?: boolean,
  uuid: string,
  contentType?: string,
  doNotTransformResponse?: boolean
}

export const initRequest = (
  label: string,
  url: string,
  method: RequestMethod = RequestMethod.GET,
  data?: any,
  messages?: RequestMessages,
  extra?: any,
  baseURL?: string,
  getHideErrorMessage?: any,
  hideSpinner?: boolean,
  contentType?: string,
  doNotTransformResponse?: boolean
) => ({
  type: REQUEST_INIT,
  payload: {
    uuid: uuidv4(),
    label,
    url,
    method,
    data,
    messages,
    extra,
    baseURL,
    getHideErrorMessage,
    hideSpinner,
    contentType,
    doNotTransformResponse
  }
});

export const startRequest = (initRequestPayload: RequestActionPayload) => ({
  type: REQUEST_START,
  payload: initRequestPayload
});

export const successRequest = (requestActionPayload: RequestActionPayload, response?: any, extra?: any) => ({
  type: REQUEST_SUCCESS,
  payload: {
    ...requestActionPayload,
    response,
    extra: {
      ...extra,
      ...requestActionPayload.extra
    }
  }
});

export type ManagedServerError = {
  errorCode: number,
  stackTrace?: string
};

export const errorRequest = (
  requestActionPayload: RequestActionPayload,
  statusCode?: number,
  response?: any,
  extra?: any
) => ({
  type: REQUEST_ERROR,
  payload: {
    ...requestActionPayload,
    statusCode,
    response,
    extra: {
      ...extra,
      ...requestActionPayload.extra
    }
  }
});
