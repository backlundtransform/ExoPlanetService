
import * as React from 'react'
import { ReactSVGPanZoom } from 'react-svg-pan-zoom'
import { Star, GetPlanetAsync } from '../service/getPlanets'
import { getStarSize, getSolarSystem } from '../service/getSolarSystem'
import { Gradient } from '../styles/radialgradients'
import {Loader,Dimmer} from 'semantic-ui-react'

import Svg, {
  Circle,
  Ellipse,

  Image,
  ClipPath,
  Text,
  Defs,
  G,
  Rect
} from 'react-native-svg-web'
interface SimulatorState {
  x: number
  y: number
  alpha: number
  star: Star
  loading: boolean

}
interface SimulatorProps {
  location: any
  history: any

}
export default class Simulator extends React.Component<
  SimulatorProps,
  SimulatorState
> {
  state = {
    x: 0,
    y: 0,
    alpha: 0,
    star: {} as Star,
    loading: true,
  
  }
  _viewer = null;
 _isMounted = false;
 _interval:any
 

  async componentWillMount() {
    const { location, match } = this.props as any
    this._isMounted = true;

  
  
    const starname =location.state!==undefined?location.state.star:{ name:match.params.starId}
   
    const star = await getSolarSystem(starname)
    star.radius = getStarSize(star)
    this._interval = setInterval(() => this.updateHandler(), 1000)
   
      this._isMounted&& this.setState({ star, loading: false },()=>this._viewer&&this._viewer.zoomOnViewerCenter(1))
   
  }
  updateHandler = () => this.setState({ alpha: this.state.alpha + 1 / 50 })
  RotateX = (cx: number, rx: number) =>
    cx + rx* Math.cos(this.state.alpha + rx)
  RotateY = (cy: number, ry: number) =>
    cy + ry * Math.sin(this.state.alpha + ry / 0.3)

  navigateToPlanet = async (planet: any) => {
    if (planet.id !== undefined) {
      var planetinfo = await GetPlanetAsync(planet.id)
      planetinfo &&
        this.props.history.push({
          pathname: `/planet/${planetinfo.name}`,
          state: { planet: planetinfo }
        })
    }
  }
  componentWillUnmount(){
    this._isMounted = false;
    clearInterval(this._interval);
  }

 

  render() {
    const { star, loading } = this.state
    let width = window.innerWidth - 20

    let height = window.innerHeight - 120


    return (
      <div className={'space'}>
        {loading ? (
       <Dimmer active>
       <Loader  />
       </Dimmer>
        ) : (
          <ReactSVGPanZoom
          onChangeValue={(e:any)=>console.log(e.d)}
             ref={Viewer => this._viewer = Viewer}
             
            width={width}
            height={height}
            SVGBackground={'transparent'}
            background={'transparent'}
            onClick={event => this.navigateToPlanet(event.originalEvent.target)}
          >
            <svg x={0} y={0} height={height} width={width}>
              {Gradient()}
              <Circle
                      
                      cx={width / 2 }
                      cy={height/2}
                      r={star.radius}
                      fillOpacity={1}
                      fill={`url(#b${
                        star.color != null ? star.color.toString() : 2
                      })`}
                      style={{ cursor: 'pointer' }}
                    />
              <Rect
                x={width / 2 - (Math.PI * star.radius) / 2}
                y={height/2}
                width={star.radius * Math.PI}
                height={star.radius * Math.PI}
                fill={`url(#Star${
                  star.color != null ? star.color.toString() : 2
                })`}
              />

              {star.habZoneMax != null ? (
                <Ellipse
                  cx={width / 2}
                  cy={height / 2}
                  rx={star.habZoneMax}
                  ry={star.habZoneMax * 0.3}
                  stroke="blue"
                  strokeWidth="1"
                  fillOpacity="0"
                />
              ) : (
                <React.Fragment />
              )}

              {star.habZoneMax != null ? (
                <Ellipse
                  cx={width / 2}
                  cy={height / 2}
                  rx={star.habZoneMin}
                  ry={star.habZoneMin * 0.3}
                  stroke="red"
                  strokeWidth="1"
                  fillOpacity="0"
                />
              ) : (
                <React.Fragment />
              )}

              {star.planets.map((p, index) => {
                return (
                  <G key={p.name}>
                    <Defs>
                      <ClipPath id={index.toString()}>
                        <Circle
                          cx={this.RotateX(width / 2+p.starDistance*p.eccentricity/2, p.starDistance)}
                          cy={this.RotateY(height / 2, p.starDistance * 0.3)}
                          r={p.radius}
                        />
                      </ClipPath>
                    </Defs>
                    <Image
                      width="180"
                      height="190"
                      x={this.RotateX(width / 2 - 2 * p.radius+p.starDistance*p.eccentricity/2, p.starDistance)}
                      y={this.RotateY(
                        height / 2 - 2 * p.radius,
                        p.starDistance * 0.3
                      )}
                  
                      href={`../img/${p.img.uri}.jpg`}
                      clipPath={`url(#${index.toString()})`}
                    />
                    <Circle
                      id={p.name}
                      cx={this.RotateX(width / 2+p.starDistance*p.eccentricity/2, p.starDistance)}
                      cy={this.RotateY(height / 2, p.starDistance * 0.3)}
                      r={p.radius}
                      fillOpacity={0.4}
                      fill={`url(#${p.img.uri})`}
                      style={{ cursor: 'pointer' }}
                    />
                    />
                    <Text
                      key={`text- ${index}`}
                      x={this.RotateX(width / 2+p.starDistance*p.eccentricity/2, p.starDistance)}
                      y={
                        p.radius +
                        this.RotateY(height / 2, p.starDistance * 0.3)
                      }
                      textAnchor="end"
                      id={p.name}
                      fontWeight="bold"
                      fontSize="18"
                      fill="white"
                      style={{ cursor: 'pointer' }}
                    >
                      {p.name}
                    </Text>
                  </G>
                )
              })}
              <Rect
                x={width / 2 - (Math.PI * star.radius) / 2}
                y={(height/2- (Math.PI * star.radius)+0.2)}
                width={star.radius * Math.PI}
                height={star.radius * Math.PI}
                fill={`url(#StarTop${
                  star.color != null ? star.color.toString() : 2
                })`}
              />
            </svg>
          </ReactSVGPanZoom>
        )}
      </div>
    )
  }
}
