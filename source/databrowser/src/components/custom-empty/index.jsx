import React from 'react';
import {compose} from "redux";
import withStyles from "@material-ui/core/styles/withStyles";
import {withTranslation} from "react-i18next";

const styles = theme => ({
  root: {
    position: "relative",
    height: "100%"
  },
  text: {
    fontStyle: "italic",
    position: "absolute",
    top: "50%",
    left: "50%",
    transform: "translate(-50%)"
  },
  image: {
    position: "absolute",
    top: "50%",
    left: "50%",
    transform: "translate(-50%, -100%)",
    paddingBottom: 16
  }
});

const CustomEmpty = ({
  t,
                       classes,
                       text,
                       textStyle,
                       width,
                       backgroundColor,
                       color,
                       image
                     }) =>
  <div className={classes.root} style={{width: width || "100%", backgroundColor, color}}>
    <div style={{width: width || "100%"}}>
      {image && (
        <div className={classes.image}>
          {image}
        </div>
      )}
      <div className={classes.text} style={{...textStyle}}>
        {text || t("components.customEmpty.placeholder")}
      </div>
    </div>
  </div>;

export default compose(
  withStyles(styles),
  withTranslation()
)(CustomEmpty);
