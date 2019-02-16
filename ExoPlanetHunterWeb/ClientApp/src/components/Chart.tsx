import * as React from 'react'
import { Grid, Statistic } from 'semantic-ui-react'
import HertzsprungRussell from '../chart/Hertzsprung–Russell'
import StockChart from '../chart/StockChart'
import {
  getHertzsprungRussell,
  initBubbleChart,
  initStockChart,
  getPlanetTypes
} from '../service/getChart'
import Svg, { Path } from 'react-native-svg-web'
import MdPlanet from 'react-ionicons/lib/MdPlanet'
import MdGlobe from 'react-ionicons/lib/MdGlobe'
import MdMoon from 'react-ionicons/lib/MdMoon'
export default class Chart extends React.Component<any, any> {
  constructor(props: any) {
    super(props)
    this.state = { habitableOnly: true }
  }

  bubblechart: any
  stockchart: any
  componentDidMount = async () => {
    await this.getBubbleData()
    await this.getStockData(0)
  }

  async getBubbleData() {
    let data = await getHertzsprungRussell(this.state.habitableOnly)

    const bubblechart = initBubbleChart(this)
    bubblechart.data = data
    this.bubblechart = bubblechart
  }
  async getStockData(type: number) {
    var stockchart = initStockChart(this, type)

    stockchart.data = await getPlanetTypes(type)

    this.stockchart = stockchart

    this.stockchart = stockchart
  }
  isHabitable = () => {
    this.setState({ habitableOnly: !this.state.habitableOnly }, () =>
      this.getBubbleData()
    )
  }

  setStockType = (type: number) => {
    this.setState({ habitableOnly: !this.state.habitableOnly }, () =>
      this.getStockData(type)
    )
  }

  componentWillUnmount() {
    if (this.bubblechart) {
      this.bubblechart.dispose()
    }
    if (this.stockchart) {
      this.stockchart.dispose()
    }
  }

  mainPost = () => {
    let posts = [] as Array<any>

    const options = [
      {
        key: 'hertz',

        component: <HertzsprungRussell updateParent={this.isHabitable} />
      },
      {
        key: 'stock',

        component: <StockChart updateParent={this.setStockType} />
      }
    ]
    for (let item of options) {
      posts.push(<Grid.Column key={item.key}>{item.component}</Grid.Column>)
    }

    return posts
  }

  render() {
    const main = this.mainPost()

    return (
      <React.Fragment>
        <Statistic.Group widths="three" size="small">
          <Statistic>
            <Statistic.Value>
              <MdPlanet fontSize="50px" /> 3875
            </Statistic.Value>
            <Statistic.Label>Confirmed</Statistic.Label>
            <Statistic.Label>Exoplanets</Statistic.Label>
          </Statistic>

          <Statistic>
            <Statistic.Value>
              <MdGlobe fontSize="50px" />
              55
            </Statistic.Value>
            <Statistic.Label>Potentially</Statistic.Label>
            <Statistic.Label>Habitable Planets</Statistic.Label>
          </Statistic>
          <Statistic>
            <Statistic.Value>
              <MdMoon fontSize="50px" /> 55
            </Statistic.Value>
            <Statistic.Label>Potentially</Statistic.Label>
            <Statistic.Label>Habitable Moon</Statistic.Label>
          </Statistic>
        </Statistic.Group>
        <br />
        <Grid container stackable columns={'equal'}>
          {main}
        </Grid>
      </React.Fragment>
    )
  }
}
