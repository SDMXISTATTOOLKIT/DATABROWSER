import {connect} from "react-redux";
import {setAppIsA11y} from "../state/app/appActions";
import {useLocation} from "react-router";
import React, {useEffect} from "react";

const mapStateToProps = state => ({
  isA11y: state.app.isA11y
});

const mapDispatchToProps = dispatch => ({
  onChange: value => dispatch(setAppIsA11y(value)),
});

const A11yDomain = ({isA11y, children, onChange}) => {

  const location = useLocation();

  const isA11yParam = new URLSearchParams(location.search).get("accessible") === "true";

  useEffect(() => {
    if (isA11yParam !== isA11y) {
      onChange(isA11yParam);
    }
  }, [isA11yParam, isA11y, onChange]);

  return isA11yParam === isA11y
    ? (
      <div className={isA11y ? "a11y" : undefined}>
        {children}
      </div>
    ) : null;
};

export default connect(mapStateToProps, mapDispatchToProps)(A11yDomain);
