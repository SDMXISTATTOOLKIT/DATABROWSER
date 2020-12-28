import {configureStore} from "@reduxjs/toolkit";
import rootReducer from "./state/rootReducer";
import userMiddlewareFactory from "./middlewares/user/userMiddlewareFactory";
import requestMiddlewareFactory from "./middlewares/request/requestMiddlewareFactory";
import {showGenericError, showTranslatedGenericErrorFactory} from "./utils/other";
import configMiddleware from "./middlewares/config/configMiddleware";
import fileSaveMiddleware from "./middlewares/file-save/middleware";
import persistenceMiddleware from "./middlewares/persistence/middleware";
import i18nMiddleware from "./middlewares/i18n-middleware/middleware";
import i18n from "i18next";
import {initReactI18next} from "react-i18next";
import {init as initAction} from "./state/rootActions";
import requestSpinnerMiddlewareFactory
  from "./middlewares/request-spinner/requestSpinnerMiddlewareFactory";
import {getAppConfigUrl, getHubMinimalInfoUrl, getInitConfigUrl} from "./serverApi/urls";
import feedbackMiddlewareFactory from "./middlewares/feedback/feedbackMiddleware";
import fetchDatasetAsyncHandlerMiddlewareFactory
  from "./middlewares/fetchDatasetAsyncHandler/middlewareFactory";
import fetchDashboardDatasetAsyncHandlerMiddlewareFactory
  from "./middlewares/fetchDashboardDatasetAsyncHandler/middlewareFactory";
import a11yMiddleware from "./middlewares/a11y-middleware/middleware";

const getRandomParam = () => "?random=" + Math.floor(Math.random() * 16777215).toString(16);

const init = cb => {

  fetch(`./${getInitConfigUrl()}${getRandomParam()}`)
    .then(response => response.json())
    .then(({baseURL}) => {
      fetch(`./${getAppConfigUrl()}${getRandomParam()}`)
        .then(response => response.json())
        .then(config => {

          fetch(`${baseURL}${getHubMinimalInfoUrl()}`)
            .then(response => response.json())
            .then(({hub}) => {

              Promise.all(
                ["en", ...hub.supportedLanguages].map(code =>
                  fetch(`./i18n/${code}.json${getRandomParam()}`)
                    .then(response =>
                      response.headers.get("content-type").indexOf("application/json") !== -1
                        ? response.json()
                        : null
                    )
                    .catch(showGenericError)
                )
              )
                .then(translations => {

                  let resources = {};

                  ["en", ...hub.supportedLanguages].forEach((code, idx) => {
                    if (translations[idx]) {
                      resources[code] = {translation: translations[idx]}
                    }
                  });

                  let supportedAndTranslatedLanguages = Object.keys(resources);

                  if (supportedAndTranslatedLanguages.length < 2) {
                    alert("Unable to find translation file for at least one configured language. Please check app configuration and translations file.");
                  } else if (!hub.supportedLanguages.includes("en")) {
                    supportedAndTranslatedLanguages = supportedAndTranslatedLanguages.filter(lang => lang !== "en");
                    delete resources["en"];
                  }

                  const defaultLanguage =
                    resources[hub.defaultLanguage]
                      ? hub.defaultLanguage
                      : supportedAndTranslatedLanguages[0];

                  i18n
                    .use(initReactI18next)
                    .init({
                      lng: defaultLanguage,
                      resources,
                      returnEmptyString: false,
                      interpolation: {
                        escapeValue: false
                      }
                    });

                  const store = configureStore({
                    reducer: rootReducer,
                    middleware: [
                      userMiddlewareFactory(i18n.t.bind(i18n)),
                      requestMiddlewareFactory({
                        onGenericError: showTranslatedGenericErrorFactory(i18n.t.bind(i18n))
                      }),
                      requestSpinnerMiddlewareFactory(i18n.t.bind(i18n)),
                      fileSaveMiddleware,
                      persistenceMiddleware,
                      i18nMiddleware,
                      a11yMiddleware,
                      configMiddleware,
                      feedbackMiddlewareFactory(i18n.t.bind(i18n)),
                      fetchDatasetAsyncHandlerMiddlewareFactory(i18n.t.bind(i18n)),
                      fetchDashboardDatasetAsyncHandlerMiddlewareFactory(i18n.t.bind(i18n))
                    ]
                  });

                  store.dispatch(initAction(baseURL, supportedAndTranslatedLanguages, defaultLanguage, config));

                  cb(store);

                })
                .catch(showGenericError);
            })
            .catch(showGenericError);
        })
        .catch(showGenericError);
    })
    .catch(showGenericError);
};

export default init;