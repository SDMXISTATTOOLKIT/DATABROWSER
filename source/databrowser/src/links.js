export const goToDatasets = (nodeCode, categoryPath) =>
  window.open(`./#/${window.language}/${nodeCode.toLowerCase()}/${categoryPath.join("/")}${window.isA11y ? "?accessible=true" : ""}`, '_self');

export const goToData = (nodeCode, categoryPath, datasetId, viewId) =>
  window.open(
    `./#/${window.language}/${nodeCode.toLowerCase()}/${categoryPath.length > 0 ? categoryPath.join("/") + "/" : ""}` +
    `${datasetId}${(viewId || window.isA11y) ? '?' : ''}${viewId ? `view=${viewId}` : ""}${window.isA11y ? `${viewId ? "&accessible=true" : "accessible=true"}` : ""}`,
    '_self'
  );

// filters = c1/c2,c3/c4
export const goToDatasetsSearch = (nodeCode, query, filters) => {
  window.open(
    `./#/${window.language}/${nodeCode.toLowerCase()}/search?q=${encodeURIComponent(query)}` +
    (filters && filters.length > 0 ? `&c=${encodeURIComponent(filters.map(f => encodeURIComponent(f.join("/"))).join("//"))}` : '') +
    (window.isA11y ? "&accessible=true" : ""),
    '_self'
  );
};

export const goToNode = nodeCode => window.open(`./#/${window.language}/${nodeCode}${window.isA11y ? "?accessible=true" : ""}`, '_self');
export const goToHome = noLanguage => window.open(`./#/${noLanguage ? "" : window.language}${window.isA11y ? "?accessible=true" : ""}`, '_self');

export const goToHubDashboards = () => window.open(`./#/${window.language}/dashboards${window.isA11y ? "?accessible=true" : ""}`, '_self');
export const goToNodeDashboards = nodeCode => window.open(`./#/${window.language}/${nodeCode.toLowerCase()}/dashboards${window.isA11y ? "?accessible=true" : ""}`, '_self');
export const goToDashboard = dashboardId => window.open(`./#/${window.language}/dashboards/${dashboardId}${window.isA11y ? "?accessible=true" : ""}`, '_self');