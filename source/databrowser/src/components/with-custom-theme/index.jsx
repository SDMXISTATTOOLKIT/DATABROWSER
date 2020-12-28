import React, {Fragment, useEffect, useState} from 'react';
import {ThemeProvider} from "@material-ui/core/styles";
import {createMuiTheme, CssBaseline} from "@material-ui/core";
import colorString from "color-string";
import {withTheme} from "@material-ui/styles";
import _ from "lodash";
import DEFAULT_THEME from "../../utils/defaultTheme";
import NotImplementedSnackbar from "../not-implemented-snackbar/NotImplementedSnackbar";
import ErrorSnackbar from "../error-snackbar/ErrorSnackbar";
import DeleteConflictModal from "../delete-conflict-modal";
import UserSetPasswordDialog from "../user-set-password-dialog";
import Spinner from "../spinner/Spinner";
import ErrorBoundary from "../error-boundary";
import Helmet from "react-helmet";

/*
  custom.css can be used to apply colors in the mui theme.
  any color in the following "theme object" can be changed:
  https://material-ui.com/customization/default-theme/#default-theme

  just put a css rule in custom.css like this:
  .theme__palette-error-main { color: #FF0000 }

  each selector must start with ".theme__" and continue with the path in the "theme object"
  the properties in the path must be separated by "-".
  if the selector don't follow these rules, it will be discarded.
  other rules different from color (e.g. fontSize) cannot be changed and will be discarded.

  you can apply colors also for specific nodes, like this:
  .theme__node__NODE_ID__palette-error-main { color: #FF0000 }

  make sure NODE_ID is equal (case sensitive) to the id of the node and
  make sure rules for specific nodes are written after general rules

  e.g. this is correct, second rule will override first rule if NODE_ID is selected in the app:
  .theme__palette-error-main { color: #FF0000 }
  .theme__node__NODE_ID__palette-error-main { color: #FF0000 }

  but THIS IS NOT CORRECT (second rule will always override first rule, even if NODE_ID is selected:
  .theme__node__NODE_ID__palette-error-main { color: #FF0000 }
  .theme__palette-error-main { color: #FF0000 }
*/

export const CUSTOM_CSS_SELECTOR_PREFIX = ".theme__";
export const CUSTOM_CSS_SELECTOR_NODE_PREFIX = ".theme__node__";
export const CUSTOM_CSS_SELECTOR_A11Y_PREFIX = ".a11y-theme__";
export const CUSTOM_CSS_SELECTOR_A11Y_NODE_PREFIX = ".a11y-theme__node__";
const CUSTOM_CSS_SELECTOR_PATH_SPLIT_CHAR = "-";

const WithCustomTheme = ({children, getPath}) => {

    const [theme, setTheme] = useState(undefined);

    useEffect(() => {


      fetch('./custom.css')
        .then(response => response.text())
        .then(css => {

          const doc = document.implementation.createHTMLDocument("");
          const styleEl = document.createElement("style");
          styleEl.textContent = String(css);
          doc.body.appendChild(styleEl);

          const customTheme = {};

          Object.values(styleEl.sheet.cssRules)
            .filter(({selectorText, style}) =>
              selectorText &&
              getPath(selectorText) &&
              style[0] === "color" &&
              style.color.length > 0 &&
              colorString.get.rgb(style.color) !== null
            )
            .forEach(({selectorText, style}) => {

              const path = getPath(selectorText).split(CUSTOM_CSS_SELECTOR_PATH_SPLIT_CHAR);

              let pointer = customTheme;
              path.forEach((prop, index) => {
                if (index === path.length - 1) {
                  pointer[prop] = colorString.to.rgb(colorString.get.rgb(style.color));
                } else {
                  if (!pointer[prop]) {
                    pointer[prop] = {};
                  }
                  pointer = pointer[prop];
                }
              });
            });

          setTheme(createMuiTheme(_.merge(_.cloneDeep(DEFAULT_THEME), customTheme)));
        });

    }, [getPath])
    return theme
      ? (
        <Fragment>
          <Helmet>
            {/* eslint-disable-next-line react/style-prop-object */}
            <body style={"background-color: " + theme.palette.primary.main}/>
          </Helmet>
          <CssBaseline/>
          <ThemeProvider theme={theme}>
            <ErrorBoundary>
              <Spinner>
                <NotImplementedSnackbar/>
                <ErrorSnackbar/>
                <DeleteConflictModal/>
                <UserSetPasswordDialog/>
                {children}
              </Spinner>
            </ErrorBoundary>
          </ThemeProvider>
        </Fragment>
      )
      : null;
  }
;

export default withTheme(WithCustomTheme);