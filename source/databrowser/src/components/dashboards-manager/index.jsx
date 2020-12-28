import React, {forwardRef, Fragment, useEffect, useImperativeHandle, useState} from 'react';
import {connect} from "react-redux";
import Grid from "@material-ui/core/Grid";
import Button from "@material-ui/core/Button";
import AddIcon from "@material-ui/icons/Add";
import {compose} from "redux";
import {Box, Checkbox, withStyles} from "@material-ui/core";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import ArrowDropUpIcon from "@material-ui/icons/ArrowDropUp";
import ArrowDropDownIcon from "@material-ui/icons/ArrowDropDown";
import DeleteIcon from "@material-ui/icons/Delete";
import Search from "@material-ui/icons/Search";
import Clear from "@material-ui/icons/Clear";
import ArrowUpward from "@material-ui/icons/ArrowUpward";
import {useTranslation} from "react-i18next";
import CustomMaterialTable from "../custom-material-table";
import {getI18nObjCustomFilterAndSearch, localizeI18nObj} from "../../utils/i18n";
import {MTableToolbar} from "material-table";
import "./style.css";
import FormControlLabel from "@material-ui/core/FormControlLabel";

const styles = theme => ({
  tableToolbar: {
    marginBottom: theme.spacing(1)
  },
  buttonAction: {
    ...theme.typography.button
  }
});

const mapStateToProps = state => ({
  languages: state.app.languages,
  defaultLanguage: state.app.language
});

const tableIcons = {
  Search: forwardRef((props, ref) => <Search {...props} ref={ref}/>),
  ResetSearch: forwardRef((props, ref) => <Clear {...props} ref={ref}/>),
  SortArrow: forwardRef((props, ref) => <ArrowUpward {...props} ref={ref}/>),
};

const DashboardsManager = ({
                             classes,
                             defaultLanguage,
                             languages,
                             dashboards,
                             allDashboards,
                             fetchDashboards,
                             fetchAllDashboards,
                             addDashboard,
                             removeDashboard,
                             sendDashboardsOrders,
                             clearDashboards,
                             clearAllDashboards
                           }, ref) => {

  const [needFetch, setNeedFetch] = useState(true);
  const {t} = useTranslation();

  useEffect(() => {
    if (needFetch) {
      setNeedFetch(false);
      fetchDashboards();
    }
  }, [dashboards, needFetch, setNeedFetch, fetchDashboards]);

  useImperativeHandle(ref, () => ({
    destroy(f) {
      clearDashboards();
      if (f) {
        f();
      }
    }
  }));

  const orderedDashboardsIds = (dashboards || []).map(({dashboardId}) => dashboardId);

  const moveUp = rowIndex =>
    sendDashboardsOrders([
      ...orderedDashboardsIds.slice(0, rowIndex - 1),
      orderedDashboardsIds[rowIndex],
      orderedDashboardsIds[rowIndex - 1],
      ...orderedDashboardsIds.slice(rowIndex + 1)
    ]);

  const moveDown = rowIndex =>
    sendDashboardsOrders([
      ...orderedDashboardsIds.slice(0, rowIndex),
      orderedDashboardsIds[rowIndex + 1],
      orderedDashboardsIds[rowIndex],
      ...orderedDashboardsIds.slice(rowIndex + 2)
    ]);

  return dashboards !== null && (
    <Fragment>
      <Dialog
        open={allDashboards !== null}
        disableEscapeKeyDown
        disableBackdropClick
        maxWidth="md"
        fullWidth
        onClose={() => {
          clearAllDashboards();
        }}
      >
        <DialogTitle>
          {t("components.dashboardsManager.modals.addDashboard.title")}
        </DialogTitle>
        <DialogContent style={{minHeight: 456}}>
          {allDashboards !== null && (
            <CustomMaterialTable
              components={{
                Container: Box
              }}
              columns={[
                {
                  field: "title",
                  title: t("components.dashboardsManager.modals.addDashboard.table.columns.dashboardTitle"),
                  render: ({title}) => localizeI18nObj(title, defaultLanguage, languages),
                  customFilterAndSearch: getI18nObjCustomFilterAndSearch(defaultLanguage, languages)
                }
              ]}
              data={allDashboards}
              actions={[
                ({dashboardId}) => {
                  const assigned = !!dashboards.find(dashboard => dashboard.dashboardId === dashboardId)
                  return ({
                    icon: AddIcon,
                    tooltip:
                      assigned
                        ? t("components.dashboardsManager.modals.addDashboard.table.actions.addDashboard.tooltip.assigned")
                        : t("components.dashboardsManager.modals.addDashboard.table.actions.addDashboard.tooltip.notAssigned"),
                    onClick: (_, {dashboardId}) => addDashboard(dashboardId),
                    disabled: assigned
                  });
                }
              ]}
              options={{
                paging: false,
                draggable: true,
                actionsColumnIndex: 2,
                searchFieldAlignment: "left",
                maxBodyHeight: 400,
                showTitle: false
              }}
              icons={tableIcons}
            />
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => clearAllDashboards()}>
            {t("commons.confirm.cancel")}
          </Button>
        </DialogActions>
      </Dialog>
      <FormControlLabel
        label={t("components.dashboardsManager.fields.showHubDashboards.label")}
        control={
          <Checkbox
            name="showHubDashboards"
            checked
            disabled
          />
        }
      />
      <Box className="dashboards-manager__table">
        <CustomMaterialTable
          title=""
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
                    onClick={() => {
                      fetchAllDashboards();
                    }}>
                    {t("components.dashboardsManager.addDashboard")}
                  </Button>
                </Grid>
              </Grid>
          }}
          columns={[
            {
              field: "title",
              title: t("components.dashboardsManager.table.columns.dashboardTitle"),
              render: ({title}) => localizeI18nObj(title, defaultLanguage, languages),
              customFilterAndSearch: getI18nObjCustomFilterAndSearch(defaultLanguage, languages)
            }
          ]}
          data={dashboards}
          actions={[
            ({tableData}) => ({
              icon: ArrowDropUpIcon,
              tooltip: t("components.dashboardsManager.table.actions.dashboardOrderMoveUp"),
              onClick: (_, {tableData}) => moveUp(tableData.id), // tableData.id is the rowIndex
              disabled: tableData.id === 0
            }),
            ({tableData}) => ({
              icon: ArrowDropDownIcon,
              tooltip: t("components.dashboardsManager.table.actions.dashboardOrderMoveDown"),
              onClick: (_, {tableData}) => moveDown(tableData.id), // tableData.id is the rowIndex
              disabled: tableData.id === dashboards.length - 1
            }),
            {
              icon: DeleteIcon,
              tooltip: t("components.dashboardsManager.table.actions.removeDashboard"),
              onClick: (_, {dashboardId}) => removeDashboard(dashboardId)
            }
          ]}
          options={{
            paging: false,
            draggable: true,
            actionsColumnIndex: 2,
            searchFieldAlignment: "left",
            maxBodyHeight: 320
          }}
          icons={tableIcons}
        />
      </Box>
    </Fragment>
  );
}

export default compose(
  withStyles(styles),
  connect(mapStateToProps, null, null, {forwardRef: true}),
  forwardRef
)(DashboardsManager);