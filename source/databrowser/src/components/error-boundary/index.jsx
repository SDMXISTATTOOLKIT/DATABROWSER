import React from 'react';
import ErrorOutlineIcon from '@material-ui/icons/ErrorOutline';
import Grid from "@material-ui/core/Grid";
import {withStyles} from "@material-ui/core";
import {goToHome} from "../../links";

const styles = theme => ({
  container: {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    height: "100%",
    position: "absolute",
    width: "100%",
    overflow: "hidden",
    fontSize: 24
  },
  icon: {
    marginBottom: theme.spacing(1),
    fontSize: 38
  },
  goHome: {
    marginTop: theme.spacing(2),
    textDecoration: "underline",
    cursor: "pointer",
    color: theme.palette.secondary.main
  }
});

class ErrorBoundary extends React.Component {

  constructor(props) {
    super(props);
    this.state = {hasError: false};
  }

  componentDidCatch(error, info) {
    this.setState({hasError: true});
  }

  render() {

    const {classes} = this.props;

    if (this.state.hasError) {

      return (
        <div className={classes.container} style={{color: "white"}}>
          <div>
            <Grid container justify="center" className={classes.icon}>
              <Grid item>
                <ErrorOutlineIcon fontSize="inherit"/>
              </Grid>
            </Grid>
            <Grid container justify="center">
              <Grid item>
                Something went wrong
              </Grid>
            </Grid>
            <Grid container justify="center" className={classes.goHome}>
              <Grid item>
                <div onClick={() => {
                  goToHome();
                  window.location.reload();
                }}>
                  Go to home
                </div>
              </Grid>
            </Grid>
          </div>
        </div>
      );

    } else {
      return this.props.children;
    }
  }
}

export default withStyles(styles)(ErrorBoundary);