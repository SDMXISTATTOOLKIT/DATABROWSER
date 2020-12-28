import React, {Fragment} from 'react';
import {compose} from "redux";
import withStyles from "@material-ui/core/styles/withStyles";
import Grid from "@material-ui/core/Grid";
import Select from "@material-ui/core/Select";
import FormControl from "@material-ui/core/FormControl";
import InputLabel from "@material-ui/core/InputLabel";
import MenuItem from "@material-ui/core/MenuItem";
import Alert from "@material-ui/lab/Alert";
import {DatePicker, MuiPickersUtilsProvider} from '@material-ui/pickers';
import DateFnsUtils from '@date-io/date-fns';
import moment from "moment";
import _ from "lodash"
import {
  CRITERIA_FILTER_TYPE_PERIODS,
  CRITERIA_FILTER_TYPE_RANGE,
  FREQ_MONTHLY,
  FREQ_QUARTERLY,
  FREQ_SEMESTER
} from "../../utils/criteria";
import {withTranslation} from "react-i18next";
import Switch from '@material-ui/core/Switch';
import TextField from '@material-ui/core/TextField';

const styles = theme => ({
  root: {},
  selectContainer: {},
  formControl: {
    width: "100%"
  },
  timePeriodInput: {
    width: "100%"
  },
  alert: {
    marginTop: 8
  },
  typeSelectorContainer: {
    marginTop: 8
  },
  typeSelectorLabel: {
    height: 38,
    lineHeight: "38px"
  },
  typeSelectorSwitch: {
    "& > span.MuiSwitch-switchBase": {
      color: "#00295a"
    },
    "& > span.MuiSwitch-track": {
      background: "#00295a"
    }
  },
  lastPeriodsLabel: {
    height: 48,
    lineHeight: "48px"
  },
  lastPeriodsInput: {
    "& input": {
      height: 20,
      padding: 14
    }
  }
});

const periods = year => ({
  [FREQ_SEMESTER]: {
    values: ["S1", "S2"],
    S1: {
      start: `${year}-01-01`,
      end: moment(`${year}-06`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    S2: {
      start: `${year}-07-01`,
      end: moment(`${year}-12`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    }
  },
  [FREQ_QUARTERLY]: {
    values: ["Q1", "Q2", "Q3", "Q4"],
    Q1: {
      start: `${year}-01-01`,
      end: moment(`${year}-03`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    Q2: {
      start: `${year}-04-01`,
      end: moment(`${year}-06`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    Q3: {
      start: `${year}-07-01`,
      end: moment(`${year}-09`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    Q4: {
      start: `${year}-10-01`,
      end: moment(`${year}-12`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    }
  },
  [FREQ_MONTHLY]: {
    values: ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"],
    "01": {
      start: `${year}-01-01`,
      end: moment(`${year}-01`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "02": {
      start: `${year}-02-01`,
      end: moment(`${year}-02`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "03": {
      start: `${year}-03-01`,
      end: moment(`${year}-03`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "04": {
      start: `${year}-04-01`,
      end: moment(`${year}-04`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "05": {
      start: `${year}-05-01`,
      end: moment(`${year}-05`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "06": {
      start: `${year}-06-01`,
      end: moment(`${year}-06`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "07": {
      start: `${year}-07-01`,
      end: moment(`${year}-07`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "08": {
      start: `${year}-08-01`,
      end: moment(`${year}-08`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "09": {
      start: `${year}-09-01`,
      end: moment(`${year}-09`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "10": {
      start: `${year}-10-01`,
      end: moment(`${year}-10`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "11": {
      start: `${year}-11-01`,
      end: moment(`${year}-11`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    },
    "12": {
      start: `${year}-12-01`,
      end: moment(`${year}-12`, "YYYY-MM").endOf('month').format('YYYY-MM-DD')
    }
  }
});

const getYearsArray = (startYear, endYear) => {
  const res = [];

  if (startYear && endYear) {
    const startYearNumeric = Number(startYear);
    const endYearNumeric = Number(endYear);

    for (let i = startYearNumeric; i <= endYearNumeric; i++) {
      res.push(i + "");
    }
  }

  return res;
};

const getPeriod = (freq, date) => {
  if (freq && date) {
    const year = date.slice(0, 4);

    if (periods(year)[freq]) {
      const dateMoment = moment(date, "YYYY-MM-DD");

      return periods(year)[freq].values.find(period => {
        const start = moment(periods(year)[freq][period].start, "YYYY-MM-DD");
        const end = moment(periods(year)[freq][period].end, "YYYY-MM-DD");

        return dateMoment.isBetween(start, end, null, "[]");
      });
    }
  }

  return null;
};

const getNewTimePeriodFromYear = (timePeriod, date, year, minYear, maxYear, period, minPeriod, maxPeriod, isFrom) => {
  let newDate;

  if (isFrom) {
    if (year === minYear && period && minPeriod) {
      if (moment((year + period), "YYYY-MM-DD").isSameOrAfter(moment(timePeriod.minDate, "YYYY-MM-DD"))) {
        newDate = periods(year)[timePeriod.freq][period].start;
      } else {
        newDate = periods(year)[timePeriod.freq][minPeriod].start;
      }

    } else if (year === maxYear && period && maxPeriod) {
      if (moment((year + period), "YYYY-MM-DD").isSameOrBefore(moment(timePeriod.maxDate, "YYYY-MM-DD"))) {
        newDate = periods(year)[timePeriod.freq][period].start;
      } else {
        newDate = periods(year)[timePeriod.freq][maxPeriod].start;
      }

    } else {
      newDate = period
        ? periods(year)[timePeriod.freq][period].start
        : year + "-01-01";
    }

  } else {
    if (year === minYear && period && minPeriod) {
      if (period && moment((year + period), "YYYY-MM-DD").isSameOrAfter(moment(timePeriod.minDate, "YYYY-MM-DD"))) {
        newDate = periods(year)[timePeriod.freq][period].end;
      } else {
        newDate = periods(year)[timePeriod.freq][minPeriod].end;
      }

    } else if (year === maxYear && period && maxPeriod) {
      if (period && moment((year + period), "YYYY-MM-DD").isSameOrBefore(moment(timePeriod.maxDate, "YYYY-MM-DD"))) {
        newDate = periods(year)[timePeriod.freq][period].end;
      } else {
        newDate = periods(year)[timePeriod.freq][maxPeriod].end;
      }

    } else {
      newDate = period
        ? periods(year)[timePeriod.freq][period].end
        : year + "-12-31";
    }
  }

  return {
    ...timePeriod,
    fromDate: isFrom ? newDate : timePeriod.fromDate,
    toDate: !isFrom ? newDate : timePeriod.toDate,
  }
};

const handleSetTimePeriod = (newTimePeriod, onSetTimePeriod, setCriteriaValidity) => {
  onSetTimePeriod(newTimePeriod);
  setCriteriaValidity(moment(newTimePeriod.fromDate, "YYYY-MM-DD").isSameOrBefore(moment(newTimePeriod.toDate, "YYYY-MM-DD")));
};

const TimePeriod = ({
                      t,
                      classes,
                      timePeriod,
                      onSetTimePeriod,
                      isCriteriaValid,
                      setCriteriaValidity
                    }) => {

  const minYear = timePeriod.minDate ? moment(timePeriod.minDate).format("YYYY") : null;
  const maxYear = timePeriod.maxDate ? moment(timePeriod.maxDate).format("YYYY") : null;
  const fromYear = timePeriod.fromDate ? moment(timePeriod.fromDate).format("YYYY") : null;
  const toYear = timePeriod.toDate ? moment(timePeriod.toDate).format("YYYY") : null;

  const minPeriod = getPeriod(timePeriod.freq, timePeriod.minDate);
  const maxPeriod = getPeriod(timePeriod.freq, timePeriod.maxDate);
  const fromPeriod = getPeriod(timePeriod.freq, timePeriod.fromDate);
  const toPeriod = getPeriod(timePeriod.freq, timePeriod.toDate);

  const fromPeriods = periods(fromYear)[timePeriod.freq]
    ? periods(fromYear)[timePeriod.freq].values.slice(
      fromYear === minYear ? periods(fromYear)[timePeriod.freq].values.indexOf(minPeriod) : 0,
      fromYear === maxYear ? periods(fromYear)[timePeriod.freq].values.indexOf(maxPeriod) + 1 : undefined
    )
    : [];
  const toPeriods = periods(toYear)[timePeriod.freq]
    ? periods(toYear)[timePeriod.freq].values.slice(
      toYear === minYear ? periods(toYear)[timePeriod.freq].values.indexOf(minPeriod) : 0,
      toYear === maxYear ? periods(toYear)[timePeriod.freq].values.indexOf(maxPeriod) + 1 : undefined
    )
    : [];

  return (
    <Fragment>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          {timePeriod.freq !== null && !timePeriod.missingRange && (
            <Grid container spacing={2} justify="flex-end">
              <Grid item>
                <div className={classes.typeSelectorLabel}>
                  {t("components.criteria.timePeriod.typeSelector.selectRange")}
                </div>
              </Grid>
              <Grid item>
                <Switch
                  className={classes.typeSelectorSwitch}
                  color="default"
                  checked={timePeriod.selectorType === CRITERIA_FILTER_TYPE_PERIODS}
                  onChange={() => onSetTimePeriod({
                    ...timePeriod,
                    selectorType: timePeriod.selectorType === CRITERIA_FILTER_TYPE_PERIODS
                      ? CRITERIA_FILTER_TYPE_RANGE
                      : CRITERIA_FILTER_TYPE_PERIODS
                  })}
                />
              </Grid>
              <Grid item>
                <div className={classes.typeSelectorLabel}>
                  {t("components.criteria.timePeriod.typeSelector.selectLastPeriods")}
                </div>
              </Grid>
            </Grid>
          )}
        </Grid>
        <Grid item xs={12}>
          {timePeriod.selectorType === CRITERIA_FILTER_TYPE_RANGE
            ? timePeriod.freq !== null
              ? (
                <Fragment>
                  {!isCriteriaValid && (
                    <Grid item xs={12} className={classes.alert}>
                      <Alert severity="warning">{t("components.criteria.timePeriod.alert.notValidRange")}</Alert>
                    </Grid>
                  )}
                  <Grid container spacing={4}>
                    <Grid item xs={12}>
                      <Grid container spacing={2} className={classes.selectContainer}>
                        <Grid item xs={12}>
                          {t("components.criteria.timePeriod.start")}
                        </Grid>
                        <Grid item xs={periods(fromYear)[timePeriod.freq] !== undefined ? 6 : 12}>
                          {fromYear && minYear && toYear && (
                            <FormControl className={classes.formControl}>
                              <InputLabel>{t("components.criteria.timePeriod.year")}</InputLabel>
                              <Select
                                value={fromYear}
                                onChange={ev => {
                                  const year = ev.target.value;
                                  const newTimePeriod = getNewTimePeriodFromYear(timePeriod, timePeriod.fromDate, year, minYear, maxYear, fromPeriod, minPeriod, maxPeriod, true);
                                  handleSetTimePeriod(newTimePeriod, onSetTimePeriod, setCriteriaValidity);
                                }}
                              >
                                {getYearsArray(minYear, maxYear).map((year, idx) => (
                                  <MenuItem key={idx} value={year}>{year}</MenuItem>
                                ))}
                              </Select>
                            </FormControl>
                          )}
                        </Grid>
                        {fromYear && periods(fromYear)[timePeriod.freq] !== undefined && fromPeriods && fromPeriod && (
                          <Grid item xs={6}>
                            <FormControl className={classes.formControl}>
                              <InputLabel>{t("components.criteria.timePeriod.period")}</InputLabel>
                              <Select
                                value={fromPeriod}
                                onChange={ev => {
                                  const period = ev.target.value;
                                  const newTimePeriod = _.cloneDeep(timePeriod);
                                  newTimePeriod.fromDate = periods(fromYear)[timePeriod.freq][period].start;
                                  handleSetTimePeriod(newTimePeriod, onSetTimePeriod, setCriteriaValidity);
                                }}
                              >
                                {fromPeriods.map((period, idx) => (
                                  <MenuItem key={idx} value={period}>{period}</MenuItem>
                                ))}
                              </Select>
                            </FormControl>
                          </Grid>
                        )}
                      </Grid>
                    </Grid>
                    <Grid item xs={12}>
                      <Grid container spacing={2} className={classes.selectContainer}>
                        <Grid item xs={12}>
                          {t("components.criteria.timePeriod.end")}
                        </Grid>
                        <Grid item xs={periods(toYear)[timePeriod.freq] !== undefined ? 6 : 12}>
                          {toYear && fromYear && maxYear && (
                            <FormControl className={classes.formControl}>
                              <InputLabel>{t("components.criteria.timePeriod.year")}</InputLabel>
                              <Select
                                value={toYear}
                                onChange={ev => {
                                  const year = ev.target.value;
                                  const newTimePeriod = getNewTimePeriodFromYear(timePeriod, timePeriod.toDate, year, minYear, maxYear, toPeriod, minPeriod, maxPeriod, false);
                                  handleSetTimePeriod(newTimePeriod, onSetTimePeriod, setCriteriaValidity);
                                }}
                              >
                                {getYearsArray(minYear, maxYear).map((year, idx) => (
                                  <MenuItem key={idx} value={year}>{year}</MenuItem>
                                ))}
                              </Select>
                            </FormControl>
                          )}
                        </Grid>
                        {toYear && periods(toYear)[timePeriod.freq] !== undefined && toPeriods && toPeriod && (
                          <Grid item xs={6}>
                            <FormControl className={classes.formControl}>
                              <InputLabel>{t("components.criteria.timePeriod.period")}</InputLabel>
                              <Select
                                value={toPeriods.includes(toPeriod) ? toPeriod : toPeriods[toPeriods.length - 1]}
                                onChange={ev => {
                                  const period = ev.target.value;
                                  const newTimePeriod = _.cloneDeep(timePeriod);
                                  newTimePeriod.toDate = periods(toYear)[timePeriod.freq][period].end;
                                  handleSetTimePeriod(newTimePeriod, onSetTimePeriod, setCriteriaValidity);
                                }}
                              >
                                {toPeriods.map((period, idx) => (
                                  <MenuItem key={idx} value={period}>{period}</MenuItem>
                                ))}
                              </Select>
                            </FormControl>
                          </Grid>
                        )}
                      </Grid>
                    </Grid>
                  </Grid>
                </Fragment>
              )
              : (
                <Grid container spacing={3}>
                  <MuiPickersUtilsProvider utils={DateFnsUtils}>
                    <Grid item xs={6}>
                      <DatePicker
                        openTo="year"
                        format="yyyy-MM-dd"
                        label={t("components.criteria.timePeriod.from")}
                        views={["year", "month", "date"]}
                        clearable
                        minDate={moment(timePeriod.minDate)}
                        maxDate={moment(timePeriod.toDate)}
                        value={moment(timePeriod.fromDate)}
                        onChange={date => onSetTimePeriod({
                          ...timePeriod,
                          fromDate: date ? moment(date).format("YYYY-MM-DD") : timePeriod.minDate
                        })}
                        className={classes.timePeriodInput}
                      />
                    </Grid>
                    <Grid item xs={6}>
                      <DatePicker
                        openTo="year"
                        format="yyyy-MM-dd"
                        label={t("components.criteria.timePeriod.to")}
                        views={["year", "month", "date"]}
                        clearable
                        minDate={moment(timePeriod.fromDate)}
                        maxDate={moment(timePeriod.maxDate)}
                        value={moment(timePeriod.toDate)}
                        onChange={date => onSetTimePeriod({
                          ...timePeriod,
                          toDate: date ? moment(date).format("YYYY-MM-DD") : timePeriod.maxDate
                        })}
                        className={classes.timePeriodInput}
                      />
                    </Grid>
                  </MuiPickersUtilsProvider>
                </Grid>
              )
            : (
              <Grid container spacing={2} justify="center">
                <Grid item>
                  <div className={classes.lastPeriodsLabel}>
                    {t("components.criteria.timePeriod.selectLastPeriods.selectLast")}
                  </div>
                </Grid>
                <Grid item>
                  <TextField
                    value={timePeriod.periods || ""}
                    placeholder={t("components.criteria.timePeriod.selectLastPeriods.noPeriodsSelected")}
                    type="number"
                    variant="outlined"
                    className={classes.lastPeriodsInput}
                    style={{width: 400}}
                    onChange={({target}) => {
                      if (target.value === "") {
                        onSetTimePeriod({
                          ...timePeriod,
                          periods: null
                        })
                      } else if (!isNaN(target.value) && Number(target.value) > 0) {
                        onSetTimePeriod({
                          ...timePeriod,
                          periods: Number(target.value)
                        })
                      }
                    }}
                  />
                </Grid>
                <Grid item>
                  <div className={classes.lastPeriodsLabel}>
                    {t("components.criteria.timePeriod.selectLastPeriods.periods")}
                  </div>
                </Grid>
              </Grid>
            )
          }
        </Grid>
      </Grid>
    </Fragment>
  )
};

export default compose(
  withStyles(styles),
  withTranslation()
)(TimePeriod)