const a11yMiddleware = ({getState}) => next => action => {

  const res = next(action);

  window.isA11y = getState().app.isA11y;

  return res;

};

export default a11yMiddleware;