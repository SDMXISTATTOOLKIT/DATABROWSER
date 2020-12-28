import React, {Fragment, useState} from 'react';
import InputAdornment from "@material-ui/core/InputAdornment";
import Button from "@material-ui/core/Button";
import IconButton from "@material-ui/core/IconButton";
import CancelIcon from "@material-ui/icons/Cancel";
import TextField from "@material-ui/core/TextField";
import {withStyles} from "@material-ui/core";
import PublishIcon from '@material-ui/icons/Publish';
import {v4 as uuidv4} from 'uuid';
import {connect} from "react-redux";
import {getFileUploadUrl} from "../../serverApi/urls";
import {compose} from "redux";
import {addSpinnerMessage, markSpinnerMessage} from "../../state/spinner/spinnerActions";
import {useTranslation} from "react-i18next";

const styles = theme => ({
  startAdornment: {
    marginRight: theme.spacing(2)
  },
  fileInput: {
    display: "none"
  }
});

const mapStateToProps = state => ({
  baseURL: state.config.baseURL,
  user: state.user
});

const mapDispatchToProps = dispatch => ({
  addSpinnerMessage: (uuid, message) => dispatch(addSpinnerMessage(uuid, message)),
  markSpinnerMessage: (uuid, isError) => dispatch(markSpinnerMessage(uuid, isError))
});

const FileInput = ({label, value, onChange, classes, baseURL, error, helperText, required, addSpinnerMessage, markSpinnerMessage, user}) => {

  const [id] = useState(uuidv4());
  const [fileName, setFileName] = useState(value?.substring(value.lastIndexOf('/') + 1) || '');
  const {t} = useTranslation();

  return (
    <Fragment>
      <input
        accept="image/*, video/*"
        className={classes.fileInput}
        id={id}
        type="file"
        onChange={e => {
          const file = e.target.files[0];

          let fileFormData = new FormData();
          fileFormData.append('files', file);

          const uuid = uuidv4();

          addSpinnerMessage(uuid, t("components.fileInput.messages.upload.start"));

          fetch(baseURL + getFileUploadUrl(), {
            method: 'POST',
            body: fileFormData,
            headers: {
              "Authorization": user.token ? `bearer ${user.token}` : undefined
            },
            credentials: 'include'
          })
            .then(res => res.json())
            .then(res => {
              markSpinnerMessage(uuid);
              const filePath = res[0];
              onChange(filePath);
              setFileName(filePath.substring(filePath.lastIndexOf('/') + 1));
            })
            .catch(() => {
              markSpinnerMessage(uuid, true);
              window.error.show(t("components.fileInput.messages.upload.error"));
            });
        }}
      />
      <TextField
        variant="outlined"
        label={label}
        value={fileName}
        InputProps={{
          startAdornment: (
            <InputAdornment className={classes.startAdornment} position="start">
              <label htmlFor={id}>
                <Button startIcon={<PublishIcon/>} variant="outlined" disabled={!!value}
                        title="Upload new file" component="span">
                  {t("components.fileInput.upload")}
                </Button>
              </label>
            </InputAdornment>
          ),
          endAdornment:
            value && (
              <InputAdornment position="end">
                <IconButton
                  title= {t("components.fileInput.reset")}
                  onClick={() => {
                    setFileName('');
                    onChange('');
                  }}
                >
                  <CancelIcon/>
                </IconButton>
              </InputAdornment>
            ),
        }}
        error={error}
        required={required}
        helperText={helperText}
      />
    </Fragment>
  );
};

export default compose(
  withStyles(styles),
  connect(mapStateToProps, mapDispatchToProps)
)(FileInput);