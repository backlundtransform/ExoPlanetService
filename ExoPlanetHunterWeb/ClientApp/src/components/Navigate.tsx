import * as React from 'react'
import { BrowserRouter as Router, Route, Link } from 'react-router-dom'
import Catalog from './Catalog'
import Map from './Map'
import Chart from './Chart'
import PlanetInfo from './PlanetInfo'
import Simulator from './Simulator'
import Constellations from './Constellations'
import StarInfo from './StarInfo'

import { Menu, Icon, Search } from 'semantic-ui-react'

export default class Navigate extends React.Component {
  state = { activeItem: 'catalog', isLoading: false, value: '', results: {} }

  async componentDidMount() {
    this.setState({ activeItem: window.location.pathname })
  }
  handleItemClick = (e: any, { name }: any) =>
    this.setState({ activeItem: name })
  handleResultSelect = (e: any, { result }: any) =>
    this.setState({ value: result.title, isLoading: false })

  handleSearchChange = (e: any, { value }: any) => {
    this.setState({ isLoading: false, value })
  }

  render() {
    const { isLoading, value, results, activeItem } = this.state
    return (
      <React.Fragment>
        <Router>
          <React.Fragment>
            <Menu icon="labeled" pointing secondary inverted stackable>
              <a className="item" href="/">
                <Icon name="home" />
                {'Home'}
              </a>

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
              <Menu.Item
                name="/chart"
                active={activeItem === '/chart'}
                as={Link}
                to="/chart"
                onClick={this.handleItemClick}
              >
                <Icon name="chart line" />
                {'Chart'}
              </Menu.Item>
              <Menu.Item position="right">
                <br />
                <Search
                  loading={isLoading}
                  onResultSelect={this.handleResultSelect}
                  onSearchChange={this.handleSearchChange}
                  results={results}
                  value={value}
                  {...this.props}
                />
              </Menu.Item>
            </Menu>
            <Route exact path="/catalog" component={Catalog} />
            <Route exact path="/Map" component={Map} />
            <Route exact path="/chart" component={Chart} />
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
