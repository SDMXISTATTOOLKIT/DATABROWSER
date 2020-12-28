export const NSI_RESPONSE_DOWNLOAD_SIZE_KEY = "nsiResponseDownloadSize";
export const TOTAL_KEY = "total";

export const getTimingsLabelTranslations = t => {

  const translations = t => ({
    nsiResponse: t("timings.nsiResponse"),
    nsiResponseDownload: t("timings.nsiResponseDownload"),
    [NSI_RESPONSE_DOWNLOAD_SIZE_KEY]: t("timings.nsiResponseDownloadSize"),
    jsonToJsonStat: t("timings.jsonToJsonStat"),
    xmlToJsonStat: t("timings.xmlToJsonStat"),
    others: t("timings.others"),
    [TOTAL_KEY]: t("timings.total"),
    cacheRead: t("timings.cacheRead")
  });

  return translations(t !== undefined ? t : str => str);
};