import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {getDatasetDownloadUrl, getNodeUrl} from "../../serverApi/urls";
import {getCriteriaArrayFromObject} from "../../utils/criteria";

export const NODE_FETCH = "node/fetch";
export const NODE_CLEAR = "node/clear";

export const NODE_DATASET_CSV_FETCH = "node/dataset/csv/fetch";

export const fetchNode = (nodeId: number) =>
  initRequest(
    NODE_FETCH,
    getNodeUrl(nodeId),
    undefined,
    undefined,
    t => ({
      onStart: t("domains.node.messages.fetch.start")
    })
  );

export const clearNode = () => ({
  type: NODE_CLEAR
})

export const fetchNodeDatasetCsv = (nodeId: number, datasetId: string, criteria: any, datasetTitle: string) => initRequest(
  NODE_DATASET_CSV_FETCH,
  getDatasetDownloadUrl(nodeId, datasetId, "csv"),
  RequestMethod.POST,
  getCriteriaArrayFromObject(criteria),
  t => ({
    onStart: t("domains.node.messages.datasetCsvFetch.start", {title: datasetTitle})
  }),
  {
    fileSave: {
      name: `${datasetTitle}.csv`
    }
  }
);