export const scrollResultsToDatasetByParam = () => {
  let datasetIdParam;

  const split = window.location.hash.split("datasetId=");
  if (split && split.length === 2 && split[1].length > 0) {
    const split2 = split[1].split("&");
    if (split2[0] && split2[0].length > 0) {
      datasetIdParam = decodeURIComponent(split2[0]);
    }
  }

  if (datasetIdParam) {
    const element = document.getElementById(encodeURIComponent(datasetIdParam));
    if (element) {
      element.scrollIntoView();
    }
  }
};