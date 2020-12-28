import React from "react";
import Tooltip from "@material-ui/core/Tooltip";
import InfoIcon from '@material-ui/icons/Info';
import IconButton from "@material-ui/core/IconButton";
import withTheme from "@material-ui/core/styles/withTheme";

const InfoTooltip = ({children, theme}) => {

  return children
    ? (
      <Tooltip title={children} placement="top">
        <IconButton
          aria-label="info"
          style={{
            color: theme.palette.primary.main,
            padding: 0,
            transform: "translateY(-1px)"
          }}
        >
          <InfoIcon fontSize="small"/>
        </IconButton>
      </Tooltip>
    )
    : (
      <IconButton
        aria-label="info"
        style={{
          color: theme.palette.primary.main,
          padding: 0,
          transform: "translateY(-1px)"
        }}
      >
        <InfoIcon fontSize="small"/>
      </IconButton>
    );
};

export default withTheme(InfoTooltip);