import {Reducer} from "redux";
import {REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {CATALOG_CLEAR, CATALOG_FETCH} from "./catalogActions";
import {ICategoryProvider} from "../../model/ICategoryProvider";
import {LocalCategoryProvider} from "../../model/LocalCategoryProvider";

export type CatalogState = ICategoryProvider | null;

const catalogReducer: Reducer<CatalogState> = (
  state = null,
  action
) => {
  switch (action.type) {
    case CATALOG_CLEAR: {
      return null;
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case CATALOG_FETCH: {
          return new LocalCategoryProvider(
            action.payload.response.categoryGroups,
            action.payload.response.datasetMap,
            action.payload.response.datasetUncategorized,
            action.payload.extra.nodeId
          );
        }
        default:
          return state;
      }
    }
    default:
      return state;
  }
};

export default catalogReducer;