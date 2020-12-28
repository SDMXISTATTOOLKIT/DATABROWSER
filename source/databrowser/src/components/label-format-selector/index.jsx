import React from "react";
import {compose} from "redux";
import {withTranslation} from "react-i18next";
import withStyles from "@material-ui/core/styles/withStyles";
import FormControl from "@material-ui/core/FormControl";
import Grid from "@material-ui/core/Grid";
import Select from "@material-ui/core/Select";
import MenuItem from "@material-ui/core/MenuItem";
import {
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_BOTH,
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_ID,
  LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME
} from "./constants";

const styles = theme => ({
  root: {
    margin: "0 8px"
  },
  label: {
    color: "rgba(0, 0, 0, 0.54)",
    fontSize: 13,
    height: 40,
    lineHeight: "40px",
    marginRight: 8
  },
  formControl: {
    margin: "8px 0"
  },
  select: {
    "& .MuiSelect-root.MuiSelect-select": {
      padding: "2px 24px 3px 0",
      fontSize: 14
    }
  },
  menuItem: {
    fontSize: 14
  }
});

const LabelFormatSelector = ({
                               t,
                               classes,
                               labelFormat,
                               setLabelFormat
                             }) =>
  <Grid container className={classes.root}>
    <Grid item className={classes.label}>
      {t("components.labelFormatSelector.labelFormat.title")}:
    </Grid>
    <Grid item>
      <FormControl className={classes.formControl}>
        <Select
          className={classes.select}
          value={(labelFormat || "")}
          onChange={(ev) => setLabelFormat(ev.target.value)}
        >
          <MenuItem className={classes.menuItem} value={LABEL_FORMAT_SELECTOR_LABEL_FORMAT_NAME}>
            {t("components.labelFormatSelector.labelFormat.values.name")}
          </MenuItem>
          <MenuItem className={classes.menuItem} value={LABEL_FORMAT_SELECTOR_LABEL_FORMAT_ID}>
            {t("components.labelFormatSelector.labelFormat.values.id")}
          </MenuItem>
          <MenuItem className={classes.menuItem} value={LABEL_FORMAT_SELECTOR_LABEL_FORMAT_BOTH}>
            {t("components.labelFormatSelector.labelFormat.values.both")}
          </MenuItem>
        </Select>
      </FormControl>
    </Grid>
  </Grid>

export default compose(
  withStyles(styles),
  withTranslation()
)(LabelFormatSelector)