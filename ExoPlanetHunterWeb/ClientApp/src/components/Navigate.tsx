import * as React from 'react'
import { BrowserRouter as Router, Route, Link } from 'react-router-dom'
import Catalog from './Catalog'
import Map from './Map'
import Chart from './Chart'
import PlanetInfo from './PlanetInfo'
import Simulator from './Simulator'
import TransitFinder from './TransitsFinder'
import Constellations from './Constellations'
import StarInfo from './StarInfo'
import HertzsprungRussell  from '../chart/Hertzsprung–Russell'
import StockChart from '../chart/StockChart'
import { Menu, Icon} from 'semantic-ui-react'

export default class Navigate extends React.Component {
  state = { activeItem: 'catalog'}

  async componentDidMount() {
    this.setState({ activeItem: window.location.pathname })
  }
  handleItemClick = (e: any, { name }: any) =>
    this.setState({ activeItem: name })

  render() {
    const {  activeItem } = this.state
 
    return (
      <React.Fragment>
        <Router >
          <React.Fragment>
            <Menu icon="labeled" pointing secondary inverted stackable>
              <a className="item" href="/">
                <Icon name="home" />
                {'Home'}
              </a>
              <Menu.Item
                name="/dashboard"
                active={activeItem === '/dashboard'}
                as={Link}
                to="/dashboard"
                onClick={this.handleItemClick}
              >
                <Icon name="chart line" />
                {'Dashboard'}
              </Menu.Item>

              <Menu.Item
                name="/catalog"
                active={activeItem === '/catalog'}
                as={Link}
                to="/catalog"
                onClick={this.handleItemClick}
              >
                <Icon name="book" />
                {'Catalog'}
              </Menu.Item>
              <Menu.Item
                name="/map"
                active={activeItem === '/map'}
                as={Link}
                to="/map"
                onClick={this.handleItemClick}
              >
                <Icon name="map" />
                {'Star map'}
              </Menu.Item>
              {/* <Menu.Item
                name="/finder"
                active={activeItem === '/finder'}
                as={Link}
                to="/finder"
                onClick={this.handleItemClick}
              >
                <Icon name="search" />
                {'Transit finder'}
              </Menu.Item> */}
           
            </Menu>
            <Route exact path="/catalog" component={Catalog} />
            <Route exact path="/Map" component={Map} />
            <Route exact path="/dashboard" component={Chart} />
            <Route exact path="/dashboard/hertz" component={HertzsprungRussell} />
            <Route exact path="/dashboard/planets" component={StockChart} />
            {/* <Route exact path="/finder" component={TransitFinder} /> */}
        
            <Route
              name="planet"
              path="/planet/:planetId"
              component={PlanetInfo}
            />
                <Route
              name="star"
              path="/star/:starId"
              component={StarInfo}
            />
            <Route name="system" path="/system/:starId" component={Simulator} />
            <Route
              name="constellation"
              path="/constellation/:constellationId"
              component={Constellations}
            />
          </React.Fragment>
        </Router>
      </React.Fragment>
    )
  }
}
