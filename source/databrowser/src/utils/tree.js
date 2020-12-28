import _ from "lodash"

export function getMaxTreeDepth(tree, childrenKey) {

  const recursive = node =>
    node
      ? 1 + Math.max(...(node[childrenKey] || []).map(recursive), 0)
      : 0;

  return tree
    ? Math.max(...tree.map(recursive), 0)
    : 0;
}

export function getNodesAtDepth(tree, childrenKey, depth) {

  const res = [];

  const recursive = (subTree, currDepth) =>
    subTree
      ? (
        subTree.map(node => {
          if (depth === currDepth) {
            res.push(_.cloneDeep(node));
          }
          if (node[childrenKey] && node[childrenKey].length) {
            recursive(node[childrenKey], currDepth + 1);
          }
          return null;
        })
      )
      : [];

  recursive(tree, 1);

  return res;
}

export function getTreeFromArray(arr, parentKey, childrenKey) {
  const mappedArr = {};
  const tree = [];

  // first map the nodes of the array to an object -> create a hash table.
  arr.forEach(el => {
    el[childrenKey] = [];
    mappedArr[el.id] = el;
  });

  arr.forEach(el => {
    if (el[parentKey]) {
      // if the element is not at the root level, add it to its parent array of children.
      mappedArr[el[parentKey]][childrenKey].push(el);
    } else {
      // if the element is at the root level, add it to first level elements array.
      tree.push(el);
    }
  });

  return tree;
}

export function getNode(tree, childrenKey, test) {

  if (!(tree !== null && tree.length)) return null;

  const foundNodes = tree.filter(test);

  if (foundNodes.length) {
    return foundNodes[0];
  } else {
    const foundNodesInChild =
      tree
        .filter(node => node[childrenKey])
        .map(node => getNode(node[childrenKey], childrenKey, test))
        .filter(result => result !== null);

    if (foundNodesInChild.length) {
      return foundNodesInChild[0];
    } else {
      return null;
    }
  }
}

export function getNodes(tree, childrenKey, test) {

  const res = [];

  const recursive = subTree =>
    subTree
      ? (
        subTree.map(node => {
          if (test && test(node)) {
            res.push(_.cloneDeep(node));
          }
          if (node[childrenKey] && node[childrenKey].length) {
            recursive(node[childrenKey]);
          }
          return null;
        })
      )
      : [];

  recursive(tree);

  return res;
}