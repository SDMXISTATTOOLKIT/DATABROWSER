import React from 'react';
import InputAdornment from "@material-ui/core/InputAdornment";
import SearchIcon from "@material-ui/icons/Search";
import IconButton from "@material-ui/core/IconButton";
import ClearIcon from "@material-ui/icons/Clear";
import TextField from "@material-ui/core/TextField";
import {withTranslation} from "react-i18next";

const AUTO_SEARCH_INPUT_TIMEOUT = 500;

class AutoSearchInput extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      tempValue: props.value || "",
      value: props.value || "",
      timeout: null
    };
    this.onChange = this.onChange.bind(this);
    this.onClear = this.onClear.bind(this);
  }

  static getDerivedStateFromProps(nextProps, state) {

    if (nextProps.value !== undefined && (nextProps.value !== state.value)) {
      if (state.timeout) {
        clearTimeout(state.timeout)
      }
      return ({
        tempValue: null,
        value: nextProps.value
      });
    } else {
      return null;
    }

  }

  onClear() {
    if (this.state.timeout) {
      clearTimeout(this.state.timeout)
    }
    this.setState({
      tempValue: null,
      value: "",
      timeout: null
    });
    this.props.onSearch("");
  }

  onChange(value) {
    if (this.state.timeout) {
      clearTimeout(this.state.timeout)
    }
    this.setState({
      tempValue: value,
      timeout: setTimeout(
        () => {
          this.setState({
            tempValue: null,
            value
          });
          this.props.onSearch(value);
        },
        AUTO_SEARCH_INPUT_TIMEOUT
      ),
    });
  }

  componentWillUnmount() {
    if (this.state.timeout) {
      clearTimeout(this.state.timeout)
    }
  }

  render() {

    const {
      t,
      placeholder
    } = this.props;

    const {
      tempValue,
      value
    } = this.state;

    return (
      <TextField
        value={tempValue !== null ? tempValue : value}
        placeholder={placeholder || (t("components.autoSearchInput.placeholder") + "...")}
        onChange={({target}) => this.onChange(target.value)}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <SearchIcon/>
            </InputAdornment>
          ),
          endAdornment: (
            <InputAdornment position="end">
              <IconButton
                disabled={!value || value.length === 0}
                onClick={this.onClear}
                style={{padding: 6}}
              >
                <ClearIcon fontSize="small"/>
              </IconButton>
            </InputAdornment>
          )
        }}
        style={{width: "100%"}}
      />
    );
  }
}


export default withTranslation()(AutoSearchInput);
