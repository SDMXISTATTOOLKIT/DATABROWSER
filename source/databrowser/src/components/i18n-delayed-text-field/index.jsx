import React, {useState} from 'react';
import {TextField} from "@material-ui/core";
import InputAdornment from "@material-ui/core/InputAdornment";
import I18nInputAdornmentSelect from "../i18n-input-adornment-select";
import {connect} from "react-redux";
import _ from "lodash";

const ON_CHANGE_TIMEOUT = 500;

const getInitialInternalValue = (value, languages) => {
  const emptyValue = {};
  languages.forEach(lang => emptyValue[lang] = "");

  return _.merge(emptyValue, value)
};

const Component = ({
                     textFieldProps,
                     defaultLanguage,
                     languages
                   }) => {

  const [language, setLanguage] = useState(defaultLanguage);
  const [internalValue, setInternalValue] = useState(getInitialInternalValue(textFieldProps.value, languages));
  const [timeoutId, setTimeoutId] = useState(null);

  const callOnChange = value => {
    if (textFieldProps.onChange) {
      if (timeoutId) {
        clearTimeout(timeoutId)
      }
      setTimeoutId(setTimeout(
        () => {
          textFieldProps.onChange(value);
        },
        ON_CHANGE_TIMEOUT
      ));
    }
  };

  const setValueForLanguage = value => {
    let newInternalValue = _.cloneDeep(internalValue);
    newInternalValue[language] = value;

    setInternalValue(newInternalValue);
    callOnChange(newInternalValue);
  };

  return (
    <TextField
      {...textFieldProps}
      value={(internalValue && internalValue[language])
        ? internalValue[language]
        : ""
      }
      onChange={({target}) => setValueForLanguage(target.value)}
      InputProps={{
        startAdornment: (
          <InputAdornment position="start">
            <I18nInputAdornmentSelect value={language} onChange={lang => setLanguage(lang)}/>
          </InputAdornment>
        )
      }}
    >
      {textFieldProps.children}
    </TextField>
  );
};

const ConnectedComponent =
  connect(state => ({
    languages: state.app.languages,
    defaultLanguage: state.app.language
  }))(Component);

const I18nDelayedTextField = props =>
  <ConnectedComponent textFieldProps={props}/>;

export default I18nDelayedTextField;