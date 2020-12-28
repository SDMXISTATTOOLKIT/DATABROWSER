import {IHubNode} from "../../model/IHubNode";
import {Reducer} from "redux";
import {REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {NODE_CLEAR, NODE_DATASET_CSV_FETCH, NODE_FETCH} from "./nodeActions";

export type NodeState = IHubNode | null;

const nodeReducer: Reducer<NodeState> = (
  state = null,
  action
) => {
  switch (action.type) {
    case NODE_CLEAR: {
      return null;
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case NODE_FETCH: {
          return action.payload.response;
        }
        case NODE_DATASET_CSV_FETCH: {
          return {
            ...state
          }
        }
        default:
          return state;
      }
    }
    default:
      return state;
  }
};

export default nodeReducer;