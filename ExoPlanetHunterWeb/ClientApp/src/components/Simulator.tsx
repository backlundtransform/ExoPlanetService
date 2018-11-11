import * as React from "react"
import { ReactSVGPanZoom } from "react-svg-pan-zoom"
import { Star, storeBase64, GetPlanetAsync } from "../service/getPlanets"
import { getStarSize, getSolarSystem } from "../service/getSolarSystem"
import { Gradient } from "../styles/radialgradients"

import Svg, {
  Circle,
  Ellipse,
  Path,
  Image,
  ClipPath,
  Text,
  Defs,
  G
} from "react-native-svg-web"
interface SimulatorState {
  x: number
  y: number
  alpha: number
  star: Star
  loading: boolean
  color: Array<any>
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
    color: [] as Array<any>
  }

  async componentWillMount() {
    const { location } = this.props as any

    const star = await getSolarSystem(location.state.star)

    let color = JSON.parse(await storeBase64())

    star.radius = getStarSize(star)
    setInterval(() => this.updateHandler(), 1000)
    this.setState({ star, loading: false, color })
  }
  updateHandler = () => this.setState({ alpha: this.state.alpha + 1 / 50 })
  RotateX = (cx: number, rx: number) =>
    cx + rx * Math.cos(this.state.alpha + rx)
  RotateY = (cy: number, ry: number) =>
    cy + ry * Math.sin(this.state.alpha + ry / 0.3)

  navigateToPlanet = async (planet: any) => {
    if (planet.id !== undefined) {
      var planetinfo = await GetPlanetAsync(planet.id)

      this.props.history.push({
        pathname: `/planet/${planetinfo.name}`,
        state: { planet: planetinfo }
      })
    }
  }

  render() {
    const { star, loading, color } = this.state
    let width = window.innerWidth - 20

    let height = window.innerHeight - 120

    return (
      <div className={"space"}>
        {loading ? (
          <React.Fragment />
        ) : (
          <ReactSVGPanZoom
            width={width}
            height={height}
            SVGBackground={"transparent"}
            background={"transparent"}
            onClick={event => this.navigateToPlanet(event.originalEvent.target)}
          >
            <Svg x={0} y={0} height={height} width={width}>
              {Gradient()}

              <Path
                d={`M${width / 2 - star.radius},${height /
                  2} a1,1 0 0,0 ${star.radius * 2},0`}
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
                  <G key={index.toString()}>
                    <Defs>
                      <ClipPath id={index.toString()}>
                        <Circle
                          cx={this.RotateX(width / 2, p.starDistance)}
                          cy={this.RotateY(height / 2, p.starDistance * 0.3)}
                          r={p.radius}
                        />
                      </ClipPath>
                    </Defs>
                    <Image
                      width="180"
                      height="190"
                      x={this.RotateX(width / 2 - 2 * p.radius, p.starDistance)}
                      y={this.RotateY(
                        height / 2 - 2 * p.radius,
                        p.starDistance * 0.3
                      )}
                      href={`${color[p.img.uri]}`}
                      clipPath={`url(#${index.toString()}`}
                      style={{ cursor: "pointer" }}
                    />
                    <Circle
                      id={p.name}
                      cx={this.RotateX(width / 2, p.starDistance)}
                      cy={this.RotateY(height / 2, p.starDistance * 0.3)}
                      r={p.radius}
                      fillOpacity={0.4}
                      fill={`url(#${p.img.uri})`}
                    />
                    />
                    <Text
                      key={`text- ${index}`}
                      x={p.radius + this.RotateX(width / 2, p.starDistance)}
                      y={
                        p.radius +
                        this.RotateY(height / 2, p.starDistance * 0.3)
                      }
                      textAnchor="end"
                      id={p.name}
                      fontWeight="bold"
                      fontSize="18"
                      fill="white"
                    >
                      {p.name}
                    </Text>
                  </G>
                )
              })}

              <Path
                d={`M${width / 2 + star.radius},${height /
                  2} a1,1 0 0,0  ${star.radius * -2},0`}
                fill={`url(#StarTop${
                  star.color != null ? star.color.toString() : 2
                })`}
              />
            </Svg>
          </ReactSVGPanZoom>
        )}
      </div>
    )
  }
}
