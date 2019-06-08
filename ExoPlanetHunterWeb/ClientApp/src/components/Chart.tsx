import * as React from 'react'
import { Grid, Statistic } from 'semantic-ui-react'
import HertzsprungRussell from '../chart/Hertzsprung–Russell'
import StockChart from '../chart/StockChart'
import  Distance from '../chart/DistanceChart'
import {
  getHertzsprungRussell,
  getPlanetDistance,
  initBubbleChart,
  initPolarChart,
  initStockChart,
  getPlanetTypes
} from '../service/getChart'
import { Link } from 'react-router-dom'

import { GetStatisticsAsync , statistics} from '../service/getPlanets'
import MdPlanet from 'react-ionicons/lib/MdPlanet'
import MdGlobe from 'react-ionicons/lib/MdGlobe'
import MdMoon from 'react-ionicons/lib/MdMoon'
import { XYChart,XYChart3D } from '@amcharts/amcharts4/charts';
import { NumberFormatter } from '@amcharts/amcharts4/core';
export default class Chart extends React.Component<any, any> {
  constructor(props: any) {
    super(props)
    this.state = { habitableOnly: true, distance:2500,max:2500,
      stat:{} as statistics
    }
  }

  bubblechart: XYChart
  stockchart: XYChart3D
  polarchart: any
  componentDidMount = async () => {
    await this.getBubbleData()
    await this.getStockData(0)
    await this.getPolarData(null)
  const stat =  await  GetStatisticsAsync()
  this.setState({stat})
  }

  async getBubbleData() {
    let data = await getHertzsprungRussell(this.state.habitableOnly)

    const bubblechart = initBubbleChart(this)
    bubblechart.data = data
    this.bubblechart = bubblechart
  }

  async getPolarData(distance:number | null) {
  let data = await getPlanetDistance(distance)
   const max =Math.max(...data.map(p=>p.distance))
   this.polarchart= initPolarChart(data,this)
    this.setState({distance:max, max:distance==null?max: this.state.max})
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
    if (this.polarchart) {
      this.polarchart.dispose()
    }
  }
  ZoomCallback=async(factor:number)=>{
    let {distance} = this.state 
    distance = (factor+0.5) *distance
    await this.getPolarData(distance)
    this.setState({distance })
  }
  mainPost = () => {
    let posts = [] as Array<any>
    let {distance, max} = this.state 
    const options = [
      {
        key: 'hertz',

        component: <HertzsprungRussell updateParent={this.isHabitable} />
      },
      {
        key: 'stock',

        component: <StockChart updateParent={this.setStockType} />
      },
      
    ]
    for (let item of options) {
      posts.push(<Grid.Column key={item.key}>{item.component}</Grid.Column>)
    }
    posts.push(<Grid.Row key={'polar'}><Grid.Column><Distance ZoomCallback={async(factor:number)=>this.ZoomCallback(factor)} distance={distance} max={max}/></Grid.Column></Grid.Row>)
    return posts
  }

  render() {
    const main = this.mainPost()
    const stat= this.state.stat  as statistics 

    return (
      <React.Fragment>
        <Statistic.Group widths="three" size="small">
          <Statistic>  <Link
          className="ui statistic"
                to={{
                  pathname: `/catalog`,
                }}
              >
            <Statistic.Value>
              <MdPlanet fontSize="50px" /> {stat.confirmedPlanets}
            </Statistic.Value>
            <Statistic.Label>Confirmed</Statistic.Label>
            <Statistic.Label>Exoplanets</Statistic.Label></Link>
          </Statistic>

          <Statistic>
          <Link
          className="ui statistic"
                to={{
                  pathname: `/catalog`,
                  state: {  selectedvalue: "Hab"}
                }}
              >
            <Statistic.Value>
              <MdGlobe fontSize="50px" />
              {stat.confirmedHabitablePlanets}
            </Statistic.Value>
            <Statistic.Label>Potentially</Statistic.Label>
            <Statistic.Label>Habitable Planets</Statistic.Label></Link>
          </Statistic>
          <Statistic>
          <Link
          className="ui statistic"
                to={{
                  pathname: `/catalog`,
                  state: { selectedvalue: "Moons" }
                }}
              >
            <Statistic.Value>
              <MdMoon fontSize="50px" />      {stat.possibleHabitableMoons}
            </Statistic.Value>
            <Statistic.Label>Potentially</Statistic.Label>
            <Statistic.Label>Habitable Moons</Statistic.Label><Statistic.Label>Candidates</Statistic.Label></Link>
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
