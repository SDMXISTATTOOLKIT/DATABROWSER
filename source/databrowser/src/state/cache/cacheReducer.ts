import {Reducer} from "redux";
import {REQUEST_ERROR, REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {IDataflowCache} from "../../model/IDataflowCache";
import {
  DATAFLOW_CACHE_CLEAR,
  DATAFLOW_CACHE_CREATE,
  DATAFLOW_CACHE_DELETE,
  DATAFLOW_CACHE_FETCH,
  DATAFLOW_CACHE_UPDATE
} from "./cacheActions";

export type DataflowCacheState =  IDataflowCache[] | null;

const cacheReducer: Reducer<DataflowCacheState> = (
  state =  null,
  action
) => {
  switch (action.type) {
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case DATAFLOW_CACHE_FETCH: {
          return action.payload.response;
        }
        case DATAFLOW_CACHE_UPDATE: {
          const newState = (state || []).map((e:IDataflowCache) => {
                if(e.id === action.payload.extra.cacheId && e.nodeId === action.payload.extra.nodeId) {
                  e.ttl = action.payload.extra.ttl;
                }
                return e;
              }
          );
          return newState
        }
        case DATAFLOW_CACHE_CREATE: {
          const newState = (state || []).map((e:IDataflowCache) => {
                if(e.dataflowId === action.payload.extra.oldData.dataflowId && e.nodeId === action.payload.extra.oldData.nodeId) {
                  e.ttl = action.payload.response.ttl;
                  e.id = action.payload.response.id;
                }
                return e;
              }
          );
          return newState
        }
        case DATAFLOW_CACHE_DELETE: {
          const newState = (state || []).map((e:IDataflowCache) => {
                if(e.id === action.payload.extra.cacheId && e.nodeId === action.payload.extra.nodeId) {
                  e.cacheSize = 0;
                  e.cachedDataflow = 0;
                }
                return e;
              }
          );
          return newState
        }
        default:
          return state;
      }
    }
    case REQUEST_ERROR: {
      switch (action.payload.label) {
        case DATAFLOW_CACHE_FETCH: {
          return null;
        }
        default:
          return state;
      }
    }
    case DATAFLOW_CACHE_CLEAR: {
      return null;
    }
    default:
      return state;
  }
};

export default cacheReducer;