import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {
  getCreateDashboardUrl,
  getDashboardsUrl,
  getDatasetUrl,
  getDeleteDashboardUrl,
  getDeleteViewUrl,
  getGeometryUrl,
  getUpdateDashboardUrl,
  getUserDashboardsUrl,
  getViewsUrl
} from "../../serverApi/urls";
import {
  DASHBOARD_ELEM_FILTER_DIMENSION_KEY,
  DASHBOARD_ELEM_TYPE_KEY,
  DASHBOARD_ELEM_TYPE_VALUE_VIEW,
  DASHBOARD_ELEM_VALUE_KEY
} from "../../utils/dashboards";
import {getCriteriaArrayFromObject} from "../../utils/criteria";
import _ from "lodash"
import filters from "../../dummy/filters.json";

export const OTHER_CONFIG_VIEWS_FETCH = "otherConfig/fetchViews";
export const OTHER_CONFIG_VIEWS_CLEAR = "otherConfig/clearViews";
export const OTHER_CONFIG_VIEW_DELETE = "otherConfig/deleteView";

export const OTHER_CONFIG_DASHBOARDS_FETCH = "otherConfig/fetchDashboards";
export const OTHER_CONFIG_DASHBOARDS_CLEAR = "otherConfig/clearDashboards";
export const OTHER_CONFIG_DASHBOARDS_CLOSE = "otherConfig/closeDashboards";

export const OTHER_CONFIG_DASHBOARD_CREATE = "otherConfig/dashboard/create";
export const OTHER_CONFIG_DASHBOARD_UPDATE = "otherConfig/dashboard/update";
export const OTHER_CONFIG_DASHBOARD_CHANGE = "otherConfig/dashboard/change";
export const OTHER_CONFIG_DASHBOARD_CREATE_SUBMIT = "otherConfig/dashboard/create/submit";
export const OTHER_CONFIG_DASHBOARD_UPDATE_SUBMIT = "otherConfig/dashboard/update/submit";
export const OTHER_CONFIG_DASHBOARD_HIDE = "otherConfig/dashboard/hide";
export const OTHER_CONFIG_DASHBOARD_DELETE = "otherConfig/dashboard/create";

export const OTHER_CONFIG_DASHBOARD_VIEWS_FETCH = "otherConfig/dashboard/views/fetch";
export const OTHER_CONFIG_DASHBOARD_VIEWS_CLEAR = "otherConfig/dashboard/views/clear";

export const OTHER_CONFIG_DASHBOARD_DATASET_FETCH = "otherConfig/dashboard/dataset/fetch";
export const OTHER_CONFIG_DASHBOARD_DATASET_FILTER_SET = "otherConfig/dashboard/dataset/filter/set";
export const OTHER_CONFIG_DASHBOARD_MAP_GEOMETRY_FETCH = "otherConfig/dashboard/map/geometry/fetch";
export const OTHER_CONFIG_DASHBOARD_DATASET_CLEAR = "otherConfig/dashboard/dataset/clear";

export const fetchOtherConfigViews = () => initRequest(
  OTHER_CONFIG_VIEWS_FETCH,
  getViewsUrl(),
  undefined,
  undefined,
  t => ({
    onStart: t("scenes.viewsConfig.messages.fetchViews.start")
  })
);

export const clearOtherConfigViews = () => ({
  type: OTHER_CONFIG_VIEWS_CLEAR
});

export const deleteOtherConfigView = (nodeId: number, id: number) => initRequest(
  OTHER_CONFIG_VIEW_DELETE,
  getDeleteViewUrl(nodeId, id),
  RequestMethod.DELETE,
  undefined,
  t => ({
    onStart: t("scenes.viewsConfig.messages.deleteView.start")
  })
);

export const fetchOtherConfigDashboards = () => initRequest(
  OTHER_CONFIG_DASHBOARDS_FETCH,
  getUserDashboardsUrl(),
  undefined,
  undefined,
  t => ({
    onStart: t("scenes.dashboardsSettings.messages.fetchDashboards.start")
  })
);

export const clearOtherConfigDashboards = () => ({
  type: OTHER_CONFIG_DASHBOARDS_CLEAR
});

export const closeOtherConfigDashboard = () => ({
  type: OTHER_CONFIG_DASHBOARDS_CLOSE
});

export const createOtherConfigDashboard = (dashboard: any) => ({
  type: OTHER_CONFIG_DASHBOARD_CREATE,
  payload: {
    dashboard: dashboard
  }
});

export const updateOtherConfigDashboard = (dashboardId: number) => initRequest(
  OTHER_CONFIG_DASHBOARD_UPDATE,
  getDashboardsUrl(dashboardId),
  undefined,
  undefined,
  t => ({
    onStart: t("scenes.dashboardsSettings.messages.fetchDashboard.start")
  })
);

export const changeOtherConfigDashboard = (dashboard: any) => ({
  type: OTHER_CONFIG_DASHBOARD_CHANGE,
  payload: {
    dashboard: dashboard
  }
});

const getNewDashboard = (dashboard: any, dashboardId?: number) => {
  const newDashboard = _.cloneDeep(dashboard);
  const ids: string[] = [];

  newDashboard.dashboardConfig.forEach((row: any, rowIdx: number) => {
    row.forEach((col: any, colIdx: number) => {
      const type = col[DASHBOARD_ELEM_TYPE_KEY];
      const value = col[DASHBOARD_ELEM_VALUE_KEY];
      const filterDimension = col[DASHBOARD_ELEM_FILTER_DIMENSION_KEY]

      if (type === DASHBOARD_ELEM_TYPE_VALUE_VIEW && value !== null && value !== undefined && !ids.includes(value)) {
        ids.push(value)
      }
      if (filterDimension === "") {
        newDashboard.dashboardConfig[rowIdx][colIdx].filterDimension = null;
      }
    });
  });

  let newFilterLevels = {};
  filters.labels.forEach((label: string) => {
    newFilterLevels = {
      ...newFilterLevels,
      [label]: (newDashboard.filterLevels[label] || false)
    }
  });

  return {
    dashboardId: dashboardId,
    title: newDashboard.title,
    dashboardConfig: JSON.stringify(newDashboard.dashboardConfig),
    viewIds: ids,
    filterLevels: JSON.stringify(newFilterLevels)
  }
};

export const submitOtherConfigDashboardCreate = (dashboard: any) => initRequest(
  OTHER_CONFIG_DASHBOARD_CREATE_SUBMIT,
  getCreateDashboardUrl(),
  RequestMethod.POST,
  getNewDashboard(dashboard),
  t => ({
    onStart: t("scenes.dashboardsSettings.messages.submitDashboardCreate.start")
  })
);

export const submitOtherConfigDashboardUpdate = (dashboardId: number, dashboard: any) => initRequest(
  OTHER_CONFIG_DASHBOARD_UPDATE_SUBMIT,
  getUpdateDashboardUrl(dashboardId),
  RequestMethod.PUT,
  getNewDashboard(dashboard, dashboardId),
  t => ({
    onStart: t("scenes.dashboardsSettings.messages.submitDashboardUpdate.start")
  }),
  {
    dashboardId: dashboardId
  }
);

export const deleteOtherConfigDashboard = (dashboardId: number) => initRequest(
  OTHER_CONFIG_DASHBOARD_DELETE,
  getDeleteDashboardUrl(dashboardId),
  RequestMethod.DELETE,
  undefined,
  t => ({
    onStart: t("scenes.dashboardsSettings.messages.deleteDashboard.start")
  })
);

export const hideOtherConfigDashboard = () => ({
  type: OTHER_CONFIG_DASHBOARD_HIDE
});

export const fetchOtherConfigDashboardViews = () => initRequest(
  OTHER_CONFIG_DASHBOARD_VIEWS_FETCH,
  getViewsUrl(),
  undefined,
  undefined,
  t => ({
    onStart: t("scenes.dashboardsSettings.messages.fetchDashboardViews.start")
  })
);

export const clearOtherConfigDashboardViews = () => ({
  type: OTHER_CONFIG_DASHBOARD_VIEWS_CLEAR
});

export const fetchOtherConfigDashboardsDataset = (nodeId: number, datasetId: string, criteria: any, requestIds: string[]) => initRequest(
  OTHER_CONFIG_DASHBOARD_DATASET_FETCH,
  getDatasetUrl(nodeId, datasetId),
  RequestMethod.POST,
  getCriteriaArrayFromObject(criteria),
  t => ({
    onStart: t("scenes.dashboard.actions.fetchingDataset")
  }),
  {
    requestIds: requestIds,
  },
  "",
  () => true,
  true
);

export const setOtherConfigDashboardsDatasetFilter = (viewIdx: string, dimension: string, value: string) => ({
  type: OTHER_CONFIG_DASHBOARD_DATASET_FILTER_SET,
  payload: {
    viewIdx,
    dimension,
    value
  }
});

export const fetchOtherConfigDashboardsMapGeometries = (viewIdx: string, idList: string[], t: any) => initRequest(
  OTHER_CONFIG_DASHBOARD_MAP_GEOMETRY_FETCH,
  getGeometryUrl(),
  RequestMethod.POST,
  idList,
  t => ({
    onStart: t(`scenes.dashboard.actions.fetchingGeometries`)
  }),
  {
    viewIdx: viewIdx,
    t: t
  },
  "",
  () => true,
  true
);

export const clearOtherConfigDashboardsDataset = () => ({
  type: OTHER_CONFIG_DASHBOARD_DATASET_CLEAR
});