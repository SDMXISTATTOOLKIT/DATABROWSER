import React from 'react';
import {connect} from "react-redux";
import ButtonSelect from "../button-select";
import AccessibilityNewIcon from '@material-ui/icons/AccessibilityNew';
import {useTranslation} from "react-i18next";
import Badge from "@material-ui/core/Badge";
import {withStyles} from "@material-ui/core";
import {compose} from "redux";
import CheckIcon from '@material-ui/icons/Check';
import {useLocation} from "react-router";

const styles = theme => ({
  badge: {
    padding: 0,
    width: 12,
    height: 12,
    minWidth: 0
  },
  badgeActive: {
    padding: 0,
    width: 12,
    height: 12,
    minWidth: 0,
    backgroundColor: theme.palette.success.main,
  },
  badgeIcon: {
    fontSize: 12
  }
});

const mapStateToProps = state => ({
  isA11y: state.app.isA11y
});

const A11ySelect = ({classes, isA11y, getCustomA11yPath, getAdditionalA11yUrlParams}) => {

  const location = useLocation();
  const {t} = useTranslation();

  return (
    <ButtonSelect
      value={
        <Badge
          showZero
          color="secondary"
          anchorOrigin={{
            vertical: 'bottom',
            horizontal: 'right',
          }}
          badgeContent={isA11y ? <CheckIcon classes={{root: classes.badgeIcon}}/> : undefined}
          classes={{
            badge: isA11y ? classes.badgeActive : classes.badge
          }}
        >
          <AccessibilityNewIcon/>
        </Badge>
      }
      onChange={val => {

        if (val !== isA11y) {

          const params = new URLSearchParams(location.search);

          if (val) {
            params.set("accessible", "true");
          } else {
            params.delete("accessible")
          }

          if (getAdditionalA11yUrlParams) {
            const additionalA11yUrlParams = getAdditionalA11yUrlParams(val);
            if (additionalA11yUrlParams && Object.keys(additionalA11yUrlParams).length > 0) {
              Object.keys(additionalA11yUrlParams).forEach(key => params.set(key, additionalA11yUrlParams[key]));
            }
          }

          const paramsStr = params.toString();

          const path = getCustomA11yPath
            ? (getCustomA11yPath(val) || location.pathname)
            : location.pathname;

          window.open(
            "./#" + path + (paramsStr.length > 0 ? "?" : "") + paramsStr,
            "_self"
          );
        }
      }}
      ariaLabel={t("ariaLabels.header.a11y")}
    >
      <span data-value={false}>{t("components.a11ySelect.disable")}</span>
      <span data-value={true}>{t("components.a11ySelect.enable")}</span>
    </ButtonSelect>
  );
};

export default compose(
  withStyles(styles),
  connect(mapStateToProps)
)(A11ySelect);