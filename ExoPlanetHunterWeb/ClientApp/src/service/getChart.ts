import { Star } from './getPlanets'
import * as am4core from '@amcharts/amcharts4/core'
import * as am4charts from '@amcharts/amcharts4/charts'
import am4themes_dark from '@amcharts/amcharts4/themes/dark'
import am4themes_animated from '@amcharts/amcharts4/themes/animated'

export const planetTypes = [
  'Mass',
  'Discovery method',
  'Zone',
  'Year',
]
export interface HertzsprungRussell {
  title: string
  type: string
  lum: number
  color: string
  size: number
  temp: number
  constellation: string
}
export interface PlanetTypes {
  title: string
  value: string
}
export interface PlanetDistance {
  angle: number
  distance: number
  title: string
}
const getData = async (uri: string): Promise<any> => {
  const data = await fetch(uri)
    .then(response => {
      return response.json()
    })
    .then(myJson => {
      return myJson
    })

  return data
}

export const getHertzsprungRussell = async (
  habitableOnly: boolean
): Promise<Array<HertzsprungRussell>> =>
  (await getData(
    `../api/Chart/HertzsprungRussell?habitableOnly=${habitableOnly}`
  )) as Promise<Array<HertzsprungRussell>>

export const getPlanetDistance = async (
  max: Number | null
): Promise<Array<PlanetDistance>> =>
  (await getData(`../api/Chart/PlanetDistance?max=${max}`)) as Promise<
    Array<PlanetDistance>
  >

export const getPlanetTypes = async (
  planetType: number
): Promise<Array<PlanetTypes>> =>
  (await getData(`../api/Chart/PlanetTypes?type=${planetType}`)) as Promise<
    Array<PlanetTypes>
  >
export const initStockChart = (
  parent: any,
  planetType: number
): am4charts.XYChart3D => {
  am4core.useTheme(am4themes_dark)
  let chart = am4core.create('stockchartdiv', am4charts.XYChart3D)
  chart.exporting.menu = new am4core.ExportMenu()

  let categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis())
  categoryAxis.dataFields.category = 'title'
  categoryAxis.renderer.labels.template.rotation = 270
  categoryAxis.renderer.labels.template.hideOversized = false
  categoryAxis.renderer.minGridDistance = 20
  categoryAxis.renderer.labels.template.horizontalCenter = 'right'
  categoryAxis.renderer.labels.template.verticalCenter = 'middle'
  categoryAxis.tooltip.label.rotation = 270
  categoryAxis.tooltip.label.horizontalCenter = 'right'
  categoryAxis.tooltip.label.verticalCenter = 'middle'

  let valueAxis = chart.yAxes.push(new am4charts.ValueAxis())
  valueAxis.title.text = 'Planets'
  valueAxis.title.fontWeight = 'bold'

  let series = chart.series.push(new am4charts.ColumnSeries3D())
  series.dataFields.valueY = 'value'
  series.dataFields.categoryX = 'title'
  series.name = 'Visits'
  series.tooltipText = '{categoryX}: [bold]{valueY}[/]'
  series.columns.template.fillOpacity = 0.8

  let columnTemplate = series.columns.template
  columnTemplate.strokeWidth = 2
  columnTemplate.cursorOverStyle = am4core.MouseCursorStyle.pointer
  columnTemplate.strokeOpacity = 1
  columnTemplate.stroke = am4core.color('#FFFFFF')
  columnTemplate.events.on(
    'hit',
    (ev: any) => {
      let key = (ev.target.dataItem as any)._dataContext.title

      parent.props.history.push({
        pathname: `../catalog`,
        state: { key, type: planetType }
      })
    },
    this
  )

  columnTemplate.adapter.add('fill', (fill, target) => {
    return chart.colors.getIndex(target.dataItem.index)
  })

  columnTemplate.adapter.add('stroke', (stroke, target) => {
    return chart.colors.getIndex(target.dataItem.index)
  })

  chart.cursor = new am4charts.XYCursor()
  chart.cursor.lineX.strokeOpacity = 0
  chart.cursor.lineY.strokeOpacity = 0

  return chart
}

export const initBubbleChart = (parent: any): am4charts.XYChart => {
  am4core.useTheme(am4themes_dark)
  let chart = am4core.create('bubblechartdiv', am4charts.XYChart)
  chart.exporting.menu = new am4core.ExportMenu()

  chart.hiddenState.properties.opacity = 0

  let valueAxisX = chart.xAxes.push(new am4charts.ValueAxis())
  let valueAxisY = chart.yAxes.push(new am4charts.ValueAxis())

  valueAxisY.title.text = 'Luminosity'

  valueAxisX.title.text = 'Temperature K'
  valueAxisX.renderer.labels.template.rotation = 45

  valueAxisX.logarithmic = true
  valueAxisX.renderer.inversed = true
  valueAxisY.logarithmic = true

  chart.cursor = new am4charts.XYCursor()
  chart.cursor.behavior = 'zoomXY'
  chart.responsive.enabled = true
  chart.scrollbarX = new am4core.Scrollbar()
  chart.scrollbarY = new am4core.Scrollbar()

  let series = chart.series.push(new am4charts.LineSeries())
  series.dataFields.valueX = 'temp'
  series.dataFields.valueY = 'lum'
  series.dataFields.value = 'size'

  series.strokeOpacity = 0
  series.sequencedInterpolation = true
  series.yAxis = valueAxisY

  let bullet = series.bullets.push(new am4charts.CircleBullet())
  bullet.fill = am4core.color('#ff0000')
  bullet.propertyFields.fill = 'color'
  bullet.strokeOpacity = 0.7
  bullet.strokeWidth = 2
  bullet.fillOpacity = 0.7
  bullet.stroke = am4core.color('#ffffff')
  bullet.circle.events.on(
    'hit',
    (ev: any) => {
      let name = (ev.target.dataItem as any)._dataContext.title
      parent.props.history.push({
        pathname: `../system/${name}`,
        state: { star: { name } as Star }
      })
    },
    this
  )
  bullet.circle.adapter.add('tooltipText', (text: string, s: any) => {
    const size = s.dataItem._dataContext.size / 100

    return text.replace('{size}', size.toString())
  })
  bullet.circle.cursorOverStyle = am4core.MouseCursorStyle.pointer
  bullet.circle.tooltipText =
    '[bold]{title}:[/]\nMass: {size} M⊙︎\nTemperature: {valueX.value}\nLuminosity: {valueY.value}\nSpectral class: {type}\nConstellation: {constellation}'

  let hoverState = bullet.states.create('hover')
  hoverState.properties.fillOpacity = 1
  hoverState.properties.strokeOpacity = 1

  series.heatRules.push({
    target: bullet.circle,
    min: 2,
    max: 100,
    property: 'radius'
  })

  bullet.circle.adapter.add('tooltipY', (tooltipY, target) => {
    return -target.radius
  })
  return chart
}

export const initPolarChart = (
  seriesData: Array<PlanetDistance> = [],
  parent: any
): am4charts.RadarChart => {
  am4core.useTheme(am4themes_animated)
  let chart = am4core.create('polarchartdiv', am4charts.RadarChart)
  let xAxis = chart.xAxes.push(new am4charts.ValueAxis() as any)
  xAxis.renderer.maxLabelPosition = 0.99
  let yAxis = chart.yAxes.push(new am4charts.ValueAxis() as any)
  yAxis.renderer.labels.template.verticalCenter = 'bottom'
  yAxis.renderer.labels.template.horizontalCenter = 'right'
  yAxis.renderer.maxLabelPosition = 0.99
  yAxis.renderer.labels.template.paddingBottom = 1
  yAxis.renderer.labels.template.paddingRight = 3
  let series1 = chart.series.push(new am4charts.RadarSeries())
  let bullet = series1.bullets.push(new am4charts.CircleBullet())
  series1.strokeOpacity = 0
  series1.dataFields.valueX = 'angle'
  series1.dataFields.valueY = 'distance'
  series1.name='potentially habitable exoplanets'
  var image = bullet.createChild(am4core.Image)
  image.href = '/img/ic_launcher_web.png'
  image.width = 30
  image.height = 30
  image.horizontalCenter = 'middle'
  image.verticalCenter = 'middle'
  image.adapter.add('tooltipText', (text: string, s: any) => {
     const title = s.dataItem._dataContext.title
     return text.replace('{title}', title)
  })
  image.adapter.add('tooltipY', (tooltipY, target) => {
    return -target
  })
  image.tooltipText = '[bold]{title}[/]'
 image.events.on(
    'hit',
    (ev: any) => {
      let name = (ev.target.dataItem as any)._dataContext.title
      parent.props.history.push({
        pathname: `../planet/${name}`,
      
      })
    },
    this
  )

  image.cursorOverStyle = am4core.MouseCursorStyle.pointer
  series1.sequencedInterpolation = true
  series1.sequencedInterpolationDelay = 10
  seriesData.push({distance:0, angle:24, title:'Tellus' })
  series1.data = seriesData
  chart.legend = new am4charts.Legend()
  chart.cursor = new am4charts.RadarCursor()
  chart.cursor.behavior = 'none'
  return chart
}
