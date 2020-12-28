import {REQUEST_SUCCESS} from "../request/requestActions";
import FileSaver from "file-saver";
import {QUERY_LOG_DOWNLOAD} from "../../components/settings-select/actions";
import {DATASET_DOWNLOAD_SUBMIT} from "../../state/dataset/datasetActions";
import {DASHBOARDS_DATASET_DOWNLOAD_SUBMIT} from "../../state/dashboard/dashboardActions";
import {NODE_DATASET_CSV_FETCH} from "../../state/node/nodeActions";

const fileSaveMiddleware = () => next => action => {

  const result = next(action);

  if (action.type === REQUEST_SUCCESS && action.payload.extra.fileSave) {
    if (
      action.payload.label === QUERY_LOG_DOWNLOAD ||
      action.payload.label === DATASET_DOWNLOAD_SUBMIT ||
      action.payload.label === DASHBOARDS_DATASET_DOWNLOAD_SUBMIT ||
      action.payload.label === NODE_DATASET_CSV_FETCH
    ) {

      let name = action.payload.extra.fileSave.name;

      FileSaver.saveAs(
        new Blob(
          action.payload?.extra?.stringifyResponse
            ? [JSON.stringify(action.payload.response, null, 2)]
            : [action.payload.response],
          {type: action.payload.extra.fileSave.type}
        ),
        name
      )
    }
  }

  return result;

};

export default fileSaveMiddleware;
