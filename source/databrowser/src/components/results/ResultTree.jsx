import React, {useState} from "react";
import TreeItem from "@material-ui/lab/TreeItem";
import Grid from "@material-ui/core/Grid";
import TreeView from "@material-ui/lab/TreeView";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import ChevronRightIcon from "@material-ui/icons/ChevronRight";
import {goToDatasetsSearch} from "../../links";
import Checkbox from "@material-ui/core/Checkbox";
import {withStyles} from "@material-ui/core";
import Box from "@material-ui/core/Box";
import {withTranslation} from "react-i18next";
import {compose} from "redux";

const styles = () => ({
  container: {
    background: "#F5F5F5",
    paddingTop: 8,
    paddingBottom: 8
  },
  nodeIcon: {
    marginTop: 5
  },
  item: {
    background: "none !important"
  },
  resCount: {
    fontWeight: "bold",
    marginLeft: 4
  },
  checkbox: {
    padding: 0,
    transform: "translateY(-2px)"
  }
});

function ResultTree(props) {
  const {
    classes,
    catalog,
    query,
    node,
    filters = [],
    categoryGroupsWithCount,
    uncategorizedDatasetsCount,
    t
  } = props;

  const [expanded, setExpanded] = useState([]);

  const getTreeItems = (tree, prevPath, noCheckbox = false) =>
    tree.map((cat, i) => {

      const selected = !!filters.find(filter => filter.includes(cat.id));

      return (
        <TreeItem
          key={i}
          nodeId={cat.id}
          label={
            <Grid spacing={1} container alignItems="flex-start"
                  style={{flexWrap: "nowrap", marginTop: 4}}>
              {!noCheckbox && (
                <Grid item>
                  <label htmlFor={`result-tree__checkbox__${cat.id}`} style={{display: "none"}}>
                    {cat.label}
                  </label>
                  <Checkbox
                    id={`result-tree__checkbox__${cat.id}`}
                    checked={selected}
                    className={classes.checkbox}
                    onClick={
                      e => {
                        goToDatasetsSearch(
                          node.code,
                          query,
                          selected
                            ? filters.filter(path => !path.includes(cat.id))
                            : [...filters, [...prevPath, cat.id]]
                        );
                        e.stopPropagation();
                      }}
                  />
                </Grid>
              )}
              <Grid item style={{width: "100%"}}>
                {cat.label}
                {!noCheckbox && (
                  <span className={classes.resCount}> ({cat.count})</span>
                )}
              </Grid>
            </Grid>
          }
          classes={{
            label: classes.item
          }}
        >
          {cat.categories && cat.categories.length > 0 && getTreeItems(cat.categories, [...prevPath, cat.id])}
        </TreeItem>
      );
    });

  const uncategorizedSelected = !!filters.find(filter => filter.includes("uncategorized"));

  return (
    <Box className={classes.container}>
      {categoryGroupsWithCount.length > 0 && (
        <TreeView
          defaultCollapseIcon={<ExpandMoreIcon/>}
          defaultExpandIcon={<ChevronRightIcon/>}
          expanded={expanded}
          onNodeToggle={(_, nodeIds) => setExpanded(nodeIds)}
        >
          {getTreeItems(
            catalog.categoryGroups.length > 1
              ? categoryGroupsWithCount
              : categoryGroupsWithCount[0].categories,
            [],
            catalog.categoryGroups.length > 1
          )}
        </TreeView>
      )}
      {uncategorizedDatasetsCount > 0 && (
        <TreeView
          defaultCollapseIcon={<ExpandMoreIcon/>}
          defaultExpandIcon={<ChevronRightIcon/>}
          expanded={expanded}
          onNodeToggle={(_, nodeIds) => setExpanded(nodeIds)}
        >
          <TreeItem
            key={"uncategorized"}
            nodeId={"uncategorized"}
            label={
              <Grid spacing={1} container alignItems="flex-start" style={{flexWrap: "nowrap", marginTop: 4}}>
                <Grid item>
                  <label htmlFor="result-tree__checkbox__uncategorized" style={{display: "none"}}>
                    {t("commons.catalog.uncategorized")}
                  </label>
                  <Checkbox
                    id="result-tree__checkbox__uncategorized"
                    checked={uncategorizedSelected}
                    className={classes.checkbox}
                    onClick={
                      e => {
                        goToDatasetsSearch(
                          node.code,
                          query,
                          uncategorizedSelected
                            ? filters.filter(path => JSON.stringify(path) !== JSON.stringify(["uncategorized"]))
                            : [...filters, ["uncategorized"]]
                        );
                        e.stopPropagation();
                      }}
                  />
                </Grid>
                <Grid style={{width: "100%"}} item>
                  {t("commons.catalog.uncategorized")} <span
                  className={classes.resCount}>({uncategorizedDatasetsCount})</span>
                </Grid>
              </Grid>
            }
            classes={{label: classes.item}}
          />
        </TreeView>
      )}
    </Box>
  );
}

export default compose(
  withStyles(styles),
  withTranslation()
)(ResultTree);