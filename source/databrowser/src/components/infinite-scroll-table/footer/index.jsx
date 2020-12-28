import React from "react";
import {withTranslation} from "react-i18next";
import Grid from "@material-ui/core/Grid";
import CircularProgress from "@material-ui/core/CircularProgress";

const Footer = ({
                  t,
                  isLoading,
                  isHidden,
                  rowNumStart,
                  rowNumEnd,
                  rowCount
                }) =>
  <Grid container justify="flex-end" style={{marginTop: 8}}>
    <Grid item>
      {!isHidden
        ? (
          isLoading
            ? (
              <CircularProgress
                size="small"
                className="infinite-scroll-table__footer__spinner"
                style={{
                  fontSize: 10,
                  width: 13,
                  height: 13,
                  marginRight: 2
                }}
              />
            )
            : t("components.infiniteScrollTable.footer.rowCount", {rowNumStart, rowNumEnd, rowCount})
        )
        : <span/>
      }
    </Grid>
  </Grid>;

export default withTranslation()(Footer);
