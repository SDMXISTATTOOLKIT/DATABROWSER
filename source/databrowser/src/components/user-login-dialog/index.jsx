import React, {Fragment, useCallback, useRef, useState} from 'react';
import Dialog from "@material-ui/core/Dialog";
import {DialogActions, withStyles} from "@material-ui/core";
import Button from "@material-ui/core/Button";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import TextField from "@material-ui/core/TextField";
import FormControl from "@material-ui/core/FormControl";
import {useForm} from "react-hook-form";
import Alert from "@material-ui/lab/Alert";
import {useTranslation} from "react-i18next";
import SettingsDialog from "../settings-dialog";
import UserRecoverPasswordForm from "./user-recover-password-form";
import {getUserErrorsTranslations} from "../../constants/getUserErrorsTranslations";

const styles = theme => ({
  field: {
    marginTop: theme.spacing(3)
  },
  recoverPassword: {
    textDecoration: "underline",
    color: theme.palette.secondary.main,
    cursor: "pointer"
  }
});

const UserLoginDialog = ({open, onSubmit, onHide, onRegister, classes, alert}) => {

  const {register, errors, handleSubmit} = useForm();

  const {t} = useTranslation();

  const [isRecoverPasswordModalOpen, setIsRecoverPasswordModalOpen] = useState(false);

  const recoverPasswordFormRef = useRef();

  const keyDownHandler = useCallback(evt => {
    if (evt.key === "Enter") {
      handleSubmit(onSubmit)();
    }
  }, [handleSubmit, onSubmit]);

  return (
    <Dialog
      open={open}
      disableEscapeKeyDown
      disableBackdropClick
      maxWidth="xs"
      fullWidth
      onClose={onHide}
      onEntered={() => {
        const userField = document.getElementById("user-login-form__text-field__email");
        const pswField = document.getElementById("user-login-form__text-field__password");
        userField.addEventListener("keydown", keyDownHandler);
        pswField.addEventListener("keydown", keyDownHandler);
      }}
      onExit={() => {
        const userField = document.getElementById("user-login-form__text-field__email");
        const pswField = document.getElementById("user-login-form__text-field__password");
        userField.removeEventListener("keydown", keyDownHandler);
        pswField.removeEventListener("keydown", keyDownHandler);
      }}
    >
      <DialogTitle>
        {t("components.userLoginDialog.title")}
      </DialogTitle>
      <DialogContent>
        {alert && (
          <Alert severity="error">{getUserErrorsTranslations(t)[alert] || t("errors.user.generic")}</Alert>
        )}
        <FormControl fullWidth className={alert ? classes.field : undefined}>
          <TextField
            id="user-login-form__text-field__email"
            name="email"
            label={t("components.userLoginDialog.fields.email")}
            error={!!errors.email}
            helperText={errors.email?.message}
            variant="outlined"
            required
            inputRef={register({required: t("commons.validation.required")})}
          />
        </FormControl>
        <FormControl fullWidth className={classes.field}>
          <TextField
            id="user-login-form__text-field__password"
            name="password"
            type="password"
            label={t("components.userLoginDialog.fields.password")}
            inputRef={register({required: t("commons.validation.required")})}
            error={!!errors.password}
            helperText={
              <Fragment>
                {errors.password?.message}
                <span
                  onClick={() => setIsRecoverPasswordModalOpen(true)}
                  className={classes.recoverPassword}
                  style={{
                    marginLeft: errors.password?.message ? 2 : 0
                  }}>
                  {t("components.userLoginDialog.recoverPassword")}
                </span>
              </Fragment>
            }
            variant="outlined"
            required
          />
        </FormControl>
      </DialogContent>
      <DialogActions>
        <Button onClick={onHide}>
          {t("commons.confirm.cancel")}
        </Button>
        <Button onClick={onRegister}>
          {t("components.userLoginDialog.register")}
        </Button>
        <Button onClick={handleSubmit(onSubmit)}>
          {t("components.userLoginDialog.login")}
        </Button>
      </DialogActions>
      <SettingsDialog
        title={t("components.userLoginDialog.recoverPasswordModal.title")}
        maxWidth={"xs"}
        open={isRecoverPasswordModalOpen}
        onClose={() => {
          if (recoverPasswordFormRef.current) {
            recoverPasswordFormRef.current.cancel(() => {
              setIsRecoverPasswordModalOpen(false);
            });
          } else {
            setIsRecoverPasswordModalOpen(false);
          }
        }}
        onSubmit={() => {
          if (recoverPasswordFormRef.current) {
            recoverPasswordFormRef.current.submit(() => {
              setIsRecoverPasswordModalOpen(false);
            });
          }
        }}
        hasSubmit
        noMinHeight
      >
        <UserRecoverPasswordForm ref={recoverPasswordFormRef}/>
      </SettingsDialog>
    </Dialog>
  );
};

export default withStyles(styles)(UserLoginDialog);
