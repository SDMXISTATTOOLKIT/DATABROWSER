import React, {useEffect, useState} from 'react';
import {Route, Switch} from "react-router";
import Node from "../components/node";
import {fetchNode, fetchNodeDatasetCsv} from "../state/node/nodeActions";
import {fetchCatalog} from "../state/catalog/catalogActions";
import Results from "../components/results";
import DatasetDomain from "./DatasetDomain";
import {connect} from "react-redux";
import DashboardsDomain from "./DashboardsDomain";
import {goToNode} from "../links";
import Call from "../hocs/call";

const mapStateToProps = state => ({
  hub: state.hub,
  node: state.node,
  catalog: state.catalog
});

const mapDispatchToProps = dispatch => ({
  fetchNode: nodeId => dispatch(fetchNode(nodeId)),
  fetchCatalog: nodeId => dispatch(fetchCatalog(nodeId)),
  onCsvFetch: (nodeId, datasetId, criteria, datasetTitle) => dispatch(fetchNodeDatasetCsv(nodeId, datasetId, criteria, datasetTitle))
});

const NodeDomain = ({hub, nodeCode, node, catalog, isDefault, fetchNode, fetchCatalog, onCsvFetch}) => {

  const [accessibleDataset, setAccessibleDataset] = useState(null);

  useEffect(() => {

    // TODO: check if catalog is null
    if (hub && (!node || nodeCode.toLowerCase() !== node.code.toLowerCase())) {
      const nodeHavingCode = hub.nodes.find(({code}) => code.toLowerCase() === nodeCode.toLowerCase());
      if (nodeHavingCode) {
        fetchNode(nodeHavingCode.nodeId);
        fetchCatalog(nodeHavingCode.nodeId);
      }
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [hub, nodeCode, node]);


  const Recursive = ({path, match}) =>
    <Switch>
      <Route
        exact
        path={match.url}
        render={({location}) => {

          const viewParam = new URLSearchParams(location.search).get("view");
          const a11yParam = new URLSearchParams(location.search).get("accessible");
          const datasetIdParam =  new URLSearchParams(location.search).get("datasetId");

          // short url for categorized dataset
          if (path.length === 1 && catalog.datasets[path[0]]) {
            return (
              <DatasetDomain
                hub={hub}
                node={node}
                nodeCode={nodeCode}
                isDefault={isDefault}
                catalog={catalog}
                categoryPath={[]}
                datasetId={path[0]}
                datasetTitle={catalog.datasets[path[0]].title}
                notes={catalog.datasets[path[0]]?.note}
                attachedFiles={catalog.datasets[path[0]]?.attachedDataFiles}
                referenceMetadataUrl={catalog.datasets[path[0]]?.referenceMetadata}
                viewId={viewParam || undefined}
                isAccessible={!!a11yParam}
              />
            );

            // short url for uncategorized dataset
          } else if (path.length === 1 && catalog.uncategorizedDatasets.find(({identifier}) => identifier === path[0])) {

            const dataset = catalog.uncategorizedDatasets.find(({identifier}) => identifier === path[0]);

            return (
              <DatasetDomain
                hub={hub}
                node={node}
                nodeCode={nodeCode}
                isDefault={isDefault}
                catalog={catalog}
                categoryPath={[]}
                datasetId={path[0]}
                datasetTitle={dataset.title}
                notes={dataset?.note}
                attachedFiles={dataset?.attachedDataFiles}
                referenceMetadataUrl={dataset?.referenceMetadata}
                viewId={viewParam || undefined}
                isAccessible={!!a11yParam}
              />
            );
          } else if (path.length > 0 && path[0] === "uncategorized" &&
            catalog.uncategorizedDatasets && catalog.uncategorizedDatasets.length > 0) {

            if (path.length === 1) {
              return (
                <Results
                  hub={hub}
                  node={node}
                  nodeCode={nodeCode}
                  isDefault={isDefault}
                  catalog={catalog}
                  categoryPath={path}
                  datasets={catalog.uncategorizedDatasets.map(ds => ({
                    ...ds,
                    categoryPath: path
                  }))}
                  isAccessible={!!a11yParam}
                  onAccessibleDatasetFetch={setAccessibleDataset}
                  scrollToDatasetId={datasetIdParam}
                />
              );

            } else if (path.length === 2) {

              const dataset = catalog.uncategorizedDatasets.find(({identifier}) => identifier === path[1]);

              if (dataset) {
                return (
                  <DatasetDomain
                    hub={hub}
                    node={node}
                    nodeCode={nodeCode}
                    isDefault={isDefault}
                    catalog={catalog}
                    categoryPath={path.slice(0, -1)}
                    datasetId={dataset.identifier}
                    datasetTitle={dataset.title}
                    notes={dataset?.note}
                    attachedFiles={dataset?.attachedDataFiles}
                    referenceMetadataUrl={dataset?.referenceMetadata}
                    isAccessible={!!a11yParam}
                  />
                );
              }

            }
          } else if (catalog.categoryGroups.length > 0) {

            const getDatasetsIdsAndSubcategoriesOrDatasetId = (categoryPath = [], categories = [], datasetsIds = []) => {

              if (categoryPath.length > 0) {

                if (categoryPath.length === 1) {

                  const datasetId = datasetsIds.find(id => id === categoryPath[0]);

                  if (datasetId) {
                    return datasetId;
                  } else {

                    const lastCategory = categories.find(({id}) => id === categoryPath[0]);
                    if (lastCategory) {
                      return {
                        datasetIds: lastCategory.datasetIdentifiers,
                        subCategories: lastCategory.categories || lastCategory.childrenCategories
                      }
                    }

                  }

                } else {
                  const nextCategory = categories.find(({id}) => id === categoryPath[0]);

                  if (nextCategory) {
                    return getDatasetsIdsAndSubcategoriesOrDatasetId(categoryPath.slice(1), nextCategory.categories || nextCategory.childrenCategories, nextCategory.datasetIdentifiers);
                  }
                }
              }

              return null;
            };

            const datasetsIdsOrDatasetId = getDatasetsIdsAndSubcategoriesOrDatasetId(
              path,
              catalog.categoryGroups.length === 1 ? catalog.categoryGroups[0].categories : catalog.categoryGroups,
              []
            );

            if (datasetsIdsOrDatasetId?.datasetIds) {
              return (
                <Results
                  hub={hub}
                  node={node}
                  nodeCode={nodeCode}
                  isDefault={isDefault}
                  catalog={catalog}
                  categoryPath={path}
                  datasets={datasetsIdsOrDatasetId.datasetIds.map(id => ({
                    ...catalog.datasets[id],
                    identifier: id,
                    categoryPath: path
                  }))}
                  subCategories={datasetsIdsOrDatasetId.subCategories}
                  isAccessible={!!a11yParam}
                  onAccessibleDatasetFetch={setAccessibleDataset}
                  scrollToDatasetId={datasetIdParam}
                />
              );

            } else if (datasetsIdsOrDatasetId) {
              return (
                <DatasetDomain
                  hub={hub}
                  node={node}
                  nodeCode={nodeCode}
                  isDefault={isDefault}
                  catalog={catalog}
                  categoryPath={path.slice(0, -1)}
                  datasetId={datasetsIdsOrDatasetId}
                  datasetTitle={catalog.datasets[datasetsIdsOrDatasetId].title}
                  notes={catalog.datasets[datasetsIdsOrDatasetId]?.note}
                  attachedFiles={catalog.datasets[datasetsIdsOrDatasetId]?.attachedDataFiles}
                  referenceMetadataUrl={catalog.datasets[datasetsIdsOrDatasetId]?.referenceMetadata}
                  isAccessible={!!a11yParam}
                />
              );
            } else {
              goToNode(nodeCode);
            }
          }

          goToNode(nodeCode);
        }}
      />
      <Route
        path={`${match.url}/:categoryId`}
        render={({match}) => <Recursive match={match} path={[...path, match.params.categoryId]}/>}
      />
    </Switch>;

  return (
    <div style={{width: "100%", height: "100%"}} id={node?.code ? ("node__" + node.code) : null}>
      <Switch>
        <Route
          exact
          path={isDefault ? ['/:lang/:nodeCode', '/:lang'] : '/:lang/:nodeCode'}
          render={props => (
            <Node
              {...props}
              nodeCode={nodeCode}
              hub={hub}
              node={node}
              catalog={catalog}
              isDefault={isDefault}
            />
          )}
        />
        <Route
          exact
          path={['/:lang/:nodeCode/dashboards', '/:nodeCode/dashboards']}
          render={() => (
            <DashboardsDomain
              nodeCode={nodeCode}
              isDefault={isDefault}
            />
          )}
        />
        {node && catalog && node.code.toLowerCase() === nodeCode.toLowerCase() && node.nodeId === catalog.nodeId && (
          <Route
            path='/:lang/:nodeCode/search'
            render={({location}) => {
              const datasetIdParam =  new URLSearchParams(location.search).get("datasetId");

              const query = new URLSearchParams(location.search).get("q");

              const filtersParams =
                (new URLSearchParams(location.search).get("c") || "")
                  .split("//").filter(str => str.length > 0).map(decodeURIComponent)
                  .map(filter => filter.split("/")).filter(path => path.length > 0);

              let filters = [];
              filtersParams.forEach(filter => filter.forEach(id => filters.push(id)));
              filters = [...new Set(filters)]; // remove duplicates

              if (query && query.length > 0) {

                const testDSForQuery = ds =>
                  (ds.title || "").toLowerCase().includes(query.toLowerCase()) ||
                  !!ds.keywords?.find(keyword => keyword.toLowerCase().includes(query.toLowerCase()));

                const testCatForFilters = cat => filters.length === 0 || filters.includes(cat.id);

                const dssForCount = [];   // satisfy only query (necessary to display count numbers)
                const dssForResults = []; // satisfy query and filters

                if (catalog.uncategorizedDatasets && catalog.uncategorizedDatasets.length > 0) {
                  catalog.uncategorizedDatasets.forEach(ds => {

                    ds.categoryPath = ["uncategorized"];

                    if (testDSForQuery(ds)) {
                      dssForCount.push(ds);
                      if (filters.length === 0 || filters.includes("uncategorized")) {
                        dssForResults.push(ds);
                      }
                    }
                  });
                }

                const searchRecursive = (categories, categoryPath, overrideCatSatisfyFilters = false) => {

                  categories.forEach(c => {

                    const catSatisfyFilters = overrideCatSatisfyFilters || testCatForFilters(c);

                    c.datasetIdentifiers.forEach(id => {
                      const ds = {
                        ...catalog.datasets[id],
                        identifier: id,
                        categoryPath: [...categoryPath, c.id]
                      };
                      if (testDSForQuery(ds)) {
                        dssForCount.push(ds);
                        if (catSatisfyFilters) {
                          dssForResults.push(ds);
                        }
                      }
                    });

                    searchRecursive(c.childrenCategories, [...categoryPath, c.id], catSatisfyFilters);
                  });
                };

                if (catalog.categoryGroups.length > 1) {
                  // with category schemes
                  catalog.categoryGroups.forEach(group => searchRecursive(group.categories, [group.id]));
                } else if (catalog.categoryGroups.length === 1) {
                  // no category schemes
                  searchRecursive(catalog.categoryGroups[0].categories, []);
                }

                const dssSatisfyQueryMap = {};
                dssForCount.forEach(ds => dssSatisfyQueryMap[ds.identifier] = ds);

                const recursiveCountAndFilterZero = tree => tree.map(node => {

                  const children = recursiveCountAndFilterZero(node.childrenCategories || node.categories).filter(({count}) => count > 0);

                  return ({
                    ...node,
                    childrenCategories: node.childrenCategories ? children : undefined,
                    categories: node.categories ? children : undefined,
                    count:
                      children.reduce((acc, node) => acc + node.count, 0) +
                      (node.datasetIdentifiers?.filter(dsId => dssSatisfyQueryMap[dsId] !== undefined).length || 0)
                  });
                }).filter(({count}) => count > 0);

                const categoryGroupsWithCount = recursiveCountAndFilterZero(catalog.categoryGroups);

                const uncategorizedDatasetsCount = catalog.uncategorizedDatasets.filter(({identifier}) => dssSatisfyQueryMap[identifier] !== undefined).length;

                // remove duplicates from results ( O(2n) )
                const resultsMap = {};
                dssForResults.forEach(ds => resultsMap[ds.identifier] = ds);

                const a11yParam = new URLSearchParams(location.search).get("accessible");

                return (
                  <Results
                    query={query}
                    filters={filtersParams}
                    filtered
                    hub={hub}
                    node={node}
                    nodeCode={nodeCode}
                    isDefault={isDefault}
                    catalog={catalog}
                    datasets={Object.keys(resultsMap).map(id => resultsMap[id])}
                    categoryGroupsWithCount={categoryGroupsWithCount}
                    uncategorizedDatasetsCount={uncategorizedDatasetsCount}
                    isAccessible={!!a11yParam}
                    onAccessibleDatasetFetch={setAccessibleDataset}
                    scrollToDatasetId={datasetIdParam}
                  />
                );
              } else {
                goToNode(nodeCode);
              }
            }}
          />
        )}
        {node && catalog && node.code.toLowerCase() === nodeCode.toLowerCase() && node.nodeId === catalog.nodeId && (
          <Route
            path='/:lang/:nodeCode/:path'
            render={({match}) => (
              <Recursive match={match} path={[match.params.path]}/>
            )}
          />
        )}
      </Switch>
      <Call
        cb={({nodeId, datasetId, criteria, datasetTitle}) => {
          onCsvFetch(nodeId, datasetId, criteria, datasetTitle);
          setAccessibleDataset(null);
        }}
        cbParam={{
          nodeId: node?.nodeId,
          datasetId: accessibleDataset?.identifier,
          criteria: {},
          datasetTitle: accessibleDataset?.title
        }}
        disabled={!accessibleDataset}
      >
        <span/>
      </Call>
    </div>
  )
};

export default connect(mapStateToProps, mapDispatchToProps)(NodeDomain);