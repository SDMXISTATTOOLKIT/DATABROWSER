/* config.json */
export const getInitConfigUrl = () => "config.json";
export const getAppConfigUrl = () => "appConfig.json";

/* hub */
export const getHubMinimalInfoUrl = () => "hub/minimalInfo";

/* node */
export const getNodeUrl = (nodeId: number) => `nodes/${nodeId}`;
export const getNodeCatalogUrl = (nodeId: number) => `nodes/${nodeId}/catalog`;

/* cache */
export const getClearMemoryCacheUrl = () => "MemoryCache/Clear";
export const getDataflowsCacheListUrl = (nodeId: number) => `DataflowCache/DataflowData/Nodes/${nodeId}`;
export const getDataflowsCacheUpdateUrl = (nodeId: number, cacheId: string) => `DataflowCache/DataflowData/${cacheId}/Nodes/${nodeId}`;
export const getDataflowsCreateCacheUrl = (nodeId: number) => `DataflowCache/DataflowData/Nodes/${nodeId}`;
export const getDataflowsClearCacheUrl = (nodeId: number, cacheId: string) => `DataflowCache/Clear/${cacheId}/Nodes/${nodeId}`;
export const getDataflowsClearAllCacheUrl = (nodeId: number) => `DataflowCache/ClearAll/Nodes/${nodeId}`;
export const getClearNodeCatalogMemoryCacheUrl = (nodeId: number) => `MemoryCache/Clear/CatalogTree/Nodes/${nodeId}`;

/* templates */
export const getNodeTemplatesUrl = (nodeId: number) => `nodes/${nodeId}/templates`;
export const getCreateTemplateUrl = (nodeId: number) => `nodes/${nodeId}/templates`;
export const getDeleteTemplateUrl = (nodeId: number, id: number) => `nodes/${nodeId}/templates/${id}`;

/* views */
export const getViewsUrl = () => `views`;
export const getCreateViewUrl = (nodeId: number) => `nodes/${nodeId}/views`;
export const getDeleteViewUrl = (nodeId: number, id: number) => `nodes/${nodeId}/views/${id}`;

/* dataset */
export const getDatasetUrl = (nodeId: number, datasetId: string) =>
  `nodes/${nodeId}/datasets/${datasetId}/data`;
export const getDatasetStructureUrl = (nodeId: number, datasetId: string, viewId: string) =>
  `nodes/${nodeId}/datasets/${datasetId}/structure${viewId ? `/${viewId}` : ""}`;
export const getDatasetStructureCodelistUrl = (nodeId: number, datasetId: string, dimensionId: string, isFull: boolean) =>
  `nodes/${nodeId}/datasets/${datasetId}/column/${dimensionId}/${isFull ? "full" : "partial"}/values`;
export const getDatasetStructureDynamicCodelistUrl = (nodeId: number, datasetId: string, dimensionId: string) =>
  `nodes/${nodeId}/datasets/${datasetId}/PartialCodelists/${dimensionId}`;
export const getDatasetStructureCodelistsUrl = (nodeId: number, datasetId: string, isFull: boolean) =>
  `nodes/${nodeId}/datasets/${datasetId}/columns/${isFull ? "full" : "partial"}/values`;
export const getDatasetDownloadUrl = (nodeId: number, datasetId: string, format: string, zipped?: boolean) =>
  `nodes/${nodeId}/datasets/${datasetId}/${zipped === true ? "downloadZip" : "download"}/${format}`;

/* config */
export const getHubConfigGetUrl = () => "hub/config";
export const getHubConfigPutUrl = () => "hub";
export const getNodesConfigUrl = () => "nodes/config";
export const getNodesOrderPutUrl = () => "nodes/order";
export const getNodeConfigGetUrl = (nodeId: number) => `nodes/${nodeId}/config`;
export const getNodesConfigPostUrl = () => 'nodes';
export const getNodesConfigPutUrl = (nodeId: number) => `nodes/${nodeId}`;
export const getNodeDeleteUrl = (nodeId: number, force?: boolean) => `nodes/${nodeId}${force ? "/true" : ""}`;

/* log */
export const getQueryLog = (limit: number) => `tracing/limit/${limit}`;

/* files */
export const getFileUploadUrl = () => 'File/Upload';

/* authentication */
export const getTokenUrl = () => 'auth/token';
export const getRefreshTokenUrl = () => 'auth/refreshToken';

/* users management */
export const getUsersConfigGetUrl = () => "users";
export const getUserConfigGetUrl = (userId: number) => `users/${userId}`;
export const getUserConfigPostUrl = () => "users/register";
export const getUserConfigPutUrl = (userId: number) => `users/${userId}`;
export const getUserDeleteUrl = (userId: number) => `users/${userId}`;
export const getUserSendResetPasswordMailUrl = () => 'Users/RecoveryPassword';
export const getUserResetPasswordUrl = () => 'Users/ResetPassword';
export const getUserChangePasswordUrl = () => 'Users/ChangePassword';

/* Geometry */
export const getGeometryUrl = () => `Geometry`;

/* permissions */
export const getNodePermissionsConfigGetUrl = (nodeId: number) => `Users/Permissions/Nodes/${nodeId}`;
export const getNodePermissionsConfigPutUrl = (nodeId: number) => `Users/Permissions/Nodes/${nodeId}`;

/* dashboards */
export const getUserDashboardsUrl = () => "Dashboards";
export const getDashboardsUrl = (dashboardId: number) => `Dashboards/${dashboardId}`;
export const getDeleteDashboardUrl = (dashboardId: number) => `Dashboards/${dashboardId}`;
export const getCreateDashboardUrl = () => `Dashboards/`;
export const getUpdateDashboardUrl = (dashboardId: number) => `Dashboards/${dashboardId}`;
export const getHubDashboardsUrl = () => "Dashboards/Hub";
export const getNodeDashboardsUrl = (nodeId: number) => `Dashboards/Nodes/${nodeId}`;
export const getAddHubDashboardUrl = (dashboardId: number) => `Dashboards/${dashboardId}/Hub`;
export const getRemoveHubDashboardUrl = (dashboardId: number) => `Dashboards/${dashboardId}/Hub`;
export const getOrderHubDashboardsUrl = () => 'Dashboards/Order/Hub';
export const getAddNodeDashboardUrl = (nodeId: number, dashboardId: number) => `Dashboards/${dashboardId}/Nodes/${nodeId}`;
export const getRemoveNodeDashboardUrl = (nodeId: number, dashboardId: number) => `Dashboards/${dashboardId}/Nodes/${nodeId}`;
export const getOrderNodeDashboardsUrl = (nodeId: number) => `Dashboards/Order/Nodes/${nodeId}`;