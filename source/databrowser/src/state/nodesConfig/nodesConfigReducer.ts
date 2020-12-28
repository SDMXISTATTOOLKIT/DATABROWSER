import {Reducer} from "redux";
import {REQUEST_START, REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {
  NODES_CONFIG_NODE_CLEAR,
  NODES_CONFIG_NODE_CREATE,
  NODES_CONFIG_NODE_DASHBOARDS_CLEAR,
  NODES_CONFIG_NODE_DASHBOARDS_CLEAR_ALL,
  NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_ADD,
  NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_REMOVE,
  NODES_CONFIG_NODE_DASHBOARDS_FETCH,
  NODES_CONFIG_NODE_DASHBOARDS_FETCH_ALL,
  NODES_CONFIG_NODE_DASHBOARDS_ORDERS_SEND,
  NODES_CONFIG_NODE_EDIT,
  NODES_CONFIG_NODE_FETCH,
  NODES_CONFIG_NODES_CLEAR,
  NODES_CONFIG_NODES_FETCH
} from "./nodesConfigActions";
import {IHubNode} from "../../model/IHubNode";

export type NodesConfigState = {
  nodes: IHubNode[] | null,
  node: IHubNode | null,
  nodeDashboards: any[] | null,
  allDashboards: any[] | null
};

const nodesConfigReducer: Reducer<NodesConfigState> = (
  state = {
    nodes: null,
    node: null,
    nodeDashboards: null,
    allDashboards: null
  },
  action
) => {
  switch (action.type) {
    case NODES_CONFIG_NODES_CLEAR: {
      return {
        ...state,
        nodes: null
      };
    }
    case NODES_CONFIG_NODE_CLEAR: {
      return {
        ...state,
        node: null
      };
    }
    case NODES_CONFIG_NODE_DASHBOARDS_CLEAR: {
      return {
        ...state,
        nodeDashboards: null
      };
    }
    case NODES_CONFIG_NODE_DASHBOARDS_CLEAR_ALL: {
      return {
        ...state,
        allDashboards: null
      };
    }
    case REQUEST_START: {
      switch (action.payload.label) {
        case NODES_CONFIG_NODE_EDIT: {
          return {
            ...state,
            node: null
          };
        }
        default:
          return state;
      }
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case NODES_CONFIG_NODES_FETCH: {
          return {
            ...state,
            nodes: action.payload.response
          };
        }
        case NODES_CONFIG_NODE_FETCH: {
          return {
            ...state,
            node: action.payload.response
          }
        }
        case NODES_CONFIG_NODE_CREATE:
        case NODES_CONFIG_NODE_EDIT:
          return {
            ...state,
            node: null
          };
        case NODES_CONFIG_NODE_DASHBOARDS_FETCH:
          return {
            ...state,
            nodeDashboards: action.payload.response
          };
        case NODES_CONFIG_NODE_DASHBOARDS_FETCH_ALL:
          return {
            ...state,
            allDashboards: action.payload.response
          };
        case NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_ADD:
          return {
            ...state,
            allDashboards: null,
            nodeDashboards: null
          };
        case NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_REMOVE:
        case NODES_CONFIG_NODE_DASHBOARDS_ORDERS_SEND:
          return {
            ...state,
            nodeDashboards: null
          }
        default:
          return state;
      }
    }
    default:
      return state;
  }
};

export default nodesConfigReducer;