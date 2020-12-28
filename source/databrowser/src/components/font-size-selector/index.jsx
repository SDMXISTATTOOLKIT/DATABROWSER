import React, {Fragment} from "react";
import {compose} from "redux";
import {withTranslation} from "react-i18next";
import withStyles from "@material-ui/core/styles/withStyles";
import Tooltip from "@material-ui/core/Tooltip";
import IconButton from "@material-ui/core/IconButton";

export const FONT_SIZE_SELECTOR_FONT_SIZE_SM = "sm";
export const FONT_SIZE_SELECTOR_FONT_SIZE_MD = "md";
export const FONT_SIZE_SELECTOR_FONT_SIZE_LG = "lg";

const styles = theme => ({
  fontSizeIcon: {
    width: 24,
    height: 24,
    fontWeight: "bold",
    display: "table"
  },
  fontSizeIconSm: {
    fontSize: 14
  },
  fontSizeIconMd: {
    fontSize: 16
  },
  fontSizeIconLg: {
    fontSize: 20
  },
  fontSizeSelected: {
    "& div": {
      boxShadow: "inset 0 -2px 0 0 rgba(0, 0, 0, .54)"
    }
  }
});

const getFontSizeIcon = styleClasses =>
  <div className={styleClasses}>
    <div style={{display: "table-cell", verticalAlign: "middle"}}>
      A
    </div>
  </div>;

const FontSizeSelector = ({
  t,
                            classes,
                            fontSize,
                            setFontSize
                          }) =>

  <Fragment>
    <Tooltip title={t("components.fontSizeSelector.small.tooltip")}>
      <IconButton
        className={fontSize === FONT_SIZE_SELECTOR_FONT_SIZE_SM ? classes.fontSizeSelected : ""}
        onClick={() => setFontSize(FONT_SIZE_SELECTOR_FONT_SIZE_SM)}
      >
        {getFontSizeIcon(`${classes.fontSizeIcon} ${classes.fontSizeIconSm}`)}
      </IconButton>
    </Tooltip>
    <Tooltip title={t("components.fontSizeSelector.normal.tooltip")}>
      <IconButton
        className={fontSize === FONT_SIZE_SELECTOR_FONT_SIZE_MD ? classes.fontSizeSelected : ""}
        onClick={() => setFontSize(FONT_SIZE_SELECTOR_FONT_SIZE_MD)}
      >
        {getFontSizeIcon(`${classes.fontSizeIcon} ${classes.fontSizeIconMd}`)}
      </IconButton>
    </Tooltip>
    <Tooltip title={t("components.fontSizeSelector.big.tooltip")}>
      <IconButton
        className={fontSize === FONT_SIZE_SELECTOR_FONT_SIZE_LG ? classes.fontSizeSelected : ""}
        onClick={() => setFontSize(FONT_SIZE_SELECTOR_FONT_SIZE_LG)}
      >
        {getFontSizeIcon(`${classes.fontSizeIcon} ${classes.fontSizeIconLg}`)}
      </IconButton>
    </Tooltip>
  </Fragment>;

export default compose(
  withStyles(styles),
  withTranslation()
)(FontSizeSelector)