import {getJsonStatTableSupportStructures} from "../components/table/jsonStatTable";

onmessage = event => {
  const {
    jsonStat,
    layout,
    isPreview,
    removeEmptyLines,
    hiddenAttributes
  } = event.data;

  const result = getJsonStatTableSupportStructures(jsonStat, layout, isPreview, removeEmptyLines, hiddenAttributes);
  postMessage(result);
};