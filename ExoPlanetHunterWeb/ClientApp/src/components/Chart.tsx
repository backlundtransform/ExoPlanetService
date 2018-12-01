import * as React from 'react'
import * as am4core from '@amcharts/amcharts4/core'
import * as am4charts from '@amcharts/amcharts4/charts'
import { Container } from 'semantic-ui-react'
import am4themes_animated from '@amcharts/amcharts4/themes/animated'
import am4themes_dark from '@amcharts/amcharts4/themes/dark'
import { Star } from '../service/getPlanets';

class Chart extends React.Component<any> {
  chart: any
  componentDidMount() {
    am4core.useTheme(am4themes_animated)
    am4core.useTheme(am4themes_dark)
    let chart = am4core.create('chartdiv', am4charts.XYChart)
    chart.exporting.menu = new am4core.ExportMenu()
    chart.hiddenState.properties.opacity = 0

    let valueAxisX = chart.xAxes.push(new am4charts.ValueAxis())
    let valueAxisY = chart.yAxes.push(new am4charts.ValueAxis())

    let valueAxisY2 = chart.yAxes.push(new am4charts.ValueAxis())
    valueAxisY.title.text ="Luminosity"
    valueAxisY2.title.text ="Absolute magnitude"
    valueAxisX.title.text ="Temperature K"

    valueAxisY2.renderer.opposite = true
   
    valueAxisY.logarithmic =true
    let serieslum = chart.series.push(new am4charts.LineSeries())
    serieslum.dataFields.valueX = 'temp'
    serieslum.dataFields.valueY = 'lum'
    serieslum.dataFields.value = 'size'
    
    serieslum.strokeOpacity = 0
    serieslum.sequencedInterpolation = true
    serieslum.yAxis =valueAxisY

    let seriesmag = chart.series.push(new am4charts.LineSeries())
    seriesmag.dataFields.valueX = 'temp'
    seriesmag.dataFields.valueY = 'mag'
    seriesmag.dataFields.value = 'size'
    
    seriesmag.strokeOpacity = 0
    seriesmag.sequencedInterpolation = true
    seriesmag.yAxis =valueAxisY2

    let bullet = serieslum.bullets.push(new am4charts.CircleBullet())
    bullet.fill = am4core.color('#ff0000')
    bullet.propertyFields.fill = 'color'
    bullet.strokeOpacity = 0
    bullet.strokeWidth = 2
    bullet.fillOpacity = 0.7
    bullet.stroke = am4core.color('#ffffff')
    bullet.circle.events.on("hit",(ev)=> {
     let name= (ev.target.dataItem as any)._dataContext.title
      this.props.history.push({
        pathname: `system/${name}`,
        state: { star: {name} as Star }
      })
      console.log("clicked on ", (ev.target.dataItem as any)._dataContext);
    }, this);
    bullet.circle.tooltipText =
      '[bold]{title}:[/]\nMass: {value.value}\nTemperature: {valueX.value}\nLuminosity:{valueY.value}'

    let hoverState = bullet.states.create('hover')
    hoverState.properties.fillOpacity = 1
    hoverState.properties.strokeOpacity = 1

    serieslum.heatRules.push({
      target: bullet.circle,
      min: 2,
      max: 60,
      property: 'radius'
    })

    bullet.circle.adapter.add('tooltipY', (tooltipY, target) =>{
      return -target.radius
    })

    chart.cursor = new am4charts.XYCursor()
    chart.cursor.behavior = 'zoomXY'

    chart.scrollbarX = new am4core.Scrollbar()
    chart.scrollbarY = new am4core.Scrollbar()

    chart.data = [
      {
        title: 'Kepler-283',
        type:"O",
        color: '#eea638',
        constellation: 'Centaurus',
        lum: 10000,
        temp:10000.524,
        size: 200,
        mag:-5
      }, {
        title: 'Kepler-298',
        type:"B",
        color: '#eea638',
        constellation: 'Centaurus',
        lum: 1,
        temp: 5000.524,
        size: 334,
        mag:5
      }, {
        title: 'KOI-4427',
        type:"B",
        color: '#eea638',
        constellation: 'Centaurus',
        lum: 0.00001,
        temp: 3000.524,
        size: 134,
        mag:5
      }
     
     
    ]

    this.chart = chart
  }

  componentWillUnmount() {
    if (this.chart) {
      this.chart.dispose()
    }
  }

  render() {
    return (
      <Container className={'post-preview'}>
        <div id="chartdiv" style={{ width: '100%', maxHeight: '700px', height:'99vh' }} />
      </Container>
    )
  }
}

export default Chart
