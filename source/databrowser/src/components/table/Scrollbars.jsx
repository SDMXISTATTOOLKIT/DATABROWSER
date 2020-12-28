import React, {useCallback, useEffect, useState} from 'react';
import withStyles from "@material-ui/core/styles/withStyles";
import {v4 as uuidv4} from 'uuid';
import Slider from "@material-ui/core/Slider";
import IconButton from "@material-ui/core/IconButton";
import ArrowDropUpIcon from '@material-ui/icons/ArrowDropUp';
import ArrowDropDownIcon from '@material-ui/icons/ArrowDropDown';
import ArrowLeftIcon from '@material-ui/icons/ArrowLeft';
import ArrowRightIcon from '@material-ui/icons/ArrowRight';
import CustomEmpty from "../custom-empty";
import CircularProgress from "@material-ui/core/CircularProgress";
import {useTranslation} from "react-i18next";

const $ = window.jQuery;

let timeoutID = null;
const SCROLLBARS_TIMEOUT = 100;
let intervalID = null;
const SCROLLBARS_INTERVAL = 200;

let isOnMouseDownOnSliderThumb = null;

const WHEEL_DELTA = 1;

export const SLIDER_WIDTH = 32;
const SLIDER_MARGIN = 4;

const SLIDER_RAIL_COLOR = "#cfcfcf";
const SLIDER_POINTER_COLOR = "#808080";

const styles = theme => ({
  root: {},
  sliderButton: {
    width: SLIDER_WIDTH,
    height: SLIDER_WIDTH,
    padding: 0,
    "& svg": {
      width: SLIDER_WIDTH,
      height: SLIDER_WIDTH,
    }
  },
  horizontalSlider: {
    color: SLIDER_POINTER_COLOR,
    "& span.MuiSlider-rail": {
      backgroundColor: SLIDER_RAIL_COLOR,
      opacity: 1
    },
    "& span.MuiSlider-track": {
      backgroundColor: SLIDER_RAIL_COLOR,
      opacity: 1
    }
  },
  verticalSlider: {
    color: SLIDER_POINTER_COLOR,
    "& span.MuiSlider-rail": {
      backgroundColor: SLIDER_RAIL_COLOR,
      opacity: 1
    },
    "& span.MuiSlider-track": {
      backgroundColor: SLIDER_RAIL_COLOR,
      opacity: 1
    }
  },
  spinnerOverlay: {
    width: "100%",
    height: "100%",
    position: "absolute",
    top: 0,
    left: 0
  }
});

const isEventInContainer = (ev, container) => {
  const containerRect = container.getBoundingClientRect();
  const BUFFER = 32;

  return (
    ev.clientY >= (containerRect.top - BUFFER) &&
    ev.clientY <= (containerRect.bottom + BUFFER) &&
    ev.clientX >= (containerRect.left - BUFFER) &&
    ev.clientX <= (containerRect.right + BUFFER)
  );
};

function CustomSlider(props) {
  const {
    uuid,
    classes,
    orientation,
    value,
    maxValue,
    onChange,
    onIncrement,
    ticks,
    isVerticalScrollbarVisible
  } = props;

  useEffect(() => {
    const onMouseUp = () => {
      clearInterval(intervalID);
      isOnMouseDownOnSliderThumb = null;
    }
    window.addEventListener("mouseup", onMouseUp);
    return () => window.removeEventListener("mouseup", onMouseUp);
  }, []);

  const handleChange = (value, ev, newValue, sliderId) => {
    const step = Math.ceil(ticks / 10);

    if (ev.type === "mousedown") {

      isOnMouseDownOnSliderThumb = isEventInContainer(ev, $(`#${sliderId} span.MuiSlider-thumb`)[0]);

      if (newValue < value) {
        if ((value - step) > newValue) {
          onIncrement(false, step, newValue);
          clearInterval(intervalID);
          intervalID = setInterval(
            () => onIncrement(false, step, newValue),
            SCROLLBARS_INTERVAL
          );
        } else {
          onChange(newValue);
        }
      } else {
        if ((value + step) < newValue) {
          onIncrement(true, step, newValue);
          clearInterval(intervalID);
          intervalID = setInterval(
            () => onIncrement(true, step, newValue),
            SCROLLBARS_INTERVAL
          );
        } else {
          onChange(newValue);
        }
      }

    } else if (ev.type === "mousemove" && isOnMouseDownOnSliderThumb) {
      onChange(newValue);
    }
  };

  return orientation === "vertical"
    ? (
      <div style={{width: SLIDER_WIDTH, height: "100%", display: "inline-block", verticalAlign: "top"}}>
        <IconButton
          className={classes.sliderButton}
          onMouseDown={() => {
            if (value > 0) {
              onIncrement(false, 1);
              intervalID = setInterval(
                () => onIncrement(false, 1),
                SCROLLBARS_INTERVAL
              );
            }
          }}
          disabled={value === 0}
        >
          <ArrowDropUpIcon/>
        </IconButton>
        <Slider
          id={`sliders__custom-sliders__vertical__${uuid}`}
          orientation="vertical"
          value={ticks - value}
          min={0}
          max={ticks}
          onChange={(ev, newValue) => {
            const invertedValue = (ticks - newValue);
            handleChange(value, ev, invertedValue, `sliders__custom-sliders__vertical__${uuid}`)
          }}
          track="inverted"
          className={classes.verticalSlider}
          style={{
            height: `calc(100% - ${SLIDER_WIDTH + SLIDER_WIDTH + SLIDER_MARGIN + SLIDER_MARGIN}px)`,
            padding: "0 15px",
            margin: `${SLIDER_MARGIN}px 0`
          }}
        />
        <IconButton
          className={classes.sliderButton}
          onMouseDown={() => {
            if (value < (maxValue - 1)) {
              onIncrement(true, 1);
              intervalID = setInterval(
                () => onIncrement(true, 1),
                SCROLLBARS_INTERVAL
              );
            }
          }}
          disabled={value >= (maxValue - 1)}
        >
          <ArrowDropDownIcon/>
        </IconButton>
      </div>
    )
    : (
      <div style={{width: "100%", height: SLIDER_WIDTH, display: "inline-block", verticalAlign: "middle"}}>
        <IconButton
          className={classes.sliderButton}
          style={{display: "inline-block", verticalAlign: "middle"}}
          onMouseDown={() => {
            onIncrement(false, 1);
            intervalID = setInterval(
              () => onIncrement(false, 1),
              SCROLLBARS_INTERVAL
            );
          }}
          disabled={value === 0}
        >
          <ArrowLeftIcon fontSize="small"/>
        </IconButton>
        <Slider
          id={`sliders__custom-sliders__horizontal__${uuid}`}
          orientation="horizontal"
          value={value}
          min={0}
          max={ticks}
          onChange={(ev, newValue) => handleChange(value, ev, newValue, `sliders__custom-sliders__horizontal__${uuid}`)}
          className={classes.horizontalSlider}
          style={{
            width: `calc(100% - ${SLIDER_WIDTH + SLIDER_WIDTH + SLIDER_MARGIN + SLIDER_MARGIN + (isVerticalScrollbarVisible ? SLIDER_WIDTH : 0)}px)`,
            padding: "15px 0",
            display: "inline-block",
            verticalAlign: "middle",
            margin: `0 ${SLIDER_MARGIN}px`
          }}
        />
        <IconButton
          className={classes.sliderButton}
          style={{display: "inline-block", verticalAlign: "middle"}}
          onMouseDown={() => {
            onIncrement(true, 1);
            intervalID = setInterval(
              () => onIncrement(true, 1),
              SCROLLBARS_INTERVAL
            );
          }}
          disabled={value >= (maxValue - 1)}
        >
          <ArrowRightIcon/>
        </IconButton>
        {isVerticalScrollbarVisible && (
          <div style={{width: SLIDER_WIDTH, height: SLIDER_WIDTH, display: "inline-block", verticalAlign: "middle"}}/>
        )}
      </div>
    )
}

function Scrollbars(props) {
  const {
    classes,
    children,
    verticalValue,
    verticalMaxValue,
    verticalTicks,
    onVerticalScroll,
    isVerticalScrollbarVisible,
    horizontalValue,
    horizontalMaxValue,
    horizontalTicks,
    onHorizontalScroll,
    isHorizontalScrollbarVisible,
    disableWheelZoom
  } = props;

  const {t} = useTranslation();

  const [uuid] = useState(uuidv4());

  const [row, setRow] = useState(verticalValue);
  const [col, setCol] = useState(horizontalValue);

  const [isPaginating, setIsPaginating] = useState(false);

  const handleSliderScroll = (value, isVertical) => {
    if (isVertical) {
      setRow(value);
      if (value !== row) {
        setIsPaginating(true);
      }
    } else {
      setCol(value);
      if (value !== col) {
        setIsPaginating(true);
      }
    }
  };

  const getIncrementedValue = (prevValue, delta, isIncrement, limit) => isIncrement
    ? (prevValue + delta) > limit
      ? limit
      : (prevValue + delta)
    : (prevValue - delta) < limit
      ? limit
      : (prevValue - delta);

  const handleIncrement = (isIncrement, isVertical, delta, limit) => isVertical
    ? setRow(prevRow => isIncrement
      ? getIncrementedValue(prevRow, delta, isIncrement, (limit || (verticalMaxValue - 1)))
      : getIncrementedValue(prevRow, delta, isIncrement, (limit || 0))
    )
    : setCol(prevCol => isIncrement
      ? getIncrementedValue(prevCol, delta, isIncrement, (limit || (horizontalMaxValue - 1)))
      : getIncrementedValue(prevCol, delta, isIncrement, (limit || 0))
    );

  const handleWheelScroll = useCallback(
    ev => {
      const isVertical = ev.wheelDeltaY !== 0;
      const wheelDelta = isVertical
        ? ev.wheelDeltaY > 0 ? -WHEEL_DELTA : WHEEL_DELTA
        : ev.wheelDeltaX > 0 ? -WHEEL_DELTA : WHEEL_DELTA

      const isBetween = (val, min, max) => (val >= min && val < max);

      if (isVertical) {
        setRow(prevRow => {
          const newRow = isBetween(prevRow + wheelDelta, 0, verticalMaxValue) ? (prevRow + wheelDelta) : prevRow
          return isVerticalScrollbarVisible
            ? newRow
            : prevRow
        });
      } else {
        setCol(prevCol => {
          const newCol = isBetween(prevCol + wheelDelta, 0, horizontalMaxValue) ? (prevCol + wheelDelta) : prevCol;
          return isHorizontalScrollbarVisible
            ? newCol
            : prevCol
        });
      }
    },
    [isVerticalScrollbarVisible, isHorizontalScrollbarVisible, verticalMaxValue, horizontalMaxValue, setRow, setCol],
  );

  useEffect(() => {
    if (!disableWheelZoom) {
      document.getElementById(`scrollbars-container__${uuid}`).addEventListener("wheel", handleWheelScroll);
    }
    return () => document.getElementById(`scrollbars-container__${uuid}`).removeEventListener("wheel", handleWheelScroll);
  }, [uuid, disableWheelZoom, handleWheelScroll]);

  useEffect(() => {
    if (timeoutID) {
      clearTimeout(timeoutID);
    }

    timeoutID = setTimeout(
      () => {
        onVerticalScroll(row);
        onHorizontalScroll(col);
        setIsPaginating(false);
      },
      SCROLLBARS_TIMEOUT
    );
  }, [row, col, onVerticalScroll, onHorizontalScroll, setIsPaginating]);

  useEffect(() => {
    setRow(verticalValue);
    setCol(horizontalValue);
  }, [verticalValue, horizontalValue, setRow, setCol]);

  return (
    <div id={`scrollbars-container__${uuid}`} style={{width: "100%", height: "100%"}}>
      <div style={{width: "100%", height: `calc(100% - ${isHorizontalScrollbarVisible ? SLIDER_WIDTH : 0}px)`}}>
        <div
          style={{
            width: `calc(100% - ${isVerticalScrollbarVisible ? SLIDER_WIDTH : 0}px)`,
            height: "100%",
            display: "inline-block",
            verticalAlign: "top",
            position: "relative"
          }}
        >
          <div className={classes.spinnerOverlay} style={{filter: isPaginating ? "blur(1px)" : ""}}>{children}</div>
          {isPaginating && (
            <div className={classes.spinnerOverlay}>
              <CustomEmpty
                text={t("components.table.scrollbars.loading") + "..."}
                textStyle={{fontWeight: "bold"}}
                color="#ffffff"
                backgroundColor="rgba(0, 0, 0, 0.1)"
                image={<CircularProgress style={{color: "#ffffff"}}/>}
              />
            </div>
          )}
        </div>
        {isVerticalScrollbarVisible && (
          <CustomSlider
            uuid={uuid}
            classes={classes}
            orientation="vertical"
            value={row}
            maxValue={verticalMaxValue}
            onChange={value => handleSliderScroll(value, true)}
            onIncrement={(isIncrement, delta, limit) => handleIncrement(isIncrement, true, delta, limit)}
            ticks={verticalTicks}
            isVerticalScrollbarVisible={isVerticalScrollbarVisible}
          />
        )}
      </div>
      {
        isHorizontalScrollbarVisible && (
          <CustomSlider
            uuid={uuid}
            classes={classes}
            orientation="horizontal"
            value={col}
            maxValue={horizontalMaxValue}
            onChange={value => handleSliderScroll(value, false)}
            onIncrement={(isIncrement, delta, limit) => handleIncrement(isIncrement, false, delta, limit)}
            ticks={horizontalTicks}
            isVerticalScrollbarVisible={isVerticalScrollbarVisible}
          />
        )
      }
    </div>
  )
}

export default withStyles(styles)(Scrollbars)