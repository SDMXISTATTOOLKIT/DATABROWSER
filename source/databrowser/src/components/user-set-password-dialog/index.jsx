import React, {useRef} from 'react';
import {useTranslation} from "react-i18next";
import SettingsDialog from "../settings-dialog";
import UserSetPasswordForm from "./user-set-password-form";
import {hideUserSetPasswordForm} from "../../state/user/userActions";
import {connect} from "react-redux";

const mapStateToProps = state => ({
  user: state.user
});

const mapDispatchToProps = dispatch => ({
  onClose: () => dispatch(hideUserSetPasswordForm())
});

const UserSetPasswordDialog = ({user, onClose}) => {

  const {t} = useTranslation();

  const setPasswordFormRef = useRef();

  return (
    <SettingsDialog
      title={t("components.userSetPasswordDialog.title")}
      maxWidth={"xs"}
      open={user.isSetPasswordDialogOpen || false}
      onClose={() => {
        if (setPasswordFormRef.current) {
          setPasswordFormRef.current.cancel(() => {
            onClose();
          });
        } else {
          onClose();
        }
      }}
      onSubmit={() => {
        if (setPasswordFormRef.current) {
          setPasswordFormRef.current.submit(() => {
          });
        }
      }}
      hasSubmit
      noMinHeight
    >
      <UserSetPasswordForm ref={setPasswordFormRef} token={user.setPasswordToken}/>
    </SettingsDialog>
  );
};

export default connect(mapStateToProps, mapDispatchToProps)(UserSetPasswordDialog);
