import React from 'react';
import TextField from '@material-ui/core/TextField';
import Tooltip from '@material-ui/core/Tooltip';
import AddIcon from '@material-ui/icons/Add';
import CloseIcon from '@material-ui/icons/Close';
import VisibilityIcon from '@material-ui/icons/Visibility';

const iconStyle = {
  cursor: 'pointer'
};

/* Based on https://ant.design/components/form/#components-form-demo-customized-form-controls */
class Selector extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      value: null
    };
    this.callOnChange = this.callOnChange.bind(this);
  }

  static getDerivedStateFromProps(nextProps, state) {
    if (nextProps.value !== state.value) {
      return {
        value: nextProps.value
      };
    } else {
      return null;
    }
  }

  callOnChange(value) {
    if (this.props.onChange) {
      this.props.onChange(value);
    }
  }

  render() {

    const {
      disabled,
      detailTitle,
      selectTitle,
      resetTitle,
      onDetail,
      onSelect,
      onReset,
      render,
      variant,
      className
    } = this.props;

    const {
      value
    } = this.state;

    return (
      <TextField
        value={render ? render(value) : value}
        onChange={f => f}
        variant={variant || "outlined"}
        InputProps={{
          readOnly: true,
          endAdornment: disabled
            ? undefined
            : value !== null
              ? resetTitle
                ? (
                  <Tooltip title={resetTitle}>
                    <CloseIcon onClick={onReset} style={iconStyle}/>
                  </Tooltip>
                )
                : <CloseIcon onClick={onReset} style={iconStyle}/>
              : selectTitle
                ? (
                  <Tooltip title={selectTitle}>
                    <AddIcon onClick={onSelect} style={iconStyle}/>
                  </Tooltip>
                )
                : <AddIcon onClick={onSelect} style={iconStyle}/>,
          startAdornment: (onDetail && value !== null)
            ? detailTitle
              ? (
                <Tooltip title={detailTitle}>
                  <VisibilityIcon onClick={onDetail} style={iconStyle}/>
                </Tooltip>
              )
              : <VisibilityIcon onClick={onDetail} style={iconStyle}/>
            : undefined
        }}
        className={className}
      />
    );
  }
}

export default Selector;
