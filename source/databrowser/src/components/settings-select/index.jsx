import React, {Fragment} from 'react';
import ButtonSelect from "../button-select";
import SettingsIcon from "@material-ui/icons/Settings";
import SettingsDialog from "../settings-dialog";
import AppSettingsForm from "./app-settings-form";
import {withStyles} from "@material-ui/core";
import NodesSettingsForm from "./nodes-settings-form";
import {connect} from "react-redux";
import {compose} from "redux";
import {clearMemoryCache, downloadQueryLog} from "./actions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";
import Input from "@material-ui/core/Input";
import Grid from "@material-ui/core/Grid";
import UsersSettingsForm from "./users-settings-form";
import {
  canDisplayAppSettingsForm,
  canDisplayNodesSettingsForm,
  canDisplayUsersSettingsForm,
  canGetQueryLog
} from "../../utils/user";
import {withTranslation} from "react-i18next";

const styles = theme => ({
  option: {
    width: "100%"
  }
});

const mapStateToProps = state => ({
  user: state.user
});

const mapDispatchToProps = dispatch => ({
  onLogDownload: limit => dispatch(downloadQueryLog(limit)),
  onCacheClear: () => dispatch(clearMemoryCache())
});

class SettingsSelect extends React.Component {

  state = {
    isAppOpen: false,
    isNodesOpen: false,
    isUsersOpen: false,
    appSettingsRef: React.createRef(),
    nodesSettingsRef: React.createRef(),
    usersSettingsRef: React.createRef(),
    isLogDownloadOpen: false,
    logLimit: 5,
  };

  onAppOpen = () => this.setState({
    ...this.state,
    isAppOpen: true
  });

  onAppClose = showSnackbar => this.setState({
    ...this.state,
    isAppOpen: false,
    isAppSubmitSnackbarVisible: showSnackbar || false
  });

  onNodesOpen = () => this.setState({
    ...this.state,
    isNodesOpen: true
  });

  onNodesClose = () => this.setState({
    ...this.state,
    isNodesOpen: false
  });

  onUsersOpen = () => this.setState({
    ...this.state,
    isUsersOpen: true
  });

  onUsersClose = () => this.setState({
    ...this.state,
    isUsersOpen: false
  });

  onLogDownloadOpen = () => this.setState({
    ...this.state,
    isLogDownloadOpen: true
  });

  onLogDownloadClose = () => this.setState({
    ...this.state,
    isLogDownloadOpen: false,
    logLimit: 5
  });

  onLogLimitSet = logLimit => {
    if (!isNaN(logLimit)) {
      this.setState({
        ...this.state,
        logLimit: logLimit
      })
    }
  }

  render() {

    const {
      classes,
      onLogDownload,
      nodes,
      user,
      t
    } = this.props;

    const {
      isAppOpen,
      isNodesOpen,
      isUsersOpen,
      appSettingsRef,
      nodesSettingsRef,
      usersSettingsRef,
      isLogDownloadOpen,
      logLimit
    } = this.state;

    return (
      <Fragment>
        <ButtonSelect value="" icon={<SettingsIcon/>} ariaLabel={t("ariaLabels.header.settings")}>
          {canDisplayAppSettingsForm(user) && (
            <div onClick={this.onAppOpen} className={classes.option}>{t("components.header.settings.app")}</div>
          )}
          {canDisplayUsersSettingsForm(user) && (
            <div onClick={this.onUsersOpen} className={classes.option}>{t("components.header.settings.users")}</div>
          )}
          {canDisplayNodesSettingsForm(user) && (
            <div onClick={this.onNodesOpen} className={classes.option}>{t("components.header.settings.nodes")}</div>
          )}
          {canGetQueryLog(user) && (
            <div onClick={this.onLogDownloadOpen}
                 className={classes.option}>{t("components.header.settings.queriesLog")}</div>
          )}
          {/*{canClearServerCache(user) && (
            <div onClick={onCacheClear}
                 className={classes.option}>{t("components.header.settings.clearServerCache")}</div>
          )}*/}
        </ButtonSelect>
        <SettingsDialog
          title={t("scenes.appSettings.title")}
          open={isAppOpen}
          onClose={() => {
            if (appSettingsRef.current) {
              appSettingsRef.current.cancel(() => this.onAppClose());
            } else {
              this.onAppClose();
            }
          }}
          onSubmit={() => {
            if (appSettingsRef.current) {
              appSettingsRef.current.submit(() => {
                this.onAppClose();
              });
            } else {
              this.onAppClose();
            }
          }}
          hasSubmit
        >
          <AppSettingsForm ref={appSettingsRef}/>
        </SettingsDialog>
        <SettingsDialog
          title={t("scenes.nodesSettings.title")}
          open={isNodesOpen}
          onClose={() => {
            if (nodesSettingsRef.current) {
              nodesSettingsRef.current.cancel(() => this.onNodesClose());
            } else {
              this.onNodesClose();
            }
          }}
          maxWidth="md"
        >
          <NodesSettingsForm ref={nodesSettingsRef} nodes={nodes} onNodesClose={this.onNodesClose}/>
        </SettingsDialog>

        <SettingsDialog
          title={t("scenes.usersSettings.title")}
          open={isUsersOpen}
          onClose={() => {
            if (usersSettingsRef.current) {
              usersSettingsRef.current.cancel(() => this.onUsersClose());
            } else {
              this.onUsersClose();
            }
          }}
          maxWidth="md"
        >
          <UsersSettingsForm ref={usersSettingsRef}/>
        </SettingsDialog>

        <Dialog
          open={isLogDownloadOpen}
          onClose={this.onLogDownloadClose}
        >
          <DialogContent>
            <Grid container spacing={2} style={{width: 400}}>
              <Grid item xs={12}>
                <div>{t("components.header.settings.queriesLogModal.content")}:</div>
              </Grid>
              <Grid item xs={12}>
                <Input
                  value={logLimit}
                  onChange={ev => this.onLogLimitSet(ev.target.value)}
                  style={{width: "100%"}}
                />
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            <Button onClick={this.onLogDownloadClose}>
              {t("commons.confirm.cancel")}
            </Button>
            <Button
              autoFocus
              onClick={() => {
                this.onLogDownloadClose()
                onLogDownload(logLimit);
              }}
              color="primary">
              {t("commons.confirm.download")}
            </Button>
          </DialogActions>
        </Dialog>

      </Fragment>
    );
  }
}

export default compose(
  connect(mapStateToProps, mapDispatchToProps),
  withStyles(styles),
  withTranslation()
)(SettingsSelect);