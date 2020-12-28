import React, {forwardRef, useEffect, useImperativeHandle, useRef, useState} from 'react';
import Search from "@material-ui/icons/Search";
import Clear from "@material-ui/icons/Clear";
import ArrowUpward from "@material-ui/icons/ArrowUpward";
import EditIcon from '@material-ui/icons/Edit';
import {Box, withStyles} from "@material-ui/core";
import SettingsDialog from "../../settings-dialog";
import AddIcon from '@material-ui/icons/Add';
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Grid from "@material-ui/core/Grid";
import {compose} from "redux";
import DeleteIcon from '@material-ui/icons/Delete';
import UserSettingsForm from "./user-settings-form";
import {connect} from "react-redux";
import {
  clearUsersConfigUsers,
  deleteUsersConfigUser,
  fetchUsersConfigUsers,
  hideUserConfigForm,
  showUserConfigForm
} from "../../../state/usersConfig/usersConfigActions";
import {useTranslation} from "react-i18next";
import CustomMaterialTable from "../../custom-material-table";
import {MTableToolbar} from "material-table";
import "./style.css";
import {localizeI18nObj} from "../../../utils/i18n";

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
  config: state.usersConfig.users,
  userErrors: state.usersConfig.userErrors,
  userId: state.usersConfig.userId,
  appConfig: state.appConfig,
  languages: state.app.languages,
  language: state.app.language
});

const mapDispatchToProps = dispatch => ({
  fetchConfig: () => dispatch(fetchUsersConfigUsers()),
  deleteUser: userId => dispatch(deleteUsersConfigUser(userId)),
  clearConfig: () => dispatch(clearUsersConfigUsers()),
  onUserFormShow: userId => dispatch(showUserConfigForm(userId)),
  onUserFormHide: () => dispatch(hideUserConfigForm())
});

const UsersSettingsForm = ({classes, config, fetchConfig, deleteUser, clearConfig, users, userErrors, userId, onUserFormShow, onUserFormHide, appConfig, languages, language}, ref) => {

  const [needConfig, setNeedConfig] = useState(true);

  useEffect(() => {

    if (needConfig) {
      setNeedConfig(false);
      fetchConfig();
    }
  }, [config, needConfig, setNeedConfig, fetchConfig]);

  useImperativeHandle(ref, () => ({
    cancel(f) {
      clearConfig();
      f();
    }
  }));

  const [userDeleteFormUserId, setUserDeleteFormUserId] = useState(null);

  const userFormRef = useRef();

  const {t} = useTranslation();

  return config && (
    <div className="users-settings-form__table">
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
                  onClick={() => onUserFormShow(-1)}>
                  {t("scenes.usersSettings.createUser")}
                </Button>
              </Grid>
            </Grid>
        }}
        columns={[
          {field: "firstName", title: t("scenes.usersSettings.table.columns.userFirstName")},
          {field: "lastName", title: t("scenes.usersSettings.table.columns.userLastName")},
          {
            field: "type",
            title: t("scenes.usersSettings.table.columns.type"),
            render: ({type}) => localizeI18nObj(appConfig?.user?.typeOptions?.find(({id}) => type === id)?.label, language, languages)
          },
          {field: "organization", title: t("scenes.usersSettings.table.columns.userOrganization")},
          {field: "email", title: t("scenes.usersSettings.table.columns.userEmail")},
          {
            field: "isActive",
            title: t("scenes.usersSettings.table.columns.isActive.title"),
            render: ({isActive}) =>
              isActive
                ? t("scenes.usersSettings.table.columns.isActive.values.true")
                : t("scenes.usersSettings.table.columns.isActive.values.false")
          }
        ]}
        data={config}
        actions={[
          {
            icon: EditIcon,
            tooltip: t("scenes.usersSettings.table.actions.editUser"),
            onClick: (_, {userId}) => {
              onUserFormShow(userId);
            }
          },
          {
            icon: DeleteIcon,
            tooltip: t("scenes.usersSettings.table.actions.deleteUser"),
            onClick: (_, {userId}) => {
              setUserDeleteFormUserId(userId);
            }
          }
        ]}
        options={{
          paging: false,
          draggable: true,
          actionsColumnIndex: 5,
          searchFieldAlignment: "left",
          maxBodyHeight: 400,
          showTitle: false
        }}
        icons={tableIcons}
      />
      <SettingsDialog
        title={
          userId === -1
            ? t("scenes.usersSettings.modals.createUser")
            : t("scenes.usersSettings.modals.editUser")
        }
        maxWidth={"sm"}
        open={userId !== null || userErrors !== null}
        onClose={() => {
          if (userFormRef.current) {
            userFormRef.current.cancel(() => {
              onUserFormHide();
            });
          } else {
            onUserFormHide();
          }
        }}
        onSubmit={() => {
          if (userFormRef.current) {
            userFormRef.current.submit(() => {
            });
          }
        }}
        hasSubmit
      >
        <UserSettingsForm ref={userFormRef} userId={userId}/>
      </SettingsDialog>

      <Dialog
        disableBackdropClick
        disableEscapeKeyDown
        maxWidth="xs"
        open={userDeleteFormUserId !== null}
      >
        <DialogTitle>{t("scenes.usersSettings.modals.deleteUser.title")}</DialogTitle>
        <DialogContent>
          {t("scenes.usersSettings.modals.deleteUser.content")}
        </DialogContent>
        <DialogActions>
          <Button autoFocus onClick={() => {
            setUserDeleteFormUserId(null);
          }}>
            {t("commons.confirm.cancel")}
          </Button>
          <Button onClick={() => {
            deleteUser(userDeleteFormUserId);
            setUserDeleteFormUserId(null);
          }}>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
};

export default compose(
  withStyles(styles),
  connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}),
  forwardRef
)(UsersSettingsForm);