import React, {Fragment, useState} from 'react';
import {withStyles} from "@material-ui/core";
import {useTranslation} from "react-i18next";
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogActions from "@material-ui/core/DialogActions";
import Button from "@material-ui/core/Button";
import Chip from "@material-ui/core/Chip";
import Tooltip from "@material-ui/core/Tooltip";
import CancelIcon from '@material-ui/icons/Cancel';

const styles = theme => ({
  root: {
    whiteSpace: "nowrap"
  },
  sheet: {
    display: "inline-block",
    marginRight: 8
  }
});

function DataViewerFooter(props) {

  const {
    classes,
    labels,
    dataIdx,
    setData
  } = props;

  const {t} = useTranslation();

  const [isConfirmOpen, setConfirmVisibility] = useState(false);

  return (
    <Fragment>

      <div className={classes.root}>
        {labels
          .map((label, idx) => ({key: idx, label: label}))
          .map(sheet => (
            <div key={sheet.key} className={classes.sheet}>
              <Chip
                label={sheet.label}
                color={sheet.key === dataIdx ? "primary" : undefined}
                onClick={() => setData(sheet.key)}
                deleteIcon={
                  <Tooltip title={t("scenes.dataViewer.footer.worksheetDelete")}>
                    <CancelIcon/>
                  </Tooltip>
                }
                onDelete={dataIdx === sheet.key
                  ? undefined
                  : () => setConfirmVisibility(true)
                }
              />
            </div>
          ))}
      </div>

      <Dialog
        open={isConfirmOpen}
        onClose={() => setConfirmVisibility(false)}
      >
        <DialogTitle>
          {t("scenes.dataViewer.footer.dialogs.worksheetDelete.title")}
        </DialogTitle>
        <DialogActions>
          <Button onClick={() => setConfirmVisibility(false)} color="primary">
            {t("commons.confirm.cancel")}
          </Button>
          <Button onClick={() => setConfirmVisibility(false)} color="primary" autoFocus>
            {t("commons.confirm.confirm")}
          </Button>
        </DialogActions>
      </Dialog>

    </Fragment>
  )
}

export default withStyles(styles)(DataViewerFooter)