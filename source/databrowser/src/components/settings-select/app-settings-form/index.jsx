import React, {forwardRef, Fragment, useEffect, useImperativeHandle, useState} from 'react';
import TextField from "@material-ui/core/TextField";
import FormControl from "@material-ui/core/FormControl";
import {withStyles} from "@material-ui/core";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import Box from "@material-ui/core/Box";
import {useForm} from 'react-hook-form'
import {compose} from "redux";
import {
  addHubConfigDashboardsDashboard,
  clearAllHubConfigDashboards,
  clearHubConfig,
  clearHubConfigDashboards,
  closeHubConfig,
  fetchAllHubConfigDashboards,
  fetchHubConfig,
  fetchHubConfigDashboards,
  removeHubConfigDashboardsDashboard,
  sendHubConfig,
  sendHubConfigDashboardsOrders
} from "../../../state/hubConfig/hubConfigActions";
import {connect} from "react-redux";
import FileInput from "../../file-input";
import DashboardsManager from "../../dashboards-manager";
import {canManageAppDashboards} from "../../../utils/user";
import {useTranslation} from "react-i18next";
import I18nTextField from "../../i18n-text-field";
import {validateI18nObj} from "../../../utils/i18n";
import I18nHtmlEditor from "../../i18n-html-editor";
import Autocomplete from "@material-ui/lab/Autocomplete";
import Chip from "@material-ui/core/Chip";
import MenuItem from "@material-ui/core/MenuItem";

const styles = theme => ({
  root: {
    height: 440
  },
  field: {
    marginTop: theme.spacing(3)
  },
  tabContent: {
    overflowY: "auto",
    overflowX: "hidden",
    height: "calc(100% - 56px)",
    marginTop: 8,
    padding: "0 4px"
  },
  disclaimerLabel: {
    fontSize: "1rem",
    marginTop: theme.spacing(3)
  }
});

const mapStateToProps = state => ({
  config: state.hubConfig.hub,
  user: state.user,
  dashboards: state.hubConfig.hubDashboards,
  allDashboards: state.hubConfig.allDashboards
});

const mapDispatchToProps = dispatch => ({
  clearConfig: () => dispatch(clearHubConfig()),
  fetchConfig: () => dispatch(fetchHubConfig()),
  sendConfig: config => dispatch(sendHubConfig(config)),
  fetchDashboards: () => dispatch(fetchHubConfigDashboards()),
  fetchAllDashboards: () => dispatch(fetchAllHubConfigDashboards()),
  clearDashboards: () => dispatch(clearHubConfigDashboards()),
  clearAllDashboards: () => dispatch(clearAllHubConfigDashboards()),
  addDashboard: dashboardId => dispatch(addHubConfigDashboardsDashboard(dashboardId)),
  removeDashboard: dashboardId => dispatch(removeHubConfigDashboardsDashboard(dashboardId)),
  sendDashboardsOrders: orderedDashboarIds => dispatch(sendHubConfigDashboardsOrders(orderedDashboarIds)),
  onClose: () => dispatch(closeHubConfig())
});

const Form = compose(withStyles(styles), forwardRef)(({
                                                        classes,
                                                        config,
                                                        onSubmit,
                                                        onCancel,
                                                        user,
                                                        dashboards,
                                                        allDashboards,
                                                        fetchDashboards,
                                                        fetchAllDashboards,
                                                        clearDashboards,
                                                        clearAllDashboards,
                                                        addDashboard,
                                                        removeDashboard,
                                                        sendDashboardsOrders
                                                      }, ref) => {

  const [tab, setTab] = useState("general");
  const dashboardsManagerRef = React.createRef();
  const {t} = useTranslation();

  const configExtras = config.extras ? JSON.parse(config.extras) : {};

  const {register, errors, handleSubmit, watch, setValue} = useForm({
    defaultValues: {
      ...config,
      pageSize: configExtras.PageSize
    }
  });

  useImperativeHandle(ref, () => ({
    submit(f) {
      handleSubmit(val => {
        const data = {
          ...config,
          ...val,
          pageSize: undefined
        };

        data.extras = JSON.stringify({
          ...JSON.parse(data.extras || "{}"),
          PageSize: val.pageSize
        });

        onSubmit(data);
        f(data);
        if (dashboardsManagerRef.current) {
          dashboardsManagerRef.current.destroy();
        }
      })();
    },
    cancel(f) {
      onCancel();
      f();
      if (dashboardsManagerRef.current) {
        dashboardsManagerRef.current.destroy();
      }
    }
  }));

  /* custom register */
  useEffect(() => {
    register({
      name: 'title'
    }, {
      validate: val => validateI18nObj(val) || t("commons.validation.requiredAnyLanguage")
    });
    register({name: 'slogan'});
    register({
      name: 'supportedLanguages'
    }, {
      validate: val => val.length > 0 || t("commons.validation.required")
    });
    register({
      name: 'defaultLanguage'
    }, {
      required: t("commons.validation.required")
    });
    register({
      name: 'maxObservationsAfterCriteria'
    }, {
      required: t("commons.validation.required"),
      min: {
        value: 1,
        message: t("commons.validation.positiveInteger")
      }
    })
    register({
      name: "maxCells"
    }, {
      required: t("commons.validation.required"),
      min: {
        value: 1,
        message: t("commons.validation.positiveInteger")
      }
    })
    register({name: 'backgroundMediaURL'});
    register({name: 'logoURL'});
    register({name: 'description'});
    register({
      name: 'pageSize'
    }, {
      validate: val => !val || val > 0 || t("commons.validation.positiveInteger")
    });
    register({
      name: "disclaimer"
    });
  }, [register, t]);

  const defaultLanguage = watch('defaultLanguage');
  const supportedLanguages = watch('supportedLanguages');

  return (
    <Box className={classes.root}>
      <Tabs
        value={tab}
        onChange={(_, tab) => {
          setTab(tab)
        }}
      >
        <Tab value="general" label={t("scenes.appSettings.tabs.general")}/>
        <Tab value="infos" label={t("scenes.appSettings.tabs.information")}/>
        <Tab value="users" label={t("scenes.appSettings.tabs.users")}/>
        {canManageAppDashboards(user) && (<Tab value="dashboards" label={t("scenes.appSettings.tabs.dashboards")}/>)}
      </Tabs>
      <div className={classes.tabContent}>
        {tab === "general" && (
          <Fragment>
            <FormControl fullWidth className={classes.field}>
              <I18nTextField
                name="title"
                label={t("scenes.appSettings.fields.title.label")}
                required
                error={!!errors.title}
                helperText={errors.title?.message}
                variant="outlined"
                value={watch('title') || {}}
                onChange={value => setValue('title', value)}
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <I18nTextField
                name="slogan"
                label={t("scenes.appSettings.fields.slogan.label")}
                variant="outlined"
                value={watch('slogan') || {}}
                onChange={value => setValue('slogan', value)}
              />
            </FormControl>
            <Autocomplete
              multiple
              variant="outlined"
              freeSolo
              options={[]}
              value={supportedLanguages || []}
              renderTags={(value, getTagProps) =>
                value.map((option, index) => (
                  <Chip variant="outlined" label={option} {...getTagProps({index})} />
                ))
              }
              required
              onChange={(e, val) => {
                setValue('supportedLanguages', val);
                if (defaultLanguage && !val.includes(defaultLanguage)) {
                  setValue('defaultLanguage', "");
                }
              }}
              renderInput={params => (
                <FormControl fullWidth className={classes.field}>
                  <TextField
                    label={t("scenes.appSettings.fields.supportedLanguages.label")}
                    {...params}
                    variant="outlined"
                    placeholder={t("scenes.appSettings.fields.supportedLanguages.placeholder")}
                    error={!!errors.supportedLanguages}
                    helperText={
                      errors.supportedLanguages?.message || (
                        <Fragment>
                          {t("scenes.appSettings.fields.supportedLanguages.helper.text")}
                          <a
                            href="https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes"
                            target="_blank"
                            rel="noopener noreferrer"
                            style={{marginLeft: 4}}
                          >
                            {t("scenes.appSettings.fields.supportedLanguages.helper.linkText")}
                          </a>
                        </Fragment>
                      )}
                  />
                </FormControl>
              )}
            />
            <FormControl fullWidth className={classes.field}>
              <TextField
                name="defaultLanguage"
                select
                required
                label={t("scenes.nodeSettings.fields.defaultLanguage.label")}
                onChange={e => setValue('defaultLanguage', e.target.value)}
                value={watch("defaultLanguage") || ''}
                error={!!errors.defaultLanguage}
                helperText={errors.defaultLanguage?.message || t("scenes.nodeSettings.fields.defaultLanguage.helper")}
                variant="outlined"
              >
                {supportedLanguages.map((val, index) =>
                  <MenuItem key={index} value={val}>{val}</MenuItem>
                )}
              </TextField>
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <TextField
                name="maxObservationsAfterCriteria"
                label={t("scenes.appSettings.fields.maxObservationsAfterCriteria.label")}
                required
                type="number"
                error={!!errors.maxObservationsAfterCriteria}
                helperText={errors.maxObservationsAfterCriteria?.message}
                variant="outlined"
                value={watch('maxObservationsAfterCriteria') || ''}
                onChange={({target}) => setValue('maxObservationsAfterCriteria', target.value)}
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <TextField
                name="maxCells"
                label={t("scenes.appSettings.fields.maxCells.label")}
                required
                error={!!errors.maxCells}
                helperText={errors.maxCells?.message}
                variant="outlined"
                type="number"
                value={watch('maxCells') || ''}
                onChange={({target}) => setValue('maxCells', target.value)}
              />
            </FormControl>
            {/*<FormControl fullWidth className={classes.field}>
              <TextField
                name="pageSize"
                label={t("scenes.appSettings.fields.pageSize.label")}
                variant="outlined"
                type="number"
                error={!!errors.pageSize}
                helperText={errors.pageSize?.message}
                value={watch('pageSize') || ''}
                onChange={({target}) => setValue('pageSize', Number(target.value))}
              />
            </FormControl>*/}
            <FormControl fullWidth className={classes.field}>
              <FileInput
                label={t("scenes.appSettings.fields.backgroundMediaURL.label")}
                value={watch('backgroundMediaURL')}
                onChange={value => setValue('backgroundMediaURL', value)}
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <FileInput
                label={t("scenes.appSettings.fields.logoURL.label")}
                value={watch('logoURL')}
                onChange={value => setValue('logoURL', value)}
              />
            </FormControl>
          </Fragment>
        )}
        {tab === "infos" && (
          <I18nHtmlEditor
            value={watch('description')}
            onChange={val => setValue('description', val)}
          />
        )}
        {tab === "users" && (
          <Fragment>
            {/* <Autocomplete
              multiple
              variant="outlined"
              freeSolo
              options={[]}
              value={watch('userTypes') || []}
              renderTags={(value, getTagProps) =>
                value.map((option, index) => (
                  <Chip variant="outlined" label={option} {...getTagProps({index})} />
                ))
              }
              required
              onChange={(e, val) => setValue('userTypes', val)}
              renderInput={params => (
                <FormControl fullWidth className={classes.field}>
                  <TextField
                    label={t("scenes.appSettings.fields.userTypes.label")}
                    {...params}
                    variant="outlined"
                    placeholder={t("scenes.appSettings.fields.userTypes.placeholder")}
                    error={!!errors.userTypes}
                    helperText={errors.userTypes?.message}
                  />
                </FormControl>
              )}
            />*/}
            <div className={classes.disclaimerLabel}>
              {t("scenes.appSettings.fields.disclaimer.label")}:
            </div>
            <I18nHtmlEditor
              value={watch('disclaimer')}
              onChange={val => setValue('disclaimer', val)}
            />
          </Fragment>
        )}
        {canManageAppDashboards(user) && tab === "dashboards" && (
          <DashboardsManager
            dashboards={dashboards}
            allDashboards={allDashboards}
            fetchDashboards={fetchDashboards}
            fetchAllDashboards={fetchAllDashboards}
            clearDashboards={clearDashboards}
            clearAllDashboards={clearAllDashboards}
            addDashboard={addDashboard}
            removeDashboard={removeDashboard}
            sendDashboardsOrders={sendDashboardsOrders}
            ref={dashboardsManagerRef}
          />
        )}
      </div>
    </Box>
  );
});

const AppSettingsForm = ({
                           config,
                           fetchConfig,
                           sendConfig,
                           clearConfig,
                           user,
                           dashboards,
                           allDashboards,
                           fetchDashboards,
                           fetchAllDashboards,
                           clearDashboards,
                           clearAllDashboards,
                           addDashboard,
                           removeDashboard,
                           sendDashboardsOrders,
                           onClose
                         }, ref) => {

  const [needConfig, setNeedConfig] = useState(true);

  useEffect(() => {

    if (needConfig) {
      setNeedConfig(false);
      fetchConfig();
    }
  }, [config, needConfig, setNeedConfig, fetchConfig]);

  return (config &&
    <Form
      config={config}
      ref={ref}
      onSubmit={sendConfig}
      onCancel={() => {
        clearConfig();
        onClose();
      }}
      user={user}
      dashboards={dashboards}
      allDashboards={allDashboards}
      fetchDashboards={fetchDashboards}
      fetchAllDashboards={fetchAllDashboards}
      clearDashboards={clearDashboards}
      clearAllDashboards={clearAllDashboards}
      addDashboard={addDashboard}
      removeDashboard={removeDashboard}
      sendDashboardsOrders={sendDashboardsOrders}
    />);

};

export default compose(connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(AppSettingsForm);