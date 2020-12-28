import React, {forwardRef} from 'react';
import {compose} from "redux";
import {connect} from "react-redux";
import DashboardsManager from "../../../dashboards-manager";
import {
  addNodesConfigDashboardsNodeDashboard,
  clearAllNodesConfigNodeDashboards,
  clearNodesConfigNodeDashboards,
  fetchAllNodeConfigDashboards,
  fetchNodesConfigNodeDashboards,
  removeNodesConfigDashboardsNodeDashboard,
  sendNodesConfigDashboardsNodeOrders
} from "../../../../state/nodesConfig/nodesConfigActions";

const mapStateToProps = state => ({
  config: state.hubConfig.hub,
  user: state.user,
  dashboards: state.nodesConfig.nodeDashboards,
  allDashboards: state.nodesConfig.allDashboards
});

const mapDispatchToProps = dispatch => ({
  fetchDashboards: nodeId => dispatch(fetchNodesConfigNodeDashboards(nodeId)),
  fetchAllDashboards: () => dispatch(fetchAllNodeConfigDashboards()),
  clearDashboards: () => dispatch(clearNodesConfigNodeDashboards()),
  clearAllDashboards: () => dispatch(clearAllNodesConfigNodeDashboards()),
  addDashboard: (nodeId, dashboardId) => dispatch(addNodesConfigDashboardsNodeDashboard(nodeId, dashboardId)),
  removeDashboard: (nodeId, dashboardId) => dispatch(removeNodesConfigDashboardsNodeDashboard(nodeId, dashboardId)),
  sendDashboardsOrders: (nodeId, dashboardsIds) => dispatch(sendNodesConfigDashboardsNodeOrders(nodeId, dashboardsIds))
});

const DashboardsSettingsForm = ({
                                  dashboards,
                                  allDashboards,
                                  fetchDashboards,
                                  fetchAllDashboards,
                                  clearDashboards,
                                  clearAllDashboards,
                                  addDashboard,
                                  removeDashboard,
                                  sendDashboardsOrders,
                                  nodeId
                                }, ref) =>
  <DashboardsManager
    dashboards={dashboards}
    allDashboards={allDashboards}
    fetchDashboards={() => fetchDashboards(nodeId)}
    fetchAllDashboards={fetchAllDashboards}
    clearDashboards={clearDashboards}
    clearAllDashboards={clearAllDashboards}
    addDashboard={dashboardId => addDashboard(nodeId, dashboardId)}
    removeDashboard={dashboardId => removeDashboard(nodeId, dashboardId)}
    sendDashboardsOrders={dashboardsIds => sendDashboardsOrders(nodeId, dashboardsIds)}
    ref={ref}
  />;

export default compose(connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(DashboardsSettingsForm);