import React, {Fragment} from 'react';
import ButtonSelect from "../button-select";
import SettingsDialog from "../settings-dialog";
import {withStyles} from "@material-ui/core";
import ViewsSettingsForm from "./views-settings-form";
import WorkIcon from '@material-ui/icons/Work';
import {compose} from "redux";
import {connect} from "react-redux";
import {canDisplayUserViews, canManagePersonalDashboards} from "../../utils/user";
import UserDashboardsSettingsForm from "./user-dashboards-settings-form";
import {withTranslation} from "react-i18next";

const styles = theme => ({
  option: {
    width: "100%"
  }
});

const mapStateToProps = state => ({
  user: state.user
});

class OtherSettingsSelect extends React.Component {

  state = {
    isViewsOpen: false,
    isDashboardsOpen: false,
    viewsSettigsRef: React.createRef(),
    dashboardsSettingsRef: React.createRef()
  };

  onViewsOpen = () => this.setState({
    ...this.state,
    isViewsOpen: true
  });

  onViewsClose = () => this.setState({
    ...this.state,
    isViewsOpen: false
  });

  onDashboardsOpen = () => this.setState({
    ...this.state,
    isDashboardsOpen: true
  });

  onDashboardsClose = () => this.setState({
    ...this.state,
    isDashboardsOpen: false
  });

  render() {

    const {
      classes,
      nodes,
      user,
      t
    } = this.props;

    const {
      isViewsOpen,
      isDashboardsOpen,
      viewsSettigsRef,
      dashboardsSettingsRef
    } = this.state;

    return (
      <Fragment>
        <ButtonSelect value="" icon={<WorkIcon/>} ariaLabel={t("ariaLabels.header.otherSettings")}>
          {canDisplayUserViews(user) && (
            <div onClick={this.onViewsOpen} className={classes.option}>
              {t("components.header.otherSettings.views")}
            </div>
          )}
          {canManagePersonalDashboards(user) && (
            <div onClick={this.onDashboardsOpen} className={classes.option}>
              {t("components.header.otherSettings.dashboards")}
            </div>
          )}
        </ButtonSelect>
        <SettingsDialog
          title={t("scenes.viewsSettings.title")}
          open={isViewsOpen}
          onClose={() => {
            if (viewsSettigsRef.current) {
              viewsSettigsRef.current.cancel(() => this.onViewsClose());
            } else {
              this.onViewsClose();
            }
          }}
          maxWidth="md"
        >
          <ViewsSettingsForm
            ref={viewsSettigsRef}
            nodes={nodes}
            onViewsClose={this.onViewsClose}
          />
        </SettingsDialog>
        <SettingsDialog
          title={t("scenes.dashboardsSettings.title")}
          open={isDashboardsOpen}
          onClose={() => {
            if (dashboardsSettingsRef.current) {
              dashboardsSettingsRef.current.cancel(() => this.onDashboardsClose());
            } else {
              this.onDashboardsClose();
            }
          }}
          maxWidth="md"
        >
          <UserDashboardsSettingsForm
            ref={dashboardsSettingsRef}
            nodes={nodes}
            onDashboardsClose={this.onDashboardsClose}
          />
        </SettingsDialog>
      </Fragment>
    );
  }
}

export default compose(
  withStyles(styles),
  connect(mapStateToProps),
  withTranslation()
)(OtherSettingsSelect);