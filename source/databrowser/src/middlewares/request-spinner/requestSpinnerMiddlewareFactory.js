import {REQUEST_ERROR, REQUEST_START, REQUEST_SUCCESS} from "../request/requestActions";
import {
  addSpinnerMessage,
  clearSpinnerTimeout,
  flushSpinner,
  markSpinnerMessage,
  setSpinnerTimeout, SPINNER_FLUSH,
  SPINNER_MESSAGE_ADD,
  SPINNER_MESSAGE_REMOVE
} from "../../state/spinner/spinnerActions";
import {scrollResultsToDatasetByParam} from "../../components/results/utils";

const SPINNER_TIMEOUT = 1000;

const requestSpinnerMiddlewareFactory = t => ({dispatch, getState}) => next => action => {

  if ((action.type === REQUEST_START || action.type === REQUEST_SUCCESS || action.type === REQUEST_ERROR) && action.payload.messages) {

    const messages = action.payload.messages(t);

    if (messages.onStart) {
      if (action.type === REQUEST_START && !action.payload.hideSpinner) {
        dispatch(addSpinnerMessage(action.payload.uuid, messages.onStart));
      } else if (action.type === REQUEST_SUCCESS && !action.payload.response?.haveError) {
        dispatch(markSpinnerMessage(action.payload.uuid));
      } else if (action.type === REQUEST_ERROR || (action.type === REQUEST_SUCCESS && action.payload.response?.haveError)) {
        dispatch(markSpinnerMessage(action.payload.uuid, true));
      }
    }
  }

  const result = next(action);

  const spinnerState = getState().spinner;

  if (action.type === SPINNER_MESSAGE_ADD) {
    if (spinnerState.timeout) {
      clearTimeout(spinnerState.timeout);
      dispatch(clearSpinnerTimeout());
    }
  } else if (action.type === SPINNER_MESSAGE_REMOVE && !spinnerState.timeout && spinnerState.messages.length > 0 &&
    spinnerState.messages.filter(({removed}) => !removed).length === 0
  ) {
    dispatch(setSpinnerTimeout(setTimeout(() => dispatch(flushSpinner()), SPINNER_TIMEOUT)));
  } else if (action.type === SPINNER_FLUSH) {
    scrollResultsToDatasetByParam();
  }

  return result;
};

export default requestSpinnerMiddlewareFactory;