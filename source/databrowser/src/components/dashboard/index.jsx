import React, {Fragment, useEffect, useState} from "react";
import _ from "lodash";
import "./style.css"
import DashboardRow from "./DashboardRow";

function Dashboard(props) {
  const {
    dashboardId,
    dashboard: externalDashboard,
    filterValue,
    jsonStats,
    layouts,
    filterTrees,
    onFilterSet,
    maps,
    fetchMapGeometries,
    downloadCsv
  } = props;

  const [dashboard, setDashboard] = useState(externalDashboard);

  useEffect(() => {
    setDashboard(prevLayoutObj => {
      if (_.isEqual(prevLayoutObj, externalDashboard)) {
        return prevLayoutObj
      } else {
        return externalDashboard
      }
    });
  }, [externalDashboard]);

  return (
    <Fragment>
      {(dashboard.dashboardConfig || []).map((row, idx) =>
        <DashboardRow
          key={idx}
          dashboardId={dashboardId}
          dashboard={dashboard}
          filterValue={filterValue}
          row={row}
          rowIdx={idx}
          jsonStats={jsonStats}
          layouts={layouts}
          filterTrees={filterTrees}
          maps={maps}
          onSelect={onFilterSet}
          fetchMapGeometries={fetchMapGeometries}
          downloadCsv={downloadCsv}
        />
      )}
    </Fragment>
  )
}

export default Dashboard;