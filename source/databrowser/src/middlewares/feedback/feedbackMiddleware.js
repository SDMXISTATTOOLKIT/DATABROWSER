import {CATALOG_FETCH} from "../../state/catalog/catalogActions";
import {REQUEST_SUCCESS} from "../request/requestActions";

const feedbackMiddlewareFactory = t => ({getState}) => next => action => {

  const res = next(action);

  const state = getState();

  if (
    action.type === REQUEST_SUCCESS &&
    action.payload.label === CATALOG_FETCH &&
    (!state.catalog.uncategorizedDatasets || state.catalog.uncategorizedDatasets.length === 0) &&
    (!state.catalog.datasets || Object.keys(state.catalog.datasets).length === 0)
  ) {
    window.error.show(t('middlewares.feedback.emptyCatalog'));
  }

  return res;
};

export default feedbackMiddlewareFactory;