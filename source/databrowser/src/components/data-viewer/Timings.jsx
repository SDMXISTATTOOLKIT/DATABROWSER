import React, {Fragment} from 'react';
import {compose} from "redux";
import {withTranslation} from "react-i18next";
import {withStyles} from "@material-ui/core";
import Grid from "@material-ui/core/Grid";
import Divider from "@material-ui/core/Divider";
import {GENERATING_HTML_TIME_KEY, OBSERVATION_COUNT_KEY, SERVER_TIMINGS_KEY} from "../../state/dataset/datasetReducer";
import {
  getTimingsLabelTranslations,
  NSI_RESPONSE_DOWNLOAD_SIZE_KEY,
  TOTAL_KEY
} from "../../constants/getTimingsLabelTranslation";

const styles = theme => ({
  root: {},
  divider: {
    margin: "16px 0"
  }
});

const getRow = (label, value, isTitle) => (
  <Grid
    container
    key={label}
    justify="space-between"
    spacing={2}
    style={{
      height: isTitle ? 40 : undefined
    }}
  >
    <Grid
      item
      style={isTitle
        ? {
          fontSize: 15,
          fontWeight: "bold",
          textDecoration: "underline"
        }
        : undefined
      }
    >
      {label}
    </Grid>
    {value && (
      <Grid item><b>{value}</b></Grid>
    )}
  </Grid>
);

const DataViewerTimings = ({
                             t,
                             classes,
                             timings
                           }) => {
  const rows = [];

  if (timings) {
    if (timings[GENERATING_HTML_TIME_KEY] !== null && timings[GENERATING_HTML_TIME_KEY] !== undefined) {
      rows.push(getRow(t("timings.observationCount"), timings[OBSERVATION_COUNT_KEY], true));
      rows.push(<Divider className={classes.divider} key="divider1"/>);
    }

    /** server side **/
    if (timings[SERVER_TIMINGS_KEY]) {

      rows.push(getRow(t("timings.serverTimings"), null, true));

      for (const key in timings[SERVER_TIMINGS_KEY]) {
        if (timings[SERVER_TIMINGS_KEY].hasOwnProperty(key)) {
          if (key !== TOTAL_KEY && key !== NSI_RESPONSE_DOWNLOAD_SIZE_KEY) {
            rows.push(getRow(getTimingsLabelTranslations(t)[key], timings[SERVER_TIMINGS_KEY][key]))
          }
        }
      }
      rows.push(getRow(getTimingsLabelTranslations(t)[TOTAL_KEY], timings[SERVER_TIMINGS_KEY][TOTAL_KEY], true));
      rows.push(<Divider className={classes.divider} key="divider2"/>);

      if (timings[SERVER_TIMINGS_KEY][NSI_RESPONSE_DOWNLOAD_SIZE_KEY]) {
        rows.push(getRow(getTimingsLabelTranslations(t)[NSI_RESPONSE_DOWNLOAD_SIZE_KEY], timings[SERVER_TIMINGS_KEY][NSI_RESPONSE_DOWNLOAD_SIZE_KEY], true));
        rows.push(<Divider className={classes.divider} key="divider3"/>);
      }
    }

    /** client side **/
    if (timings[GENERATING_HTML_TIME_KEY] !== null && timings[GENERATING_HTML_TIME_KEY] !== undefined) {
      rows.push(getRow(t("timings.clientTimings"), null, true));
      rows.push(getRow(t("timings.generatingHtml"), `${timings[GENERATING_HTML_TIME_KEY]}ms`));
    }
  }

  return (
    <Fragment>
      {rows}
    </Fragment>
  )
}


export default compose(
  withStyles(styles),
  withTranslation()
)(DataViewerTimings)