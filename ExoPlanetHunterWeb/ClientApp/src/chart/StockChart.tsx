import * as React from 'react'
import * as am4core from '@amcharts/amcharts4/core'
import { Segment } from 'semantic-ui-react'
import am4themes_animated from '@amcharts/amcharts4/themes/animated'
import { getPlanetTypes, initStockChart } from '../service/getChart'
class StockChart extends React.Component<any> {
  chart: any
  async componentDidMount() {


    var chart = initStockChart(this.props.props?this.props:this)

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
      <Segment inverted  style={{ minWidth: '250px'}}>
        <h3>{'Planets type diagram'}</h3>
        <div
          id="stockchartdiv"
          style={{ width: '100%', maxHeight: '700px', height: '69vh',margin:"10px" }}
        />
      </Segment>
    )
  }
}

export default StockChart
