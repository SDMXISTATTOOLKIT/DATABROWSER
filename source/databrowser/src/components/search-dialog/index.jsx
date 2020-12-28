import React, {useState} from 'react';
import Dialog from "@material-ui/core/Dialog";
import {fade, withStyles} from "@material-ui/core";
import {v4 as uuidv4} from 'uuid';
import IconButton from '@material-ui/core/IconButton';

import SearchIcon from "@material-ui/icons/Search";
import SearchInput from "../search-input";

import {useTranslation} from "react-i18next";

const $ = window.jQuery;

const styles = theme => ({
  button: {
    borderRadius: theme.shape.borderRadius,
    backgroundColor: fade(theme.palette.common.white, 0.15),
    '&:hover': {
      backgroundColor: fade(theme.palette.common.white, 0.25),
    },
    position: "relative",
    height: 35,
  },
  icon: {
    width: theme.spacing(6),
    height: 35,
    position: 'absolute',
    pointerEvents: 'none',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  inputRoot: {
    color: 'inherit',
    position: 'absolute',
    width: `calc(100% - ${theme.spacing(6)})`,
    left: theme.spacing(6)
  },
  inputInput: {
    padding: theme.spacing(1, 1, 1, 0),
  }
});

const SearchDialog = ({classes, dialogTop, modalWidth, query, onSubmit}) => {

  const [uuid] = useState(uuidv4());
  const [isOpen, setIsOpen] = useState(false);

  const {t} = useTranslation();

  return (
    <div>
      <IconButton
        color="inherit"
        onClick={() => setIsOpen(true)}
        aria-label={t("ariaLabels.header.search")}
      >
        <SearchIcon/>
      </IconButton>
      <Dialog
        open={isOpen}
        onClose={() => setIsOpen(false)}
        onRendered={() => $(`#search-dialog__dialog__${uuid} input`).focus()}
        PaperProps={{
          style: {
            position: "absolute",
            top: dialogTop,
            height: 40
          }
        }}
      >
        <div id={`search-dialog__dialog__${uuid}`}>
          <SearchInput
            query={query}
            onSubmit={value => {
              setIsOpen(false);
              onSubmit(value);
            }}
          />
        </div>
      </Dialog>
    </div>
  );
};

export default withStyles(styles)(SearchDialog);