import {REQUEST_ERROR, REQUEST_INIT, REQUEST_SUCCESS} from "../request/requestActions";
import {DATASET_FETCH} from "../../state/dataset/datasetActions";
import {errorFetchDatasetAsyncHandler, initFetchDatasetAsyncHandler, successFetchDatasetAsyncHandler} from "./actions";
import {addSpinnerMessage, markSpinnerMessage} from "../../state/spinner/spinnerActions";
import {v4 as uuidv4} from 'uuid';

const fetchDatasetAsyncHandlerMiddlewareFactory = t => ({dispatch}) => next => action => {

  const result = next(action);

  if (action?.payload?.label === DATASET_FETCH) {

    if (action.type === REQUEST_INIT) {
      dispatch(initFetchDatasetAsyncHandler());

    } else if (action.type === REQUEST_SUCCESS) {

      const spinnerUuid = uuidv4();
      dispatch(addSpinnerMessage(spinnerUuid, t("middlewares.fetchDatasetAsyncHandlerMiddlewareFactory.spinners.layoutHandling")));

      const myWorker = new Worker("./workers/fetchDatasetAsyncHandlerWorker.js");
      myWorker.onmessage = event => {
        const {
          cellCount,
          isTooBigData,
          isEmptyData,
          isPartialData,
          isResponseNotValid,
          tableLayout,
          mapLayout,
          chartLayout,
          tableFilterTree,
          mapFilterTree,
          chartFilterTree
        } = event.data;
        dispatch(successFetchDatasetAsyncHandler(
          action.payload.response,
          action.payload.extra.status,
          action.payload.extra.responseHeaders,
          cellCount,
          isTooBigData,
          isEmptyData,
          isPartialData,
          isResponseNotValid,
          tableLayout,
          tableFilterTree,
          mapLayout,
          mapFilterTree,
          chartLayout,
          chartFilterTree,
          myWorker
        ));
        dispatch(markSpinnerMessage(spinnerUuid));
      }
      myWorker.onerror = () => {
        dispatch(errorFetchDatasetAsyncHandler());
        dispatch(markSpinnerMessage(spinnerUuid, true));
        window.error.show(t("middlewares.fetchDatasetAsyncHandlerMiddlewareFactory.feedback.layoutHandlingError"));
      }
      myWorker.postMessage({
        response: action.payload.response,
        extra: action.payload.extra
      });

    } else if (action.type === REQUEST_ERROR) {
      dispatch(errorFetchDatasetAsyncHandler(action.payload.statusCode));
    }
  }

  return result;

};

export default fetchDatasetAsyncHandlerMiddlewareFactory;
