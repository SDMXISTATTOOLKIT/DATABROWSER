import React, {Fragment, useState} from 'react';
import I18nInputAdornmentSelect from "../i18n-input-adornment-select";
import {connect} from "react-redux";
import HtmlEditor from "../html-editor";
import Grid from "@material-ui/core/Grid";

const Component = ({
                     htmlEditorProps,
                     defaultLanguage,
                     languages
                   }) => {

  const [language, setLanguage] = useState(defaultLanguage);

  return (
    <Fragment>
      <Grid container justify="flex-end" style={{marginBottom: 4}}>
        <Grid item>
          <I18nInputAdornmentSelect value={language} onChange={lang => setLanguage(lang)}/>
        </Grid>
      </Grid>
      <HtmlEditor
        {...htmlEditorProps}
        value={
          htmlEditorProps.value && htmlEditorProps.value[language]
            ? htmlEditorProps.value[language]
            : ""
        }
        onChange={val => {
          const res = {};
          languages.forEach(lang => {
            if (htmlEditorProps.value && htmlEditorProps.value[lang]) {
              res[lang] = htmlEditorProps.value[lang];
            }
          });
          res[language] = val;
          htmlEditorProps.onChange(res);
        }}

      />
    </Fragment>
  );
};

const ConnectedComponent =
  connect(state => ({
    languages: state.app.languages,
    defaultLanguage: state.app.language
  }))(Component);

const I18nHtmlEditor = props =>
  <ConnectedComponent htmlEditorProps={props}/>;

export default I18nHtmlEditor;