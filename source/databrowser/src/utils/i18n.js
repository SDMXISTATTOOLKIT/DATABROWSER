import _ from "lodash";

export const validateI18nObj = obj =>
  obj !== undefined && obj !== null &&
  _(obj)
    .pickBy(val => val !== undefined && val !== null && val.length > 0)
    .keys()
    .value().length > 0;

export const localizeI18nObj = (obj, prefLang, langs) => {

  if (obj === null || obj === undefined || obj === {}) {
    return null
  }
  if (typeof (obj) === "string") {
    return obj
  }

  if (obj[prefLang]) {
    return obj[prefLang];
  }

  for (let code of langs) {
    if (obj[code]) {
      return obj[code];
    }
  }

  return obj[Object.keys(obj)[0]];
};

export const getI18nObjCustomFilterAndSearch = (language, languages) => (str, row, {field}) =>
  localizeI18nObj(row[field], language, languages).toLowerCase().includes(str.toLowerCase());