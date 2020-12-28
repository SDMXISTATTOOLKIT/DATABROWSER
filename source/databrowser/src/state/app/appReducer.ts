import {Reducer} from "redux";
import {APP_IS_A11Y_SET, APP_LANGUAGE_SET} from "./appActions";
import {INIT} from '../rootActions';
import _ from "lodash";

export type AppState = {
    language: string,
    languages: string[],
    isA11y: boolean
} | null;

const appReducer: Reducer<AppState> = (
    state = null,
    action
) => {
    switch (action.type) {
        case INIT:
            const initialState = {
                language: action.payload.defaultLanguage,
                languages: action.payload.supportedLanguages,
                isA11y: false
            };
            return _.merge(initialState) || initialState;
        case APP_LANGUAGE_SET: {
            return {
                ...state,
                language: action.payload.language
            };
        }
        case APP_IS_A11Y_SET: {
            return {
                ...state,
                isA11y: action.payload.isA11y
            };
        }
        default:
            return state;
    }
};

export default appReducer;