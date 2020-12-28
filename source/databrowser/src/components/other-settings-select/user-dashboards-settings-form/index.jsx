import React, {forwardRef, Fragment, useEffect, useImperativeHandle, useState} from 'react';
import Search from "@material-ui/icons/Search";
import Clear from "@material-ui/icons/Clear";
import {Box, withStyles} from "@material-ui/core";
import DeleteIcon from '@material-ui/icons/Delete';
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import {connect} from "react-redux";
import {compose} from "redux";
import VisibilityIcon from '@material-ui/icons/Visibility';
import EditIcon from "@material-ui/icons/Edit";
import {
  changeOtherConfigDashboard,
  clearOtherConfigDashboards,
  clearOtherConfigDashboardsDataset,
  clearOtherConfigDashboardViews,
  closeOtherConfigDashboard,
  createOtherConfigDashboard,
  deleteOtherConfigDashboard,
  fetchOtherConfigDashboards,
  fetchOtherConfigDashboardsDataset,
  fetchOtherConfigDashboardsMapGeometries,
  fetchOtherConfigDashboardViews,
  hideOtherConfigDashboard,
  setOtherConfigDashboardsDatasetFilter,
  submitOtherConfigDashboardCreate,
  submitOtherConfigDashboardUpdate,
  updateOtherConfigDashboard
} from "../../../state/otherConfig/otherConfigActions";
import Grid from "@material-ui/core/Grid";
import AddIcon from "@material-ui/icons/Add";
import {goToDashboard} from "../../../links";
import DashboardBuilder from "../../dashboard-builder";
import {useTranslation} from "react-i18next";
import CustomMaterialTable from "../../custom-material-table";
import {getI18nObjCustomFilterAndSearch, localizeI18nObj, validateI18nObj} from "../../../utils/i18n";
import Call from "../../../hocs/call";
import Dashboard from "../../dashboard";
import {
  DASHBOARD_ELEM_FILTER_DIMENSION_KEY,
  DASHBOARD_ELEM_TYPE_KEY,
  DASHBOARD_ELEM_TYPE_VALUE_VIEW,
  DASHBOARD_ELEM_VALUE_KEY,
  getViewIdxFromRowAndCol
} from "../../../utils/dashboards";
import _ from "lodash"
import {MTableToolbar} from "material-table";

const tableIcons = {
  Search: forwardRef((props, ref) => <Search {...props} ref={ref}/>),
  ResetSearch: forwardRef((props, ref) => <Clear {...props} ref={ref}/>),
};

const styles = theme => ({
  tableToolbar: {
    marginBottom: theme.spacing(1)
  },
  previewDialog: {
    "& .MuiDialogTitle-root, & .MuiDialogContent-root, & .MuiDialogActions-root": {
      background: "#f5f5f5"
    }
  }
});

const mapStateToProps = state => ({
  defaultLanguage: state.app.language,
  languages: state.app.languages,
  needDashboards: state.otherConfig.needDashboards,
  dashboards: state.otherConfig.dashboards,
  dashboard: state.otherConfig.dashboard,
  dashboardViews: state.otherConfig.dashboardViews,
  dashboardJsonStats: state.otherConfig.dashboardJsonStats,
  dashboardLayouts: state.otherConfig.dashboardLayouts,
  dashboardFilterTrees: state.otherConfig.dashboardFilterTrees,
  dashboardMaps: state.otherConfig.dashboardMaps
});

const mapDispatchToProps = dispatch => ({
  fetchDashboards: () => dispatch(fetchOtherConfigDashboards()),
  clearDashboards: () => dispatch(clearOtherConfigDashboards()),
  createDashboard: dashboard => dispatch(createOtherConfigDashboard(dashboard)),
  updateDashboard: dashboardId => dispatch(updateOtherConfigDashboard(dashboardId)),
  changeDashboard: dashboard => dispatch(changeOtherConfigDashboard(dashboard)),
  submitDashboardCreate: dashboard => dispatch(submitOtherConfigDashboardCreate(dashboard)),
  submitDashboardUpdate: (dashboardId, dashboard) => dispatch(submitOtherConfigDashboardUpdate(dashboardId, dashboard)),
  deleteDashboard: dashboardId => dispatch(deleteOtherConfigDashboard(dashboardId)),
  hideDashboard: () => dispatch(hideOtherConfigDashboard()),
  fetchViews: () => dispatch(fetchOtherConfigDashboardViews()),
  clearViews: () => dispatch(clearOtherConfigDashboardViews()),
  onClose: () => dispatch(closeOtherConfigDashboard()),
  fetchDataset: (nodeId, datasetId, criteria, requestIds) => dispatch(fetchOtherConfigDashboardsDataset(nodeId, datasetId, criteria, requestIds)),
  onFilterSet: (dashboardId, viewIdx, dimension, value) => dispatch(setOtherConfigDashboardsDatasetFilter(viewIdx, dimension, value)),
  fetchMapGeometries: (dashboardId, viewIdx, idList, t) => dispatch(fetchOtherConfigDashboardsMapGeometries(viewIdx, idList, t)),
  clearDatasets: () => dispatch(clearOtherConfigDashboardsDataset()),
});

const getStaticDashboard = dashboard => {
  if (!dashboard) {
    return null
  }

  const staticDashboard = _.cloneDeep(dashboard);
  staticDashboard.dashboardConfig.forEach(row => {
    row.forEach(col => {
      col[DASHBOARD_ELEM_FILTER_DIMENSION_KEY] = null
    });
  });

  return staticDashboard
};

const initialDashboard = {
  title: {},
  dashboardConfig: [],
  viewIds: [],
  views: {},
  filterLevels: {}
};

const UserDashboardsSettingsForm = ({
                                      classes,
                                      defaultLanguage,
                                      languages,
                                      nodes,
                                      needDashboards,
                                      dashboards,
                                      dashboard,
                                      fetchDashboards,
                                      clearDashboards,
                                      createDashboard,
                                      updateDashboard,
                                      changeDashboard,
                                      submitDashboardCreate,
                                      submitDashboardUpdate,
                                      deleteDashboard,
                                      hideDashboard,
                                      dashboardViews,
                                      fetchViews,
                                      clearViews,
                                      onClose,
                                      dashboardJsonStats,
                                      dashboardLayouts,
                                      dashboardFilterTrees,
                                      onFilterSet,
                                      dashboardMaps,
                                      fetchDataset,
                                      fetchMapGeometries,
                                      clearDatasets,
                                      onDashboardsClose
                                    }, ref) => {

  const [deleteDashboardId, setDeleteDashboardId] = useState(null);
  const [updateDashboardId, setUpdateDashboardId] = useState(null);

  const [isPreviewVisible, setPreviewVisibility] = useState(false);

  const {t} = useTranslation();

  useEffect(() => {
    fetchDashboards();
  }, [fetchDashboards]);

  useEffect(() => {
    if (needDashboards) {
      fetchDashboards();
    }
  }, [dashboards, needDashboards, fetchDashboards]);

  useImperativeHandle(ref, () => ({
    cancel(f) {
      clearDashboards();
      onClose();
      f();
    }
  }));

  const handleFetchDatasets = () => {

    const requests = [];
    dashboard.dashboardConfig.forEach((row, rowIdx) => {
      row.forEach((col, colIdx) => {
        if (col[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_VIEW) {

          const view = dashboard.views[col[DASHBOARD_ELEM_VALUE_KEY]];

          let newCriteria = view.criteria;

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
              datasetId: view.datasetId.split("+").join(","),
              criteria: newCriteria,
              requestIds: [getViewIdxFromRowAndCol(rowIdx, colIdx)]
            });
          } else {
            request.requestIds.push(getViewIdxFromRowAndCol(rowIdx, colIdx));
          }
        }
      });
    });
    requests.forEach(({nodeId, datasetId, criteria, requestIds}) => fetchDataset(nodeId, datasetId, criteria, requestIds));
  };

  return dashboards && (
    <Fragment>
      <CustomMaterialTable
        components={{
          Container: Box,
          Toolbar: props =>
            <Grid container justify="space-between" spacing={1} alignItems="center">
              <Grid item>
                <MTableToolbar {...props} />
              </Grid>
              <Grid item>
                <Button
                  size="small"
                  startIcon={<AddIcon/>}
                  onClick={() => createDashboard(initialDashboard)}
                >
                  {t("scenes.dashboardsSettings.createDashboard")}
                </Button>
              </Grid>
            </Grid>
        }}
        columns={[
          {
            field: "title",
            title: t("scenes.dashboardsSettings.table.columns.dashboardTitle"),
            render: ({title}) => localizeI18nObj(title, defaultLanguage, languages),
            customFilterAndSearch: getI18nObjCustomFilterAndSearch(defaultLanguage, languages)
          }
        ]}
        data={dashboards}
        actions={[
          {
            icon: VisibilityIcon,
            tooltip: t("scenes.dashboardsSettings.table.actions.viewDashboard"),
            onClick: (_, {dashboardId}) => window.location.href.toLowerCase().includes(`/dashboards/${dashboardId}`)
              ? onDashboardsClose()
              : goToDashboard(dashboardId)
          },
          {
            icon: EditIcon,
            tooltip: t("scenes.dashboardsSettings.table.actions.editDashboard"),
            onClick: (_, {dashboardId}) => {
              updateDashboard(dashboardId);
              setUpdateDashboardId(dashboardId);
            }
          },
          {
            icon: DeleteIcon,
            tooltip: t("scenes.dashboardsSettings.table.actions.deleteDashboard"),
            onClick: (_, {dashboardId}) => setDeleteDashboardId(dashboardId)
          }
        ]}
        options={{
          paging: false,
          draggable: true,
          actionsColumnIndex: 1,
          maxBodyHeight: 400,
          showTitle: false,
          searchFieldAlignment: "left"
        }}
        icons={tableIcons}
      />

      <Dialog
        disableBackdropClick
        disableEscapeKeyDown
        maxWidth="xs"
        open={deleteDashboardId !== null}
      >
        <DialogTitle>{t("scenes.dashboardsSettings.modals.deleteDashboard.title")}</DialogTitle>
        <DialogContent>
          {t("scenes.dashboardsSettings.modals.deleteDashboard.content")}
        </DialogContent>
        <DialogActions>
          <Button
            autoFocus
            onClick={() => setDeleteDashboardId(null)}
          >
            {t("commons.confirm.cancel")}
          </Button>
          <Button
            onClick={() => {
              deleteDashboard(deleteDashboardId);
              setDeleteDashboardId(null);
            }}
          >
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={dashboard !== null}
        fullScreen
        onClose={hideDashboard}
      >
        <DialogTitle>
          {updateDashboardId
            ? t("scenes.dashboardsSettings.modals.updateDashboard.title")
            : t("scenes.dashboardsSettings.modals.createDashboard.title")
          }
        </DialogTitle>
        <DialogContent>
          <DashboardBuilder
            nodes={nodes}
            dashboard={dashboard}
            onChange={changeDashboard}
            views={dashboardViews}
            fetchViews={fetchViews}
            onViewsHide={clearViews}
            jsonStats={dashboardJsonStats}
            fetchDataset={fetchDataset}
          />
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              hideDashboard();
              setUpdateDashboardId(null);
            }}
          >
            {t("commons.confirm.close")}
          </Button>
          <Button
            onClick={() => setPreviewVisibility(true)}
          >
            {t("commons.confirm.preview")}
          </Button>
          <Button
            onClick={updateDashboardId
              ? () => submitDashboardUpdate(updateDashboardId, dashboard)
              : () => {
                submitDashboardCreate(dashboard);
                hideDashboard();
              }
            }
            disabled={dashboard && !validateI18nObj(dashboard.title)}
          >
            {t("commons.confirm.save")}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={isPreviewVisible}
        fullScreen
        onClose={() => setPreviewVisibility(false)}
        className={classes.previewDialog}
      >
        <DialogTitle>
          {t("components.dashboardBuilder.modals.preview.title")}
        </DialogTitle>
        <DialogContent>
          <Call cb={handleFetchDatasets}>
            <Dashboard
              dashboard={getStaticDashboard(dashboard)}
              jsonStats={dashboardJsonStats}
              layouts={dashboardLayouts}
              filterTrees={dashboardFilterTrees}
              onFilterSet={onFilterSet}
              maps={dashboardMaps}
              fetchMapGeometries={fetchMapGeometries}
            />
          </Call>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => {
              setPreviewVisibility(false);
              clearDatasets();
            }}
          >
            {t("commons.confirm.close")}
          </Button>
        </DialogActions>
      </Dialog>

    </Fragment>
  );
};

export default compose(withStyles(styles), connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(UserDashboardsSettingsForm);