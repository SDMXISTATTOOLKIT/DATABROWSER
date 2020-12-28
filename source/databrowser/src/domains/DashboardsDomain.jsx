import React, {Fragment, useEffect, useState} from 'react';
import {compose} from "redux";
import {connect} from "react-redux";
import {withStyles} from "@material-ui/core";
import {
  clearDashboardsDashboards,
  fetchDashboardsDashboard,
  fetchDashboardsDataset,
  fetchDashboardsMapGeometries,
  resetDashboardsUpdatedDashboardId,
  setDashboardsDatasetFetchEnableStatus,
  setDashboardsDatasetFilter,
  submitDashboardDatasetDownload
} from "../state/dashboard/dashboardActions";
import Call from "../hocs/call";
import Dashboard from "../components/dashboard";
import filters from "../dummy/filters.json";
import DashboardFilters from "../components/dashboard-filters";
import {
  DASHBOARD_ELEM_FILTER_DIMENSION_KEY,
  DASHBOARD_ELEM_TYPE_KEY,
  DASHBOARD_ELEM_TYPE_VALUE_VIEW,
  DASHBOARD_ELEM_VALUE_KEY,
  getViewIdxFromRowAndCol
} from "../utils/dashboards";
import _ from "lodash";
import Card from "@material-ui/core/Card";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import CustomEmpty from "../components/custom-empty";
import NodeHeader from "../components/node-header";
import {localizeI18nObj} from "../utils/i18n";
import {useTranslation} from "react-i18next";
import {CRITERIA_FILTER_TYPE_CODES} from "../utils/criteria";

const styles = () => ({
  root: {
    backgroundColor: "#f5f5f5",
    width: "100%",
    height: "100%",
  },
  fullWidthContainer: {
    padding: 8,
    width: "100%",
    height: "100%"
  },
  commonsFilters: {
    padding: "16px 8px"
  },
  dashboardHeader: {
    padding: 8
  },
  dashboardTitle: {
    padding: "8px 0",
    fontSize: 20,
    fontWeight: 500
  },
  dashboard: {
    overflow: "auto",
    margin: "0 8px"
  }
});

const mapStateToProps = state => ({
  hub: state.hub,
  node: state.node,
  nodeId: state.node?.nodeId,
  catalog: state.catalog,
  languages: state.app.languages,
  defaultLanguage: state.app.language,
  dashboards: state.dashboard.dashboards,
  isFetchDatasetDisabled: state.dashboard.isFetchDatasetDisabled,
  jsonStats: state.dashboard.jsonStats,
  layouts: state.dashboard.layouts,
  filterTrees: state.dashboard.filterTrees,
  maps: state.dashboard.maps,
  updatedDashboardId: state.dashboard.updatedDashboardId
});

const mapDispatchToProps = dispatch => ({
  fetchDashboard: dashboardId => dispatch(fetchDashboardsDashboard(dashboardId)),
  clearDashboards: () => dispatch(clearDashboardsDashboards()),
  resetUpdatedDashboardId: () => dispatch(resetDashboardsUpdatedDashboardId()),
  setDatasetFetchEnableStatus: enabled => dispatch(setDashboardsDatasetFetchEnableStatus(enabled)),
  fetchDataset: (dashboardId, nodeId, datasetId, criteria, requestIds, worker) =>
    dispatch(fetchDashboardsDataset(dashboardId, nodeId, datasetId, criteria, requestIds, worker)),
  onFilterSet: (dashboardId, viewIdx, dimension, value) => dispatch(setDashboardsDatasetFilter(dashboardId, viewIdx, dimension, value)),
  fetchMapGeometries: (dashboardId, viewIdx, idList, t) => dispatch(fetchDashboardsMapGeometries(dashboardId, viewIdx, idList, t)),
  downloadCsv: (nodeId, datasetId, viewTitle, criteria) => dispatch(submitDashboardDatasetDownload(nodeId, datasetId, viewTitle, criteria)),
});

const handleHeight = () => {
  const nodeHeaderHeight = document.getElementById("node-header")
    ? document.getElementById("node-header").offsetHeight
    : 0;
  const dashboardsHeaderHeight = document.getElementById("dashboards-domain_header")
    ? document.getElementById("dashboards-domain_header").offsetHeight
    : 0;
  const dashboardsFiltersHeight = document.getElementById("dashboards-domain_filters")
    ? document.getElementById("dashboards-domain_filters").offsetHeight
    : 0;

  if (document.getElementById("dashboards-domain")) {
    document.getElementById("dashboards-domain").setAttribute("style", `padding-top: ${nodeHeaderHeight}px`);
  }

  if (document.getElementById("dashboards-domain_dashboards")) {
    document.getElementById("dashboards-domain_dashboards").setAttribute("style", `height: calc(100% - ${dashboardsFiltersHeight + dashboardsHeaderHeight}px)`);
  }
};

const getIsFilterEnabled = dashboard => {
  if (!dashboard) {
    return false
  }

  let hasDynamicView = false;
  dashboard.dashboardConfig.forEach(row => {
    row.forEach(col => {
      if ((col[DASHBOARD_ELEM_FILTER_DIMENSION_KEY] || "").length > 0) {
        hasDynamicView = true;
      }
    });
  });

  let lastLavorizableIdx = -1;
  filters.labels.forEach((label, idx) => {
    if (dashboard?.filterLevels?.[filters.labels[idx]] === true) {
      lastLavorizableIdx = idx;
    }
  });

  return (hasDynamicView && lastLavorizableIdx >= 0)
};

function DashboardsDomain(props) {
  const {
    classes,
    defaultLanguage,
    languages,
    hub,
    node,
    nodeId,
    nodeCode,
    isDefault,
    catalog,
    dashboardId,
    dashboards,
    isFetchDatasetDisabled,
    jsonStats,
    layouts,
    filterTrees,
    maps,
    updatedDashboardId,
    fetchDashboard,
    clearDashboards,
    resetUpdatedDashboardId,
    setDatasetFetchEnableStatus,
    fetchDataset,
    onFilterSet,
    fetchMapGeometries,
    downloadCsv
  } = props;

  const {t} = useTranslation();

  const [filterValue, setFilterValue] = useState("");
  const [selectValues, setSelectValues] = useState({});
  const [lastValorizedIdx, setLastValorizedIdx] = useState(-1);

  const [allowedDashboards, setAllowedDashboards] = useState(null);

  const [fetchedDashboards, setFetchedDashboards] = useState(null);

  const [tabIdx, setTabIdx] = useState(0);

  const [worker] = useState(() => new Worker("./workers/fetchDashboardDatasetAsyncHandlerWorker.js"));

  useEffect(() => {
    return () => {
      if (worker) {
        worker.terminate();
      }
    }
  }, [worker]);

  // const {
  //   labels
  // } = filters;

  useEffect(() => {
    if (updatedDashboardId) {
      setFetchedDashboards(prevFetchedDashboards => {
        const newFetchedDashboards = {};
        Object.keys(prevFetchedDashboards).forEach(key => {
          newFetchedDashboards[key] = {static: false, dynamic: false}
        });
        return newFetchedDashboards
      });
      resetUpdatedDashboardId();
    }
  }, [updatedDashboardId, resetUpdatedDashboardId]);

  useEffect(() => {
    handleHeight();
  }, [dashboards]);

  useEffect(() => {
    window.addEventListener("resize", handleHeight);
    return () => window.removeEventListener("resize", handleHeight)
  }, []);

  useEffect(() => {
    if (hub && (!nodeCode || (node && catalog && nodeId === catalog.nodeId && node.code.toLowerCase() === nodeCode.toLowerCase()))) {

      const nodeDashboards = nodeId ? (hub.nodes.find(node => node.nodeId === nodeId).dashboards || []) : [];
      const hubDashboards = (hub.hub.dashboards || []);

      const newAllowedDashboards = dashboardId
        ? [{id: dashboardId}]
        : nodeId
          ? _.uniqBy(hubDashboards.concat(nodeDashboards), "id")
          : hubDashboards
      setAllowedDashboards(newAllowedDashboards);

      const newFetchedDashboards = {};
      newAllowedDashboards.forEach(({id}) => newFetchedDashboards[id] = {static: false, dynamic: false});
      setFetchedDashboards(newFetchedDashboards);

    } else {
      clearDashboards();
      setFilterValue("");
      setAllowedDashboards(null);
    }

    return () => {
      clearDashboards();
    }
  }, [hub, nodeId, nodeCode, node, catalog, dashboardId, clearDashboards]);

  const handleFilterApply = value => {
    if (value !== filterValue) {
      setFilterValue(value);
      setDatasetFetchEnableStatus(true);

      const newFetchedDashboard = _.cloneDeep(fetchedDashboards);
      allowedDashboards.forEach(({id}) => newFetchedDashboard[id] = {...fetchedDashboards[id], dynamic: false});
      setFetchedDashboards(newFetchedDashboard);
    }
  };

  const handleFetchDatasets = ({dashboardId, fetchStatic, fetchDynamic, dynamicFilterValue}) => {

    const newFetchedDashboards = _.cloneDeep(fetchedDashboards);
    if (fetchStatic) {
      newFetchedDashboards[dashboardId].static = true;
    }
    if (fetchDynamic && dynamicFilterValue) {
      newFetchedDashboards[dashboardId].dynamic = true;
    }
    setFetchedDashboards(newFetchedDashboards);

    const requests = [];
    dashboards[dashboardId].dashboardConfig.forEach((row, rowIdx) => {
      row.forEach((col, colIdx) => {
        if (col[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_VIEW) {
          const view = dashboards[dashboardId].views[col[DASHBOARD_ELEM_VALUE_KEY]];

          if (view) {
            let newCriteria = (view?.criteria || {});

            let addRequest = false;

            const filterDimension = col[DASHBOARD_ELEM_FILTER_DIMENSION_KEY];

            if (!filterDimension || filterDimension.length === 0) {
              addRequest = fetchStatic;

            } else {
              if (fetchDynamic) {
                addRequest = true;
                if (dynamicFilterValue) {
                  newCriteria = {
                    ...newCriteria,
                    [filterDimension]: {
                      ...newCriteria[filterDimension],
                      id: filterDimension,
                      type: CRITERIA_FILTER_TYPE_CODES,
                      filterValues: [dynamicFilterValue]
                    }
                  };
                } else {
                  newCriteria = {
                    ...newCriteria,
                    [filterDimension]: {
                      ...newCriteria[filterDimension],
                      id: filterDimension,
                      type: CRITERIA_FILTER_TYPE_CODES,
                      filterValues: [filters.defaultFilterValue]
                    }
                  };
                }
              }
            }

            if (addRequest) {
              const request = requests.find(({nodeId, datasetId, criteria}) => {
                return (
                  nodeId === view.nodeId &&
                  datasetId === view.datasetId &&
                  _.isEqual(criteria, newCriteria)
                )
              });

              if (request === undefined) {
                requests.push({
                  nodeId: view.nodeId,
                  datasetId: view.datasetId,
                  criteria: newCriteria,
                  requestIds: [getViewIdxFromRowAndCol(rowIdx, colIdx)]
                });
              } else {
                request.requestIds.push(getViewIdxFromRowAndCol(rowIdx, colIdx));
              }
            }
          }
        }
      });
    });
    requests.forEach(({nodeId, datasetId, criteria, requestIds}) => fetchDataset(dashboardId, nodeId, datasetId, criteria, requestIds, worker));
  };

  // const resetFilter = () => {
  //   setFilterValue("");
  //   setSelectValues({});
  //   setLastValorizedIdx(-1);
  //   if (allowedDashboards) {
  //     const newFetchedDashboards = {};
  //     allowedDashboards.forEach(({id}) => newFetchedDashboards[id] = {static: fetchedDashboards[id].static, dynamic: false});
  //     setFetchedDashboards(newFetchedDashboards);
  //   }
  // };

  const currentDashboardId = dashboardId
    ? dashboardId
    : allowedDashboards?.[tabIdx]?.id
      ? allowedDashboards[tabIdx].id
      : null;

  const hubDashboards = (hub.hub.dashboards || []);

  return (
    <div key={dashboardId} className={classes.root}>
      {hub && (
        nodeId
          ? (
            <NodeHeader
              hub={hub.hub}
              nodes={hub.nodes}
              node={node}
              nodeId={nodeId}
              isDefault={isDefault}
              title={node?.name}
              catalog={catalog}
              defaultNodeConfigOpen={props.defaultNodeConfigOpen}
              defaultAppConfigOpen={props.defaultAppConfigOpen}
              defaultUserConfigOpen={props.defaultUserConfigOpen}
              defaultNodesConfigOpen={props.defaultNodesConfigOpen}
              getCustomA11yPath={
                isA11y => {
                  if (isA11y && currentDashboardId) {
                    if (hubDashboards.find(({id}) => id === currentDashboardId)) {
                      return `/${window.language}`;
                    } else {
                      return `/${window.language}/${nodeCode.toLowerCase()}`;
                    }
                  } else {
                    return false;
                  }
                }
              }
            />
          )
          : (
            <NodeHeader
              noNode
              hub={hub.hub}
              nodes={hub.nodes}
              defaultNodeConfigOpen={props.defaultNodeConfigOpen}
              defaultAppConfigOpen={props.defaultAppConfigOpen}
              defaultUserConfigOpen={props.defaultUserConfigOpen}
              defaultNodesConfigOpen={props.defaultNodesConfigOpen}
              getCustomA11yPath={
                isA11y => {
                  if (isA11y && currentDashboardId) {
                    return `/${window.language}`;
                  } else {
                    return false;
                  }
                }
              }
            />
          )
      )}
      <div className={classes.fullWidthContainer} id="dashboards-domain">
        {currentDashboardId
          ? (
            <Call
              cb={fetchDashboard}
              cbParam={currentDashboardId}
              disabled={dashboards !== null && dashboards[currentDashboardId] !== null && dashboards[currentDashboardId] !== undefined}
            >
              {(() => {
                return (
                  <Call
                    cb={handleFetchDatasets}
                    cbParam={{
                      dashboardId: currentDashboardId,
                      fetchStatic: true,
                      fetchDynamic: !filterValue,
                      dynamicFilterValue: null
                    }}
                    disabled={!dashboards || !dashboards[currentDashboardId] || !fetchedDashboards || fetchedDashboards[currentDashboardId].static === true}
                  >
                    <Fragment>
                      <div id="dashboards-domain_filters">
                        {(() => {
                          const isEnable = getIsFilterEnabled(dashboards?.[currentDashboardId]);
                          const FilterComponent = (
                            <DashboardFilters
                              filters={filters}
                              selectValues={selectValues}
                              setSelectValues={setSelectValues}
                              lastValorizedIdx={lastValorizedIdx}
                              setLastValorizedIdx={setLastValorizedIdx}
                              filterLevels={dashboards?.[currentDashboardId] ? dashboards[currentDashboardId].filterLevels : null}
                              onFilterApply={handleFilterApply}
                            />
                          );

                          if (dashboardId) {
                            return isEnable && (
                              <div className={classes.commonsFilters}>
                                {FilterComponent}
                              </div>
                            )
                          } else {
                            return (
                              <div className={classes.commonsFilters} style={{height: 77}}>
                                {FilterComponent}
                              </div>
                            )
                          }
                        })()}
                      </div>
                      {(() => {
                        if (dashboards?.[currentDashboardId]) {

                          let newFilterValue = filterValue;

                          // if (lastValorizedIdx >= 0 && dashboards[currentDashboardId].filterLevels && dashboards[currentDashboardId].filterLevels?.[labels[lastValorizedIdx]] === false) {
                          //   newFilterValue = "";
                          //   resetFilter();
                          // }

                          return (
                            <Call
                              cb={handleFetchDatasets}
                              cbParam={{
                                dashboardId: currentDashboardId,
                                fetchStatic: false,
                                fetchDynamic: !!newFilterValue,
                                dynamicFilterValue: newFilterValue
                              }}
                              disabled={isFetchDatasetDisabled || fetchedDashboards[currentDashboardId].dynamic === true || fetchedDashboards[currentDashboardId].static !== true}
                            >
                              <Fragment>
                                <div id="dashboards-domain_header" className={classes.dashboardHeader}>
                                  {((allowedDashboards || []).length > 1)
                                    ? (
                                      <Card>
                                        <Tabs
                                          value={tabIdx}
                                          variant="scrollable"
                                          scrollButtons="auto"
                                          onChange={(_, newValue) => setTabIdx(newValue)}
                                        >
                                          {allowedDashboards.map(({id, title}, idx) =>
                                            <Tab key={idx} label={title}/>
                                          )}
                                        </Tabs>
                                      </Card>
                                    )
                                    : (
                                      <div className={classes.dashboardTitle}>
                                        {localizeI18nObj(dashboards[currentDashboardId].title, defaultLanguage, languages)}
                                      </div>
                                    )
                                  }
                                </div>
                                <div
                                  id="dashboards-domain_dashboards"
                                  className={classes.dashboard}
                                >
                                  {dashboards[currentDashboardId] && (
                                    <Dashboard
                                      key={currentDashboardId}
                                      dashboardId={currentDashboardId}
                                      dashboard={dashboards[currentDashboardId]}
                                      filterValue={filterValue}
                                      jsonStats={jsonStats?.[currentDashboardId] || null}
                                      layouts={layouts?.[currentDashboardId] || null}
                                      filterTrees={filterTrees?.[currentDashboardId] || null}
                                      maps={maps?.[currentDashboardId] || null}
                                      onFilterSet={onFilterSet}
                                      fetchMapGeometries={fetchMapGeometries}
                                      downloadCsv={downloadCsv}
                                    />
                                  )}
                                </div>
                              </Fragment>
                            </Call>
                          )
                        }
                      })()}
                    </Fragment>
                  </Call>
                )
              })()}
            </Call>
          )
          : (
            <CustomEmpty
              text={nodeId
                ? t("scenes.dashboard.noDashboard.node")
                : t("scenes.dashboard.noDashboard.hub")
              }
            />
          )
        }
      </div>
    </div>
  )
}

export default compose(
  connect(mapStateToProps, mapDispatchToProps),
  withStyles(styles)
)(DashboardsDomain);