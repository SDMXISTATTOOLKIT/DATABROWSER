import React from 'react';
import ButtonSelect from "../button-select";
import LanguageIcon from '@material-ui/icons/Language';

const LanguageSelect = () =>
  <ButtonSelect value="Italiano" icon={<LanguageIcon/>}>
    <span>Italiano</span>
    <span>Inglese</span>
  </ButtonSelect>;

export default LanguageSelect;