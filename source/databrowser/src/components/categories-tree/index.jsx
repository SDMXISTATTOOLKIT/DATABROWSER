import React, {Component, Fragment} from 'react';
import TreeView from '@material-ui/lab/TreeView';
import TreeItem from '@material-ui/lab/TreeItem';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import InfoOutlinedIcon from '@material-ui/icons/InfoOutlined';
import IconButton from "@material-ui/core/IconButton";
import Grid from "@material-ui/core/Grid";
import {Link, withStyles} from "@material-ui/core";
import {goToDatasets} from "../../links";
import FolderOpenIcon from '@material-ui/icons/FolderOpen';
import ListIcon from '@material-ui/icons/List';
import FolderIcon from '@material-ui/icons/Folder';
import {withTranslation} from "react-i18next";
import {compose} from "redux";

const styles = () => ({
  nodeIcon: {
    marginTop: 5
  },
  selectedItem: {
    background: "rgba(0, 41, 90, 0.2) !important"
  }
});

class CategoriesTree extends Component {

  state = {
    expanded: this.props.selectedCategoryPath || []
  };

  onExpand = expanded => this.setState({expanded});

  render() {

    const {classes, node, catalog, selectedCategoryPath = [], onClose, t} = this.props;
    const {expanded} = this.state;

    const getTreeItems = (tree, prevPath) =>
      tree.map((cat, i) =>
        <TreeItem
          key={i}
          nodeId={cat.id}
          label={
            <Grid spacing={1} container alignItems="center">
              <Grid item>
                {(expanded.includes(cat.id))
                  ? <FolderOpenIcon className={classes.nodeIcon}/>
                  : <FolderIcon className={classes.nodeIcon}/>
                }
              </Grid>
              <Grid item>
                {cat.label}
              </Grid>
              {cat.hasAnnotation && (
                <Grid item>
                  <IconButton
                    size="small"
                    onClick={this.onDialogOpen}
                  >
                    <InfoOutlinedIcon/>
                  </IconButton>
                </Grid>
              )}
            </Grid>
          }
          /* TODO: inefficente */
          classes={{
            label: selectedCategoryPath.includes(cat.id) ? classes.selectedItem : null
          }}
        >
          {(
            (cat.datasetIdentifiers && cat.datasetIdentifiers.length > 0) ||
            (cat.childrenCategories && cat.childrenCategories.length > 0)
          ) && (
            <Fragment>
              {(cat.datasetIdentifiers && cat.datasetIdentifiers.length > 0) && (
                <TreeItem
                  nodeId={cat.id + "-results"}
                  label={
                    <Link onClick={() => {
                      onClose();
                      goToDatasets(node.code.toLowerCase(), [...prevPath, cat.id]);
                    }}>
                      <Grid spacing={1} container alignItems="center">
                        <Grid item>
                          <ListIcon className={classes.nodeIcon}/>
                        </Grid>
                        <Grid item>
                          <span style={{fontStyle: "italic"}}>
                            {t("components.categoriesTree.goToData", {datasetsCount: cat.datasetIdentifiers.length})}
                          </span>
                        </Grid>
                      </Grid>
                    </Link>
                  }
                />
              )}
              {cat.childrenCategories && cat.childrenCategories.length > 0 &&
              getTreeItems(cat.childrenCategories, [...prevPath, cat.id])}
            </Fragment>
          )}
        </TreeItem>
      );

    const getCategorySchemeTree = ({id, label, categories}) =>
      <TreeView
        defaultCollapseIcon={<ExpandMoreIcon/>}
        defaultExpandIcon={<ChevronRightIcon/>}
        expanded={expanded}
        onNodeToggle={(_, nodeIds) => this.onExpand(nodeIds)}
        key={id}
      >
        {getTreeItems([{id, label, childrenCategories: categories}], [])}
      </TreeView>;

    return (
      <Fragment>
        {catalog.categoryGroups.length > 0 && (
          catalog.categoryGroups.length > 1
            ? (
              <Fragment>
                {catalog.categoryGroups.map(getCategorySchemeTree)}
              </Fragment>
            )
            : (
              <TreeView
                defaultCollapseIcon={<ExpandMoreIcon/>}
                defaultExpandIcon={<ChevronRightIcon/>}
                expanded={expanded}
                onNodeToggle={(_, nodeIds) => this.onExpand(nodeIds)}
              >
                {getTreeItems(catalog.categoryGroups[0].categories, [])}
              </TreeView>
            )
        )}
        {catalog.uncategorizedDatasets && catalog.uncategorizedDatasets.length > 0 && (
          <TreeView
            defaultCollapseIcon={<ExpandMoreIcon/>}
            defaultExpandIcon={<ChevronRightIcon/>}
            expanded={expanded}
            onNodeToggle={(_, nodeIds) => this.onExpand(nodeIds)}
          >
            <TreeItem
              key={"uncategorized"}
              nodeId={"uncategorized"}
              label={
                <Grid spacing={1} container alignItems="center">
                  <Grid item>
                    {(expanded.includes("uncategorized")
                        ? <FolderOpenIcon className={classes.nodeIcon}/>
                        : <FolderIcon className={classes.nodeIcon}/>
                    )}
                  </Grid>
                  <Grid item>
                    {t("commons.catalog.uncategorized")}
                  </Grid>
                </Grid>
              }
              /* TODO: inefficente */
              classes={{
                content: selectedCategoryPath.includes("uncategorized") ? classes.selectedItem : null
              }}
            >
              <TreeItem
                nodeId={"uncategorized-results"}
                label={
                  <Link onClick={() => {
                    onClose();
                    goToDatasets(node.code.toLowerCase(), ["uncategorized"]);
                  }}>
                    <Grid spacing={1} container alignItems="center">
                      <Grid item>
                        <ListIcon className={classes.nodeIcon}/>
                      </Grid>
                      <Grid item>
                        <i>{t("components.categoriesTree.goToData", {datasetsCount: catalog.uncategorizedDatasets.length})}</i>
                      </Grid>
                    </Grid>
                  </Link>
                }
              />
            </TreeItem>
          </TreeView>
        )}
      </Fragment>
    );
  }
}

export default compose(
  withStyles(styles),
  withTranslation()
)(CategoriesTree);