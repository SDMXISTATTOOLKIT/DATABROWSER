export const INIT = "INIT";

export const init = (baseURL: string, supportedLanguages: string[], defaultLanguage: string, appConfig: object) => ({
    type: INIT,
    payload: {
        baseURL,
        supportedLanguages,
        defaultLanguage,
        appConfig
    }
});
