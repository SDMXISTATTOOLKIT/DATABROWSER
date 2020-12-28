import {Reducer} from "redux";
import {REQUEST_ERROR, REQUEST_INIT, REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {
  OTHER_CONFIG_DASHBOARD_CHANGE,
  OTHER_CONFIG_DASHBOARD_CREATE,
  OTHER_CONFIG_DASHBOARD_CREATE_SUBMIT,
  OTHER_CONFIG_DASHBOARD_DATASET_CLEAR,
  OTHER_CONFIG_DASHBOARD_DATASET_FETCH,
  OTHER_CONFIG_DASHBOARD_DATASET_FILTER_SET,
  OTHER_CONFIG_DASHBOARD_DELETE,
  OTHER_CONFIG_DASHBOARD_HIDE,
  OTHER_CONFIG_DASHBOARD_MAP_GEOMETRY_FETCH,
  OTHER_CONFIG_DASHBOARD_UPDATE,
  OTHER_CONFIG_DASHBOARD_UPDATE_SUBMIT,
  OTHER_CONFIG_DASHBOARD_VIEWS_CLEAR,
  OTHER_CONFIG_DASHBOARD_VIEWS_FETCH,
  OTHER_CONFIG_DASHBOARDS_CLEAR,
  OTHER_CONFIG_DASHBOARDS_FETCH,
  OTHER_CONFIG_VIEW_DELETE,
  OTHER_CONFIG_VIEWS_CLEAR,
  OTHER_CONFIG_VIEWS_FETCH
} from "./otherConfigActions";
import _ from "lodash"
import {DASHBOARD_VIEW_STATE_ERROR, DASHBOARD_VIEW_STATE_FETCHING} from "../dashboard/dashboardReducer";
import {
  DASHBOARD_ELEM_ENABLE_FILTERS_KEY,
  DASHBOARD_ELEM_TYPE_KEY,
  DASHBOARD_ELEM_TYPE_VALUE_VIEW,
  DASHBOARD_ELEM_VALUE_KEY,
  getViewIdxFromRowAndCol
} from "../../utils/dashboards";
import {
  getFilteredChartLayout,
  getFilteredMapLayout,
  getFilteredTableLayout,
  getFilterTreeFromJsonStat,
  getUpdatedLayout
} from "../../utils/jsonStat";
import {getGeometryDetailLevels} from "../../utils/other";

export type OtherConfigState = {
  views: any[] | null,
  needDashboards: boolean,
  dashboards: any[] | null,
  dashboard: any | null,
  dashboardViews: any[] | null,
  dashboardJsonStats: any,
  dashboardLayouts: any,
  dashboardFilterTrees: any,
  dashboardMaps: any,
};

const otherConfigReducer: Reducer<OtherConfigState> = (
  state = {
    views: null,
    needDashboards: false,
    dashboards: null,
    dashboard: null,
    dashboardViews: null,
    dashboardJsonStats: null,
    dashboardLayouts: null,
    dashboardFilterTrees: null,
    dashboardMaps: null
  },
  action
) => {
  switch (action.type) {
    case OTHER_CONFIG_VIEWS_CLEAR: {
      return {
        ...state,
        views: null
      };
    }
    case OTHER_CONFIG_DASHBOARD_VIEWS_CLEAR: {
      return {
        ...state,
        dashboardViews: null
      };
    }
    case OTHER_CONFIG_DASHBOARDS_CLEAR: {
      return {
        ...state,
        dashboards: null
      };
    }
    case OTHER_CONFIG_DASHBOARD_CREATE: {
      return {
        ...state,
        dashboard: action.payload.dashboard
      };
    }
    case OTHER_CONFIG_DASHBOARD_CHANGE: {
      return {
        ...state,
        dashboard: action.payload.dashboard
      };
    }
    case OTHER_CONFIG_DASHBOARD_HIDE: {
      return {
        ...state,
        dashboard: null
      };
    }
    case OTHER_CONFIG_DASHBOARD_DATASET_CLEAR: {
      return {
        ...state,
        dashboardJsonStats: null,
        dashboardMaps: null
      };
    }
    case OTHER_CONFIG_DASHBOARD_DATASET_FILTER_SET: {
      return {
        ...state,
        dashboardLayouts: {
          ...state.dashboardLayouts,
          [action.payload.viewIdx]: {
            ...state.dashboardLayouts[action.payload.viewIdx],
            layout: getUpdatedLayout(
              action.payload.dimension,
              action.payload.value,
              state.dashboardJsonStats[action.payload.viewIdx],
              state.dashboardLayouts[action.payload.viewIdx].layout,
              state.dashboardFilterTrees[action.payload.viewIdx]
            )
          }
        }
      }
    }
    case REQUEST_INIT: {
      switch (action.payload.label) {
        case OTHER_CONFIG_DASHBOARDS_FETCH: {
          return {
            ...state,
            needDashboards: false
          };
        }
        case OTHER_CONFIG_DASHBOARD_DATASET_FETCH: {
          let jsonStats = _.cloneDeep(state.dashboardJsonStats);
          let layouts = _.cloneDeep(state.dashboardLayouts);
          let filterTrees = _.cloneDeep(state.dashboardFilterTrees);

          (action.payload.extra.requestIds || []).forEach((id: string) => {
            jsonStats = {
              ...jsonStats,
              [id]: DASHBOARD_VIEW_STATE_FETCHING
            };
            layouts = {
              ...layouts,
              [id]: null
            };
            filterTrees = {
              ...filterTrees,
              [id]: null
            };
          });

          return {
            ...state,
            dashboardJsonStats: jsonStats,
            dashboardLayouts: layouts,
            dashboardFilterTrees: filterTrees
          }
        }
        default:
          return state;
      }
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case OTHER_CONFIG_VIEWS_FETCH: {
          return {
            ...state,
            views: action.payload.response.map((v: any) => ({
              ...v,
              datasetId: v.datasetId ? v.datasetId.split("+").join(",") : undefined
            }))
          };
        }
        case OTHER_CONFIG_DASHBOARD_VIEWS_FETCH: {
          return {
            ...state,
            dashboardViews: action.payload.response.map((v: any) => ({
              ...v,
              datasetId: v.datasetId ? v.datasetId.split("+").join(",") : undefined
            }))
          };
        }
        case OTHER_CONFIG_VIEW_DELETE: {
          return {
            ...state,
            views: null
          };
        }
        case OTHER_CONFIG_DASHBOARDS_FETCH: {
          return {
            ...state,
            dashboards: action.payload.response
          };
        }
        case OTHER_CONFIG_DASHBOARD_UPDATE: {
          return {
            ...state,
            dashboard: {
              ...action.payload.response,
              dashboardConfig: action.payload.response?.dashboardConfig
                ? JSON.parse(action.payload.response.dashboardConfig)
                : {},
              filterLevels: action.payload.response?.filterLevels
                ? JSON.parse(action.payload.response.filterLevels)
                : {}
            }
          };
        }
        case OTHER_CONFIG_DASHBOARD_DELETE:
        case OTHER_CONFIG_DASHBOARD_CREATE_SUBMIT:
        case OTHER_CONFIG_DASHBOARD_UPDATE_SUBMIT: {
          return {
            ...state,
            needDashboards: true
          };
        }
        case OTHER_CONFIG_DASHBOARD_DATASET_FETCH: {
          let jsonStats = _.cloneDeep(state.dashboardJsonStats);
          let layouts = _.cloneDeep(state.dashboardLayouts);
          let filterTrees = _.cloneDeep(state.dashboardFilterTrees);

          state.dashboard.dashboardConfig.forEach((row: any, rowIdx: number) => {
            row.forEach((col: any, colIdx: number) => {
              const viewIdx = getViewIdxFromRowAndCol(rowIdx, colIdx);
              if (col[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_VIEW && action.payload.extra.requestIds.includes(viewIdx)) {
                const view = state.dashboard.views[col[DASHBOARD_ELEM_VALUE_KEY]];

                if ((action.payload.response?.id || []).length === 0) {
                  jsonStats[viewIdx] = "";
                  layouts[viewIdx] = null;
                  filterTrees[viewIdx] = null;

                } else {
                  const viewLayouts = JSON.parse(view.layouts);
                  let viewLayout;
                  if (viewLayouts.tableLayout) {
                    viewLayout = getFilteredTableLayout(viewLayouts.tableLayout, action.payload.response);
                  } else if (viewLayouts.mapLayout) {
                    viewLayout = getFilteredMapLayout(viewLayouts.mapLayout, action.payload.response);
                  } else {
                    viewLayout = getFilteredChartLayout(viewLayouts.chartLayout, action.payload.response);
                  }

                  jsonStats[viewIdx] = action.payload.response;
                  layouts[viewIdx] = {
                    layout: viewLayout,
                    tableEmptyChar: viewLayouts.tableEmptyChar,
                    mapDetailLevel: viewLayouts.mapDetailLevel,
                    chartStacked: viewLayouts.chartStacked,
                    chartLegendPosition: viewLayouts.chartLegendPosition,
                    chartColors: viewLayouts.chartColors
                  };
                  filterTrees[viewIdx] = col[DASHBOARD_ELEM_ENABLE_FILTERS_KEY]
                    ? getFilterTreeFromJsonStat(viewLayout, action.payload.response)
                    : null;
                }
              }
            });
          });

          return {
            ...state,
            dashboardJsonStats: jsonStats,
            dashboardLayouts: layouts,
            dashboardFilterTrees: filterTrees
          }
        }
        case OTHER_CONFIG_DASHBOARD_MAP_GEOMETRY_FETCH: {
          return {
            ...state,
            dashboardMaps: {
              ...state.dashboardMaps,
              [action.payload.extra.viewIdx]: {
                geometries: action.payload.response,
                geometryDetailLevels: getGeometryDetailLevels(action.payload.response, false, action.payload.extra.t)
              }
            }
          }
        }
        default:
          return state;
      }
    }
    case REQUEST_ERROR:
      switch (action.payload.label) {
        case OTHER_CONFIG_DASHBOARDS_FETCH: {
          return {
            ...state,
            dashboards: action.payload.statusCode === 404 ? [] : null
          };
        }
        case OTHER_CONFIG_DASHBOARD_DATASET_FETCH: {
          let jsonStats = _.cloneDeep(state.dashboardJsonStats);
          let layouts = _.cloneDeep(state.dashboardLayouts);
          let filterTrees = _.cloneDeep(state.dashboardFilterTrees);

          (action.payload.extra.requestIds || []).forEach((id: string) => {
            jsonStats = {
              ...jsonStats,
              [id]: DASHBOARD_VIEW_STATE_ERROR
            };
            layouts = {
              ...layouts,
              [id]: null
            };
            filterTrees = {
              ...filterTrees,
              [id]: null
            };
          });

          return {
            ...state,
            dashboardJsonStats: jsonStats,
            dashboardLayouts: layouts,
            dashboardFilterTrees: filterTrees
          }
        }
        default:
          return state;
      }
    default:
      return state;
  }
};

export default otherConfigReducer;