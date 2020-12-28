import {APP_LANGUAGE_SET} from "../../state/app/appActions";
import i18n from 'i18next';
import {clearHub} from "../../state/hub/hubActions";
import {clearNode} from "../../state/node/nodeActions";
import {clearCatalog} from "../../state/catalog/catalogActions";

const i18nMiddleware = ({getState, dispatch}) => next => action => {

  const res = next(action);

  window.language = getState().app.language;

  if (action.type === APP_LANGUAGE_SET) {
    i18n.changeLanguage(action.payload.language, () => {
      dispatch(clearHub());
      if (action.payload.nodeId) {
        dispatch(clearCatalog());
        dispatch(clearNode());
      }
    });
  }

  return res;

};

export default i18nMiddleware;