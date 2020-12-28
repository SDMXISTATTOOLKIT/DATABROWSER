import React, {Fragment, useRef} from 'react';
import AccountCircleIcon from '@material-ui/icons/AccountCircle';
import Button from "@material-ui/core/Button";
import {connect} from "react-redux";
import {
  clearUser,
  fetchUser,
  hideUserChangePasswordForm,
  hideUserLoginForm,
  hideUserRegisterModal,
  showUserChangePasswordForm,
  showUserLoginForm,
  showUserRegisterModal
} from "../../state/user/userActions";
import UserLoginDialog from "../user-login-dialog";
import {useTranslation} from "react-i18next";
import UserSettingsForm from "../settings-select/users-settings-form/user-settings-form";
import SettingsDialog from "../settings-dialog";
import Tooltip from "@material-ui/core/Tooltip";
import ButtonSelect from "../button-select";
import {withStyles} from "@material-ui/core";
import {compose} from "redux";
import UserChangePasswordForm from "./user-change-password-form";

const mapStateToProps = state => ({
  user: state.user
});

const mapDispatchToProps = dispatch => ({
  onLogin: (email, password) => dispatch(fetchUser(email, password)),
  onLogout: () => dispatch(clearUser()),
  onLoginFormShow: () => dispatch(showUserLoginForm()),
  onLoginFormHide: () => dispatch(hideUserLoginForm()),
  onChangePasswordShow: () => dispatch(showUserChangePasswordForm()),
  onChangePasswordHide: () => dispatch(hideUserChangePasswordForm()),
  onRegisterModalShow: () => dispatch(showUserRegisterModal()),
  onRegisterModalHide: () => dispatch(hideUserRegisterModal())
});

const styles = () => ({
  email: {
    cursor: "initial",
    color: "gray"
  }
});

const UserSelect = ({
                      classes, user, onLogin, onLogout,
                      onLoginFormShow, onLoginFormHide, onChangePasswordShow, onChangePasswordHide,
                      onRegisterModalShow, onRegisterModalHide
                    }) => {

  const {t} = useTranslation();

  const registerFormRef = useRef();
  const changePasswordFormRef = useRef();

  return (
    <Fragment>
      {user.isAuthenticated
        ? (
          <Tooltip title={user.email}>
            <ButtonSelect value="" icon={<AccountCircleIcon/>} ariaLabel={t("ariaLabels.header.user")}>
              <div className={classes.email}>
                {user.email}
              </div>
              <div onClick={() => onChangePasswordShow()}>
                {t("components.userSelect.changePassword")}
              </div>
              <div onClick={onLogout}>
                {t("components.userSelect.logout")}
              </div>
            </ButtonSelect>
          </Tooltip>
        )
        : (
          <Button color="inherit" onClick={onLoginFormShow}>
            {t("components.userSelect.login")}
          </Button>
        )}
      <UserLoginDialog open={user.isLoginDialogOpen}
                       onSubmit={({email, password}) => onLogin(email, password)}
                       onRegister={() => onRegisterModalShow()}
                       onHide={onLoginFormHide} alert={user.message}/>
      <SettingsDialog
        title={t("components.userSelect.registerModal.title")}
        maxWidth={"md"}
        open={user.isRegisterDialogOpen}
        onClose={() => {
          if (registerFormRef.current) {
            registerFormRef.current.cancel(() => {
              onRegisterModalHide();
            });
          } else {
            onRegisterModalHide();
          }
        }}
        onSubmit={() => {
          if (registerFormRef.current) {
            registerFormRef.current.submit(() => {
            });
          }
        }}
        hasSubmit
        customSubmitLabel={t("components.userSelect.registerModal.submit.label")}
      >
        <UserSettingsForm ref={registerFormRef} userId={-1} isAnonymous/>
      </SettingsDialog>
      <SettingsDialog
        title={t("components.userSelect.changePasswordModal.title")}
        maxWidth={"xs"}
        open={user.isChangePasswordDialogOpen || false}
        onClose={() => {
          if (changePasswordFormRef.current) {
            changePasswordFormRef.current.cancel(() => {
              onChangePasswordHide();
            });
          } else {
            onChangePasswordHide();
          }
        }}
        onSubmit={() => {
          if (changePasswordFormRef.current) {
            changePasswordFormRef.current.submit(() => {
            });
          }
        }}
        hasSubmit
        noMinHeight
      >
        <UserChangePasswordForm ref={changePasswordFormRef}/>
      </SettingsDialog>
    </Fragment>
  );
};

export default compose(
  withStyles(styles),
  connect(mapStateToProps, mapDispatchToProps)
)(UserSelect);