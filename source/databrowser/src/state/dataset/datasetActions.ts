import {initRequest, RequestMethod} from "../../middlewares/request/requestActions";
import {
  getCreateTemplateUrl,
  getCreateViewUrl,
  getDatasetDownloadUrl,
  getDatasetStructureCodelistsUrl,
  getDatasetStructureCodelistUrl,
  getDatasetStructureDynamicCodelistUrl,
  getDatasetStructureUrl,
  getDatasetUrl,
  getGeometryUrl
} from "../../serverApi/urls";
import {getCriteriaArrayFromObject} from "../../utils/criteria";

export const DATASET_STRUCTURE_FETCH_ENABLE = "dataset/structure/fetch/enable";
export const DATASET_STRUCTURE_FETCH = "dataset/structure/fetch";
export const DATASET_FETCH_ENABLE = "dataset/fetch/enable";
export const DATASET_FETCH = "dataset/fetch";
export const DATASET_CRITERIA_LENGTH_WARNING_SHOW = "dataset/criteria/length/warning/show";
export const DATASET_CRITERIA_LENGTH_WARNING_HIDE = "dataset/criteria/length/warning/hide";
export const DATASET_CRITERIA_SHOW = "dataset/criteria/show";
export const DATASET_CRITERIA_HIDE = "dataset/criteria/hide";
export const DATASET_CRITERIA_ALERT_HIDE = "dataset/criteria/alert/hide";
export const DATASET_STRUCTURE_CODELIST_FETCH = "dataset/structure/codelist/fetch";
export const DATASET_STRUCTURE_CODELISTS_FETCH = "dataset/structure/codelists/fetch";
export const DATASET_STRUCTURE_CODELIST_CRITERIA_SET = "dataset/structure/codelist/criteria/set";
export const DATASET_STRUCTURE_CODELIST_TIME_PERIOD_SET = "dataset/structure/codelist/timePeriod/set";

export const DATASET_STATE_BACKUP = "dataset/state/backup";
export const DATASET_STATE_RESET = "dataset/state/reset";

export const DATASET_VIEWER_SET = "dataset/viewer/set";

export const DATASET_TABLE_LAYOUT_SUBMIT = "dataset/table/layout/submit";
export const DATASET_TABLE_FILTERS_TREE_SUBMIT = "dataset/table/filters/tree/submit";
export const DATASET_MAP_LAYOUT_SUBMIT = "dataset/map/layout/submit";
export const DATASET_CHART_LAYOUT_SUBMIT = "dataset/chart/layout/submit";
export const DATASET_CHART_FILTERS_TREE_SUBMIT = "dataset/chart/filters/tree/submit";

export const DATASET_GEOMETRIES_FETCH = "dataset/geometries/fetch";

export const DATASET_LABEL_FORMAT_SET = "dataset/label/format/set";
export const DATASET_CHART_STACKED_SET = "dataset/chart/stacked/set";
export const DATASET_CHART_LEGEND_POSITION_SET = "dataset/chart/legend/position/set";
export const DATASET_CHART_COLORS_SET = "dataset/chart/colors/set";
export const DATASET_MAP_DETAIL_LEVEL_SET = "dataset/map/detail/level/set";
export const DATASET_MAP_SETTINGS_SET = "dataset/map/settings/set";

export const DATASET_DOWNLOAD_SUBMIT = "dataset/download/submit";
export const DATASET_DOWNLOAD_WARNING_HIDE = "dataset/download/warning/hide";

export const DATASET_VIEW_TEMPLATE_SHOW = "dataset/view/template/show";
export const DATASET_VIEW_TEMPLATE_HIDE = "dataset/view/template/hide";
export const DATASET_VIEW_TEMPLATE_SUBMIT = "dataset/view/template/submit";
export const DATASET_VIEW_ERROR_HIDE = "dataset/view/error/hide";
export const DATASET_VIEW_TEMPLATE_GEOMETRIES_FETCH = "dataset/view/template/geometries/fetch";

export const DATASET_UNAVAILABLE_VIEW_WARNING_HIDE = "dataset/unavailable/view/warning/hide";

export const DATASET_HTML_GENERATING_TIME_SET = "dataset/html/generating/time/set";

export const ALL_FULL = "FullAll"
export const ALL_PARTIAL = "PartialAll"
export const STEP_BY_STEP_FULL = "FullStep"
export const STEP_BY_STEP_PARTIAL = "PartialStep"
export const STEP_BY_STEP_DYNAMIC = "Dynamic"

export const CRITERIA_SELECTION_MODE_STEP_BY_STEP = "stepByStep"
export const CRITERIA_SELECTION_MODE_ALL = "all"

export const CRITERIA_SELECTION_TYPE_FULL = "full"
export const CRITERIA_SELECTION_TYPE_PARTIAL = "partial"
export const CRITERIA_SELECTION_TYPE_DYNAMIC = "dynamic"

const ENDPOINT_STRING_LENGTH = 100;
const MAX_CRITERIA_STRING_LENGTH = 2000;

export const enableDatasetStructureFetch = (nodeCode: string, categoryPath: string[], datasetId: string, viewId: string) => ({
  type: DATASET_STRUCTURE_FETCH_ENABLE,
  payload: {
    datasetId: datasetId,
    nodeCode: nodeCode,
    categoryPath: categoryPath,
    viewId: viewId
  }
});

export const fetchDatasetStructure = (nodeId: number, datasetId: string, viewId: string, defaultLanguage: string, languages: string[]) => initRequest(
  DATASET_STRUCTURE_FETCH,
  getDatasetStructureUrl(nodeId, datasetId, viewId),
  RequestMethod.GET,
  undefined,
  t => ({
    onStart: t("scenes.dashboard.actions.fetchingDatasetStructure")
  }),
  {
    viewId: viewId,
    defaultLanguage: defaultLanguage,
    languages: languages
  }
);

export const enableDatasetFetch = () => ({
  type: DATASET_FETCH_ENABLE
});

export const fetchDataset = (nodeId: number, datasetId: string, criteria: any, datasetTitle: string, checkCriteriaLength: boolean, tableLayout: any, chartLayout: any, mapLayout: any, maxAllowedCells: number, hasAnnotationLayout: boolean) => {
  return (checkCriteriaLength && ((JSON.stringify(criteria).length + ENDPOINT_STRING_LENGTH) > MAX_CRITERIA_STRING_LENGTH))
    ? {
      type: DATASET_CRITERIA_LENGTH_WARNING_SHOW
    }
    : initRequest(
      DATASET_FETCH,
      getDatasetUrl(nodeId, datasetId),
      RequestMethod.POST,
      getCriteriaArrayFromObject(criteria),
      t => ({
        onStart: t("scenes.dataset.actions.fetchingDataset", {title: datasetTitle})
      }),
      {
        tableLayout: tableLayout,
        chartLayout: chartLayout,
        mapLayout: mapLayout,
        maxAllowedCells: maxAllowedCells,
        hasAnnotationLayout: hasAnnotationLayout
      },
      "",
      (statusCode: number) => statusCode === 413
    );
};

export const hideDatasetCriteriaLengthWarning = () => ({
  type: DATASET_CRITERIA_LENGTH_WARNING_HIDE
});

export const showDatasetCriteria = () => ({
  type: DATASET_CRITERIA_SHOW
});

export const hideDatasetCriteria = () => ({
  type: DATASET_CRITERIA_HIDE
});

export const hideDatasetCriteriaAlert = () => ({
  type: DATASET_CRITERIA_ALERT_HIDE
});

export const fetchDatasetStructureCodelist = (nodeId: number, datasetId: string, dimensionId: string, type: string, criteria: any) => {

  let flatCriteria = getCriteriaArrayFromObject(criteria);
  // @ts-ignore
  flatCriteria = flatCriteria.filter(({id}) => id !== dimensionId)

  return type === CRITERIA_SELECTION_TYPE_DYNAMIC
    ? (
      initRequest(
        DATASET_STRUCTURE_CODELIST_FETCH,
        getDatasetStructureDynamicCodelistUrl(nodeId, datasetId, dimensionId),
        RequestMethod.POST,
        flatCriteria,
        t => ({
          onStart: t("scenes.dataset.actions.fetchingCodelist")
        })
      )
    )
    : (
      initRequest(
        DATASET_STRUCTURE_CODELIST_FETCH,
        getDatasetStructureCodelistUrl(nodeId, datasetId, dimensionId, type === CRITERIA_SELECTION_TYPE_FULL),
        undefined,
        undefined,
        t => ({
          onStart: t("scenes.dataset.actions.fetchingCodelist")
        })
      )
    );
};

export const fetchDatasetStructureCodelists = (nodeId: number, datasetId: string, type: string) => initRequest(
  DATASET_STRUCTURE_CODELISTS_FETCH,
  getDatasetStructureCodelistsUrl(nodeId, datasetId, type === CRITERIA_SELECTION_TYPE_FULL),
  undefined,
  undefined,
  t => ({
    onStart: t("scenes.dataset.actions.fetchingCodelists")
  })
);

export const setDatasetStructureCodelistCriteria = (criteria: any) => ({
  type: DATASET_STRUCTURE_CODELIST_CRITERIA_SET,
  criteria
});

export const setDatasetStructureCodelistTimePeriod = (timePeriod: any) => ({
  type: DATASET_STRUCTURE_CODELIST_TIME_PERIOD_SET,
  timePeriod
});

export const backupDatasetState = () => ({
  type: DATASET_STATE_BACKUP
});

export const resetDatasetState = () => ({
  type: DATASET_STATE_RESET
});

export const setDatasetViewer = (viewerIdx: number) => ({
  type: DATASET_VIEWER_SET,
  viewerIdx
});

export const submitDatasetTableLayout = (layout: any) => ({
  type: DATASET_TABLE_LAYOUT_SUBMIT,
  layout
});

export const submitDatasetTableFilterTree = (filterTree: any) => ({
  type: DATASET_TABLE_FILTERS_TREE_SUBMIT,
  filterTree
});

export const submitDatasetMapLayout = (layout: any) => ({
  type: DATASET_MAP_LAYOUT_SUBMIT,
  layout
});

export const submitDatasetChartLayout = (layout: any) => ({
  type: DATASET_CHART_LAYOUT_SUBMIT,
  layout
});

export const submitDatasetChartFilterTree = (filterTree: any) => ({
  type: DATASET_CHART_FILTERS_TREE_SUBMIT,
  filterTree
});

export const fetchDatasetGeometries = (idList: string[], t: any) => initRequest(
  DATASET_GEOMETRIES_FETCH,
  getGeometryUrl(),
  RequestMethod.POST,
  idList,
  t => ({
    onStart: t(`scenes.dataset.actions.fetchingGeometries`)
  }),
  {
    t
  }
);

export const setDatasetLabelFormat = (labelFormat: string) => ({
  type: DATASET_LABEL_FORMAT_SET,
  labelFormat
});

export const setDatasetChartStacked = (stacked: string) => ({
  type: DATASET_CHART_STACKED_SET,
  stacked
});

export const setDatasetChartLegendPosition = (legendPosition: string) => ({
  type: DATASET_CHART_LEGEND_POSITION_SET,
  legendPosition
});

export const setDatasetChartColors = (colors: string) => ({
  type: DATASET_CHART_COLORS_SET,
  colors
});

export const setDatasetMapDetailLevel = (detailLevel: number) => ({
  type: DATASET_MAP_DETAIL_LEVEL_SET,
  detailLevel
});

export const setDatasetMapSettings = (mapSettings: any) => ({
  type: DATASET_MAP_SETTINGS_SET,
  mapSettings
});

export const submitDatasetDownload = (nodeId: number, datasetId: string, datasetTitle: string, criteria: any, format: string, extension: string, zipped?: boolean) => initRequest(
  DATASET_DOWNLOAD_SUBMIT,
  getDatasetDownloadUrl(nodeId, datasetId, format, zipped),
  RequestMethod.POST,
  getCriteriaArrayFromObject(criteria),
  t => ({
    onStart: t("scenes.dataset.actions.downloadingDataset")
  }),
  {
    fileSave: {
      name: `${datasetTitle}.${extension}`,
      type: "text/plain;charset=utf-8"
    }
  },
  "",
  (statusCode: number) => statusCode === 406,
  undefined,
  undefined,
  format === "jsondata"
);

export const hideDatasetDownloadWarning = () => ({
  type: DATASET_DOWNLOAD_WARNING_HIDE
});

export const showDatasetViewTemplate = (isView: boolean) => ({
  type: DATASET_VIEW_TEMPLATE_SHOW,
  isView
});

export const hideDatasetViewTemplate = (isView: boolean) => ({
  type: DATASET_VIEW_TEMPLATE_HIDE,
  isView
});

export const submitDatasetViewTemplate = (nodeId: number, viewTemplate: any, isView: boolean) => initRequest(
  DATASET_VIEW_TEMPLATE_SUBMIT,
  isView
    ? getCreateViewUrl(nodeId)
    : getCreateTemplateUrl(nodeId),
  RequestMethod.POST,
  viewTemplate,
  t => ({
    onStart: isView
      ? t("scenes.dataset.actions.savingView")
      : t("scenes.dataset.actions.savingTemplate")
  }),
  {
    isView: isView
  },
  "",
  (statusCode: number) => statusCode === 409,
);

export const hideDatasetViewError = () => ({
  type: DATASET_VIEW_ERROR_HIDE
});

export const fetchDatasetViewTemplateGeometries = (idList: string[], t: any) => initRequest(
  DATASET_VIEW_TEMPLATE_GEOMETRIES_FETCH,
  getGeometryUrl(),
  RequestMethod.POST,
  idList,
  t => ({
    onStart: t(`scenes.dataset.actions.fetchingGeometries`)
  }),
  {
    t
  }
);

export const hideDatasetUnavailableViewWarning = () => ({
  type: DATASET_UNAVAILABLE_VIEW_WARNING_HIDE
});

export const setDatasetHtmlGeneratingTime = (time: number) => ({
  type: DATASET_HTML_GENERATING_TIME_SET,
  time
});