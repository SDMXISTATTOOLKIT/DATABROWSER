import React, {Component} from 'react';
import Hero from "../hero";
import CardsGrid from "../cards-grid";
import Container from "@material-ui/core/Container";
import PageSection from "../page-section";
import {withStyles} from "@material-ui/core";
import Button from "@material-ui/core/Button";
import NodeHeader from "../node-header";
import Page from "../page";
import {goToDatasets, goToNode, goToNodeDashboards} from "../../links";
import CategoriesLists from "../../components/categories-lists";
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import Box from "@material-ui/core/Box";
import {connect} from "react-redux";
import {compose} from "redux";
import {withTranslation} from "react-i18next";
import SanitizedHTML from "../sanitized-html";
import Divider from "@material-ui/core/Divider";
import Footer from "../footer";

const styles = theme => ({
  anchor: {
    position: "absolute",
    top: -theme.mixins.toolbar.minHeight
  },
  anchorContainer: {
    position: "relative"
  },
  fullWidthContainer: {
    backgroundColor: "#f5f5f5",
    width: "100%",
    minHeight: "100%"
  },
  container: {
    paddingTop: theme.spacing(3),
    width: 1024,
  },
  categoriesContainer: {
    paddingTop: theme.spacing(2)
  },
  section: {
    paddingTop: theme.spacing(3)
  },
  categorySchemesTabs: {
    marginBottom: theme.spacing(2)
  },
  nodesContainer: {
    marginTop: theme.spacing(2)
  },
  footer: {
    paddingTop: theme.spacing(4)
  },
  footerContent: {
    paddingTop: theme.spacing(2)
  },
});

const mapStateToProps = state => ({
  baseURL: state.config.baseURL,
  isA11y: state.app.isA11y
});

class Node extends Component {

  state = {
    configIsListMode: this.props.hub.nodes
      .find(({code}) => code.toLowerCase() === this.props.nodeCode.toLowerCase())
      .showCategoryLevels === 5,
    isListMode: this.props.hub.nodes
      .find(({code}) => code.toLowerCase() === this.props.nodeCode.toLowerCase())
      .showCategoryLevels === 5,
    categorySchemeTab: 0
  };

  info = React.createRef();
  explore = React.createRef();
  nodes = React.createRef();

  scroll(ref) {
    ref.current.scrollIntoView({
      behavior: 'smooth',
      alignToTop: true
    });
  }

  onListModeToggle = () => {
    this.setState({isListMode: !this.state.isListMode});
  };

  setCategorySchemeTab = tab => {
    this.setState({categorySchemeTab: tab});
  };

  static getDerivedStateFromProps(props, state) {

    const configIsListMode = props.hub.nodes
      .find(({code}) => code.toLowerCase() === props.nodeCode.toLowerCase())
      .showCategoryLevels === 5;

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

  render() {

    const {
      classes,
      defaultTreeOpen,
      defaultNodeConfigOpen,
      defaultAppConfigOpen,
      defaultUserConfigOpen,
      defaultNodesConfigOpen,
      node,
      nodeCode,
      hub,
      catalog,
      baseURL,
      isDefault,
      isA11y,
      t
    } = this.props;

    const {isListMode, categorySchemeTab} = this.state;

    const nodeMinimalInfo = hub.nodes.find(({code}) => code.toLowerCase() === nodeCode.toLowerCase());

    const uncategorizedCategory = {
      id: "uncategorized",
      label: t("commons.catalog.uncategorized"),
      childrenCategories: [],
      datasetIdentifiers: catalog?.uncategorizedDatasets.map(({identifier}) => identifier)
    };

    const otherNodes = hub.nodes.filter(n => n.code.toLowerCase() !== nodeCode.toLowerCase());

    const categoriesForExplore =
      catalog
        ? (
          catalog.categoryGroups.length > 1
            ? (
              categorySchemeTab === "uncategorized"
                ? [uncategorizedCategory]
                : catalog.categoryGroups[categorySchemeTab].categories.map(c => ({
                  ...c,
                  image: c.image ? baseURL + c.image : undefined
                }))
            )
            : (
              catalog.uncategorizedDatasets.length > 0
                ? catalog.categoryGroups[0]?.categories.map(c => ({
                ...c,
                image: c.image ? baseURL + c.image : undefined
              })).concat([uncategorizedCategory]) || [uncategorizedCategory]
                : catalog.categoryGroups[0]?.categories.map(c => ({
                ...c,
                image: c.image ? baseURL + c.image : undefined
              })) || []
            ))
        : [];

    return (
      <Page id={isDefault ? "landing-page" : null}>
        <NodeHeader
          hub={hub.hub}
          nodes={hub.nodes}
          catalog={catalog}
          defaultTreeOpen={defaultTreeOpen}
          title={nodeMinimalInfo.name}
          node={node}
          defaultNodeConfigOpen={defaultNodeConfigOpen}
          defaultAppConfigOpen={defaultAppConfigOpen}
          defaultUserConfigOpen={defaultUserConfigOpen}
          defaultNodesConfigOpen={defaultNodesConfigOpen}
          nodeId={nodeMinimalInfo.nodeId}
          isDefault={isDefault}
        />
        <Hero
          title={nodeMinimalInfo.name}
          slogan={node && node.slogan}
          logo={node && nodeMinimalInfo.logoURL &&
          <img src={baseURL + nodeMinimalInfo.logoURL} alt={nodeMinimalInfo.name}/>}
          background={
            nodeMinimalInfo.backgroundMediaURL
              ? (
                nodeMinimalInfo.backgroundMediaURL.match(/\.(jpeg|jpg|gif|png|JPEG|JPG|GIF|PNG|svg|SVG)$/)
                  ?
                  <img src={baseURL + nodeMinimalInfo.backgroundMediaURL} alt={nodeMinimalInfo.name}/>
                  : (
                    <video autoPlay muted loop>
                      <source src={baseURL + nodeMinimalInfo.backgroundMediaURL}/>
                    </video>
                  )
              )
              : <img src="./images/istat/istat-background.jpg" alt={nodeMinimalInfo.name}/>
          }
        >
          {node && node.description && node.description.length > 0 && (
            <Button size="large" color="secondary" variant="contained"
                    onClick={() => this.scroll(this.info)}>
              {t("scenes.node.informations")}
            </Button>
          )}
          {catalog && categoriesForExplore.length > 0 && (
            <Button id="explore-btn" size="large" color="secondary" variant="contained"
                    onClick={() => this.scroll(this.explore)}>
              {t("scenes.node.explore")}
            </Button>
          )}
          {!isA11y && nodeMinimalInfo.dashboards && nodeMinimalInfo.dashboards.length > 0 && (
            <Button size="large" color="secondary" variant="contained"
                    onClick={() => goToNodeDashboards(nodeCode)}>
              {t("scenes.node.dashboards")}
            </Button>
          )}
          {isDefault && otherNodes.length > 0 && (
            <Button id="other-nodes-btn" size="large" color="secondary" variant="contained"
                    onClick={() => this.scroll(this.nodes)}>
              {t("scenes.node.otherNodes")}
            </Button>
          )}
        </Hero>
        <div className={classes.fullWidthContainer}>
          <Container className={classes.container}>
            {node && node.description && node.description.length > 0 && (
              <div className={classes.anchorContainer}>
                <div className={classes.anchor} ref={this.info}/>
                <PageSection className={classes.section}
                             sectiontitle={t("scenes.node.informations")}>
                  <Box textAlign="justify">
                    <SanitizedHTML html={node.description} allowTarget/>
                  </Box>
                </PageSection>
              </div>
            )}
            {catalog && categoriesForExplore.length > 0 && (
              <div id="explore-section" className={classes.anchorContainer}>
                <div className={classes.anchor} ref={this.explore}/>
                <PageSection className={classes.section} sectiontitle={t("scenes.node.explore")}>
                  <div className={classes.categoriesContainer}>
                    {catalog.categoryGroups.length > 1 && (
                      <Tabs
                        value={categorySchemeTab}
                        onChange={(_, tab) => this.setCategorySchemeTab(tab)}
                        className={classes.categorySchemesTabs}
                      >
                        {catalog.categoryGroups.map(({label}, index) =>
                          <Tab value={index} label={label} key={index}/>
                        )}
                        {catalog.uncategorizedDatasets?.length > 0 && (
                          <Tab value="uncategorized" label={t("commons.catalog.uncategorized")}
                               key="uncategorized"/>
                        )}
                      </Tabs>
                    )}
                    {isListMode
                      ? (
                        <CategoriesLists
                          categories={categoriesForExplore}
                          onClick={
                            categoryPath =>
                              goToDatasets(
                                node.code,
                                categoryPath[0] === "uncategorized"
                                  ? ["uncategorized"]
                                  : catalog.categoryGroups.length > 1 ? [catalog.categoryGroups[categorySchemeTab].id, ...categoryPath] : categoryPath
                              )
                          }
                        />
                      )
                      : (
                        <CardsGrid
                          squareImage
                          list={categoriesForExplore}
                          onClick={
                            category =>
                              goToDatasets(
                                node.code,
                                category.id === "uncategorized"
                                  ? ["uncategorized"]
                                  : catalog.categoryGroups.length > 1 ? [catalog.categoryGroups[categorySchemeTab].id, category.id] : [category.id]
                              )
                          }
                        />
                      )
                    }
                  </div>
                </PageSection>
              </div>
            )}
            {isDefault && otherNodes.length > 0 && (
              <div id="other-nodes-section" className={classes.anchorContainer}>
                <div className={classes.anchor} ref={this.nodes}/>
                <PageSection className={classes.section} sectiontitle={t("scenes.node.otherNodes")}>
                  <Box className={classes.nodesContainer}>
                    <CardsGrid
                      list={otherNodes.sort((a, b) => a.order - b.order).map(({code, name, backgroundMediaURL}) => ({
                        code,
                        id: code,
                        label: name,
                        image: backgroundMediaURL ? baseURL + backgroundMediaURL : "./images/istat/istat-background.jpg"
                      }))}
                      onClick={node => goToNode(node.code.toLowerCase())}
                    />
                  </Box>
                </PageSection>
              </div>
            )}
            <div className={classes.footer}>
              <Divider/>
              <div className={classes.footerContent}>
                <Footer/>
              </div>
            </div>
          </Container>
        </div>
      </Page>
    );
  }
}

export default compose(
  withStyles(styles),
  withTranslation(),
  connect(mapStateToProps)
)(Node);