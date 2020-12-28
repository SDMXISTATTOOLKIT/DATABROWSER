import React, {useState} from "react";
import {useTranslation} from "react-i18next";
import withStyles from "@material-ui/core/styles/withStyles";
import Divider from "@material-ui/core/Divider";
import MenuList from "@material-ui/core/MenuList";
import BarChartIcon from '@material-ui/icons/BarChart';
import BottomNavigationAction from "@material-ui/core/BottomNavigationAction";
import BottomNavigation from "@material-ui/core/BottomNavigation";
import SettingsIcon from "@material-ui/icons/Settings";
import EditIcon from "@material-ui/icons/Edit";
import MenuItem from "@material-ui/core/MenuItem";
import Menu from "@material-ui/core/Menu";
import Typography from "@material-ui/core/Typography";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogActions from "@material-ui/core/DialogActions";
import DialogTitle from "@material-ui/core/DialogTitle";
import Button from "@material-ui/core/Button";

const styles = theme => ({
  root: {
    textAlign: "center",
    '& ul': {
      padding: "4px 0",
      outline: "none"
    },
    "& .MuiBottomNavigation-root": {
      height: "unset !important",
      outline: "none"
    }
  },
  floatingMenuOption: {
    width: 200,
    color: "gray"
  },
  floatingMenuOptionSelected: {
    color: theme.palette.primary.main
  },
  divider: {
    margin: "4px 0"
  },
  categoryTitle: {
    color: "gray",
    fontSize: 11,
    fontWeight: 500
  },
  referenceMetadataIcon: {
    width: 24,
    height: 24,
    fontSize: 22,
    fontWeight: "bold",
    display: "table"
  },
  iFrame: {
    width: 960,
    border: 0
  },
  navigationActionDisabled: {
    color: "rgba(0, 0, 0, 0.26) !important",
    "& svg": {
      color: "rgba(0, 0, 0, 0.26)"
    }
  }
});

function DataViewerSideBar(props) {

  const {
    classes,
    viewers,
    selected,
    isCriteriaDisabled,
    isLayoutDisabled,
    isTableDisabled,
    isChartDisabled,
    isMapDisabled,
    referenceMetadataUrl,
    onViewerSelect,
    onCriteriaOpen,
    onLayoutOpen
  } = props;

  const [anchorEl, setAnchorEl] = useState(null);

  const [isRefMetaVisible, setRefMetaVisibility] = useState(false);

  const {t} = useTranslation();

  const handleViewerSelect = index => {
    onViewerSelect(index);
    setAnchorEl(null);
  };

  return (
    <div className={classes.root}>
      <MenuList>

        <BottomNavigation onChange={onCriteriaOpen} showLabels>
          <BottomNavigationAction
            label={t("scenes.dataViewer.sidebar.criteria")}
            icon={<SettingsIcon/>}
            className={isCriteriaDisabled ? classes.navigationActionDisabled : ""}
            disabled={isCriteriaDisabled}
          />
        </BottomNavigation>
        <BottomNavigation onChange={onLayoutOpen} showLabels>
          <BottomNavigationAction
            label={t("scenes.dataViewer.sidebar.layout")}
            icon={<EditIcon/>}
            className={(isLayoutDisabled || selected === 1) ? classes.navigationActionDisabled : ""}
            disabled={isLayoutDisabled || selected === 1}
          />
        </BottomNavigation>

        <Divider className={classes.divider}/>

        {referenceMetadataUrl && (
          <BottomNavigation
            onChange={() => setRefMetaVisibility(true)}
            showLabels style={{marginBottom: 8}}
          >
            <BottomNavigationAction
              label={t("scenes.dataViewer.sidebar.referenceMetadata")}
              icon={
                <div className={classes.referenceMetadataIcon}>
                  <div style={{display: "table-cell", verticalAlign: "middle"}}>
                    M
                  </div>
                </div>
              }
            />
          </BottomNavigation>
        )}

        <BottomNavigation
          value={selected === 0 ? 0 : null}
          onChange={() => handleViewerSelect(0)}
          showLabels
        >
          <BottomNavigationAction
            label={viewers[0].title}
            icon={viewers[0].icon}
            className={isTableDisabled ? classes.navigationActionDisabled : ""}
            disabled={isTableDisabled}
          />
        </BottomNavigation>
        <BottomNavigation
          value={selected >= 2 ? 0 : null}
          onChange={({currentTarget}) => setAnchorEl(currentTarget)}
          showLabels
        >
          <BottomNavigationAction
            label={selected >= 2 ? viewers[selected].title : t("scenes.dataViewer.sidebar.chart")}
            icon={selected >= 2 ? viewers[selected].icon : <BarChartIcon/>}
            className={isChartDisabled ? classes.navigationActionDisabled : ""}
            disabled={isChartDisabled}
          />
        </BottomNavigation>
        <BottomNavigation
          value={selected === 1 ? 0 : null}
          onChange={() => handleViewerSelect(1)}
          showLabels
        >
          <BottomNavigationAction
            label={viewers[1].title}
            icon={viewers[1].icon}
            className={isMapDisabled ? classes.navigationActionDisabled : ""}
            disabled={isMapDisabled}
          />
        </BottomNavigation>

      </MenuList>

      <Menu
        anchorEl={anchorEl}
        keepMounted
        open={Boolean(anchorEl)}
        onClose={() => setAnchorEl(null)}
      >
        {viewers
          .slice(2)
          .filter(({hidden}) => hidden !== true)
          .map(option => (
            <MenuItem
              className={`${classes.floatingMenuOption} ${selected === option.key ? classes.floatingMenuOptionSelected : ''}`}
              key={option.key}
              selected={selected === option.key}
              onClick={() => handleViewerSelect(option.key)}
            >
              {option.icon}
              <Typography style={{marginLeft: 8}}>{option.title}</Typography>
            </MenuItem>
          ))}
      </Menu>

      <Dialog
        open={isRefMetaVisible}
        fullWidth
        maxWidth="md"
        onClose={() => setRefMetaVisibility(false)}
      >
        <DialogTitle>
          {t("scenes.dataViewer.sidebar.dialogs.referenceMetadata.title")}
        </DialogTitle>
        <DialogContent style={{height: 480}}>
          <iframe
            title={"title"}
            src={referenceMetadataUrl}
            style={{
              border: 0,
              width: "100%",
              height: "calc(100% - 6px)"
            }}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setRefMetaVisibility(false)}>
            {t("commons.confirm.close")}
          </Button>
        </DialogActions>
      </Dialog>

    </div>
  );
}

export default withStyles(styles)(DataViewerSideBar)