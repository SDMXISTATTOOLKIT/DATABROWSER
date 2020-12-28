export const downloadFormats = t => ({
  genericdata: {
    label: t ? t("commons.downloadFormat.genericdata") : "genericdata",
    extension: "xml"
  },
  genericdata20: {
    label: t ? t("commons.downloadFormat.genericdata20") : "genericdata20",
    extension: "xml"
  },
  compactdata: {
    label: t ? t("commons.downloadFormat.compactdata") : "compactdata",
    extension: "xml"
  },
  structurespecificdata: {
    label: t ? t("commons.downloadFormat.structurespecificdata") : "structurespecificdata",
    extension: "xml"
  },
  jsondata: {
    label: t ? t("commons.downloadFormat.jsondata") : "jsondata",
    extension: "json"
  },
  csv: {
    label: t ? t("commons.downloadFormat.csv") : "csv",
    extension: "csv"
  }
});

export const isDownloadFormatValid = format => downloadFormats()?.[format] !== undefined

export const getDownloadFormatLabels = t => {
  const labels = [];

  for (let format in downloadFormats) {
    if (downloadFormats.hasOwnProperty(format)) {
      labels.push(downloadFormats(t)[format].label);
    }
  }

  return labels;
};

export const getDownloadFormatLabelFromFormat = (format, t) => downloadFormats(t)[format]?.label;

export const getDownloadFormatExtensionFromFormat = (format, t) => downloadFormats(t)[format]?.extension;

export const exportChartJpeg = (canvasId, fileName) => {
  const canvas = document.getElementById(canvasId);

  const link = document.createElement("a");
  link.download = fileName;
  link.target = "_blank";
  link.href = canvas.toDataURL("image/jpeg");

  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};