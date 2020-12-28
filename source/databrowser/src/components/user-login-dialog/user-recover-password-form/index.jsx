import React, {forwardRef, Fragment, useImperativeHandle} from 'react';
import FormControl from "@material-ui/core/FormControl";
import TextField from "@material-ui/core/TextField";
import {withStyles} from "@material-ui/core";
import Grid from "@material-ui/core/Grid";
import {useForm} from 'react-hook-form'
import {compose} from "redux";
import {connect} from "react-redux";
import {useTranslation} from "react-i18next";
import {sendUserResetPasswordEmail} from "../../../state/user/userActions";
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

  const {register, errors, handleSubmit} = useForm();

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
            style={{marginBottom: 8}}
            name="email"
            label={t("components.userLoginDialog.recoverPasswordModal.fields.email")}
            error={!!errors.email}
            helperText={errors.email?.message}
            variant="outlined"
            required
            inputRef={register({required: t("commons.validation.required")})}
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
  sendConfig: config => dispatch(sendUserResetPasswordEmail(config.email))
});

const UserRecoverPasswordForm = ({config, sendConfig, userErrors}, ref) => {

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

export default compose(connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(UserRecoverPasswordForm);
