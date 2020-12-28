import moment from "moment";
import {FREQ_DIMENSION_KEY, TIME_PERIOD_DIMENSION_KEY} from "./jsonStat";

export const CRITERIA_FILTER_TYPE_CODES = "CodeValues";
export const CRITERIA_FILTER_TYPE_STRING = "StringValues";
export const CRITERIA_FILTER_TYPE_RANGE = "TimeRange";
export const CRITERIA_FILTER_TYPE_PERIODS = "TimePeriod";

export const FREQ_ANNUAL = "A";
export const FREQ_SEMESTER = "S";
export const FREQ_QUARTERLY = "Q";
export const FREQ_MONTHLY = "M";
const frequencies = [FREQ_MONTHLY, FREQ_QUARTERLY, FREQ_SEMESTER, FREQ_ANNUAL];

export const getDimensionCriteria = (criteria, dimensionId) => (criteria || []).find(({id}) => id === dimensionId) || null;

const getMinAndMax = timePeriodCodelist => {
  let min = moment().add(-20, 'y').format("YYYY") + "-01-01";
  let max = moment().add(10, 'y').format("YYYY") + "-12-31";

  return {
    min: timePeriodCodelist.values && timePeriodCodelist.values[0] && timePeriodCodelist.values[0].id
      ? moment(timePeriodCodelist.values[0].id).format("YYYY-MM-DD")
      : min,
    max: timePeriodCodelist.values && timePeriodCodelist.values[1] && timePeriodCodelist.values[1].id
      ? moment(timePeriodCodelist.values[1].id).format("YYYY-MM-DD")
      : max,
    missingRange: (!timePeriodCodelist.values || timePeriodCodelist.values.length !== 2)
  }
};

export const getTimePeriod = (initialTimePeriod, criteria, timePeriodCodelist, freqCodelist) => {

  const minAndMax = timePeriodCodelist
    ? getMinAndMax(timePeriodCodelist)
    : {
      min: initialTimePeriod.minDate,
      max: initialTimePeriod.maxDate,
      missingRange: initialTimePeriod.missingRange,
    };

  let freq = initialTimePeriod.freq;
  if (freqCodelist !== null) {
    freq = (frequencies.find(freq => (freqCodelist.values || []).map(({id}) => id).includes(freq)) || null);
  }

  return {
    freq: freq,
    selectorType: (criteria?.[TIME_PERIOD_DIMENSION_KEY]?.type || CRITERIA_FILTER_TYPE_RANGE),
    minDate: minAndMax.min,
    maxDate: minAndMax.max,
    fromDate: (criteria?.[TIME_PERIOD_DIMENSION_KEY]?.from || minAndMax.min),
    toDate: (criteria?.[TIME_PERIOD_DIMENSION_KEY]?.to || minAndMax.max),
    periods: (criteria?.[TIME_PERIOD_DIMENSION_KEY]?.period || null),
    missingRange: minAndMax.missingRange
  }
};

export const getFreq = criteria => {
  const freqValues = (criteria && criteria[FREQ_DIMENSION_KEY])
    ? (criteria[FREQ_DIMENSION_KEY].filterValues || [])
    : [];
  return (frequencies.find(freq => freqValues.includes(freq)) || null)
};

export const getCriteriaArrayFromObject = criteriaObj => {
  const criteriaArr = [];
  Object.keys(criteriaObj).forEach(key => {
    if (criteriaObj[key] !== null && criteriaObj[key] !== undefined) {
      criteriaArr.push(criteriaObj[key])
    }
  });

  return criteriaArr;
};

export const getCriteriaObjectFromArray = criteriaArr => {
  let criteriaObj = {};
  (criteriaArr || []).forEach((dimensionCriteria) => {
    criteriaObj = {
      ...criteriaObj,
      [dimensionCriteria.id]: dimensionCriteria
    }
  });

  return criteriaObj;
};