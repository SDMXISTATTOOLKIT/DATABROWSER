import {getNutsLevelTranslations} from "../constants/getNutsLevelTranslations";

export const showGenericError = () => {
  if (window && window.error) {
    window.error.show("An error occurred while contacting the server.");
  } else {
    console.error("An error occurred while contacting the server.");
  }
};

export const showTranslatedGenericErrorFactory = t => () => {
  if (window && window.error) {
    window.error.show(t("errors.generic"));
  } else {
    console.error(t("errors.generic"));
  }
};

export const getCombinationArrays = arrays => {
  if (!arrays || arrays.length === 0) {
    return [];
  }

  const ret = [];
  const max = arrays.length - 1;

  function helper(arr, i) {
    for (let j = 0, l = arrays[i].length; j < l; j++) {
      const a = arr.slice(0); // clone arr
      a.push(arrays[i][j]);
      if (i === max) {
        ret.push(a);
      } else {
        helper(a, i + 1);
      }
    }
  }

  helper([], 0);
  return ret;
};

export const getNthValorizedElementIndexInBooleanArray = (array, n) => {
  let count = 0;
  let res = 0;
  let lastIdx = 0;
  let found = false;

  array.forEach((el, idx) => {
    if (el === true) {
      count++;
      lastIdx = idx;
      if (count === n + 1) {
        res = idx;
        found = true;
      }
    }
  });

  return found ? res : (lastIdx + 1);
};

export const getGeometryDetailLevels = (geometries, hideSingleGeometry, t) => {
  return geometries
    .map(val => val.nutsLevel)
    .reduce((acc, level) => {
      if (!acc[level]) {
        // @ts-ignore
        const label = getNutsLevelTranslations(t)[level];
        acc[level] = {level: level, counter: 0, label: (label || `nutsLevel${level}`)};
      }
      acc[level].counter++;
      return acc;
    }, [])
    .filter(val => {
      if (!val) {
        return false
      }
      if (hideSingleGeometry === true && val.counter <= 1) {
        // console.log("hiding NUTS level " + val.level + " (size = " + val.counter + ")");
        return false
      } else {
        return true
      }
    })
    .sort((a, b) => a.level - b.level);
}