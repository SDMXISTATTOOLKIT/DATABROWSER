import {REQUEST_ERROR, REQUEST_INIT, REQUEST_SUCCESS} from "../request/requestActions";
import {
  errorFetchDashboardDatasetAsyncHandler,
  initFetchDashboardDatasetAsyncHandler,
  successFetchDashboardDatasetAsyncHandler
} from "./actions";
import {DASHBOARDS_DATASET_FETCH} from "../../state/dashboard/dashboardActions";

const fetchDashboardDatasetAsyncHandlerMiddlewareFactory = t => ({getState, dispatch}) => next => action => {

  const result = next(action);

  if (action?.payload?.label === DASHBOARDS_DATASET_FETCH) {

    const {
      dashboardId,
      requestIds,
      worker
    } = action.payload.extra;

    if (action.type === REQUEST_INIT) {
      dispatch(initFetchDashboardDatasetAsyncHandler(dashboardId));

    } else if (action.type === REQUEST_SUCCESS) {

      const state = getState();

      worker.onmessage = event => {
        const {
          dashboardId,
          dashboardJsonStats,
          dashboardLayouts,
          dashboardFilterTrees
        } = event.data;
        dispatch(successFetchDashboardDatasetAsyncHandler(dashboardId, dashboardJsonStats, dashboardLayouts, dashboardFilterTrees));
      };
      worker.onerror = () => {
        dispatch(errorFetchDashboardDatasetAsyncHandler(dashboardId, requestIds));
        window.error.show(t("middlewares.fetchDatasetAsyncHandlerMiddlewareFactory.feedback.layoutHandlingError"));
      };
      worker.postMessage({
        dashboardId,
        requestIds,
        response: action.payload.response,
        dashboard: state.dashboard.dashboards[dashboardId]
      });

    } else if (action.type === REQUEST_ERROR) {
      dispatch(errorFetchDashboardDatasetAsyncHandler(dashboardId, requestIds));
    }
  }

  return result;

};

export default fetchDashboardDatasetAsyncHandlerMiddlewareFactory;
