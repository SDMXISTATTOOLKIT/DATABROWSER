import {Reducer} from "redux";
import {REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {
  HUB_CONFIG_CLEAR,
  HUB_CONFIG_DASHBOARDS_CLEAR,
  HUB_CONFIG_DASHBOARDS_DASHBOARD_ADD,
  HUB_CONFIG_DASHBOARDS_DASHBOARD_REMOVE,
  HUB_CONFIG_DASHBOARDS_FETCH,
  HUB_CONFIG_DASHBOARDS_ORDERS_SEND,
  HUB_CONFIG_FETCH,
  HUB_CONFIG_SEND,
  HUB_CONFIG_DASHBOARDS_CLEAR_ALL,
  HUB_CONFIG_DASHBOARDS_FETCH_ALL
} from "./hubConfigActions";

export type HubConfigState = {
  hub: any | null,
  hubDashboards: any[] | null,
  allDashboards: any[] | null
};

const hubConfigReducer: Reducer<HubConfigState> = (
    state = {
      hub: null,
      hubDashboards: null,
      allDashboards: null
    },
    action
) => {
  switch (action.type) {
    case HUB_CONFIG_CLEAR: {
      return {
        ...state,
        hub: null
      };
    }
    case HUB_CONFIG_DASHBOARDS_CLEAR: {
      return {
        ...state,
        hubDashboards: null
      };
    }
    case HUB_CONFIG_DASHBOARDS_CLEAR_ALL: {
      return {
        ...state,
        allDashboards: null
      };
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case HUB_CONFIG_FETCH: {
          return {
            ...state,
            hub: action.payload.response[0]
          }
        }
        case HUB_CONFIG_SEND: {
          return {
            ...state,
            hub: null,
            hubDashboards: null
          };
        }
        case HUB_CONFIG_DASHBOARDS_FETCH: {
          return {
            ...state,
            hubDashboards: action.payload.response
          };
        }
        case HUB_CONFIG_DASHBOARDS_FETCH_ALL: {
          return {
            ...state,
            allDashboards: action.payload.response
          };
        }
        case HUB_CONFIG_DASHBOARDS_DASHBOARD_ADD:
          return {
            ...state,
            allDashboards: null,
            hubDashboards: null
          };
        case HUB_CONFIG_DASHBOARDS_DASHBOARD_REMOVE:
        case HUB_CONFIG_DASHBOARDS_ORDERS_SEND:
          return {
            ...state,
            hubDashboards: null
          }
        default:
          return state;
      }
    }
    default:
      return state;
  }
};

export default hubConfigReducer;