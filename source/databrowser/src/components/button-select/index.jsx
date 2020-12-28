import React, {Component, forwardRef, Fragment} from 'react';
import {withStyles} from "@material-ui/core";
import Menu from "@material-ui/core/Menu";
import MenuItem from "@material-ui/core/MenuItem";
import Button from "@material-ui/core/Button";
import ArrowDropDownIcon from '@material-ui/icons/ArrowDropDown';
import IconButton from '@material-ui/core/IconButton';
import Tooltip from "@material-ui/core/Tooltip";

const styles = theme => ({
  menuItem: {
    background: "white",
    "&:focus": {
      background: "rgba(0, 41, 90, 0.2)"
    }
  }
});

class ButtonSelect extends Component {

  state = {
    anchorEl: null
  };

  setAnchorEl = (el) => {
    this.setState({
      anchorEl: el
    });
  };

  handleMenu = (event) => {
    this.setAnchorEl(event.currentTarget);
  };

  handleClose = () => {
    this.setAnchorEl(null);
  };

  render() {

    const {classes, children, icon, value, color, tooltip, disabled, selectedIdx, onChange, ariaLabel} = this.props;
    const {anchorEl} = this.state;

    const open = Boolean(anchorEl);

    const Item = forwardRef(({c, isSelected}, ref) =>
      <MenuItem
        ref={ref}
        onClick={() => {
          if (c.props?.onClick) {
            c.props.onClick();
          }
          if (onChange) {
            onChange(c.props["data-value"]);
          }
          this.handleClose();
        }}
        className={classes.menuItem}
        style={{background: isSelected ? "#fff9e5" : undefined}}
        tabIndex={0}
      >
        {c}
      </MenuItem>);

    return (
      <Fragment>
        <Tooltip title={tooltip || ""}>
          <span>
            {value
              ? (
                <Button
                  color={color || "inherit"}
                  onClick={this.handleMenu}
                  startIcon={icon}
                  endIcon={<ArrowDropDownIcon/>}
                  disabled={disabled}
                  aria-label={ariaLabel}
                >
                  {value}
                </Button>
              )
              : (
                <IconButton
                  color={color || "inherit"}
                  onClick={this.handleMenu}
                  disabled={disabled}
                  aria-label={ariaLabel}
                >
                  {icon}
                </IconButton>
              )
            }
          </span>
        </Tooltip>
        <Menu
          anchorEl={anchorEl}
          anchorOrigin={{
            vertical: 'top',
            horizontal: 'right',
          }}
          keepMounted
          transformOrigin={{
            vertical: 'top',
            horizontal: 'right',
          }}
          open={open}
          onClose={this.handleClose}
        >
          {children.length
            ? children.filter(c => c).map((c, i) => c !== null
              ? <Item c={c} key={i} isSelected={i === selectedIdx}/>
              : null
            )
            : <Item c={children}/>
          }
        </Menu>
      </Fragment>
    );
  }
}

export default withStyles(styles)(ButtonSelect);