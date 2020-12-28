import {Reducer} from "redux";
import {
  SPINNER_FLUSH,
  SPINNER_MESSAGE_ADD,
  SPINNER_MESSAGE_REMOVE,
  SPINNER_TIMEOUT_CLEAR,
  SPINNER_TIMEOUT_SET
} from "./spinnerActions";

export type SpinnerState = {
  messages: any[],
  timeout: any
};

const spinnerReducer: Reducer<SpinnerState> = (
  state = {
    messages: [],
    timeout: null
  },
  action
) => {
  switch (action.type) {
    case SPINNER_MESSAGE_ADD: {
      return {
        ...state,
        messages: [...state.messages, ({uid: action.payload.uid, message: action.payload.message, removed: false})]
      };
    }
    case SPINNER_MESSAGE_REMOVE: {
      return {
        ...state,
        messages: [
          ...state.messages.map(message => message.uid !== action.payload.uid ? message : ({...message, removed: true, isError: action.payload.isError}))
        ]
      };
    }
    case SPINNER_TIMEOUT_SET: {
      return {
        ...state,
        timeout: action.payload.timeout
      };
    }
    case SPINNER_TIMEOUT_CLEAR: {
      return {
        ...state,
        timeout: null
      };
    }
    case SPINNER_FLUSH: {
      if (state.timeout) {
        clearTimeout(state.timeout);
      }
      return {
        ...state,
        timeout: null,
        messages: []
      };
    }
    default:
      return state;
  }
};

export default spinnerReducer;