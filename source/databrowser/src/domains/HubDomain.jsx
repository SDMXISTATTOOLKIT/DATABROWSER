import React, {Fragment, useEffect} from 'react';
import {Route, Switch} from 'react-router';
import Home from '../components/home';
import {connect} from 'react-redux';
import {fetchHub} from '../state/hub/hubActions';
import NodeDomain from './NodeDomain';
import {HashRouter} from 'react-router-dom';
import DashboardsDomain from './DashboardsDomain';
import {goToHome} from '../links';
import Call from '../hocs/call';
import MultiLanguageDomain from './MultiLanguageDomain';
import {showUserSetPasswordForm} from '../state/user/userActions';
import WithCustomTheme, {
  CUSTOM_CSS_SELECTOR_A11Y_NODE_PREFIX, CUSTOM_CSS_SELECTOR_A11Y_PREFIX,
  CUSTOM_CSS_SELECTOR_NODE_PREFIX,
  CUSTOM_CSS_SELECTOR_PREFIX
} from "../components/with-custom-theme";
import A11yDomain from "./A11yDomain";

const mapStateToProps = state => ({
  hub: state.hub,
  defaultLanguage: state.app.language,
  isA11y: state.app.isA11y
});

const mapDispatchToProps = dispatch => ({
  fetchHub: () => dispatch(fetchHub()),
  onUserSetPasswordFormShow: token => dispatch(showUserSetPasswordForm(token))
});

const WithCustomThemeForNode = ({children, nodeCode, isA11y}) =>
  <WithCustomTheme
    getPath={selectorText => {
      const prefix = CUSTOM_CSS_SELECTOR_NODE_PREFIX + nodeCode + "__";
      const a11yPrefix = CUSTOM_CSS_SELECTOR_A11Y_NODE_PREFIX + nodeCode + "__";
      if (selectorText.startsWith(prefix)) {
        return selectorText.substr(prefix.length);
      } else if (selectorText.startsWith(CUSTOM_CSS_SELECTOR_PREFIX) &&
        !selectorText.startsWith(CUSTOM_CSS_SELECTOR_NODE_PREFIX)) {
        return selectorText.substr(CUSTOM_CSS_SELECTOR_PREFIX.length);
      } else if (isA11y && selectorText.startsWith(a11yPrefix)) {
        return selectorText.substr(a11yPrefix.length);
      } else if (isA11y && selectorText.startsWith(CUSTOM_CSS_SELECTOR_A11Y_PREFIX) &&
        !selectorText.startsWith(CUSTOM_CSS_SELECTOR_A11Y_NODE_PREFIX)) {
        return selectorText.substr(CUSTOM_CSS_SELECTOR_A11Y_PREFIX.length);
      } else {
        return false;
      }
    }}
  >
    {children}
  </WithCustomTheme>;

const WithCustomThemeNoNode = ({children, isA11y}) =>
  <WithCustomTheme
    getPath={selectorText => {
      if (selectorText.startsWith(CUSTOM_CSS_SELECTOR_PREFIX) &&
        !selectorText.startsWith(CUSTOM_CSS_SELECTOR_NODE_PREFIX)) {
        return selectorText.substr(CUSTOM_CSS_SELECTOR_PREFIX.length);
      } else if (isA11y && selectorText.startsWith(CUSTOM_CSS_SELECTOR_A11Y_PREFIX) &&
        !selectorText.startsWith(CUSTOM_CSS_SELECTOR_A11Y_NODE_PREFIX)) {
        return selectorText.substr(CUSTOM_CSS_SELECTOR_A11Y_PREFIX.length);
      } else {
        return false;
      }
    }}
  >
    {children}
  </WithCustomTheme>;

const HubDomain = ({hub, fetchHub, isA11y, onUserSetPasswordFormShow}) => {

  useEffect(() => {
    if (!hub) {
      fetchHub();
    }
  }, [hub, fetchHub]);

  const defaultNode = hub?.nodes.find(node => node.default === true);

  return (
    <Fragment>
      {hub && (
        <HashRouter>
          <A11yDomain>
            <Switch>
              <Route
                path='/'
                exact
                render={props => {

                  if (defaultNode !== undefined) {
                    return (
                      <MultiLanguageDomain
                        language={props.match.params.lang}>
                        <WithCustomThemeForNode nodeCode={defaultNode.code} isA11y={isA11y}>
                          <NodeDomain {...props}
                                      nodeCode={defaultNode.code}
                                      isDefault
                          />
                        </WithCustomThemeForNode>
                      </MultiLanguageDomain>
                    );
                  } else {
                    return (
                      <WithCustomThemeNoNode isA11y={isA11y}>
                        <Home {...props}/>
                      </WithCustomThemeNoNode>
                    );
                  }
                }}
              />
              <Route
                path='/:lang'
                exact
                render={props => {

                  if (defaultNode !== undefined) {
                    return (
                      <MultiLanguageDomain
                        language={props.match.params.lang}>
                        <WithCustomThemeForNode nodeCode={defaultNode.code} isA11y={isA11y}>
                          <NodeDomain {...props}
                                      nodeCode={defaultNode.code}
                                      isDefault
                          />
                        </WithCustomThemeForNode>
                      </MultiLanguageDomain>
                    );
                  } else {
                    return (
                      <MultiLanguageDomain language={props.match.params.lang}>
                        <WithCustomThemeNoNode isA11y={isA11y}>
                          <Home {...props}/>
                        </WithCustomThemeNoNode>
                      </MultiLanguageDomain>
                    );
                  }
                }}
              />
              <Route
                path='/:lang/dashboards'
                exact
                render={props => (
                  <MultiLanguageDomain language={props.match.params.lang}>
                    <WithCustomThemeNoNode isA11y={isA11y}>
                      <DashboardsDomain isDefault={defaultNode !== undefined} {...props}/>
                    </WithCustomThemeNoNode>
                  </MultiLanguageDomain>
                )}
              />
              <Route
                path='/:lang/dashboards/:dashboardId'
                render={props => (
                  <MultiLanguageDomain language={props.match.params.lang}>
                    <WithCustomThemeNoNode isA11y={isA11y}>
                      <DashboardsDomain {...props}
                                        dashboardId={props.match.params.dashboardId}
                                        isDefault={defaultNode !== undefined}/>
                    </WithCustomThemeNoNode>
                  </MultiLanguageDomain>
                )}
              />
              <Route
                path='/:lang/resetPassword'
                render={props => {
                  const tokenParam = new URLSearchParams(props.location.search).get('token');
                  if (tokenParam) {
                    return (
                      <MultiLanguageDomain language={props.match.params.lang}>
                        <Call cb={onUserSetPasswordFormShow} cbParam={tokenParam}>
                          <WithCustomThemeNoNode isA11y={isA11y}>
                            <Home {...props}/>
                          </WithCustomThemeNoNode>
                        </Call>
                      </MultiLanguageDomain>
                    );
                  } else {
                    goToHome();
                  }
                }}
              />
              <Route
                path='/:lang/:nodeCode'
                render={props => {
                  const node = hub.nodes.find(({code}) =>
                    code.toLowerCase() === props.match.params.nodeCode.toLowerCase()
                  );
                  if (node) {
                    return (
                      <MultiLanguageDomain language={props.match.params.lang}>
                        <WithCustomThemeForNode nodeCode={node.code} isA11y={isA11y}>
                          <NodeDomain {...props} nodeCode={props.match.params.nodeCode} isDefault={defaultNode?.nodeId === node.nodeId}/>
                        </WithCustomThemeForNode>
                      </MultiLanguageDomain>
                    );
                  } else {
                    goToHome();
                  }
                }}
              />
            </Switch>
          </A11yDomain>
        </HashRouter>
      )}
    </Fragment>
  );
};

export default connect(mapStateToProps, mapDispatchToProps)(HubDomain);
