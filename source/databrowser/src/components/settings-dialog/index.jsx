import React, {Fragment} from 'react';
import Dialog from "@material-ui/core/Dialog";
import {DialogActions} from "@material-ui/core";
import Button from "@material-ui/core/Button";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import {useTranslation} from "react-i18next";

const SettingsDialog = ({title, children, onSubmit, onClose, open, maxWidth, hasSubmit, noMinHeight, customSubmitLabel}) => {

  const {t} = useTranslation();

  return (
    <Dialog
      open={open}
      disableEscapeKeyDown
      disableBackdropClick
      disableEnforceFocus
      maxWidth={maxWidth || "md"}
      fullWidth
      onClose={onClose}
    >
      <DialogTitle>
        {title}
      </DialogTitle>
      <DialogContent
        style={{
          minHeight: noMinHeight ? undefined : 456,
          paddingBottom: noMinHeight ? 16 : undefined
        }}
      >
        {children}
      </DialogContent>
      <DialogActions>
        {hasSubmit
          ? (
            <Fragment>
              <Button onClick={onClose}>
                {t("commons.confirm.cancel")}
              </Button>
              <Button
                onClick={onSubmit}
              >
                {customSubmitLabel || t("commons.confirm.submit")}
              </Button>
            </Fragment>
          )
          : (
            <Button onClick={onClose}>
              {t("commons.confirm.close")}
            </Button>
          )}
      </DialogActions>
    </Dialog>
  );
};

export default SettingsDialog;
