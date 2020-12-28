import React, {forwardRef, Fragment, useEffect, useImperativeHandle, useState} from 'react';
import FormControl from "@material-ui/core/FormControl";
import TextField from "@material-ui/core/TextField";
import {Checkbox, withStyles} from "@material-ui/core";
import Grid from "@material-ui/core/Grid";
import {useForm} from 'react-hook-form'
import {compose} from "redux";
import {connect} from "react-redux";
import {
  clearUsersConfigUser,
  fetchUsersConfigUser,
  sendUsersConfigUserCreate,
  sendUsersConfigUserEdit,
} from "../../../../state/usersConfig/usersConfigActions";
import {useTranslation} from "react-i18next";
import IconButton from "@material-ui/core/IconButton";
import InputAdornment from "@material-ui/core/InputAdornment";
import {Visibility, VisibilityOff} from "@material-ui/icons";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import FormHelperText from "@material-ui/core/FormHelperText";
import SanitizedHTML from "../../../sanitized-html";
import MenuItem from "@material-ui/core/MenuItem";
import {localizeI18nObj} from "../../../../utils/i18n";
import UserErrors from "../../../user-errors";

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
  },
  disclaimerLink: {
    marginLeft: theme.spacing(1),
    color: theme.palette.secondary.main,
    textDecoration: "underline"
  }
});


const Form = compose(withStyles(styles), forwardRef)(({classes, config, onSubmit, onCancel, isAnonymous, hub, types, languages, language}, ref) => {

  const {register, errors, handleSubmit, watch, setValue} = useForm({
    defaultValues: {
      isActive: true,
      type: "null",
      organization: "",
      disclaimerAccepted: isAnonymous ? false : undefined,
      ...config
    }
  });

  const [showPassword, setShowPassword] = useState(false);
  const [showPasswordConfirm, setShowPasswordConfirm] = useState(false);
  const [isDisclaimerModalOpen, setIsDisclaimerModalOpen] = useState(false);

  const {t} = useTranslation();

  useImperativeHandle(ref, () => ({
    submit(f) {
      handleSubmit(val => {
        const data = {
          ...config,
          ...val,
          type: val.type !== "null" ? val.type : null,
          disclaimerAccepted: undefined,
          emailConfirm: undefined,
          passwordConfirm: undefined
        };

        onSubmit(data);
        f(data);
      })();
    },
    cancel(f) {
      onCancel();
      f();
    }
  }));

  useEffect(() => {

    register("type");

    if (isAnonymous) {
      register({
          name: "disclaimerAccepted"
        },
        {
          validate: val => val || t("scenes.usersSettings.userSettings.validation.mustAcceptDisclaimer")
        });
    } else {
      register("isActive");
    }

  }, [isAnonymous, register, t]);

  return (
    <Grid container spacing={3}>
      <Grid item xs={6}>
        <FormControl fullWidth>
          <TextField
            disabled={!!config}
            name="email"
            label={t("scenes.usersSettings.userSettings.fields.email")}
            inputRef={register({required: t("commons.validation.required")})}
            error={!!errors.email}
            helperText={errors.email?.message}
            variant="outlined"
            required
          />
        </FormControl>
      </Grid>
      {!config && (
        <Grid item xs={6}>
          <FormControl fullWidth>
            <TextField
              disabled={!!config}
              name="emailConfirm"
              label={t("scenes.usersSettings.userSettings.fields.emailConfirm")}
              inputRef={
                register({
                  required: t("commons.validation.required"),
                  validate: val =>
                    val === watch('email') ||
                    t("scenes.usersSettings.userSettings.validation.emailsMustBeEqual")
                })
              }
              error={!!errors.emailConfirm}
              helperText={errors.emailConfirm?.message}
              variant="outlined"
              required
            />
          </FormControl>
        </Grid>
      )}
      {!isAnonymous && (
        <Grid item xs={12}>
          <FormControl fullWidth>
            <FormControlLabel
              label={t("scenes.usersSettings.userSettings.fields.isActive.label")}
              control={
                <Checkbox
                  name="isActive"
                  required
                  checked={watch('isActive')}
                  onChange={(e, value) => setValue('isActive', value)}
                />
              }
            />
          </FormControl>
        </Grid>
      )}
      {types && types.length > 0 && (
        <Grid item xs={12}>
          <FormControl fullWidth>
            <TextField
              name="type"
              select
              label={t("scenes.usersSettings.userSettings.fields.type.label")}
              variant="outlined"
              onChange={e => setValue('type', e.target.value)}
              value={watch("type") || ''}
              helperText={errors.type?.message}
              error={!!errors.type}
            >
              <MenuItem value="null">
                <i>{t("scenes.usersSettings.userSettings.fields.type.values.empty")}</i>
              </MenuItem>
              {types.map(({id, label}) =>
                <MenuItem value={id} key={id}>
                  {localizeI18nObj(label, language, languages)}
                </MenuItem>
              )}
            </TextField>
          </FormControl>
        </Grid>
      )}
      <Grid item xs={6}>
        <FormControl fullWidth>
          <TextField
            name="firstName"
            label={t("scenes.usersSettings.userSettings.fields.firstName")}
            error={!!errors.firstName}
            helperText={errors.firstName?.message}
            variant="outlined"
            required
            inputRef={register({required: t("commons.validation.required")})}
          />
        </FormControl>
      </Grid>
      <Grid item xs={6}>
        <FormControl fullWidth>
          <TextField
            name="lastName"
            label={t("scenes.usersSettings.userSettings.fields.lastName")}
            inputRef={register({required: t("commons.validation.required")})}
            error={!!errors.lastName}
            helperText={errors.lastName?.message}
            variant="outlined"
            required
          />
        </FormControl>
      </Grid>
      <Grid item xs={12}>
        <FormControl fullWidth>
          <TextField
            name="organization"
            label={t("scenes.usersSettings.userSettings.fields.organization")}
            error={!!errors.organization}
            helperText={errors.organization?.message}
            variant="outlined"
            inputRef={register}
          />
        </FormControl>
      </Grid>
      {!config && (
        <Fragment>
          <Grid item xs={6}>
            <FormControl fullWidth>
              <TextField
                name="password"
                label={t("scenes.usersSettings.userSettings.fields.password")}
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
          <Grid item xs={6}>
            <FormControl fullWidth>
              <TextField
                name="passwordConfirm"
                label={t("scenes.usersSettings.userSettings.fields.passwordConfirm")}
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
                      t("scenes.usersSettings.userSettings.validation.passwordsMustBeEqual")
                  })
                }
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
          {isAnonymous && (
            <Grid item xs={12}>
              <FormControl fullWidth error={!!errors.disclaimerAccepted}>
                <FormControlLabel
                  label={
                    <Fragment>
                      {t("scenes.usersSettings.userSettings.fields.disclaimerAccepted.label")}
                      <span
                        onClick={() => setIsDisclaimerModalOpen(true)}
                        className={classes.disclaimerLink}
                      >
                        {t("scenes.usersSettings.userSettings.fields.disclaimerAccepted.link")}
                      </span>
                    </Fragment>
                  }
                  control={
                    <Checkbox
                      name="disclaimerAccepted"
                      required
                      checked={watch('disclaimerAccepted')}
                      onChange={(e, value) => setValue('disclaimerAccepted', value)}
                    />
                  }
                />
                {errors.disclaimerAccepted?.message && (
                  <FormHelperText id="my-helper-text">
                    {errors.disclaimerAccepted?.message}
                  </FormHelperText>
                )}
              </FormControl>
            </Grid>
          )}
          <Dialog
            open={isDisclaimerModalOpen}
            fullWidth
            maxWidth="md"
            onClose={() => setIsDisclaimerModalOpen(false)}
          >
            <DialogTitle>
              {t("scenes.usersSettings.userSettings.disclaimerModal.title")}
            </DialogTitle>
            <DialogContent>
              <SanitizedHTML html={hub.hub.disclaimer} style={{maxHeight: 460}} allowTarget/>
            </DialogContent>
            <DialogActions>
              <Button onClick={() => setIsDisclaimerModalOpen(false)}>
                {t("commons.confirm.cancel")}
              </Button>
            </DialogActions>
          </Dialog>
        </Fragment>
      )}
    </Grid>
  );
});

const mapStateToProps = state => ({
  config: state.usersConfig.user,
  userErrors: state.usersConfig.userErrors,
  hub: state.hub,
  appConfig: state.appConfig,
  languages: state.app.languages,
  language: state.app.language
});

const mapDispatchToProps = dispatch => ({
  fetchConfig: nodeId => dispatch(fetchUsersConfigUser(nodeId)),
  sendConfigCreate: (config, isAnonymous) => dispatch(sendUsersConfigUserCreate(config, isAnonymous)),
  sendConfig: config => dispatch(sendUsersConfigUserEdit(config)),
  clearConfig: () => dispatch(clearUsersConfigUser())
});

const UserSettingsForm = ({config, userId, fetchConfig, sendConfigCreate, sendConfig, clearConfig, userErrors, isAnonymous, hub, appConfig, languages, language}, ref) => {

  const [needConfig, setNeedConfig] = useState(userId >= 0);

  useEffect(() => {

    if (needConfig) {
      setNeedConfig(false);
      fetchConfig(userId);
    }
  }, [config, needConfig, setNeedConfig, fetchConfig, userId]);
  
  return (userId === -1 || config) && (
    <Fragment>
      <UserErrors errors={userErrors}/>
      <Form
        config={userId === -1 ? undefined : config}
        ref={ref}
        onSubmit={
          userId === -1
            ? (config) => sendConfigCreate(config, isAnonymous)
            : sendConfig
        }
        onCancel={clearConfig}
        isAnonymous={isAnonymous}
        hub={hub}
        types={appConfig?.user?.typeOptions}
        languages={languages}
        language={language}
      />
    </Fragment>

  );
};

export default compose(connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(UserSettingsForm);
