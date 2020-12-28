import {Reducer} from "@reduxjs/toolkit";
import {IHub} from "../../model/IHub";
import {IHubMinimalNode} from "../../model/IHubMinimalNode";
import {REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {HUB_CLEAR, HUB_FETCH} from "./hubActions";

export type HubState = {
  hub: IHub,
  nodes: IHubMinimalNode[]
} | null;

const hubReducer: Reducer<HubState> = (
  state: HubState = null,
  action
) => {
  switch (action.type) {
    case HUB_CLEAR: {
      return null;
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case HUB_FETCH: {
          return ({
            hub: action.payload.response.hub,
            nodes: action.payload.response.nodes
          });
        }
        default:
          return state;
      }
    }
    default:
      return state;
  }
};

export default hubReducer;