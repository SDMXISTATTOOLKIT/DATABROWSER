import {
  getFilteredChartLayout,
  getFilteredMapLayout,
  getFilteredTableLayout,
  getFilterTreeFromJsonStat,
  getInitialChartLayout,
  getInitialFiltersValue,
  getInitialMapLayout,
  getInitialTableLayout,
  getJsonStatTableSize
} from "../utils/jsonStat";

onmessage = event => {
  const {
    response,
    extra
  } = event.data;

  const cellCount = getJsonStatTableSize(response);

  const isTooBigData = cellCount > extra.maxAllowedCells;
  const isEmptyData = (extra.status === 204 || (response?.id || "").length === 0);
  const isPartialData = extra.status === 206;

  const isResponseNotValid = (!response || isTooBigData || isEmptyData);

  let tableLayout = null, tableFilterTree = null;
  let mapLayout = null, mapFilterTree = null;
  let chartLayout = null, chartFilterTree = null;

  if (!isResponseNotValid) {

    /** table layout handling **/
    tableLayout = extra.tableLayout
      ? getFilteredTableLayout(extra.tableLayout, response)
      : getInitialTableLayout(response);
    if (tableLayout) {
      tableFilterTree = getFilterTreeFromJsonStat(tableLayout, response);
      tableLayout = {
        ...tableLayout,
        filtersValue: tableLayout.filtersValue
          ? tableLayout.filtersValue
          : getInitialFiltersValue(response, tableLayout, tableFilterTree)
      }
    }

    /** map layout handling **/
    if ((response?.role?.geo || []).length > 0) {
      mapLayout = extra.mapLayout
        ? getFilteredMapLayout(extra.mapLayout, response)
        : getInitialMapLayout(response);
      if (mapLayout) {
        mapFilterTree = getFilterTreeFromJsonStat(mapLayout, response);
        mapLayout = {
          ...mapLayout,
          filtersValue: mapLayout.filtersValue
            ? mapLayout.filtersValue
            : getInitialFiltersValue(response, mapLayout, mapFilterTree)
        }
      }
    }

    /** chart layout handling **/
    chartLayout = extra.chartLayout
      ? getFilteredChartLayout(extra.chartLayout, response)
      : getInitialChartLayout(response);
    if (chartLayout) {
      chartFilterTree = getFilterTreeFromJsonStat(chartLayout, response);
      chartLayout = {
        ...chartLayout,
        filtersValue: chartLayout.filtersValue
          ? chartLayout.filtersValue
          : getInitialFiltersValue(response, chartLayout, chartFilterTree)
      }
    }
  }

  postMessage({
    cellCount,
    isTooBigData,
    isEmptyData,
    isPartialData,
    isResponseNotValid,
    tableLayout,
    tableFilterTree,
    mapLayout,
    mapFilterTree,
    chartLayout,
    chartFilterTree
  });
};