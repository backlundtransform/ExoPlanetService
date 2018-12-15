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
import { BrowserRouter as Router, Route, Link } from 'react-router-dom';
import Catalog from './Catalog';
import Map from './Map';
import Chart from './Chart';
import PlanetInfo from './PlanetInfo';
import Simulator from './Simulator';
import Constellations from './Constellations';
import StarInfo from './StarInfo';
import { Menu, Icon } from 'semantic-ui-react';
var Navigate = /** @class */ (function (_super) {
    __extends(Navigate, _super);
    function Navigate() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.state = { activeItem: 'catalog' };
        _this.handleItemClick = function (e, _a) {
            var name = _a.name;
            return _this.setState({ activeItem: name });
        };
        return _this;
    }
    Navigate.prototype.componentDidMount = function () {
        return __awaiter(this, void 0, void 0, function () {
            return __generator(this, function (_a) {
                this.setState({ activeItem: window.location.pathname });
                return [2 /*return*/];
            });
        });
    };
    Navigate.prototype.render = function () {
        var activeItem = this.state.activeItem;
        return (React.createElement(React.Fragment, null,
            React.createElement(Router, null,
                React.createElement(React.Fragment, null,
                    React.createElement(Menu, { icon: "labeled", pointing: true, secondary: true, inverted: true, stackable: true },
                        React.createElement("a", { className: "item", href: "/" },
                            React.createElement(Icon, { name: "home" }),
                            'Home'),
                        React.createElement(Menu.Item, { name: "/catalog", active: activeItem === '/catalog', as: Link, to: "/catalog", onClick: this.handleItemClick },
                            React.createElement(Icon, { name: "book" }),
                            'Catalog'),
                        React.createElement(Menu.Item, { name: "/map", active: activeItem === '/map', as: Link, to: "/map", onClick: this.handleItemClick },
                            React.createElement(Icon, { name: "map" }),
                            'Star map'),
                        React.createElement(Menu.Item, { name: "/chart", active: activeItem === '/chart', as: Link, to: "/chart", onClick: this.handleItemClick },
                            React.createElement(Icon, { name: "chart line" }),
                            'Chart')),
                    React.createElement(Route, { exact: true, path: "/catalog", component: Catalog }),
                    React.createElement(Route, { exact: true, path: "/Map", component: Map }),
                    React.createElement(Route, { exact: true, path: "/chart", component: Chart }),
                    React.createElement(Route, { name: "planet", path: "/planet/:planetId", component: PlanetInfo }),
                    React.createElement(Route, { name: "star", path: "/star/:starId", component: StarInfo }),
                    React.createElement(Route, { name: "system", path: "/system/:starId", component: Simulator }),
                    React.createElement(Route, { name: "constellation", path: "/constellation/:constellationId", component: Constellations })))));
    };
    return Navigate;
}(React.Component));
export default Navigate;
