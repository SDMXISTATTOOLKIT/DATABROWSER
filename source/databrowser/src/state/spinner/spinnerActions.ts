export const SPINNER_MESSAGE_ADD = "spinner/addMessage";
export const SPINNER_MESSAGE_REMOVE = "spinner/removeMessage";
export const SPINNER_TIMEOUT_SET = "spinner/setTimeout";
export const SPINNER_TIMEOUT_CLEAR = "spinner/clearTimeout";
export const SPINNER_FLUSH = "spinner/flush";

export const addSpinnerMessage = (uid: any, message: any) => ({
  type: SPINNER_MESSAGE_ADD,
  payload: {
    uid,
    message
  }
});

export const markSpinnerMessage = (uid: any, isError?: boolean) => ({
  type: SPINNER_MESSAGE_REMOVE,
  payload: {
    uid,
    isError
  }
});

export const setSpinnerTimeout = (timeout: any) => ({
  type: SPINNER_TIMEOUT_SET,
  payload: {
    timeout
  }
});

export const clearSpinnerTimeout = () => ({
  type: SPINNER_TIMEOUT_CLEAR
});

export const flushSpinner = () => ({
  type: SPINNER_FLUSH
});


