import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {getClearMemoryCacheUrl, getQueryLog} from "../../serverApi/urls";

export const QUERY_LOG_DOWNLOAD = "query/log/download";
export const MEMORY_CACHE_CLEAR = "memory/cache/clear";

export const downloadQueryLog = limit => initRequest(
  QUERY_LOG_DOWNLOAD,
  getQueryLog(limit),
  RequestMethod.GET,
  null,
  null,
  {
    fileSave: {
      name: "sdmx-query-log.txt",
      type: "text/plain;charset=utf-8"
    },
    stringifyResponse: true
  }
);

export const clearMemoryCache = () => initRequest(
  MEMORY_CACHE_CLEAR,
  getClearMemoryCacheUrl(),
  RequestMethod.POST,
  null,
  t => ({
    onStart: t("components.header.settings.clearServerCacheAction.messages.clear.start")
  })
);
