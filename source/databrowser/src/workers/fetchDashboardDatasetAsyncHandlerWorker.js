import {
  DASHBOARD_ELEM_ENABLE_FILTERS_KEY,
  DASHBOARD_ELEM_TYPE_KEY,
  DASHBOARD_ELEM_TYPE_VALUE_VIEW,
  DASHBOARD_ELEM_VALUE_KEY,
  getViewIdxFromRowAndCol
} from "../utils/dashboards";
import {
  getFilteredChartLayout,
  getFilteredMapLayout,
  getFilteredTableLayout,
  getFilterTreeFromJsonStat
} from "../utils/jsonStat";

onmessage = event => {
  const {
    dashboardId,
    requestIds,
    response,
    dashboard
  } = event.data;

  const dashboardJsonStats = {};
  const dashboardLayouts = {};
  const dashboardFilterTrees = {};

  dashboard.dashboardConfig.forEach((row, rowIdx) => {
    row.forEach((col, colIdx) => {
      const viewIdx = getViewIdxFromRowAndCol(rowIdx, colIdx);
      if (col[DASHBOARD_ELEM_TYPE_KEY] === DASHBOARD_ELEM_TYPE_VALUE_VIEW && requestIds.includes(viewIdx)) {
        const view = dashboard.views[col[DASHBOARD_ELEM_VALUE_KEY]];

        if ((response?.id || []).length === 0) {
          dashboardJsonStats[viewIdx] = "";
          dashboardLayouts[viewIdx] = null;
          dashboardFilterTrees[viewIdx] = null;

        } else {
          let layout;
          if (view.layouts.tableLayout) {
            layout = getFilteredTableLayout(view.layouts.tableLayout, response);
          } else if (view.layouts.mapLayout) {
            layout = getFilteredMapLayout(view.layouts.mapLayout, response);
          } else {
            layout = getFilteredChartLayout(view.layouts.chartLayout, response);
          }

          dashboardJsonStats[viewIdx] = response;
          dashboardLayouts[viewIdx] = {
            layout: layout,
            tableEmptyChar: view.layouts.tableEmptyChar,
            mapDetailLevel: view.layouts.mapDetailLevel,
            mapClassificationMethod: view.layouts.mapClassificationMethod,
            mapPaletteStartColor: view.layouts.mapPaletteStartColor,
            mapPaletteEndColor: view.layouts.mapPaletteEndColor,
            mapPaletteCardinality: view.layouts.mapPaletteCardinality,
            mapOpacity: view.layouts.mapOpacity,
            mapIsLegendCollapsed: true, // proprietà cablata per evitare doppia update
            // mapIsLegendCollapsed: view.layouts.mapIsLegendCollapsed,
            chartStacked: view.layouts.chartStacked,
            chartLegendPosition: view.layouts.chartLegendPosition,
            chartColors: view.layouts.chartColors
          };
          dashboardFilterTrees[viewIdx] = col[DASHBOARD_ELEM_ENABLE_FILTERS_KEY]
            ? getFilterTreeFromJsonStat(layout, response)
            : null;
        }
      }
    });
  });

  postMessage({
    dashboardId,
    dashboardJsonStats,
    dashboardLayouts,
    dashboardFilterTrees
  });
};