export const numberFormatter = (new Intl.NumberFormat()).format;

export const DECIMAL_SEPARATOR_DEFAULT = ",";
export const DECIMAL_PLACES_DEFAULT = 2;
export const TABLE_EMPTY_CHAR_DEFAULT = "";

const getFormattedInt = (value, separator) => {
  let str = value + "";
  let subStrs = [];

  while (str.length > 3) {
    subStrs.push(str.slice(-3));
    str = str.slice(0, -3)
    subStrs.push((separator || "."));
  }
  if (str.length > 0) {
    subStrs.push(str);
  } else {
    subStrs.pop()
  }
  subStrs.reverse();

  return subStrs.join("");
};

export const getFormattedValue = (value, decimalSeparator, decimalPlaces, emptyChar) => {

  if (value === null || value === undefined || (typeof value === "string" && value.length === 0)) {
    return (emptyChar || TABLE_EMPTY_CHAR_DEFAULT);
  }

  if (isNaN(value)) {
    return value;
  }

  const intSeparator = decimalSeparator === "." ? "," : ".";

  if (Number.isInteger(value)) {
    return getFormattedInt(value, intSeparator);
  }

  const int = Math.floor(value);

  if (decimalPlaces === 0 || DECIMAL_PLACES_DEFAULT === 0) {
    return getFormattedInt(int, intSeparator);
  }

  const decimal = (value - int).toFixed(decimalPlaces || DECIMAL_PLACES_DEFAULT).slice(2);

  return getFormattedInt(int, intSeparator) + (decimalSeparator || DECIMAL_SEPARATOR_DEFAULT) + decimal;
}