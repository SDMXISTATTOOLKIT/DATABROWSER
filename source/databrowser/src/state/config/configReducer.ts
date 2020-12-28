import {Reducer} from "redux";
import {INIT} from '../rootActions';

export type ConfigState = {
  baseURL: string
} | null;

const configReducer: Reducer<ConfigState> = (
    state = null,
    action
) => {
  switch (action.type) {
    case INIT: {
      return {
        baseURL: action.payload.baseURL
      };
    }
    default:
      return state;
  }
};

export default configReducer;