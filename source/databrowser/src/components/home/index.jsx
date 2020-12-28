import React, {Component} from 'react';
import Box from "@material-ui/core/Box";
import Hero from "../hero";
import CardsGrid from "../cards-grid";
import PageSection from "../page-section";
import Button from "@material-ui/core/Button";
import Page from "../page";
import NodeHeader from "../node-header";
import withStyles from "@material-ui/core/styles/withStyles";
import Container from "@material-ui/core/Container";
import {goToHubDashboards, goToNode} from "../../links";
import {compose} from "redux";
import {connect} from "react-redux";
import Divider from "@material-ui/core/Divider";
import {withTranslation} from "react-i18next";
import Footer from "../footer";
import SanitizedHTML from "../sanitized-html";

const styles = theme => ({
  appBar: {
    transition: "opacity 1s",
  },
  toolbarButtons: {
    marginLeft: "auto",
    '& > *': {
      marginRight: theme.spacing(1)
    }
  },
  anchorContainer: {
    position: "relative"
  },
  anchor: {
    position: "absolute",
    top: -theme.mixins.toolbar.minHeight
  },
  fullWidthContainer: {
    backgroundColor: "#f5f5f5",
    width: "100%"
  },
  container: {
    paddingTop: theme.spacing(3),
    width: 1024,
  },
  section: {
    paddingTop: theme.spacing(3)
  },
  footer: {
    paddingTop: theme.spacing(4)
  },
  footerContent: {
    paddingTop: theme.spacing(2)
  },
  nodesContainer: {
    marginTop: theme.spacing(2)
  },
  centerContainer: {
    width: 316,
    textAlign: "center"
  },
  sistanLogo: {
    width: 32,
    height: 32,
    transform: "translateY(3px)"
  },
  leftContainer: {
    width: "calc((100% - 316px)/2)"
  },
  rightContainer: {
    width: "calc((100% - 316px)/2)"
  },
});

const mapStateToProps = state => ({
  hub: state.hub,
  baseURL: state.config.baseURL,
  isA11y: state.app.isA11y
});

class Home extends Component {

  info = React.createRef();
  nodesRef = React.createRef();

  state = {
    isListMode: true,
  };

  scroll(ref) {
    ref.current.scrollIntoView({
      behavior: 'smooth',
      alignToTop: true,
    });
  }

  /*  onListModeToggle = () => {
      this.setState({isListMode: !this.state.isListMode});
    };*/

  render() {

    const {classes, hub, baseURL, isA11y, t} = this.props;
    // const {isListMode} = this.state;

    return (
      <Page id="landing-page">
        <NodeHeader
          noNode
          defaultNodeConfigOpen={this.props.defaultNodeConfigOpen}
          defaultAppConfigOpen={this.props.defaultAppConfigOpen}
          defaultUserConfigOpen={this.props.defaultUserConfigOpen}
          defaultNodesConfigOpen={this.props.defaultNodesConfigOpen}
          hub={hub.hub}
          nodes={hub.nodes}
        />
        <Hero
          title={hub.hub.name}
          slogan={hub.hub.slogan}
          logo={hub.hub.logoURL && <img src={baseURL + hub.hub.logoURL} alt={hub.hub.name}/>}
          background={
            hub.hub.backgroundMediaURL
              ? (hub.hub.backgroundMediaURL.match(/\.(jpeg|jpg|gif|png|JPEG|JPG|GIF|PNG|svg|SVG)$/)
                ? <img src={baseURL + hub.hub.backgroundMediaURL} alt={hub.hub.name}/>
                : (
                  <video autoPlay muted loop>
                    <source src={baseURL + hub.hub.backgroundMediaURL}/>
                  </video>
                )
              ) : <video autoPlay muted loop>
                <source src="./videos/landing-background.mp4"/>
              </video>
          }
        >
          {hub.hub.description && hub.hub.description.length > 0 && (
            <Button size="large" color="secondary" variant="contained"
                    onClick={() => this.scroll(this.info)}>
              {t("scenes.hub.informations")}
            </Button>
          )}
          <Button size="large" color="secondary" variant="contained"
                  onClick={() => this.scroll(this.nodesRef)}>
            {t("scenes.hub.nodes")}
          </Button>
          {!isA11y && hub.hub.dashboards && hub.hub.dashboards.length > 0 && (
            <Button size="large" color="secondary" variant="contained" onClick={goToHubDashboards}>
              {t("scenes.hub.dashboards")}
            </Button>
          )}
        </Hero>
        <div className={classes.fullWidthContainer}>
          <Container className={classes.container}>
            {hub.hub.description && hub.hub.description.length > 0 && (
              <div className={classes.anchorContainer}>
                <div className={classes.anchor} ref={this.info}/>
                <PageSection className={classes.section}
                             sectiontitle={t("scenes.hub.informations")}>
                  <Box textAlign="justify">
                    <SanitizedHTML html={hub.hub.description} allowTarget/>
                  </Box>
                </PageSection>
              </div>
            )}
            <div className={classes.anchorContainer}>
              <div className={classes.anchor} ref={this.nodesRef}/>
              <PageSection className={classes.section} sectiontitle={t("scenes.hub.nodes")}>
                <Box className={classes.nodesContainer}>
                  <CardsGrid
                    list={hub.nodes.sort((a, b) => a.order - b.order).map(({code, name, backgroundMediaURL}) => ({
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
)(Home);