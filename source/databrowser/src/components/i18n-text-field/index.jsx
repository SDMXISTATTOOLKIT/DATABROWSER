import React, {useState} from 'react';
import {TextField} from "@material-ui/core";
import InputAdornment from "@material-ui/core/InputAdornment";
import I18nInputAdornmentSelect from "../i18n-input-adornment-select";
import {connect} from "react-redux";

const Component = ({
                     textFieldProps,
                     defaultLanguage,
                     languages,
                     children
                   }) => {

  const [language, setLanguage] = useState(defaultLanguage);

  return (
    <TextField
      {...textFieldProps}
      value={
        textFieldProps.value && textFieldProps.value[language]
          ? textFieldProps.value[language]
          : ""
      }
      onChange={({target}) => {
        const res = {};
        languages.forEach(lang => {
          if (textFieldProps.value[lang]) {
            res[lang] = textFieldProps.value[lang];
          }
        });
        res[language] = target.value;
        textFieldProps.onChange(res);
      }}
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

const I18nTextField = props =>
  <ConnectedComponent textFieldProps={props}/>;

export default I18nTextField;