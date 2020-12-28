import React, {useRef} from 'react';
import {withStyles} from "@material-ui/core";

import SearchIcon from "@material-ui/icons/Search";
import TextField from "@material-ui/core/TextField";
import InputAdornment from "@material-ui/core/InputAdornment";
import {useTranslation} from "react-i18next";


const styles = theme => ({
  button: {
    borderRadius: theme.shape.borderRadius,
    backgroundColor: "#f5f5f5",
    borderColor: theme.palette.primary.main,
    borderWidth: 1,
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

const SearchInput = ({classes, query = "", onSubmit}) => {

  const input = useRef(null);
  const {t} = useTranslation();

  return (
    <div className={classes.button} style={{width: 320}}>
      <label htmlFor="search-input" style={{display: "none"}}>
        {t("components.searchInput.label")}
      </label>
      <TextField
        id="search-input"
        fullWidth
        size="small"
        variant="outlined"
        placeholder={t("components.searchInput.placeholder")}
        defaultValue={query}
        InputProps={{
          startAdornment:
            <InputAdornment position="start">
              <SearchIcon/>
            </InputAdornment>,
          inputRef: input,
        }}
        onKeyDown={
          e => {
            if (e.key === "Enter" && input.current.value && input.current.value.length > 0) {
              e.preventDefault();
              onSubmit(input.current.value);
            }
          }
        }
      />
    </div>
  );
};

export default withStyles(styles)(SearchInput);