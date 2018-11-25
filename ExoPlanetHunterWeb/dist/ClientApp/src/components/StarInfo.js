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
import { Container, Header, Table, Grid, Button } from 'semantic-ui-react';
import { GetPlanetAsync } from '../service/getPlanets';
import { resource } from '../config/Resource';
import { Gradient } from '../styles/radialgradients';
import MaterialIcon from 'material-icons-react';
import Svg, { Rect } from 'react-native-svg-web';
import { Link } from 'react-router-dom';
import { getSolarSystem } from '../service/getSolarSystem';
var StarInfo = /** @class */ (function (_super) {
    __extends(StarInfo, _super);
    function StarInfo() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.state = {
            star: {}
        };
        _this.getPlanetText = function (star) {
            var knownhabplanets = star.noHabPlanets;
            var knownplanets = star.noPlanets;
            if (knownhabplanets >= 1 && knownplanets > 1) {
                if (knownhabplanets === 1) {
                    return resource.numberplanet[1] + " " + knownplanets + " " + resource.numberplanet[2] + " " + resource.numberplanet[0];
                }
                return resource.numberplanet[1] + " " + knownplanets + " " + resource.numberplanet[2] + " " + knownhabplanets + " " + resource.numberplanet[3];
            }
            if (knownhabplanets == 0 && knownplanets > 1) {
                return resource.numberplanet[1] + " " + knownplanets + " " + resource.numberplanet[4];
            }
            if (knownhabplanets == 0 && knownplanets == 1) {
                return "" + resource.numberplanet[5];
            }
        };
        return _this;
    }
    StarInfo.prototype.componentDidMount = function () {
        return __awaiter(this, void 0, void 0, function () {
            var star, starobj, planet;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        star = this.props.location.state.star;
                        if (!(star.planets === null)) return [3 /*break*/, 3];
                        return [4 /*yield*/, getSolarSystem(star)];
                    case 1:
                        starobj = _a.sent();
                        return [4 /*yield*/, GetPlanetAsync(starobj.planets[0].name)];
                    case 2:
                        planet = _a.sent();
                        star = planet.star;
                        _a.label = 3;
                    case 3:
                        this.setState({ star: star });
                        return [2 /*return*/];
                }
            });
        });
    };
    StarInfo.prototype.render = function () {
        var star = this.state.star;
        var planetext = this.getPlanetText(star);
        var size = window.innerWidth / 5 > 200 ? window.innerWidth / 5 : 200;
        size = size > 400 ? 400 : size;
        return (React.createElement(Container, { className: 'post-preview' },
            React.createElement(Grid, { columns: 2, stackable: true },
                React.createElement(Grid.Row, null,
                    React.createElement(Grid.Column, null,
                        React.createElement(Container, { text: true }, star.radius && (React.createElement(Svg, { height: Math.round(star.radius) * 4, width: Math.round(star.radius) * 3, x: 0 },
                            Gradient(),
                            React.createElement(Rect, { x: 0, y: -Math.round(star.radius), width: Math.round(star.radius) * 3, height: Math.round(star.radius) * 3, fill: "url(#StarTop" + (star.color != null ? star.color.toString() : 2) + ")" }),
                            React.createElement(Rect, { x: 0, y: 2 * Math.round(star.radius), width: Math.round(star.radius) * 3, height: Math.round(star.radius) * 3, fill: "url(#Star" + (star.color != null ? star.color.toString() : 2) + ")" }))))),
                    React.createElement(Grid.Column, null,
                        React.createElement(Header, { className: 'post-preview' }, star.name),
                        React.createElement(Table, { celled: true },
                            ' ',
                            React.createElement(Table.Body, null,
                                star.mass && (React.createElement(Table.Row, null,
                                    React.createElement(Table.Cell, null, "" + resource.starinfo[0]),
                                    React.createElement(Table.Cell, null, star.mass + "*" + resource.oursun))),
                                star.radiusSu !== 0 && star.radiusSu && (React.createElement(Table.Row, null,
                                    React.createElement(Table.Cell, null, "" + resource.starinfo[1]),
                                    React.createElement(Table.Cell, null, star.radiusSu + "*" + resource.oursun))),
                                star.age && (React.createElement(Table.Row, null,
                                    React.createElement(Table.Cell, null, "" + resource.starinfo[2]),
                                    React.createElement(Table.Cell, null, star.age + "*" + resource.oursun))),
                                star.temp && (React.createElement(Table.Row, null,
                                    React.createElement(Table.Cell, null, "" + resource.starinfo[3]),
                                    React.createElement(Table.Cell, null, star.temp + " C")))),
                            React.createElement(Table.Footer, null,
                                React.createElement(Table.Row, null))),
                        React.createElement(Link, { to: {
                                pathname: "../system/" + star.name,
                                state: { star: star }
                            } },
                            ' ',
                            React.createElement(Button, { icon: true, inverted: true, basic: true, color: "grey", height: "40" },
                                React.createElement(MaterialIcon, { icon: "3d_rotation", color: "#c6d4ff", size: 40 }),
                                'Visit Solar System')),
                        React.createElement(Link, { to: {
                                pathname: "../constellation/" + star.constellation,
                                state: { constellation: star.constellation }
                            } },
                            ' ',
                            React.createElement(Button, { icon: true, inverted: true, basic: true, color: "grey", height: "40" },
                                React.createElement(MaterialIcon, { icon: "scatter_plot", color: "#c6d4ff", size: 40 }),
                                'Visit Constellation'))))),
            React.createElement("hr", null),
            React.createElement(Container, null,
                React.createElement("p", null, resource.starname[0] + " " + star.name + " " + resource.starname[1] + " " + (resource.const[star.constellation] &&
                    resource.const[star.constellation]) + ".",
                    ' ', "" + (star.luminosity === 9
                    ? resource.startype[0]
                    : resource.startype[1] +
                        ' ' +
                        resource.color[star.color] +
                        ' ' +
                        resource.typecolor) + (star.luminosity < 9
                    ? ' ' +
                        resource.startype[2] +
                        ' ' +
                        resource.lum[star.luminosity] +
                        '.'
                    : '.') + " " + (resource.mag[star.magnitude] != null
                    ? resource.mag[star.magnitude]
                    : '')),
                star.habZoneMin != null && star.habZoneMax != null
                    ? resource.habzone[0] + " " + star.habZoneMin + " AU " + resource.habzone[1] + " " + star.habZoneMax + " AU. "
                    : '',
                planetext == null ? React.createElement(React.Fragment, null) : planetext)));
    };
    return StarInfo;
}(React.Component));
export default StarInfo;
