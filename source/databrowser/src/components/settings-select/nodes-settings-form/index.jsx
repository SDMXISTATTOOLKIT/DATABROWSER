import React, {forwardRef, Fragment, useEffect, useImperativeHandle, useRef, useState} from 'react';
import NodeSettingsForm from "./node-settings-form";
import Search from "@material-ui/icons/Search";
import Clear from "@material-ui/icons/Clear";
import ArrowUpward from "@material-ui/icons/ArrowUpward";
import EditIcon from '@material-ui/icons/Edit';
import {Box, withStyles} from "@material-ui/core";
import SettingsDialog from "../../settings-dialog";
import AddIcon from '@material-ui/icons/Add';
import ArrowDropUpIcon from '@material-ui/icons/ArrowDropUp';
import ArrowDropDownIcon from '@material-ui/icons/ArrowDropDown';
import DeleteIcon from '@material-ui/icons/Delete';
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import {
  clearNodesConfigNodes,
  closeNodesConfig,
  deleteNodesConfigNode,
  fetchNodesConfigNodes,
  sendNodesConfigNodesOrder
} from "../../../state/nodesConfig/nodesConfigActions";
import {connect} from "react-redux";
import Grid from "@material-ui/core/Grid";
import {compose} from "redux";
import CacheSettingsForm from "./cache-setting-form/index";
import TemplatesSettingsForm from "./templates-settings-form";
import {
  canCreateNode,
  canDeleteNodes,
  canDisplayCacheSettingForm,
  canDisplayPermissionsSettingsForm,
  canDisplayTemplatesSettingsForm,
  canEditNode,
  canManageNodeDashboards,
  canSortNodes
} from "../../../utils/user";
import PermissionsSettingsForm from "./permissions-settings-form";
import PersonIcon from '@material-ui/icons/Person';
import DashboardIcon from "@material-ui/icons/Dashboard";
import DashboardsSettingsForm from "./dashboards-settings-form";
import {useTranslation} from "react-i18next";
import CustomMaterialTable from "../../custom-material-table";
import {getI18nObjCustomFilterAndSearch, localizeI18nObj} from "../../../utils/i18n";
import "./style.css";
import {MTableToolbar} from "material-table";

const tableIcons = {
  Search: forwardRef((props, ref) => <Search {...props} ref={ref}/>),
  ResetSearch: forwardRef((props, ref) => <Clear {...props} ref={ref}/>),
  SortArrow: forwardRef((props, ref) => <ArrowUpward {...props} ref={ref}/>),
};

const styles = theme => ({
  tableToolbar: {
    marginBottom: theme.spacing(1)
  },
  buttonAction: {
    ...theme.typography.button
  }
});

const mapStateToProps = state => ({
  config: state.nodesConfig.nodes,
  user: state.user,
  language: state.app.language,
  languages: state.app.languages
});

const mapDispatchToProps = dispatch => ({
  fetchConfig: () => dispatch(fetchNodesConfigNodes()),
  sendOrders: orderedIds => dispatch(sendNodesConfigNodesOrder(orderedIds)),
  deleteNode: nodeId => dispatch(deleteNodesConfigNode(nodeId)),
  clearConfig: () => dispatch(clearNodesConfigNodes()),
  onClose: () => dispatch(closeNodesConfig())
});

const NodesSettingsForm = ({
                             classes,
                             defaultNodeOpen,
                             config,
                             fetchConfig,
                             sendOrders,
                             deleteNode,
                             clearConfig,
                             nodes,
                             user,
                             onClose,
                             language,
                             languages,
                             onNodesClose
                           }, ref) => {

  const [needConfig, setNeedConfig] = useState(true);
  const {t} = useTranslation();

  useEffect(() => {

    if (needConfig) {
      setNeedConfig(false);
      fetchConfig();
    }
  }, [config, needConfig, setNeedConfig, fetchConfig]);

  useImperativeHandle(ref, () => ({
    cancel(f) {
      clearConfig();
      onClose();
      f();
    }
  }));

  const [isNodeFormVisible, setNodeFormVisibility] = useState(defaultNodeOpen || false);
  const [nodeFormNodeId, setNodeFormNodeId] = useState(null);
  const [nodeDeleteFormNodeId, setNodeDeleteFormNodeId] = useState(null);
  const [cacheFormNodeId, setCacheFormNodeId] = useState(null);
  const [isCacheFormVisible, setCacheFormVisibility] = useState(false);
  const [templatesNodeId, setTemplatesNodeId] = useState(null);
  const [permissionsNodeId, setPermissionsNodeId] = useState(null);
  const [dashboardsNodeId, setDashboardsNodeId] = useState(null);

  const nodeFormRef = useRef();
  const cacheFormRef = useRef();
  const templatesRef = useRef();
  const permissionsRef = useRef();
  const dashboardsRef = useRef();

  const allowedNodes = (config || []).filter(({nodeId}) =>
    canCreateNode(user) ||
    canSortNodes(user) ||
    canDisplayCacheSettingForm(user, nodeId) ||
    canDisplayTemplatesSettingsForm(user, nodeId) ||
    canDisplayPermissionsSettingsForm(user) ||
    canEditNode(user, nodeId) ||
    canDeleteNodes(user)
  );
  const orderedNodes = (allowedNodes || []).sort((a, b) => a.order - b.order)
  const orderedNodesIds = orderedNodes.map(({nodeId}) => nodeId);

  const moveUp = rowIndex =>
    sendOrders([
      ...orderedNodesIds.slice(0, rowIndex - 1),
      orderedNodesIds[rowIndex],
      orderedNodesIds[rowIndex - 1],
      ...orderedNodesIds.slice(rowIndex + 1)
    ]);

  const moveDown = rowIndex =>
    sendOrders([
      ...orderedNodesIds.slice(0, rowIndex),
      orderedNodesIds[rowIndex + 1],
      orderedNodesIds[rowIndex],
      ...orderedNodesIds.slice(rowIndex + 2)
    ]);

  return config && (
    <Fragment>
      <Box className="nodes-settings-form__table">
        <CustomMaterialTable
          components={{
            Container: Box,
            Toolbar: props =>
              <Grid container justify="space-between" spacing={1} alignItems="center">
                <Grid item>
                  <MTableToolbar {...props} />
                </Grid>
                {canCreateNode(user) && (
                  <Grid item>
                    <Button
                      size="small"
                      startIcon={<AddIcon/>}
                      onClick={() => {
                        setNodeFormNodeId(null);
                        setNodeFormVisibility(true);
                        setCacheFormVisibility(false);
                      }}>
                      {t("scenes.nodesSettings.createNode")}
                    </Button>
                  </Grid>
                )}
              </Grid>
          }}
          columns={[
            {
              field: "code",
              title: t("scenes.nodesSettings.table.columns.nodeCode")
            },
            {
              field: "title",
              title: t("scenes.nodesSettings.table.columns.nodeName"),
              render: ({title}) => localizeI18nObj(title, language, languages),
              customFilterAndSearch: getI18nObjCustomFilterAndSearch(language, languages)
            },
            {
              field: "active",
              title: t("scenes.nodesSettings.table.columns.isNodeActive.title"),
              render: ({active}) =>
                active
                  ? t("scenes.nodesSettings.table.columns.isNodeActive.values.true")
                  : t("scenes.nodesSettings.table.columns.isNodeActive.values.false")
            },
            {
              field: "default",
              title: t("scenes.nodesSettings.table.columns.isNodeDefault.title"),
              render: node =>
                node.default
                  ? t("scenes.nodesSettings.table.columns.isNodeDefault.values.true")
                  : t("scenes.nodesSettings.table.columns.isNodeDefault.values.false")
            }
          ]}
          data={orderedNodes}
          actions={[
            ({tableData}) =>
              canSortNodes(user)
                ? {
                  icon: ArrowDropUpIcon,
                  tooltip: t("scenes.nodesSettings.table.actions.nodeOrderMoveUp"),
                  onClick: (_, {tableData}) => moveUp(tableData.id), // tableData.id is the rowIndex
                  disabled: tableData.id === 0
                }
                : null,
            ({tableData}) =>
              canSortNodes(user)
                ? {
                  icon: ArrowDropDownIcon,
                  tooltip: t("scenes.nodesSettings.table.actions.nodeOrderMoveDown"),
                  onClick: (_, {tableData}) => moveDown(tableData.id), // tableData.id is the rowIndex
                  disabled: tableData.id === config.length - 1
                }
                : null,
            ({nodeId}) =>
              canDisplayCacheSettingForm(user, nodeId)
                ? {
                  icon: () =>
                    <div className={classes.buttonAction}>
                      {t("scenes.nodesSettings.table.actions.nodeDataflowsCache.label")}
                    </div>,
                  tooltip: t("scenes.nodesSettings.table.actions.nodeDataflowsCache.tooltip"),
                  onClick: (_, {nodeId}) => {
                    setCacheFormNodeId(nodeId);
                    setNodeFormVisibility(false);
                    setCacheFormVisibility(true);
                  }
                }
                : null,
            ({nodeId}) =>
              canDisplayTemplatesSettingsForm(user, nodeId)
                ? {
                  icon: () =>
                    <div className={classes.buttonAction}>
                      {t("scenes.nodesSettings.table.actions.nodeTemplates.label")}
                    </div>,
                  tooltip: t("scenes.nodesSettings.table.actions.nodeTemplates.tooltip"),
                  onClick: (_, {nodeId}) => {
                    setTemplatesNodeId(nodeId);
                  }
                }
                : null,
            () =>
              canDisplayPermissionsSettingsForm(user)
                ? {
                  icon: PersonIcon,
                  tooltip: t("scenes.nodesSettings.table.actions.nodePermissions"),
                  onClick: (_, {nodeId}) => {
                    setPermissionsNodeId(nodeId);
                  }
                }
                : null,
            ({nodeId}) =>
              canManageNodeDashboards(user, nodeId)
                ? {
                  icon: DashboardIcon,
                  tooltip: t("scenes.nodesSettings.table.actions.nodeDashboards"),
                  onClick: (_, {nodeId}) => {
                    setDashboardsNodeId(nodeId);
                  }
                }
                : null,
            ({nodeId}) =>
              canEditNode(user, nodeId)
                ? {
                  icon: EditIcon,
                  tooltip: t("scenes.nodesSettings.table.actions.editNode"),
                  onClick: (_, {nodeId}) => {
                    setNodeFormNodeId(nodeId);
                    setNodeFormVisibility(true);
                    setCacheFormVisibility(false);
                  }
                }
                : null,
            () =>
              canDeleteNodes(user)
                ? {
                  icon: DeleteIcon,
                  tooltip: t("scenes.nodesSettings.table.actions.deleteNode"),
                  onClick: (_, {nodeId}) => setNodeDeleteFormNodeId(nodeId)
                }
                : null
          ]}
          options={{
            paging: false,
            draggable: true,
            actionsColumnIndex: 4,
            searchFieldAlignment: "left",
            maxBodyHeight: 400,
            showTitle: false
          }}
          icons={tableIcons}
        />
      </Box>
      <SettingsDialog
        title={t("scenes.nodesSettings.modals.editNode")}
        open={isNodeFormVisible}
        onClose={() => {
          if (nodeFormRef.current) {
            nodeFormRef.current.cancel(() => {
              setNodeFormVisibility(false);
              setNodeFormNodeId(null);
            });
          } else {
            setNodeFormVisibility(false);
            setNodeFormNodeId(null);
          }
        }}
        onSubmit={() => {
          if (nodeFormRef.current) {
            nodeFormRef.current.submit(() => {
              setNodeFormVisibility(false);
              setNodeFormNodeId(null);
            });
          } else {
            setNodeFormVisibility(false);
            setNodeFormNodeId(null);
          }
        }}
        hasSubmit
      >
        <NodeSettingsForm ref={nodeFormRef} nodeId={nodeFormNodeId}/>

      </SettingsDialog>

      <SettingsDialog
        title={t("scenes.nodesSettings.modals.nodeCache", {nodeCode: config.find(node => node.nodeId === cacheFormNodeId)?.code})}
        open={isCacheFormVisible}
        onClose={() => {
          if (cacheFormRef.current) {
            cacheFormRef.current.cancel(() => {
              setCacheFormVisibility(false);
              setCacheFormNodeId(null);
            });
          } else {
            setCacheFormVisibility(false);
            setCacheFormNodeId(null);
          }
        }}
        onSubmit={() => {
          if (cacheFormRef.current) {
            cacheFormRef.current.submit(() => {
              setCacheFormVisibility(false);
              setCacheFormNodeId(null);
            });
          } else {
            setCacheFormVisibility(false);
            setCacheFormNodeId(null);
          }
        }}
        false
      >
        <CacheSettingsForm ref={cacheFormRef} nodeId={cacheFormNodeId}/>
      </SettingsDialog>

      <SettingsDialog
        title={t("scenes.nodesSettings.modals.nodeTemplates", {nodeCode: config.find(node => node.nodeId === templatesNodeId)?.code})}
        open={templatesNodeId !== null}
        onClose={() => {
          if (templatesRef.current) {
            templatesRef.current.cancel(() => setTemplatesNodeId(null));
          } else {
            setTemplatesNodeId(null);
          }
        }}
        maxWidth="md"
      >
        <TemplatesSettingsForm
          ref={templatesRef}
          nodeId={templatesNodeId}
          nodes={config}
          onTemplatesClose={() => {
            setTemplatesNodeId(null);
            onNodesClose();
          }}
        />
      </SettingsDialog>

      <SettingsDialog
        title={t("scenes.nodesSettings.modals.nodePermissions", {nodeCode: config.find(node => node.nodeId === permissionsNodeId)?.code})}
        open={permissionsNodeId !== null}
        onClose={() => {
          if (permissionsRef.current) {
            permissionsRef.current.cancel(() => {
              setPermissionsNodeId(null);
            });
          } else {
            setPermissionsNodeId(null);
          }
        }}
        onSubmit={() => {
          if (permissionsRef.current) {
            permissionsRef.current.submit(() => {
              setPermissionsNodeId(null);
            });
          } else {
            setPermissionsNodeId(null);
          }
        }}
        hasSubmit
      >
        <PermissionsSettingsForm ref={permissionsRef} nodeId={permissionsNodeId}/>
      </SettingsDialog>

      <SettingsDialog
        title={t("scenes.nodesSettings.modals.nodeDashboards", {nodeCode: config.find(node => node.nodeId === dashboardsNodeId)?.code})}
        open={dashboardsNodeId !== null}
        onClose={() => {
          if (dashboardsRef.current) {
            dashboardsRef.current.destroy(() => {
              setDashboardsNodeId(null);
            });
          } else {
            setDashboardsNodeId(null);
          }
        }}
        onSubmit={() => {
          if (dashboardsRef.current) {
            dashboardsRef.current.destroy(() => {
              setDashboardsNodeId(null);
            });
          } else {
            setDashboardsNodeId(null);
          }
        }}
      >
        <DashboardsSettingsForm ref={dashboardsRef} nodeId={dashboardsNodeId}/>
      </SettingsDialog>

      <Dialog
        disableBackdropClick
        disableEscapeKeyDown
        maxWidth="xs"
        open={nodeDeleteFormNodeId !== null}
      >
        <DialogTitle>{t("scenes.nodesSettings.modals.nodeDelete.title")}</DialogTitle>
        <DialogContent>
          {t("scenes.nodesSettings.modals.nodeDelete.content")}
        </DialogContent>
        <DialogActions>
          <Button autoFocus onClick={() => {
            setNodeDeleteFormNodeId(null);
            setCacheFormNodeId(null);
          }}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button onClick={() => {
            deleteNode(nodeDeleteFormNodeId);
            setNodeDeleteFormNodeId(null);
            setCacheFormNodeId(null);
          }}>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>
    </Fragment>
  );
};

export default compose(withStyles(styles), connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(NodesSettingsForm);