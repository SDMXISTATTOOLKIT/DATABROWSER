import React from "react";
import withStyles from "@material-ui/core/styles/withStyles";
import {DASHBOARD_ELEM_FILTER_DIMENSION_KEY, getViewIdxFromRowAndCol} from "../../utils/dashboards";
import DashboardCol from "./DashboardCol";

const styles = theme => ({
  row: {
    width: "100%",
    marginBottom: 8
  }
});

const DashboardRow = ({
                        classes,
                        dashboardId,
                        dashboard,
                        filterValue,
                        row,
                        rowIdx,
                        jsonStats,
                        layouts,
                        filterTrees,
                        onSelect,
                        maps,
                        fetchMapGeometries,
                        downloadCsv
                      }) =>

  <div className={classes.row}>
    {row.map((col, idx) => {
      const viewIdx = getViewIdxFromRowAndCol(rowIdx, idx);
      return (
        <DashboardCol
          key={idx}
          dashboardId={dashboardId}
          dashboard={dashboard}
          filterValue={col[DASHBOARD_ELEM_FILTER_DIMENSION_KEY] ? filterValue : null}
          rowIdx={rowIdx}
          colIdx={idx}
          dashboardElem={col}
          jsonStat={jsonStats?.[viewIdx]}
          layout={layouts?.[viewIdx]}
          filterTree={filterTrees?.[viewIdx]}
          map={maps?.[viewIdx]}
          onSelect={onSelect}
          fetchMapGeometries={fetchMapGeometries}
          downloadCsv={downloadCsv}
        />
      )
    })}
  </div>;

export default withStyles(styles)(DashboardRow);