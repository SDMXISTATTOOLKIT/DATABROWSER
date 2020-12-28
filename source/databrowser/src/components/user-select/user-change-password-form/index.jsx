import React, {forwardRef, Fragment, useImperativeHandle, useState} from 'react';
import FormControl from "@material-ui/core/FormControl";
import TextField from "@material-ui/core/TextField";
import {withStyles} from "@material-ui/core";
import Grid from "@material-ui/core/Grid";
import {useForm} from 'react-hook-form'
import {compose} from "redux";
import {connect} from "react-redux";
import {useTranslation} from "react-i18next";
import IconButton from "@material-ui/core/IconButton";
import InputAdornment from "@material-ui/core/InputAdornment";
import {Visibility, VisibilityOff} from "@material-ui/icons";
import {changeUserPassword} from "../../../state/user/userActions";
import UserErrors from "../../user-errors";

const styles = theme => ({
  root: {
    height: 440
  },
  field: {
    marginTop: theme.spacing(3)
  },
  paper: {
    marginTop: theme.spacing(2),
    padding: theme.spacing(3)
  },
  tabContent: {
    overflowY: "auto",
    overflowX: "hidden",
    height: "calc(100% - 56px)",
    marginTop: 8,
    padding: "0 4px"
  },
  title: {
    fontSize: 16
  }
});


const Form = compose(withStyles(styles), forwardRef)(({classes, config, onSubmit}, ref) => {

  const {register, errors, handleSubmit, watch} = useForm();

  const [showOldPassword, setShowOldPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showNewPasswordConfirm, setShowNewPasswordConfirm] = useState(false);

  const {t} = useTranslation();

  useImperativeHandle(ref, () => ({
    submit(f) {
      handleSubmit(val => {
        onSubmit(val);
        f(val);
      })();
    },
    cancel(f) {
      f();
    }
  }));

  return (
    <Grid container spacing={3}>
      <Grid item xs={12}>
        <FormControl fullWidth>
          <TextField
            name="oldPassword"
            label={t("components.userSelect.changePasswordModal.fields.oldPassword")}
            error={!!errors.oldPassword}
            helperText={errors.oldPassword?.message}
            variant="outlined"
            required
            type={showOldPassword ? undefined : "password"}
            inputRef={register({required: t("commons.validation.required")})}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowOldPassword(!showOldPassword)}
                  >
                    {showOldPassword ? <Visibility/> : <VisibilityOff/>}
                  </IconButton>
                </InputAdornment>
              )
            }}
          />
        </FormControl>
      </Grid>
      <Grid item xs={12}>
        <FormControl fullWidth>
          <TextField
            name="newPassword"
            label={t("components.userSelect.changePasswordModal.fields.newPassword")}
            error={!!errors.newPassword}
            helperText={errors.newPassword?.message}
            variant="outlined"
            required
            type={showNewPassword ? undefined : "password"}
            inputRef={register({required: t("commons.validation.required")})}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowNewPassword(!showNewPassword)}
                  >
                    {showNewPassword ? <Visibility/> : <VisibilityOff/>}
                  </IconButton>
                </InputAdornment>
              )
            }}
            inputProps={{
              autoComplete: "new-password"
            }}
          />
        </FormControl>
      </Grid>
      <Grid item xs={12}>
        <FormControl fullWidth>
          <TextField
            name="newPasswordConfirm"
            label={t("components.userSelect.changePasswordModal.fields.newPasswordConfirm")}
            error={!!errors.newPasswordConfirm}
            helperText={errors.newPasswordConfirm?.message}
            variant="outlined"
            required
            type={showNewPasswordConfirm ? undefined : "password"}
            inputRef={
              register({
                required: t("commons.validation.required"),
                validate: val =>
                  val === watch('newPassword') ||
                  t("components.userSelect.changePasswordModal.validation.newPasswordMustBeEqual")
              })}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowNewPasswordConfirm(!showNewPasswordConfirm)}
                  >
                    {showNewPasswordConfirm ? <Visibility/> : <VisibilityOff/>}
                  </IconButton>
                </InputAdornment>
              )
            }}
            inputProps={{
              autoComplete: "new-password"
            }}
          />
        </FormControl>
      </Grid>
    </Grid>
  );
});

const mapStateToProps = state => ({
  config: state.usersConfig.user,
  userErrors: state.usersConfig.userErrors
});

const mapDispatchToProps = dispatch => ({
  sendConfig: config => dispatch(changeUserPassword(config.oldPassword, config.newPassword))
});

const UserChangePasswordForm = ({config, sendConfig, userErrors}, ref) => {

  return (
    <Fragment>
      <UserErrors errors={userErrors}/>
      <Form
        ref={ref}
        onSubmit={sendConfig}
      />
    </Fragment>
  );
};

export default compose(connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(UserChangePasswordForm);
