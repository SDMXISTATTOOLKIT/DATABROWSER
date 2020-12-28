import {initRequest} from "../../middlewares/request/requestActions";
import {getNodeCatalogUrl} from "../../serverApi/urls";

export const CATALOG_FETCH = "catalog/fetch";
export const CATALOG_CLEAR = "catalog/clear";

export const fetchCatalog = (nodeId: number) => initRequest(
  CATALOG_FETCH,
  getNodeCatalogUrl(nodeId),
  undefined,
  undefined,
  t => ({
    onStart: t("domains.catalog.messages.fetch.start")
  }),
  {
    nodeId
  }
);

export const clearCatalog = () => ({
  type: CATALOG_CLEAR
})