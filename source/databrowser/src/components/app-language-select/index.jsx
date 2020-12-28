import React from 'react';
import I18nInputAdornmentSelect from "../i18n-input-adornment-select";
import {connect} from "react-redux";
import {useLocation} from "react-router";
import {backupDatasetState} from "../../state/dataset/datasetActions";
import {useTranslation} from "react-i18next";

const mapStateToProps = state => ({
  language: state.app.language
});
const mapDispatchToProps = dispatch => ({
  onDatasetBackup: () => dispatch(backupDatasetState())
});

const AppLanguageSelect = ({language, onDatasetBackup}) => {

  const location = useLocation();
  const {t} = useTranslation();

  return (
    <I18nInputAdornmentSelect
      value={language}
      onChange={val => {
        if (val !== language) {
          onDatasetBackup();

          const url = location.pathname.includes(language)
            ? "./#" + location.pathname.replace(language, val) + location.search
            : `./#/${val}`;
          window.open(url, "_self");
        }
      }}
      ariaLabel={t("ariaLabels.header.appLanguage")}
    />
  );
};

export default connect(mapStateToProps, mapDispatchToProps)(AppLanguageSelect);