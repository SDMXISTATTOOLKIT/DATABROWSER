import React, {useEffect, useState} from 'react';
import {compose} from "redux";
import {withStyles} from "@material-ui/core";
import {useTranslation} from "react-i18next";
import "./style.css";
import {numberFormatter} from "../../utils/formatters";
import FormControl from "@material-ui/core/FormControl";
import InputLabel from "@material-ui/core/InputLabel";
import MenuItem from "@material-ui/core/MenuItem";
import Select from "@material-ui/core/Select";
import Call from "../../hocs/call/index";
import {getDataIdxFromCoordinatesArray, getDimensionValuesIndexesMap} from "../../utils/jsonStat";
import {addSpinnerMessage, markSpinnerMessage} from "../../state/spinner/spinnerActions";
import {v4 as uuidv4} from "uuid";
import {connect} from "react-redux";
import CustomEmpty from "../custom-empty";
import {getTextWidth} from "../../utils/style";
import SanitizedHTML from "../sanitized-html";

const $ = window.jQuery;

const styles = theme => ({
  map: {
    position: "relative",
    width: "100%",
    height: "100%",
    zIndex: 0
  },
  detailLevelSelector: {
    position: "absolute",
    top: 0,
    left: 0,
    padding: 8,
    margin: 8,
    zIndex: 1,
    background: "rgba(255, 255, 255, 0.7)"
  },
  copyright: {
    position: "absolute",
    bottom: 0,
    right: 0,
    padding: "0 4px",
    zIndex: 1,
    fontSize: 12,
    background: "rgba(255, 255, 255, 0.7)"
  }
});

const mapStateToProps = state => ({
  mapConfig: state.appConfig?.map
});

const mapDispatchToProps = dispatch => ({
  addSpinner: (uuid, message) => dispatch(addSpinnerMessage(uuid, message)),
  markSpinner: (uuid, isError) => dispatch(markSpinnerMessage(uuid, isError))
});

export const MAP_ZOOM_CONTROL_POSITION_TOP_LEFT = "top-left-ol-zoom";
export const MAP_ZOOM_CONTROL_POSITION_TOP_RIGHT = "top-right-ol-zoom";
export const MAP_ZOOM_CONTROL_POSITION_BOTTOM_LEFT = "bottom-left-ol-zoom";
export const MAP_ZOOM_CONTROL_POSITION_BOTTOM_RIGHT = "bottom-right-ol-zoom";

const validMapZoomControlPositions = [
  MAP_ZOOM_CONTROL_POSITION_TOP_LEFT,
  MAP_ZOOM_CONTROL_POSITION_TOP_RIGHT,
  MAP_ZOOM_CONTROL_POSITION_BOTTOM_LEFT,
  MAP_ZOOM_CONTROL_POSITION_BOTTOM_RIGHT
];

const zoomControlPositionDefault = MAP_ZOOM_CONTROL_POSITION_BOTTOM_LEFT;

export const MAP_CLASSIFICATION_METHOD_DEFAULT = "quantile";
export const MAP_PALETTE_CARDINALITY_DEFAULT = 3;
export const MAP_PALETTE_COLOR_START_DEFAULT = "#eff3ff";
export const MAP_PALETTE_COLOR_END_DEFAULT = "#2171b5";
export const MAP_OPACITY_DEFAULT = 0.9;
export const MAP_IS_LEGEND_COLLAPSED_DEFAULT = true;

function Map(props) {
  const {
    mapConfig,
    classes,
    mapId = `map__${uuidv4()}`,
    jsonStat,
    layout,
    onGeometryFetch,
    geometries,
    geometryDetailLevels,
    hideSpinner = false,
    isFullscreen,
    addSpinner,
    markSpinner,
    detailLevel: externalDetailLevel,
    setDetailLevel: setExternalDetailLevel,
    classificationMethod: externalClassificationMethod,
    paletteStartColor: externalPaletteStartColor,
    paletteEndColor: externalPaletteEndColor,
    paletteCardinality: externalPaletteCardinality,
    opacity: externalOpacity,
    isLegendCollapsed: externalIsLegendCollapsed,
    setSettings,
    zoomControlPosition = zoomControlPositionDefault,
    disableWheelZoom = false,
    readOnly = false,
    isGeometryData = false,
    enableLog = false
  } = props;

  const {t} = useTranslation();

  const [isRendering, setIsRendering] = useState(true);

  const [detailLevel, setDetailLevel] = useState(null);

  const [filteredGeometries, setFilteredGeometries] = useState(null);

  const [layerOptions, setLayerOptions] = useState(null);

  const [classificationMethod, setClassificationMethod] = useState(MAP_CLASSIFICATION_METHOD_DEFAULT);
  const [paletteStartColor, setPaletteStartColor] = useState(MAP_PALETTE_COLOR_START_DEFAULT);
  const [paletteEndColor, setPaletteEndColor] = useState(MAP_PALETTE_COLOR_END_DEFAULT);
  const [paletteCardinality, setPaletteCardinality] = useState(MAP_PALETTE_CARDINALITY_DEFAULT);
  const [opacity, setOpacity] = useState(MAP_OPACITY_DEFAULT);
  const [isLegendCollapsed, setIsLegendCollapsed] = useState(MAP_IS_LEGEND_COLLAPSED_DEFAULT);

  const [spinnerUuid] = useState(uuidv4());

  // destroy map on component unmount
  useEffect(() => {
    return () => {
      if (window.LMap.isInitialized(mapId)) {
        window.LMap.destroyMap(mapId);
      }
    }
  }, [mapId]);

  // dispatch resize event to handle fullscreen event
  useEffect(() => {
    window.dispatchEvent(new Event('resize'));
  }, [isFullscreen]);

  // check for missing geometry ids
  useEffect(() => {
    if (geometries !== null && geometries !== undefined && enableLog) {

      const geometryMap = {};
      geometries.forEach(g => geometryMap[g.id] = g);

      const missingGeometryList = [];
      jsonStat.dimension[layout.territoryDim].category.index.forEach(geomId => {
        if (!geometryMap[geomId]) {
          const missingEntry = {id: geomId, label: jsonStat.dimension[layout.territoryDim].category.label[geomId]};
          missingGeometryList.push(missingEntry);
        }
      });

      console.log("missing ids", missingGeometryList);
    }
  }, [geometries, jsonStat, layout, enableLog]);

  // handle detail level
  useEffect(() => {
    if (geometryDetailLevels !== null && geometryDetailLevels !== undefined) {

      if (externalDetailLevel !== null && externalDetailLevel !== undefined && geometryDetailLevels.map(({level}) => level).includes(externalDetailLevel)) {
        setDetailLevel(prevDetailLevel => prevDetailLevel !== externalDetailLevel
          ? externalDetailLevel
          : prevDetailLevel
        );

      } else if (geometryDetailLevels && geometryDetailLevels?.length > 0) {
        const firstNotEmptyLevelIndex = geometryDetailLevels.findIndex(el => el);
        const newDetailLevel = geometryDetailLevels[firstNotEmptyLevelIndex].level;
        if (setExternalDetailLevel && newDetailLevel !== externalDetailLevel) {
          setExternalDetailLevel(newDetailLevel);
        } else {
          setDetailLevel(prevDetailLevel => prevDetailLevel !== newDetailLevel
            ? newDetailLevel
            : prevDetailLevel
          );
        }

      } else {
        const newDetailLevel = -1;
        if (setExternalDetailLevel && newDetailLevel !== externalDetailLevel) {
          setExternalDetailLevel(newDetailLevel);
        }
        setDetailLevel(prevDetailLevel => prevDetailLevel !== newDetailLevel
          ? newDetailLevel
          : prevDetailLevel
        );
      }
    }
  }, [mapId, geometryDetailLevels, externalDetailLevel, setExternalDetailLevel]);

  // calculate filtered geometries
  useEffect(() => {
    if (geometries !== null && geometries !== undefined && detailLevel !== null) {

      if (detailLevel >= 0) {

        const geometryMap = {};
        geometries.forEach(g => geometryMap[g.id] = g);

        const territoryDimName = layout?.territoryDim;
        const dimensionFilterValues = jsonStat.dimension[territoryDimName].category.index;
        const indexesMap = getDimensionValuesIndexesMap(jsonStat);

        const data = [];
        let levelsCounter = {};
        dimensionFilterValues.forEach(territoryDim => {
          const dimValueArray = jsonStat.id.map(dim => {
            if (dim === territoryDimName) {
              return territoryDim;
            } else {
              return layout.filtersValue[dim];
            }
          });
          const valueIdx = getDataIdxFromCoordinatesArray(
            dimValueArray.map((value, idx) => indexesMap[jsonStat.id[idx]][value]),
            jsonStat.size
          );
          const valueId = jsonStat.value[valueIdx];
          const geom = geometryMap[territoryDim];

          if (valueId !== null && valueId !== undefined && geom && geom.nutsLevel === +detailLevel) {
            data.push({
              "value": valueId,
              "identifier": geom.uniqueId,
              "geometry": geom.wkt,
              "label": jsonStat.dimension[territoryDimName].category.label[territoryDim]
            });
          }
          if (valueId && geom?.nutsLevel >= 0) {
            if (!levelsCounter[geom.nutsLevel]) {
              levelsCounter[geom.nutsLevel] = 0;
            }
            levelsCounter[geom.nutsLevel]++;
          }
        });

        setFilteredGeometries(data);

      } else {
        markSpinner(spinnerUuid);
        setIsRendering(false);
      }
    }
  }, [geometries, detailLevel, jsonStat, layout, spinnerUuid, markSpinner]);

  // calculate layer options
  useEffect(() => {
    if (filteredGeometries !== null) {

      if (filteredGeometries.length > 0) {

        if (!hideSpinner) {
          addSpinner(spinnerUuid, t("components.map.spinners.rendering"));
        } else {
          setIsRendering(true);
        }

        setLayerOptions({
          srid: "EPSG:4326",
          legendSettings: {
            classificationMethod: classificationMethod,
            paletteStartColor: paletteStartColor,
            paletteEndColor: paletteEndColor,
            paletteCardinality: paletteCardinality,
            opacity: opacity,
            isLegendCollapsed: isLegendCollapsed,
            pointStartRadius: 1,
            pointEndRadius: 9,
            pointRadiusCardinality: 9
          },
          onDataRender: function () {
            markSpinner(spinnerUuid);
            setIsRendering(false);
          },
          borderColor: [128, 128, 128, 0.5],
          valueFormatter: numberFormatter,
          centerMarker: isGeometryData
            ? "data:image/png;base64,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"
            : undefined,
          centerMarkerText: isGeometryData ? "test" : undefined,
          selectedCenterMarker: isGeometryData
            ? "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAADIklEQVR4AbXXA3hrWRQF4Ixt27Zt27atcGzbto3YeUZt2zbiNau+PVVO0uzv+9+r97o4UsVT6z7iUa1D66s9q26pc6+6td6lGpK0OuMF05BVaEs6jR6h949/1vzzdjrHz1tonJ+Qgc6hbbbUulbZUuuct8ZDdqCnKJsGCISTnjODAcCmY/yUTy/SzpurnSoGSbj5KZRGUYKSGECQTWdvrXcLIeSbVxIIkgFAdXQWDYWQDrAdrSCITnveiNPp1Bcs2NHgxKZqBzZ9ZMYg6bQjSV+9Tmx8Op1J93+1GD8vKcXiwkZYc5rwtqscZ7yzHFuy4ebTh3hi5H1wxRxgI1oqNj/nZQs+dxegsy8AsVp7/HjGWIRt9a7pQqTwEWxKMQc4mNonBXjehHctOQiEwpipBoNhqH/Pw2Zqhxigk46gmANcSMGJ5kZc+54LVS09mKsKG3tw4PMLGWJSgDBdIRPgZsKYU58z4pnfUxGKRDFXBcMR3PBVBjaZ+lLellCA9625iLV0f+YnHOBiCikDvPZvJqJRzFlR/tA9P+WIASJ0lUyAo6hbOe7v/GwhOvr8mKuau/048Y2l4ovYS8fLBNiGCgljznrJjH9TKjFXfbGoClvrXOLtL6MdZAKsSt8SlCPh6ndcWFbcNO2jiPCL1txm7P/sAnEEgH7iJLS6zDww9h74xSn4srcc+NJTgKL6TrT3+tHMCSi9uhPPmYqx11O+6eaAwNjz30rnkpqON6FFBGFCIiMuecOOGz704YTXFmO3J7zDjTcXr5xoOW1Oca2GNyknJNEJz5qwjVZoPFmY7qS4l+RNyJvAcrxEuPq47sJF1BtHgH66nMRVUDrEWvR1HAF+5G1fh+ZlT7gflUoEqKSDxKtPNMRd5I8hQJDu21ztEF+8hEOsSz/EEOB3Nt5A4sWTugt7Ud4sAYpof+Wkk4wQl1LXNAF66Gr5s4B8iNXoeQorAkTpVTZeXbz1yQqxMXfHfykCmGiz5DYXHsXZL5r2POFZc9b2ekfeZhrnvmye3AOqGGIXg1115NOWY3c12E/kIVV87smvTTRu1ca0yah4638QYUyY6yhczwAAAABJRU5ErkJggg=="
            : undefined,
          disableSettings: readOnly
        });

      } else {
        markSpinner(spinnerUuid);
        setIsRendering(false);
      }
    }
  }, [filteredGeometries, isGeometryData, classificationMethod, paletteStartColor, paletteEndColor, paletteCardinality, opacity, isLegendCollapsed, readOnly, hideSpinner, spinnerUuid, addSpinner, markSpinner, t]);

  // init map and add layer or update layer with filtered geometries
  useEffect(() => {
    if (mapId && filteredGeometries !== null && layerOptions !== null) {

      setTimeout(function () {
        const legendTitle = t("components.map.translation.legend");

        if (!window.LMap.isInitialized(mapId)) {

          const mapTranslations = {
            configure: t("components.map.translation.configure"),
            opacity: t("components.map.translation.opacity"),
            from: t("components.map.translation.from"),
            to: t("components.map.translation.to"),
            cancel: t("components.map.translation.cancel"),
            apply: t("components.map.translation.apply"),
            classification: t("components.map.translation.classification"),
            equalInterval: t("components.map.translation.equalInterval"),
            naturalBreaks: t("components.map.translation.naturalBreaks"),
            quantile: t("components.map.translation.quantile"),
            numberOfClasses: t("components.map.translation.numberOfClasses"),
            startColor: t("components.map.translation.startColor"),
            endColor: t("components.map.translation.endColor"),
            preview: t("components.map.translation.preview"),
            collapse: t("components.map.translation.collapse"),
            uncollapse: t("components.map.translation.uncollapse")
          };

          const mapOptions = {
            baseMap: mapConfig?.baseMap || "italy_regions",
            baseMapToGray: mapConfig?.baseMapToGray || false,
            zoomControlClass: validMapZoomControlPositions.includes(zoomControlPosition) ? zoomControlPosition : zoomControlPositionDefault,
            disableWheelZoom: disableWheelZoom,
            onLoadingStart: function () {
              if (enableLog) {
                console.log("Map creation - start");
              }
            },
            onLoadingFinish: function () {
              if (enableLog) {
                console.log("Map creation - done");
              }
            },
            onSettingsChange: function (settings) {
              const {
                classificationMethod: newClassificationMethod,
                paletteStartColor: newPaletteStartColor,
                paletteEndColor: newPaletteEndColor,
                paletteCardinality: newPaletteCardinality,
                opacity: newOpacity,
                // isLegendCollapsed: newIsLegendCollapsed
              } = settings;

              if (setSettings) {
                setSettings({
                  classificationMethod: newClassificationMethod,
                  paletteStartColor: newPaletteStartColor,
                  paletteEndColor: newPaletteEndColor,
                  paletteCardinality: newPaletteCardinality,
                  opacity: newOpacity,
                  // isLegendCollapsed: newIsLegendCollapsed // commentato per evitare doppia render espandendo/collassando la legenda
                });
              } else {
                setClassificationMethod(newClassificationMethod);
                setPaletteStartColor(newPaletteStartColor);
                setPaletteEndColor(newPaletteEndColor);
                setPaletteCardinality(newPaletteCardinality);
                setOpacity(newOpacity);
                // setIsLegendCollapsed(newIsLegendCollapsed); // commentato per evitare doppia render espandendo/collassando la legenda
              }
            },
            legendFontFamily: "Roboto"
          };

          window.LMap.initMap(mapId, mapOptions, mapTranslations);
          window.LMap.addLayer(mapId, filteredGeometries, legendTitle, layerOptions);

        } else {
          window.LMap.updateLayer(mapId, filteredGeometries, legendTitle, layerOptions);
        }
      }, 1);

      setLayerOptions(null);
    }
  }, [mapConfig, mapId, filteredGeometries, layerOptions, setSettings, disableWheelZoom, zoomControlPosition, enableLog, t]);

  // handle properties change
  useEffect(() => {
    if (externalClassificationMethod !== null && externalClassificationMethod !== undefined) {
      setClassificationMethod(externalClassificationMethod);
    }
    if (externalPaletteStartColor !== null && externalPaletteStartColor !== undefined) {
      setPaletteStartColor(externalPaletteStartColor);
    }
    if (externalPaletteEndColor !== null && externalPaletteEndColor !== undefined) {
      setPaletteEndColor(externalPaletteEndColor);
    }
    if (externalPaletteCardinality !== null && externalPaletteCardinality !== undefined) {
      setPaletteCardinality(externalPaletteCardinality);
    }
    if (externalOpacity !== null && externalOpacity !== undefined) {
      setOpacity(externalOpacity);
    }
    if (externalIsLegendCollapsed !== null && externalIsLegendCollapsed !== undefined) {
      setIsLegendCollapsed(externalIsLegendCollapsed);
    }
  }, [externalClassificationMethod, externalPaletteStartColor, externalPaletteEndColor, externalPaletteCardinality, externalOpacity, externalIsLegendCollapsed]);

  const isMapVisible = (!isRendering && filteredGeometries !== null && filteredGeometries.length > 0);

  return (layout?.territoryDim && jsonStat.dimension[layout.territoryDim])
    ? (
      <Call
        cb={onGeometryFetch}
        cbParam={[...jsonStat.dimension[layout.territoryDim].category.index]}
        disabled={geometries !== null && geometries !== undefined && geometryDetailLevels !== null && geometryDetailLevels !== undefined}
      >
        {geometryDetailLevels !== null && geometryDetailLevels !== undefined && geometryDetailLevels.length > 1 && detailLevel !== null && detailLevel >= 0 && (
          <div className={classes.detailLevelSelector}>
            {(() => {
              const getTextWidthEl = $('<span>').css({
                visibility: 'hidden',
                position: 'absolute',
                fontSize: 16
              }).appendTo('body').get(0);

              const label = t("components.map.detailLevel");
              const Component = (
                <FormControl>
                  <InputLabel>{label}</InputLabel>
                  <Select
                    value={detailLevel}
                    style={{minWidth: (getTextWidth(label, getTextWidthEl) + 16)}}
                    onChange={ev => setExternalDetailLevel
                      ? setExternalDetailLevel(ev.target.value)
                      : setDetailLevel(ev.target.value)
                    }
                    disabled={readOnly}
                  >
                    {geometryDetailLevels.map(val =>
                      <MenuItem value={val.level} key={val.level}>{val.label}</MenuItem>
                    )}
                  </Select>
                </FormControl>
              );

              $(getTextWidthEl).remove();

              return Component
            })()}
          </div>
        )}
        {(mapConfig?.copyright || "").length > 0 && (
          <div
            className={classes.copyright}
            style={{visibility: isMapVisible ? "visible" : "hidden"}}
          >
            <SanitizedHTML
              html={mapConfig.copyright}
              allowTarget
            />
          </div>
        )}
        {detailLevel === null
          ? <CustomEmpty text={t("components.map.spinners.loading") + "..."}/>
          : isRendering
            ? <CustomEmpty text={t("components.map.spinners.rendering") + "..."}/>
            : (filteredGeometries === null || filteredGeometries.length === 0)
              ? detailLevel >= 0
                ? (
                  <CustomEmpty
                    text={
                      t("components.map.noDataToDisplayFor") +
                      " " +
                      ((geometryDetailLevels || []).find(lev => lev.level === +detailLevel)?.label || "")
                    }
                  />
                )
                : <CustomEmpty text={t("components.map.noDataToDisplay")}/>
              : <span/>
        }
        <div
          id={mapId}
          className={classes.map}
          style={{visibility: isMapVisible ? "visible" : "hidden"}}
        />
      </Call>
    )
    : <CustomEmpty text={t("components.map.noTerritorialDim")}/>;
}

export default compose(
  withStyles(styles),
  connect(mapStateToProps, mapDispatchToProps)
)(Map);
