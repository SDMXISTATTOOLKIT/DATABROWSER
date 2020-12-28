import {REQUEST_START, REQUEST_SUCCESS} from "../request/requestActions";
import {
  fetchHubConfigDashboards,
  HUB_CONFIG_DASHBOARDS_DASHBOARD_ADD,
  HUB_CONFIG_DASHBOARDS_DASHBOARD_REMOVE,
  HUB_CONFIG_DASHBOARDS_ORDERS_SEND,
  HUB_CONFIG_SEND
} from "../../state/hubConfig/hubConfigActions";
import {fetchHub} from "../../state/hub/hubActions";
import {
  fetchNodesConfigNodeDashboards,
  fetchNodesConfigNodes,
  NODES_CONFIG_NODE_CREATE,
  NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_ADD,
  NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_REMOVE,
  NODES_CONFIG_NODE_DASHBOARDS_ORDERS_SEND,
  NODES_CONFIG_NODE_DELETE,
  NODES_CONFIG_NODE_EDIT,
  NODES_CONFIG_NODES_ORDER_SEND
} from "../../state/nodesConfig/nodesConfigActions";
import {
  fetchUsersConfigUsers,
  USERS_CONFIG_USER_CREATE,
  USERS_CONFIG_USER_DELETE,
  USERS_CONFIG_USER_EDIT,
} from "../../state/usersConfig/usersConfigActions";
import {goToHome} from "../../links";
import {
  fetchOtherConfigViews,
  OTHER_CONFIG_DASHBOARD_UPDATE_SUBMIT,
  OTHER_CONFIG_VIEW_DELETE
} from "../../state/otherConfig/otherConfigActions";
import {
  fetchNodeTemplatesConfig,
  NODE_TEMPLATES_CONFIG_TEMPLATE_DELETE
} from "../../state/noteTemplatesConfig/nodeTemplatesConfigActions";
import {
  DATAFLOW_CACHE_DELETE_ALL,
  DATAFLOW_CACHE_DELETE_CATALOG,
  fetchDataflowCache
} from "../../state/cache/cacheActions";
import {fetchCatalog} from "../../state/catalog/catalogActions";
import {fetchNode} from "../../state/node/nodeActions";
import {clearDashboardsDashboards} from "../../state/dashboard/dashboardActions";

const configMiddleware = ({dispatch, getState}) => next => action => {

  const state = getState();

  const res = next(action);

  if (action.type === REQUEST_START && action.payload.label === NODES_CONFIG_NODE_DELETE) {
    if (state.node && state.node.nodeId === action.payload.extra?.nodeId) {
      goToHome();
    }
  }

  if (action.type === REQUEST_SUCCESS) {
    switch (action.payload.label) {
      case HUB_CONFIG_SEND: {
        goToHome();
        dispatch(fetchHub());
        break;
      }
      case HUB_CONFIG_DASHBOARDS_DASHBOARD_ADD:
      case HUB_CONFIG_DASHBOARDS_DASHBOARD_REMOVE:
      case HUB_CONFIG_DASHBOARDS_ORDERS_SEND: {
        dispatch(fetchHubConfigDashboards());
        break;
      }
      case NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_ADD:
      case NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_REMOVE:
      case NODES_CONFIG_NODE_DASHBOARDS_ORDERS_SEND: {
        dispatch(fetchNodesConfigNodeDashboards(action.payload.extra.nodeId));
        break;
      }
      case NODES_CONFIG_NODES_ORDER_SEND: {
        dispatch(fetchNodesConfigNodes());
        break;
      }
      case NODES_CONFIG_NODE_EDIT: {
        dispatch(fetchNodesConfigNodes());
        if (state.node && state.node.nodeId === action.payload.extra.nodeId) {
          goToHome();
          dispatch(fetchNode(action.payload.extra.nodeId));
        }
        break;
      }
      case NODES_CONFIG_NODE_CREATE:
      case NODES_CONFIG_NODE_DELETE: {
        dispatch(fetchNodesConfigNodes());
        break;
      }
      case USERS_CONFIG_USER_EDIT:
      case USERS_CONFIG_USER_DELETE: {
        if (!action.payload.response.haveError && !action.payload.extra?.isAnonymous) {
          dispatch(fetchUsersConfigUsers());
        }
        break;
      }
      case USERS_CONFIG_USER_CREATE:
        if (!action.payload.response.haveError && !action.payload.extra?.isAnonymous) {
          dispatch(fetchUsersConfigUsers());
        }
        break;
      case OTHER_CONFIG_VIEW_DELETE: {
        dispatch(fetchOtherConfigViews());
        break;
      }
      case NODE_TEMPLATES_CONFIG_TEMPLATE_DELETE: {
        dispatch(fetchNodeTemplatesConfig(action.payload.extra.nodeId));
        break;
      }
      case DATAFLOW_CACHE_DELETE_ALL: {
        dispatch(fetchDataflowCache(action.payload.extra.nodeId));
        break;
      }
      case DATAFLOW_CACHE_DELETE_CATALOG: {
        if (state.node && state.node.nodeId === action.payload.extra.nodeId) {
          goToHome();
          dispatch(fetchCatalog(action.payload.extra.nodeId));
        }
        break;
      }
      case OTHER_CONFIG_DASHBOARD_UPDATE_SUBMIT: {
        if (state.dashboard.dashboards && state.dashboard.dashboards[action.payload.extra.dashboardId]) {
          dispatch(clearDashboardsDashboards(action.payload.extra.dashboardId));
        }
        break;
      }
      default:
        break;
    }
  }

  return res;
};

export default configMiddleware;