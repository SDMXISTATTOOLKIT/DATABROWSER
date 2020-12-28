import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {getDashboardsUrl, getDatasetDownloadUrl, getDatasetUrl, getGeometryUrl} from "../../serverApi/urls";
import {getCriteriaArrayFromObject} from "../../utils/criteria";

export const DASHBOARDS_DASHBOARD_FETCH = "dashboards/dashboard/fetch";
export const DASHBOARDS_DASHBOARDS_CLEAR = "dashboards/dashboards/clear";
export const DASHBOARDS_UPDATED_DASHBOARD_ID_RESET = "dashboards/updated/dashboard/id/reset";
export const DASHBOARDS_DATASET_FETCH_ENABLE_STATUS_SET = "dashboards/dataset/fetch/enableStatus/set";
export const DASHBOARDS_DATASET_FETCH = "dashboards/dataset/fetch";
export const DASHBOARDS_DATASET_FILTER_SET = "dashboards/dataset/filter/set";
export const DASHBOARDS_MAP_GEOMETRIES_FETCH = "dashboards/map/geometries/fetch";
export const DASHBOARDS_DATASET_DOWNLOAD_SUBMIT = "dashboards/dataset/download/submit";

export const fetchDashboardsDashboard = (dashboardId: number) => initRequest(
  DASHBOARDS_DASHBOARD_FETCH,
  getDashboardsUrl(dashboardId),
  RequestMethod.GET,
  undefined,
  t => ({
    onStart: t("scenes.dashboard.actions.fetchingDashboard")
  }),
  {
    dashboardId: dashboardId
  }
);

export const clearDashboardsDashboards = (updatedDashboardId?: number) => ({
  type: DASHBOARDS_DASHBOARDS_CLEAR,
  payload: {
    updatedDashboardId: updatedDashboardId
  }
});

export const resetDashboardsUpdatedDashboardId = () => ({
  type: DASHBOARDS_UPDATED_DASHBOARD_ID_RESET
});

export const setDashboardsDatasetFetchEnableStatus = (enabled: boolean) => ({
  type: DASHBOARDS_DATASET_FETCH_ENABLE_STATUS_SET,
  payload: {
    disabled: !enabled
  }
});

export const fetchDashboardsDataset = (dashboardId: number, nodeId: number, datasetId: string, criteria: any, requestIds: string[], worker: any) => initRequest(
  DASHBOARDS_DATASET_FETCH,
  getDatasetUrl(nodeId, datasetId),
  RequestMethod.POST,
  getCriteriaArrayFromObject(criteria),
  t => ({
    onStart: t("scenes.dashboard.actions.fetchingDataset")
  }),
  {
    dashboardId: dashboardId,
    requestIds: requestIds,
    worker: worker
  },
  "",
  () => true,
  true
);

export const setDashboardsDatasetFilter = (dashboardId: number, viewIdx: string, dimension: string, value: string) => ({
  type: DASHBOARDS_DATASET_FILTER_SET,
  payload: {
    dashboardId,
    viewIdx,
    dimension,
    value
  }
});

export const fetchDashboardsMapGeometries = (dashboardId: number, viewIdx: string, idList: string[], t: any) => initRequest(
  DASHBOARDS_MAP_GEOMETRIES_FETCH,
  getGeometryUrl(),
  RequestMethod.POST,
  idList,
  t => ({
    onStart: t(`scenes.dashboard.actions.fetchingGeometries`)
  }),
  {
    dashboardId: dashboardId,
    viewIdx: viewIdx,
    t: t
  },
  "",
  () => true,
  true
);

export const submitDashboardDatasetDownload = (nodeId: number, datasetId: string, viewTitle: string, criteria: any) => initRequest(
  DASHBOARDS_DATASET_DOWNLOAD_SUBMIT,
  getDatasetDownloadUrl(nodeId, datasetId, "csv"),
  RequestMethod.POST,
  getCriteriaArrayFromObject(criteria),
  t => ({
    onStart: t("scenes.dataset.actions.downloadingDataset")
  }),
  {
    fileSave: {
      name: `${viewTitle}.csv`,
      type: "text/plain;charset=utf-8"
    }
  },
  "",
  (statusCode: number) => statusCode === 406
);