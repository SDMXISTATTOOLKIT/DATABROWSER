import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {getNodePermissionsConfigGetUrl, getNodePermissionsConfigPutUrl} from "../../serverApi/urls";

export const NODE_PERMISSIONS_CONFIG_FETCH = "nodePermissionsConfig/fetch";
export const NODE_PERMISSIONS_CONFIG_CLEAR = "nodePermissionsConfig/clear";
export const NODE_PERMISSIONS_CONFIG_SEND = "nodePermissionsConfig/send";

export const fetchNodePermissionsConfig = (nodeId: number) => initRequest(
    NODE_PERMISSIONS_CONFIG_FETCH,
    getNodePermissionsConfigGetUrl(nodeId),
    undefined,
    undefined,
    t => ({
        onStart: t("scenes.nodesSettings.permissionsSettings.fetchConfig.start")
    })
);

export const sendNodePermissionsConfig = (nodeId: number, config: any) => initRequest(
    NODE_PERMISSIONS_CONFIG_SEND,
    getNodePermissionsConfigPutUrl(nodeId),
    RequestMethod.PUT,
    config,
    t => ({
        onStart: t("scenes.nodesSettings.permissionsSettings.sendConfig.start")
    })
);

export const clearNodePermissionsConfig = () => ({
    type: NODE_PERMISSIONS_CONFIG_CLEAR
});
