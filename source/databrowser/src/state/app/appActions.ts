export const APP_LANGUAGE_SET = "app/languageSet";
export const APP_IS_A11Y_SET = "app/isA11ySet";

export const setAppLanguage = (language: string, nodeId: number) => ({
  type: APP_LANGUAGE_SET,
  payload: {
    language,
    nodeId
  }
});

export const setAppIsA11y = (isA11y: boolean) => ({
  type: APP_IS_A11Y_SET,
  payload: {
    isA11y
  }
});
