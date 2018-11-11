var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
import * as React from 'react';
import * as L from 'leaflet';
import { GetConstellationsLines, GetStarsMarkers } from '../service/getConstellations';
import { Icon, Statistic } from 'semantic-ui-react';
import siderealtime from '../siderealtime/';
import { GetHabitablePlanets } from '../service/getPlanets';
var Map = /** @class */ (function (_super) {
    __extends(Map, _super);
    function Map() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.state = {
            constlines: {},
            stars: {},
            longitude: -90,
            latitude: 40,
            siderealtime: '',
            planets: []
        };
        _this.init = function () {
            var _a = _this.state, constlines = _a.constlines, planets = _a.planets, stars = _a.stars;
            var lineStyle = {
                color: '#fff',
                weight: 5,
                opacity: 1
            };
            L.geoJSON(constlines, {
                style: lineStyle,
                onEachFeature: _this.onEachFeature
            }).addTo(_this._map);
            var planetIcon = L.icon({
                iconUrl: '/img/ic_launcher_web.png',
                iconSize: [60, 60]
            });
            planets.map(function (planet) {
                var planetmarker = L.marker([
                    planet.coordinate.latitude,
                    planet.coordinate.longitude
                ], { icon: planetIcon })
                    .bindTooltip(planet.name, { direction: 'left' })
                    .openTooltip()
                    .addTo(_this._map);
                planetmarker.addEventListener('click', function () {
                    return _this.props.history.push({
                        pathname: "system/" + planet.star.name,
                        state: { star: planet.star }
                    });
                });
            });
            var starIcon = L.icon({
                iconUrl: '/img/smarker.png',
                iconSize: [60, 60]
            });
            L.geoJSON(stars, {
                pointToLayer: function (feature, latlng) {
                    return L.marker(latlng, { icon: starIcon })
                        .bindTooltip(feature.properties.name, { direction: 'left' })
                        .openTooltip()
                        .addTo(_this._map);
                }
            }).addTo(_this._map);
        };
        _this.updatetime = function (position) {
            _this.setState({ siderealtime: siderealtime(position) });
            setInterval(function () {
                _this.setState({ siderealtime: siderealtime(position) });
            }, 60000);
        };
        _this.onEachFeature = function (feature, layer) {
            var options = {
                radius: 100,
                fillColor: 'white',
                color: 'white',
                weight: 0,
                opacity: 0,
                fillOpacity: 0
            };
            var coord = feature.geometry.coordinates;
            if (feature.properties.constellation != null) {
                var marker = L.circleMarker([coord[0][1], coord[0][0]], options)
                    .addTo(_this._map)
                    .bindTooltip(feature.properties.constellation, {
                    permanent: true,
                    direction: 'left'
                })
                    .openTooltip();
                marker.addEventListener('click', function () {
                    return _this.props.history.push({
                        pathname: "constellation/" + feature.properties.constellationid,
                        state: { constellation: feature.properties.constellation }
                    });
                });
            }
            options = {
                radius: 6,
                fillColor: 'white',
                color: 'white',
                weight: 1,
                opacity: 1,
                fillOpacity: 1
            };
            coord.map(function (p) {
                L.circleMarker([p[1], p[0]], options).addTo(_this._map);
            });
        };
        return _this;
    }
    Map.prototype.componentDidMount = function () {
        return __awaiter(this, void 0, void 0, function () {
            var _a, longitude, latitude, constlines, stars, planets;
            var _this = this;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        if (navigator.geolocation) {
                            navigator.geolocation.getCurrentPosition(function (position) {
                                return _this.updatetime(position.coords.longitude);
                            });
                        }
                        _a = this.state, longitude = _a.longitude, latitude = _a.latitude;
                        this._map = L.map('map', {
                            zoom: 5,
                            minZoom: 4,
                            worldCopyJump: true,
                            center: [latitude, longitude]
                        });
                        return [4 /*yield*/, GetConstellationsLines()];
                    case 1:
                        constlines = _b.sent();
                        return [4 /*yield*/, GetStarsMarkers()];
                    case 2:
                        stars = _b.sent();
                        return [4 /*yield*/, GetHabitablePlanets()];
                    case 3:
                        planets = (_b.sent());
                        this._map.on('moveend', function () {
                            var _a = _this._map.getCenter(), lng = _a.lng, lat = _a.lat;
                            _this.setState({ longitude: lng, latitude: lat });
                        });
                        L.tileLayer('/img/tile.png', {}).addTo(this._map);
                        this.setState({ constlines: constlines, planets: planets, stars: stars }, function () { return _this.init(); });
                        return [2 /*return*/];
                }
            });
        });
    };
    Map.prototype.render = function () {
        var _a = this.state, longitude = _a.longitude, latitude = _a.latitude, siderealtime = _a.siderealtime;
        return (React.createElement(React.Fragment, null,
            React.createElement(Statistic.Group, { widths: "three" },
                React.createElement(Statistic, null,
                    React.createElement(Statistic.Value, null,
                        React.createElement(Icon, { name: "compass outline" }),
                        Math.round(100 * latitude) / 100),
                    React.createElement(Statistic.Label, null, 'Declination')),
                React.createElement(Statistic, null,
                    React.createElement(Statistic.Value, null,
                        React.createElement(Icon, { name: "clock" }),
                        siderealtime),
                    React.createElement(Statistic.Label, null, 'Sidereal time')),
                React.createElement(Statistic, null,
                    React.createElement(Statistic.Value, null,
                        React.createElement(Icon, { name: "compass" }),
                        Math.round(100 * (12 + (-1 * longitude) / 15)) / 100),
                    React.createElement(Statistic.Label, null, 'Right ascension'))),
            React.createElement("div", { id: "map" })));
    };
    return Map;
}(React.Component));
export default Map;
