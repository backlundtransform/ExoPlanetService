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
import { GetPlanetListAsync } from '../service/getPlanets';
import { Card, Button, Grid, Rating, Input, Dropdown, Icon } from 'semantic-ui-react';
import Svg, { Circle, G, ClipPath, Image, Defs } from 'react-native-svg-web';
import MaterialIcon from 'material-icons-react';
import { Link } from 'react-router-dom';
import { Gradient } from '../styles/radialgradients';
export var getGroupedItems = function (posts) {
    var groupeditems = groupBy(posts, 3);
    var groupedposts = [];
    for (var _i = 0, groupeditems_1 = groupeditems; _i < groupeditems_1.length; _i++) {
        var items = groupeditems_1[_i];
        groupedposts.push(row(items));
    }
    return groupedposts;
};
var groupBy = function (arr, n) {
    var group = [];
    for (var i = 0, end = arr.length / n; i < end; ++i)
        group.push(arr.slice(i * n, (i + 1) * n));
    return group;
};
var row = function (post) { return (React.createElement(Grid.Row, { centered: true, columns: 4 }, post)); };
var options = [
    { key: 'all', text: 'All Planets', value: 'all' },
    { key: 'hab', text: 'Habitable Planets', value: 'hab' }
];
var Catalog = /** @class */ (function (_super) {
    __extends(Catalog, _super);
    function Catalog(props) {
        var _this = _super.call(this, props) || this;
        _this.mainPost = function () {
            var planets = _this.state.planets;
            var posts = [];
            for (var _i = 0, _a = planets; _i < _a.length; _i++) {
                var item = _a[_i];
                posts.push(React.createElement(Grid.Column, null,
                    React.createElement(Card, { className: 'post-preview' },
                        React.createElement(Link, { to: {
                                pathname: "planet/" + item.name,
                                state: { planet: item }
                            } },
                            React.createElement(Svg, { height: "190", width: "180" },
                                ' ',
                                Gradient(),
                                React.createElement(G, null,
                                    React.createElement(Defs, null,
                                        React.createElement(ClipPath, { id: "clip" },
                                            React.createElement(Circle, { cx: "100", cy: "100", r: "65" }))),
                                    React.createElement(Image, { width: "180", height: "190", href: "../img/" + item.img.uri + ".jpg", clipPath: "url(#clip)" }),
                                    React.createElement(Circle, { cx: "100", cy: "100", r: "65", fillOpacity: 0.4, fill: "url(#" + item.img.uri }))),
                            ' '),
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
                            React.createElement(Rating, { icon: "star", defaultRating: Math.round(item.esi * 10), maxRating: 10, size: "large", disabled: true })),
                        React.createElement(Card.Content, { extra: true },
                            React.createElement(Link, { to: {
                                    pathname: "star/" + item.star.name,
                                    state: { star: item.star }
                                } },
                                React.createElement(Button, { icon: true, inverted: true, basic: true, color: "grey", height: "25" },
                                    React.createElement(MaterialIcon, { icon: "wb_sunny", color: "#c6d4ff", size: 25 }),
                                    'Visit Star',
                                    ' ')),
                            React.createElement(Link, { to: {
                                    pathname: "system/" + item.star.name,
                                    state: { star: item.star }
                                } },
                                React.createElement(Button, { icon: true, inverted: true, basic: true, color: "grey", height: "25" },
                                    React.createElement(MaterialIcon, { icon: "3d_rotation", color: "#c6d4ff", size: 25 }), item.star.noPlanets + " Planets"))))));
            }
            return getGroupedItems(posts);
        };
        _this.handleSearchChange = function (e) { return __awaiter(_this, void 0, void 0, function () {
            var searchValue;
            return __generator(this, function (_a) {
                searchValue = this.state.searchValue;
                searchValue = e.target.value;
                this.setState({ searchValue: searchValue });
                return [2 /*return*/];
            });
        }); };
        _this.handleSearchClick = function () { return __awaiter(_this, void 0, void 0, function () {
            var filter, planets;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        filter = this.setSearchFilter();
                        return [4 /*yield*/, GetPlanetListAsync(filter, {}, 30)];
                    case 1:
                        planets = _a.sent();
                        this.setState({ planets: planets, loading: false });
                        return [2 /*return*/];
                }
            });
        }); };
        _this.handleHabClick = function () { return __awaiter(_this, void 0, void 0, function () {
            var habactive, planets;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        habactive = this.state.habactive;
                        habactive = !habactive;
                        planets = [];
                        if (!habactive) return [3 /*break*/, 2];
                        return [4 /*yield*/, GetPlanetListAsync({ Key: 'Hab' }, {}, 30)];
                    case 1:
                        planets = _a.sent();
                        _a.label = 2;
                    case 2:
                        if (!!habactive) return [3 /*break*/, 4];
                        return [4 /*yield*/, GetPlanetListAsync({}, {}, 30)];
                    case 3:
                        planets = _a.sent();
                        _a.label = 4;
                    case 4:
                        this.setState({ planets: planets, loading: false, habactive: habactive });
                        return [2 /*return*/];
                }
            });
        }); };
        _this.handlePaginate = function (top) { return __awaiter(_this, void 0, void 0, function () {
            var filter, planets;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0:
                        filter = this.setSearchFilter();
                        return [4 /*yield*/, GetPlanetListAsync(filter, {}, top)];
                    case 1:
                        planets = _a.sent();
                        this.setState({ planets: planets, loading: false, top: top });
                        return [2 /*return*/];
                }
            });
        }); };
        _this.setSearchFilter = function () {
            var _a = _this.state, habactive = _a.habactive, searchValue = _a.searchValue;
            if (habactive && searchValue !== '') {
                return { Key: 'Hab', Name: searchValue };
            }
            if (!habactive && searchValue !== '') {
                return { Name: searchValue };
            }
            if (habactive && searchValue === '') {
                return { Key: 'Hab' };
            }
            return {};
        };
        _this.state = {
            loading: true,
            top: 30,
            color: '',
            habactive: false,
            searchValue: '',
            planets: []
        };
        return _this;
    }
    Catalog.prototype.componentDidMount = function () {
        return __awaiter(this, void 0, void 0, function () {
            var planets;
            return __generator(this, function (_a) {
                switch (_a.label) {
                    case 0: return [4 /*yield*/, GetPlanetListAsync(null, {}, 30)];
                    case 1:
                        planets = _a.sent();
                        this.setState({ planets: planets, loading: false });
                        return [2 /*return*/];
                }
            });
        });
    };
    Catalog.prototype.render = function () {
        var _this = this;
        var _a = this.state, loading = _a.loading, searchValue = _a.searchValue, top = _a.top, planets = _a.planets;
        var main = this.mainPost();
        return loading ? (React.createElement(React.Fragment, null)) : (React.createElement(React.Fragment, null,
            React.createElement("div", { className: 'bar' },
                React.createElement(Input, { type: 'text', size: "large", value: searchValue, onChange: function (e) { return _this.handleSearchChange(e); }, placeholder: 'Search...', action: true },
                    React.createElement("input", null),
                    React.createElement(Button, { type: 'submit', onClick: function () { return _this.handleSearchClick(); } },
                        React.createElement(Icon, { name: "search" })),
                    React.createElement(Dropdown, { button: true, basic: true, floating: true, onChange: function () { return _this.handleHabClick(); }, options: options, defaultValue: "all" }))),
            React.createElement("div", { className: "catalog" },
                React.createElement(Grid, { stackable: true, centered: true, columns: 2 }, main),
                "  "),
            React.createElement("div", { className: 'bar' },
                React.createElement(Button.Group, null,
                    React.createElement(Button, { labelPosition: "left", icon: "left chevron", content: "Previous", color: top <= 30 ? 'grey' : 'black', disabled: top <= 30, onClick: function () { return _this.handlePaginate(top - 30); } }),
                    React.createElement(Button, { labelPosition: "right", icon: "right chevron", content: "Next", color: planets.length <= 29 ? 'grey' : 'black', disabled: planets.length <= 29, onClick: function () { return _this.handlePaginate(top + 30); } })))));
    };
    return Catalog;
}(React.Component));
export default Catalog;
