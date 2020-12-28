export const getIndexesFromPaginationParams = (pageSize, pageNum) => ({
  startIdx: pageSize * (pageNum - 1),
  stopIdx: (pageSize * pageNum) - 1
});

export const getPaginationParamsFromIndexes = (startIdx, stopIdx, maxIdx) => {

  const minPageSize = stopIdx - startIdx + 1;
  const maxPageSize = maxIdx + 1;

  const pageNumForMinPageSize = (stopIdx + 1) / minPageSize;

  if (Number.isInteger(pageNumForMinPageSize)) {
    return ({
      pageSize: minPageSize,
      pageNum: pageNumForMinPageSize
    });
  } else {

    for (let pageSize = minPageSize + 1; pageSize <= maxPageSize; pageSize++) {

      const minPageNum = Math.ceil((stopIdx + 1) / pageSize);
      const maxPageNum = Math.floor((startIdx / pageSize) + 1);

      for (let pageNum = minPageNum; pageNum <= maxPageNum; pageNum++) {

        const indexes = getIndexesFromPaginationParams(pageSize, pageNum);

        if (indexes.startIdx <= startIdx && stopIdx <= indexes.stopIdx) {
          return ({
            pageSize,
            pageNum
          });
        }
      }
    }
  }
};
