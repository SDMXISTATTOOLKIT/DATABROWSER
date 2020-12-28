import React, {Fragment, useEffect, useState} from 'react';
import withStyles from "@material-ui/core/styles/withStyles";
import {useTranslation} from "react-i18next";
import DataViewerHeader from "./Header";
import DataViewerSideBar from "./Sidebar";
import DataViewerFooter from "./Footer";
import DatasetFilters from "../dataset-filters";
import JsonStatTable, {
  JSONSTAT_TABLE_FONT_SIZE_LG,
  JSONSTAT_TABLE_FONT_SIZE_MD,
  JSONSTAT_TABLE_FONT_SIZE_SM
} from "../table";
import Criteria from "../criteria";
import TableLayout from "../table-layout";
import Dialog from "@material-ui/core/Dialog";
import Button from "@material-ui/core/Button";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import DialogTitle from "@material-ui/core/DialogTitle";
import Card from "@material-ui/core/Card";
import Grid from "@material-ui/core/Grid";
import FontSizeSelector, {
  FONT_SIZE_SELECTOR_FONT_SIZE_MD,
  FONT_SIZE_SELECTOR_FONT_SIZE_SM
} from "../font-size-selector";
import Tooltip from "@material-ui/core/Tooltip";
import IconButton from "@material-ui/core/IconButton";
import FullscreenIcon from '@material-ui/icons/Fullscreen';
import FullscreenExitIcon from '@material-ui/icons/FullscreenExit';
import "./style.css"
import CustomEmpty from "../custom-empty";
import TableChartIcon from "@material-ui/icons/TableChart";
import MapIcon from "@material-ui/icons/Map";
import BarChartIcon from "@material-ui/icons/BarChart";
import TimelineIcon from "@material-ui/icons/Timeline";
import DonutLargeIcon from "@material-ui/icons/DonutLarge";
import PieChartIcon from "@material-ui/icons/PieChart";
import AreaIcon from "../custom-icons/AreaIcon";
import RadarIcon from "../custom-icons/RadarIcon";
import PolarIcon from "../custom-icons/PolarIcon";
import PyramidIcon from "../custom-icons/PyramindIcon";
import {compose} from "redux";
import {connect} from "react-redux";
import {
  enableDatasetFetch,
  fetchDataset,
  fetchDatasetGeometries,
  hideDatasetCriteria,
  hideDatasetCriteriaAlert,
  hideDatasetCriteriaLengthWarning,
  hideDatasetDownloadWarning,
  hideDatasetUnavailableViewWarning,
  setDatasetChartColors,
  setDatasetChartLegendPosition,
  setDatasetChartStacked,
  setDatasetLabelFormat,
  setDatasetMapDetailLevel,
  setDatasetMapSettings,
  setDatasetViewer,
  showDatasetCriteria,
  submitDatasetChartFilterTree,
  submitDatasetChartLayout,
  submitDatasetDownload,
  submitDatasetMapLayout,
  submitDatasetTableFilterTree,
  submitDatasetTableLayout,
  submitDatasetViewTemplate
} from "../../state/dataset/datasetActions";
import Alert from "@material-ui/lab/Alert";
import {
  getDimensionFilterValues,
  getFilterTreeFromJsonStat,
  getInitialFiltersValue,
  getJsonStatTableSize,
  getUpdatedLayout
} from "../../utils/jsonStat";
import {numberFormatter} from "../../utils/formatters";
import Call from "../../hocs/call";
import ChartLayout from "../chart-layout";
import {v4 as uuidv4} from 'uuid';
import Map, {MAP_PALETTE_COLOR_END_DEFAULT, MAP_PALETTE_COLOR_START_DEFAULT} from "../map/index";
import LabelFormatSelector from "../label-format-selector";
import Chart from "../chart";
import SettingsIcon from "@material-ui/icons/Settings";
import ChartSettings from "../chart-settings";
import colorString from "color-string";

const SIDEBAR_WIDTH = 80;

const styles = theme => ({
  root: {
    position: "absolute",
    width: "100%"
  },
  header: {
    width: "100%"
  },
  page: {
    width: "100%"
  },
  sidebar: {
    display: "inline-block",
    verticalAlign: "top",
    width: SIDEBAR_WIDTH,
    height: "100%",
    overflow: "auto",
    marginLeft: theme.spacing(2),
    padding: "0 8px"
  },
  data: {
    display: "inline-block",
    verticalAlign: "top",
    width: `calc(100% - ${SIDEBAR_WIDTH + theme.spacing(2) + theme.spacing(2) + theme.spacing(2)}px)`,
    height: "100%",
    marginLeft: theme.spacing(2),
    marginRight: theme.spacing(2),
    padding: theme.spacing(2)
  },
  viewerContainer: {
    width: "100%",
    height: "100%"
  },
  filtersAndActions: {
    width: "100%",
    paddingBottom: theme.spacing(1)
  },
  filters: {
    display: "inline-block",
    verticalAlign: "top"
  },
  actions: {
    display: "inline-block",
    verticalAlign: "top"
  },
  action: {
    height: 40,
    "& button": {
      padding: 8
    }
  },
  viewer: {
    width: "100%",
    position: "relative"
  },
  attributes: {
    width: "100%",
    paddingTop: 12
  },
  footer: {
    bottom: 0,
    left: 0,
    width: "100%",
    overflowX: "auto",
    overflowY: "hidden",
    padding: theme.spacing(1)
  },
  warningAlert: {
    width: "100%",
    marginBottom: 16
  },
  criteriaContent: {
    overflow: "hidden !important"
  }
});

const mapStateToProps = ({dataset}) => ({
  dataset: dataset.dataset,
  isFetchDatasetDisabled: dataset.isFetchDatasetDisabled,
  isCriteriaVisible: dataset.isCriteriaVisible,
  isCriteriaAlertVisible: dataset.isCriteriaAlertVisible,
  hasViewLayout: dataset.hasViewLayout,
  hasTemplateLayout: dataset.hasTemplateLayout,
  hasAnnotationLayout: dataset.hasAnnotationLayout,
  dimensions: dataset.dimensions,
  mode: dataset.mode,
  type: dataset.type,
  codelistsLength: dataset.codelistsLength,
  viewerIdx: dataset.viewerIdx,
  tableLayout: dataset.tableLayout,
  mapLayout: dataset.mapLayout,
  chartLayout: dataset.chartLayout,
  tableFilterTree: dataset.tableFilterTree,
  mapFilterTree: dataset.mapFilterTree,
  chartFilterTree: dataset.chartFilterTree,
  labelFormat: dataset.labelFormat,
  criteria: dataset.criteria,
  decimalSeparator: dataset.decimalSeparator,
  decimalPlaces: dataset.decimalPlaces,
  tableEmptyChar: dataset.tableEmptyChar,
  chartStacked: dataset.chartStacked,
  chartLegendPosition: dataset.chartLegendPosition,
  chartColors: dataset.chartColors,
  mapDetailLevel: dataset.mapDetailLevel,
  mapClassificationMethod: dataset.mapClassificationMethod,
  mapPaletteStartColor: dataset.mapPaletteStartColor,
  mapPaletteEndColor: dataset.mapPaletteEndColor,
  mapPaletteCardinality: dataset.mapPaletteCardinality,
  mapOpacity: dataset.mapOpacity,
  mapIsLegendCollapsed: dataset.mapIsLegendCollapsed,
  hasMapStartColor: dataset.hasMapStartColor,
  hasMapEndColor: dataset.hasMapEndColor,
  enableCriteria: dataset.enableCriteria,
  enableLayout: dataset.enableLayout,
  maxAllowedCells: dataset.maxAllowedCells,
  codes: dataset.codes,
  isCodesFlat: dataset.isCodesFlat,
  codelists: dataset.codelists,
  areCodelistsFlat: dataset.areCodelistsFlat,
  timePeriod: dataset.timePeriod,
  criteriaObsCount: dataset.criteriaObsCount,
  geometries: dataset.geometries,
  geometryDetailLevels: dataset.geometryDetailLevels,
  isPartialData: dataset.isPartialData,
  isEmptyData: dataset.isEmptyData,
  isTooBigData: dataset.isTooBigData,
  isDownloadWarningVisible: dataset.isDownloadWarningVisible,
  isUnavailableViewWarningVisible: dataset.isUnavailableViewWarningVisible,
  isCriteriaLengthWarningVisible: dataset.isCriteriaLengthWarningVisible,
  timings: dataset.timings
});

const mapDispatchToProps = dispatch => ({
  onDatasetFetch: ({nodeId, datasetId, criteria, datasetTitle, checkCriteriaLength, tableLayout, chartLayout, mapLayout, maxAllowedCells, hasAnnotationLayout}) =>
    dispatch(fetchDataset(nodeId, datasetId, criteria, datasetTitle, checkCriteriaLength, tableLayout, chartLayout, mapLayout, maxAllowedCells, hasAnnotationLayout)),
  onFetchDatasetEnable: () => dispatch(enableDatasetFetch()),
  onCriteriaShow: () => dispatch(showDatasetCriteria()),
  onCriteriaHide: () => dispatch(hideDatasetCriteria()),
  onCriteriaAlertHide: () => dispatch(hideDatasetCriteriaAlert()),
  onViewerSet: viewerIdx => dispatch(setDatasetViewer(viewerIdx)),
  onTableLayoutSet: layout => dispatch(submitDatasetTableLayout(layout)),
  onTableFilterTreeSet: filterTree => dispatch(submitDatasetTableFilterTree(filterTree)),
  onMapLayoutSet: layout => dispatch(submitDatasetMapLayout(layout)),
  onChartLayoutSet: layout => dispatch(submitDatasetChartLayout(layout)),
  onChartFilterTreeSet: filterTree => dispatch(submitDatasetChartFilterTree(filterTree)),
  onLabelFormatSet: labelFormat => dispatch(setDatasetLabelFormat(labelFormat)),
  onChartStackedSet: labelFormat => dispatch(setDatasetChartStacked(labelFormat)),
  onChartLegendPositionSet: labelFormat => dispatch(setDatasetChartLegendPosition(labelFormat)),
  onChartColorsSet: labelFormat => dispatch(setDatasetChartColors(labelFormat)),
  onMapDetailLevelSet: detailLevel => dispatch(setDatasetMapDetailLevel(detailLevel)),
  onMapSettingsSet: mapSettings => dispatch(setDatasetMapSettings(mapSettings)),
  onDownloadSubmit: (nodeId, datasetId, datasetTitle, criteria, format, extension) =>
    dispatch(submitDatasetDownload(nodeId, datasetId, datasetTitle, criteria, format, extension)),
  onDownloadHide: () => dispatch(hideDatasetDownloadWarning()),
  onViewTemplateSubmit: (nodeId, viewTemplate, isView) => dispatch(submitDatasetViewTemplate(nodeId, viewTemplate, isView)),
  onUnavailableViewHide: () => dispatch(hideDatasetUnavailableViewWarning()),
  onCriteriaLengthWarningHide: () => dispatch(hideDatasetCriteriaLengthWarning()),
  onGeometryFetch: (idList, t) => dispatch(fetchDatasetGeometries(idList, t))
});

const viewers = t => [
  {
    key: 0,
    type: "table",
    title: t ? t("scenes.dataViewers.viewers.table") : "",
    icon: <TableChartIcon/>
  },
  {
    key: 1,
    title: t ? t("scenes.dataViewers.viewers.map") : "",
    type: "map",
    icon: <MapIcon/>
  },
  {
    key: 2,
    title: t ? t("scenes.dataViewers.viewers.verticalBar") : "",
    type: "bar",
    icon: <BarChartIcon/>,
    chartType: "bar"
  },
  {
    key: 3,
    title: t ? t("scenes.dataViewers.viewers.horizontalBar") : "",
    type: "horizontalBar",
    icon: <BarChartIcon style={{transform: "rotate(90deg)"}}/>,
    chartType: "horizontalBar"
  },
  {
    key: 4,
    title: t ? t("scenes.dataViewers.viewers.line") : "",
    type: "line",
    icon: <TimelineIcon/>,
    chartType: "line"
  },
  {
    key: 5,
    title: t ? t("scenes.dataViewers.viewers.area") : "",
    type: "area",
    icon: <AreaIcon/>,
    chartType: "area"
  },
  {
    key: 6,
    title: t ? t("scenes.dataViewers.viewers.doughnut") : "",
    type: "doughnut",
    icon: <DonutLargeIcon/>,
    chartType: "doughnut"
  },
  {
    key: 7,
    title: t ? t("scenes.dataViewers.viewers.pie") : "",
    type: "pie",
    icon: <PieChartIcon/>,
    chartType: "pie"
  },
  {
    key: 8,
    title: t ? t("scenes.dataViewers.viewers.radar") : "",
    type: "radar",
    icon: <RadarIcon/>,
    chartType: "radar"
  },
  {
    key: 9,
    title: t ? t("scenes.dataViewers.viewers.polarArea") : "",
    type: "polarArea",
    icon: <PolarIcon/>,
    chartType: "polarArea"
  },
  {
    key: 10,
    title: t ? t("scenes.dataViewers.viewers.pyramid") : "",
    type: "pyramid",
    icon: <PyramidIcon/>,
    chartType: "pyramid",
    hidden: true
  }
];

export const getViewerIdxFromType = viewerType => {
  if (!viewerType) {
    return 0
  }

  const viewer = viewers().find(({type}) => type === viewerType);
  return viewer
    ? viewer.key
    : 0
};

export const getViewerTypeFromIdx = viewerIdx => viewers()[viewerIdx].type;

const handleStyle = () => {

  const nodeHeaderHeight = document.getElementById("node-header")
    ? document.getElementById("node-header").offsetHeight
    : 0;
  if (document.getElementById("data-viewer")) {
    document.getElementById("data-viewer").setAttribute("style", `height: calc(100% - ${nodeHeaderHeight}px); top: ${nodeHeaderHeight}px`);
  }

  const headerHeight = document.getElementById("data-viewer__header")
    ? document.getElementById("data-viewer__header").offsetHeight
    : 0;
  const footerHeight = document.getElementById("data-viewer__footer")
    ? document.getElementById("data-viewer__footer").offsetHeight
    : 0;
  if (document.getElementById("data-viewer__page")) {
    document.getElementById("data-viewer__page").setAttribute("style", `height: calc(100% - ${headerHeight + footerHeight}px)`);
  }

  const actionsWidth = document.getElementById("data-viewer__page__viewer__actions")
    ? document.getElementById("data-viewer__page__viewer__actions").offsetWidth + 5
    : 0;
  if (document.getElementById("data-viewer__page__viewer__filters")) {
    document.getElementById("data-viewer__page__viewer__filters").setAttribute("style", `width: calc(100% - ${actionsWidth}px)`);
  }

  const filtersAndActionsHeight = document.getElementById("data-viewer__page__viewer__filters-and-actions")
    ? document.getElementById("data-viewer__page__viewer__filters-and-actions").offsetHeight
    : 0;
  if (document.getElementById("data-viewer__page__viewer__viewer")) {
    document.getElementById("data-viewer__page__viewer__viewer").setAttribute("style", `height: calc(100% - ${filtersAndActionsHeight}px)`);
  }
};

function DataViewer(props) {
  const {
    classes,

    nodeId,
    nodeCode,
    datasetId,
    datasetTitle,
    viewId,
    notes,
    attachedFiles,
    referenceMetadataUrl,
    nodeExtras,
    maxObservation,

    dataset,
    isFetchDatasetDisabled,
    isCriteriaVisible,
    isCriteriaAlertVisible,
    hasViewLayout,
    hasTemplateLayout,
    hasAnnotationLayout,
    dimensions,
    mode,
    type,
    codelistsLength,
    viewerIdx,
    tableLayout,
    mapLayout,
    chartLayout,
    tableFilterTree,
    mapFilterTree,
    chartFilterTree,
    labelFormat,
    criteria,
    decimalSeparator,
    decimalPlaces,
    tableEmptyChar,
    chartStacked,
    chartLegendPosition,
    chartColors,
    mapDetailLevel,
    mapClassificationMethod,
    mapPaletteStartColor,
    mapPaletteEndColor,
    mapPaletteCardinality,
    mapOpacity,
    mapIsLegendCollapsed,
    hasMapStartColor,
    hasMapEndColor,
    enableCriteria,
    enableLayout,
    maxAllowedCells,
    codes,
    isCodesFlat,
    codelists,
    areCodelistsFlat,
    timePeriod,
    criteriaObsCount,
    geometries,
    geometryDetailLevels,
    isPartialData,
    isEmptyData,
    isTooBigData,
    isDownloadWarningVisible,
    isUnavailableViewWarningVisible,
    isCriteriaLengthWarningVisible,
    timings,

    onDatasetFetch,
    onFetchDatasetEnable,
    onCriteriaShow,
    onCriteriaHide,
    onCriteriaAlertHide,
    onViewerSet,
    onTableLayoutSet,
    onTableFilterTreeSet,
    onMapLayoutSet,
    onChartLayoutSet,
    onChartFilterTreeSet,
    onLabelFormatSet,
    onChartStackedSet,
    onChartLegendPositionSet,
    onChartColorsSet,
    onMapDetailLevelSet,
    onMapSettingsSet,
    onDownloadSubmit,
    onDownloadHide,
    onViewTemplateSubmit,
    onUnavailableViewHide,
    onGeometryFetch,
    onCriteriaLengthWarningHide
  } = props;

  const {t} = useTranslation();

  const [tmpTableLayout, setTmpTableLayout] = useState(null);
  const [tmpChartLayout, setTmpChartLayout] = useState(null);

  const [chartId] = useState("chart__" + uuidv4());

  const [mapId] = useState("map__" + uuidv4());

  const [isLayoutVisible, setLayoutVisibility] = useState(false);
  const [isCriteriaValid, setCriteriaValidity] = useState(true);

  const [isFullscreen, setFullscreen] = useState(false);
  const [fontSize, setFontSize] = useState(FONT_SIZE_SELECTOR_FONT_SIZE_MD);

  const [isChartSettingsVisible, setChartSettingsVisibility] = useState(false);
  const [tmpChartStacked, setTmpChartStacked] = useState(null);
  const [tmpChartLegendPosition, setTmpChartLegendPosition] = useState(null);
  const [tmpChartColors, setTmpChartColors] = useState(null);

  const [isObsCountWarningVisible, setIsObsCountWarningVisible] = useState(false);

  useEffect(() => {
    window.addEventListener("resize", handleStyle);
    return () => window.removeEventListener("resize", handleStyle)
  }, []);

  useEffect(() => {
    handleStyle();
  }, [dataset, viewerIdx, tableLayout, mapLayout, chartLayout, labelFormat]);

  useEffect(() => {
    setTmpTableLayout(tableLayout);
  }, [tableLayout]);

  useEffect(() => {
    setTmpChartLayout(chartLayout);
  }, [chartLayout]);

  useEffect(() => {
    setTmpChartStacked(chartStacked);
  }, [chartStacked]);

  useEffect(() => {
    setTmpChartLegendPosition(chartLegendPosition);
  }, [chartLegendPosition]);

  useEffect(() => {
    setTmpChartColors(chartColors);
  }, [chartColors]);

  // handling customCss map customization
  useEffect(() => {
    if (!hasMapStartColor || !hasMapEndColor) {

      const CUSTOM_CSS_MAP_SELECTOR_PREFIX = ".map__";
      const CUSTOM_CSS_MAP_SELECTOR_NODE_PREFIX = ".map__node__"

      const CUSTOM_CSS_MAP_SELECTOR_COLOR_START = "start-color"
      const CUSTOM_CSS_MAP_SELECTOR_COLOR_END = "end-color"

      fetch('./custom.css')
        .then(response => response.text())
        .then(css => {
          const doc = document.implementation.createHTMLDocument("");
          const styleEl = document.createElement("style");
          styleEl.textContent = String(css);
          doc.body.appendChild(styleEl);

          const colorRules = {};
          Object.values(styleEl.sheet.cssRules)
            .filter(({selectorText, style}) =>
              selectorText !== null &&
              selectorText !== undefined &&
              selectorText.startsWith(CUSTOM_CSS_MAP_SELECTOR_PREFIX) &&
              style[0] === "color" &&
              style.color.length > 0 &&
              colorString.get.rgb(style.color) !== null
            )
            .forEach(({selectorText, style}) => {
              colorRules[selectorText] = colorString.to.hex(colorString.get.rgb(style.color));
            });

          let start = null;
          if (!hasMapStartColor) {
            start = (nodeCode && colorRules[CUSTOM_CSS_MAP_SELECTOR_NODE_PREFIX + nodeCode + "__" + CUSTOM_CSS_MAP_SELECTOR_COLOR_START]) ||
              colorRules[CUSTOM_CSS_MAP_SELECTOR_PREFIX + CUSTOM_CSS_MAP_SELECTOR_COLOR_START] ||
              MAP_PALETTE_COLOR_START_DEFAULT;
          }

          let end = null;
          if (!hasMapEndColor) {
            end = (nodeCode && colorRules[CUSTOM_CSS_MAP_SELECTOR_NODE_PREFIX + nodeCode + "__" + CUSTOM_CSS_MAP_SELECTOR_COLOR_END]) ||
              colorRules[CUSTOM_CSS_MAP_SELECTOR_PREFIX + CUSTOM_CSS_MAP_SELECTOR_COLOR_END] ||
              MAP_PALETTE_COLOR_END_DEFAULT;
          }

          onMapSettingsSet({
            paletteStartColor: start,
            paletteEndColor: end
          });
        });
    }
  }, [nodeCode, hasMapStartColor, hasMapEndColor, onMapSettingsSet]);

  const handleSetViewer = viewerIdx => {
    onViewerSet(viewerIdx);
  };

  const handleSetData = dataIdx => {
    // fetch new data
  };

  const handleLayoutOpen = () => {
    setLayoutVisibility(true);
  };

  const handleLayoutClose = () => {
    setLayoutVisibility(false);
    setTmpTableLayout(tableLayout);
    setTmpChartLayout(chartLayout);
  };

  const handleLayoutSubmit = () => {
    setLayoutVisibility(false);

    if (viewerIdx === 0) {
      const newTableFilterTree = getFilterTreeFromJsonStat(tmpTableLayout, dataset);
      onTableLayoutSet({
        ...tmpTableLayout,
        filtersValue: getInitialFiltersValue(dataset, tmpTableLayout, newTableFilterTree)
      });
      onTableFilterTreeSet(newTableFilterTree);

    } else if (viewerIdx >= 2) {
      const newPrimaryDim = tmpChartLayout.primaryDim[0];
      const newSecondaryDim = tmpChartLayout.secondaryDim[0];
      const prevPrimaryDim = chartLayout.primaryDim[0];
      const prevSecondaryDim = chartLayout.secondaryDim[0];

      const isSecondaryDimChanged = (
        (!prevSecondaryDim && !!newSecondaryDim) ||
        (!!prevSecondaryDim && !newSecondaryDim) ||
        (!!prevSecondaryDim && !!newSecondaryDim && prevSecondaryDim !== newSecondaryDim)
      );

      const newChartFilterTree = getFilterTreeFromJsonStat(tmpChartLayout, dataset);
      if (newPrimaryDim !== prevPrimaryDim) {
        tmpChartLayout.primaryDimValues = dataset.dimension[newPrimaryDim].category.index;
        tmpChartLayout.secondaryDimValues = newSecondaryDim
          ? getDimensionFilterValues(newSecondaryDim, dataset, tmpChartLayout, newChartFilterTree)
          : []
      } else if (isSecondaryDimChanged) {
        tmpChartLayout.secondaryDimValues = newSecondaryDim
          ? getDimensionFilterValues(newSecondaryDim, dataset, tmpChartLayout, newChartFilterTree)
          : []
      }
      onChartLayoutSet({
        ...tmpChartLayout,
        filtersValue: getInitialFiltersValue(dataset, tmpChartLayout, newChartFilterTree)
      });
      onChartFilterTreeSet(newChartFilterTree);
    }
  };

  const handleCriteriaOpen = () => {
    onCriteriaShow();
  }

  const handleCriteriaClose = () => {
    onCriteriaHide();
    setCriteriaValidity(true);
  }

  const handleCriteriaSubmit = () => {
    onFetchDatasetEnable();
  }

  const handleFilterSelect = (dimension, value) => {
    if (viewerIdx === 0) {
      onTableLayoutSet(getUpdatedLayout(dimension, value, dataset, tableLayout, tableFilterTree));
    } else if (viewerIdx === 1) {
      onMapLayoutSet(getUpdatedLayout(dimension, value, dataset, mapLayout, mapFilterTree));
    } else if (viewerIdx >= 2) {
      onChartLayoutSet(getUpdatedLayout(dimension, value, dataset, chartLayout, chartFilterTree));
    }
  };

  const handleFontSizeSelect = size => {
    setFontSize(size);
  };

  const handleChartSettingsOpen = () => {
    setChartSettingsVisibility(true);
  };

  const handleChartSettingsClose = () => {
    setChartSettingsVisibility(false);
    setTmpChartStacked(chartStacked);
    setTmpChartLegendPosition(chartLegendPosition);
    setTmpChartColors(chartColors);
  };

  const handleChartSettingsSubmit = () => {
    setChartSettingsVisibility(false);
    onChartStackedSet(tmpChartStacked);
    onChartLegendPositionSet(tmpChartLegendPosition);
    onChartColorsSet(tmpChartColors);
  };

  const handleFullscreen = () => {
    !isFullscreen
      ? document.getElementById("data-viewer__page__viewer").classList.add("data-viewer__page__viewer--fullscreen")
      : document.getElementById("data-viewer__page__viewer").classList.remove("data-viewer__page__viewer--fullscreen");

    setFullscreen(!isFullscreen);
  };

  return (
    <div id="data-viewer" className={classes.root}>
      <Call
        cb={onDatasetFetch}
        cbParam={{
          nodeId,
          datasetId,
          criteria,
          datasetTitle,
          checkCriteriaLength: false,
          tableLayout,
          chartLayout,
          mapLayout,
          maxAllowedCells,
          hasAnnotationLayout
        }}
        disabled={!nodeId || !datasetId || !criteria || !datasetTitle || isFetchDatasetDisabled}
      >
        <div id="data-viewer__header" className={classes.header}>
          <DataViewerHeader
            nodeId={nodeId}
            nodeCode={nodeCode}
            nodeDownloadFormats={nodeExtras?.DownloadFormats}
            datasetId={datasetId}
            datasetTitle={datasetTitle}
            viewId={viewId}
            hasViewLayout={hasViewLayout}
            hasTemplateLayout={hasTemplateLayout}
            hasAnnotationLayout={hasAnnotationLayout}
            notes={notes}
            attachedFiles={attachedFiles}
            jsonStat={dataset}
            hiddenAttributes={nodeExtras?.HiddenAttributes}
            tableLayout={tableLayout}
            mapLayout={mapLayout}
            chartLayout={chartLayout}
            labelFormat={labelFormat}
            criteria={criteria}
            decimalSeparator={decimalSeparator}
            decimalPlaces={decimalPlaces}
            tableEmptyChar={tableEmptyChar}
            chartStacked={chartStacked}
            chartLegendPosition={chartLegendPosition}
            chartColors={chartColors}
            mapDetailLevel={mapDetailLevel}
            mapClassificationMethod={mapClassificationMethod}
            mapPaletteStartColor={mapPaletteStartColor}
            mapPaletteEndColor={mapPaletteEndColor}
            mapPaletteCardinality={mapPaletteCardinality}
            mapOpacity={mapOpacity}
            mapIsLegendCollapsed={mapIsLegendCollapsed}
            viewers={viewers(t)}
            viewerIdx={viewerIdx}
            onViewTemplateSubmit={(viewTemplate, isView) => onViewTemplateSubmit(nodeId, viewTemplate, isView)}
            onDownloadSubmit={(format, extension) => onDownloadSubmit(nodeId, datasetId, datasetTitle, criteria, format, extension)}
            timings={timings}
            chartId={chartId}
            mapId={mapId}
          />
        </div>
        <div id="data-viewer__page" className={classes.page}>
          <Card className={classes.sidebar}>
            <DataViewerSideBar
              viewers={viewers(t)}
              selected={viewerIdx}
              isCriteriaDisabled={!enableCriteria}
              isLayoutDisabled={!enableLayout || !dataset || !(!!tableLayout || !!mapLayout || !!chartLayout)}
              isTableDisabled={!dataset || !tableLayout}
              isChartDisabled={!dataset || !chartLayout}
              isMapDisabled={!dataset || !mapLayout}
              referenceMetadataUrl={referenceMetadataUrl}
              onViewerSelect={handleSetViewer}
              onCriteriaOpen={handleCriteriaOpen}
              onLayoutOpen={handleLayoutOpen}
            />
          </Card>
          <Card className={classes.data}>
            <div id="data-viewer__page__viewer" className={classes.viewerContainer}>
              <div id="data-viewer__page__viewer__filters-and-actions" className={classes.filtersAndActions}>
                {dataset && (
                  <Fragment>
                    {dataset && isPartialData && (
                      <Alert severity="warning" className={classes.warningAlert}>
                        {t("scenes.dataViewer.warnings.maxObservation.label", {maxObservation: maxObservation ? numberFormatter(maxObservation) : ""})}
                      </Alert>
                    )}
                    <div id="data-viewer__page__viewer__filters" className={classes.filters}>
                      {(() => {
                        let layout, filterTree;

                        if (viewerIdx === 0) {
                          layout = tableLayout;
                          filterTree = tableFilterTree;
                        } else if (viewerIdx === 1) {
                          layout = mapLayout;
                          filterTree = mapFilterTree;
                        } else {
                          layout = chartLayout;
                          filterTree = chartFilterTree;
                        }

                        return (
                          <DatasetFilters
                            key={uuidv4()}
                            jsonStat={dataset}
                            hiddenAttributes={nodeExtras?.HiddenAttributes}
                            layout={layout}
                            filterTree={filterTree}
                            labelFormat={labelFormat}
                            onSelect={handleFilterSelect}
                          />
                        )
                      })()}
                    </div>
                    <div id="data-viewer__page__viewer__actions" className={classes.actions}>
                      <Grid container>
                        <Grid item className={classes.action}>
                          <LabelFormatSelector
                            labelFormat={labelFormat}
                            setLabelFormat={labelFormat => onLabelFormatSet(labelFormat)}
                          />
                        </Grid>
                        {viewerIdx >= 2 && (
                          <Grid item className={classes.action}><Tooltip
                            title={t("scenes.dataViewer.actions.chartSettings")}
                          >
                            <IconButton onClick={handleChartSettingsOpen}>
                              <SettingsIcon/>
                            </IconButton>
                          </Tooltip>
                          </Grid>
                        )}
                        <Grid item className={classes.action}>
                          <FontSizeSelector
                            fontSize={fontSize}
                            setFontSize={handleFontSizeSelect}
                          />
                        </Grid>
                        <Grid item className={classes.action}>
                          <Tooltip
                            title={isFullscreen
                              ? t("scenes.dataViewer.actions.fullscreen.exit")
                              : t("scenes.dataViewer.actions.fullscreen.enter")
                            }
                          >
                            <IconButton onClick={handleFullscreen}>
                              {isFullscreen ? <FullscreenExitIcon/> : <FullscreenIcon/>}
                            </IconButton>
                          </Tooltip>
                        </Grid>
                      </Grid>
                    </div>
                  </Fragment>
                )}
              </div>
              <div id="data-viewer__page__viewer__viewer" className={classes.viewer}>
                {(() => {
                  if (!dataset) {
                    return isEmptyData
                      ? <CustomEmpty text={t("scenes.dataViewer.errors.emptyData")}/>
                      : <CustomEmpty text={t("scenes.dataViewer.errors.applyCriteria")}/>
                  }
                  if (viewerIdx === 0 && tableLayout) {
                    return !isTooBigData
                      ? (
                        <JsonStatTable
                          jsonStat={dataset}
                          layout={tableLayout}
                          isFullscreen={isFullscreen}
                          labelFormat={labelFormat}
                          fontSize={fontSize === FONT_SIZE_SELECTOR_FONT_SIZE_SM
                            ? JSONSTAT_TABLE_FONT_SIZE_SM
                            : fontSize === FONT_SIZE_SELECTOR_FONT_SIZE_MD
                              ? JSONSTAT_TABLE_FONT_SIZE_MD
                              : JSONSTAT_TABLE_FONT_SIZE_LG
                          }
                          decimalSeparator={decimalSeparator}
                          decimalPlaces={decimalPlaces}
                          emptyChar={tableEmptyChar}
                          hiddenAttributes={nodeExtras?.HiddenAttributes}
                        />
                      )
                      : (
                        <CustomEmpty
                          text={t("scenes.dataViewer.errors.tooBigData")}
                        />
                      )
                  } else if (viewerIdx === 1 && mapLayout) {
                    return (
                      <Map
                        mapId={mapId}
                        jsonStat={dataset}
                        layout={mapLayout}
                        geometries={geometries}
                        geometryDetailLevels={geometryDetailLevels}
                        onGeometryFetch={idList => onGeometryFetch(idList, t)}
                        detailLevel={mapDetailLevel}
                        setDetailLevel={onMapDetailLevelSet}
                        classificationMethod={mapClassificationMethod}
                        paletteStartColor={mapPaletteStartColor}
                        paletteEndColor={mapPaletteEndColor}
                        paletteCardinality={mapPaletteCardinality}
                        opacity={mapOpacity}
                        isLegendCollapsed={mapIsLegendCollapsed}
                        setSettings={onMapSettingsSet}
                        isFullscreen={isFullscreen}
                      />
                    )
                  } else if (viewerIdx >= 2 && chartLayout) {
                    return (
                      <Chart
                        chartId={chartId}
                        type={viewers(t)[viewerIdx].chartType}
                        jsonStat={dataset}
                        layout={chartLayout}
                        labelFormat={labelFormat}
                        decimalSeparator={decimalSeparator}
                        decimalPlaces={decimalPlaces}
                        stacked={chartStacked}
                        legendPosition={chartLegendPosition}
                        colors={chartColors}
                      />
                    )
                  }
                })()}
              </div>
            </div>
          </Card>
        </div>
        <div id="data-viewer__footer" className={classes.footer}>
          <DataViewerFooter
            labels={dataset ? [datasetTitle] : []}
            dataIdx={0}
            setData={handleSetData}
          />
        </div>
      </Call>

      <Dialog
        open={isCriteriaVisible}
        fullWidth
        maxWidth="md"
        onClose={handleCriteriaClose}
      >
        <DialogTitle>
          {t("scenes.dataViewer.dialogs.criteria.title")}
        </DialogTitle>
        <DialogContent className={classes.criteriaContent}>
          {dimensions && isCriteriaVisible && (
            <Fragment>

              <Criteria
                nodeId={nodeId}
                datasetId={datasetId}
                dimensions={dimensions}
                mode={mode}
                type={type}
                codes={codes}
                isCodesFlat={isCodesFlat}
                codelists={codelists}
                areCodelistsFlat={areCodelistsFlat}
                codelistsLength={codelistsLength}
                criteria={criteria}
                timePeriod={timePeriod}
                isCriteriaValid={isCriteriaValid}
                setCriteriaValidity={setCriteriaValidity}
              />

              <Dialog
                open={isCriteriaAlertVisible}
                fullWidth
                maxWidth="sm"
                onClose={onCriteriaAlertHide}
              >
                {(() => {
                  if (isEmptyData) {
                    return (
                      <DialogTitle>
                        {t("scenes.dataViewer.errors.emptyData")}
                      </DialogTitle>
                    )

                  } else if (isTooBigData) {
                    const count = getJsonStatTableSize(dataset);

                    return (
                      <Fragment>
                        <DialogTitle>
                          {t("scenes.dataViewer.dialogs.tooBigData.title")}
                        </DialogTitle>
                        <DialogContent>
                          {count !== 0 && (
                            <div style={{width: "100%", textAlign: "end"}}>
                              {t("scenes.dataViewer.dialogs.tooBigData.content.requestCells")}: <b>{numberFormatter(count)}</b>
                            </div>
                          )}
                          <div style={{width: "100%", textAlign: "end"}}>
                            {t("scenes.dataViewer.dialogs.tooBigData.content.maxCells")}: <b>{numberFormatter(maxAllowedCells)}</b>
                          </div>
                          <div style={{width: "100%", textAlign: "end", marginTop: 16}}>
                            {t("scenes.dataViewer.dialogs.tooBigData.content.applyCriteria")}
                          </div>
                        </DialogContent>
                      </Fragment>
                    )

                  } else {
                    return (
                      <DialogTitle>
                        {t("scenes.dataViewer.errors.genericError")}
                      </DialogTitle>
                    )
                  }
                })()}
                <DialogActions>
                  <Button autoFocus onClick={onCriteriaAlertHide}>
                    {t("commons.confirm.confirm")}
                  </Button>
                </DialogActions>
              </Dialog>

              <Dialog
                open={isObsCountWarningVisible}
                fullWidth
                maxWidth="sm"
                onClose={() => setIsObsCountWarningVisible(false)}
              >
                <DialogContent>
                  {t("scenes.dataViewer.dialogs.obsCountWarning.content")}
                </DialogContent>
                <DialogActions>
                  <Button onClick={() => setIsObsCountWarningVisible(false)}>
                    {t("commons.confirm.cancel")}
                  </Button>
                  <Button
                    autoFocus
                    onClick={() => {
                      setIsObsCountWarningVisible(false);
                      handleCriteriaSubmit();
                    }}
                  >
                    {t("commons.confirm.confirm")}
                  </Button>
                </DialogActions>
              </Dialog>

            </Fragment>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCriteriaClose}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button
            autoFocus
            color="primary"
            onClick={() => (criteriaObsCount && criteriaObsCount > maxObservation)
              ? setIsObsCountWarningVisible(true)
              : handleCriteriaSubmit()
            }
            disabled={!isCriteriaValid}
          >
            {t("commons.confirm.apply")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isLayoutVisible && viewerIdx === 0}
        fullWidth
        maxWidth="md"
        onClose={handleLayoutClose}
      >
        <DialogTitle>
          {t("scenes.dataViewer.dialogs.tableLayout.title")}
        </DialogTitle>
        <DialogContent>
          <TableLayout
            jsonStat={dataset}
            layout={tmpTableLayout}
            labelFormat={labelFormat}
            setLayout={setTmpTableLayout}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleLayoutClose}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button autoFocus onClick={handleLayoutSubmit} color="primary">
            {t("commons.confirm.apply")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isLayoutVisible && viewerIdx >= 2}
        fullWidth
        maxWidth="md"
        onClose={handleLayoutClose}
      >
        <DialogTitle>
          {t("scenes.dataViewer.dialogs.chartLayout.title")}
        </DialogTitle>
        <DialogContent>
          <ChartLayout
            jsonStat={dataset}
            layout={tmpChartLayout}
            setLayout={setTmpChartLayout}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleLayoutClose}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button autoFocus onClick={handleLayoutSubmit} color="primary">
            {t("commons.confirm.apply")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isDownloadWarningVisible}
        maxWidth="md"
        onClose={onDownloadHide}
      >
        <DialogContent>
          {t("scenes.dataViewer.dialogs.downloadFormat.content")}
        </DialogContent>
        <DialogActions>
          <Button onClick={onDownloadHide}>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isUnavailableViewWarningVisible}
        maxWidth="md"
        onClose={onUnavailableViewHide}
      >
        <DialogContent>
          {t("scenes.dataViewer.dialogs.unavailableView.content")}
        </DialogContent>
        <DialogActions>
          <Button onClick={onUnavailableViewHide}>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isCriteriaLengthWarningVisible}
        maxWidth="md"
        onClose={onCriteriaLengthWarningHide}
      >
        <DialogContent>
          {t("scenes.dataViewer.dialogs.tooLongQuery.content")}
        </DialogContent>
        <DialogActions>
          <Button onClick={onCriteriaLengthWarningHide}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button
            onClick={() => onDatasetFetch({
              nodeId,
              datasetId,
              criteria,
              datasetTitle,
              checkCriteriaLength: false,
              tableLayout,
              chartLayout,
              mapLayout,
              maxAllowedCells,
              hasAnnotationLayout
            })}
          >
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isChartSettingsVisible}
        maxWidth="md"
        fullWidth
        onClose={handleChartSettingsClose}
      >
        <DialogTitle>
          {t("scenes.dataViewer.dialogs.chartSettings.title")}
        </DialogTitle>
        <DialogContent>
          <ChartSettings
            jsonStat={dataset}
            isStacked={tmpChartStacked}
            onIsStackedSet={setTmpChartStacked}
            legendPosition={tmpChartLegendPosition}
            onLegendPositionSet={setTmpChartLegendPosition}
            colors={tmpChartColors}
            onColorsSet={setTmpChartColors}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleChartSettingsClose}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button autoFocus onClick={handleChartSettingsSubmit} color="primary">
            {t("commons.confirm.apply")}
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}

export default compose(
  withStyles(styles),
  connect(mapStateToProps, mapDispatchToProps)
)(DataViewer)