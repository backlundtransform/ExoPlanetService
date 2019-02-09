import * as React from 'react'
import {  Menu, Segment } from 'semantic-ui-react'
import { getHertzsprungRussell, initBubbleChart } from '../service/getChart'

class HertzsprungRussell extends React.Component<any> {
  chart: any
  async componentDidMount() {
    let data = await getHertzsprungRussell()

    const chart = initBubbleChart(this.props.props ? this.props : this)
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
      <React.Fragment>
        <Menu attached="top" tabular>
          <Menu.Item name="active" active={true} onClick={() => {}}>
            Habitable
          </Menu.Item>
          <Menu.Item name="2" active={false} onClick={() => {}}>
            All
          </Menu.Item>
        </Menu>
        <Segment inverted attached="bottom" style={{ minWidth: '250px' }}>
          <h3>{'Hertzsprung–Russell diagram'}</h3>
          <div
            id="bubblechartdiv"
            style={{
              width: '100%',
              maxHeight: '700px',
              height: '69vh',
              margin: '10px',
              minWidth: '250px'
            }}
          />
        </Segment>
      </React.Fragment>
    )
  }
}

export default HertzsprungRussell
