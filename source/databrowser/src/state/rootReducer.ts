import {ReducersMapObject} from "redux";
import hubReducer from "./hub/hubReducer";
import nodeReducer from "./node/nodeReducer";
import catalogReducer from "./catalog/catalogReducer";
import datasetReducer from "./dataset/datasetReducer";
import appReducer from "./app/appReducer";
import hubConfigReducer from "./hubConfig/hubConfigReducer";
import spinnerReducer from "./spinner/spinnerReducer";
import configReducer from "./config/configReducer";
import nodesConfigReducer from "./nodesConfig/nodesConfigReducer";
import cacheReducer from "./cache/cacheReducer";
import otherConfigReducer from "./otherConfig/otherConfigReducer";
import nodeTemplatesConfigReducer from "./noteTemplatesConfig/nodeTemplatesConfigReducer";
import userReducer from './user/userReducer';
import usersConfigReducer from "./usersConfig/usersConfigReducer";
import dashboardReducer from "./dashboard/dashboardReducer";
import nodePermissionsConfigReducer from './nodePermissionsConfig/nodePermissionsConfigReducer';
import deleteConflictModalReducer from './delete-conflict-modal/deleteConflictModalReducer';
import appConfigReducer from './appConfig/appConfigReducer';

const rootReducer: ReducersMapObject = ({
  spinner: spinnerReducer,
  appConfig: appConfigReducer,
  config: configReducer,
  app: appReducer,
  hub: hubReducer,
  node: nodeReducer,
  nodeTemplatesConfig: nodeTemplatesConfigReducer,
  catalog: catalogReducer,
  dataset: datasetReducer,
  hubConfig: hubConfigReducer,
  nodesConfig: nodesConfigReducer,
  cacheConfig: cacheReducer,
  otherConfig: otherConfigReducer,
  user: userReducer,
  usersConfig: usersConfigReducer,
  dashboard: dashboardReducer,
  nodePermissionsConfig: nodePermissionsConfigReducer,
  deleteConflictModal: deleteConflictModalReducer
});

export default rootReducer;