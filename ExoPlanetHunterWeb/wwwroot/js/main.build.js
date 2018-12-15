/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "/js/";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _css_custom_blog_css__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(1);
/* harmony import */ var _css_custom_blog_css__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(_css_custom_blog_css__WEBPACK_IMPORTED_MODULE_0__);







/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {


var content = __webpack_require__(2);

if(typeof content === 'string') content = [[module.i, content, '']];

var transform;
var insertInto;



var options = {"hmr":true}

options.transform = transform
options.insertInto = undefined;

var update = __webpack_require__(4)(content, options);

if(content.locals) module.exports = content.locals;

if(false) {}

/***/ }),
/* 2 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(3)(false);
// imports


// module
exports.push([module.i, "body {\r\n    font-size: 20px;\r\n    color: #c6d4ff;\r\n    font-family: Lora,'Times New Roman',serif;\r\n    background: #818ea5\r\n}\r\n\r\np {\r\n    line-height: 1.5;\r\n    margin: 30px 0\r\n}\r\n\r\n    p a {\r\n        text-decoration: underline\r\n    }\r\n\r\nh1, h2, h3, h4, h5, h6 {\r\n    font-weight: 800;\r\n    font-family: 'Open Sans','Helvetica Neue',Helvetica,Arial,sans-serif\r\n}\r\n\r\na {\r\n    color: #c6d4ff;\r\n    -webkit-transition: all .2s;\r\n    -moz-transition: all .2s;\r\n    transition: all .2s\r\n}\r\n\r\n    a:focus, a:hover {\r\n        color: #c6d4ff\r\n    }\r\n\r\nblockquote {\r\n    font-style: italic;\r\n    color: #868e96\r\n}\r\n\r\n.section-heading {\r\n    font-size: 36px;\r\n    font-weight: 700;\r\n    margin-top: 60px\r\n}\r\n\r\n.caption {\r\n    font-size: 14px;\r\n    font-style: italic;\r\n    display: block;\r\n    margin: 0;\r\n    padding: 10px;\r\n    text-align: center;\r\n    border-bottom-right-radius: 5px;\r\n    border-bottom-left-radius: 5px\r\n}\r\n\r\n::-moz-selection {\r\n    color: #c6d4ff;\r\n    background: #343a40;\r\n    text-shadow: none\r\n}\r\n\r\n::selection {\r\n    color: #c6d4ff;\r\n    background: #343a40;\r\n    text-shadow: none\r\n}\r\n\r\nimg::selection {\r\n    color: #c6d4ff;\r\n    background: 0 0\r\n}\r\n\r\nimg::-moz-selection {\r\n    color: #c6d4ff;\r\n    background: 0 0\r\n}\r\n\r\n.responsive {\r\n    width: 100%;\r\n    max-width: 280px;\r\n    height: auto;\r\n}\r\n\r\n#mainNav {\r\n    position: absolute;\r\n    border-bottom: 1px solid #e9ecef;\r\n    background-color: #fff;\r\n    font-family: 'Open Sans','Helvetica Neue',Helvetica,Arial,sans-serif\r\n}\r\n\r\n    #mainNav .navbar-brand {\r\n        font-weight: 800;\r\n        color: #343a40\r\n    }\r\n\r\n    #mainNav .navbar-toggler {\r\n        font-size: 12px;\r\n        font-weight: 800;\r\n        padding: 13px;\r\n        text-transform: uppercase;\r\n        color: #343a40\r\n    }\r\n\r\n    #mainNav .navbar-nav > li.nav-item > a {\r\n        font-size: 12px;\r\n        font-weight: 800;\r\n        letter-spacing: 1px;\r\n        text-transform: uppercase\r\n    }\r\n\r\n@media only screen and (min-width:992px) {\r\n    #mainNav {\r\n        border-bottom: 1px solid transparent;\r\n        background: 0 0\r\n    }\r\n\r\n        #mainNav .navbar-brand {\r\n            padding: 10px 20px;\r\n            color: #c6d4ff\r\n        }\r\n\r\n            #mainNav .navbar-brand:focus, #mainNav .navbar-brand:hover {\r\n                color: rgba(255,255,255,.8)\r\n            }\r\n\r\n        #mainNav .navbar-nav > li.nav-item > a {\r\n            padding: 10px 20px;\r\n            color: #c6d4ff\r\n        }\r\n\r\n            #mainNav .navbar-nav > li.nav-item > a:focus, #mainNav .navbar-nav > li.nav-item > a:hover {\r\n                color: #c6d4ff\r\n            }\r\n}\r\n\r\n@media only screen and (min-width:992px) {\r\n    #mainNav {\r\n        -webkit-transition: background-color .2s;\r\n        -moz-transition: background-color .2s;\r\n        transition: background-color .2s;\r\n        -webkit-transform: translate3d(0,0,0);\r\n        -moz-transform: translate3d(0,0,0);\r\n        -ms-transform: translate3d(0,0,0);\r\n        -o-transform: translate3d(0,0,0);\r\n        transform: translate3d(0,0,0);\r\n        -webkit-backface-visibility: hidden\r\n    }\r\n\r\n        #mainNav.is-fixed {\r\n            position: fixed;\r\n            top: -67px;\r\n            -webkit-transition: -webkit-transform .2s;\r\n            -moz-transition: -moz-transform .2s;\r\n            transition: transform .2s;\r\n            border-bottom: 1px solid #fff;\r\n            background-color: rgba(255,255,255,.9)\r\n        }\r\n\r\n            #mainNav.is-fixed .navbar-brand {\r\n                color: #c6d4ff\r\n            }\r\n\r\n                #mainNav.is-fixed .navbar-brand:focus, #mainNav.is-fixed .navbar-brand:hover {\r\n                    color: #c6d4ff\r\n                }\r\n\r\n            #mainNav.is-fixed .navbar-nav > li.nav-item > a {\r\n                color: #c6d4ff\r\n            }\r\n\r\n                #mainNav.is-fixed .navbar-nav > li.nav-item > a:focus, #mainNav.is-fixed .navbar-nav > li.nav-item > a:hover {\r\n                    color: #c6d4ff\r\n                }\r\n\r\n        #mainNav.is-visible {\r\n            -webkit-transform: translate3d(0,100%,0);\r\n            -moz-transform: translate3d(0,100%,0);\r\n            -ms-transform: translate3d(0,100%,0);\r\n            -o-transform: translate3d(0,100%,0);\r\n            transform: translate3d(0,100%,0)\r\n        }\r\n}\r\n\r\nheader.masthead {\r\n    margin-bottom: 50px;\r\n    background: no-repeat center center;\r\n    background-color: #868e96;\r\n    background-attachment: scroll;\r\n    position: relative;\r\n    -webkit-background-size: cover;\r\n    -moz-background-size: cover;\r\n    -o-background-size: cover;\r\n    background-size: cover\r\n}\r\n\r\n    header.masthead .overlay {\r\n        position: absolute;\r\n        top: 0;\r\n        left: 0;\r\n        height: 100%;\r\n        width: 100%;\r\n        background-color: #c6d4ff;\r\n        opacity: 0\r\n    }\r\n\r\n    header.masthead .page-heading, header.masthead .post-heading, header.masthead .site-heading {\r\n        padding: 200px 0 150px;\r\n        color: #c6d4ff\r\n    }\r\n\r\n@media only screen and (min-width:768px) {\r\n    header.masthead .page-heading, header.masthead .post-heading, header.masthead .site-heading {\r\n        padding: 200px 0\r\n    }\r\n}\r\n\r\nheader.masthead .page-heading, header.masthead .site-heading {\r\n    text-align: center\r\n}\r\n\r\n    header.masthead .page-heading h1, header.masthead .site-heading h1 {\r\n        font-size: 50px;\r\n        margin-top: 0\r\n    }\r\n\r\n    header.masthead .page-heading .subheading, header.masthead .site-heading .subheading {\r\n        font-size: 24px;\r\n        font-weight: 300;\r\n        line-height: 1.1;\r\n        display: block;\r\n        margin: 10px 0 0;\r\n        font-family: 'Open Sans','Helvetica Neue',Helvetica,Arial,sans-serif\r\n    }\r\n\r\n@media only screen and (min-width:768px) {\r\n    header.masthead .page-heading h1, header.masthead .site-heading h1 {\r\n        font-size: 80px\r\n    }\r\n}\r\n\r\nheader.masthead .post-heading h1 {\r\n    font-size: 35px\r\n}\r\n\r\nheader.masthead .post-heading .meta, header.masthead .post-heading .subheading {\r\n    line-height: 1.1;\r\n    display: block\r\n}\r\n\r\nheader.masthead .post-heading .subheading {\r\n    font-size: 24px;\r\n    font-weight: 600;\r\n    margin: 10px 0 30px;\r\n    font-family: 'Open Sans','Helvetica Neue',Helvetica,Arial,sans-serif\r\n}\r\n\r\nheader.masthead .post-heading .meta {\r\n    font-size: 20px;\r\n    font-weight: 300;\r\n    font-style: italic;\r\n    font-family: Lora,'Times New Roman',serif\r\n}\r\n\r\n    header.masthead .post-heading .meta a {\r\n        color: #c6d4ff\r\n    }\r\n\r\n@media only screen and (min-width:768px) {\r\n    header.masthead .post-heading h1 {\r\n        font-size: 55px\r\n    }\r\n\r\n    header.masthead .post-heading .subheading {\r\n        font-size: 30px\r\n    }\r\n}\r\n.post-preview {\r\n    color: #c6d4ff;\r\n    background-color: #50607a;\r\n    padding:40px\r\n}\r\n\r\n.post-preview > a {\r\n    color: #c6d4ff;\r\n\r\n}\r\nimg {\r\n    background-color: transparent;\r\n   \r\n}\r\n\r\n    .post-preview > a:focus, .post-preview > a:hover {\r\n        text-decoration: none;\r\n        color: #c6d4ff\r\n    }\r\n\r\n    .post-preview > a > .post-title {\r\n        font-size: 30px;\r\n        margin-top: 30px;\r\n        margin-bottom: 10px\r\n    }\r\n\r\n    .post-preview > a > .post-subtitle {\r\n        font-weight: 300;\r\n        margin: 0 0 10px\r\n    }\r\n\r\n.post-preview > .post-meta {\r\n    font-size: 18px;\r\n    font-style: italic;\r\n    margin-top: 0;\r\n    color: #c6d4ff\r\n}\r\n\r\n    .post-preview > .post-meta > a {\r\n        text-decoration: none;\r\n        color: #c6d4ff\r\n    }\r\n\r\n        .post-preview > .post-meta > a:focus, .post-preview > .post-meta > a:hover {\r\n            text-decoration: underline;\r\n            color: #50607a\r\n        }\r\n\r\n@media only screen and (min-width:768px) {\r\n    .post-preview > a > .post-title {\r\n        font-size: 36px\r\n    }\r\n}\r\n\r\n.floating-label-form-group {\r\n    font-size: 14px;\r\n    position: relative;\r\n    margin-bottom: 0;\r\n    padding-bottom: .5em;\r\n    border-bottom: 1px solid #dee2e6\r\n}\r\n\r\n    .floating-label-form-group input, .floating-label-form-group textarea {\r\n        font-size: 1.5em;\r\n        position: relative;\r\n        z-index: 1;\r\n        padding: 0;\r\n        resize: none;\r\n        border: none;\r\n        border-radius: 0;\r\n        background: 0 0;\r\n        box-shadow: none !important;\r\n        font-family: Lora,'Times New Roman',serif\r\n    }\r\n\r\n        .floating-label-form-group input::-webkit-input-placeholder, .floating-label-form-group textarea::-webkit-input-placeholder {\r\n            color: #868e96;\r\n            font-family: Lora,'Times New Roman',serif\r\n        }\r\n\r\n    .floating-label-form-group label {\r\n        font-size: .85em;\r\n        line-height: 1.764705882em;\r\n        position: relative;\r\n        z-index: 0;\r\n        top: 2em;\r\n        display: block;\r\n        margin: 0;\r\n        -webkit-transition: top .3s ease,opacity .3s ease;\r\n        -moz-transition: top .3s ease,opacity .3s ease;\r\n        -ms-transition: top .3s ease,opacity .3s ease;\r\n        transition: top .3s ease,opacity .3s ease;\r\n        vertical-align: middle;\r\n        vertical-align: baseline;\r\n        opacity: 0\r\n    }\r\n\r\n    .floating-label-form-group .help-block {\r\n        margin: 15px 0\r\n    }\r\n\r\n.floating-label-form-group-with-value label {\r\n    top: 0;\r\n    opacity: 1\r\n}\r\n\r\n.floating-label-form-group-with-focus label {\r\n    color: #c6d4ff\r\n}\r\n\r\nform .form-group:first-child .floating-label-form-group {\r\n    border-top: 1px solid #dee2e6\r\n}\r\n\r\nfooter {\r\n    padding: 50px 0 65px\r\n}\r\n\r\n    footer .list-inline {\r\n        margin: 0;\r\n        padding: 0\r\n    }\r\n\r\n    footer .copyright {\r\n        font-size: 14px;\r\n        margin-bottom: 0;\r\n        text-align: center\r\n    }\r\n\r\n.btn {\r\n    font-size: 14px;\r\n    font-weight: 800;\r\n    padding: 15px 25px;\r\n    letter-spacing: 1px;\r\n    text-transform: uppercase;\r\n    border-radius: 0;\r\n    font-family: 'Open Sans','Helvetica Neue',Helvetica,Arial,sans-serif\r\n}\r\n\r\n.btn-primary {\r\n    background-color: #0085a1;\r\n    border-color: #0085a1\r\n}\r\n\r\n    .btn-primary:active, .btn-primary:focus, .btn-primary:hover {\r\n        color: #fff;\r\n        background-color: #00657b !important;\r\n        border-color: #00657b !important\r\n    }\r\n\r\n.btn-lg {\r\n    font-size: 16px;\r\n    padding: 25px 35px\r\n}\r\n", ""]);

// exports


/***/ }),
/* 3 */
/***/ (function(module, exports) {

/*
	MIT License http://www.opensource.org/licenses/mit-license.php
	Author Tobias Koppers @sokra
*/
// css base code, injected by the css-loader
module.exports = function(useSourceMap) {
	var list = [];

	// return the list of modules as css string
	list.toString = function toString() {
		return this.map(function (item) {
			var content = cssWithMappingToString(item, useSourceMap);
			if(item[2]) {
				return "@media " + item[2] + "{" + content + "}";
			} else {
				return content;
			}
		}).join("");
	};

	// import a list of modules into the list
	list.i = function(modules, mediaQuery) {
		if(typeof modules === "string")
			modules = [[null, modules, ""]];
		var alreadyImportedModules = {};
		for(var i = 0; i < this.length; i++) {
			var id = this[i][0];
			if(typeof id === "number")
				alreadyImportedModules[id] = true;
		}
		for(i = 0; i < modules.length; i++) {
			var item = modules[i];
			// skip already imported module
			// this implementation is not 100% perfect for weird media query combinations
			//  when a module is imported multiple times with different media queries.
			//  I hope this will never occur (Hey this way we have smaller bundles)
			if(typeof item[0] !== "number" || !alreadyImportedModules[item[0]]) {
				if(mediaQuery && !item[2]) {
					item[2] = mediaQuery;
				} else if(mediaQuery) {
					item[2] = "(" + item[2] + ") and (" + mediaQuery + ")";
				}
				list.push(item);
			}
		}
	};
	return list;
};

function cssWithMappingToString(item, useSourceMap) {
	var content = item[1] || '';
	var cssMapping = item[3];
	if (!cssMapping) {
		return content;
	}

	if (useSourceMap && typeof btoa === 'function') {
		var sourceMapping = toComment(cssMapping);
		var sourceURLs = cssMapping.sources.map(function (source) {
			return '/*# sourceURL=' + cssMapping.sourceRoot + source + ' */'
		});

		return [content].concat(sourceURLs).concat([sourceMapping]).join('\n');
	}

	return [content].join('\n');
}

// Adapted from convert-source-map (MIT)
function toComment(sourceMap) {
	// eslint-disable-next-line no-undef
	var base64 = btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap))));
	var data = 'sourceMappingURL=data:application/json;charset=utf-8;base64,' + base64;

	return '/*# ' + data + ' */';
}


/***/ }),
/* 4 */
/***/ (function(module, exports, __webpack_require__) {

/*
	MIT License http://www.opensource.org/licenses/mit-license.php
	Author Tobias Koppers @sokra
*/

var stylesInDom = {};

var	memoize = function (fn) {
	var memo;

	return function () {
		if (typeof memo === "undefined") memo = fn.apply(this, arguments);
		return memo;
	};
};

var isOldIE = memoize(function () {
	// Test for IE <= 9 as proposed by Browserhacks
	// @see http://browserhacks.com/#hack-e71d8692f65334173fee715c222cb805
	// Tests for existence of standard globals is to allow style-loader
	// to operate correctly into non-standard environments
	// @see https://github.com/webpack-contrib/style-loader/issues/177
	return window && document && document.all && !window.atob;
});

var getTarget = function (target) {
  return document.querySelector(target);
};

var getElement = (function (fn) {
	var memo = {};

	return function(target) {
                // If passing function in options, then use it for resolve "head" element.
                // Useful for Shadow Root style i.e
                // {
                //   insertInto: function () { return document.querySelector("#foo").shadowRoot }
                // }
                if (typeof target === 'function') {
                        return target();
                }
                if (typeof memo[target] === "undefined") {
			var styleTarget = getTarget.call(this, target);
			// Special case to return head of iframe instead of iframe itself
			if (window.HTMLIFrameElement && styleTarget instanceof window.HTMLIFrameElement) {
				try {
					// This will throw an exception if access to iframe is blocked
					// due to cross-origin restrictions
					styleTarget = styleTarget.contentDocument.head;
				} catch(e) {
					styleTarget = null;
				}
			}
			memo[target] = styleTarget;
		}
		return memo[target]
	};
})();

var singleton = null;
var	singletonCounter = 0;
var	stylesInsertedAtTop = [];

var	fixUrls = __webpack_require__(5);

module.exports = function(list, options) {
	if (typeof DEBUG !== "undefined" && DEBUG) {
		if (typeof document !== "object") throw new Error("The style-loader cannot be used in a non-browser environment");
	}

	options = options || {};

	options.attrs = typeof options.attrs === "object" ? options.attrs : {};

	// Force single-tag solution on IE6-9, which has a hard limit on the # of <style>
	// tags it will allow on a page
	if (!options.singleton && typeof options.singleton !== "boolean") options.singleton = isOldIE();

	// By default, add <style> tags to the <head> element
        if (!options.insertInto) options.insertInto = "head";

	// By default, add <style> tags to the bottom of the target
	if (!options.insertAt) options.insertAt = "bottom";

	var styles = listToStyles(list, options);

	addStylesToDom(styles, options);

	return function update (newList) {
		var mayRemove = [];

		for (var i = 0; i < styles.length; i++) {
			var item = styles[i];
			var domStyle = stylesInDom[item.id];

			domStyle.refs--;
			mayRemove.push(domStyle);
		}

		if(newList) {
			var newStyles = listToStyles(newList, options);
			addStylesToDom(newStyles, options);
		}

		for (var i = 0; i < mayRemove.length; i++) {
			var domStyle = mayRemove[i];

			if(domStyle.refs === 0) {
				for (var j = 0; j < domStyle.parts.length; j++) domStyle.parts[j]();

				delete stylesInDom[domStyle.id];
			}
		}
	};
};

function addStylesToDom (styles, options) {
	for (var i = 0; i < styles.length; i++) {
		var item = styles[i];
		var domStyle = stylesInDom[item.id];

		if(domStyle) {
			domStyle.refs++;

			for(var j = 0; j < domStyle.parts.length; j++) {
				domStyle.parts[j](item.parts[j]);
			}

			for(; j < item.parts.length; j++) {
				domStyle.parts.push(addStyle(item.parts[j], options));
			}
		} else {
			var parts = [];

			for(var j = 0; j < item.parts.length; j++) {
				parts.push(addStyle(item.parts[j], options));
			}

			stylesInDom[item.id] = {id: item.id, refs: 1, parts: parts};
		}
	}
}

function listToStyles (list, options) {
	var styles = [];
	var newStyles = {};

	for (var i = 0; i < list.length; i++) {
		var item = list[i];
		var id = options.base ? item[0] + options.base : item[0];
		var css = item[1];
		var media = item[2];
		var sourceMap = item[3];
		var part = {css: css, media: media, sourceMap: sourceMap};

		if(!newStyles[id]) styles.push(newStyles[id] = {id: id, parts: [part]});
		else newStyles[id].parts.push(part);
	}

	return styles;
}

function insertStyleElement (options, style) {
	var target = getElement(options.insertInto)

	if (!target) {
		throw new Error("Couldn't find a style target. This probably means that the value for the 'insertInto' parameter is invalid.");
	}

	var lastStyleElementInsertedAtTop = stylesInsertedAtTop[stylesInsertedAtTop.length - 1];

	if (options.insertAt === "top") {
		if (!lastStyleElementInsertedAtTop) {
			target.insertBefore(style, target.firstChild);
		} else if (lastStyleElementInsertedAtTop.nextSibling) {
			target.insertBefore(style, lastStyleElementInsertedAtTop.nextSibling);
		} else {
			target.appendChild(style);
		}
		stylesInsertedAtTop.push(style);
	} else if (options.insertAt === "bottom") {
		target.appendChild(style);
	} else if (typeof options.insertAt === "object" && options.insertAt.before) {
		var nextSibling = getElement(options.insertInto + " " + options.insertAt.before);
		target.insertBefore(style, nextSibling);
	} else {
		throw new Error("[Style Loader]\n\n Invalid value for parameter 'insertAt' ('options.insertAt') found.\n Must be 'top', 'bottom', or Object.\n (https://github.com/webpack-contrib/style-loader#insertat)\n");
	}
}

function removeStyleElement (style) {
	if (style.parentNode === null) return false;
	style.parentNode.removeChild(style);

	var idx = stylesInsertedAtTop.indexOf(style);
	if(idx >= 0) {
		stylesInsertedAtTop.splice(idx, 1);
	}
}

function createStyleElement (options) {
	var style = document.createElement("style");

	if(options.attrs.type === undefined) {
		options.attrs.type = "text/css";
	}

	addAttrs(style, options.attrs);
	insertStyleElement(options, style);

	return style;
}

function createLinkElement (options) {
	var link = document.createElement("link");

	if(options.attrs.type === undefined) {
		options.attrs.type = "text/css";
	}
	options.attrs.rel = "stylesheet";

	addAttrs(link, options.attrs);
	insertStyleElement(options, link);

	return link;
}

function addAttrs (el, attrs) {
	Object.keys(attrs).forEach(function (key) {
		el.setAttribute(key, attrs[key]);
	});
}

function addStyle (obj, options) {
	var style, update, remove, result;

	// If a transform function was defined, run it on the css
	if (options.transform && obj.css) {
	    result = options.transform(obj.css);

	    if (result) {
	    	// If transform returns a value, use that instead of the original css.
	    	// This allows running runtime transformations on the css.
	    	obj.css = result;
	    } else {
	    	// If the transform function returns a falsy value, don't add this css.
	    	// This allows conditional loading of css
	    	return function() {
	    		// noop
	    	};
	    }
	}

	if (options.singleton) {
		var styleIndex = singletonCounter++;

		style = singleton || (singleton = createStyleElement(options));

		update = applyToSingletonTag.bind(null, style, styleIndex, false);
		remove = applyToSingletonTag.bind(null, style, styleIndex, true);

	} else if (
		obj.sourceMap &&
		typeof URL === "function" &&
		typeof URL.createObjectURL === "function" &&
		typeof URL.revokeObjectURL === "function" &&
		typeof Blob === "function" &&
		typeof btoa === "function"
	) {
		style = createLinkElement(options);
		update = updateLink.bind(null, style, options);
		remove = function () {
			removeStyleElement(style);

			if(style.href) URL.revokeObjectURL(style.href);
		};
	} else {
		style = createStyleElement(options);
		update = applyToTag.bind(null, style);
		remove = function () {
			removeStyleElement(style);
		};
	}

	update(obj);

	return function updateStyle (newObj) {
		if (newObj) {
			if (
				newObj.css === obj.css &&
				newObj.media === obj.media &&
				newObj.sourceMap === obj.sourceMap
			) {
				return;
			}

			update(obj = newObj);
		} else {
			remove();
		}
	};
}

var replaceText = (function () {
	var textStore = [];

	return function (index, replacement) {
		textStore[index] = replacement;

		return textStore.filter(Boolean).join('\n');
	};
})();

function applyToSingletonTag (style, index, remove, obj) {
	var css = remove ? "" : obj.css;

	if (style.styleSheet) {
		style.styleSheet.cssText = replaceText(index, css);
	} else {
		var cssNode = document.createTextNode(css);
		var childNodes = style.childNodes;

		if (childNodes[index]) style.removeChild(childNodes[index]);

		if (childNodes.length) {
			style.insertBefore(cssNode, childNodes[index]);
		} else {
			style.appendChild(cssNode);
		}
	}
}

function applyToTag (style, obj) {
	var css = obj.css;
	var media = obj.media;

	if(media) {
		style.setAttribute("media", media)
	}

	if(style.styleSheet) {
		style.styleSheet.cssText = css;
	} else {
		while(style.firstChild) {
			style.removeChild(style.firstChild);
		}

		style.appendChild(document.createTextNode(css));
	}
}

function updateLink (link, options, obj) {
	var css = obj.css;
	var sourceMap = obj.sourceMap;

	/*
		If convertToAbsoluteUrls isn't defined, but sourcemaps are enabled
		and there is no publicPath defined then lets turn convertToAbsoluteUrls
		on by default.  Otherwise default to the convertToAbsoluteUrls option
		directly
	*/
	var autoFixUrls = options.convertToAbsoluteUrls === undefined && sourceMap;

	if (options.convertToAbsoluteUrls || autoFixUrls) {
		css = fixUrls(css);
	}

	if (sourceMap) {
		// http://stackoverflow.com/a/26603875
		css += "\n/*# sourceMappingURL=data:application/json;base64," + btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap)))) + " */";
	}

	var blob = new Blob([css], { type: "text/css" });

	var oldSrc = link.href;

	link.href = URL.createObjectURL(blob);

	if(oldSrc) URL.revokeObjectURL(oldSrc);
}


/***/ }),
/* 5 */
/***/ (function(module, exports) {


/**
 * When source maps are enabled, `style-loader` uses a link element with a data-uri to
 * embed the css on the page. This breaks all relative urls because now they are relative to a
 * bundle instead of the current page.
 *
 * One solution is to only use full urls, but that may be impossible.
 *
 * Instead, this function "fixes" the relative urls to be absolute according to the current page location.
 *
 * A rudimentary test suite is located at `test/fixUrls.js` and can be run via the `npm test` command.
 *
 */

module.exports = function (css) {
  // get current location
  var location = typeof window !== "undefined" && window.location;

  if (!location) {
    throw new Error("fixUrls requires window.location");
  }

	// blank or null?
	if (!css || typeof css !== "string") {
	  return css;
  }

  var baseUrl = location.protocol + "//" + location.host;
  var currentDir = baseUrl + location.pathname.replace(/\/[^\/]*$/, "/");

	// convert each url(...)
	/*
	This regular expression is just a way to recursively match brackets within
	a string.

	 /url\s*\(  = Match on the word "url" with any whitespace after it and then a parens
	   (  = Start a capturing group
	     (?:  = Start a non-capturing group
	         [^)(]  = Match anything that isn't a parentheses
	         |  = OR
	         \(  = Match a start parentheses
	             (?:  = Start another non-capturing groups
	                 [^)(]+  = Match anything that isn't a parentheses
	                 |  = OR
	                 \(  = Match a start parentheses
	                     [^)(]*  = Match anything that isn't a parentheses
	                 \)  = Match a end parentheses
	             )  = End Group
              *\) = Match anything and then a close parens
          )  = Close non-capturing group
          *  = Match anything
       )  = Close capturing group
	 \)  = Match a close parens

	 /gi  = Get all matches, not the first.  Be case insensitive.
	 */
	var fixedCss = css.replace(/url\s*\(((?:[^)(]|\((?:[^)(]+|\([^)(]*\))*\))*)\)/gi, function(fullMatch, origUrl) {
		// strip quotes (if they exist)
		var unquotedOrigUrl = origUrl
			.trim()
			.replace(/^"(.*)"$/, function(o, $1){ return $1; })
			.replace(/^'(.*)'$/, function(o, $1){ return $1; });

		// already a full url? no change
		if (/^(#|data:|http:\/\/|https:\/\/|file:\/\/\/|\s*$)/i.test(unquotedOrigUrl)) {
		  return fullMatch;
		}

		// convert the url to a full url
		var newUrl;

		if (unquotedOrigUrl.indexOf("//") === 0) {
		  	//TODO: should we add protocol?
			newUrl = unquotedOrigUrl;
		} else if (unquotedOrigUrl.indexOf("/") === 0) {
			// path should be relative to the base url
			newUrl = baseUrl + unquotedOrigUrl; // already starts with '/'
		} else {
			// path should be relative to current directory
			newUrl = currentDir + unquotedOrigUrl.replace(/^\.\//, ""); // Strip leading './'
		}

		// send back the fixed url(...)
		return "url(" + JSON.stringify(newUrl) + ")";
	});

	// send back the fixed css
	return fixedCss;
};


/***/ })
/******/ ]);