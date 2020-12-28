import React, {forwardRef, Fragment, useEffect, useImperativeHandle, useState} from 'react';
import FormControl from "@material-ui/core/FormControl";
import TextField from "@material-ui/core/TextField";
import {Checkbox, withStyles} from "@material-ui/core";
import Grid from "@material-ui/core/Grid";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import Box from "@material-ui/core/Box";
import Paper from "@material-ui/core/Paper";
import {useForm} from 'react-hook-form'
import FormControlLabel from "@material-ui/core/FormControlLabel";
import {compose} from "redux";
import validator from "validator";
import {connect} from "react-redux";
import {
  clearNodesConfigNode,
  fetchNodesConfigNode,
  sendNodesConfigNodeCreate,
  sendNodesConfigNodeEdit
} from "../../../../state/nodesConfig/nodesConfigActions";
import {
  ALL_FULL,
  ALL_PARTIAL,
  STEP_BY_STEP_DYNAMIC,
  STEP_BY_STEP_FULL,
  STEP_BY_STEP_PARTIAL
} from "../../../../state/dataset/datasetActions";
import MenuItem from "@material-ui/core/MenuItem";
import Autocomplete from "@material-ui/lab/Autocomplete";
import Chip from "@material-ui/core/Chip";
import FileInput from "../../../file-input";
import FormLabelWithTooltip from "../../../form-label-with-tooltip";
import {useTranslation} from "react-i18next";
import I18nTextField from "../../../i18n-text-field";
import {validateI18nObj} from "../../../../utils/i18n";
import I18nHtmlEditor from "../../../i18n-html-editor";
import {downloadFormats} from "../../../../utils/download";

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


const Form = compose(withStyles(styles), forwardRef)(({classes, config, onSubmit, onCancel}, ref) => {

  const [tab, setTab] = useState("general");
  const [mustChangeTabOnErrors, setMustChangeTabOnErrors] = useState(true);
  const {t} = useTranslation();

  const {register, errors, handleSubmit, watch, setValue} = useForm({
    defaultValues: {
      active: false,
      ...config,
      annotationConfig:
        config?.extras?.find(({key}) => key === "AnnotationConfig")
          ? JSON.parse(config.extras.find(({key}) => key === "AnnotationConfig").value)
          : {
            "ORDER": "ORDER",
            "ORDER_CATEGORY": "ORDER",
            "ORDER_CODELIST": "ORDER",
            "NOT_DISPLAYED": "NOT_DISPLAYED",
            "DEFAULT": "DEFAULT",
            "LAYOUT_ROW": "LAYOUT_ROW",
            "LAYOUT_COLUMN": "LAYOUT_COLUMN",
            "LAYOUT_FILTER": "LAYOUT_FILTER",
            "LAYOUT_ROW_SECTION": "LAYOUT_ROW_SECTION",
            "LAYOUT_CHART_PRIMARY_DIM": "LAYOUT_CHART_PRIMARY_DIM",
            "LAYOUT_CHART_SECONDARY_DIM": "LAYOUT_CHART_SECONDARY_DIM",
            "LAYOUT_CHART_FILTER": "LAYOUT_CHART_FILTER",
            "CRITERIA_SELECTION": "CRITERIA_SELECTION",
            "ATTACHED_DATA_FILES": "ATTACHED_DATA_FILES",
            "LAYOUT_DECIMAL_SEPARATOR": "LAYOUT_DECIMAL_SEPARATOR",
            "LAYOUT_NUMBER_OF_DECIMALS": "LAYOUT_NUMBER_OF_DECIMALS",
            "LAYOUT_EMPTY_CELL_PLACEHOLDER": "LAYOUT_EMPTY_CELL_PLACEHOLDER",
            "DATAFLOW_NOTES": "DATAFLOW_NOTES",
            "DATAFLOW_SOURCE": "DATAFLOW_SOURCE",
            "METADATA_URL": "METADATA_URL",
            "KEYWORDS": "KEYWORDS",
            "DEFAULT_VIEW": "DEFAULT_VIEW",
            "GEO_ID": "GEO_ID",
            "LAST_UPDATE": "LAST_UPDATE",
            "VIRTUAL_DATAFLOW_NODE": "VIRTUAL_DATAFLOW_NODE",
            "DATAFLOW_CATALOG_TYPE": "DATAFLOW_CATALOG_TYPE"
          },
      showCategoryLevels: config?.showCategoryLevels || 1,
      decimalNumber: config?.decimalNumber || 1,
      decimalSeparator: config?.decimalSeparator || {en: ","},
      catalogNavigationMode: config?.catalogNavigationMode || "List",
      hiddenAttributes:
        config?.extras?.find(({key}) => key === "HiddenAttributes")
          ? JSON.parse(config.extras.find(({key}) => key === "HiddenAttributes").value)
          : [],
      pageSize:
        config?.extras?.find(({key}) => key === "PageSize")
          ? JSON.parse(config.extras.find(({key}) => key === "PageSize").value)
          : "",
      restDataResponseXml:
        config?.extras?.find(({key}) => key === "RestDataResponseXml") && (
          config.extras.find(({key}) => key === "RestDataResponseXml").value === "false" ||
          config.extras.find(({key}) => key === "RestDataResponseXml").value === "true"
        )
          ? config.extras.find(({key}) => key === "RestDataResponseXml").value
          : "",
      downloadFormats:
        config?.extras?.find(({key}) => key === "DownloadFormats")
          ? JSON.parse(config.extras.find(({key}) => key === "DownloadFormats").value)
          : [],
      supportPostFilters:
        config?.extras?.find(({key}) => key === "SupportPostFilters") && (
          config.extras.find(({key}) => key === "SupportPostFilters").value === "false" ||
          config.extras.find(({key}) => key === "SupportPostFilters").value === "true"
        )
          ? JSON.parse(config.extras.find(({key}) => key === "SupportPostFilters").value)
          : false,
      enableEndpointV20:
        config?.extras?.find(({key}) => key === "EnableEndPointV20") && (
          config.extras.find(({key}) => key === "EnableEndPointV20").value === "false" ||
          config.extras.find(({key}) => key === "EnableEndPointV20").value === "true"
        )
          ? JSON.parse(config.extras.find(({key}) => key === "EnableEndPointV20").value)
          : false,
      endpointV20:
        config?.extras?.find(({key}) => key === "EndPointV20")
          ? config.extras.find(({key}) => key === "EndPointV20").value || ""
          : "",
    }
  });

  const enableHttpAuth = watch("enableHttpAuth");
  const enableProxy = watch("enableProxy");
  const enableEndpointV20 = watch("enableEndpointV20");

  useImperativeHandle(ref, () => ({
    submit(f) {
      setMustChangeTabOnErrors(true);
      handleSubmit(val => {
        const data = {
          ...config,
          ...val,
          type: "SDMX-REST",
          annotationConfig: undefined,
          restDataResponseXml: undefined,
          optimizeCallWithSoap: undefined,
          namespaceV20: undefined,
          namespaceV21: undefined,
          hiddenAttributes: undefined,
          pageSize: undefined,
          downloadFormats: undefined,
          enableEndpointV20: undefined,
          endpointV20: undefined
        };

        if (data.extras?.find(({key}) => key === "AnnotationConfig")) {
          data.extras.find(({key}) => key === "AnnotationConfig").value = JSON.stringify(val.annotationConfig)
        } else {
          data.extras = data.extras || [];
          data.extras.push({
            key: "AnnotationConfig",
            value: JSON.stringify(val.annotationConfig),
            isPublic: false
          });
        }

        if (data.extras?.find(({key}) => key === "RestDataResponseXml")) {
          data.extras.find(({key}) => key === "RestDataResponseXml").value = val.restDataResponseXml
        } else {
          data.extras = data.extras || [];
          data.extras.push({
            key: "RestDataResponseXml",
            value: val.restDataResponseXml,
            isPublic: false
          });
        }

        if (data.extras?.find(({key}) => key === "SupportPostFilters")) {
          data.extras.find(({key}) => key === "SupportPostFilters").value = JSON.stringify(val.supportPostFilters)
        } else {
          data.extras = data.extras || [];
          data.extras.push({
            key: "SupportPostFilters",
            value: JSON.stringify(val.supportPostFilters),
            isPublic: false
          });
        }

        if (data.extras?.find(({key}) => key === "HiddenAttributes")) {
          data.extras.find(({key}) => key === "HiddenAttributes").value = JSON.stringify(val.hiddenAttributes)
        } else {
          data.extras = data.extras || [];
          data.extras.push({
            key: "HiddenAttributes",
            value: JSON.stringify(val.hiddenAttributes),
            isPublic: true
          });
        }

        if (data.extras?.find(({key}) => key === "PageSize")) {
          data.extras.find(({key}) => key === "PageSize").value = JSON.stringify(val.pageSize)
        } else {
          data.extras = data.extras || [];
          data.extras.push({
            key: "PageSize",
            value: JSON.stringify(val.pageSize),
            isPublic: true
          });
        }

        if (data.extras?.find(({key}) => key === "DownloadFormats")) {
          data.extras.find(({key}) => key === "DownloadFormats").value = JSON.stringify(val.downloadFormats)
        } else {
          data.extras = data.extras || [];
          data.extras.push({
            key: "DownloadFormats",
            value: JSON.stringify(val.downloadFormats),
            isPublic: true
          });
        }

        if (data.extras?.find(({key}) => key === "EnableEndPointV20")) {
          data.extras.find(({key}) => key === "EnableEndPointV20").value = JSON.stringify(val.enableEndpointV20)
        } else {
          data.extras = data.extras || [];
          data.extras.push({
            key: "EnableEndPointV20",
            value: JSON.stringify(val.enableEndpointV20),
            isPublic: false
          });
        }

        if (data.extras?.find(({key}) => key === "EndPointV20")) {
          data.extras.find(({key}) => key === "EndPointV20").value = val.endpointV20
        } else {
          data.extras = data.extras || [];
          data.extras.push({
            key: "EndPointV20",
            value: val.endpointV20 || "",
            isPublic: false
          });
        }

        onSubmit(data);
        f(data);
      })();
    },
    cancel(f) {
      onCancel();
      f();
    }
  }));

  /* custom register */
  useEffect(() => {

    register({
      name: "code"
    }, {
      required: t("commons.validation.required"),
    });
    register({
      name: "title"
    }, {
      validate: val => validateI18nObj(val) || t("commons.validation.requiredAnyLanguage")
    });
    register({
      name: "agency"
    }, {
      required: t("commons.validation.required")
    })
    register({name: 'active'});
    register({name: 'default'});
    register({name: "slogan"});
    register({name: 'backgroundMediaURL'});
    register({name: 'logo'});
    register({name: 'description'});
    register({
      name: 'criteriaSelectionMode'
    }, {
      required: t("commons.validation.required")
    });
    register({
      name: "endPoint"
    }, {
      required: t("commons.validation.required")
    });
    register({
      name: "supportPostFilters"
    });
    register({
      name: "enableEndpointV20"
    });
    register({name: 'enableHttpAuth'});
    register({name: 'enableProxy'});
    register({name: 'annotationConfig.ORDER'});
    register({name: 'annotationConfig.ORDER_CATEGORY'});
    register({name: 'annotationConfig.ORDER_CODELIST'});
    register({name: 'annotationConfig.NOT_DISPLAYED'});
    register({name: 'annotationConfig.DEFAULT'});
    register({name: 'annotationConfig.LAYOUT_ROW'});
    register({name: 'annotationConfig.LAYOUT_COLUMN'});
    register({name: 'annotationConfig.LAYOUT_FILTER'});
    register({name: 'annotationConfig.LAYOUT_ROW_SECTION'});
    register({name: 'annotationConfig.LAYOUT_CHART_PRIMARY_DIM'});
    register({name: 'annotationConfig.LAYOUT_CHART_SECONDARY_DIM'});
    register({name: 'annotationConfig.LAYOUT_CHART_FILTER'});
    register({name: 'annotationConfig.CRITERIA_SELECTION'});
    register({name: 'annotationConfig.ATTACHED_DATA_FILES'});
    register({name: 'annotationConfig.LAYOUT_DECIMAL_SEPARATOR'});
    register({name: 'annotationConfig.LAYOUT_NUMBER_OF_DECIMALS'});
    register({name: 'annotationConfig.LAYOUT_EMPTY_CELL_PLACEHOLDER'});
    register({name: 'annotationConfig.DATAFLOW_NOTES'});
    register({name: 'annotationConfig.DATAFLOW_SOURCE'});
    register({name: 'annotationConfig.METADATA_URL'});
    register({name: 'annotationConfig.KEYWORDS'});
    register({name: 'annotationConfig.DEFAULT_VIEW'});
    register({name: 'annotationConfig.GEO_ID'});
    register({name: 'annotationConfig.LAST_UPDATE'});
    register({name: 'annotationConfig.VIRTUAL_DATAFLOW_NODE'});
    register({name: 'annotationConfig.DATAFLOW_CATALOG_TYPE'});
    register({
      name: "ttlCatalog"
    }, {
      validate: val => !val || val > 0 || t("commons.validation.positiveInteger")
    });
    register({
      name: "ttlDataflow"
    }, {
      validate: val => !val || val > 0 || t("commons.validation.positiveInteger")
    });
    register({name: 'showDataflowUncategorized'});
    register({name: 'showDataflowNotInProduction'});
    register({
      name: 'showCategoryLevels'
    }, {
      required: t("commons.validation.required")
    });
    register({
      name: "decimalNumber"
    }, {
      required: t("commons.validation.required"),
      validate: val => !val || val > 0 || t("commons.validation.positiveInteger")
    });
    register({
      name: 'decimalSeparator'
    }, {
      validate: val => validateI18nObj(val) || t("commons.validation.requiredAnyLanguage")
    });
    register({name: 'labelDimensionTerritorials'});
    register({name: 'labelDimensionTemporals'});
    register({name: 'categorySchemaExcludes'});
    register({
      name: 'catalogNavigationMode'
    }, {
      required: t("commons.validation.required")
    });
    register({name: 'hiddenAttributes'});
    register({
      name: 'pageSize'
    }, {
      validate: val => !val || val > 0 || t("commons.validation.positiveInteger")
    });
    register({
      name: 'restDataResponseXml'
    }, {
      required: t("commons.validation.required")
    });
    register({name: 'downloadFormats'});
  }, [register, t]);

  /* register enableHttpAuth fieldset */
  useEffect(() => {

    if (enableHttpAuth) {
      register({
        name: "authHttpUsername"
      }, {
        required: t("commons.validation.required")
      });
      register({
        name: "authHttpPassword"
      });
      register({
        name: "authHttpDomain"
      });
    }

  }, [register, enableHttpAuth, t]);

  /* register enableProxy fieldset */
  useEffect(() => {

    if (enableProxy) {
      register({
        name: "proxyAddress"
      }, {
        required: t("commons.validation.required")
      });
      register({
        name: "proxyPort"
      }, {
        required: t("commons.validation.required"),
        validate: val => validator.isPort(val) || t("scenes.nodeSettings.fields.proxyPort.validation")
      });
      register({name: "proxyUsername"});
      register({name: "proxyPassword"});
    }
  }, [register, enableProxy, t]);

  useEffect(() => {
    if (enableEndpointV20) {
      register({
        name: "endpointV20"
      }, {
        required: t("commons.validation.required")
      });
    }
  }, [enableEndpointV20, register, t]);

  /* switch to first tab with errors */
  if (Object.keys(errors).length > 0 && mustChangeTabOnErrors) {

    setMustChangeTabOnErrors(false);

    const firstFieldWithErrors = Object.keys(errors)[0];
    if (firstFieldWithErrors === "code" || firstFieldWithErrors === "agency" || firstFieldWithErrors === "title" || firstFieldWithErrors === "slogan") {
      if (tab !== "general") {
        setTab("general")
      }
    } else if (
      firstFieldWithErrors === "showCategoryLevels" ||
      firstFieldWithErrors === "decimalNumber" ||
      firstFieldWithErrors === "decimalSeparator" ||
      firstFieldWithErrors === "labelDimensionTerritorials" ||
      firstFieldWithErrors === "labelDimensionTemporals" ||
      firstFieldWithErrors === "categorySchemaExcludes" ||
      firstFieldWithErrors === "catalogNavigationMode" ||
      firstFieldWithErrors === "hiddenAttributes" ||
      firstFieldWithErrors === "pageSize" ||
      firstFieldWithErrors === "downloadFormats"
    ) {

      if (tab !== "view") {
        setTab("view")
      }

    } else if (firstFieldWithErrors) {

      if (tab !== "endpoint") {
        setTab("endpoint");
      }

    }
  }

  return (
    <Box className={classes.root}>
      <Tabs
        value={tab}
        onChange={(_, tab) => setTab(tab)}
      >
        <Tab value="general" label={t("scenes.nodeSettings.tabs.general.label")}/>
        <Tab value="infos" label={t("scenes.nodeSettings.tabs.information.label")}/>
        <Tab value="endpoint" label={t("scenes.nodeSettings.tabs.endpoint.label")}/>
        <Tab value="annotations" label={t("scenes.nodeSettings.tabs.annotations.label")}/>
        <Tab value="view" label={t("scenes.nodeSettings.tabs.view.label")}/>
        <Tab value="cache" label={t("scenes.nodeSettings.tabs.cache.label")}/>
      </Tabs>
      <div className={classes.tabContent}>
        <Box style={tab !== "general" ? {display: "none"} : undefined}>
          <Fragment>
            <Grid container spacing={3}>
              <Grid item xs={3}>
                <FormControl fullWidth className={classes.field}>
                  <TextField
                    disabled={!!config}
                    name="code"
                    variant="outlined"
                    label={
                      <FormLabelWithTooltip tooltip={t("scenes.nodeSettings.fields.code.tooltip")}>
                        {t("scenes.nodeSettings.fields.code.label")}
                      </FormLabelWithTooltip>
                    }
                    required
                    error={!!errors.code}
                    helperText={errors.code?.message}
                    value={watch('code') || ""}
                    onChange={({target}) => setValue('code', (target.value || "").toUpperCase())}
                    inputProps={{
                      style: {textTransform: "uppercase"}
                    }}
                  />
                </FormControl>
              </Grid>
              <Grid item xs={6}>
                <FormControl fullWidth className={classes.field}>
                  <I18nTextField
                    name="title"
                    label={t("scenes.nodeSettings.fields.title.label")}
                    required
                    error={!!errors.title}
                    helperText={errors.title?.message}
                    variant="outlined"
                    value={watch('title') || {}}
                    onChange={value => setValue('title', value)}
                  />
                </FormControl>
              </Grid>
              <Grid item xs={3}>
                <FormControl fullWidth className={classes.field}>
                  <TextField
                    name="agency"
                    label={t("scenes.nodeSettings.fields.agency.label")}
                    required
                    error={!!errors.agency}
                    helperText={errors.agency?.message}
                    variant="outlined"
                    value={watch('agency') || ""}
                    onChange={({target}) => setValue('agency', target.value)}
                  />
                </FormControl>
              </Grid>
            </Grid>
            <FormControl fullWidth className={classes.field}>
              <FormControlLabel
                label={
                  <FormLabelWithTooltip
                    tooltip={t("scenes.nodeSettings.fields.active.tooltip")}
                    tooltipOnRight
                  >
                    {t("scenes.nodeSettings.fields.active.label")}
                  </FormLabelWithTooltip>
                }
                control={
                  <Checkbox
                    name="active"
                    required
                    checked={watch('active')}
                    onChange={(e, value) => setValue('active', value)}
                  />
                }
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <FormControlLabel
                label={t("scenes.nodeSettings.fields.default.label")}
                control={
                  <Checkbox
                    name="default"
                    required
                    checked={watch('default')}
                    onChange={(e, value) => setValue('default', value)}
                  />
                }
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <I18nTextField
                name="slogan"
                label={t("scenes.nodeSettings.fields.slogan.label")}
                variant="outlined"
                value={watch('slogan') || {}}
                onChange={value => setValue('slogan', value)}
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <FileInput
                label={t("scenes.nodeSettings.fields.backgroundMediaURL.label")}
                value={watch('backgroundMediaURL')}
                onChange={value => setValue('backgroundMediaURL', value)}
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <FileInput
                label={t("scenes.nodeSettings.fields.logo.label")}
                value={watch('logo')}
                onChange={value => setValue('logo', value)}
              />
            </FormControl>
          </Fragment>
        </Box>
        <Box style={tab !== "infos" ? {display: "none"} : undefined}>
          <I18nHtmlEditor
            value={watch('description')}
            onChange={val => setValue('description', val)}
          />
        </Box>
        <Box style={tab !== "endpoint" ? {display: "none"} : undefined}>
          <Paper variant="outlined" className={classes.paper}>
            <Grid container spacing={3}>
              <Grid item sm={12}>
                <FormControl fullWidth>
                  <TextField
                    name="type"
                    select
                    label={t("scenes.nodeSettings.fields.type.label")}
                    value="SDMX-REST"
                    variant="outlined"
                  >
                    <MenuItem value="SDMX-REST">SDMX-REST</MenuItem>
                  </TextField>
                </FormControl>
              </Grid>
              <Grid item sm={12}>
                <FormControl fullWidth>
                  <TextField
                    name="criteriaSelectionMode"
                    select
                    label={
                      <FormLabelWithTooltip
                        tooltip={t("scenes.nodeSettings.fields.criteriaSelectionMode.tooltip")}
                      >
                        {t("scenes.nodeSettings.fields.criteriaSelectionMode.label")}
                      </FormLabelWithTooltip>
                    }
                    variant="outlined"
                    onChange={e => setValue('criteriaSelectionMode', e.target.value)}
                    value={watch("criteriaSelectionMode") || ''}
                    helperText={errors.criteriaSelectionMode?.message}
                    error={!!errors.criteriaSelectionMode}
                  >
                    <MenuItem value={ALL_FULL}>ALL_FULL</MenuItem>
                    <MenuItem value={ALL_PARTIAL}>ALL_PARTIAL</MenuItem>
                    <MenuItem value={STEP_BY_STEP_FULL}>STEP_BY_STEP_FULL</MenuItem>
                    <MenuItem value={STEP_BY_STEP_PARTIAL}>STEP_BY_STEP_PARTIAL</MenuItem>
                    <MenuItem value={STEP_BY_STEP_DYNAMIC}>STEP_BY_STEP_DYNAMIC</MenuItem>
                  </TextField>
                </FormControl>
              </Grid>
            </Grid>
          </Paper>
          <Paper variant="outlined" className={classes.paper}>
            <FormControl fullWidth>
              <TextField
                name="endPoint"
                label={t("scenes.nodeSettings.fields.endPoint.label")}
                required
                error={!!errors.endPoint}
                helperText={errors.endPoint?.message}
                variant="outlined"
                value={watch('endPoint') || ""}
                onChange={({target}) => setValue('endPoint', target.value)}
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <TextField
                select
                label={t("scenes.nodeSettings.fields.responseFormat.label")}
                value={watch("restDataResponseXml") || ""}
                required
                variant="outlined"
                error={!!errors.restDataResponseXml}
                helperText={errors.restDataResponseXml?.message}
                onChange={({target}) => setValue('restDataResponseXml', target.value)}
              >
                <MenuItem value="true">XML</MenuItem>
                <MenuItem value="false">JSON</MenuItem>
              </TextField>
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <FormControlLabel
                label={t("scenes.nodeSettings.fields.supportPostFilters.label")}
                control={
                  <Checkbox
                    name="supportPostFilters"
                    required
                    checked={watch('supportPostFilters') || false}
                    onChange={(e, value) => setValue('supportPostFilters', value)}
                  />
                }
              />
            </FormControl>
            <FormControl fullWidth className={classes.field}>
              <FormControlLabel
                label={
                  <FormLabelWithTooltip
                    tooltip={t("scenes.nodeSettings.fields.enableEndpointV20.tooltip")}
                  >
                    {t("scenes.nodeSettings.fields.enableEndpointV20.label")}
                  </FormLabelWithTooltip>
                }
                control={
                  <Checkbox
                    name="enableEndpointV20"
                    required
                    checked={enableEndpointV20}
                    onChange={(e, value) => setValue('enableEndpointV20', value)}
                  />
                }
              />
            </FormControl>
            {enableEndpointV20 && (
              <FormControl fullWidth className={classes.field}>
                <TextField
                  name="endpointV20"
                  label={t("scenes.nodeSettings.fields.endpointV20.label")}
                  required
                  error={!!errors.endpointV20}
                  helperText={errors.endpointV20?.message}
                  variant="outlined"
                  value={watch('endpointV20') || ""}
                  onChange={({target}) => setValue('endpointV20', target.value)}
                />
              </FormControl>
            )}
          </Paper>
          {/*<Paper variant="outlined" className={classes.paper}>
            <Grid container spacing={3}>
              <Grid item sm={4}>
                <FormControlLabel
                  label="SOAP endpoint"
                  control={
                    <Checkbox
                      name="optimizeCallWithSoap"
                      checked={watch('optimizeCallWithSoap')}
                      onChange={(e, value) => setValue('optimizeCallWithSoap', value)}
                    />
                  }
                />
              </Grid>
              {optimizeCallWithSoap && (
                <Fragment>
                  <Grid item sm={8}>
                    <FormControl fullWidth>
                      <TextField
                        name="namespaceV20"
                        label="Endpoint V20"
                        required
                        inputRef={register({
                          required: REQUIRED_MESSAGE
                        })}
                        error={!!errors.namespaceV20}
                        helperText={errors.namespaceV20?.message}
                        variant="outlined"
                      />
                    </FormControl>
                  </Grid>
                  <Grid item sm={4}></Grid>
                  <Grid item sm={8}>
                    <FormControl fullWidth>
                      <TextField
                        name="namespaceV21"
                        label="Endpoint V21"
                        required
                        inputRef={register({
                          required: REQUIRED_MESSAGE
                        })}
                        error={!!errors.namespaceV21}
                        helperText={errors.namespaceV21?.message}
                        variant="outlined"
                      />
                    </FormControl>
                  </Grid>
                </Fragment>
              )}
            </Grid>
          </Paper>*/}
          <Paper variant="outlined" className={classes.paper}>
            <Grid container spacing={3}>
              <Grid item sm={4}>
                <FormControlLabel
                  label={t("scenes.nodeSettings.fields.enableHttpAuth.label")}
                  control={
                    <Checkbox
                      name="enableHttpAuth"
                      checked={enableHttpAuth || false}
                      onChange={(e, value) => setValue('enableHttpAuth', value)}
                    />
                  }
                />
              </Grid>
              {enableHttpAuth && (
                <Fragment>
                  <Grid item sm={4}>
                    <FormControl fullWidth>
                      <TextField
                        name="authHttpUsername"
                        label={t("scenes.nodeSettings.fields.authHttpUsername.label")}
                        required
                        error={!!errors.authHttpUsername}
                        helperText={errors.authHttpUsername?.message}
                        variant="outlined"
                        value={watch('authHttpUsername') || ""}
                        onChange={({target}) => setValue('authHttpUsername', target.value)}
                      />
                    </FormControl>
                  </Grid>
                  <Grid item sm={4}>
                    <FormControl fullWidth>
                      <TextField
                        name="authHttpPassword"
                        label={t("scenes.nodeSettings.fields.authHttpPassword.label")}
                        type="password"
                        variant="outlined"
                        value={watch('authHttpPassword') || ""}
                        onChange={({target}) => setValue('authHttpPassword', target.value)}
                      />
                    </FormControl>
                  </Grid>
                  <Grid item sm={4}/>
                  <Grid item sm={4}>
                    <FormControl fullWidth>
                      <TextField
                        name="authHttpDomain"
                        label={t("scenes.nodeSettings.fields.authHttpDomain.label")}
                        variant="outlined"
                        value={watch('authHttpDomain') || ""}
                        onChange={({target}) => setValue('authHttpDomain', target.value)}
                      />
                    </FormControl>
                  </Grid>
                </Fragment>
              )}
            </Grid>
          </Paper>
          <Paper variant="outlined" className={classes.paper}>
            <Grid container spacing={3}>
              <Grid item sm={4}>
                <FormControlLabel
                  label={t("scenes.nodeSettings.fields.enableProxy.label")}
                  control={
                    <Checkbox
                      name="enableProxy"
                      checked={enableProxy || false}
                      onChange={(e, value) => setValue('enableProxy', value)}
                    />
                  }
                />
              </Grid>
              {enableProxy && (
                <Fragment>
                  <Grid item sm={4}>
                    <FormControl fullWidth>
                      <TextField
                        name="proxyAddress"
                        label={t("scenes.nodeSettings.fields.proxyAddress.label")}
                        required
                        error={!!errors.proxyAddress}
                        helperText={errors.proxyAddress?.message}
                        variant="outlined"
                        value={watch('proxyAddress') || ""}
                        onChange={({target}) => setValue('proxyAddress', target.value)}
                      />
                    </FormControl>
                  </Grid>
                  <Grid item sm={4}>
                    <FormControl fullWidth>
                      <TextField
                        name="proxyPort"
                        label={t("scenes.nodeSettings.fields.proxyPort.label")}
                        required
                        error={!!errors.proxyPort}
                        helperText={errors.proxyPort?.message}
                        variant="outlined"
                        value={watch('proxyPort') || ""}
                        onChange={({target}) => setValue('proxyPort', target.value)}
                      />
                    </FormControl>
                  </Grid>
                  <Grid item sm={4}/>
                  <Grid item sm={4}>
                    <FormControl fullWidth>
                      <TextField
                        name="proxyUsername"
                        label={t("scenes.nodeSettings.fields.proxyUsername.label")}
                        variant="outlined"
                        value={watch('proxyUsername') || ""}
                        onChange={({target}) => setValue('proxyUsername', target.value)}
                      />
                    </FormControl>
                  </Grid>
                  <Grid item sm={4}>
                    <FormControl fullWidth>
                      <TextField
                        name="proxyPassword"
                        label={t("scenes.nodeSettings.fields.proxyPassword.label")}
                        type="password"
                        variant="outlined"
                        value={watch('proxyPassword') || ""}
                        onChange={e => setValue('proxyPassword', e.target.value)}
                      />
                    </FormControl>
                  </Grid>
                </Fragment>
              )}
            </Grid>
          </Paper>
        </Box>
        <Box style={tab !== "annotations" ? {display: "none"} : undefined}>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.ORDER"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.ORDER.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.ORDER') || ""}
              onChange={({target}) => setValue('annotationConfig.ORDER', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.ORDER_CATEGORY"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.ORDER_CATEGORY.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.ORDER_CATEGORY') || ""}
              onChange={({target}) => setValue('annotationConfig.ORDER_CATEGORY', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.ORDER_CODELIST"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.ORDER_CODELIST.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.ORDER_CODELIST') || ""}
              onChange={({target}) => setValue('annotationConfig.ORDER_CODELIST', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.NOT_DISPLAYED"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.NOT_DISPLAYED.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.NOT_DISPLAYED') || ""}
              onChange={({target}) => setValue('annotationConfig.NOT_DISPLAYED', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.DEFAULT"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.DEFAULT.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.DEFAULT') || ""}
              onChange={({target}) => setValue('annotationConfig.DEFAULT', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_ROW"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_ROW.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_ROW') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_ROW', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_COLUMN"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_COLUMN.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_COLUMN') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_COLUMN', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_FILTER"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_FILTER.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_FILTER') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_FILTER', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_ROW_SECTION"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_ROW_SECTION.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_ROW_SECTION') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_ROW_SECTION', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_CHART_PRIMARY_DIM"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_CHART_PRIMARY_DIM.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_CHART_PRIMARY_DIM') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_CHART_PRIMARY_DIM', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_CHART_SECONDARY_DIM"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_CHART_SECONDARY_DIM.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_CHART_SECONDARY_DIM') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_CHART_SECONDARY_DIM', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_CHART_FILTER"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_CHART_FILTER.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_CHART_FILTER') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_CHART_FILTER', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.CRITERIA_SELECTION"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.CRITERIA_SELECTION.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.CRITERIA_SELECTION') || ""}
              onChange={({target}) => setValue('annotationConfig.CRITERIA_SELECTION', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.ATTACHED_DATA_FILES"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.ATTACHED_DATA_FILES.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.ATTACHED_DATA_FILES') || ""}
              onChange={({target}) => setValue('annotationConfig.ATTACHED_DATA_FILES', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_DECIMAL_SEPARATOR"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_DECIMAL_SEPARATOR.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_DECIMAL_SEPARATOR') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_DECIMAL_SEPARATOR', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_NUMBER_OF_DECIMALS"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_NUMBER_OF_DECIMALS.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_NUMBER_OF_DECIMALS') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_NUMBER_OF_DECIMALS', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAYOUT_EMPTY_CELL_PLACEHOLDER"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAYOUT_EMPTY_CELL_PLACEHOLDER.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAYOUT_EMPTY_CELL_PLACEHOLDER') || ""}
              onChange={({target}) => setValue('annotationConfig.LAYOUT_EMPTY_CELL_PLACEHOLDER', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.DATAFLOW_NOTES"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.DATAFLOW_NOTES.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.DATAFLOW_NOTES') || ""}
              onChange={({target}) => setValue('annotationConfig.DATAFLOW_NOTES', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.DATAFLOW_SOURCE"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.DATAFLOW_SOURCE.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.DATAFLOW_SOURCE') || ""}
              onChange={({target}) => setValue('annotationConfig.DATAFLOW_SOURCE', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.METADATA_URL"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.METADATA_URL.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.METADATA_URL') || ""}
              onChange={({target}) => setValue('annotationConfig.METADATA_URL', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.KEYWORDS"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.KEYWORDS.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.KEYWORDS') || ""}
              onChange={({target}) => setValue('annotationConfig.KEYWORDS', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.DEFAULT_VIEW"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.DEFAULT_VIEW.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.DEFAULT_VIEW') || ""}
              onChange={({target}) => setValue('annotationConfig.DEFAULT_VIEW', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.GEO_ID"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.GEO_ID.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.GEO_ID') || ""}
              onChange={({target}) => setValue('annotationConfig.GEO_ID', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.LAST_UPDATE"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.LAST_UPDATE.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.LAST_UPDATE') || ""}
              onChange={({target}) => setValue('annotationConfig.LAST_UPDATE', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.VIRTUAL_DATAFLOW_NODE"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.VIRTUAL_DATAFLOW_NODE.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.VIRTUAL_DATAFLOW_NODE') || ""}
              onChange={({target}) => setValue('annotationConfig.VIRTUAL_DATAFLOW_NODE', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="annotationConfig.DATAFLOW_CATALOG_TYPE"
              label={
                <FormLabelWithTooltip>{t("scenes.nodeSettings.fields.annotations.DATAFLOW_CATALOG_TYPE.label")}</FormLabelWithTooltip>}
              variant="outlined"
              value={watch('annotationConfig.DATAFLOW_CATALOG_TYPE') || ""}
              onChange={({target}) => setValue('annotationConfig.DATAFLOW_CATALOG_TYPE', target.value)}
            />
          </FormControl>
        </Box>
        <Box style={tab !== "cache" ? {display: "none"} : undefined}>


          <FormControl fullWidth className={classes.field}>
            <TextField
              name="ttlCatalog"
              variant="outlined"
              helperText={errors.ttlCatalog?.message}
              label={
                <FormLabelWithTooltip
                  tooltip={t("scenes.nodeSettings.fields.catalogCacheValidity.tooltip")}>
                  {t("scenes.nodeSettings.fields.catalogCacheValidity.label")}
                </FormLabelWithTooltip>
              }
              error={!!errors.ttlCatalog}
              value={watch('ttlCatalog') || ""}
              onChange={({target}) => setValue('ttlCatalog', target.value)}
            />
          </FormControl>

          <FormControl fullWidth className={classes.field}>
            <TextField
              name="ttlDataflow"
              variant="outlined"
              helperText={errors.ttlDataflow?.message}
              label={
                <FormLabelWithTooltip
                  tooltip={t("scenes.nodeSettings.fields.dataflowCacheValidity.tooltip")}>
                  {t("scenes.nodeSettings.fields.dataflowCacheValidity.label")}
                </FormLabelWithTooltip>
              }
              error={!!errors.ttlDataflow}
              value={watch('ttlDataflow') || ""}
              onChange={({target}) => setValue('ttlDataflow', target.value)}
            />
          </FormControl>


        </Box>
        <Box style={tab !== "view" ? {display: "none"} : undefined}>
          <FormControl fullWidth className={classes.field}>
            <FormControlLabel
              label={t("scenes.nodeSettings.fields.showDataflowUncategorized.label")}
              control={
                <Checkbox
                  name="showDataflowUncategorized"
                  required
                  checked={watch('showDataflowUncategorized') || false}
                  onChange={(e, value) => setValue('showDataflowUncategorized', value)}
                />
              }
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <FormControlLabel
              label={t("scenes.nodeSettings.fields.showDataflowNotInProduction.label")}
              control={
                <Checkbox
                  name="showDataflowNotInProduction"
                  required
                  checked={watch('showDataflowNotInProduction') || false}
                  onChange={(e, value) => setValue('showDataflowNotInProduction', value)}
                />
              }
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="showCategoryLevels"
              select
              required
              label={t("scenes.nodeSettings.fields.showCategoryLevels.label")}
              onChange={e => setValue('showCategoryLevels', e.target.value)}
              value={watch("showCategoryLevels") || ''}
              error={!!errors.showCategoryLevels}
              helperText={errors.showCategoryLevels?.message}
              variant="outlined"
            >
              <MenuItem value={1}>1</MenuItem>
              <MenuItem value={5}>5</MenuItem>
            </TextField>
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <TextField
              name="decimalNumber"
              label={t("scenes.nodeSettings.fields.decimalNumber.label")}
              variant="outlined"
              required
              type="number"
              error={!!errors.decimalNumber}
              helperText={errors.decimalNumber?.message}
              value={watch('decimalNumber') || ''}
              onChange={({target}) => setValue('decimalNumber', target.value)}
            />
          </FormControl>
          <FormControl fullWidth className={classes.field}>
            <I18nTextField
              name="decimalSeparator"
              select
              required
              label={t("scenes.nodeSettings.fields.decimalSeparator.label")}
              onChange={value => setValue('decimalSeparator', value)}
              value={watch("decimalSeparator") || ''}
              helper={t("scenes.nodeSettings.fields.decimalSeparator.helper")}
              error={!!errors.decimalSeparator}
              helperText={errors.decimalSeparator?.message}
              variant="outlined"
            >
              <MenuItem value=".">Dot</MenuItem>
              <MenuItem value=",">Comma</MenuItem>
            </I18nTextField>
          </FormControl>
          <Autocomplete
            multiple
            variant="outlined"
            freeSolo
            options={[]}
            value={watch('labelDimensionTerritorials') || []}
            renderTags={(value, getTagProps) =>
              value.map((option, index) => (
                <Chip variant="outlined" label={option} {...getTagProps({index})} />
              ))
            }
            onChange={(e, val) => setValue('labelDimensionTerritorials', val)}
            renderInput={params => (
              <FormControl fullWidth className={classes.field}>
                <TextField
                  label={t("scenes.nodeSettings.fields.labelDimensionTerritorials.label")}
                  {...params}
                  variant="outlined"
                  placeholder={t("scenes.nodeSettings.fields.labelDimensionTerritorials.placeholder")}
                />
              </FormControl>
            )}
          />
          <Autocomplete
            multiple
            variant="outlined"
            freeSolo
            options={[]}
            value={watch('labelDimensionTemporals') || []}
            renderTags={(value, getTagProps) =>
              value.map((option, index) => (
                <Chip variant="outlined" label={option} {...getTagProps({index})} />
              ))
            }
            onChange={(e, val) => setValue('labelDimensionTemporals', val)}
            renderInput={params => (
              <FormControl fullWidth className={classes.field}>
                <TextField
                  label={t("scenes.nodeSettings.fields.labelDimensionTemporals.label")}
                  {...params}
                  variant="outlined"
                  placeholder={t("scenes.nodeSettings.fields.labelDimensionTemporals.placeholder")}
                />
              </FormControl>
            )}
          />
          <Autocomplete
            multiple
            variant="outlined"
            freeSolo
            options={[]}
            value={watch('categorySchemaExcludes') || []}
            renderTags={(value, getTagProps) =>
              value.map((option, index) => (
                <Chip variant="outlined" label={option} {...getTagProps({index})} />
              ))
            }
            onChange={(e, val) => setValue('categorySchemaExcludes', val)}
            renderInput={params => (
              <FormControl fullWidth className={classes.field}>
                <TextField
                  label={t("scenes.nodeSettings.fields.categorySchemaExcludes.label")}
                  {...params}
                  variant="outlined"
                  placeholder={t("scenes.nodeSettings.fields.categorySchemaExcludes.placeholder")}
                />
              </FormControl>
            )}
          />
          <FormControl fullWidth className={classes.field}>
            <TextField
              select
              label={t("scenes.nodeSettings.fields.catalogNavigationMode.label")}
              onChange={e => setValue('catalogNavigationMode', e.target.value)}
              value={watch("catalogNavigationMode") || ''}
              required
              variant="outlined"
              error={!!errors.catalogNavigationMode}
              helperText={errors.catalogNavigationMode?.message}
            >
              <MenuItem
                value="Card">{t("scenes.nodeSettings.fields.catalogNavigationMode.values.card")}</MenuItem>
              <MenuItem
                value="List">{t("scenes.nodeSettings.fields.catalogNavigationMode.values.list")}</MenuItem>
            </TextField>
          </FormControl>
          <Autocomplete
            multiple
            variant="outlined"
            freeSolo
            options={[]}
            value={watch('hiddenAttributes') || []}
            renderTags={(value, getTagProps) =>
              value.map((option, index) => (
                <Chip variant="outlined" label={option} {...getTagProps({index})} />
              ))
            }
            onChange={(e, val) => setValue('hiddenAttributes', val)}
            renderInput={params => (
              <FormControl fullWidth className={classes.field}>
                <TextField
                  label={t("scenes.nodeSettings.fields.hiddenAttributes.label")}
                  {...params}
                  variant="outlined"
                  placeholder={t("scenes.nodeSettings.fields.hiddenAttributes.placeholder")}
                />
              </FormControl>
            )}
          />
          {/*<FormControl fullWidth className={classes.field}>
            <TextField
              name="pageSize"
              label={t("scenes.nodeSettings.fields.pageSize.label")}
              variant="outlined"
              type="number"
              value={watch('pageSize') || ""}
              onChange={({target}) => setValue('pageSize', Number(target.value))}
            />
          </FormControl>*/}
          <Autocomplete
            multiple
            variant="outlined"
            options={Object.keys(downloadFormats(t))}
            getOptionLabel={option => downloadFormats(t)[option].label}
            value={watch('downloadFormats') || []}
            renderTags={(value, getTagProps) =>
              value.map((option, index) => (
                <Chip
                  variant="outlined"
                  label={downloadFormats(t)[option].label}
                  {...getTagProps({index})}
                />
              ))
            }
            onChange={(e, val) => setValue('downloadFormats', val)}
            renderInput={params => (
              <FormControl fullWidth className={classes.field}>
                <TextField
                  label={t("scenes.nodeSettings.fields.downloadFormats.label")}
                  {...params}
                  variant="outlined"
                  placeholder={t("scenes.nodeSettings.fields.downloadFormats.placeholder")}
                />
              </FormControl>
            )}
          />
        </Box>
      </div>
    </Box>
  );
});

const mapStateToProps = state => ({
  config: state.nodesConfig.node
});

const mapDispatchToProps = dispatch => ({
  fetchConfig: nodeId => dispatch(fetchNodesConfigNode(nodeId)),
  sendConfigCreate: config => dispatch(sendNodesConfigNodeCreate(config)),
  sendConfig: config => dispatch(sendNodesConfigNodeEdit(config)),
  clearConfig: () => dispatch(clearNodesConfigNode())
});

const NodeSettingsForm = ({config, nodeId, fetchConfig, sendConfigCreate, sendConfig, clearConfig}, ref) => {

  const [needConfig, setNeedConfig] = useState(nodeId !== null);

  useEffect(() => {

    if (needConfig) {
      setNeedConfig(false);
      fetchConfig(nodeId);
    }
  }, [config, needConfig, setNeedConfig, fetchConfig, nodeId]);

  return ((nodeId === null || config) && (
    <Form
      config={nodeId === null ? undefined : config}
      ref={ref}
      onSubmit={nodeId === null ? sendConfigCreate : sendConfig}
      onCancel={clearConfig}
    />
  ));
};

export default compose(connect(mapStateToProps, mapDispatchToProps, null, {forwardRef: true}), forwardRef)(NodeSettingsForm);
