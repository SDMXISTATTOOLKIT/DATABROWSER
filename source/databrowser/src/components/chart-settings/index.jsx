import React, {useState} from 'react';
import {compose} from "redux";
import withStyles from "@material-ui/core/styles/withStyles";
import {withTranslation} from "react-i18next";
import Box from "@material-ui/core/Box";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import Grid from "@material-ui/core/Grid";
import FormControlLabel from "@material-ui/core/FormControlLabel";
import TextField from "@material-ui/core/TextField";
import Checkbox from "@material-ui/core/Checkbox";
import MenuItem from "@material-ui/core/MenuItem";
import {
  CHART_LEGEND_POSITION_BOTTOM,
  CHART_LEGEND_POSITION_lEFT,
  CHART_LEGEND_POSITION_RIGHT,
  CHART_LEGEND_POSITION_TOP
} from "../chart";
import ChartSettingsColors from "./Colors";

const styles = theme => ({
  root: {
    padding: 4
  },
  tabs: {
    marginBottom: 4
  },
  tabContent: {
    height: 320
  }
});

function ChartSettings(props) {
  const {
    t,
    classes,
    jsonStat,
    isStacked,
    onIsStackedSet,
    legendPosition,
    onLegendPositionSet,
    colors,
    onColorsSet
  } = props;

  const [tabId, setTabId] = useState(0);

  return (
    <div className={classes.root}>
      <Box className={classes.tabs}>
        <Tabs
          value={tabId}
          variant="scrollable"
          scrollButtons="auto"
          onChange={(event, newValue) => setTabId(newValue)}
        >
          <Tab key={0} label={t("scenes.dataViewer.chartSettings.tabs.general")}/>
          <Tab key={1} label={t("scenes.dataViewer.chartSettings.tabs.colors")}/>
        </Tabs>
      </Box>
      <div className={classes.tabContent}>
        {tabId === 0 && (
          <Grid container spacing={3} style={{marginTop: 0}}>
            <Grid item xs={12}>
              <FormControlLabel
                label={t("scenes.dataViewer.chartSettings.isStacked")}
                control={
                  <Checkbox
                    checked={isStacked}
                    onChange={(e, value) => onIsStackedSet(value)}
                  />
                }
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                select
                fullWidth
                label={t("scenes.dataViewer.chartSettings.legendPosition.label")}
                value={legendPosition}
                variant="outlined"
                onChange={ev => onLegendPositionSet(ev.target.value)}
              >
                <MenuItem value={CHART_LEGEND_POSITION_TOP}>
                  {t("scenes.dataViewer.chartSettings.legendPosition.values.top")}
                </MenuItem>
                <MenuItem value={CHART_LEGEND_POSITION_RIGHT}>
                  {t("scenes.dataViewer.chartSettings.legendPosition.values.right")}
                </MenuItem>
                <MenuItem value={CHART_LEGEND_POSITION_BOTTOM}>
                  {t("scenes.dataViewer.chartSettings.legendPosition.values.bottom")}
                </MenuItem>
                <MenuItem value={CHART_LEGEND_POSITION_lEFT}>
                  {t("scenes.dataViewer.chartSettings.legendPosition.values.left")}
                </MenuItem>
              </TextField>
            </Grid>
          </Grid>
        )}
        {tabId === 1 && (
          <ChartSettingsColors
            jsonStat={jsonStat}
            colors={colors}
            onColorsSet={onColorsSet}
          />
        )}
      </div>
    </div>
  )
}

export default compose(
  withStyles(styles),
  withTranslation()
)(ChartSettings);