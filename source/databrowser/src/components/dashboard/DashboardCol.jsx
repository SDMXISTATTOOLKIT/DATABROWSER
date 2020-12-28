import React, {Fragment, useEffect, useState} from "react";
import {compose} from "redux";
import {connect} from "react-redux";
import {withTranslation} from "react-i18next";
import withStyles from "@material-ui/core/styles/withStyles";
import {
  DASHBOARD_ELEM_ENABLE_FILTERS_KEY,
  DASHBOARD_ELEM_FILTER_DIMENSION_KEY,
  DASHBOARD_ELEM_SHOW_TITLE_KEY,
  DASHBOARD_ELEM_TYPE_KEY,
  DASHBOARD_ELEM_TYPE_VALUE_TEXT,
  DASHBOARD_ELEM_TYPE_VALUE_VIEW,
  DASHBOARD_ELEM_VALUE_KEY,
  DASHBOARD_ELEM_WIDTH_KEY,
  getViewIdxFromRowAndCol
} from "../../utils/dashboards";
import {getViewerIdxFromType} from "../data-viewer";
import {localizeI18nObj} from "../../utils/i18n";
import {exportChartJpeg} from "../../utils/download";
import SanitizedHTML from "../sanitized-html";
import CustomEmpty from "../custom-empty";
import {
  DASHBOARD_VIEW_STATE_APPLY_FILTERS,
  DASHBOARD_VIEW_STATE_ERROR,
  DASHBOARD_VIEW_STATE_ERROR_FETCHING_GEOMETRIES,
  DASHBOARD_VIEW_STATE_FETCHING
} from "../../state/dashboard/dashboardReducer";
import {
  getDimensionAttributeMap,
  getDimensionLabelFromJsonStat,
  getDimensionValueLabelFromJsonStat,
  TIME_PERIOD_DIMENSION_KEY
} from "../../utils/jsonStat";
import DatasetFilters from "../dataset-filters";
import JsonStatTable from "../table";
import Map from "../map";
import Chart from "../chart";
import _ from "lodash";
import CircularProgress from "@material-ui/core/CircularProgress";
import "./style.css"
import IconButton from "@material-ui/core/IconButton";
import Tooltip from "@material-ui/core/Tooltip";
import FullscreenIcon from '@material-ui/icons/Fullscreen';
import FullscreenExitIcon from '@material-ui/icons/FullscreenExit';
import {v4 as uuidv4} from 'uuid';
import GetAppIcon from '@material-ui/icons/GetApp';
import Card from "@material-ui/core/Card";
import ButtonSelect from "../button-select";

const styles = theme => ({
  col: {
    display: "inline-block",
    verticalAlign: "top"
  },
  viewContainer: {
    padding: 12,
    height: 640,
    width: "100%",
    overflow: "auto"
  },
  viewCard: {
    width: "100%",
    height: "100%",
    padding: 24
  },
  viewPlaceholder: {
    height: "100%"
  },
  viewControllers: {
    paddingBottom: 16
  },
  viewTitle: {
    fontSize: 18,
    marginBottom: 8
  },
  viewStaticFilters: {
    marginBottom: 8
  },
  viewStaticFilter: {
    display: "inline-block",
    marginRight: 8
  },
  viewActiveFilters: {},
  view: {
    width: "100%",
    position: "relative"
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

const handleHeight = (uuid, viewIdx) => {
  const controllersHeight = document.getElementById(`dashboard__view-container__${uuid}__controllers__${viewIdx}`)?.offsetHeight || 0;
  const view = document.getElementById(`dashboard__view-container__${uuid}__view__${viewIdx}`);
  if (view) {
    view.setAttribute("style", `height: calc(100% - ${controllersHeight}px)`);
  }
};

function DashboardCol(props) {

  const {
    t,
    classes,
    defaultLanguage,
    languages,
    dashboardId,
    dashboard,
    filterValue,
    rowIdx,
    colIdx,
    dashboardElem,
    jsonStat: externalJsonStat,
    layout: externalLayoutObj,
    filterTree: externalFilterTree,
    map: externalMap,
    onSelect,
    fetchMapGeometries,
    downloadCsv
  } = props;

  const [uuid] = useState(uuidv4());

  const [chartId] = useState("chart__" + uuidv4());
  const [mapId] = useState("map__" + uuidv4());

  const [jsonStat, setJsonStat] = useState(externalJsonStat);
  const [layoutObj, setLayoutObj] = useState(externalLayoutObj);
  const [filterTree, setFilterTree] = useState(externalFilterTree);
  const [map, setMap] = useState(externalMap);

  const [isFullscreen, setFullscreen] = useState(false);

  useEffect(() => {
    const func = () => handleHeight(uuid, getViewIdxFromRowAndCol(rowIdx, colIdx));

    window.addEventListener("resize", func);
    return () => window.removeEventListener("resize", func)
  }, [uuid, rowIdx, colIdx]);

  useEffect(() => {
    handleHeight(uuid, getViewIdxFromRowAndCol(rowIdx, colIdx));
  }, [uuid, rowIdx, colIdx, dashboardElem, jsonStat, layoutObj]);

  useEffect(() => {
    setJsonStat(prevJsonStat => {
      if (_.isEqual(prevJsonStat, externalJsonStat)) {
        return prevJsonStat
      } else {
        return externalJsonStat
      }
    });
  }, [externalJsonStat]);

  useEffect(() => {
    setLayoutObj(prevLayoutObj => {
      if (_.isEqual(prevLayoutObj, externalLayoutObj)) {
        return prevLayoutObj
      } else {
        return externalLayoutObj
      }
    });
  }, [externalLayoutObj]);

  useEffect(() => {
    setFilterTree(prevFilterTree => {
      if (_.isEqual(prevFilterTree, externalFilterTree)) {
        return prevFilterTree
      } else {
        return externalFilterTree
      }
    });
  }, [externalFilterTree]);

  useEffect(() => {
    setMap(prevMaps => {
      if (_.isEqual(prevMaps, externalMap)) {
        return prevMaps
      } else {
        return externalMap
      }
    });
  }, [externalMap]);

  const handleFullscreen = () => {
    !isFullscreen
      ? document.getElementById(`dashboard__view-container__${uuid}__${getViewIdxFromRowAndCol(rowIdx, colIdx)}`).classList.add("dashboard__view-container--fullscreen")
      : document.getElementById(`dashboard__view-container__${uuid}__${getViewIdxFromRowAndCol(rowIdx, colIdx)}`).classList.remove("dashboard__view-container--fullscreen");
    handleHeight(uuid, getViewIdxFromRowAndCol(rowIdx, colIdx));

    setFullscreen(!isFullscreen);
  };

  const viewIdx = getViewIdxFromRowAndCol(rowIdx, colIdx);

  const view = _.cloneDeep((dashboard.views || {})[dashboardElem[DASHBOARD_ELEM_VALUE_KEY]]);
  const viewerIdx = view ? getViewerIdxFromType(view.defaultView) : null;

  return (
    <div className={classes.col} style={{width: `${dashboardElem[DASHBOARD_ELEM_WIDTH_KEY]}%`}}>
      <div className={classes.viewContainer}>
        <Card
          id={`dashboard__view-container__${uuid}__${viewIdx}`}
          className={classes.viewCard}
        >
          {downloadCsv && dashboardElem[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_VIEW && jsonStat && layoutObj && (
            <div style={{float: "right"}}>
              <ButtonSelect
                icon={<GetAppIcon/>}
                tooltip={t("components.dashboard.actions.download")}
                color="primary"
                onChange={({format}) => {
                  if (format === "png" && viewerIdx === 1) {
                    window.LMap.exportPng(mapId, `${localizeI18nObj(view.title, defaultLanguage, languages)}.png`);
                  } else if (format === "jpeg" && viewerIdx >= 2) {
                    exportChartJpeg(chartId, `${localizeI18nObj(view.title, defaultLanguage, languages)}.jpeg`);
                  } else if (format === "csv") {
                    const newCriteria = {...view.criteria};
                    Object.keys(layoutObj.layout.filtersValue).forEach(key => {
                      if (key !== TIME_PERIOD_DIMENSION_KEY) {
                        newCriteria[key] = {
                          id: key,
                          filterValues: [layoutObj.layout.filtersValue[key]]
                        }
                      }
                    });
                    const primaryDim = layoutObj.layout?.primaryDim?.[0] || null;
                    if (primaryDim && primaryDim !== TIME_PERIOD_DIMENSION_KEY) {
                      newCriteria[primaryDim] = {
                        id: primaryDim,
                        filterValues: layoutObj.layout.primaryDimValues
                      }
                    }
                    const secondaryDim = layoutObj.layout?.secondaryDim?.[0] || null;
                    if (secondaryDim && secondaryDim !== TIME_PERIOD_DIMENSION_KEY) {
                      newCriteria[secondaryDim] = {
                        id: secondaryDim,
                        filterValues: layoutObj.layout.secondaryDimValues
                      }
                    }
                    downloadCsv(view.nodeId, view.datasetId, localizeI18nObj(view.title, defaultLanguage, languages), newCriteria);
                  }
                }}
              >
                {[{format: "csv", label: "CSV", extension: "csv"}]
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
            </div>
          )}
          {(dashboardElem[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_TEXT || (jsonStat && layoutObj)) && (
            <div style={{float: "right"}}>
              <Tooltip
                title={isFullscreen
                  ? t("components.dashboard.actions.fullscreen.exit")
                  : t("components.dashboard.actions.fullscreen.enter")
                }
              >
                <IconButton
                  color="primary"
                  onClick={handleFullscreen}
                >
                  {isFullscreen ? <FullscreenExitIcon/> : <FullscreenIcon/>}
                </IconButton>
              </Tooltip>
            </div>
          )}
          {(() => {
            if (dashboardElem[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_TEXT) {
              return (
                <SanitizedHTML
                  html={localizeI18nObj(dashboardElem[DASHBOARD_ELEM_VALUE_KEY], defaultLanguage, languages)}
                  allowTarget
                />
              )

            } else if (view === null || view === undefined) {
              return (
                <div className={classes.viewPlaceholder}>
                  <CustomEmpty
                    text={t("components.dashboard.missingView")}
                  />
                </div>
              )

            } else if (jsonStat !== null && jsonStat !== undefined) {

              if (jsonStat === DASHBOARD_VIEW_STATE_APPLY_FILTERS) {
                return (
                  <div className={classes.viewPlaceholder}>
                    <CustomEmpty
                      text={t("components.dashboard.applyFilters")}
                    />
                  </div>
                )

              } else if (jsonStat === DASHBOARD_VIEW_STATE_FETCHING) {
                return (
                  <div className={classes.viewPlaceholder}>
                    <CustomEmpty
                      text={t("components.dashboard.fetching") + "..."}
                      image={<CircularProgress/>}
                    />
                  </div>
                )

              } else if (jsonStat === DASHBOARD_VIEW_STATE_ERROR) {
                return (
                  <div className={classes.viewPlaceholder}>
                    <CustomEmpty
                      text={t("components.dashboard.fetchingDatasetError")}
                    />
                  </div>
                )

              } else if (jsonStat === DASHBOARD_VIEW_STATE_ERROR_FETCHING_GEOMETRIES) {
                return (
                  <div className={classes.viewPlaceholder}>
                    <CustomEmpty
                      text={t("components.dashboard.fetchingGeometriesError")}
                    />
                  </div>
                )

              } else if (jsonStat === "") {
                return (
                  <div className={classes.viewPlaceholder}>
                    <CustomEmpty
                      text={t("components.dashboard.emptyView")}
                    />
                  </div>
                )

              } else if (layoutObj) {
                const filter = dashboardElem[DASHBOARD_ELEM_FILTER_DIMENSION_KEY];

                let layout = {...layoutObj.layout};
                let staticFilters = [];
                let dimensionAttributesMap = null;
                getDimensionAttributeMap(jsonStat);

                if (filterValue === "" || (jsonStat.dimension[filter]?.category?.index || []).includes(filterValue)) {

                  if (filter && filterValue) {
                    if ((layout.filters || []).includes(filter)) {
                      layout.filtersValue = {
                        ...layout.filtersValue,
                        [filter]: filterValue
                      };
                    } else if ((layout.primaryDim || []).includes(filter)) {
                      layout.primaryDimValues = {
                        ...layout.primaryDimValues,
                        [filter]: filterValue
                      };
                    } else if ((layout.secondaryDim || []).includes(filter)) {
                      layout.secondaryDimValues = {
                        ...layout.secondaryDimValues,
                        [filter]: filterValue
                      };
                    }
                  }

                  layout.filters.forEach(dim => {
                    if (!dashboardElem[DASHBOARD_ELEM_ENABLE_FILTERS_KEY] || jsonStat.size[jsonStat.id.indexOf(dim)] === 1) {
                      const value = layout.filtersValue[dim];
                      staticFilters.push({
                        dim: dim,
                        dimLabel: getDimensionLabelFromJsonStat(jsonStat, dim),
                        value: value,
                        valueLabel: getDimensionValueLabelFromJsonStat(jsonStat, dim, value)
                      });
                    }
                  });

                  dimensionAttributesMap = getDimensionAttributeMap(jsonStat);
                }

                return (
                  <Fragment>
                    <div
                      id={`dashboard__view-container__${uuid}__controllers__${viewIdx}`}
                      className={classes.viewControllers}
                    >
                      {dashboardElem[DASHBOARD_ELEM_SHOW_TITLE_KEY] && (
                        <div className={classes.viewTitle}>
                          {localizeI18nObj(view.title, defaultLanguage, languages)}
                        </div>
                      )}
                      <div className={classes.viewStaticFilters}>
                        {staticFilters.map(({dim, dimLabel, value, valueLabel}, idx) =>
                          <div key={idx} style={{display: "inline-block", marginRight: 8}}>
                            <div style={{display: "inline-block"}}>
                              <b>{(dimLabel || dim)}</b>: <i>{(valueLabel || value)}</i>
                            </div>
                            {dimensionAttributesMap && dimensionAttributesMap[dim][value] && (
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
                              {idx < staticFilters.length - 1 ? "," : ""}
                            </div>
                          </div>
                        )}
                      </div>
                      {dashboardElem[DASHBOARD_ELEM_ENABLE_FILTERS_KEY] && filterTree && (
                        <div className={classes.viewActiveFilters}>
                          <DatasetFilters
                            jsonStat={jsonStat}
                            layout={layout}
                            filterTree={filterTree}
                            onSelect={(dimension, value) => onSelect(dashboardId, viewIdx, dimension, value)}
                          />
                        </div>
                      )}
                    </div>
                    <div
                      id={`dashboard__view-container__${uuid}__view__${viewIdx}`}
                      className={classes.view}
                    >
                      {(() => {
                        const decimalSeparator = ","; // TODO
                        const decimalPlaces = 2; // TODO

                        if (viewerIdx === 0) {
                          return (
                            <JsonStatTable
                              jsonStat={jsonStat}
                              layout={layout}
                              labelFormat={view.layouts.labelFormat}
                              decimalSeparator={decimalSeparator}
                              decimalPlaces={decimalPlaces}
                              emptyChar={layoutObj.tableEmptyChar}
                              isFullscreen={isFullscreen}
                              hideSpinner
                              disableWheelZoom
                            />
                          )
                        } else if (viewerIdx === 1) {
                          return (
                            <Map
                              mapId={mapId}
                              jsonStat={jsonStat}
                              layout={layout}
                              geometries={map?.geometries || null}
                              geometryDetailLevels={map?.geometryDetailLevels || null}
                              onGeometryFetch={idList => fetchMapGeometries(dashboardId, viewIdx, idList, t)}
                              detailLevel={layoutObj.mapDetailLevel}
                              classificationMethod={layoutObj.mapClassificationMethod}
                              paletteStartColor={layoutObj.mapPaletteStartColor}
                              paletteEndColor={layoutObj.mapPaletteEndColor}
                              paletteCardinality={layoutObj.mapPaletteCardinality}
                              opacity={layoutObj.mapOpacity}
                              isLegendCollapsed={layoutObj.mapIsLegendCollapsed}
                              isFullscreen={isFullscreen}
                              readOnly
                              hideSpinner
                              disableWheelZoom
                            />
                          )
                        } else {
                          return (
                            <Chart
                              chartId={chartId}
                              type={view.defaultView}
                              jsonStat={jsonStat}
                              layout={layout}
                              labelFormat={view.layouts.labelFormat}
                              decimalSeparator={decimalSeparator}
                              decimalPlaces={decimalPlaces}
                              stacked={layoutObj.chartStacked}
                              legendPosition={layoutObj.chartLegendPosition}
                              colors={layoutObj.chartColors}
                              disableWheelZoom
                            />
                          )
                        }
                      })()}
                    </div>
                  </Fragment>
                )
              } else {
                return <span/>
              }
            } else {
              return <span/>
            }
          })()}
        </Card>
      </div>
    </div>
  )
}

export default compose(
  withTranslation(),
  withStyles(styles),
  connect(state => ({
    languages: state.app.languages,
    defaultLanguage: state.app.language
  }))
)(DashboardCol);