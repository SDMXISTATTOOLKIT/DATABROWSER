import React from 'react';

/* Calls a function right after the WrappedComponent has been mounted and updated (if props changed) */
class Call extends React.Component {

  componentDidMount() {
    /* children has been mounted */
    if (this.props.disabled === undefined || !this.props.disabled) {
      this.props.cb(this.props.cbParam);
    }
  }

  componentDidUpdate({cb, cbParam, disabled}) {
    /* children has been updated */
    if (
      (this.props.disabled === undefined || !this.props.disabled)
      &&
      (
        JSON.stringify(cbParam) !== JSON.stringify(this.props.cbParam)
        ||
        (this.props.disabled !== undefined && disabled === true && this.props.disabled === false)
      )
    ) {
      this.props.cb(this.props.cbParam);
    }
  }

  render() {
    return this.props.children;
  }
}

export default Call;
