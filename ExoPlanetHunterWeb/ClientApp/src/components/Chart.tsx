import * as React from 'react'
import * as am4core from '@amcharts/amcharts4/core'
import * as am4charts from '@amcharts/amcharts4/charts'
import { Container } from 'semantic-ui-react'
import am4themes_animated from '@amcharts/amcharts4/themes/animated'
import am4themes_dark from '@amcharts/amcharts4/themes/dark'
import { Star } from '../service/getPlanets';
import { getHertzsprungRussell } from '../service/getChart';

class Chart extends React.Component<any> {
  chart: any
  async componentDidMount() {

   let data =await getHertzsprungRussell()

    am4core.useTheme(am4themes_animated)
    am4core.useTheme(am4themes_dark)
    let chart = am4core.create('chartdiv', am4charts.XYChart)
    chart.exporting.menu = new am4core.ExportMenu()
    chart.hiddenState.properties.opacity = 0

    let valueAxisX = chart.xAxes.push(new am4charts.ValueAxis())
    let valueAxisY = chart.yAxes.push(new am4charts.ValueAxis())

    valueAxisY.title.text ="Luminosity"

    valueAxisX.title.text ="Temperature K"

  
    valueAxisX.logarithmic =true
    valueAxisX.renderer.inversed = true
    valueAxisY.logarithmic =true
    let serieslum = chart.series.push(new am4charts.LineSeries())
    serieslum.dataFields.valueX = 'temp'
    serieslum.dataFields.valueY = 'lum'
    serieslum.dataFields.value = 'size'
    
    serieslum.strokeOpacity = 0
    serieslum.sequencedInterpolation = true
    serieslum.yAxis =valueAxisY


    let bullet = serieslum.bullets.push(new am4charts.CircleBullet())
    bullet.fill = am4core.color('#ff0000')
    bullet.propertyFields.fill = 'color'
    bullet.strokeOpacity =  0.7
    bullet.strokeWidth = 2
    bullet.fillOpacity = 0.7
    bullet.stroke = am4core.color('#ffffff')
    bullet.circle.events.on("hit",(ev:any)=> {
     let name= (ev.target.dataItem as any)._dataContext.title
      this.props.history.push({
        pathname: `system/${name}`,
        state: { star: {name} as Star }
      })
    }, this);
    bullet.circle.adapter.add("tooltipText", (text:string,s:any) => {
      const size= s.dataItem._dataContext.size/100
   
      return  text.replace("{size}", size.toString())
     })
     bullet.circle.cursorOverStyle= am4core.MouseCursorStyle.pointer
    bullet.circle.tooltipText =
      '[bold]{title}:[/]\nMass: {size} M⊙︎\nTemperature: {valueX.value}\nLuminosity: {valueY.value}\nSpectral class: {type}\nConstellation: {constellation}'

    let hoverState = bullet.states.create('hover')
    hoverState.properties.fillOpacity = 1
    hoverState.properties.strokeOpacity = 1

    serieslum.heatRules.push({
      target: bullet.circle,
      min: 2,
      max: 100,
      property: 'radius'
    })

    bullet.circle.adapter.add('tooltipY', (tooltipY, target) =>{
      return -target.radius
    })

    chart.cursor = new am4charts.XYCursor()
    chart.cursor.behavior = 'zoomXY'

    chart.scrollbarX = new am4core.Scrollbar()
    chart.scrollbarY = new am4core.Scrollbar()

    chart.data = data
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
     <h3>{'Hertzsprung–Russell diagram'}</h3> 
        <div id="chartdiv" style={{ width: '100%', maxHeight: '700px', height:'99vh' }} />
      </Container>
    )
  }
}

export default Chart
