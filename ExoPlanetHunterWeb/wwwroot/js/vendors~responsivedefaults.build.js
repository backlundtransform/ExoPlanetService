(window["webpackJsonp"] = window["webpackJsonp"] || []).push([[4],{

/***/ 840:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _charts_axes_AxisRenderer__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(841);
/* harmony import */ var _charts_axes_AxisRendererX__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(849);
/* harmony import */ var _charts_axes_AxisRendererY__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(855);
/* harmony import */ var _charts_axes_AxisRendererCircular__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(856);
/* harmony import */ var _charts_Chart__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(860);
/* harmony import */ var _charts_Legend__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(862);
/* harmony import */ var _charts_map_SmallMap__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(864);
/* harmony import */ var _charts_map_ZoomControl__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(865);
/**
 * Defines default Responsive rules
 * @hidden
 */








/**
 * ============================================================================
 * RULES
 * ============================================================================
 * @hidden
 */
/**
 * Default rules.
 *
 * @ignore Exclude from docs
 * @todo Do not create states for objects that do not have any overrides
 */
/* harmony default export */ __webpack_exports__["default"] = ([
    /**
     * --------------------------------------------------------------------------
     * Microcharts and sparklines
     * W<=100 || H<=100
     * @todo
     */
    {
        relevant: function (container) {
            if ((container.pixelWidth <= 100) || (container.pixelHeight <= 100)) {
                return true;
            }
            return false;
        },
        state: function (object, stateId) {
            // Put vertical axis labels inside
            if (object instanceof _charts_axes_AxisRenderer__WEBPACK_IMPORTED_MODULE_0__["AxisRenderer"]) {
                var state = object.states.create(stateId);
                state.properties.minLabelPosition = 1;
                state.properties.maxLabelPosition = 0;
                return state;
            }
        }
    },
    /**
     * --------------------------------------------------------------------------
     * Narrow
     * W<=200
     */
    {
        relevant: function (container) {
            if ((container.pixelWidth <= 200)) {
                return true;
            }
            return false;
        },
        state: function (object, stateId) {
            // Put vertical axis labels inside
            if (object instanceof _charts_axes_AxisRendererY__WEBPACK_IMPORTED_MODULE_2__["AxisRendererY"]) {
                var state = object.states.create(stateId);
                state.properties.inside = true;
                return state;
            }
            if (object instanceof _charts_axes_AxisRendererCircular__WEBPACK_IMPORTED_MODULE_3__["AxisRendererCircular"]) {
                var state = object.states.create(stateId);
                state.properties.inside = true;
                return state;
            }
            if (object instanceof _charts_map_SmallMap__WEBPACK_IMPORTED_MODULE_6__["SmallMap"]) {
                var state = object.states.create(stateId);
                state.properties.disabled = true;
                return state;
            }
            /*if (object instanceof Container && object.parent instanceof ZoomControl && !(object instanceof Button)) {
                let state = object.states.create(stateId);
                state.properties.height = 0;
                return state;
            }*/
            if (object instanceof _charts_map_ZoomControl__WEBPACK_IMPORTED_MODULE_7__["ZoomControl"]) {
                var state = object.states.create(stateId);
                state.properties.layout = "vertical";
                return state;
            }
            if (object instanceof _charts_Chart__WEBPACK_IMPORTED_MODULE_4__["Chart"]) {
                var state = object.states.create(stateId);
                state.properties.marginLeft = 0;
                state.properties.marginRight = 0;
                return state;
            }
            if (object instanceof _charts_Legend__WEBPACK_IMPORTED_MODULE_5__["Legend"] && (object.position == "left" || object.position == "right")) {
                var state = object.states.create(stateId);
                state.properties.position = "bottom";
                return state;
            }
        }
    },
    /**
     * --------------------------------------------------------------------------
     * Short
     * H<=200
     */
    {
        relevant: function (container) {
            if ((container.pixelHeight <= 200)) {
                return true;
            }
            return false;
        },
        state: function (object, stateId) {
            // Put vertical axis labels inside
            if (object instanceof _charts_axes_AxisRendererX__WEBPACK_IMPORTED_MODULE_1__["AxisRendererX"]) {
                var state = object.states.create(stateId);
                state.properties.inside = true;
                return state;
            }
            if (object instanceof _charts_axes_AxisRendererCircular__WEBPACK_IMPORTED_MODULE_3__["AxisRendererCircular"]) {
                var state = object.states.create(stateId);
                state.properties.inside = true;
                return state;
            }
            if (object instanceof _charts_map_SmallMap__WEBPACK_IMPORTED_MODULE_6__["SmallMap"]) {
                var state = object.states.create(stateId);
                state.properties.disabled = true;
                return state;
            }
            /*if (object instanceof Container && object.parent instanceof ZoomControl && !(object instanceof Button)) {
                let state = object.states.create(stateId);
                state.properties.width = 100;
                return state;
            }*/
            if (object instanceof _charts_map_ZoomControl__WEBPACK_IMPORTED_MODULE_7__["ZoomControl"]) {
                var state = object.states.create(stateId);
                state.properties.layout = "horizontal";
                return state;
            }
            if (object instanceof _charts_Chart__WEBPACK_IMPORTED_MODULE_4__["Chart"]) {
                var state = object.states.create(stateId);
                state.properties.marginTop = 0;
                state.properties.marginBottom = 0;
                return state;
            }
            if (object instanceof _charts_Legend__WEBPACK_IMPORTED_MODULE_5__["Legend"] && (object.position == "bottom" || object.position == "top")) {
                var state = object.states.create(stateId);
                state.properties.position = "right";
                return state;
            }
        }
    },
    /**
     * --------------------------------------------------------------------------
     * Super-small
     * W<=200 && H<=200
     */
    {
        relevant: function (container) {
            if ((container.pixelWidth <= 200) && (container.pixelHeight <= 200)) {
                return true;
            }
            return false;
        },
        state: function (object, stateId) {
            // Hide legend
            if (object instanceof _charts_Legend__WEBPACK_IMPORTED_MODULE_5__["Legend"]) {
                var state = object.states.create(stateId);
                state.properties.disabled = true;
                return state;
            }
            if (object instanceof _charts_map_ZoomControl__WEBPACK_IMPORTED_MODULE_7__["ZoomControl"]) {
                var state = object.states.create(stateId);
                state.properties.disabled = true;
                return state;
            }
        }
    }
]);
//# sourceMappingURL=ResponsiveDefaults.js.map

/***/ }),

/***/ 864:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "SmallMap", function() { return SmallMap; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(754);
/* harmony import */ var _core_Container__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(766);
/* harmony import */ var _core_elements_Rectangle__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(832);
/* harmony import */ var _core_utils_List__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(770);
/* harmony import */ var _core_utils_Disposer__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(755);
/* harmony import */ var _core_Registry__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(752);
/* harmony import */ var _core_utils_Color__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(775);
/* harmony import */ var _core_utils_InterfaceColorSet__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(801);
/* harmony import */ var _core_utils_Utils__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(796);
/* harmony import */ var _core_utils_Type__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(758);
/**
 * A module for the mini-map control.
 */

/**
 * ============================================================================
 * IMPORTS
 * ============================================================================
 * @hidden
 */









/**
 * ============================================================================
 * MAIN CLASS
 * ============================================================================
 * @hidden
 */
/**
 * Creates a "bird's eye" view of the whole map.
 *
 * This control creates a mini-map with the whole of the map, highlighting
 * the area which is in the current viewport of the map map.
 *
 * @see {@link ISmallMapEvents} for a list of available events
 * @see {@link ISmallMapAdapters} for a list of available Adapters
 * @important
 */
var SmallMap = /** @class */ (function (_super) {
    tslib__WEBPACK_IMPORTED_MODULE_0__["__extends"](SmallMap, _super);
    /**
     * Constructor
     */
    function SmallMap() {
        var _this = 
        // Init
        _super.call(this) || this;
        /**
         * A target map.
         *
         * @type {MutableValueDisposer<MapChart>}
         */
        _this._chart = new _core_utils_Disposer__WEBPACK_IMPORTED_MODULE_4__["MutableValueDisposer"]();
        _this.className = "SmallMap";
        // Set defaults
        _this.align = "left";
        _this.valign = "bottom";
        _this.percentHeight = 20;
        _this.percentWidth = 20;
        _this.margin(5, 5, 5, 5);
        var interfaceColors = new _core_utils_InterfaceColorSet__WEBPACK_IMPORTED_MODULE_7__["InterfaceColorSet"]();
        // Set background defailts
        _this.background.fillOpacity = 0.9;
        _this.background.fill = interfaceColors.getFor("background");
        // Set up events
        _this.events.on("hit", _this.moveToPosition, _this, false);
        _this.events.on("maxsizechanged", _this.updateMapSize, _this, false);
        // Create a container
        _this.seriesContainer = _this.createChild(_core_Container__WEBPACK_IMPORTED_MODULE_1__["Container"]);
        _this.seriesContainer.shouldClone = false;
        // Create an outline rectangle
        var rectangle = _this.createChild(_core_elements_Rectangle__WEBPACK_IMPORTED_MODULE_2__["Rectangle"]);
        rectangle.shouldClone = false;
        rectangle.stroke = interfaceColors.getFor("alternativeBackground");
        rectangle.strokeWidth = 1;
        rectangle.strokeOpacity = 0.5;
        rectangle.fill = Object(_core_utils_Color__WEBPACK_IMPORTED_MODULE_6__["color"])(); //"none";
        rectangle.verticalCenter = "middle";
        rectangle.horizontalCenter = "middle";
        rectangle.isMeasured = false;
        _this.rectangle = rectangle;
        _this._disposers.push(_this._chart);
        // Apply theme
        _this.applyTheme();
        return _this;
    }
    Object.defineProperty(SmallMap.prototype, "series", {
        /**
         * A list of map series used to draw the mini-map.
         *
         * @readonly
         * @return {List<MapSeries>} Series
         */
        get: function () {
            if (!this._series) {
                this._series = new _core_utils_List__WEBPACK_IMPORTED_MODULE_3__["List"]();
                this._series.events.on("inserted", this.handleSeriesAdded, this, false);
                this._series.events.on("removed", this.handleSeriesRemoved, this, false);
            }
            return this._series;
        },
        enumerable: true,
        configurable: true
    });
    /**
     * Decorates a new series when they are pushed into a `series` list.
     *
     * @param {IListEvents<MapSeries>["inserted"]} event Event
     */
    SmallMap.prototype.handleSeriesAdded = function (event) {
        var series = event.newValue;
        if (this.chart.series.contains(series)) {
            var newSeries = series.clone();
            this._series.removeValue(series);
            this._series.push(newSeries);
            series = newSeries;
            this.chart.dataUsers.push(newSeries);
        }
        series.chart = this.chart;
        series.parent = this.seriesContainer;
        series.interactionsEnabled = false;
    };
    /**
     * Cleans up after series are removed from Scrollbar.
     *
     * @param {IListEvents<XYSeries>["removed"]}  event  Event
     */
    SmallMap.prototype.handleSeriesRemoved = function (event) {
        //let sourceSeries: MapSeries = event.oldValue;
        this.invalidate();
    };
    /**
     * Moves main map pan position after click on the small map.
     *
     * @ignore Exclude from docs
     * @param {AMEvent<Sprite, ISpriteEvents>["hit"]}  event  Event
     */
    SmallMap.prototype.moveToPosition = function (event) {
        var svgPoint = event.svgPoint;
        var rectPoint = _core_utils_Utils__WEBPACK_IMPORTED_MODULE_8__["svgPointToSprite"](svgPoint, this.rectangle);
        var zoomLevel = this.chart.zoomLevel;
        var scale = Math.min(this.percentWidth, this.percentHeight) / 100;
        var x = (rectPoint.x + this.rectangle.pixelWidth / 2) / scale * zoomLevel;
        var y = (rectPoint.y + this.rectangle.pixelHeight / 2) / scale * zoomLevel;
        var geoPoint = this.chart.svgPointToGeo({ x: x, y: y });
        this.chart.zoomToGeoPoint(geoPoint, this.chart.zoomLevel, true);
    };
    Object.defineProperty(SmallMap.prototype, "chart", {
        /**
         * @return {MapChart} Chart/map
         */
        get: function () {
            return this._chart.get();
        },
        /**
         * A chart/map that this control is meant for.
         *
         * @param {MapChart}  chart  Chart/map
         */
        set: function (chart) {
            if (this.chart != chart) {
                this._chart.set(chart, new _core_utils_Disposer__WEBPACK_IMPORTED_MODULE_4__["MultiDisposer"]([
                    //chart.events.on("zoomlevelchanged", this.updateRectangle, this, false),
                    chart.events.on("mappositionchanged", this.updateRectangle, this, false),
                    chart.events.on("scaleratiochanged", this.updateMapSize, this, false)
                ]));
            }
        },
        enumerable: true,
        configurable: true
    });
    /**
     * Updates the viewport recangle as per current map zoom/pan position.
     *
     * @ignore Exclude from docs
     */
    SmallMap.prototype.updateRectangle = function () {
        var chart = this.chart;
        var zoomLevel = chart.zoomLevel;
        var rectangle = this.rectangle;
        rectangle.width = this.pixelWidth / zoomLevel;
        rectangle.height = this.pixelHeight / zoomLevel;
        var scale = Math.min(this.percentWidth, this.percentHeight) / 100;
        var seriesContainer = chart.seriesContainer;
        rectangle.x = Math.ceil((zoomLevel * seriesContainer.pixelWidth / 2 - seriesContainer.pixelX) * scale / zoomLevel + rectangle.pixelWidth / 2);
        rectangle.y = Math.ceil((zoomLevel * seriesContainer.pixelHeight / 2 - seriesContainer.pixelY) * scale / zoomLevel + rectangle.pixelHeight / 2);
        rectangle.validate();
    };
    /**
     * Update map size so that internal elements can redraw themselves after
     * the size of the small map changes.
     *
     * @ignore Exclude from docs
     */
    SmallMap.prototype.updateMapSize = function () {
        if (this.chart) {
            this.seriesContainer.scale = this.chart.scaleRatio * Math.min(this.percentWidth, this.percentHeight) / 100;
            this.updateRectangle();
            this.afterDraw();
        }
    };
    /**
     * Update elements after drawing the small map.
     */
    SmallMap.prototype.afterDraw = function () {
        _super.prototype.afterDraw.call(this);
        this.seriesContainer.moveTo({ x: this.pixelWidth / 2, y: this.pixelHeight / 2 });
        this.rectangle.maskRectangle = { x: -1, y: -1, width: Math.ceil(this.pixelWidth + 2), height: Math.ceil(this.pixelHeight + 2) };
    };
    /**
     * Processes JSON-based config before it is applied to the object.
     *
     * @ignore Exclude from docs
     * @param {object}  config  Config
     */
    SmallMap.prototype.processConfig = function (config) {
        if (config) {
            // Set up series
            if (_core_utils_Type__WEBPACK_IMPORTED_MODULE_9__["hasValue"](config.series) && _core_utils_Type__WEBPACK_IMPORTED_MODULE_9__["isArray"](config.series)) {
                for (var i = 0, len = config.series.length; i < len; i++) {
                    var series = config.series[i];
                    if (_core_utils_Type__WEBPACK_IMPORTED_MODULE_9__["hasValue"](series) && _core_utils_Type__WEBPACK_IMPORTED_MODULE_9__["isString"](series) && this.map.hasKey(series)) {
                        config.series[i] = this.map.getKey(series);
                    }
                }
            }
        }
        _super.prototype.processConfig.call(this, config);
    };
    return SmallMap;
}(_core_Container__WEBPACK_IMPORTED_MODULE_1__["Container"]));

/**
 * Register class in system, so that it can be instantiated using its name from
 * anywhere.
 *
 * @ignore
 */
_core_Registry__WEBPACK_IMPORTED_MODULE_5__["registry"].registeredClasses["SmallMap"] = SmallMap;
//# sourceMappingURL=SmallMap.js.map

/***/ }),

/***/ 865:
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "ZoomControl", function() { return ZoomControl; });
/* harmony import */ var tslib__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(754);
/* harmony import */ var _core_Container__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(766);
/* harmony import */ var _core_elements_Button__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(866);
/* harmony import */ var _core_elements_RoundedRectangle__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(863);
/* harmony import */ var _core_utils_Disposer__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(755);
/* harmony import */ var _core_utils_Keyboard__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(793);
/* harmony import */ var _core_interaction_Interaction__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(785);
/* harmony import */ var _core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(759);
/* harmony import */ var _core_Registry__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(752);
/* harmony import */ var _core_utils_InterfaceColorSet__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(801);
/**
 * Zoom control module
 */

/**
 * ============================================================================
 * IMPORTS
 * ============================================================================
 * @hidden
 */









/**
 * ============================================================================
 * MAIN CLASS
 * ============================================================================
 * @hidden
 */
/**
 * Creates a control for zooming the map.
 *
 * @see {@link IZoomControlEvents} for a list of available events
 * @see {@link IZoomControlAdapters} for a list of available Adapters
 * @important
 */
var ZoomControl = /** @class */ (function (_super) {
    tslib__WEBPACK_IMPORTED_MODULE_0__["__extends"](ZoomControl, _super);
    /**
     * Constructor
     */
    function ZoomControl() {
        var _this = _super.call(this) || this;
        /**
         * A target map.
         *
         * @type {MutableValueDisposer<MapChart>}
         */
        _this._chart = new _core_utils_Disposer__WEBPACK_IMPORTED_MODULE_4__["MutableValueDisposer"]();
        _this.className = "ZoomControl";
        _this.align = "right";
        _this.valign = "bottom";
        _this.layout = "vertical";
        _this.padding(5, 5, 5, 5);
        var interfaceColors = new _core_utils_InterfaceColorSet__WEBPACK_IMPORTED_MODULE_9__["InterfaceColorSet"]();
        var plusButton = _this.createChild(_core_elements_Button__WEBPACK_IMPORTED_MODULE_2__["Button"]);
        plusButton.shouldClone = false;
        plusButton.label.text = "+";
        plusButton.width = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
        plusButton.padding(5, 5, 5, 5);
        //plusButton.fontFamily = "Verdana";
        _this.plusButton = plusButton;
        var slider = _this.createChild(_core_Container__WEBPACK_IMPORTED_MODULE_1__["Container"]);
        slider.shouldClone = false;
        slider.width = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
        slider.background.fill = interfaceColors.getFor("alternativeBackground");
        slider.background.fillOpacity = 0.05;
        slider.background.events.on("hit", _this.handleBackgroundClick, _this, false);
        slider.events.on("sizechanged", _this.updateThumbSize, _this, false);
        _this.slider = slider;
        var thumb = slider.createChild(_core_elements_Button__WEBPACK_IMPORTED_MODULE_2__["Button"]);
        thumb.shouldClone = false;
        thumb.padding(0, 0, 0, 0);
        thumb.draggable = true;
        thumb.events.on("drag", _this.handleThumbDrag, _this, false);
        _this.thumb = thumb;
        var minusButton = _this.createChild(_core_elements_Button__WEBPACK_IMPORTED_MODULE_2__["Button"]);
        minusButton.shouldClone = false;
        minusButton.label.text = "-";
        minusButton.padding(5, 5, 5, 5);
        //minusButton.fontFamily = "Verdana";
        _this.minusButton = minusButton;
        // Set roles
        _this.thumb.role = "slider";
        _this.thumb.readerLive = "polite";
        // Set reader text
        _this.thumb.readerTitle = _this.language.translate("Use arrow keys to zoom in and out");
        _this.minusButton.readerTitle = _this.language.translate("Press ENTER to zoom in");
        _this.plusButton.readerTitle = _this.language.translate("Press ENTER to zoom out");
        _this.applyTheme();
        _this.events.on("propertychanged", function (event) {
            if (event.property == "layout") {
                _this.fixLayout();
            }
        }, undefined, false);
        _this._disposers.push(_this._chart);
        _this.fixLayout();
        return _this;
    }
    ZoomControl.prototype.fixLayout = function () {
        if (this.layout == "vertical") {
            this.width = 40;
            this.height = undefined;
            this.minusButton.width = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
            this.thumb.width = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
            this.plusButton.width = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
            this.slider.width = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
            this.minusButton.marginTop = 1;
            this.plusButton.marginBottom = 2;
            this.slider.height = 0;
            this.minusButton.toFront();
            this.plusButton.toBack();
            this.thumb.minX = 0;
            this.thumb.maxX = 0;
            this.thumb.minY = 0;
        }
        else if (this.layout == "horizontal") {
            this.thumb.minX = 0;
            this.thumb.minY = 0;
            this.thumb.maxY = 0;
            this.height = 40;
            this.width = undefined;
            this.minusButton.height = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
            this.minusButton.width = 30;
            this.thumb.height = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
            this.thumb.width = undefined;
            this.plusButton.height = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
            this.plusButton.width = 30;
            this.slider.height = Object(_core_utils_Percent__WEBPACK_IMPORTED_MODULE_7__["percent"])(100);
            this.slider.width = 0;
            this.minusButton.marginLeft = 2;
            this.plusButton.marginRight = 2;
            this.minusButton.toBack();
            this.plusButton.toFront();
        }
    };
    /**
     * Handles zoom operation after clicking on the slider background.
     *
     * @ignore Exclude from docs
     * @param {AMEvent<Sprite, ISpriteEvents>["hit"]}  event  Event
     */
    ZoomControl.prototype.handleBackgroundClick = function (event) {
        var sprite = event.target;
        var y = event.spritePoint.y;
        var chart = this.chart;
        var maxPower = Math.log(chart.maxZoomLevel) / Math.LN2;
        var minPower = Math.log(chart.minZoomLevel) / Math.LN2;
        var power = (sprite.pixelHeight - y) / sprite.pixelHeight * (minPower + (maxPower - minPower));
        var zoomLevel = Math.pow(2, power);
        chart.zoomToGeoPoint(chart.zoomGeoPoint, zoomLevel);
    };
    Object.defineProperty(ZoomControl.prototype, "chart", {
        /**
         * @return {MapChart} Map/chart
         */
        get: function () {
            return this._chart.get();
        },
        /**
         * A main chart/map that this zoom control is for.
         *
         * @param {MapChart}  chart  Map/chart
         */
        set: function (chart) {
            var _this = this;
            this._chart.set(chart, new _core_utils_Disposer__WEBPACK_IMPORTED_MODULE_4__["MultiDisposer"]([
                chart.events.on("maxsizechanged", this.updateThumbSize, this, false),
                chart.events.on("zoomlevelchanged", this.updateThumb, this, false),
                this.minusButton.events.on("hit", function () { chart.zoomOut(chart.zoomGeoPoint); }, chart, false),
                Object(_core_interaction_Interaction__WEBPACK_IMPORTED_MODULE_6__["getInteraction"])().body.events.on("keyup", function (ev) {
                    if (_this.topParent.hasFocused) {
                        if (_core_utils_Keyboard__WEBPACK_IMPORTED_MODULE_5__["keyboard"].isKey(ev.event, "enter")) {
                            if (_this.minusButton.isFocused) {
                                chart.zoomOut();
                            }
                            else if (_this.plusButton.isFocused) {
                                chart.zoomIn();
                            }
                        }
                        else if (_core_utils_Keyboard__WEBPACK_IMPORTED_MODULE_5__["keyboard"].isKey(ev.event, "plus")) {
                            chart.zoomIn();
                        }
                        else if (_core_utils_Keyboard__WEBPACK_IMPORTED_MODULE_5__["keyboard"].isKey(ev.event, "minus")) {
                            chart.zoomOut();
                        }
                    }
                }, chart),
                this.plusButton.events.on("hit", function () { chart.zoomIn(chart.zoomGeoPoint); }, chart, false)
            ]));
        },
        enumerable: true,
        configurable: true
    });
    /**
     * Updates the slider's thumb size based on the available zoom space.
     *
     * @ignore Exclude from docs
     */
    ZoomControl.prototype.updateThumbSize = function () {
        var chart = this.chart;
        if (chart) {
            var slider = this.slider;
            var thumb = this.thumb;
            if (this.layout == "vertical") {
                thumb.minHeight = Math.min(this.slider.pixelHeight, 20);
                thumb.height = slider.pixelHeight / this.stepCount;
                thumb.maxY = slider.pixelHeight - thumb.pixelHeight;
                if (thumb.pixelHeight <= 1) {
                    thumb.visible = false;
                }
                else {
                    thumb.visible = true;
                }
            }
            else {
                thumb.minWidth = Math.min(this.slider.pixelWidth, 20);
                thumb.width = slider.pixelWidth / this.stepCount;
                thumb.maxX = slider.pixelWidth - thumb.pixelWidth;
                if (thumb.pixelWidth <= 1) {
                    thumb.visible = false;
                }
                else {
                    thumb.visible = true;
                }
            }
        }
    };
    /**
     * Updates thumb according to current zoom position from map.
     *
     * @ignore Exclude from docs
     */
    ZoomControl.prototype.updateThumb = function () {
        var slider = this.slider;
        var chart = this.chart;
        var thumb = this.thumb;
        if (!thumb.isDown) {
            var step = (Math.log(chart.zoomLevel) - Math.log(this.chart.minZoomLevel)) / Math.LN2;
            if (this.layout == "vertical") {
                thumb.y = slider.pixelHeight - (slider.pixelHeight - thumb.pixelHeight) * step / this.stepCount - thumb.pixelHeight;
            }
            else {
                thumb.x = slider.pixelWidth * step / this.stepCount;
            }
        }
    };
    /**
     * Zooms the actual map when slider position changes.
     *
     * @ignore Exclude from docs
     */
    ZoomControl.prototype.handleThumbDrag = function () {
        var slider = this.slider;
        var chart = this.chart;
        var thumb = this.thumb;
        var step;
        var minStep = Math.log(this.chart.minZoomLevel) / Math.LN2;
        if (this.layout == "vertical") {
            step = this.stepCount * (slider.pixelHeight - thumb.pixelY - thumb.pixelHeight) / (slider.pixelHeight - thumb.pixelHeight);
        }
        else {
            step = this.stepCount * thumb.pixelX / slider.pixelWidth;
        }
        step = minStep + step;
        var zoomLevel = Math.pow(2, step);
        chart.zoomToGeoPoint(undefined, zoomLevel, false, 0);
    };
    Object.defineProperty(ZoomControl.prototype, "stepCount", {
        /**
         * Returns the step countfor the slider grid according to map's min and max
         * zoom level settings.
         *
         * @ignore Exclude from docs
         * @return {number} Step count
         */
        get: function () {
            return Math.log(this.chart.maxZoomLevel) / Math.LN2 - Math.log(this.chart.minZoomLevel) / Math.LN2;
        },
        enumerable: true,
        configurable: true
    });
    /**
     * Creates a background element for slider control.
     *
     * @ignore Exclude from docs
     * @return {this} Background
     */
    ZoomControl.prototype.createBackground = function () {
        return new _core_elements_RoundedRectangle__WEBPACK_IMPORTED_MODULE_3__["RoundedRectangle"]();
    };
    return ZoomControl;
}(_core_Container__WEBPACK_IMPORTED_MODULE_1__["Container"]));

/**
 * Register class in system, so that it can be instantiated using its name from
 * anywhere.
 *
 * @ignore
 */
_core_Registry__WEBPACK_IMPORTED_MODULE_8__["registry"].registeredClasses["ZoomControl"] = ZoomControl;
//# sourceMappingURL=ZoomControl.js.map

/***/ })

}]);