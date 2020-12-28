import React, {Component} from 'react';
import NodeHeader from "../node-header";
import {Link, withStyles} from "@material-ui/core";
import Page from "../page";
import Container from "@material-ui/core/Container";
import Card from "@material-ui/core/Card";
import Grid from "@material-ui/core/Grid";
import Typography from "@material-ui/core/Typography";
import {goToData, goToDatasets, goToNode} from "../../links";
import CardActionArea from "@material-ui/core/CardActionArea";
import IconButton from "@material-ui/core/IconButton";
import ListIcon from '@material-ui/icons/List';
import GridOnIcon from '@material-ui/icons/GridOn';
import Breadcrumbs from "@material-ui/core/Breadcrumbs";
import ResultTree from "./ResultTree";
import {withTranslation} from "react-i18next";
import {compose} from "redux";
import FolderIcon from "@material-ui/icons/Folder";
import DatasetCard from "./DatasetCard";
import {scrollResultsToDatasetByParam} from "./utils";

const styles = theme => ({
  fullWidthContainer: {
    width: "100%",
    paddingTop: 64,
    backgroundColor: "#f5f5f5",
    minHeight: "100%"
  },
  container: {
    width: 1024,
  },
  results: {
    marginTop: theme.spacing(1)
  },
  toolbar: {
    marginTop: theme.spacing(2),
    marginBottom: theme.spacing(1)
  },
  subCat: {
    padding: theme.spacing(1)
  },
  row: {
    marginBottom: theme.spacing(1)
  }
});

class Results extends Component {

  state = {
    expandedIndexes: [],
    configIsListMode: this.props.hub.nodes
      .find(({code}) => code.toLowerCase() === this.props.nodeCode.toLowerCase())
      .catalogNavigationMode === "List",
    isListMode:
      this.props.hub.nodes
        .find(({code}) => code.toLowerCase() === this.props.nodeCode.toLowerCase())
        .catalogNavigationMode === "List"
  };

  componentDidMount() {
    scrollResultsToDatasetByParam();
  }

  static getDerivedStateFromProps(props, state) {

    const configIsListMode = props.hub.nodes
      .find(({code}) => code.toLowerCase() === props.nodeCode.toLowerCase())
      .catalogNavigationMode === "List";

    if (configIsListMode !== state.configIsListMode) {
      return {
        ...state,
        configIsListMode,
        isListMode: configIsListMode
      };
    } else {
      return null;
    }
  }

  expand = index =>
    this.setState({
      expandedIndexes: [...this.state.expandedIndexes, index]
    });

  collapse = index =>
    this.setState({
      expandedIndexes: [...this.state.expandedIndexes.filter(i => i !== index)]
    });

  onListModeToggle = () => {
    this.setState({isListMode: !this.state.isListMode});
  };

  render() {

    const {
      classes,
      query,
      hub,
      catalog,
      node,
      nodeCode,
      isDefault,
      categoryPath,
      datasets,
      subCategories,
      filters,
      categoryGroupsWithCount,
      uncategorizedDatasetsCount,
      t,
      isAccessible,
      onAccessibleDatasetFetch
    } = this.props;

    const {expandedIndexes, isListMode} = this.state;

    const nodeMinimalInfo = hub.nodes.find(({code}) => code.toLowerCase() === nodeCode.toLowerCase());

    let categoryPathLabels = [];
    if (categoryPath) {
      if (categoryPath[0] === "uncategorized") {
        categoryPathLabels = [t("commons.catalog.uncategorized")];
      } else {

        let category;

        if (catalog.categoryGroups.length > 1) {
          category = catalog.categoryGroups.find(({id}) => id === categoryPath[0]);
        } else {
          category = catalog.categoryGroups[0].categories.find(({id}) => id === categoryPath[0]);
        }

        categoryPathLabels.push(category.label);
        for (let i = 1; i < categoryPath.length; i++) {
          category = (category.childrenCategories || category.categories).find(({id}) => id === categoryPath[i]);
          categoryPathLabels.push(category.label);
        }
      }
    }

    return (
      <Page>
        <NodeHeader
          hub={hub.hub}
          query={query}
          nodes={hub.nodes}
          catalog={catalog}
          title={nodeMinimalInfo.name}
          node={node}
          nodeId={nodeMinimalInfo.nodeId}
          isDefault={isDefault}
          selectedCategoryPath={categoryPath}
        />
        <div className={classes.fullWidthContainer}>
          <Container className={classes.container}>
            <Grid container justify="space-between" alignItems="center" className={classes.toolbar}>
              <Grid item>
                {query ?
                  (
                    <Typography variant={"h5"}>
                      {t("scenes.results.searchTitle", {query})}
                    </Typography>
                  )
                  : (
                    <Breadcrumbs>
                      <Link underline="none" onClick={() => goToNode(node.code)} style={{cursor: "pointer"}}>
                        <Typography>
                          {nodeCode.toUpperCase()}
                        </Typography>
                      </Link>
                      {categoryPath.map((id, idx) => {
                        if (idx >= (catalog && catalog.categoryGroups.length === 1 ? 0 : 1)) {
                          return (
                            <Link
                              key={idx}
                              underline="none"
                              onClick={
                                idx >= (
                                  catalog && catalog.categoryGroups.length === 1
                                    ? 0
                                    : 1
                                )
                                  ? () => goToDatasets(node.code, categoryPath.slice(0, idx + 1))
                                  : null
                              }
                              style={idx < categoryPath.length - 1 ? {cursor: "pointer"} : undefined}
                            >
                              <Typography>
                                {id === "uncategorized"
                                  ? t("commons.catalog.uncategorized")
                                  : categoryPathLabels[idx]
                                }
                              </Typography>
                            </Link>
                          );
                        } else {
                          return (
                            <Typography key="idx" color="primary">
                              {id === "uncategorized"
                                ? t("commons.catalog.uncategorized")
                                : categoryPathLabels[idx]
                              }
                            </Typography>
                          );
                        }
                      })}
                    </Breadcrumbs>
                  )}
              </Grid>
              {!isAccessible && (
                <Grid item>
                  <Grid container alignItems="center" justify="flex-end" spacing={1}>
                    <Grid item>
                      {datasets.length > 0 && (
                        <IconButton onClick={this.onListModeToggle}>
                          {isListMode
                            ? <GridOnIcon/>
                            : <ListIcon/>
                          }
                        </IconButton>
                      )}
                    </Grid>
                  </Grid>
                </Grid>
              )}
            </Grid>
            <Grid container spacing={2} className={classes.results}>
              {query && (
                <Grid item style={{width: 256, transform: "translateX(-24px)"}}>
                  <ResultTree
                    query={query} filters={filters} catalog={catalog} node={node}
                    datasets={datasets}
                    categoryGroupsWithCount={categoryGroupsWithCount}
                    uncategorizedDatasetsCount={uncategorizedDatasetsCount}
                  />
                </Grid>
              )}
              <Grid item style={{width: query ? `calc(100% - 256px)` : "100%"}}>
                {subCategories && subCategories.length
                  ? (
                    <Grid container spacing={2} className={classes.row}>
                      {subCategories.map(({id, label, datasetIdentifiers}) =>
                        <Grid item key={id}>
                          <Card className={classes.subCat}>
                            <CardActionArea
                              onClick={() => goToDatasets(node.code.toLowerCase(), [...categoryPath, id])}>
                              <Grid container spacing={1}>
                                <Grid item>
                                  <FolderIcon/>
                                </Grid>
                                <Grid item>
                                  <Typography variant="body1">
                                    {label} {datasetIdentifiers && datasetIdentifiers.length ? `(${datasetIdentifiers.length})` : ''}
                                  </Typography>
                                </Grid>
                              </Grid>
                            </CardActionArea>
                          </Card>
                        </Grid>
                      )}
                    </Grid>
                  )
                  : null}
                {query && datasets.length
                  ? (
                    <Typography variant="body1" className={classes.row}>
                      {datasets.length > 1
                        ? t("scenes.results.datasetsCount.plural", {datasetsCount: datasets.length})
                        : t("scenes.results.datasetsCount.singular", {datasetsCount: datasets.length})
                      }:
                    </Typography>
                  ) : null
                }
                <Grid container spacing={2}>
                  {datasets.length > 0
                    ? datasets.map((dataset, i) =>
                      <DatasetCard
                        key={i}
                        isExpanded={expandedIndexes.includes(i)}
                        onViewDataset={() => isAccessible
                          ? onAccessibleDatasetFetch(dataset)
                          : goToData(node.code, dataset.categoryPath, dataset.identifier, undefined)
                        }
                        dataset={dataset}
                        onExpand={() => this.expand(i)}
                        onCollapse={() => this.collapse(i)}
                        xs={isListMode ? 12 : 4}
                      />
                    )
                    : (
                      (query || !subCategories || !subCategories.length)
                        ? (
                          <Grid container>
                            <Grid item>
                              {t("scenes.results.datasetsCount.none")}
                            </Grid>
                          </Grid>
                        )
                        : null
                    )}
                </Grid>
              </Grid>
            </Grid>
          </Container>
        </div>
      </Page>
    );
  }
}

export default compose(
  withStyles(styles),
  withTranslation()
)(Results);