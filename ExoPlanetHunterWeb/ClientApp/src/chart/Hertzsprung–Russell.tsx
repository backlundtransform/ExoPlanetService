import * as React from 'react'
import { Container } from 'semantic-ui-react'
import { getHertzsprungRussell, initBubbleChart } from '../service/getChart'

class HertzsprungRussell extends React.Component<any> {
  chart: any
  async componentDidMount() {
    let data = await getHertzsprungRussell()

    const chart = initBubbleChart(this)
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
        <div
          id="chartdiv"
          style={{ width: '100%', maxHeight: '700px', height: '99vh' }}
        />
      </Container>
    )
  }
}

export default HertzsprungRussell
