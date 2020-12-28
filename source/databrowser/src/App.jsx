import React, {Fragment} from 'react';
import HubDomain from "./domains/HubDomain";
import Helmet from "react-helmet";
import {connect} from "react-redux";

const App = ({language}) =>
  <Fragment>
    <Helmet>
      <html lang={language}/>
    </Helmet>
    <HubDomain/>
  </Fragment>;

export default connect(state => ({
  language: state.app.language
}))(App);