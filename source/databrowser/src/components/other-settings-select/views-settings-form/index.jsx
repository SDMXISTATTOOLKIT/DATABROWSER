import React, {forwardRef, Fragment, useEffect, useImperativeHandle, useState} from 'react';
import Search from "@material-ui/icons/Search";
import Clear from "@material-ui/icons/Clear";
import ArrowUpward from "@material-ui/icons/ArrowUpward";
import {Box, withStyles} from "@material-ui/core";
import DeleteIcon from '@material-ui/icons/Delete';
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import {connect} from "react-redux";
import {compose} from "redux";
import {Visibility} from "@material-ui/icons";
import {
  clearOtherConfigViews,
  deleteOtherConfigView,
  fetchOtherConfigViews
} from "../../../state/otherConfig/otherConfigActions";
import {goToData} from "../../../links";
import {useTranslation} from "react-i18next";
import CustomMaterialTable from "../../custom-material-table";
import {getI18nObjCustomFilterAndSearch, localizeI18nObj} from "../../../utils/i18n";

const tableIcons = {
  Search: forwardRef((props, ref) => <Search {...props} ref={ref}/>),
  ResetSearch: forwardRef((props, ref) => <Clear {...props} ref={ref}/>),
  SortArrow: forwardRef((props, ref) => <ArrowUpward {...props} ref={ref}/>),
};

const styles = theme => ({
  tableToolbar: {
    marginBottom: theme.spacing(1)
  }
});

const mapStateToProps = state => ({
  defaultLanguage: state.app.language,
  languages: state.app.languages,
  views: state.otherConfig.views
});

const mapDispatchToProps = dispatch => ({
  fetchViews: () => dispatch(fetchOtherConfigViews()),
  clearViews: () => dispatch(clearOtherConfigViews()),
  deleteView: (nodeId, id) => dispatch(deleteOtherConfigView(nodeId, id))
});

const ViewsSettingsForm = ({
                             classes,
                             defaultLanguage,
                             languages,
                             views,
                             fetchViews,
                             clearViews,
                             deleteView,
                             nodes,
                             onViewsClose
                           }, ref) => {

  const [needViews, setNeedViews] = useState(true);
  const {t} = useTranslation();

  useEffect(() => {

    if (needViews) {
      setNeedViews(false);
      fetchViews();
    }
  }, [views, needViews, setNeedViews, fetchViews]);

  useImperativeHandle(ref, () => ({
    cancel(f) {
      clearViews();
      f();
    }
  }));

  const [deleteViewId, setDeleteViewId] = useState(null);
  const [deleteViewNodeId, setDeleteViewNodeId] = useState(null);

  return nodes && views && (
    <Fragment>
      <CustomMaterialTable
        components={{
          Container: Box,
        }}
        columns={[
          {
            field: "nodeId",
            title: t("scenes.viewsSettings.table.columns.viewNodeId"),
            render: ({nodeId}) => nodes.find(node => node.nodeId === nodeId).code,
            customFilterAndSearch: (str, {nodeId}) =>
              nodes.find(node => node.nodeId === nodeId).code.toLowerCase()
                .includes(str.toLowerCase())
          },
          {field: "datasetId", title: t("scenes.viewsSettings.table.columns.viewDatasetId")},
          {
            field: "title",
            title: t("scenes.viewsSettings.table.columns.viewTitle"),
            render: ({title}) => localizeI18nObj(title, defaultLanguage, languages),
            customFilterAndSearch: getI18nObjCustomFilterAndSearch(defaultLanguage, languages)
          },
        ]}
        data={(views||[]).filter(({nodeId}) => nodes.find(node => node.nodeId === nodeId))}
        actions={[
          {
            icon: Visibility,
            tooltip: t("scenes.viewsSettings.table.actions.viewView"),
            onClick: (_, {nodeId, datasetId, viewTemplateId}) => window.location.href.toLowerCase().includes(`view=${viewTemplateId}`)
              ? onViewsClose()
              : goToData(nodes.find(node => node.nodeId === nodeId).code, [], datasetId, viewTemplateId)
          },
          {
            icon: DeleteIcon,
            tooltip: t("scenes.viewsSettings.table.actions.deleteView"),
            onClick: (_, {nodeId, viewTemplateId}) => {
              setDeleteViewId(viewTemplateId);
              setDeleteViewNodeId(nodeId);
            }
          }
        ]}
        options={{
          paging: false,
          draggable: true,
          actionsColumnIndex: 3,
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
        open={deleteViewId !== null}
      >
        <DialogTitle>{t("scenes.viewsSettings.modals.deleteView.title")}</DialogTitle>
        <DialogContent>
          {t("scenes.viewsSettings.modals.deleteView.content")}
        </DialogContent>
        <DialogActions>
          <Button autoFocus onClick={() => {
            setDeleteViewId(null);
            setDeleteViewNodeId(null);
          }}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button onClick={() => {
            deleteView(deleteViewNodeId, deleteViewId);
            setDeleteViewId(null);
            setDeleteViewNodeId(null);
          }}>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>
    </Fragment>
  );
};

export default compose(withStyles(styles), connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(ViewsSettingsForm);