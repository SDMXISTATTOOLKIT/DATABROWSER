import React, {Component} from 'react';
import Zoom from "@material-ui/core/Zoom";
import Fab from "@material-ui/core/Fab";
import {ArrowUpward} from "@material-ui/icons";
import {withStyles} from "@material-ui/core";
import Grid from "@material-ui/core/Grid";
import IconButton from "@material-ui/core/IconButton";
import ClearIcon from "@material-ui/icons/Clear";

const styles = theme => ({
  root: {
    width: "100%",
    height: "100%",
  },
  fab: {
    position: 'fixed',
    right: 8,
    bottom: 55,
    height: '50px',
    width: '50px',
    zIndex: theme.zIndex.appBar
  },
  childrenContainer: {
    width: "100%",
    height: "100%",
    '& > *:last-child': {
      paddingBottom: theme.spacing(10)
    }
  },
  oldBrowserWarning: {
    position: "fixed",
    bottom: 0,
    left: 0,
    zIndex: 1999,
    height: 48,
    width: "100%",
    background: "rgba(0, 0, 0, 0.7)",
    fontSize: 16,
    lineHeight: "34px",
    color: "white",
    padding: 8
  }
});

class Page extends Component {

  state = {
    transparent: true,
    isOldBrowserWarningVisible: false
  };

  scroll(ref) {
    ref.current.scrollIntoView({
      behavior: 'smooth',
      alignToTop: true,
    });
  }

  scrollTop() {
    window.scroll({top: 0, left: 0, behavior: 'smooth'});
  }

  handleScroll = () => {
    const {transparent} = this.state;
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

    if (scrollTop > 50 && transparent) {
      this.setState({transparent: false});
    }

    if (scrollTop <= 50 && !transparent) {
      this.setState({transparent: true});
    }
  };

  closeWarning = () => {
    this.setState({isOldBrowserWarningVisible: false});
  }

  componentDidMount() {
    // Internet Explorer 6-11
    const isIE = !!document.documentMode;

    // Edge 20+
    const isEdge = !isIE && !!window.StyleMedia;

    this.setState({isOldBrowserWarningVisible: (isIE || isEdge)});

    window.scroll({top: 0, left: 0});

    window.addEventListener('scroll', this.handleScroll);
  }

  componentWillUnmount() {
    window.removeEventListener('scroll', this.handleScroll);
  }

  render() {

    const {classes, children, id} = this.props;
    const {transparent, isOldBrowserWarningVisible} = this.state;

    return (
      <div id={id} className={classes.root}>
        {isOldBrowserWarningVisible && (
          <div className={classes.oldBrowserWarning}>
            <Grid container justify="space-between">
              <Grid item>
                This browser in not completely supported, please consider to install the new version
                of <a href="https://www.microsoft.com/edge" target="_blank" rel="noopener noreferrer"
                      style={{color: "white"}}>Microsoft Edge</a>
              </Grid>
              <Grid item>
                <IconButton onClick={this.closeWarning} style={{color: "white", padding: 7}}>
                  <ClearIcon fontSize="small"/>
                </IconButton>
              </Grid>
            </Grid>
          </div>
        )}
        <Zoom in={!transparent}>
          <Fab className={classes.fab} color="secondary" onClick={this.scrollTop}>
            <ArrowUpward/>
          </Fab>
        </Zoom>
        <div className={classes.childrenContainer}>
          {children}
        </div>
      </div>
    );
  }
}

export default withStyles(styles)(Page);