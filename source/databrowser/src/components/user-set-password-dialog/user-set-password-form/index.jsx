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
import {setUserPassword} from "../../../state/user/userActions";
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

  const [showPassword, setShowPassword] = useState(false);
  const [showPasswordConfirm, setShowPasswordConfirm] = useState(false);

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
            name="email"
            label={t("components.userSetPasswordDialog.fields.email")}
            error={!!errors.email}
            helperText={errors.email?.message}
            variant="outlined"
            required
            inputRef={register({required: t("commons.validation.required")})}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowPassword(!showPassword)}
                  >
                    {showPassword ? <Visibility/> : <VisibilityOff/>}
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
            name="password"
            label={t("components.userSetPasswordDialog.fields.password")}
            error={!!errors.password}
            helperText={errors.password?.message}
            variant="outlined"
            required
            type={showPassword ? undefined : "password"}
            inputRef={register({required: t("commons.validation.required")})}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowPassword(!showPassword)}
                  >
                    {showPassword ? <Visibility/> : <VisibilityOff/>}
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
            name="passwordConfirm"
            label={t("components.userSetPasswordDialog.fields.passwordConfirm")}
            error={!!errors.passwordConfirm}
            helperText={errors.passwordConfirm?.message}
            variant="outlined"
            required
            type={showPasswordConfirm ? undefined : "password"}
            inputRef={
              register({
                required: t("commons.validation.required"),
                validate: val =>
                  val === watch('password') ||
                  t("components.userSetPasswordDialog.validation.passwordMustBeEqual")
              })}
            InputProps={{
              endAdornment: (
                <InputAdornment position="end">
                  <IconButton
                    onClick={() => setShowPasswordConfirm(!showPasswordConfirm)}
                  >
                    {showPasswordConfirm ? <Visibility/> : <VisibilityOff/>}
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
  sendConfig: (token, config) => dispatch(setUserPassword(config.email, token, config.password)),
});

const UserSetPasswordForm = ({config, token, sendConfig, userErrors}, ref) => {

  return (
    <Fragment>
      <UserErrors errors={userErrors}/>
      <Form
        ref={ref}
        onSubmit={config => sendConfig(token, config)}
      />
    </Fragment>
  );
};

export default compose(connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(UserSetPasswordForm);
