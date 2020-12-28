import React from 'react';
import NodeHeader from "../node-header";
import DataViewer from "../data-viewer";
import Page from "../page";
import {withStyles} from "@material-ui/core";

const styles = () => ({
  fullWidthContainer: {
    backgroundColor: "#f5f5f5",
    width: "100%",
    minHeight: "100%"
  }
});

const Data = ({
                classes,
                hub,
                node,
                isDefault,
                catalog,
                categoryPath,
                isAccessible,
                datasetId,
                datasetTitle,
                viewId,
                notes,
                attachedFiles,
                referenceMetadataUrl
              }) => {

  const nodeExtras = {};
  if (node?.extras) {
    node.extras.map(({key, value}) => nodeExtras[key] = JSON.parse(value));
  }

  const hubExtras = JSON.parse(hub.hub.extras || "{}");

  return (
    <Page>
      <NodeHeader
        hub={hub.hub}
        nodes={hub?.nodes}
        catalog={catalog}
        title={node?.name}
        node={node}
        nodeId={node?.nodeId}
        isDefault={isDefault}
        selectedCategoryPath={categoryPath}
        getCustomA11yPath={isA11y => {
          if (isA11y) {
            return `/${window.language}/${node?.code.toLowerCase()}/${categoryPath.join("/")}`;
          } else {
            return false;
          }
        }}
        getAdditionalA11yUrlParams={isA11y => {
          if (isA11y) {
            return {datasetId};
          } else {
            return false;
          }
        }}
      />
      <div style={{paddingTop: 64}} className={classes.fullWidthContainer}>
        <div style={{marginLeft: 16}}/>
        <DataViewer
          nodeId={node?.nodeId}
          nodeCode={node?.code}
          isAccessible={isAccessible}
          datasetId={datasetId}
          datasetTitle={datasetTitle}
          viewId={viewId}
          notes={notes}
          attachedFiles={attachedFiles}
          referenceMetadataUrl={referenceMetadataUrl}
          hubExtras={hubExtras}
          nodeExtras={nodeExtras}
          maxObservation={hub?.hub?.maxObservationsAfterCriteria}
        />
      </div>
    </Page>
  )
};

export default withStyles(styles)(Data);