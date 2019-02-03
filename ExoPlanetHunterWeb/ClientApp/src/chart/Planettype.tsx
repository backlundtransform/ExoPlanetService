import * as React from 'react'
import * as am4core from '@amcharts/amcharts4/core'
import * as am4charts from '@amcharts/amcharts4/charts'
import { Container } from 'semantic-ui-react'
import am4themes_animated from '@amcharts/amcharts4/themes/animated'
import { getPlanetTypes, initStockChart } from '../service/getChart'
class PlanetTypeChart extends React.Component<any> {
  chart: any
  async componentDidMount() {
    am4core.useTheme(am4themes_animated)

    var chart = initStockChart(this)

    chart.data = await getPlanetTypes()

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
        <h3>{'Planets type diagram'}</h3>
        <div
          id="chartdiv"
          style={{ width: '100%', maxHeight: '700px', height: '99vh' }}
        />
      </Container>
    )
  }
}

export default PlanetTypeChart
