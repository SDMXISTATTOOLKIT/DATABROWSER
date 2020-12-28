import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {getDeleteTemplateUrl, getNodeTemplatesUrl} from "../../serverApi/urls";

export const NODE_TEMPLATES_CONFIG_FETCH = "nodeTemplatesConfig/fetch";
export const NODE_TEMPLATES_CONFIG_CLEAR = "nodeTemplatesConfig/clear";
export const NODE_TEMPLATES_CONFIG_TEMPLATE_DELETE = "nodeTemplatesConfig/deleteTemplate";

export const fetchNodeTemplatesConfig = (nodeId: number) => initRequest(
  NODE_TEMPLATES_CONFIG_FETCH,
  getNodeTemplatesUrl(nodeId),
  undefined,
  undefined,
  t => ({
    onStart: t("scenes.nodesSettings.templatesSettings.fetchConfig.start")
  })
);

export const clearNodeTemplatesConfig = () => ({
  type: NODE_TEMPLATES_CONFIG_CLEAR
});

export const deleteNodeTemplatesConfigTemplate = (nodeId: number, id: number) => initRequest(
  NODE_TEMPLATES_CONFIG_TEMPLATE_DELETE,
  getDeleteTemplateUrl(nodeId, id),
  RequestMethod.DELETE,
  undefined,
  t => ({
    onStart: t("scenes.nodesSettings.templatesSettings.deleteTemplate.start")
  }),
  {
    nodeId
  }
);