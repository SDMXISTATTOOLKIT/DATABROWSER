import {Reducer} from "redux";
import {INIT} from '../rootActions';

export type ConfigState = object | null;

const appConfigReducer: Reducer<ConfigState> = (
    state = null,
    action
) => {
    switch (action.type) {
        case INIT: {
            return action.payload.appConfig;
        }
        default:
            return state;
    }
};

export default appConfigReducer;