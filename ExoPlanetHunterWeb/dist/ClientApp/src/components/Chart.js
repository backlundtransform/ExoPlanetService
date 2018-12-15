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
import * as React from 'react';
import * as am4core from '@amcharts/amcharts4/core';
import * as am4charts from '@amcharts/amcharts4/charts';
import { Container } from 'semantic-ui-react';
import am4themes_animated from '@amcharts/amcharts4/themes/animated';
import am4themes_dark from '@amcharts/amcharts4/themes/dark';
var Chart = /** @class */ (function (_super) {
    __extends(Chart, _super);
    function Chart() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Chart.prototype.componentDidMount = function () {
        var _this = this;
        am4core.useTheme(am4themes_animated);
        am4core.useTheme(am4themes_dark);
        var chart = am4core.create('chartdiv', am4charts.XYChart);
        chart.exporting.menu = new am4core.ExportMenu();
        chart.hiddenState.properties.opacity = 0;
        var valueAxisX = chart.xAxes.push(new am4charts.ValueAxis());
        var valueAxisY = chart.yAxes.push(new am4charts.ValueAxis());
        var valueAxisY2 = chart.yAxes.push(new am4charts.ValueAxis());
        valueAxisY.title.text = "Luminosity";
        valueAxisY2.title.text = "Absolute magnitude";
        valueAxisX.title.text = "Temperature K";
        valueAxisY2.renderer.opposite = true;
        valueAxisY.logarithmic = true;
        var serieslum = chart.series.push(new am4charts.LineSeries());
        serieslum.dataFields.valueX = 'temp';
        serieslum.dataFields.valueY = 'lum';
        serieslum.dataFields.value = 'size';
        serieslum.strokeOpacity = 0;
        serieslum.sequencedInterpolation = true;
        serieslum.yAxis = valueAxisY;
        var seriesmag = chart.series.push(new am4charts.LineSeries());
        seriesmag.dataFields.valueX = 'temp';
        seriesmag.dataFields.valueY = 'mag';
        seriesmag.dataFields.value = 'size';
        seriesmag.strokeOpacity = 0;
        seriesmag.sequencedInterpolation = true;
        seriesmag.yAxis = valueAxisY2;
        var bullet = serieslum.bullets.push(new am4charts.CircleBullet());
        bullet.fill = am4core.color('#ff0000');
        bullet.propertyFields.fill = 'color';
        bullet.strokeOpacity = 0;
        bullet.strokeWidth = 2;
        bullet.fillOpacity = 0.7;
        bullet.stroke = am4core.color('#ffffff');
        bullet.circle.events.on("hit", function (ev) {
            var name = ev.target.dataItem._dataContext.title;
            _this.props.history.push({
                pathname: "system/" + name,
                state: { star: { name: name } }
            });
            console.log("clicked on ", ev.target.dataItem._dataContext);
        }, this);
        bullet.circle.tooltipText =
            '[bold]{title}:[/]\nMass: {value.value}\nTemperature: {valueX.value}\nLuminosity:{valueY.value}';
        var hoverState = bullet.states.create('hover');
        hoverState.properties.fillOpacity = 1;
        hoverState.properties.strokeOpacity = 1;
        serieslum.heatRules.push({
            target: bullet.circle,
            min: 2,
            max: 60,
            property: 'radius'
        });
        bullet.circle.adapter.add('tooltipY', function (tooltipY, target) {
            return -target.radius;
        });
        chart.cursor = new am4charts.XYCursor();
        chart.cursor.behavior = 'zoomXY';
        chart.scrollbarX = new am4core.Scrollbar();
        chart.scrollbarY = new am4core.Scrollbar();
        chart.data = [
            {
                title: 'Kepler-283',
                type: "O",
                color: '#eea638',
                constellation: 'Centaurus',
                lum: 10000,
                temp: 10000.524,
                size: 200,
                mag: -5
            }, {
                title: 'Kepler-298',
                type: "B",
                color: '#eea638',
                constellation: 'Centaurus',
                lum: 1,
                temp: 5000.524,
                size: 334,
                mag: 5
            }, {
                title: 'KOI-4427',
                type: "B",
                color: '#eea638',
                constellation: 'Centaurus',
                lum: 0.00001,
                temp: 3000.524,
                size: 134,
                mag: 5
            }
        ];
        this.chart = chart;
    };
    Chart.prototype.componentWillUnmount = function () {
        if (this.chart) {
            this.chart.dispose();
        }
    };
    Chart.prototype.render = function () {
        return (React.createElement(Container, { className: 'post-preview' },
            React.createElement("div", { id: "chartdiv", style: { width: '100%', maxHeight: '700px', height: '99vh' } })));
    };
    return Chart;
}(React.Component));
export default Chart;
