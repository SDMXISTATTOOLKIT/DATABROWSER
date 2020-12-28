export const getNutsLevelTranslations = t => {

  const translations = t => ({
    0: t("nutsLevel0"),
    1: t("nutsLevel1"),
    2: t("nutsLevel2"),
    3: t("nutsLevel3"),
    4: t("nutsLevel4"),
    5: t("nutsLevel5"),
    6: t("nutsLevel6"),
    7: t("nutsLevel7"),
    8: t("nutsLevel8"),
    9: t("nutsLevel9")
  })

  return translations(t !== undefined ? t : str => str);
};