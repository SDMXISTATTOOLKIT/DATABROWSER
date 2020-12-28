import {Link, withStyles} from "@material-ui/core";
import AppBar from "@material-ui/core/AppBar";
import Toolbar from "@material-ui/core/Toolbar";
import IconButton from "@material-ui/core/IconButton";
import MenuIcon from "@material-ui/icons/Menu";
import Typography from "@material-ui/core/Typography";
import React, {Component} from "react";
import Grid from "@material-ui/core/Grid";
import UserSelect from "../user-select";
import Drawer from "@material-ui/core/Drawer";
import ChevronLeftIcon from "@material-ui/icons/ChevronLeft";
import Divider from "@material-ui/core/Divider";
import CategoriesTree from "../categories-tree";
import {withRouter} from "react-router";
import SettingsSelect from "../settings-select";
import SearchDialog from "../search-dialog";
import ButtonSelect from "../button-select";
import {goToDatasetsSearch, goToHome, goToNode, goToNodeDashboards} from "../../links";
import HomeIcon from '@material-ui/icons/Home';
import {compose} from "redux";
import {connect} from "react-redux";
import OtherSettingsSelect from "../other-settings-select";
import DashboardIcon from '@material-ui/icons/Dashboard';
import {
  canClearServerCache,
  canDisplayAppSettingsForm,
  canDisplayNodesSettingsForm,
  canDisplayUsersSettingsForm,
  canDisplayUserViews,
  canGetQueryLog,
  canManagePersonalDashboards
} from "../../utils/user";
import {withTranslation} from "react-i18next";
import AppLanguageSelect from "../app-language-select";
import A11ySelect from "../a11y-select";

const styles = theme => ({
  root: {
    position: "fixed",
    width: "100%",
    zIndex: theme.zIndex.appBar
  },
  title: {
    flexGrow: 1,
    display: 'none',
    [theme.breakpoints.up('sm')]: {
      display: 'block',
    },
    color: "white"
  },
  leftToolbar: {
    zIndex: theme.zIndex.appBar
  },
  leftContainer: {
    width: 448
  },
  rightContainer: {
    width: 448
  },
  drawer: {
    minWidth: 448
  },
  drawerHeader: {
    display: 'flex',
    alignItems: 'center',
    padding: theme.spacing(0, 1),
    ...theme.mixins.toolbar,
    justifyContent: 'flex-end',
  },
  drawerHeaderTitle: {
    position: "absolute",
    left: theme.spacing(2),
  },
  drawerCategories: {
    margin: theme.spacing(2)
  },
  drawerCategoriesTitle: {
    marginBottom: theme.spacing(2)
  },
  goToHome: {
    margin: theme.spacing(2, 2, 0, 2)
  },
  centerContainer: {
    width: "calc(100% - 896px)",
    textAlign: "center"
  },
  sistanLogo: {
    height: 32,
    transform: "translateY(3px)"
  }
});


const mapStateToProps = state => ({
  baseURL: state.config.baseURL,
  user: state.user,
  isA11y: state.app.isA11y
});

class NodeHeader extends Component {

  state = {
    isDrawerOpen: this.props.defaultTreeOpen || false
  };

  onDrawerOpen = () => {
    this.setState({
      isDrawerOpen: true
    });
  };

  onDrawerClose = () => {
    this.setState({
      isDrawerOpen: false
    });
  };

  render() {

    const {
      classes,
      selectedCategoryPath,
      title,
      defaultAppConfigOpen,
      defaultUsersConfigOpen,
      defaultNodesConfigOpen,
      defaultNodeConfigOpen,
      nodes,
      noNode,
      catalog,
      node,
      query,
      hub,
      baseURL,
      user,
      isA11y,
      isDefault,
      getCustomA11yPath,
      getAdditionalA11yUrlParams,
      t
    } = this.props;

    const {isDrawerOpen} = this.state;

    const isDefaultUniqueNode =
      !noNode && isDefault && node &&
      nodes.filter(n => n.code.toLowerCase() !== node.code.toLowerCase()).length === 0;

    return (
      <div className={classes.root} id="node-header" role="navigation">
        <AppBar position="static" color="primary">
          <Grid container justify="space-between" alignItems="center">
            <Grid item className={classes.leftContainer}>
              {!noNode && (
                <Toolbar className={classes.leftToolbar}>
                  <Grid container spacing={1} alignItems="center">
                    <Grid item>
                      <IconButton
                        className={classes.menuButton}
                        edge="start"
                        color="inherit"
                        onClick={this.onDrawerOpen}
                      >
                        <MenuIcon/>
                      </IconButton>
                    </Grid>
                    {!isDefaultUniqueNode && (
                      <Grid item>
                        <div id="node-selector">
                          <ButtonSelect
                            value={
                              <Typography variant="h6" color="inherit">
                                {node?.name}
                              </Typography>
                            }
                            onChange={code => goToNode(code.toLowerCase())}
                          >
                            {nodes.sort((a, b) => a.order - b.order).map(({code, name}, i) =>
                                <span key={i} data-value={code}>
                            {name}
                          </span>
                            )}
                          </ButtonSelect>
                        </div>
                      </Grid>
                    )}
                  </Grid>
                </Toolbar>
              )}
            </Grid>
            <Grid item className={classes.centerContainer}>
              <Link onClick={goToHome} style={{cursor: "pointer"}}>
                <Grid container spacing={2} alignItems="center" justify="center">
                  {hub.logoURL && (
                    <Grid item>
                      <img src={baseURL + hub.logoURL} alt={hub.name}
                           className={classes.sistanLogo}/>
                    </Grid>
                  )}
                  <Grid item>
                    <Typography variant="h6" color="secondary">
                      {hub.name}
                    </Typography>
                  </Grid>
                </Grid>
              </Link>
            </Grid>
            <Grid item className={classes.rightContainer}>
              <Toolbar>
                <Grid container justify="flex-end" alignItems="center" spacing={1}>
                  {!noNode && (
                    <div>
                      <SearchDialog
                        query={query}
                        modalWidth={320}
                        dialogTop={56}
                        onSubmit={value => goToDatasetsSearch(node.code, value)}
                      />
                    </div>
                  )}
                  <Grid item id="language-btn">
                    <AppLanguageSelect/>
                  </Grid>
                  <Grid item id="a11y-btn">
                    <A11ySelect
                      getAdditionalA11yUrlParams={getAdditionalA11yUrlParams}
                      getCustomA11yPath={getCustomA11yPath}
                    />
                  </Grid>
                  {!noNode && !isA11y && node && node.dashboards && node.dashboards.length > 0 && (
                    <Grid item id="dashboards-btn">
                      <IconButton
                        onClick={() => goToNodeDashboards(node.code)}
                        color="inherit"
                        aria-label={t("ariaLabels.header.dashboard")}
                      >
                        <DashboardIcon/>
                      </IconButton>
                    </Grid>
                  )}
                  {(canDisplayAppSettingsForm(user) ||
                    canDisplayUsersSettingsForm(user) ||
                    canDisplayNodesSettingsForm(user) ||
                    canGetQueryLog(user) ||
                    canClearServerCache(user)
                  ) && (
                    <Grid item id="admin-settings-btn">
                      <SettingsSelect
                        defaultAppOpen={defaultAppConfigOpen}
                        defaultNodesOpen={defaultNodesConfigOpen}
                        defaultUsersOpen={defaultUsersConfigOpen}
                        defaultNodeOpen={defaultNodeConfigOpen}
                        nodes={nodes}
                      />
                    </Grid>
                  )}
                  {(canDisplayUserViews(user) || canManagePersonalDashboards(user)) && (
                    <Grid item id="other-settings-btn">
                      <OtherSettingsSelect nodes={nodes}/>
                    </Grid>
                  )}
                  <Grid item id="user-settings-btn">
                    <UserSelect/>
                  </Grid>
                </Grid>
              </Toolbar>
            </Grid>
          </Grid>
        </AppBar>
        {!noNode && (
          <Drawer
            anchor="left"
            open={isDrawerOpen}
            classes={{
              paper: classes.drawer
            }}
            onClose={this.onDrawerClose}
          >
            <div className={classes.drawerHeader}>
              <Typography variant="h6" className={classes.drawerHeaderTitle}>
                {title}
              </Typography>
              <IconButton onClick={this.onDrawerClose}>
                <ChevronLeftIcon/>
              </IconButton>
            </div>
            <Divider/>
            <div className={classes.goToHome}>
              <Link underline="none" onClick={() => goToNode(node.code)}
                    style={{cursor: "pointer"}}>
                <Grid container spacing={1}>
                  <Grid item>
                    <HomeIcon/>
                  </Grid>
                  <Grid item>
                    <Typography>
                      {t("components.header.drawer.backToNodeHome")}
                    </Typography>
                  </Grid>
                </Grid>
              </Link>
            </div>
            {catalog && (
              <div className={classes.drawerCategories}>
                <div className={classes.drawerCategoriesTitle}>
                  <Typography>
                    {t("components.header.drawer.categoriesTitle")}
                  </Typography>
                </div>
                <CategoriesTree selectedCategoryPath={selectedCategoryPath} catalog={catalog}
                                node={node}
                                onClose={this.onDrawerClose}/>
              </div>
            )}
          </Drawer>
        )}
      </div>
    );
  }
}

export default compose(
  withStyles(styles),
  withTranslation(),
  connect(mapStateToProps),
  withRouter
)(NodeHeader);