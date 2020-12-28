import React, {useEffect, useState} from 'react';
import Snackbar from "@material-ui/core/Snackbar";
import Alert from "@material-ui/lab/Alert";
import SanitizedHTML from "../sanitized-html";


const ErrorSnackbar = () => {

  const [isInitialized, setIsInitialized] = useState(false);
  const [isVisible, setIsVisible] = useState(false);
  const [message, setMessage] = useState(null);
  const [isError, setIsError] = useState(null);

  useEffect(() => {
    if (!isInitialized) {
      window.error = {
        show: (message, isError) => {
          setIsVisible(true);
          setMessage(message);
          setIsError(isError !== undefined ? isError : true);
        }
      };
      setIsInitialized(true);
    }
  }, [isInitialized]);

  return (
    <Snackbar
      open={isVisible}
      autoHideDuration={5000}
      onClose={() => {
        setIsVisible(false);
      }}
      onExited={() => {
        setMessage(null);
        setIsError(null);
      }}
      ClickAwayListenerProps={{
        onClickAway: () => {
        }
      }}
      anchorOrigin={{vertical: 'top', horizontal: 'center'}}
    >
      <Alert severity={isError ? "error" : "success"}>
        <SanitizedHTML component="span" html={message}/>
      </Alert>
    </Snackbar>
  );
};

export default ErrorSnackbar;