import  * as React from "react"
import { BrowserRouter as Router,Route, Link } from 'react-router-dom'

import { Catalog } from './Catalog'
import { Map} from './Map'
import {Menu, Icon} from 'semantic-ui-react'

export default class Navigate extends React.Component {
  state = { activeItem: 'catalog' }

  handleItemClick = (e:any, { name }:any) => this.setState({ activeItem: name })

  render() {
    const { activeItem } = this.state
    return (
      <React.Fragment>
<Router><React.Fragment>
        <Menu icon='labeled' pointing secondary inverted>
        <a className="item" href="/"><Icon name='home' />
{"Home"}</a>

<Menu.Item name='catalog' active={activeItem === 'catalog'}  as={Link} to='/catalog' onClick={this.handleItemClick}>
<Icon name='book' />
{"Catalog"}
</Menu.Item>
<Menu.Item name='map' active={activeItem === 'map'}  as={Link} to='/map' onClick={this.handleItemClick}>
<Icon name='map' />
{"Star Map"}
</Menu.Item></Menu>
     <Route exact path="/catalog" component={Catalog} />
     <Route exact path="/Map" component={Map} />
</React.Fragment>
      </Router></React.Fragment>
    );
  }
}


