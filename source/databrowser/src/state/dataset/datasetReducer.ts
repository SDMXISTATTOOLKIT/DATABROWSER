import {Reducer} from "@reduxjs/toolkit";
import {REQUEST_ERROR, REQUEST_INIT, REQUEST_SUCCESS} from "../../middlewares/request/requestActions";
import {
  ALL_FULL,
  ALL_PARTIAL,
  CRITERIA_SELECTION_MODE_ALL,
  CRITERIA_SELECTION_MODE_STEP_BY_STEP,
  CRITERIA_SELECTION_TYPE_DYNAMIC,
  CRITERIA_SELECTION_TYPE_FULL,
  CRITERIA_SELECTION_TYPE_PARTIAL,
  DATASET_CHART_COLORS_SET,
  DATASET_CHART_FILTERS_TREE_SUBMIT,
  DATASET_CHART_LAYOUT_SUBMIT,
  DATASET_CHART_LEGEND_POSITION_SET,
  DATASET_CHART_STACKED_SET,
  DATASET_CRITERIA_ALERT_HIDE,
  DATASET_CRITERIA_HIDE,
  DATASET_CRITERIA_LENGTH_WARNING_HIDE,
  DATASET_CRITERIA_LENGTH_WARNING_SHOW,
  DATASET_CRITERIA_SHOW,
  DATASET_DOWNLOAD_SUBMIT,
  DATASET_DOWNLOAD_WARNING_HIDE,
  DATASET_FETCH_ENABLE,
  DATASET_GEOMETRIES_FETCH,
  DATASET_HTML_GENERATING_TIME_SET,
  DATASET_LABEL_FORMAT_SET,
  DATASET_MAP_DETAIL_LEVEL_SET,
  DATASET_MAP_LAYOUT_SUBMIT,
  DATASET_MAP_SETTINGS_SET,
  DATASET_STATE_BACKUP,
  DATASET_STATE_RESET,
  DATASET_STRUCTURE_CODELIST_CRITERIA_SET,
  DATASET_STRUCTURE_CODELIST_FETCH,
  DATASET_STRUCTURE_CODELIST_TIME_PERIOD_SET,
  DATASET_STRUCTURE_CODELISTS_FETCH,
  DATASET_STRUCTURE_FETCH,
  DATASET_STRUCTURE_FETCH_ENABLE,
  DATASET_TABLE_FILTERS_TREE_SUBMIT,
  DATASET_TABLE_LAYOUT_SUBMIT,
  DATASET_UNAVAILABLE_VIEW_WARNING_HIDE,
  DATASET_VIEW_ERROR_HIDE,
  DATASET_VIEW_TEMPLATE_GEOMETRIES_FETCH,
  DATASET_VIEW_TEMPLATE_HIDE,
  DATASET_VIEW_TEMPLATE_SHOW,
  DATASET_VIEW_TEMPLATE_SUBMIT,
  DATASET_VIEWER_SET,
  STEP_BY_STEP_FULL,
  STEP_BY_STEP_PARTIAL
} from "./datasetActions";
import {getTreeFromArray} from "../../utils/tree";
import {FREQ_DIMENSION_KEY, TIME_PERIOD_DIMENSION_KEY} from "../../utils/jsonStat";
import {LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME} from "../../components/label-format-selector/constants";
import {getViewerIdxFromType, getViewerTypeFromIdx} from "../../components/data-viewer";
import {localizeI18nObj} from "../../utils/i18n";
import {
  FETCH_DATASET_ASYNC_HANDLER_ERROR,
  FETCH_DATASET_ASYNC_HANDLER_INIT,
  FETCH_DATASET_ASYNC_HANDLER_SUCCESS
} from "../../middlewares/fetchDatasetAsyncHandler/actions";
import {
  CRITERIA_FILTER_TYPE_CODES,
  CRITERIA_FILTER_TYPE_PERIODS,
  CRITERIA_FILTER_TYPE_RANGE,
  getCriteriaArrayFromObject,
  getCriteriaObjectFromArray,
  getFreq,
  getTimePeriod
} from "../../utils/criteria";
import _ from "lodash";
import {CHART_LEGEND_POSITION_DEFAULT} from "../../components/chart";
import {TABLE_EMPTY_CHAR_DEFAULT} from "../../utils/formatters";
import {getGeometryDetailLevels} from "../../utils/other";
import {
  MAP_CLASSIFICATION_METHOD_DEFAULT,
  MAP_IS_LEGEND_COLLAPSED_DEFAULT,
  MAP_OPACITY_DEFAULT,
  MAP_PALETTE_CARDINALITY_DEFAULT,
  MAP_PALETTE_COLOR_END_DEFAULT,
  MAP_PALETTE_COLOR_START_DEFAULT
} from "../../components/map";

export const GENERATING_HTML_TIME_KEY = "generatingHtml";
export const OBSERVATION_COUNT_KEY = "observationCount";
export const SERVER_TIMINGS_KEY = "serverTimings";

const MAX_ALLOWED_CELLS = 1000000;

type TimePeriod = {
  selectorType: string,
  freq: string | null,
  minDate: string | null,
  maxDate: string | null,
  fromDate: string | null,
  toDate: string | null,
  periods: number | null,
  missingRange: boolean
};

const initialTimePeriod = {
  selectorType: CRITERIA_FILTER_TYPE_RANGE,
  freq: null,
  minDate: null,
  maxDate: null,
  fromDate: null,
  toDate: null,
  periods: null,
  missingRange: false
};

type DatasetState = {
  datasetId: string | null,
  nodeCode: string | null
  categoryPath: string[] | null,
  viewId: string | null,
  dataset: any | null,
  isPartialData: boolean,
  isEmptyData: boolean,
  isTooBigData: boolean,
  isCriteriaVisible: boolean,
  isCriteriaAlertVisible: boolean,
  dimensions: any[] | null,
  mode: string | null,
  type: string | null,
  viewerIdx: number,
  view: any | null,
  template: any | null,
  hasViewLayout: boolean,
  hasTemplateLayout: boolean,
  hasAnnotationLayout: boolean,
  tableLayout: any | null,
  mapLayout: any | null,
  chartLayout: any | null,
  tableFilterTree: any | null,
  mapFilterTree: any | null,
  chartFilterTree: any | null,
  labelFormat: string | null,
  criteria: object,
  initialCriteria: any | null,
  decimalSeparator: string | null,
  decimalPlaces: number | null
  tableEmptyChar: string,
  chartStacked: boolean,
  chartLegendPosition: string,
  chartColors: any,
  mapDetailLevel: number | null,
  mapClassificationMethod: string | null,
  mapPaletteStartColor: string | null,
  mapPaletteEndColor: string | null,
  mapPaletteCardinality: number | null,
  mapOpacity: number | null,
  mapIsLegendCollapsed: boolean | null,
  hasMapStartColor: boolean,
  hasMapEndColor: boolean,
  enableCriteria: boolean,
  enableLayout: boolean,
  maxAllowedCells: number,
  codes: any[] | null,
  isCodesFlat: boolean,
  codelists: any[] | null,
  areCodelistsFlat: boolean[],
  codelistsLength: number[] | null,
  timePeriod: TimePeriod,
  criteriaObsCount: number | null,
  timings: any | null,
  isFetchStructureDisabled: boolean,
  isFetchDatasetDisabled: boolean,
  isDownloadWarningVisible: boolean,
  isUnavailableViewWarningVisible: boolean,
  isCriteriaLengthWarningVisible: boolean,
  geometries: any | null,
  geometryDetailLevels: any | null,
  backup: any | null,
  isViewVisible: boolean,
  isViewErrorVisible: boolean,
  viewErrorMessage: string | null,
  isTemplateVisible: boolean,
  templateGeometries: any | null,
  templateGeometryDetailLevels: any | null
};

const initialState = {
  datasetId: null,
  nodeCode: null,
  categoryPath: null,
  viewId: null,
  dataset: null,
  isPartialData: false,
  isEmptyData: false,
  isTooBigData: false,
  isCriteriaVisible: false,
  isCriteriaAlertVisible: false,
  dimensions: null,
  mode: null,
  type: null,
  viewerIdx: 0,
  view: null,
  template: null,
  hasViewLayout: false,
  hasTemplateLayout: false,
  hasAnnotationLayout: false,
  tableLayout: null,
  mapLayout: null,
  chartLayout: null,
  tableFilterTree: null,
  mapFilterTree: null,
  chartFilterTree: null,
  labelFormat: null,
  criteria: {},
  initialCriteria: null,
  decimalSeparator: null,
  decimalPlaces: null,
  tableEmptyChar: TABLE_EMPTY_CHAR_DEFAULT,
  chartStacked: false,
  chartLegendPosition: CHART_LEGEND_POSITION_DEFAULT,
  chartColors: {},
  mapDetailLevel: null,
  mapClassificationMethod: MAP_CLASSIFICATION_METHOD_DEFAULT,
  mapPaletteStartColor: MAP_PALETTE_COLOR_START_DEFAULT,
  mapPaletteEndColor: MAP_PALETTE_COLOR_END_DEFAULT,
  mapPaletteCardinality: MAP_PALETTE_CARDINALITY_DEFAULT,
  mapOpacity: MAP_OPACITY_DEFAULT,
  mapIsLegendCollapsed: MAP_IS_LEGEND_COLLAPSED_DEFAULT,
  hasMapStartColor: false,
  hasMapEndColor: false,
  enableCriteria: true,
  enableLayout: true,
  maxAllowedCells: MAX_ALLOWED_CELLS,
  codes: null,
  isCodesFlat: false,
  codelists: null,
  areCodelistsFlat: [],
  codelistsLength: null,
  timePeriod: initialTimePeriod,
  criteriaObsCount: null,
  timings: null,
  isFetchStructureDisabled: true,
  isFetchDatasetDisabled: true,
  isDownloadWarningVisible: false,
  isUnavailableViewWarningVisible: false,
  isCriteriaLengthWarningVisible: false,
  geometries: null,
  geometryDetailLevels: null,
  backup: null,
  isViewVisible: false,
  isViewErrorVisible: false,
  viewErrorMessage: null,
  isTemplateVisible: false,
  templateGeometries: null,
  templateGeometryDetailLevels: null
};

const datasetReducer: Reducer<DatasetState> = (
  state = initialState,
  action
) => {
  switch (action.type) {
    case DATASET_STATE_BACKUP: {
      return {
        ...initialState,
        backup: {
          viewId: state.viewId,
          datasetId: state.datasetId,
          nodeCode: state.nodeCode,
          categoryPath: state.categoryPath,
          template: {
            defaultView: getViewerTypeFromIdx(state.viewerIdx),
            criteria: getCriteriaArrayFromObject(state.criteria),
            layouts: JSON.stringify({
              labelFormat: state.labelFormat,
              tableLayout: state.tableLayout,
              mapLayout: state.mapLayout,
              chartLayout: state.chartLayout,
              tableEmptyChar: state.tableEmptyChar,
              chartStacked: state.chartStacked,
              chartLegendPosition: state.chartLegendPosition,
              chartColors: state.chartColors,
              mapDetailLevel: state.mapDetailLevel,
              mapClassificationMethod: state.mapClassificationMethod,
              mapPaletteStartColor: state.mapPaletteStartColor,
              mapPaletteEndColor: state.mapPaletteEndColor,
              mapPaletteCardinality: state.mapPaletteCardinality,
              mapOpacity: state.mapOpacity,
              mapIsLegendCollapsed: state.mapIsLegendCollapsed
            })
          }
        }
      }
    }
    case DATASET_STATE_RESET: {
      return {
        ...initialState,
        backup: state.backup
      }
    }
    case DATASET_STRUCTURE_FETCH_ENABLE: {
      return {
        ...state,
        isFetchStructureDisabled: false,
        datasetId: action.payload.datasetId,
        nodeCode: action.payload.nodeCode,
        categoryPath: action.payload.categoryPath,
        viewId: (action.payload.viewId || null)
      }
    }
    case DATASET_FETCH_ENABLE: {
      return {
        ...state,
        isFetchDatasetDisabled: false,
        criteriaObsCount: null
      }
    }
    case DATASET_CRITERIA_SHOW: {
      return {
        ...state,
        isCriteriaVisible: true
      }
    }
    case DATASET_CRITERIA_HIDE: {
      return {
        ...state,
        isCriteriaVisible: false,
        criteria: state.initialCriteria,
        codes: null,
        isCodesFlat: false,
        codelists: null,
        areCodelistsFlat: [],
        timePeriod: initialTimePeriod
      }
    }
    case DATASET_CRITERIA_ALERT_HIDE: {
      return {
        ...state,
        isCriteriaAlertVisible: false
      }
    }
    case DATASET_STRUCTURE_CODELIST_CRITERIA_SET: {
      return {
        ...state,
        criteria: action.criteria,
        timePeriod: {
          ...state.timePeriod,
          freq: action.criteria[FREQ_DIMENSION_KEY]
            ? getFreq(action.criteria)
            : state.timePeriod.freq
        },
        criteriaObsCount: null
      }
    }
    case DATASET_STRUCTURE_CODELIST_TIME_PERIOD_SET: {
      let newCriteria = _.cloneDeep(state.criteria);
      const type = (action.timePeriod.selectorType || CRITERIA_FILTER_TYPE_RANGE);

      newCriteria = {
        ...newCriteria,
        [TIME_PERIOD_DIMENSION_KEY]: (type === CRITERIA_FILTER_TYPE_PERIODS && action.timePeriod.periods === null)
          ? undefined
          : {
            id: TIME_PERIOD_DIMENSION_KEY,
            type: type,
            filterValues: null,
            period: type === CRITERIA_FILTER_TYPE_PERIODS ? action.timePeriod.periods : null,
            from: type === CRITERIA_FILTER_TYPE_RANGE ? action.timePeriod.fromDate : null,
            to: type === CRITERIA_FILTER_TYPE_RANGE ? action.timePeriod.toDate : null
          }
      };

      return {
        ...state,
        criteria: newCriteria,
        timePeriod: action.timePeriod,
        criteriaObsCount: null
      }
    }
    case DATASET_VIEWER_SET: {
      return {
        ...state,
        viewerIdx: action.viewerIdx
      }
    }
    case DATASET_TABLE_LAYOUT_SUBMIT: {
      return {
        ...state,
        tableLayout: action.layout
      }
    }
    case DATASET_TABLE_FILTERS_TREE_SUBMIT: {
      return {
        ...state,
        tableFilterTree: action.filterTree
      }
    }
    case DATASET_MAP_LAYOUT_SUBMIT: {
      return {
        ...state,
        mapLayout: action.layout
      }
    }
    case DATASET_CHART_LAYOUT_SUBMIT: {
      return {
        ...state,
        chartLayout: action.layout
      }
    }
    case DATASET_CHART_FILTERS_TREE_SUBMIT: {
      return {
        ...state,
        chartFilterTree: action.filterTree
      }
    }
    case DATASET_LABEL_FORMAT_SET: {
      return {
        ...state,
        labelFormat: action.labelFormat
      }
    }
    case DATASET_CHART_STACKED_SET: {
      return {
        ...state,
        chartStacked: action.stacked
      }
    }
    case DATASET_CHART_LEGEND_POSITION_SET: {
      return {
        ...state,
        chartLegendPosition: action.legendPosition
      }
    }
    case DATASET_CHART_COLORS_SET: {
      return {
        ...state,
        chartColors: action.colors
      }
    }
    case DATASET_MAP_DETAIL_LEVEL_SET: {
      return {
        ...state,
        mapDetailLevel: action.detailLevel
      }
    }
    case DATASET_MAP_SETTINGS_SET: {
      return {
        ...state,
        mapClassificationMethod: (action.mapSettings.classificationMethod !== null && action.mapSettings.classificationMethod !== undefined)
          ? action.mapSettings.classificationMethod
          : state.mapClassificationMethod,
        mapPaletteStartColor: (action.mapSettings.paletteStartColor !== null && action.mapSettings.paletteStartColor !== undefined)
          ? action.mapSettings.paletteStartColor
          : state.mapPaletteStartColor,
        mapPaletteEndColor: (action.mapSettings.paletteEndColor !== null && action.mapSettings.paletteEndColor !== undefined)
          ? action.mapSettings.paletteEndColor
          : state.mapPaletteEndColor,
        mapPaletteCardinality: (action.mapSettings.paletteCardinality !== null && action.mapSettings.paletteCardinality !== undefined)
          ? action.mapSettings.paletteCardinality
          : state.mapPaletteCardinality,
        mapOpacity: (action.mapSettings.opacity !== null && action.mapSettings.opacity !== undefined)
          ? action.mapSettings.opacity
          : state.mapOpacity,
        mapIsLegendCollapsed: (action.mapSettings.isLegendCollapsed !== null && action.mapSettings.isLegendCollapsed !== undefined)
          ? action.mapSettings.isLegendCollapsed
          : state.mapIsLegendCollapsed
      }
    }
    case DATASET_DOWNLOAD_WARNING_HIDE: {
      return {
        ...state,
        isDownloadWarningVisible: false
      }
    }
    case DATASET_UNAVAILABLE_VIEW_WARNING_HIDE: {
      return {
        ...state,
        isUnavailableViewWarningVisible: false
      }
    }
    case DATASET_CRITERIA_LENGTH_WARNING_SHOW: {
      return {
        ...state,
        isCriteriaLengthWarningVisible: true,
        isFetchDatasetDisabled: true
      }
    }
    case DATASET_CRITERIA_LENGTH_WARNING_HIDE: {
      return {
        ...state,
        isCriteriaLengthWarningVisible: false
      }
    }
    case DATASET_HTML_GENERATING_TIME_SET: {
      return {
        ...state,
        timings: {
          ...state.timings,
          [GENERATING_HTML_TIME_KEY]: action.time
        }
      }
    }
    case FETCH_DATASET_ASYNC_HANDLER_INIT: {
      return {
        ...state,
        dataset: null,
        isFetchDatasetDisabled: true,
        geometries: null,
        geometryDetailLevels: null
      }
    }
    case FETCH_DATASET_ASYNC_HANDLER_SUCCESS: {
      if (action.payload.worker) {
        action.payload.worker.terminate();
      }
      return {
        ...state,
        dataset: (action.payload.response?.id || "").length !== 0 ? action.payload.response : null,

        isTooBigData: action.payload.isTooBigData,
        isEmptyData: action.payload.isEmptyData,
        isPartialData: action.payload.isPartialData,

        isCriteriaVisible: action.payload.isResponseNotValid,
        isCriteriaAlertVisible: action.payload.isResponseNotValid,

        tableLayout: action.payload.tableLayout,
        mapLayout: action.payload.mapLayout,
        chartLayout: action.payload.chartLayout,

        tableFilterTree: action.payload.tableFilterTree,
        mapFilterTree: action.payload.mapFilterTree,
        chartFilterTree: action.payload.chartFilterTree,

        initialCriteria: action.payload.isResponseNotValid ? state.initialCriteria : state.criteria,

        codes: action.payload.isResponseNotValid ? state.codes : null,
        isCodesFlat: action.payload.isResponseNotValid ? state.isCodesFlat : false,
        codelists: action.payload.isResponseNotValid ? state.codelists : null,
        areCodelistsFlat: action.payload.isResponseNotValid ? state.areCodelistsFlat : [],
        // timePeriod: action.payload.isResponseNotValid
        //   ? initialTimePeriod
        //   : state.timePeriod,
        isCriteriaLengthWarningVisible: false,
        timings: {
          [OBSERVATION_COUNT_KEY]: action.payload.response?.value ? Object.keys(action.payload.response.value).length : null,
          [SERVER_TIMINGS_KEY]: action.payload.responseHeaders.backendtimers
            ? JSON.parse(action.payload.responseHeaders.backendtimers)
            : null
        }
      }
    }
    case FETCH_DATASET_ASYNC_HANDLER_ERROR: {
      const isPayloadTooLarge = action.payload.statusCode === 413
      return {
        ...state,
        dataset: null,
        isPartialData: false,
        isEmptyData: false,
        isTooBigData: isPayloadTooLarge,
        isCriteriaAlertVisible: isPayloadTooLarge
      }
    }
    case DATASET_VIEW_TEMPLATE_SHOW: {
      return {
        ...state,
        isViewVisible: action.isView ? true : state.isViewVisible,
        isTemplateVisible: !action.isView ? true : state.isTemplateVisible
      }
    }
    case DATASET_VIEW_TEMPLATE_HIDE: {
      return {
        ...state,
        isViewVisible: action.isView ? false : state.isViewVisible,
        isTemplateVisible: !action.isView ? false : state.isTemplateVisible,
        templateGeometries: null,
        templateGeometryDetailLevels: null
      }
    }
    case DATASET_VIEW_ERROR_HIDE: {
      return {
        ...state,
        isViewErrorVisible: false,
        viewErrorMessage: null
      }
    }
    case REQUEST_INIT: {
      switch (action.payload.label) {
        case DATASET_STRUCTURE_FETCH: {
          return {
            ...state,
            isFetchStructureDisabled: true
          }
        }
        case DATASET_STRUCTURE_CODELIST_FETCH: {
          return {
            ...state,
            codes: null,
            isCodesFlat: false,
            criteriaObsCount: null
          }
        }
        default:
          return state
      }
    }
    case REQUEST_SUCCESS: {
      switch (action.payload.label) {
        case DATASET_STRUCTURE_FETCH: {
          const criteriaView = action.payload.response.criteriaView;

          const VIEW_KEY = "view";
          const TEMPLATE_KEY = "template";

          let viewTemplate = action.payload.response[VIEW_KEY]
            ? action.payload.response[VIEW_KEY]
            : action.payload.response[TEMPLATE_KEY]
              ? action.payload.response[TEMPLATE_KEY]
              : null;

          const hasBackup = state.backup !== null && (
            state.backup.nodeCode === state.nodeCode &&
            (state.backup.categoryPath || []).join() === (state.categoryPath || []).join() &&
            state.backup.datasetId === state.datasetId &&
            (state.backup.viewId || "") === (state.viewId || "")
          );

          if (hasBackup) {
            viewTemplate = {
              ...viewTemplate,
              ...state.backup?.template
            }
          }

          let viewTemplateLayouts = viewTemplate
            ? JSON.parse(viewTemplate.layouts)
            : null;

          if (action.payload.response[VIEW_KEY]) {
            if (action.payload.response[TEMPLATE_KEY]) {
              const templateLayouts = JSON.parse(action.payload.response[TEMPLATE_KEY].layouts);

              viewTemplate = {
                ...viewTemplate,
                enableCriteria: action.payload.response[TEMPLATE_KEY].enableCriteria,
                enableLayout: action.payload.response[TEMPLATE_KEY].enableLayout,
                decimalNumber: action.payload.response[TEMPLATE_KEY].decimalNumber,
                decimalSeparator: action.payload.response[TEMPLATE_KEY].decimalSeparator,
              }
              viewTemplateLayouts = {
                ...viewTemplateLayouts,
                tableLayout: (viewTemplateLayouts.tableLayout || templateLayouts.tableLayout),
                tableEmptyChar: templateLayouts.tableEmptyChar,
                mapLayout: (viewTemplateLayouts.mapLayout || templateLayouts.mapLayout),
                chartLayout: (viewTemplateLayouts.chartLayout || templateLayouts.chartLayout)
              }

            } else {
              viewTemplate = {
                ...viewTemplate,
                enableCriteria: true,
                enableLayout: true
              }
            }
          }

          const tableLayout = viewTemplateLayouts?.tableLayout
            ? viewTemplateLayouts.tableLayout
            : action.payload.response.layout
              ? action.payload.response.layout
              : null;

          const mapLayout = viewTemplateLayouts?.mapLayout
            ? viewTemplateLayouts.mapLayout
            : null;

          const chartLayout = viewTemplateLayouts?.chartLayout
            ? viewTemplateLayouts.chartLayout
            : action.payload.response.layoutChart
              ? {
                ...action.payload.response.layoutChart,
                filters: undefined
              }
              : null;

          let structureCriteria = viewTemplate
            ? viewTemplate.criteria
            : action.payload.response.filters
              ? action.payload.response.filters
              : null;

          const criteria = getCriteriaObjectFromArray(structureCriteria);

          let viewerIdx = 0;

          if (viewTemplate) {
            viewerIdx = getViewerIdxFromType(viewTemplate.defaultView);

          } else if (action.payload.response.defaultView) {
            const type = action.payload.response.defaultView;
            if ((type || "").toLowerCase() === "table") {
              viewerIdx = 0;
            } else if ((type || "").toLowerCase() === "map") {
              viewerIdx = 1;
            } else if ((type || "").toLowerCase() === "graph") {
              viewerIdx = 2;
            }
          }

          return {
            ...state,
            backup: null,
            hasViewLayout: !!action.payload.response[VIEW_KEY],
            hasTemplateLayout: !!action.payload.response[TEMPLATE_KEY],
            hasAnnotationLayout: !!action.payload.response.layout,
            view: action.payload.response[VIEW_KEY]
              ? {
                ...action.payload.response[VIEW_KEY],
                layouts: JSON.parse(action.payload.response[VIEW_KEY].layouts)
              }
              : null,
            template: action.payload.response[TEMPLATE_KEY]
              ? {
                ...action.payload.response[TEMPLATE_KEY],
                layouts: JSON.parse(action.payload.response[TEMPLATE_KEY].layouts)
              }
              : null,
            dimensions: action.payload.response.criteria,
            mode: (criteriaView === ALL_FULL || criteriaView === ALL_PARTIAL)
              ? CRITERIA_SELECTION_MODE_ALL
              : CRITERIA_SELECTION_MODE_STEP_BY_STEP,
            type: (criteriaView === ALL_FULL || criteriaView === STEP_BY_STEP_FULL)
              ? CRITERIA_SELECTION_TYPE_FULL
              : (criteriaView === ALL_PARTIAL || criteriaView === STEP_BY_STEP_PARTIAL)
                ? CRITERIA_SELECTION_TYPE_PARTIAL
                : CRITERIA_SELECTION_TYPE_DYNAMIC,
            codelistsLength: state.codelistsLength ? state.codelistsLength : action.payload.response.criteria.map(() => null),
            isCriteriaVisible: structureCriteria === null,
            viewerIdx: viewerIdx,
            isUnavailableViewWarningVisible: (!!action.payload.extra?.viewId && !action.payload.response?.[VIEW_KEY]),
            tableLayout: tableLayout,
            mapLayout: mapLayout,
            chartLayout: chartLayout,
            labelFormat: (viewTemplateLayouts?.labelFormat || LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME),
            criteria: criteria,
            initialCriteria: criteria,
            decimalSeparator: (Object.keys(viewTemplate?.decimalSeparator || {}).length > 0)
              ? localizeI18nObj(viewTemplate.decimalSeparator, action.payload.extra.defaultLanguage, action.payload.extra.languages)
              : action.payload.response.decimalSeparator,
            decimalPlaces: (viewTemplate?.decimalNumber || action.payload.response.decimalPlaces),
            tableEmptyChar: (viewTemplateLayouts?.tableEmptyChar !== null && viewTemplateLayouts?.tableEmptyChar !== undefined)
              ? viewTemplateLayouts.tableEmptyChar
              : (action.payload.response.emptyCellPlaceHolder !== null && action.payload.response.emptyCellPlaceHolder !== undefined)
                ? action.payload.response.emptyCellPlaceHolder
                : state.tableEmptyChar,
            chartStacked: (viewTemplateLayouts?.chartStacked !== null && viewTemplateLayouts?.chartStacked !== undefined)
              ? viewTemplateLayouts.chartStacked
              : state.chartStacked,
            chartLegendPosition: (viewTemplateLayouts?.chartLegendPosition !== null && viewTemplateLayouts?.chartLegendPosition !== undefined)
              ? viewTemplateLayouts.chartLegendPosition
              : state.chartLegendPosition,
            chartColors: (viewTemplateLayouts?.chartColors !== null && viewTemplateLayouts?.chartColors !== undefined)
              ? viewTemplateLayouts.chartColors
              : state.chartColors,
            mapDetailLevel: (viewTemplateLayouts?.mapDetailLevel !== null && viewTemplateLayouts?.mapDetailLevel !== undefined)
              ? viewTemplateLayouts.mapDetailLevel
              : state.mapDetailLevel,
            mapClassificationMethod: (viewTemplateLayouts?.mapClassificationMethod !== null && viewTemplateLayouts?.mapClassificationMethod !== undefined)
              ? viewTemplateLayouts.mapClassificationMethod
              : state.mapClassificationMethod,
            mapPaletteStartColor: (viewTemplateLayouts?.mapPaletteStartColor !== null && viewTemplateLayouts?.mapPaletteStartColor !== undefined)
              ? viewTemplateLayouts.mapPaletteStartColor
              : state.mapPaletteStartColor,
            mapPaletteEndColor: (viewTemplateLayouts?.mapPaletteEndColor !== null && viewTemplateLayouts?.mapPaletteEndColor !== undefined)
              ? viewTemplateLayouts.mapPaletteEndColor
              : state.mapPaletteEndColor,
            mapPaletteCardinality: (viewTemplateLayouts?.mapPaletteCardinality !== null && viewTemplateLayouts?.mapPaletteCardinality !== undefined)
              ? viewTemplateLayouts.mapPaletteCardinality
              : state.mapPaletteCardinality,
            mapOpacity: (viewTemplateLayouts?.mapOpacity !== null && viewTemplateLayouts?.mapOpacity !== undefined)
              ? viewTemplateLayouts.mapOpacity
              : state.mapOpacity,
            mapIsLegendCollapsed: false, // proprietà cablata per evitare doppia update
            // mapIsLegendCollapsed: (viewTemplateLayouts?.mapIsLegendCollapsed !== null && viewTemplateLayouts?.mapIsLegendCollapsed !== undefined)
            //   ? viewTemplateLayouts.mapIsLegendCollapsed
            //   : state.mapIsLegendCollapsed,
            hasMapStartColor: (viewTemplateLayouts?.mapPaletteStartColor !== null && viewTemplateLayouts?.mapPaletteStartColor !== undefined),
            hasMapEndColor: (viewTemplateLayouts?.mapPaletteEndColor !== null && viewTemplateLayouts?.mapPaletteEndColor !== undefined),
            enableCriteria: viewTemplate?.enableCriteria !== false,
            enableLayout: viewTemplate?.enableLayout !== false,
            isFetchDatasetDisabled: structureCriteria === null,
            maxAllowedCells: (action.payload.response.maxTableCells || MAX_ALLOWED_CELLS)
          }
        }
        case DATASET_STRUCTURE_CODELIST_FETCH: {

          const dimensionCodelist = action.payload.response.criteria[0] || {};

          let flatCodes = dimensionCodelist.values ? dimensionCodelist.values : [];
          const codelistIdx = dimensionCodelist.id ? (state.dimensions || []).map(({id}: any) => id).indexOf(dimensionCodelist.id) : -1;

          const newCodelistsLength = state.codelistsLength ? [...state.codelistsLength] : [];
          if (codelistIdx >= 0 && dimensionCodelist.id !== TIME_PERIOD_DIMENSION_KEY) {
            newCodelistsLength[codelistIdx] = flatCodes.length;
          }

          let isCodesFlat = true;
          flatCodes.forEach((item: any) => {
            if (isCodesFlat && item.parentId !== null && item.parentId !== undefined) {
              isCodesFlat = false;
            }
          });

          let newCriteria = _.cloneDeep(state.criteria);
          if (dimensionCodelist.id !== TIME_PERIOD_DIMENSION_KEY && flatCodes.length === 1) {
            newCriteria = {
              ...newCriteria,
              [dimensionCodelist.id]: {
                id: dimensionCodelist.id,
                type: CRITERIA_FILTER_TYPE_CODES,
                filterValues: [flatCodes[0].id],
              }
            }
          }

          return {
            ...state,
            criteria: newCriteria,
            codes: getTreeFromArray((flatCodes || []), "parentId", "children"),
            isCodesFlat: isCodesFlat,
            codelistsLength: newCodelistsLength,
            timePeriod: getTimePeriod(
              state.timePeriod,
              state.criteria,
              dimensionCodelist.id === TIME_PERIOD_DIMENSION_KEY ? dimensionCodelist : null,
              dimensionCodelist.id === FREQ_DIMENSION_KEY ? dimensionCodelist : null
            ),
            criteriaObsCount: (state.mode === CRITERIA_SELECTION_MODE_STEP_BY_STEP && state.type === CRITERIA_SELECTION_TYPE_DYNAMIC)
              ? (action.payload.response?.obsCount || null)
              : null
          }
        }
        case DATASET_STRUCTURE_CODELISTS_FETCH: {

          const dimensionCodelists = (state.dimensions || []).map(({id: dimensionId}) => {
            const found = action.payload.response.criteria.find((criteria: any) => criteria.id === dimensionId);
            return {
              id: dimensionId,
              values: (found?.values || [])
            }
          });

          const flatCodelists = dimensionCodelists ? dimensionCodelists.map(({values}: any) => (values || [])) : [];

          let areCodelistsFlat = flatCodelists.map(() => true);
          flatCodelists.forEach((codelist: any, idx: number) => {
            codelist.forEach((item: any) => {
              if (areCodelistsFlat[idx] && item.parentId !== null && item.parentId !== undefined) {
                areCodelistsFlat[idx] = false;
              }
            })
          })

          const timePeriodIdx = (state.dimensions || []).map(({id}: any) => id).indexOf(TIME_PERIOD_DIMENSION_KEY);
          const freqIdx = (state.dimensions || []).map(({id}: any) => id).indexOf(FREQ_DIMENSION_KEY);

          let newCriteria = _.cloneDeep(state.criteria);
          dimensionCodelists.forEach((dimensionCodelist, idx) => {
            if (dimensionCodelist.id !== TIME_PERIOD_DIMENSION_KEY && flatCodelists[idx].length === 1) {
              newCriteria = {
                ...newCriteria,
                [dimensionCodelist.id]: {
                  id: dimensionCodelist.id,
                  type: CRITERIA_FILTER_TYPE_CODES,
                  filterValues: [flatCodelists[idx][0].id],
                }
              }
            }
          });

          return {
            ...state,
            codelists: flatCodelists.map((flatCodes: any) => getTreeFromArray((flatCodes || []), "parentId", "children")),
            areCodelistsFlat: areCodelistsFlat,
            timePeriod: getTimePeriod(
              state.timePeriod,
              state.criteria,
              dimensionCodelists[timePeriodIdx],
              dimensionCodelists[freqIdx]
            ),
            codelistsLength: flatCodelists.map((codelist: any, idx: number) => idx !== timePeriodIdx ? codelist.length : null),
            criteria: newCriteria
          }
        }
        case DATASET_GEOMETRIES_FETCH: {
          return {
            ...state,
            geometries: action.payload.response,
            geometryDetailLevels: getGeometryDetailLevels(action.payload.response, true, action.payload.extra.t)
          }
        }
        case DATASET_VIEW_TEMPLATE_GEOMETRIES_FETCH: {
          return {
            ...state,
            templateGeometries: action.payload.response,
            templateGeometryDetailLevels: getGeometryDetailLevels(action.payload.response, true, action.payload.extra.t)
          }
        }
        case DATASET_VIEW_TEMPLATE_SUBMIT: {
          return {
            ...state,
            isViewVisible: false,
            isTemplateVisible: false,
            templateGeometries: null,
            templateGeometryDetailLevels: null
          }
        }
        default:
          return state
      }
    }
    case REQUEST_ERROR: {
      switch (action.payload.label) {
        case DATASET_STRUCTURE_FETCH: {
          return {
            ...initialState,
            enableCriteria: false,
            enableLayout: false
          }
        }
        case DATASET_STRUCTURE_CODELIST_FETCH: {
          return {
            ...state
          }
        }
        case DATASET_STRUCTURE_CODELISTS_FETCH: {
          return {
            ...state
          }
        }
        case DATASET_GEOMETRIES_FETCH: {
          return {
            ...state,
            dataset: null,
            geometries: null,
            geometryDetailLevels: null
          }
        }
        case DATASET_DOWNLOAD_SUBMIT: {
          return {
            ...state,
            isDownloadWarningVisible: action.payload.statusCode === 406
          }
        }
        case DATASET_VIEW_TEMPLATE_SUBMIT: {
          return {
            ...state,
            isViewErrorVisible: !!(action.payload.extra.isView && action.payload.statusCode === 409 && action.payload.response),
            viewErrorMessage: action.payload.response
          }
        }
        default:
          return state
      }
    }
    default:
      return state
  }
};

export default datasetReducer;