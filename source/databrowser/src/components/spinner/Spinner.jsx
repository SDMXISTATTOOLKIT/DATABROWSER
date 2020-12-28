import React, {useEffect, useState} from 'react';
import Box from "@material-ui/core/Box";
import {withStyles, withTheme} from "@material-ui/core";
import {compose} from "redux";
import Typography from "@material-ui/core/Typography";
import {connect} from "react-redux";
import Grid from "@material-ui/core/Grid";
import "./style.css";
import CheckCircleIcon from '@material-ui/icons/CheckCircle';
import LinearProgress from "@material-ui/core/LinearProgress";
import {flushSpinner} from "../../state/spinner/spinnerActions";
import CancelIcon from '@material-ui/icons/Cancel';

const styles = theme => ({
  container: {
    width: "100%",
    height: "100%"
  },
  overlay: {
    position: "absolute",
    width: "100%",
    height: "100%",
    zIndex: 2000
  },
  progressContainer: {
    position: "absolute",
    width: "100%",
    height: "100%",
    display: "flex",
    alignItems: "center",
    justifyContent: "center"
  },
  progress: {
    width: 320,
    maxWidth: "50%"
  },
  messagesContainer: {
    position: "relative",
    width: "100%",
    height: "100%",
    textAlign: "center"
  },
  messages: {
    display: "inline-block",
    marginTop: "calc(50vh + 48px)"
  },
  childrenContainer: {
    position: "absolute",
    top: 0,
    width: "100%",
    height: "100%"
  },
  messageRow: {
    height: 32
  }
});

const mapStateToProps = state => ({
  messages: state.spinner.messages
});

const mapDispatchToProps = dispatch => ({
  flushSpinner: () => dispatch(flushSpinner())
});

const Spinner = ({classes, theme, children, messages, flushSpinner}) => {

  const [isHandlerOn, setIsHandlerOn] = useState(false);

  useEffect(() => {
    if (!isHandlerOn) {
      window.addEventListener('popstate', () => {
        flushSpinner();
      });
      setIsHandlerOn(true);
    }
  }, [flushSpinner, isHandlerOn]);

  return (
    <Box className={classes.container}>
      <Box
        id="spinner"
        className={classes.overlay + " " + (messages.length === 0 ? "invisible" : "")}
        style={{
          background: "rgba(0, 0, 0, 0.7)",
        }}
      >
        <Box className={classes.progressContainer}>
          <LinearProgress className={classes.progress} color="secondary"/>
        </Box>
        <Box className={classes.messagesContainer}>
          <Box className={classes.messages}>
            {messages.map(({uid, message, removed, isError}) =>
              <Grid container key={uid}
                    justify={messages.filter(({removed}) => removed).length > 0 ? "space-between" : "center"}
                    className={classes.messageRow}
                    spacing={2}>
                <Grid item>
                  <Typography color="secondary">
                    {message} {!removed && "..."}
                  </Typography>
                </Grid>
                {removed && (
                  <Grid item
                        style={{color: isError ? theme.palette.error.main : theme.palette.success.main}}>
                    {isError ? <CancelIcon fontSize="small"/> : <CheckCircleIcon fontSize="small"/>}
                  </Grid>
                )}
              </Grid>
            )}
          </Box>
        </Box>
      </Box>
      <Box className={classes.childrenContainer}
           style={{overflow: messages.length > 0 ? "hidden" : "initial"}}>
        {children}
      </Box>
    </Box>
  )
    ;
};

export default compose(withStyles(styles), withTheme, connect(mapStateToProps, mapDispatchToProps))(Spinner);