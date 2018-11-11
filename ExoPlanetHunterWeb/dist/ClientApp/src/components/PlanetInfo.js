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
import { Container } from 'semantic-ui-react';
import { resource } from '../config/Resource';
import { Gradient } from '../styles/radialgradients';
import Svg, { Circle, G, ClipPath, Image, Defs, } from 'react-native-svg-web';
var PlanetInfo = /** @class */ (function (_super) {
    __extends(PlanetInfo, _super);
    function PlanetInfo() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    PlanetInfo.prototype.componentDidMount = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                return [2 /*return*/];
            });
        });
    };
    PlanetInfo.prototype.render = function () {
        var planet = this.props.location.state.planet;
        var size = window.innerWidth / 5 > 200 ? window.innerWidth / 5 : 200;
        size = size > 500 ? 500 : size;
        return (React.createElement(Container, { className: "post-preview" },
            React.createElement(Container, { text: true },
                React.createElement(Svg, { height: size, width: size, x: size / 2 },
                    "      ",
                    Gradient(),
                    React.createElement(G, null,
                        React.createElement(Defs, null,
                            React.createElement(ClipPath, { id: "clip" },
                                React.createElement(Circle, { cx: size / 2, cy: size / 2, r: size / 2 }))),
                        React.createElement(Image, { width: 1200, height: 500, x: 0, y: 0, href: "../img/" + planet.img.uri + ".jpg\"", clipPath: "url(#clip)" },
                            " ",
                            React.createElement("animate", { attributeType: "XML", attributeName: "x", from: -1100 + size, to: -10, dur: "10s", repeatCount: "indefinite" })),
                        React.createElement(Circle, { cx: size / 2, cy: size / 2, r: size / 2, fillOpacity: 0.3, fill: "url(#" + planet.img.uri + ")" })))),
            React.createElement(Container, { text: true }, resource.planetname[0] + " " + planet.name + " " + resource.planetname[1] + "  " + planet.star.name + "  " + resource.planetname[2] + " " + (resource.const[planet.star.constellation] === undefined ? "" : resource.const[planet.star.constellation]),
                " ", "" + resource.decFormatdist[0] + (planet.distance !== 0 ? Math.round(planet.distance) : "") + " " + resource.decFormatdist[1] + " ",
                React.createElement("p", null,
                    resource.massInfo[planet.massType] === undefined ? "" : resource.massInfo[planet.massType],
                    " "),
                React.createElement("p", null,
                    resource.compInfo[planet.comp] === undefined ? "" : resource.compInfo[planet.comp],
                    " "),
                React.createElement("p", null,
                    planet.temp != null ? resource.meantemp[0] + " " + planet.temp + ". " : '',
                    planet.tempMax != null && planet.tempMin != null ? resource.meantemp[1] + " " + planet.name + " " + resource.meantemp[2] + " " + planet.tempMax + " " + resource.meantemp[3] + " " + planet.tempMin : ''),
                React.createElement("p", null,
                    planet.period != null ? resource.orbit[0] + " " + planet.period + " " + resource.orbit[1] + " " : '',
                    planet.meanDistance != null ? resource.decMean[0] + " " + planet.meanDistance + " " + resource.decMean[1] : ''),
                React.createElement("p", null),
                React.createElement("p", null, "" + (resource.hzd[planet.hzd] === undefined ? "" : resource.hzd[planet.hzd] + " ") + (resource.hza[planet.hza] === undefined ? "" : resource.hza[planet.hza] + " ") + (resource.atmosinfo[planet.atmosphere] === undefined ? "" : resource.atmosinfo[planet.atmosphere] + " ") + (planet.moon ? resource.moon : "")),
                React.createElement("p", null, (planet.discYear === undefined ? "" : resource.disc) + " " + (planet.discYear === undefined ? "" : planet.discYear) + "."),
                React.createElement("p", null, "" + (resource.discinfo[planet.discMethod] == undefined ? "" : resource.discinfo[planet.discMethod])),
                React.createElement("p", null, resource.esiratings),
                React.createElement("p", null, resource.sphratings))));
    };
    return PlanetInfo;
}(React.Component));
export default PlanetInfo;
