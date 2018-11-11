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
import { GetPlanetListAsync, storeBase64 } from '../service/getPlanets';
import { Card, Icon, Grid, Rating } from 'semantic-ui-react';
import Svg, { Circle, G, ClipPath, Image, Defs } from 'react-native-svg-web';
import MaterialIcon from 'material-icons-react';
import { Link } from 'react-router-dom';
import { Gradient } from '../styles/radialgradients';
var Catalog = /** @class */ (function (_super) {
    __extends(Catalog, _super);
    function Catalog(props) {
        var _this = _super.call(this, props) || this;
        _this.groupBy = function (arr, n) {
            var group = [];
            for (var i = 0, end = arr.length / n; i < end; ++i)
                group.push(arr.slice(i * n, (i + 1) * n));
            return group;
        };
        _this.mainPost = function () {
            var _a = _this.state, planets = _a.planets, color = _a.color;
            var posts = [];
            for (var _i = 0, _b = planets; _i < _b.length; _i++) {
                var item = _b[_i];
                posts.push(React.createElement(Grid.Column, null,
                    React.createElement(Card, { className: 'post-preview' },
                        React.createElement(Svg, { height: "190", width: "180" },
                            ' ',
                            Gradient(),
                            React.createElement(G, null,
                                React.createElement(Defs, null,
                                    React.createElement(ClipPath, { id: "clip" },
                                        React.createElement(Circle, { cx: "100", cy: "100", r: "65" }))),
                                React.createElement(Image, { width: "180", height: "190", href: "" + color[item.img.uri], clipPath: "url(#clip)" }),
                                React.createElement(Circle, { cx: "100", cy: "100", r: "65", fillOpacity: 0.4, fill: "url(#" + item.img.uri }))),
                        React.createElement(Card.Content, null,
                            React.createElement(Card.Header, null,
                                React.createElement(Link, { to: {
                                        pathname: "planet/" + item.name,
                                        state: { planet: item }
                                    } }, item.name)),
                            React.createElement(Card.Description, null,
                                React.createElement("span", null, "Discovered:" + item.discYear)),
                            React.createElement(Card.Description, null, "" + (item.type && item.type !== null ? item.type : ''),
                                ' ',
                                item.distance !== 0
                                    ? Math.round(item.distance) + " lightyears from earth."
                                    : ''),
                            React.createElement(Rating, { icon: "star", defaultRating: Math.round(item.esi * 10), maxRating: 10, size: "large" })),
                        React.createElement(Card.Content, { extra: true },
                            React.createElement("a", { style: { margin: 10 } },
                                React.createElement(Icon, { name: "sun" }),
                                item.star.name),
                            React.createElement("br", null),
                            React.createElement("a", { style: { margin: 10 } }, item.star.noPlanets + " Planets"),
                            ' ',
                            React.createElement(Link, { to: {
                                    pathname: "system/" + item.star.name,
                                    state: { star: item.star }
                                } },
                                React.createElement(MaterialIcon, { icon: "3d_rotation", color: "#c6d4ff", size: 25 }))))));
            }
            var groupedplanets = _this.groupBy(posts, 3);
            var groupedposts = [];
            for (var _c = 0, groupedplanets_1 = groupedplanets; _c < groupedplanets_1.length; _c++) {
                var planets_1 = groupedplanets_1[_c];
                groupedposts.push(_this.row(planets_1));
            }
            return groupedposts;
        };
        _this.row = function (post) { return (React.createElement(Grid.Row, { centered: true, columns: 4 }, post)); };
        _this.state = {
            loading: true,
            top: 100,
            color: '',
            planets: []
        };
        return _this;
    }
    Catalog.prototype.componentDidMount = function () {
        return __awaiter(this, void 0, void 0, function () {
            var planets, color, _a, _b;
            return __generator(this, function (_c) {
                switch (_c.label) {
                    case 0: return [4 /*yield*/, GetPlanetListAsync(null, {}, 100)];
                    case 1:
                        planets = _c.sent();
                        _b = (_a = JSON).parse;
                        return [4 /*yield*/, storeBase64()];
                    case 2:
                        color = _b.apply(_a, [_c.sent()]);
                        this.setState({ color: color, planets: planets, loading: false });
                        return [2 /*return*/];
                }
            });
        });
    };
    Catalog.prototype.render = function () {
        var loading = this.state.loading;
        var main = this.mainPost();
        return loading ? (React.createElement(React.Fragment, null)) : (React.createElement(Grid, { stackable: true, centered: true, columns: 2 }, main));
    };
    return Catalog;
}(React.Component));
export default Catalog;
