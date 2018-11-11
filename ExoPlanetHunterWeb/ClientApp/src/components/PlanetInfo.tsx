import * as React from 'react';
import { Container } from 'semantic-ui-react'
export default class PlanetInfo extends  React.Component {
 
    async componentDidMount(){
        }
      
  render() {
    const {location} = this.props as any

    return (
      <Container className={"post-preview"}>
 {location.state.planet.name}
      </Container>
    );
  }
}