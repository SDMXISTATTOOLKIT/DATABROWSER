/* already ordered by server */
export const categorySorter = (a, b) => {
  const aOrderAnn = a.extras.find(({key}) => key === "CategoryScheme_node_order");
  const bOrderAnn = b.extras.find(({key}) => key === "CategoryScheme_node_order");

  if (aOrderAnn && bOrderAnn) {
    return Number(aOrderAnn.value) - Number(bOrderAnn.value);
  } else if (aOrderAnn && !bOrderAnn) {
    return -1;
  } else if (!aOrderAnn && bOrderAnn) {
    return 1;
  } else {
    return a.id.localeCompare(b.id);
  }
};