import React, {Fragment, useState} from 'react';
import withStyles from "@material-ui/core/styles/withStyles";
import {useTranslation} from "react-i18next";
import Grid from "@material-ui/core/Grid";
import FormControl from "@material-ui/core/FormControl";
import I18nTextField from "../i18n-text-field";
import JsonStatTable from "../table";
import {localizeI18nObj} from "../../utils/i18n";
import CustomEmpty from "../custom-empty";
import Box from "@material-ui/core/Box";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import TextField from "@material-ui/core/TextField";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import MenuItem from "@material-ui/core/MenuItem";
import Checkbox from "@material-ui/core/Checkbox";
import Card from "@material-ui/core/Card";
import CardHeader from "@material-ui/core/CardHeader";
import {v4 as uuidv4} from 'uuid';
import Chart from "../chart";
import Map from "../map";

const styles = theme => ({
  card: {
    padding: theme.spacing(2)
  },
  viewer: {
    width: "100%",
    height: "calc(100% - 16px - 64px)",
    marginTop: 16
  },
  tabContent: {
    overflowY: "auto",
    overflowX: "hidden",
    height: "calc(100% - 48px)",
    paddingTop: 16
  }
});

function TemplateBuilder(props) {
  const {
    classes,
    defaultLanguage,
    languages,
    template,
    onChange,
    viewers,
    jsonStat,
    labelFormat,
    tableLayout,
    chartLayout,
    mapLayout,
    templateGeometries,
    templateGeometryDetailLevels,
    onGeometryFetch,
    hiddenAttributes
  } = props;

  const {t} = useTranslation();

  const [tabId, setTabId] = useState(0);

  const [mapId] = useState("map__" + uuidv4());

  const [chartType, setChartType] = useState("bar");

  return (
    <Fragment>
      <Box>
        <Tabs
          value={tabId}
          variant="scrollable"
          scrollButtons="auto"
          onChange={(event, newValue) => setTabId(newValue)}
        >
          <Tab key={0} label={t("scenes.dataViewer.templateBuilder.tabs.options.label")}/>
          <Tab key={1} label={t("scenes.dataViewer.templateBuilder.tabs.table.label")}/>
          <Tab key={2} label={t("scenes.dataViewer.templateBuilder.tabs.chart.label")}/>
          <Tab key={3} label={t("scenes.dataViewer.templateBuilder.tabs.map.label")} disabled={!mapLayout}/>
        </Tabs>
      </Box>
      <div className={classes.tabContent}>
        {tabId === 0 && (
          <Fragment>
            <Card variant="outlined">
              <CardHeader
                title={t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.title")}
                titleTypographyProps={{variant: "subtitle1"}}
              />
              <Grid container spacing={3} className={classes.card}>
                <Grid item xs={6}>
                  <FormControl fullWidth className={classes.field}>
                    <I18nTextField
                      label={t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.title")}
                      required
                      variant="outlined"
                      value={template.title}
                      onChange={value => onChange({...template, title: value})}
                    />
                  </FormControl>
                </Grid>
                <Grid item xs={6}>
                  <FormControl fullWidth className={classes.field}>
                    <TextField
                      select
                      label={t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.defaultView")}
                      defaultValue="table"
                      value={template.defaultView}
                      variant="outlined"
                      onChange={ev => onChange({...template, defaultView: ev.target.value})}
                    >
                      {viewers.map(({type, title}, idx) =>
                        <MenuItem key={idx} value={type}>{title}</MenuItem>
                      )}
                    </TextField>
                  </FormControl>
                </Grid>
                <Grid item xs={3}>
                  <FormControlLabel
                    label={t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.enableCriteria")}
                    className={classes.field}
                    control={
                      <Checkbox
                        checked={template.enableCriteria}
                        onChange={(ev, value) => onChange({...template, enableCriteria: value})}
                      />
                    }
                  />
                </Grid>
                <Grid item xs={3}>
                  <FormControlLabel
                    label={t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.enableLayout")}
                    className={classes.field}
                    control={
                      <Checkbox
                        checked={template.enableLayout}
                        onChange={(ev, value) => onChange({...template, enableLayout: value})}
                      />
                    }
                  />
                </Grid>
                <Grid item xs={3}>
                  <FormControlLabel
                    label={t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.enableVariation") + " (*)"}
                    className={classes.field}
                    control={
                      <Checkbox
                        checked={true} // template.enableVariation
                        onChange={(ev, value) => onChange({...template, enableVariation: value})}
                        disabled
                      />
                    }
                  />
                </Grid>
                <Grid item xs={3}/>
                <Grid item xs={6}>
                  <FormControl fullWidth className={classes.field}>
                    <TextField
                      label={t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.decimalPlaces")}
                      value={template.decimalNumber}
                      variant="outlined"
                      required
                      onChange={ev => onChange({...template, decimalNumber: ev.target.value})}
                    />
                  </FormControl>
                </Grid>
                <Grid item xs={6}>
                  <FormControl fullWidth className={classes.field}>
                    <I18nTextField
                      select
                      label={t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.decimalSeparator.label")}
                      value={template.decimalSeparator}
                      defaultValue="."
                      variant="outlined"
                      required
                      onChange={value => onChange({...template, decimalSeparator: value})}
                    >
                      <MenuItem value=".">
                        {t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.decimalSeparator.values.dot")}
                      </MenuItem>
                      <MenuItem value=",">
                        {t("scenes.dataViewer.templateBuilder.tabs.options.cards.general.form.decimalSeparator.values.comma")}
                      </MenuItem>
                    </I18nTextField>
                  </FormControl>
                </Grid>
              </Grid>
            </Card>
            <Card variant="outlined" style={{marginTop: 16}}>
              <CardHeader
                title={t("scenes.dataViewer.templateBuilder.tabs.options.cards.table.title")}
                titleTypographyProps={{variant: "subtitle1"}}
              />
              <Grid container spacing={2} className={classes.card}>
                <Grid item xs={6}>
                  <FormControl fullWidth className={classes.field}>
                    <TextField
                      label={t("scenes.dataViewer.templateBuilder.tabs.options.cards.table.form.emptyCellPlaceholder")}
                      value={template.layouts.tableEmptyChar}
                      variant="outlined"
                      required
                      onChange={ev => onChange({
                        ...template,
                        layouts: {
                          ...template.layouts,
                          tableEmptyChar: ev.target.value
                        }
                      })}
                    />
                  </FormControl>
                </Grid>
                <Grid item xs={6}/>
              </Grid>
            </Card>
            <div
              style={{marginTop: 16}}>{"(*): " + t("scenes.dataViewer.templateBuilder.tabs.options.notImplemented")}</div>
          </Fragment>
        )}
        {tabId === 1 && (
          <Fragment>
            <FormControl fullWidth className={classes.field}>
              <TextField
                select
                label={t("scenes.dataViewer.templateBuilder.tabs.table.form.defaultLayout.label")}
                value={template.layouts.tableDefaultLayout}
                variant="outlined"
                onChange={ev => onChange({
                  ...template,
                  layouts: {
                    ...template.layouts,
                    tableDefaultLayout: ev.target.value,
                    tableLayout: ev.target.value === "custom"
                      ? tableLayout
                      : null
                  }
                })}
              >
                <MenuItem value="custom">
                  {t("scenes.dataViewer.templateBuilder.tabs.table.form.defaultLayout.values.custom")}
                </MenuItem>
                <MenuItem value="default">
                  {t("scenes.dataViewer.templateBuilder.tabs.table.form.defaultLayout.values.default")}
                </MenuItem>
              </TextField>
            </FormControl>
            <div className={classes.viewer}>
              {template.layouts.tableDefaultLayout === "custom"
                ? (
                  <JsonStatTable
                    jsonStat={jsonStat}
                    layout={tableLayout}
                    labelFormat={labelFormat}
                    decimalSeparator={localizeI18nObj(template.decimalSeparator, defaultLanguage, languages)}
                    decimalPlaces={template.decimalNumber}
                    emptyChar={template.layouts.tableEmptyChar}
                    hiddenAttributes={hiddenAttributes}
                  />
                )
                : (
                  <CustomEmpty
                    text={t("scenes.dataViewer.templateBuilder.tabs.table.form.defaultLayout.defaultPlaceholder")}/>
                )
              }
            </div>
          </Fragment>
        )}
        {tabId === 2 && (
          <Fragment>
            <FormControl fullWidth className={classes.field}>
              <TextField
                select
                label={t("scenes.dataViewer.templateBuilder.tabs.chart.cards.form.chartType.label")}
                defaultValue="table"
                value={chartType}
                variant="outlined"
                onChange={ev => setChartType(ev.target.value)}
              >
                {viewers.slice(2).map(({type, title}, idx) =>
                  <MenuItem key={idx} value={type}>{title}</MenuItem>
                )}
              </TextField>
            </FormControl>
            <div className={classes.viewer}>
              <Chart
                type={chartType}
                jsonStat={jsonStat}
                layout={chartLayout}
                labelFormat={labelFormat}
                decimalSeparator={localizeI18nObj(template.decimalSeparator, defaultLanguage, languages)}
                decimalPlaces={template.decimalNumber}
                stacked={template.layouts.chartStacked}
                legendPosition={template.layouts.chartLegendPosition}
                colors={template.layouts.chartColors}
              />
            </div>
          </Fragment>
        )}
        {tabId === 3 && (
          <div style={{width: "100%", height: "100%"}}>
            <Map
              mapId={mapId}
              jsonStat={jsonStat}
              layout={mapLayout}
              onGeometryFetch={idList => onGeometryFetch(idList, t)}
              geometries={templateGeometries}
              geometryDetailLevels={templateGeometryDetailLevels}
              detailLevel={template.layouts.mapDetailLevel}
              classificationMethod={template.layouts.mapClassificationMethod}
              paletteStartColor={template.layouts.mapPaletteStartColor}
              paletteEndColor={template.layouts.mapPaletteEndColor}
              paletteCardinality={template.layouts.mapPaletteCardinality}
              opacity={template.layouts.mapOpacity}
              isLegendCollapsed={template.layouts.mapIsLegendCollapsed}
            />
          </div>
        )}
      </div>
    </Fragment>
  )
}

export default withStyles(styles)(TemplateBuilder);