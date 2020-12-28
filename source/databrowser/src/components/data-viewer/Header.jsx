import React, {Fragment, useEffect, useState} from 'react';
import {useTranslation} from "react-i18next";
import withStyles from "@material-ui/core/styles/withStyles";
import IconButton from "@material-ui/core/IconButton";
import InfoIcon from '@material-ui/icons/Info';
import ShareIcon from '@material-ui/icons/Share';
import GetAppIcon from '@material-ui/icons/GetApp';
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import DialogTitle from "@material-ui/core/DialogTitle";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import Tooltip from "@material-ui/core/Tooltip";
import {goToNode} from "../../links";
import HomeIcon from "@material-ui/icons/Home";
import AttachFileIcon from '@material-ui/icons/AttachFile';
import AccessTimeIcon from '@material-ui/icons/AccessTime';
import ButtonSelect from "../button-select";
import SaveIcon from '@material-ui/icons/Save';
import AttributeList from "../attribute-list";
import {
  getDimensionAttributeMap,
  getDimensionLabelFromJsonStat,
  getDimensionValueLabelFromJsonStat
} from "../../utils/jsonStat";
import {DECIMAL_PLACES_DEFAULT, DECIMAL_SEPARATOR_DEFAULT} from "../../utils/formatters";
import {
  canSaveAsView,
  canSaveTemplate,
  canShare,
  canViewTemplateOrAnnotationIcon,
  canViewTimesLog
} from "../../utils/user";
import {connect} from "react-redux";
import {compose} from "redux";
import {validateI18nObj} from "../../utils/i18n";
import {
  exportChartJpeg,
  getDownloadFormatExtensionFromFormat,
  getDownloadFormatLabelFromFormat,
  isDownloadFormatValid
} from "../../utils/download";
import DataViewerTimings from "./Timings";
import {LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME} from "../label-format-selector/constants";
import ViewBuilder from "./ViewBuilder";
import TemplateBuilder from "./TemplateBuilder";
import {getCriteriaArrayFromObject} from "../../utils/criteria";
import {
  fetchDatasetViewTemplateGeometries,
  hideDatasetViewError,
  hideDatasetViewTemplate,
  showDatasetViewTemplate
} from "../../state/dataset/datasetActions";

const styles = theme => ({
  root: {
    width: "100%",
    height: "100%",
    padding: "4px 16px 0"
  },
  titleContainer: {
    marginRight: 8
  },
  title: {
    fontSize: 20,
    fontWeight: 500
  },
  subtitle: {
    padding: "0 12px 8px 48px"
  },
  attributeIcon: {
    width: 24,
    height: 24,
    fontSize: 23,
    fontWeight: "bold",
    fontFamily: "Do Hyeon"
  },
  layoutIcon: {
    width: 48,
    height: 48,
    padding: 12,
    color: "rgba(0, 0, 0, 0.54)"
  },
  attributeId: {
    cursor: "default",
    fontSize: 13,
    color: "rgb(255, 255, 255)",
    backgroundColor: "rgb(136, 136, 136)",
    borderRadius: 3,
    padding: "0 4px"
  },
  attributeAst: {
    cursor: "default",
    fontSize: 15,
    color: "rgb(136, 136, 136)",
    fontFamily: "Do Hyeon"
  }
});

const mapStateToProps = state => ({
  user: state.user,
  defaultLanguage: state.app.language,
  languages: state.app.languages,
  view: state.dataset.view,
  template: state.dataset.template,
  isViewVisible: state.dataset.isViewVisible,
  isViewErrorVisible: state.dataset.isViewErrorVisible,
  viewErrorMessage: state.dataset.viewErrorMessage,
  isTemplateVisible: state.dataset.isTemplateVisible,
  templateGeometries: state.dataset.templateGeometries,
  templateGeometryDetailLevels: state.dataset.templateGeometryDetailLevels
});

const mapDispatchToProps = dispatch => ({
  onViewTemplateShow: isView => dispatch(showDatasetViewTemplate(isView)),
  onViewTemplateHide: isView => dispatch(hideDatasetViewTemplate(isView)),
  onViewErrorHide: isView => dispatch(hideDatasetViewError(isView)),
  onViewTemplateGeometriesFetch: (idList, t) => dispatch(fetchDatasetViewTemplateGeometries(idList, t))
});

const getEmptyViewTemplate = (viewTemplate, datasetId, decimalPlaces, decimalSeparator, defaultLanguage) => ({
  datasetId: datasetId,
  type: "",
  title: {},
  defaultView: "table",
  criteria: {},
  layouts: {
    labelFormat: LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME,
    tableDefaultLayout: "custom",
    tableLayout: {},
    mapLayout: {},
    chartLayout: {}
  },
  decimalNumber: (decimalPlaces !== null && decimalPlaces !== undefined) ? decimalPlaces : DECIMAL_PLACES_DEFAULT,
  decimalSeparator: {
    [defaultLanguage]: (decimalSeparator || DECIMAL_SEPARATOR_DEFAULT)
  },
  enableCriteria: true,
  enableLayout: true,
  ...viewTemplate
});

function DataViewerHeader(props) {

  const {
    classes,
    user,
    defaultLanguage,
    languages,
    nodeId,
    nodeCode,
    nodeDownloadFormats,
    datasetId,
    datasetTitle,
    viewId,
    hasViewLayout,
    hasTemplateLayout,
    hasAnnotationLayout,
    view: structureView,
    template: structureTemplate,
    notes,
    attachedFiles,
    jsonStat,
    hiddenAttributes,
    labelFormat,
    tableLayout,
    chartLayout,
    mapLayout,
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
    viewers,
    viewerIdx,
    isViewVisible,
    isViewErrorVisible,
    viewErrorMessage,
    isTemplateVisible,
    templateGeometries,
    templateGeometryDetailLevels,
    onViewTemplateShow,
    onViewTemplateHide,
    onViewTemplateSubmit,
    onViewErrorHide,
    onDownloadSubmit,
    onViewTemplateGeometriesFetch,
    timings,
    chartId,
    mapId
  } = props;

  const {t} = useTranslation();

  const [initialView, setInitialView] = useState(null);
  const [view, setView] = useState(null);

  const [initialTemplate, setInitialTemplate] = useState(null);
  const [template, setTemplate] = useState(null);

  const [isAttributesVisible, setAttributesVisibility] = useState(false);
  const [isNotesVisible, setNotesVisibility] = useState(false);
  const [isTimingsVisible, setTimingsVisibility] = useState(false);

  const [downloadFormats, setDownloadFormats] = useState([]);

  const [isUpdatingView, setIsUpdatingView] = useState(false);

  useEffect(() => {
    const actionsWidth = document.getElementById("dataset-viewer-header__actions")
      ? document.getElementById("dataset-viewer-header__actions").offsetWidth
      : 0;
    document.getElementById("dataset-viewer-header__title").setAttribute("style", `width: calc(100% - ${actionsWidth}px - 8px)`);
  });

  useEffect(() => {
    const template = getEmptyViewTemplate(structureTemplate, datasetId, decimalPlaces, decimalSeparator, defaultLanguage);
    setInitialTemplate(template);
    setTemplate(template)
    const view = getEmptyViewTemplate(structureView, datasetId, decimalPlaces, decimalSeparator, defaultLanguage);
    setInitialView(view);
    setView(view);
  }, [structureView, structureTemplate, datasetId, tableEmptyChar, decimalPlaces, decimalSeparator, defaultLanguage]);

  useEffect(() => {
    const newDownloadFormats = [];
    (nodeDownloadFormats || []).forEach(format => {
      if (isDownloadFormatValid(format)) {
        newDownloadFormats.push({
          label: getDownloadFormatLabelFromFormat(format, t),
          format: format,
          extension: getDownloadFormatExtensionFromFormat(format, t)
        })
      }
    });
    setDownloadFormats(newDownloadFormats);
  }, [nodeDownloadFormats, t]);

  const handleViewOpen = (defaultLanguage, isUpdate) => {
    onViewTemplateShow(true);
    setView({
      ...initialView,
      title: isUpdate
        ? view.title
        : {
          [defaultLanguage]: datasetTitle
        },
      defaultView: viewers[viewerIdx].type,
      criteria: criteria,
      layouts: {
        labelFormat: labelFormat,
        tableLayout: viewerIdx === 0 ? tableLayout : undefined,
        tableEmptyChar: viewerIdx === 0 ? tableEmptyChar : undefined,
        mapLayout: viewerIdx === 1 ? mapLayout : undefined,
        mapDetailLevel: viewerIdx === 1 ? mapDetailLevel : undefined,
        mapClassificationMethod: viewerIdx === 1 ? mapClassificationMethod : undefined,
        mapPaletteStartColor: viewerIdx === 1 ? mapPaletteStartColor : undefined,
        mapPaletteEndColor: viewerIdx === 1 ? mapPaletteEndColor : undefined,
        mapPaletteCardinality: viewerIdx === 1 ? mapPaletteCardinality : undefined,
        mapOpacity: viewerIdx === 1 ? mapOpacity : undefined,
        mapIsLegendCollapsed: viewerIdx === 1 ? mapIsLegendCollapsed : undefined,
        chartLayout: viewerIdx >= 2 ? chartLayout : undefined,
        chartStacked: viewerIdx >= 2 ? chartStacked : undefined,
        chartLegendPosition: viewerIdx >= 2 ? chartLegendPosition : undefined,
        chartColors: viewerIdx >= 2 ? chartColors : undefined
      }
    });
  };

  const handleViewClose = () => {
    onViewTemplateHide(true);
    setView(initialView);
  };

  const handleViewSubmit = viewId => {
    onViewTemplateSubmit({
      type: "view",
      viewTemplateId: viewId ? Number(viewId) : undefined,
      datasetId: view.datasetId,
      title: view.title,
      defaultView: view.defaultView,
      criteria: getCriteriaArrayFromObject(view.criteria),
      layouts: JSON.stringify(view.layouts)
    }, true);
    setView(initialView);
  };

  const handleTemplateOpen = (defaultLanguage, isUpdate) => {
    onViewTemplateShow(false);
    setTemplate({
      ...initialTemplate,
      title: isUpdate
        ? initialTemplate.title
        : {
          [defaultLanguage]: datasetTitle
        },
      criteria: criteria,
      layouts: {
        labelFormat: labelFormat,
        tableLayout: tableLayout,
        tableDefaultLayout: "custom",
        tableEmptyChar: tableLayout ? tableEmptyChar : undefined,
        mapLayout: mapLayout,
        mapDetailLevel: mapLayout ? mapDetailLevel : undefined,
        mapClassificationMethod: mapLayout ? mapClassificationMethod : undefined,
        mapPaletteStartColor: mapLayout ? mapPaletteStartColor : undefined,
        mapPaletteEndColor: mapLayout ? mapPaletteEndColor : undefined,
        mapPaletteCardinality: mapLayout ? mapPaletteCardinality : undefined,
        mapOpacity: mapLayout ? mapOpacity : undefined,
        mapIsLegendCollapsed: mapLayout ? mapIsLegendCollapsed : undefined,
        chartLayout: chartLayout,
        chartStacked: chartLayout ? chartStacked : undefined,
        chartLegendPosition: chartLayout ? chartLegendPosition : undefined,
        chartColors: chartLayout ? chartColors : undefined
      }
    });
  };

  const handleTemplateClose = () => {
    onViewTemplateHide(false);
    setTemplate(initialTemplate);
  };

  const handleTemplateSubmit = () => {
    onViewTemplateSubmit({
      ...template,
      type: "template",
      criteria: getCriteriaArrayFromObject(template.criteria),
      layouts: JSON.stringify({
        ...template.layouts,
        tableDefaultLayout: undefined
      })
    }, false);
    setTemplate(initialTemplate);
  };

  const datasetAttributes = [];
  const dataset = (jsonStat?.extension?.attributes?.dataSet || []);
  const datasetIndexes = (jsonStat?.extension?.attributes?.index?.dataSet || []);

  if (dataset && dataset.length > 0 && datasetIndexes && datasetIndexes.length > 0) {
    (datasetIndexes || []).forEach((el, idx) => {
      if (el !== null && !(hiddenAttributes || []).includes(dataset[idx].id)) {
        datasetAttributes.push({
          id: (dataset[idx].name || dataset[idx].id),
          value: (dataset[idx].values[el].name || dataset[idx].values[el].id)
        })
      }
    })
  }

  const otherAttributes = [];
  const series = (jsonStat?.extension?.attributes?.series || []);
  const seriesIndexes = (jsonStat?.extension?.attributes?.index?.series || []);

  if (series && series.length > 0 && seriesIndexes && seriesIndexes.length > 0) {
    seriesIndexes
      .filter(({coordinates}) => (coordinates || []).filter(c => c !== null).length > 1)
      .forEach(({coordinates, attributes}) => {
        const dimsId = [];
        const dimsValueId = [];
        coordinates.forEach((el, idx) => {
          if (el !== null) {
            const dim = jsonStat.id[idx];
            dimsId.push(dim);
            dimsValueId.push(jsonStat.dimension[dim].category.index[el]);
          }
        });
        const dims = dimsId.map((dim, idx) => ({
          id: getDimensionLabelFromJsonStat(jsonStat, dim, labelFormat),
          value: getDimensionValueLabelFromJsonStat(jsonStat, dim, dimsValueId[idx], labelFormat)
        }));
        const attrs = [];
        attributes.forEach((attr, idx) => {
          if (attr !== null && attr !== undefined && !(hiddenAttributes || []).includes(series[idx].id)) {
            attrs.push({
              id: (series[idx].name || series[idx].id),
              value: (series[idx].values[attr].name || series[idx].values[attr].id)
            })
          }
        });
        if (dims.length > 0 && attrs.length > 0) {
          otherAttributes.push({
            dims: dims,
            attributes: attrs
          });
        }
      });
  }

  const currentFilters = (viewers[viewerIdx].key === 0 && tableLayout?.filters)
    ? tableLayout.filters
    : (viewers[viewerIdx].key === 1 && mapLayout?.filters)
      ? mapLayout.filters
      : (viewers[viewerIdx].key >= 2 && chartLayout?.filters)
        ? chartLayout.filters
        : [];

  const subtitle = [];
  if (jsonStat && currentFilters) {
    currentFilters.forEach(filter => {
      if (jsonStat.size[jsonStat.id.indexOf(filter)] === 1) {
        subtitle.push({
          dim: filter,
          dimLabel: getDimensionLabelFromJsonStat(jsonStat, filter, labelFormat),
          value: jsonStat.dimension[filter].category.index[0],
          valueLabel: getDimensionValueLabelFromJsonStat(jsonStat, filter, jsonStat.dimension[filter].category.index[0], labelFormat)
        });
      }
    })
  }

  const dimensionAttributesMap = getDimensionAttributeMap(jsonStat, hiddenAttributes);

  return (
    <Fragment>
      <Grid container className={classes.root}>
        <Grid container justify="space-between" alignItems="flex-start">
          <Grid item id="dataset-viewer-header__title" className={classes.titleContainer}>
            <Grid container alignItems="flex-start">
              <Grid item>
                <Tooltip title={t("scenes.dataViewer.header.action.goHome.tooltip")}>
                  <IconButton color="primary" onClick={() => goToNode(nodeCode)}>
                    <HomeIcon/>
                  </IconButton>
                </Tooltip>
              </Grid>
              <Grid container style={{width: "calc(100% - 48px)"}}>
                <Grid item className={classes.title}>
                  <div style={{display: "table", minHeight: 48}}>
                    <div style={{display: "table-cell", verticalAlign: "middle"}}>
                      <span style={{marginRight: 4}}>{datasetTitle}</span>
                      {notes && notes.length > 0 && (
                        <Tooltip title={t("scenes.dataViewer.header.action.information.tooltip")}>
                          <IconButton id="dataset-attributes-btn" onClick={() => setNotesVisibility(true)}>
                            <InfoIcon/>
                          </IconButton>
                        </Tooltip>
                      )}
                      {datasetAttributes.concat(otherAttributes).length > 0 && (
                        <Tooltip title={t("scenes.dataViewer.header.action.attributes.tooltip")}>
                          <IconButton id="dataset-attributes-btn" onClick={() => setAttributesVisibility(true)}>
                            <div className={classes.attributeIcon}>(*)</div>
                          </IconButton>
                        </Tooltip>
                      )}
                    </div>
                  </div>
                </Grid>
              </Grid>
            </Grid>
          </Grid>
          <Grid item id="dataset-viewer-header__actions">
            <Grid container alignItems="center">
              {(() => {
                if (canViewTemplateOrAnnotationIcon(user, nodeId)) {

                  let title = null;
                  if (hasViewLayout) {
                    title = t("scenes.dataViewer.header.action.hasViewLayout.tooltip");
                  } else if (hasTemplateLayout) {
                    title = t("scenes.dataViewer.header.action.hasTemplateLayout.tooltip");
                  } else if (hasAnnotationLayout) {
                    title = t("scenes.dataViewer.header.action.hasAnnotationLayout.tooltip");
                  }

                  if (title) {
                    return (
                      <div className={classes.layoutIcon}>
                        <Tooltip title={title}>
                          <InfoIcon/>
                        </Tooltip>
                      </div>
                    )
                  }
                }
              })()}
              {canViewTimesLog(user, nodeId) && (
                <Grid item id="dataset-times-log-btn">
                  <Tooltip title={t("scenes.dataViewer.header.action.timesLog.tooltip")}>
                    <div>
                      <IconButton onClick={() => setTimingsVisibility(true)} disabled={timings === null}>
                        <AccessTimeIcon/>
                      </IconButton>
                    </div>
                  </Tooltip>
                </Grid>
              )}
              {(canSaveAsView(user) || canSaveTemplate(user, nodeId)) && (
                <Grid item id="dataset-save-btn">
                  <ButtonSelect
                    icon={<SaveIcon/>}
                    tooltip={t("scenes.dataViewer.header.action.save.tooltip")}
                    color="default"
                    disabled={!tableLayout && !chartLayout && !mapLayout}
                  >
                    {canSaveAsView(user) && (
                      <div
                        onClick={() => {
                          handleViewOpen(defaultLanguage, false);
                          setIsUpdatingView(false);
                        }}
                      >
                        {t("scenes.dataViewer.header.action.save.values.createView")}
                      </div>
                    )}
                    {canSaveAsView(user) && hasViewLayout && viewId && (
                      <div
                        onClick={() => {
                          handleViewOpen(defaultLanguage, true);
                          setIsUpdatingView(true);
                        }}
                      >
                        {t("scenes.dataViewer.header.action.save.values.updateView")}
                      </div>
                    )}
                    {canSaveTemplate(user, nodeId) && (
                      <div onClick={() => handleTemplateOpen(defaultLanguage, hasTemplateLayout)}>
                        {hasTemplateLayout
                          ? t("scenes.dataViewer.header.action.save.values.updateTemplate")
                          : t("scenes.dataViewer.header.action.save.values.createTemplate")
                        }
                      </div>
                    )}
                  </ButtonSelect>
                </Grid>
              )}
              {canShare(user) && (
                <Grid item id="dataset-share-btn">
                  <Tooltip title={t("scenes.dataViewer.header.action.share.tooltip")}>
                    <IconButton onClick={window.notImplemented.show}>
                      <ShareIcon/>
                    </IconButton>
                  </Tooltip>
                </Grid>
              )}
              <Grid item id="dataset-attachments-btn">
                <ButtonSelect
                  icon={<AttachFileIcon/>}
                  tooltip={t("scenes.dataViewer.header.action.attachments.tooltip")}
                  color="default"
                  onChange={({attachedFile}) => window.open(attachedFile.url)}
                  disabled={!attachedFiles || attachedFiles.length === 0}
                >
                  {(attachedFiles || []).map((attachedFile, idx) => {
                    const fileName = attachedFile.url.split("/").pop();
                    return (
                      <div key={idx} data-value={{attachedFile}}>
                        {`${fileName} (${attachedFile.format})`}
                      </div>
                    )
                  })}
                </ButtonSelect>
              </Grid>
              {(downloadFormats && downloadFormats.length > 0 && (
                <Grid item id="dataset-export-btn">
                  <ButtonSelect
                    icon={<GetAppIcon/>}
                    tooltip={t("scenes.dataViewer.header.action.export.tooltip")}
                    color="default"
                    onChange={({format, extension}) => {
                      if (format === "png" && viewerIdx === 1) {
                        window.LMap.exportPng(mapId, `${datasetTitle}.png`);
                      } else if (format === "jpeg" && viewerIdx >= 2) {
                        exportChartJpeg(chartId, `${datasetTitle}.jpeg`);
                      } else {
                        onDownloadSubmit(format, extension);
                      }
                    }}
                  >
                    {(downloadFormats)
                      .concat(viewerIdx === 1
                        ? {format: "png", label: "PNG", extension: "png"}
                        : viewerIdx >= 2
                          ? {format: "jpeg", label: "JPEG", extension: "jpeg"}
                          : []
                      )
                      .map(({format, label, extension}, idx) =>
                        <div key={idx} data-value={{format, extension}}>
                          {label}
                        </div>
                      )}
                  </ButtonSelect>
                </Grid>
              ))}
            </Grid>
          </Grid>
        </Grid>
        <Grid item className={classes.subtitle} xs={12}>
          {subtitle && dimensionAttributesMap && subtitle.map(({dim, dimLabel, value, valueLabel}, idx) =>
            <div key={idx} style={{display: "inline-block", marginRight: 8}}>
              <div style={{display: "inline-block"}}>
                <b>{(dimLabel || dim)}</b>: <i>{(valueLabel || value)}</i>
              </div>
              {dimensionAttributesMap[dim][value] && (
                <div style={{display: "inline-block", marginLeft: 4}}>
                  <Tooltip
                    title={
                      <div>
                        {dimensionAttributesMap[dim][value].attributes.map(({id, label, valueId, valueLabel}, idx) => (
                          <div
                            key={idx}>{`${label || id}: ${valueLabel || valueId}${valueLabel !== valueId ? ` [${valueId}]` : ''}`}</div>
                        ))}
                      </div>
                    }
                    placement="top"
                  >
                    {(() => {
                      const ids = dimensionAttributesMap[dim][value].ids;
                      if (ids && ids.length === 1 && ids[0].length <= 2) {
                        return <div className={classes.attributeId}>{ids[0]}</div>
                      } else {
                        return <div className={classes.attributeAst}>(*)</div>
                      }
                    })()}
                  </Tooltip>
                </div>
              )}
              <div style={{display: "inline-block"}}>
                {idx < subtitle.length - 1 ? "," : ""}
              </div>
            </div>
          )}
        </Grid>
      </Grid>

      <Dialog
        open={isAttributesVisible}
        fullWidth
        maxWidth="md"
        onClose={() => setAttributesVisibility(false)}
      >
        <DialogTitle>
          {t("scenes.dataViewer.header.dialogs.attributes.title")}
        </DialogTitle>
        <DialogContent>
          <AttributeList
            datasetAttributes={datasetAttributes}
            otherAttributes={otherAttributes}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAttributesVisibility(false)}>
            {t("commons.confirm.close")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isTimingsVisible}
        onClose={() => setTimingsVisibility(false)}
      >
        <DialogContent style={{width: 400}}>
          <DataViewerTimings
            timings={timings}
          />
        </DialogContent>
        <DialogActions>
          <Button autoFocus onClick={() => setTimingsVisibility(false)}>
            {t("commons.confirm.close")}
          </Button>
        </DialogActions>
      </Dialog>

      {initialView && (
        <Fragment>
          <Dialog
            open={isViewVisible}
            onClose={handleViewClose}
          >
            <DialogTitle>
              {isUpdatingView
                ? t("scenes.dataViewer.header.dialogs.view.title.update")
                : t("scenes.dataViewer.header.dialogs.view.title.create")
              }
            </DialogTitle>
            <DialogContent style={{width: 480}}>
              <ViewBuilder
                view={view}
                onChange={setView}
              />
            </DialogContent>
            <DialogActions>
              <Button onClick={handleViewClose}>
                {t("commons.confirm.cancel")}
              </Button>
              <Button
                color="primary"
                autoFocus
                onClick={() => handleViewSubmit(isUpdatingView ? viewId : null)}
                disabled={!validateI18nObj(view.title)}
              >
                {t("commons.confirm.save")}
              </Button>
            </DialogActions>
          </Dialog>
          <Dialog
            open={isViewErrorVisible}
            maxWidth="md"
            onClose={onViewErrorHide}
          >
            <DialogTitle>
              {t("scenes.dataViewer.header.dialogs.duplicateViewError.title")}
            </DialogTitle>
            <DialogContent>
              {viewErrorMessage && (
                <Fragment>
                  {t("scenes.dataViewer.header.dialogs.duplicateViewError.content") + ": "}
                  <b>{Object.keys(viewErrorMessage).join(", ")}</b>
                </Fragment>
              )}
            </DialogContent>
            <DialogActions>
              <Button onClick={onViewErrorHide}>
                {t("commons.confirm.close")}
              </Button>
            </DialogActions>
          </Dialog>
        </Fragment>
      )}

      {initialTemplate && (
        <Dialog
          open={isTemplateVisible}
          fullScreen
          onClose={handleTemplateClose}
        >
          <DialogTitle>
            {hasTemplateLayout
              ? t("scenes.dataViewer.header.dialogs.template.title.update")
              : t("scenes.dataViewer.header.dialogs.template.title.create")
            }
          </DialogTitle>
          <DialogContent>
            <TemplateBuilder
              defaultLanguage={defaultLanguage}
              languages={languages}
              template={template}
              onChange={setTemplate}
              viewers={viewers.filter(({hidden}) => hidden !== true)}
              jsonStat={jsonStat}
              labelFormat={labelFormat}
              tableLayout={tableLayout}
              chartLayout={chartLayout}
              mapLayout={mapLayout}
              templateGeometries={templateGeometries}
              templateGeometryDetailLevels={templateGeometryDetailLevels}
              onGeometryFetch={onViewTemplateGeometriesFetch}
              hiddenAttributes={hiddenAttributes}
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={handleTemplateClose}>
              {t("commons.confirm.cancel")}
            </Button>
            <Button
              color="primary"
              autoFocus
              onClick={handleTemplateSubmit}
              disabled={(
                !validateI18nObj(template.title) ||
                !validateI18nObj(template.decimalSeparator) ||
                !Number.isInteger(Number(template.decimalNumber)) || Number(template.decimalNumber) < 0 || Number(template.decimalNumber) > 20
              )}
            >
              {t("commons.confirm.save")}
            </Button>
          </DialogActions>
        </Dialog>
      )}

      <Dialog
        open={isNotesVisible}
        fullWidth
        maxWidth="md"
        onClose={() => setNotesVisibility(false)}
      >
        <DialogTitle>
          {t("scenes.dataViewer.header.dialogs.notes.title")}
        </DialogTitle>
        <DialogContent>
          {notes}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setNotesVisibility(false)}>
            {t("commons.confirm.close")}
          </Button>
        </DialogActions>
      </Dialog>

    </Fragment>
  )
}

export default compose(
  withStyles(styles),
  connect(mapStateToProps, mapDispatchToProps)
)(DataViewerHeader)