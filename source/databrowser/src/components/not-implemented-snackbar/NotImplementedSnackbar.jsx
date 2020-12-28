import React, {useEffect, useState} from 'react';
import {withTranslation} from "react-i18next";
import Snackbar from "@material-ui/core/Snackbar";
import Alert from "@material-ui/lab/Alert";

const NotImplementedSnackbar = ({t}) => {

  const [isOpen, setIsOpen] = useState(false);

  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    if (!isInitialized) {
      window.notImplemented = {
        show: () => {
          setIsOpen(true);
        }
      };
      setIsInitialized(true);
    }
  }, [isInitialized]);

  return (
    <Snackbar
      anchorOrigin={{vertical: 'top', horizontal: 'center'}}
      open={isOpen}
      autoHideDuration={5000}
      onClose={() => setIsOpen(false)}
      ClickAwayListenerProps={{
        onClickAway: () => {
        }
      }}
    >
      <Alert severity="warning">{t("components.notImplementedSnackbar.label")}</Alert>
    </Snackbar>
  );
};

export default withTranslation()(NotImplementedSnackbar);