import React, {useEffect, useState} from 'react';
import ButtonSelect from "../button-select";
import {connect} from "react-redux";
import CountryLanguage from 'country-language';

const CUSTOM_LANG_TO_COUNTRY = {
  en: "gb"
};

const I18nInputAdornmentSelect = ({value, onChange, languages, ariaLabel}) => {

  const [langToCountry, setLangToCountry] = useState(null);

  useEffect(() => {

    let res = {};

    languages.forEach(langCode =>
      CountryLanguage.getLanguageCountries(langCode, (err, countries) => {
        if (!err) {
          res[langCode] =
            CUSTOM_LANG_TO_COUNTRY[langCode] ||
            countries.find(({code_2}) => code_2.toLowerCase() === langCode)?.code_2.toLowerCase() ||
            countries[0].code_2.toLowerCase();
        }
      }));

    setLangToCountry(res);

  }, [languages]);

  return langToCountry && (
    <ButtonSelect
      value={<span className={`flag-icon flag-icon-${langToCountry[value]}`}/>}
      onChange={onChange}
      ariaLabel={ariaLabel}
    >
      {languages.map(lang =>
        <span key={lang} className={`flag-icon flag-icon-${langToCountry[lang]}`} data-value={lang} aria-label={lang}/>
      )}
    </ButtonSelect>
  );
};

export default connect(state => ({
  languages: state.app.languages
}))(I18nInputAdornmentSelect);