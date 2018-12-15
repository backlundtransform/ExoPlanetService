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
var _this = this;
import { resource } from '../config/Resource';
export var GetStatisticsAsync = function () { return __awaiter(_this, void 0, void 0, function () {
    var stat;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4 /*yield*/, fetch("../api/Statistics")
                    .then(function (response) {
                    return response.json();
                })
                    .then(function (myJson) {
                    return myJson;
                })];
            case 1:
                stat = _a.sent();
                return [2 /*return*/, stat];
        }
    });
}); };
export var GetPlanetListAsync = function (filter, filterstate, top) {
    var skip = top - 30;
    var filterstring = '%24filter=Message eq null';
    var orderby = 'DiscYear%20desc';
    if (filter != null) {
        if (filter.Key === 'Hab' || filter.Key === 'Moons') {
            filterstring = filterstring + " and " + filter.Key + "%20eq%20true";
        }
        if (filter.Key === 'Temp' || filter.Key === 'Esi' || filter.Key === 'Sph') {
            filterstring = filterstring + " and " + filter.Key + " gt " + filter.MinValue + " and " + filter.Key + " lt  " + filter.MaxValue;
        }
        if (filter.Name != null) {
            skip = 0;
            top = 10;
            filterstring = filterstring + " and indexof(Name, '" + encodeURIComponent(filter.Name) + "') gt -1";
        }
    }
    if (filterstate.filter !== undefined) {
        var currentfilter = filterstate.filter.filter;
        var compindex = resource.compsearch.indexOf(currentfilter.comp);
        var massindex = resource.masssearch.indexOf(currentfilter.mass);
        var atmosindex = resource.atmossearch.indexOf(currentfilter.atmos);
        var discindex = resource.discsearch.indexOf(currentfilter.disc);
        var tempindex = resource.tempsearch.indexOf(currentfilter.temp);
        var orderindex = resource.sortsearch.indexOf(currentfilter.sort);
        var lightyearsindex = resource.lightyearsearch.indexOf(currentfilter.lightyears);
        if (compindex > -1) {
            filterstring = filterstring + " and Comp eq " + compindex;
        }
        if (massindex > -1) {
            filterstring = filterstring + " and MassType eq " + massindex;
        }
        if (atmosindex > -1) {
            filterstring = filterstring + " and Atmosphere eq " + atmosindex;
        }
        if (tempindex > -1) {
            filterstring = filterstring + " and TempZone eq " + tempindex;
        }
        switch (lightyearsindex) {
            case 0:
                filterstring = filterstring + " and Distance lt " + 20;
                break;
            case 1:
                filterstring = filterstring + " and Distance lt " + 200;
                break;
            case 2:
                filterstring = filterstring + " and Distance lt " + 2000;
                break;
            case 3:
                filterstring = filterstring + " and Distance lt " + 20000;
                break;
        }
        if (orderindex > -1) {
            var order = currentfilter.order ? 'desc' : 'asc';
            if (orderindex === 0) {
                orderby = 'Distance';
            }
            if (orderindex === 1) {
                orderby = 'Esi';
            }
            if (orderindex === 2) {
                orderby = 'Mass';
            }
            if (orderindex === 3) {
                orderby = 'DiscYear';
            }
            orderby = orderby + "%20" + order;
        }
    }
    var planetList = fetch("../api/ExoSolarSystems/ExoPlanets?" + filterstring + "&%24top=" + top + "&%24skip=" + skip + "&%24orderby=" + orderby)
        .then(function (response) {
        return response.json();
    })
        .then(function (myJson) {
        return myJson;
    });
    return planetList;
};
export var GetPlanetAsync = function (name) {
    var planetList = fetch("../api/ExoSolarSystems/ExoPlanets?%24filter=Name eq '" + encodeURIComponent(name) + "'")
        .then(function (response) {
        return response.json();
    })
        .then(function (myJson) {
        return myJson[0];
    });
    return planetList;
};
export var GetHabitablePlanets = function () { return __awaiter(_this, void 0, void 0, function () {
    var planetList;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0: return [4 /*yield*/, fetch("../api/ExoSolarSystems/GetHabitablePlanets")
                    .then(function (response) {
                    return response.json();
                })
                    .then(function (myJson) {
                    return myJson;
                })];
            case 1:
                planetList = _a.sent();
                return [2 /*return*/, planetList];
        }
    });
}); };
