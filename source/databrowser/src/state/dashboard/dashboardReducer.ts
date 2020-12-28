import {Reducer} from "redux";
import {REQUEST_ERROR, REQUEST_INIT, REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {
  DASHBOARDS_DASHBOARD_FETCH,
  DASHBOARDS_DASHBOARDS_CLEAR,
  DASHBOARDS_DATASET_FETCH_ENABLE_STATUS_SET,
  DASHBOARDS_DATASET_FILTER_SET,
  DASHBOARDS_MAP_GEOMETRIES_FETCH,
  DASHBOARDS_UPDATED_DASHBOARD_ID_RESET
} from "./dashboardActions";
import {
  DASHBOARD_ELEM_FILTER_DIMENSION_KEY,
  DASHBOARD_ELEM_TYPE_KEY,
  DASHBOARD_ELEM_TYPE_VALUE_VIEW,
  getViewIdxFromRowAndCol
} from "../../utils/dashboards";
import {getCriteriaObjectFromArray} from "../../utils/criteria";
import {
  FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_ERROR,
  FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_INIT,
  FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_SUCCESS
} from "../../middlewares/fetchDashboardDatasetAsyncHandler/actions";
import _ from "lodash";
import {getUpdatedLayout} from "../../utils/jsonStat";
import {getGeometryDetailLevels} from "../../utils/other";

export const DASHBOARD_VIEW_STATE_APPLY_FILTERS = "DASHBOARD_VIEW_STATE_APPLY_FILTERS"
export const DASHBOARD_VIEW_STATE_FETCHING = "DASHBOARD_VIEW_STATE_FETCHING"
export const DASHBOARD_VIEW_STATE_ERROR = "DASHBOARD_VIEW_STATE_ERROR"
export const DASHBOARD_VIEW_STATE_ERROR_FETCHING_GEOMETRIES = "DASHBOARD_VIEW_STATE_ERROR_FETCHING_GEOMETRIES"

type DashboardState = {
  dashboards: any | null,
  layouts: any | null,
  isFetchDatasetDisabled: boolean,
  jsonStats: any | null,
  filterTrees: any | null,
  maps: any | null,
  updatedDashboardId: number | null
}

const initialState = {
  dashboards: null,
  layouts: null,
  isFetchDatasetDisabled: true,
  jsonStats: null,
  filterTrees: null,
  maps: null,
  updatedDashboardId: null
};

const dashboardReducer: Reducer<DashboardState> = (
  state = initialState,
  action
) => {
  switch (action.type) {
    case DASHBOARDS_DASHBOARDS_CLEAR: {
      return {
        ...initialState,
        updatedDashboardId: action.payload.updatedDashboardId || null
      }
    }
    case DASHBOARDS_UPDATED_DASHBOARD_ID_RESET: {
      return {
        ...state,
        updatedDashboardId: null
      }
    }
    case DASHBOARDS_DATASET_FETCH_ENABLE_STATUS_SET: {
      return {
        ...state,
        isFetchDatasetDisabled: action.payload.disabled
      }
    }
    case FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_INIT: {
      let currentDashboardJsonStats = _.cloneDeep(state.jsonStats?.[action.payload.dashboardId]);
      let currentDashboardLayouts = _.cloneDeep(state.layouts?.[action.payload.dashboardId]);
      let currentDashboardFilterTrees = _.cloneDeep(state.filterTrees?.[action.payload.dashboardId]);

      state.dashboards[action.payload.dashboardId].dashboardConfig.forEach((row: any, rowIdx: number) => {
        row.forEach((col: any, colIdx: number) => {
          if (col[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_VIEW) {
            const viewIdx = getViewIdxFromRowAndCol(rowIdx, colIdx);
            const isStaticView = (col[DASHBOARD_ELEM_FILTER_DIMENSION_KEY] || "").length === 0;

            const jsonStat = currentDashboardJsonStats?.[viewIdx];
            currentDashboardJsonStats = {
              ...currentDashboardJsonStats,
              [viewIdx]: (isStaticView && jsonStat)
                ? jsonStat
                : DASHBOARD_VIEW_STATE_FETCHING
            }

            const layout = currentDashboardLayouts?.[viewIdx];
            currentDashboardLayouts = {
              ...currentDashboardLayouts,
              [viewIdx]: (isStaticView && jsonStat)
                ? layout
                : null
            }

            const filterTree = currentDashboardFilterTrees?.[viewIdx];
            currentDashboardFilterTrees = {
              ...currentDashboardFilterTrees,
              [viewIdx]: (isStaticView && jsonStat)
                ? filterTree
                : null
            }
          }
        });
      });

      return {
        ...state,
        jsonStats: {
          ...state.jsonStats,
          [action.payload.dashboardId]: currentDashboardJsonStats
        },
        layouts: {
          ...state.layouts,
          [action.payload.dashboardId]: currentDashboardLayouts
        },
        filterTrees: {
          ...state.filterTrees,
          [action.payload.dashboardId]: currentDashboardFilterTrees
        }
      }
    }
    case FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_SUCCESS: {
      return (state.jsonStats && state.layouts && state.filterTrees)
        ? {
          ...state,
          jsonStats: {
            ...state.jsonStats,
            [action.payload.dashboardId]: {
              ...state.jsonStats[action.payload.dashboardId],
              ...action.payload.dashboardJsonStats
            }
          },
          layouts: {
            ...state.layouts,
            [action.payload.dashboardId]: {
              ...state.layouts[action.payload.dashboardId],
              ...action.payload.dashboardLayouts
            }
          },
          filterTrees: {
            ...state.filterTrees,
            [action.payload.dashboardId]: {
              ...state.filterTrees[action.payload.dashboardId],
              ...action.payload.dashboardFilterTrees
            }
          }
        }
        : {
          ...state
        }
    }
    case FETCH_DASHBOARD_DATASET_ASYNC_HANDLER_ERROR: {
      const jsonStats = _.cloneDeep(state.jsonStats);
      const layouts = _.cloneDeep(state.layouts);
      const filterTrees = _.cloneDeep(state.filterTrees);

      (action.payload.requestIds || []).forEach((id: string) => {
        if (jsonStats) {
          jsonStats[action.payload.dashboardId] = {
            ...jsonStats[action.payload.dashboardId],
            [id]: DASHBOARD_VIEW_STATE_ERROR
          }
        }
        if (layouts) {
          layouts[action.payload.dashboardId] = {
            ...layouts[action.payload.dashboardId],
            [id]: null
          }
        }
        if (filterTrees) {
          filterTrees[action.payload.dashboardId] = {
            ...filterTrees[action.payload.dashboardId],
            [id]: null
          }
        }
      });

      return {
        ...state,
        jsonStats: jsonStats,
        layouts: layouts,
        filterTrees: filterTrees
      }
    }
    case DASHBOARDS_DATASET_FILTER_SET: {
      return {
        ...state,
        layouts: {
          ...state.layouts,
          [action.payload.dashboardId]: {
            ...state.layouts[action.payload.dashboardId],
            [action.payload.viewIdx]: {
              ...state.layouts[action.payload.dashboardId][action.payload.viewIdx],
              layout: getUpdatedLayout(
                action.payload.dimension,
                action.payload.value,
                state.jsonStats[action.payload.dashboardId][action.payload.viewIdx],
                state.layouts[action.payload.dashboardId][action.payload.viewIdx].layout,
                state.filterTrees[action.payload.dashboardId][action.payload.viewIdx]
              )
            }
          }
        }
      }
    }
    case REQUEST_INIT: {
      switch (action.payload.label) {
        case DASHBOARDS_MAP_GEOMETRIES_FETCH: {
          return {
            ...state,
            maps: {
              ...state.maps,
              [action.payload.extra.dashboardId]: {
                ...state?.maps?.[action.payload.extra.dashboardId],
                [action.payload.extra.viewIdx]: {
                  geometries: null,
                  geometryDetailLevels: null
                }
              }
            }
          }
        }
        default:
          return state
      }
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case DASHBOARDS_DASHBOARD_FETCH: {
          const views = _.cloneDeep(action.payload.response.views);
          for (let key in views) {
            if (views.hasOwnProperty(key)) {
              views[key] = {
                ...views[key],
                datasetId: views[key].datasetId ? views[key].datasetId.split("+").join(",") : undefined,
                criteria: getCriteriaObjectFromArray(views[key].criteria),
                layouts: JSON.parse(views[key].layouts)
              }
            }
          }

          return {
            ...state,
            dashboards: {
              ...state.dashboards,
              [action.payload.extra.dashboardId]: {
                ...action.payload.response,
                dashboardConfig: action.payload.response?.dashboardConfig
                  ? JSON.parse(action.payload.response.dashboardConfig)
                  : {},
                filterLevels: action.payload.response?.filterLevels
                  ? JSON.parse(action.payload.response.filterLevels)
                  : {},
                views: views
              }
            }
          }
        }
        case DASHBOARDS_MAP_GEOMETRIES_FETCH: {
          return {
            ...state,
            maps: {
              ...state.maps,
              [action.payload.extra.dashboardId]: {
                ...state.maps[action.payload.extra.dashboardId],
                [action.payload.extra.viewIdx]: {
                  geometries: action.payload.response,
                  geometryDetailLevels: getGeometryDetailLevels(action.payload.response, false, action.payload.extra.t)
                }
              }
            }
          }
        }
        default:
          return state
      }
    }
    case REQUEST_ERROR: {
      switch (action.payload.label) {
        case DASHBOARDS_MAP_GEOMETRIES_FETCH: {
          return (state.jsonStats && state.layouts && state.filterTrees)
            ? {
              ...state,
              jsonStats: {
                ...state.jsonStats,
                [action.payload.extra.dashboardId]: {
                  ...state.jsonStats[action.payload.extra.dashboardId],
                  [action.payload.extra.viewIdx]: DASHBOARD_VIEW_STATE_ERROR_FETCHING_GEOMETRIES
                }
              },
              layouts: {
                ...state.layouts,
                [action.payload.extra.dashboardId]: {
                  ...state.layouts[action.payload.extra.dashboardId],
                  [action.payload.extra.viewIdx]: null
                }
              },
              filterTrees: {
                ...state.filterTrees,
                [action.payload.extra.dashboardId]: {
                  ...state.filterTrees[action.payload.extra.dashboardId],
                  [action.payload.extra.viewIdx]: null
                }
              },
              maps: {
                ...state.maps,
                [action.payload.extra.dashboardId]: {
                  ...state.maps[action.payload.extra.dashboardId],
                  [action.payload.extra.viewIdx]: {
                    geometries: null,
                    geometryDetailLevels: null
                  }
                }
              }
            }
            : {
              ...state
            }
        }
        default:
          return state
      }
    }
    default:
      return state
  }
};

export default dashboardReducer;