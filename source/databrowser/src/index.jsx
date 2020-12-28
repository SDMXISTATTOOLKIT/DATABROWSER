import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import 'typeface-roboto';
import "fontsource-do-hyeon";
import "./style.css";
import 'flag-icon-css/css/flag-icon.css';
import {Provider} from 'react-redux'
import init from "./init";

init(store =>
  ReactDOM.render(
    <Provider store={store}>
      <App/>
    </Provider>,
    document.getElementById('root')
  ));
