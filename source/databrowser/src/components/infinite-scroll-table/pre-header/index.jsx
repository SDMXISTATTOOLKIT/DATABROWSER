import React, {Fragment} from "react";
import {withTranslation} from "react-i18next";
import AutoSearchInput from "../../auto-search-input";
import Grid from "@material-ui/core/Grid";

const PreHeader = ({
                     t,
                     searchText,
                     onSearch,
                     selectedRowCount,
                     tableActions,
                     renderTableAction,
                     leftActions,
                     rightActions
                   }) =>
  <Grid container style={{marginBottom: 4}}>
    <Grid item xs={6}>
      <Grid container justify="space-between">
        <Grid item>
          {leftActions}
        </Grid>
        <Grid item>
          <Grid container justify="flex-end">
            {tableActions && selectedRowCount > 0 && (
              <Fragment>
                <Grid item style={{padding: "14px 16px 14px 8px"}}>
                  {selectedRowCount > 1
                    ? t("components.infiniteScrollTable.preHeader.selectedCount_plural", {selectedRowCount})
                    : t("components.infiniteScrollTable.preHeader.selectedCount", {selectedRowCount})
                  }
                </Grid>
                {tableActions.map((action, index) =>
                  <Grid item key={index}>
                    {renderTableAction(action)}
                  </Grid>
                )}
              </Fragment>
            )}
            {rightActions && (
              <Grid item>
                {rightActions}
              </Grid>
            )}
          </Grid>
        </Grid>
      </Grid>
    </Grid>
    <Grid item xs={6} style={{padding: "8px 0"}}>
      <AutoSearchInput
        value={searchText}
        onSearch={onSearch}
      />
    </Grid>

  </Grid>;

export default withTranslation()(PreHeader);
