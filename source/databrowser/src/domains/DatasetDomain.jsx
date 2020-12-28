import React, {useEffect} from 'react';
import Data from "../components/data";
import {connect} from "react-redux";
import {
  enableDatasetStructureFetch,
  fetchDatasetStructure,
  resetDatasetState
} from "../state/dataset/datasetActions";
import Call from "../hocs/call";

const mapStateToProps = state => ({
  dataset: state.dataset,
  defaultLanguage: state.app.language,
  languages: state.app.languages
});

const mapDispatchToProps = dispatch => ({
  onFetchStructureEnable: (nodeCode, categoryPath, datasetId, viewId) =>
    dispatch(enableDatasetStructureFetch(nodeCode, categoryPath, datasetId, viewId)),
  fetchStructure: ({nodeId, datasetId, viewId, defaultLanguage, languages}) =>
    dispatch(fetchDatasetStructure(nodeId, datasetId, viewId, defaultLanguage, languages)),
  onDatasetReset: () => dispatch(resetDatasetState())
});

const DatasetDomain = ({
                         dataset,
                         defaultLanguage,
                         languages,
                         hub,
                         node,
                         nodeCode,
                         isDefault,
                         catalog,
                         categoryPath,
                         isAccessible,
                         datasetId,
                         datasetTitle,
                         notes,
                         attachedFiles,
                         referenceMetadataUrl,
                         viewId,
                         onFetchStructureEnable,
                         fetchStructure,
                         onDatasetReset
                       }) => {

  useEffect(() => {
    if (hub && node && catalog && node.nodeId === catalog.nodeId && node.code.toLowerCase() === nodeCode.toLowerCase()) {
      onFetchStructureEnable(nodeCode, categoryPath, datasetId, viewId);
    }
    return () => onDatasetReset();
  }, [onDatasetReset, onFetchStructureEnable, hub, node, catalog, nodeCode, categoryPath, datasetId, datasetTitle, viewId]);

  const isTheSameData = dataset !== null && (
    nodeCode === dataset.nodeCode &&
    (categoryPath || []).join() === (dataset.categoryPath || []).join() &&
    datasetId === dataset.datasetId &&
    (viewId || "") === (dataset?.viewId || "")
  );

  return (
    <Call
      cb={fetchStructure}
      cbParam={{nodeId: node ? node.nodeId : null, datasetId, viewId, defaultLanguage, languages}}
      disabled={!node || !node.nodeId || !datasetId || !dataset || dataset.isFetchStructureDisabled}
    >
      {isTheSameData && (
        <Data
          hub={hub}
          node={node}
          isDefault={isDefault}
          catalog={catalog}
          categoryPath={categoryPath}
          isAccessible={isAccessible}
          datasetId={datasetId}
          datasetTitle={datasetTitle}
          viewId={viewId}
          notes={notes}
          attachedFiles={attachedFiles}
          referenceMetadataUrl={referenceMetadataUrl}
        />
      )}
    </Call>
  );
};

export default connect(mapStateToProps, mapDispatchToProps)(DatasetDomain);