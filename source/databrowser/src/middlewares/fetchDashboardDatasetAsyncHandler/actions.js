export const FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_INIT = "fetch/dashboard/dataset/async/handler/init"
export const FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_SUCCESS = "fetch/dashboard/dataset/async/handler/success"
export const FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_ERROR = "fetch/dashboard/dataset/async/handler/error"

export const initFetchDashboardDatasetAsyncHandler = dashboardId => ({
  type: FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_INIT,
  payload: {
    dashboardId
  }
});

export const successFetchDashboardDatasetAsyncHandler = (dashboardId, dashboardJsonStats, dashboardLayouts, dashboardFilterTrees) => ({
  type: FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_SUCCESS,
  payload: {
    dashboardId,
    dashboardJsonStats,
    dashboardLayouts,
    dashboardFilterTrees
  }
});

export const errorFetchDashboardDatasetAsyncHandler = (dashboardId, requestIds) => ({
  type: FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_ERROR,
  payload: {
    dashboardId,
    requestIds
  }
});