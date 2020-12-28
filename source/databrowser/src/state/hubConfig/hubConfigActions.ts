import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {
    getAddHubDashboardUrl,
    getHubConfigGetUrl,
    getHubConfigPutUrl,
    getHubDashboardsUrl,
    getOrderHubDashboardsUrl,
    getRemoveHubDashboardUrl,
    getUserDashboardsUrl
} from "../../serverApi/urls";

export const HUB_CONFIG_FETCH = "hubConfig/fetch";
export const HUB_CONFIG_SEND = "hubConfig/send";
export const HUB_CONFIG_CLEAR = "hubConfig/clear";
export const HUB_CONFIG_DASHBOARDS_FETCH = "hubConfig/dashboards/fetch";
export const HUB_CONFIG_DASHBOARDS_FETCH_ALL = "hubConfig/dashboards/fetchAll";
export const HUB_CONFIG_DASHBOARDS_DASHBOARD_ADD = "hubConfig/dashboards/addDashboard";
export const HUB_CONFIG_DASHBOARDS_DASHBOARD_REMOVE = "hubConfig/dashboards/removeDashboard";
export const HUB_CONFIG_DASHBOARDS_ORDERS_SEND = "hubConfig/dashboards/sendOrders";
export const HUB_CONFIG_DASHBOARDS_CLEAR = "hubConfig/dashboards/clear";
export const HUB_CONFIG_DASHBOARDS_CLEAR_ALL = "hubConfig/dashboards/clearAll";
export const HUB_CONFIG_CLOSE = "hubConfig/close";

export const fetchHubConfig = () => initRequest(
    HUB_CONFIG_FETCH,
    getHubConfigGetUrl(),
    undefined,
    undefined,
    t => ({
        onStart: t("scenes.appSettings.messages.fetchConfig.start")
    })
);

export const sendHubConfig = (config: any) => initRequest(
  HUB_CONFIG_SEND,
  getHubConfigPutUrl(),
    RequestMethod.PUT,
    config,
    t => ({
        onStart: t("scenes.appSettings.messages.sendConfig.start")
    })
);

export const clearHubConfig = () => ({
    type: HUB_CONFIG_CLEAR
});

export const fetchHubConfigDashboards = () => initRequest(
    HUB_CONFIG_DASHBOARDS_FETCH,
    getHubDashboardsUrl(),
    RequestMethod.GET,
    null,
    t => ({
        onStart: t("scenes.appSettings.messages.fetchDashboards.start")
    })
);

export const fetchAllHubConfigDashboards = () => initRequest(
    HUB_CONFIG_DASHBOARDS_FETCH_ALL,
    getUserDashboardsUrl(),
    RequestMethod.GET,
    null,
    t => ({
        onStart: t("scenes.appSettings.messages.fetchAllDashboards.start")
    })
);

export const addHubConfigDashboardsDashboard = (dashboardId: number) => initRequest(
    HUB_CONFIG_DASHBOARDS_DASHBOARD_ADD,
    getAddHubDashboardUrl(dashboardId),
    RequestMethod.POST,
    null,
    t => ({
        onStart: t("scenes.appSettings.messages.addDashboard.start")
    })
);

export const removeHubConfigDashboardsDashboard = (dashboardId: number) => initRequest(
    HUB_CONFIG_DASHBOARDS_DASHBOARD_REMOVE,
    getRemoveHubDashboardUrl(dashboardId),
    RequestMethod.DELETE,
    null,
    t => ({
        onStart:t("scenes.appSettings.messages.removeDashboard.start")
    })
);

export const sendHubConfigDashboardsOrders = (orderedDashboardsIds: number[]) => initRequest(
    HUB_CONFIG_DASHBOARDS_ORDERS_SEND,
    getOrderHubDashboardsUrl(),
    RequestMethod.POST,
    orderedDashboardsIds,
    t => ({
        onStart: t("scenes.appSettings.messages.sendDashboardsOrders.start")
    })
)

export const clearHubConfigDashboards = () => ({
    type: HUB_CONFIG_DASHBOARDS_CLEAR
});

export const clearAllHubConfigDashboards = () => ({
    type: HUB_CONFIG_DASHBOARDS_CLEAR_ALL
});

export const closeHubConfig = () => ({
    type: HUB_CONFIG_CLOSE
});