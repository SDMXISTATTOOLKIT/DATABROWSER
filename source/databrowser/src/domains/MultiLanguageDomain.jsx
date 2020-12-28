import {useEffect} from 'react';
import {connect} from "react-redux";
import {setAppLanguage} from "../state/app/appActions";
import {useLocation} from "react-router";
import {goToHome} from "../links";

const mapStateToProps = state => ({
  appLanguage: state.app.language,
  languages: state.app.languages,
  node: state.node
});

const mapDispatchToProps = dispatch => ({
  setAppLanguage: (language, nodeId) => dispatch(setAppLanguage(language, nodeId)),
});

const MultiLanguageDomain = ({language, languages, appLanguage, setAppLanguage, children, node}) => {

  const location = useLocation();

  useEffect(() => {
    if (appLanguage !== language && languages.includes(language)) {
      setAppLanguage(language, node ? node.nodeId : null);
    } else if (language !== undefined && !languages.includes(language)) {
      goToHome(true);
    } else if (!languages.includes(language)) {
      goToHome();
    }
  }, [language, languages, setAppLanguage, appLanguage, node, location.pathname]);

  return (appLanguage === language && languages.includes(language))
    ? children
    : null;
};

export default connect(mapStateToProps, mapDispatchToProps)(MultiLanguageDomain);
