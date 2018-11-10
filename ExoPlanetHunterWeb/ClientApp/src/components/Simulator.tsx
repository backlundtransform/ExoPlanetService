import * as React from 'react';
import { Container } from 'semantic-ui-react'
import{Planet, Star, GetPlanetAsync,} from '../service/getPlanets'
interface SimulatorState{x:number,y:number, alpha:number, star:Star,loading:boolean}
interface SimulatorProps{location:any}
export class Simulator extends  React.Component<SimulatorProps,SimulatorState> {
state = {
      x:0,
      y:0,
      alpha:0,
      star:{} as Star,
     loading:true
}
 

    async componentDidMount(){
        }
      
  render() {
    const {location} = this.props as any

    return (
      <Container className={"post-preview"}>
 {location.state.star.name}
      </Container>
    );
  }
}