import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {
    getAddNodeDashboardUrl,
    getNodeConfigGetUrl,
    getNodeDashboardsUrl,
    getNodeDeleteUrl,
    getNodesConfigPostUrl,
    getNodesConfigPutUrl,
    getNodesConfigUrl,
    getNodesOrderPutUrl,
    getOrderNodeDashboardsUrl,
    getRemoveNodeDashboardUrl,
    getUserDashboardsUrl
} from "../../serverApi/urls";

export const NODES_CONFIG_NODES_FETCH = "nodesConfig/fetchNodes";
export const NODES_CONFIG_NODES_CLEAR = "nodesConfig/clearNodes";
export const NODES_CONFIG_NODE_FETCH = "nodesConfig/fetchNode";
export const NODES_CONFIG_NODE_CLEAR = "nodesConfig/clearNode";
export const NODES_CONFIG_NODES_ORDER_SEND = "nodesConfig/sendNodesOrder";
export const NODES_CONFIG_NODE_DELETE = "nodesConfig/deleteNode";
export const NODES_CONFIG_NODE_CREATE = "nodesConfig/createNode";
export const NODES_CONFIG_NODE_EDIT = "nodesConfig/editNode";
export const NODES_CONFIG_NODE_DASHBOARDS_FETCH = "nodesConfig/nodeDashboards/fetch";
export const NODES_CONFIG_NODE_DASHBOARDS_FETCH_ALL = "nodesConfig/nodeDashboards/fetchAll";
export const NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_ADD = "nodesConfig/nodeDashboards/addDashboard";
export const NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_REMOVE = "nodesConfig/nodeDashboards/removeDashboard";
export const NODES_CONFIG_NODE_DASHBOARDS_ORDERS_SEND = "nodesConfig/nodeDashboards/sendOrders";
export const NODES_CONFIG_NODE_DASHBOARDS_CLEAR = "nodesConfig/nodeDashboards/clear";
export const NODES_CONFIG_NODE_DASHBOARDS_CLEAR_ALL = "nodesConfig/nodeDashboards/clearAll";
export const NODES_CONFIG_CLOSE = "nodesConfig/close";

export const fetchNodesConfigNodes = () => initRequest(
    NODES_CONFIG_NODES_FETCH,
    getNodesConfigUrl(),
    undefined,
    undefined,
    t => ({
        onStart: t("scenes.nodesSettings.messages.fetchNodes.start")
    })
);

export const clearNodesConfigNodes = () => ({
  type: NODES_CONFIG_NODES_CLEAR
});

export const sendNodesConfigNodesOrder = (orderedIds: number[]) => initRequest(
  NODES_CONFIG_NODES_ORDER_SEND,
  getNodesOrderPutUrl(),
  RequestMethod.PUT,
  orderedIds,
  t => ({
      onStart: t("scenes.nodesSettings.messages.sendNodesOrder.start")
  })
);

export const deleteNodesConfigNode = (nodeId: number) => initRequest(
  NODES_CONFIG_NODE_DELETE,
  getNodeDeleteUrl(nodeId),
  RequestMethod.DELETE,
  undefined,
  t => ({
      onStart: t("scenes.nodesSettings.messages.deleteNode.start")
  }),
  {
      nodeId,
      onForce: () => initRequest(
          NODES_CONFIG_NODE_DELETE,
          getNodeDeleteUrl(nodeId, true),
          RequestMethod.DELETE,
          undefined,
          t => ({
              onStart: t("scenes.nodesSettings.messages.deleteNode.start")
          }),
          {
              nodeId
          }
      )
  }
);

export const fetchNodesConfigNode = (nodeId: number) => initRequest(
  NODES_CONFIG_NODE_FETCH,
  getNodeConfigGetUrl(nodeId),
  undefined,
  undefined,
  t => ({
      onStart: t("scenes.nodesSettings.messages.fetchNode.start")
  })
);

export const clearNodesConfigNode = () => ({
  type: NODES_CONFIG_NODE_CLEAR
});

export const sendNodesConfigNodeCreate = (node: any) => initRequest(
  NODES_CONFIG_NODE_CREATE,
  getNodesConfigPostUrl(),
  RequestMethod.POST,
  node,
  t => ({
      onStart: t("scenes.nodesSettings.messages.sendNodeCreate.start")
  })
);

export const sendNodesConfigNodeEdit = (node: any) => initRequest(
    NODES_CONFIG_NODE_EDIT,
    getNodesConfigPutUrl(node.nodeId),
    RequestMethod.PUT,
    node,
    t => ({
        onStart: t("scenes.nodesSettings.messages.sendNodeEdit.start")
    }),
    {
        nodeId: node.nodeId
    }
);

export const fetchNodesConfigNodeDashboards = (nodeId: number) => initRequest(
    NODES_CONFIG_NODE_DASHBOARDS_FETCH,
    getNodeDashboardsUrl(nodeId),
    RequestMethod.GET,
    null,
    t => ({
        onStart: t("scenes.nodesSettings.messages.fetchNodeDashboards.start")
    }),
    {
        nodeId
    }
);

export const fetchAllNodeConfigDashboards = () => initRequest(
    NODES_CONFIG_NODE_DASHBOARDS_FETCH_ALL,
    getUserDashboardsUrl(),
    RequestMethod.GET,
    null,
    t => ({
        onStart: t("scenes.nodesSettings.messages.fetchAllDashboards.start")
    })
);

export const addNodesConfigDashboardsNodeDashboard = (nodeId: number, dashboardId: number) => initRequest(
    NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_ADD,
    getAddNodeDashboardUrl(nodeId, dashboardId),
    RequestMethod.POST,
    null,
    t => ({
        onStart: t("scenes.nodesSettings.messages.addNodeDashboard.start")
    }),
    {
        nodeId
    }
);

export const removeNodesConfigDashboardsNodeDashboard = (nodeId: number, dashboardId: number) => initRequest(
    NODES_CONFIG_NODE_DASHBOARDS_DASHBOARD_REMOVE,
    getRemoveNodeDashboardUrl(nodeId, dashboardId),
    RequestMethod.DELETE,
    null,
    t => ({
        onStart: t("scenes.nodesSettings.messages.removeNodeDashboard.start")
    }),
    {
        nodeId
    }
);

export const sendNodesConfigDashboardsNodeOrders = (nodeId: number, orderedDashboardsIds: number[]) => initRequest(
    NODES_CONFIG_NODE_DASHBOARDS_ORDERS_SEND,
    getOrderNodeDashboardsUrl(nodeId),
    RequestMethod.POST,
    orderedDashboardsIds,
    t => ({
        onStart: t("scenes.nodesSettings.messages.sendNodeDashboardsOrders.start")
    }),
    {
        nodeId
    }
)

export const clearNodesConfigNodeDashboards = () => ({
    type: NODES_CONFIG_NODE_DASHBOARDS_CLEAR
});

export const clearAllNodesConfigNodeDashboards = () => ({
    type: NODES_CONFIG_NODE_DASHBOARDS_CLEAR_ALL
});

export const closeNodesConfig = () => ({
    type: NODES_CONFIG_CLOSE
});