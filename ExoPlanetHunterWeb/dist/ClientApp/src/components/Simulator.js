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
import { ReactSVGPanZoom } from 'react-svg-pan-zoom';
import { GetPlanetAsync } from '../service/getPlanets';
import { getStarSize, getSolarSystem } from '../service/getSolarSystem';
import { Gradient } from '../styles/radialgradients';
import Svg, { Circle, Ellipse, Image, ClipPath, Text, Defs, G, Rect } from 'react-native-svg-web';
var Simulator = /** @class */ (function (_super) {
    __extends(Simulator, _super);
    function Simulator() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.state = {
            x: 0,
            y: 0,
            alpha: 0,
            star: {},
            loading: true,
        };
        _this.updateHandler = function () { return _this.setState({ alpha: _this.state.alpha + 1 / 50 }); };
        _this.RotateX = function (cx, rx) {
            return cx + rx * Math.cos(_this.state.alpha + rx);
        };
        _this.RotateY = function (cy, ry) {
            return cy + ry * Math.sin(_this.state.alpha + ry / 0.3);
        };
        _this.navigateToPlanet = function (planet) { return __awaiter(_this, void 0, void 0, function () {
            var planetinfo;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        if (!(planet.id !== undefined)) return [3 /*break*/, 2];
                        return [4 /*yield*/, GetPlanetAsync(planet.id)];
                    case 1:
                        planetinfo = _a.sent();
                        planetinfo &&
                            this.props.history.push({
                                pathname: "/planet/" + planetinfo.name,
                                state: { planet: planetinfo }
                            });
                        _a.label = 2;
                    case 2: return [2 /*return*/];
                }
            });
        }); };
        return _this;
    }
    Simulator.prototype.componentWillMount = function () {
        return __awaiter(this, void 0, void 0, function () {
            var location, star;
            var _this = this;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        location = this.props.location;
                        return [4 /*yield*/, getSolarSystem(location.state.star)];
                    case 1:
                        star = _a.sent();
                        star.radius = getStarSize(star);
                        setInterval(function () { return _this.updateHandler(); }, 1000);
                        this.setState({ star: star, loading: false });
                        return [2 /*return*/];
                }
            });
        });
    };
    Simulator.prototype.render = function () {
        var _this = this;
        var _a = this.state, star = _a.star, loading = _a.loading;
        var width = window.innerWidth - 20;
        var height = window.innerHeight - 120;
        return (React.createElement("div", { className: 'space' }, loading ? (React.createElement(React.Fragment, null)) : (React.createElement(ReactSVGPanZoom, { width: width, height: height, SVGBackground: 'transparent', background: 'transparent', onClick: function (event) { return _this.navigateToPlanet(event.originalEvent.target); } },
            React.createElement(Svg, { x: 0, y: 0, height: height, width: width },
                Gradient(),
                React.createElement(Rect, { x: width / 2 - (3 * star.radius) / 2, y: height / 2, width: star.radius * 3, height: star.radius * 3, fill: "url(#Star" + (star.color != null ? star.color.toString() : 2) + ")" }),
                star.habZoneMax != null ? (React.createElement(Ellipse, { cx: width / 2, cy: height / 2, rx: star.habZoneMax, ry: star.habZoneMax * 0.3, stroke: "blue", strokeWidth: "1", fillOpacity: "0" })) : (React.createElement(React.Fragment, null)),
                star.habZoneMax != null ? (React.createElement(Ellipse, { cx: width / 2, cy: height / 2, rx: star.habZoneMin, ry: star.habZoneMin * 0.3, stroke: "red", strokeWidth: "1", fillOpacity: "0" })) : (React.createElement(React.Fragment, null)),
                star.planets.map(function (p, index) {
                    return (React.createElement(G, { key: index.toString() },
                        React.createElement(Defs, null,
                            React.createElement(ClipPath, { id: index.toString() },
                                React.createElement(Circle, { cx: _this.RotateX(width / 2, p.starDistance), cy: _this.RotateY(height / 2, p.starDistance * 0.3), r: p.radius }))),
                        React.createElement(Image, { width: "180", height: "190", x: _this.RotateX(width / 2 - 2 * p.radius, p.starDistance), y: _this.RotateY(height / 2 - 2 * p.radius, p.starDistance * 0.3), href: "../img/" + p.img.uri + ".jpg", clipPath: "url(#" + index.toString() }),
                        React.createElement(Circle, { id: p.name, cx: _this.RotateX(width / 2, p.starDistance), cy: _this.RotateY(height / 2, p.starDistance * 0.3), r: p.radius, fillOpacity: 0.4, fill: "url(#" + p.img.uri + ")", style: { cursor: 'pointer' } }),
                        "/>",
                        React.createElement(Text, { key: "text- " + index, x: p.radius + _this.RotateX(width / 2, p.starDistance), y: p.radius +
                                _this.RotateY(height / 2, p.starDistance * 0.3), textAnchor: "end", id: p.name, fontWeight: "bold", fontSize: "18", fill: "white", style: { cursor: 'pointer' } }, p.name)));
                }),
                React.createElement(Rect, { x: width / 2 - (3 * star.radius) / 2, y: height / 2 - 3 * star.radius, width: star.radius * 3, height: star.radius * 3, fill: "url(#StarTop" + (star.color != null ? star.color.toString() : 2) + ")" }))))));
    };
    return Simulator;
}(React.Component));
export default Simulator;
