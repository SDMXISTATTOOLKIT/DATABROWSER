import React from 'react';
import Dialog from "@material-ui/core/Dialog";
import DialogTitle from "@material-ui/core/DialogTitle";
import DialogContent from "@material-ui/core/DialogContent";
import {DialogActions} from "@material-ui/core";
import Button from "@material-ui/core/Button";
import {connect} from "react-redux";
import {closeDeleteConflictModal} from "../../state/delete-conflict-modal/deleteConflictModalActions";
import {useTranslation} from "react-i18next";

const getObjectTranslation = t => ({
  dashboard: t("components.deleteConflictModal.objectType.dashboard"),
  view: t("components.deleteConflictModal.objectType.view"),
  node: t("components.deleteConflictModal.objectType.node"),
});

const DeleteConflictModal = ({
                               response,
                               onClose,
                               onForce
                             }) => {

  const {t} = useTranslation();

  return (
    <Dialog
      open={!!response}
      disableEscapeKeyDown
      disableBackdropClick
      maxWidth="xs"
      fullWidth
      onClose={onClose}
    >
      <DialogTitle>
        {t("components.deleteConflictModal.title")}
      </DialogTitle>
      {response?.usedBy?.length > 0 && (
        <DialogContent>
          {onForce
            ? <p>{t("components.deleteConflictModal.contentTitleWithForce")}</p>
            : <p>{t("components.deleteConflictModal.contentTitle")}</p>
          }
          <div>
            {response.usedBy.map(({type, title, id}, index) =>
              <ul key={index}>{getObjectTranslation(t)[type]} {id} - {title}</ul>
            )}
          </div>
        </DialogContent>
      )}
      <DialogActions>
        <Button onClick={onClose}>
          {t("commons.confirm.close")}
        </Button>
        {onForce && (
          <Button onClick={onForce}>
            {t("components.deleteConflictModal.force")}
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
};

export default connect(
  state => ({
    response: state.deleteConflictModal.response
  }),
  dispatch => ({
    onClose: () => dispatch(closeDeleteConflictModal())
  })
)(DeleteConflictModal);