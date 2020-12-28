export const getDashboardDynamicFilterLabelTranslations = t => {

  const translations = t => ({
    Region: t('dashboards.dynamicFilterLabels.region'),
    Province: t('dashboards.dynamicFilterLabels.province'),
    Municipality: t('dashboards.dynamicFilterLabels.municipality'),
  });

  return translations(t !== undefined ? t : str => str);
};