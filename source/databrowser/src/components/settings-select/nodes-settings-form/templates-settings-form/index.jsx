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
  clearNodeTemplatesConfig,
  deleteNodeTemplatesConfigTemplate,
  fetchNodeTemplatesConfig
} from "../../../../state/noteTemplatesConfig/nodeTemplatesConfigActions";
import {goToData} from "../../../../links";
import {useTranslation} from "react-i18next";
import CustomMaterialTable from "../../../custom-material-table";
import {getI18nObjCustomFilterAndSearch, localizeI18nObj} from "../../../../utils/i18n";

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
  templates: state.nodeTemplatesConfig.templates
});

const mapDispatchToProps = dispatch => ({
  fetchTemplates: nodeId => dispatch(fetchNodeTemplatesConfig(nodeId)),
  deleteTemplate: (nodeId, id) => dispatch(deleteNodeTemplatesConfigTemplate(nodeId, id)),
  clearTemplates: () => dispatch(clearNodeTemplatesConfig())
});

const TemplatesSettingsForm = ({
                                 classes,
                                 defaultLanguage,
                                 languages,
                                 nodes,
                                 nodeId,
                                 defaultNodeOpen,
                                 templates,
                                 fetchTemplates,
                                 deleteTemplate,
                                 clearTemplates,
                                 onTemplatesClose
                               }, ref) => {

  const [needTemplates, setNeedTemplates] = useState(true);

  const {t} = useTranslation();

  useEffect(() => {

    if (needTemplates) {
      setNeedTemplates(false);
      fetchTemplates(nodeId);
    }
  }, [templates, needTemplates, setNeedTemplates, fetchTemplates, nodeId]);

  useImperativeHandle(ref, () => ({
    cancel(f) {
      clearTemplates();
      f();
    }
  }));

  const [deleteTemplateId, setDeleteTemplateId] = useState(null);

  return templates && (
    <Fragment>
      <CustomMaterialTable
        components={{
          Container: Box
        }}
        columns={[
          {
            field: "datasetId",
            title: t("scenes.nodeSettings.templatesSettings.table.columns.datasetId")
          },
          {
            field: "title",
            title: t("scenes.nodeSettings.templatesSettings.table.columns.templateName"),
            render: ({title}) => localizeI18nObj(title, defaultLanguage, languages),
            customFilterAndSearch: getI18nObjCustomFilterAndSearch(defaultLanguage, languages)
          }
        ]}
        data={(templates || []).filter(({nodeId}) => nodes.find(node => node.nodeId === nodeId))}
        actions={[
          {
            icon: Visibility,
            tooltip: t("scenes.nodeSettings.templatesSettings.table.actions.viewData"),
            onClick: (_, {nodeId, datasetId}) => {
              const nodeCode = nodes.find(node => node.nodeId === nodeId).code.toLowerCase();
              return (window.location.href.toLowerCase().includes(nodeCode) && window.location.href.toLowerCase().includes(datasetId.toLowerCase()))
                ? onTemplatesClose()
                : goToData(nodeCode, [], datasetId)
            }

          },
          {
            icon: DeleteIcon,
            tooltip: t("scenes.nodeSettings.templatesSettings.table.actions.deleteTemplate"),
            onClick: (_, {viewTemplateId}) => setDeleteTemplateId(viewTemplateId)
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
        open={deleteTemplateId !== null}
      >
        <DialogTitle> {t("scenes.nodeSettings.templatesSettings.modals.deleteTemplate.title")}</DialogTitle>
        <DialogContent>
          {t("scenes.nodeSettings.templatesSettings.modals.deleteTemplate.content")}
        </DialogContent>
        <DialogActions>
          <Button autoFocus onClick={() => {
            setDeleteTemplateId(null);
          }}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button onClick={() => {
            deleteTemplate(nodeId, deleteTemplateId);
            setDeleteTemplateId(null);
          }}>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>
    </Fragment>
  );
};

export default compose(withStyles(styles), connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(TemplatesSettingsForm);