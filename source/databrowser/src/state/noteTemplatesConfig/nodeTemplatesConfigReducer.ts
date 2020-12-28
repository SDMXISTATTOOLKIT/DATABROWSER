import {Reducer} from "redux";
import {REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {NODE_TEMPLATES_CONFIG_CLEAR, NODE_TEMPLATES_CONFIG_FETCH} from "./nodeTemplatesConfigActions";

export type NodesConfigState = {
  templates: any[] | null
};

const nodeTemplatesConfigReducer: Reducer<NodesConfigState> = (
  state = {
    templates: null
  },
  action
) => {
  switch (action.type) {
    case NODE_TEMPLATES_CONFIG_CLEAR: {
      return {
        ...state,
        templates: null
      };
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case NODE_TEMPLATES_CONFIG_FETCH: {
          return {
            ...state,
            templates: action.payload.response.map((v: any) => ({
              ...v,
              datasetId: v.datasetId ? v.datasetId.split("+").join(",") : undefined
            }))
          };
        }
        default:
          return state;
      }
    }
    default:
      return state;
  }
};

export default nodeTemplatesConfigReducer;