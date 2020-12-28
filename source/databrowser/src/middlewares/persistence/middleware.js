import {INIT} from "../../state/rootActions";

export const PERSISTENCE_LOCAL_STORAGE_KEY = "state";
export const PERSISTENCE_ACTION_KEY = "persistentState";

const persistenceMiddleware = ({getState}) => next => action => {

  // when init action is dispatched
  if (action.type === INIT) {
    if (!action.payload) {
      action.payload = {};
    }
    // attach persistent state to the action
    action.payload[PERSISTENCE_ACTION_KEY] = JSON.parse(localStorage.getItem(PERSISTENCE_LOCAL_STORAGE_KEY) || "{}");
  }

  const result = next(action);

  const state = getState();

  // any time an action is dispatched, update persistent state (localStorage is fast)
  localStorage.setItem(
    PERSISTENCE_LOCAL_STORAGE_KEY,
    JSON.stringify({
      user: {
        ...state.user,
        isSetPasswordDialogOpen: undefined
      }
    })
  );

  return result;
};

export default persistenceMiddleware;