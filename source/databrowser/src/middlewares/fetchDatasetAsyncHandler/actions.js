export const FETCH_DATASET_ASYNC_HANDLER_INIT = "fetch/dataset/async/handler/init"
export const FETCH_DATASET_ASYNC_HANDLER_SUCCESS = "fetch/dataset/async/handler/success"
export const FETCH_DATASET_ASYNC_HANDLER_ERROR = "fetch/dataset/async/handler/error"

export const initFetchDatasetAsyncHandler = () => ({
  type: FETCH_DATASET_ASYNC_HANDLER_INIT
});

export const successFetchDatasetAsyncHandler = (
  response,
  statusCode,
  responseHeaders,
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
  worker
) => ({
  type: FETCH_DATASET_ASYNC_HANDLER_SUCCESS,
  payload: {
    response,
    statusCode,
    responseHeaders,
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
    worker
  }
});

export const errorFetchDatasetAsyncHandler = statusCode => ({
  type: FETCH_DATASET_ASYNC_HANDLER_ERROR,
  payload: {
    statusCode
  }
});