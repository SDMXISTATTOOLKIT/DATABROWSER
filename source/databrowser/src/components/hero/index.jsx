import React, {Fragment} from 'react'
import Box from "@material-ui/core/Box";
import Typography from "@material-ui/core/Typography";
import {withStyles} from "@material-ui/core";
import {Helmet} from "react-helmet";

const styles = theme => ({
  root: {
    display: "flex",
    flexDirection: "column",
    justifyContent: "center",
    alignItems: "center",
    width: "100%",
    height: "100%"
  },
  background: {
    "&, & > *": {
      position: "fixed",
      width: "100%",
      height: "100%",
      objectFit: "cover",
      zIndex: -9999
    }
  },
  textContainer: {
    color: "white",
    textAlign: "center",
    padding: 24,
    background: "rgba(0,0,0,0.5)"
  },
  lightTextContainer: {
    color: theme.palette.primary.main,
    textAlign: "center",
    width: "80%"
  },
  buttonsContainer: {
    marginTop: theme.spacing(3),
    '& > *': {
      margin: '8px !important'
    }
  },
  logo: {
    position: "absolute",
    top: "80%",
    width: "80%",
    textAlign: "center",
    "& > *": {
      maxWidth: 340,
      maxHeight: 64
    }
  }
});

const Hero = ({classes, title, slogan, logo, background, children}) =>
  <Fragment>
    <Helmet>
      <style type="text/css">
        {`
          html, body, #root {
            height: 100%
          }
        `}
      </style>
    </Helmet>
    <div className={classes.root}>
      <div className={classes.background}>
        {background}
      </div>
      <div className={classes.textContainer}>
        <Typography variant="h2">
          {title}
        </Typography>
        {slogan?.length > 0 && (
          <Typography variant="h3"
                      style={{fontSize: 24, paddingTop: 16}}>
            {slogan}
          </Typography>
        )}
      </div>
      <Box className={classes.buttonsContainer}>
        {children}
      </Box>
      <Box className={classes.logo}>
        {logo}
      </Box>
    </div>
  </Fragment>;

export default withStyles(styles)(Hero);