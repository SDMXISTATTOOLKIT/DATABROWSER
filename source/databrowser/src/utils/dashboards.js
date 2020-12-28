export const DASHBOARD_ELEM_TYPE_KEY = "type";
export const DASHBOARD_ELEM_VALUE_KEY = "value";
export const DASHBOARD_ELEM_WIDTH_KEY = "widthPercentage";
export const DASHBOARD_ELEM_SHOW_TITLE_KEY = "showTitle";
export const DASHBOARD_ELEM_ENABLE_FILTERS_KEY = "enableFilters";
export const DASHBOARD_ELEM_FILTER_DIMENSION_KEY = "filterDimension";

export const DASHBOARD_ELEM_TYPE_VALUE_VIEW = "view";
export const DASHBOARD_ELEM_TYPE_VALUE_TEXT = "text";

export const emptyDashboardElem = {
  [DASHBOARD_ELEM_TYPE_KEY]: null,
  [DASHBOARD_ELEM_VALUE_KEY]: null,
  [DASHBOARD_ELEM_WIDTH_KEY]: 100,
  [DASHBOARD_ELEM_SHOW_TITLE_KEY]: true,
  [DASHBOARD_ELEM_ENABLE_FILTERS_KEY]: false,
  [DASHBOARD_ELEM_FILTER_DIMENSION_KEY]: null
};

export const getViewIdxFromRowAndCol = (rowIdx, colIdx) => `${rowIdx}+${colIdx}`;

export const getFilterDimensionId = (layout, filterDimensionIds) => {
  const dimArraysKey = ["rows", "cols", "filters", "sections", "primaryDim", "secondaryDim", "territoryDim"];
  let viewDims = [];

  dimArraysKey.forEach(key => viewDims = viewDims.concat(layout[key] || []));

  return (filterDimensionIds.find(dimId => viewDims.includes(dimId)) || null);
};