import Grid from "@material-ui/core/Grid";
import React from "react";
import {withStyles} from "@material-ui/core";

const styles = theme => ({
  image: {
    height: 20,
    margin: theme.spacing(1)
  }
});

const Footer = ({classes, textOnly}) =>
  <Grid container type="flex" justify="center" alignItems="center">
    <Grid item>
      {textOnly
        ? <span>ISTAT&nbsp;</span>
        : (
          <img src="images/istat/istat-logo-gray.png" className={classes.image} alt="istat logo"/>
        )}
    </Grid>
    <Grid item>
      Data Browser v1.1.1
    </Grid>
  </Grid>;

export default withStyles(styles)(Footer);