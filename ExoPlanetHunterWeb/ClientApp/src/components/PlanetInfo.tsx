import * as React from 'react'
import { Container, Header, Rating, Table, Grid,Button  } from 'semantic-ui-react'
import { Planet } from '../service/getPlanets'
import { resource } from '../config/Resource'
import { Gradient } from '../styles/radialgradients'
import MaterialIcon from 'material-icons-react'
import Svg, { Circle, G, ClipPath, Image, Defs } from 'react-native-svg-web'
import { Link } from 'react-router-dom'
import Ad from '../common/ad'
export default class PlanetInfo extends React.Component<any> {
  async componentDidMount() {}

  render() {
    const planet = this.props.location.state.planet as Planet

    let size = window.innerWidth / 5 > 200 ? window.innerWidth / 5 : 200

    size = size > 400 ? 400 : size

    return (
 <Container className={'post-preview'}>
 
 <Grid columns={2}   stackable>
      <Grid.Row>
        <Grid.Column>
        <Container text>
      <Svg height={size} width={size} x={size / 2}>
            {' '}
            {Gradient()}
            <G>
              <Defs>
                <ClipPath id="clip">
                  <Circle cx={size / 2} cy={size / 2} r={size / 2} />
                </ClipPath>
              </Defs>

              <Image
                width={800}
                height={800}
                x={0}
                y={0}
                href={`../img/${planet.img.uri}.jpg`}
                clipPath="url(#clip)"
              >
                {' '}
                
              </Image>
              <Circle
                cx={size / 2}
                cy={size / 2}
                r={size / 2}
                fillOpacity={0.3}
                fill={`url(#${planet.img.uri})`}
              />
            </G>
          </Svg><p>{resource.esiratings}</p>
          <Rating
            icon="star"
            defaultRating={Math.round(planet.esi * 10)}
            maxRating={10}
            size="massive"
            disabled={true}
          />
          <p>{resource.sphratings}</p>
          <Rating
            icon="star"
            defaultRating={Math.round(planet.sph * 10)}
            maxRating={10}
            size="massive"
            disabled={true}
          />
        </Container>
        </Grid.Column>
        <Grid.Column>
        <Header className={'post-preview'}>{planet.name}</Header>
        <Table celled>
            <Table.Body>
              {planet.mass && (
                <Table.Row>
                  <Table.Cell>{resource.planetinfo[0]}</Table.Cell>
                  <Table.Cell>{`${planet.mass}*${resource.earth}`}</Table.Cell>
                </Table.Row>
              )}
              {planet.radiusEu && (
                <Table.Row>
                  <Table.Cell>{resource.planetinfo[1]}</Table.Cell>
                  <Table.Cell>{`${planet.radiusEu}*${
                    resource.earth
                  }`}</Table.Cell>
                </Table.Row>
              )}
              {planet.density && (
                <Table.Row>
                  <Table.Cell>{resource.planetinfo[2]}</Table.Cell>
                  <Table.Cell>{`${planet.density}*${
                    resource.earth
                  }`}</Table.Cell>
                </Table.Row>
              )}

              {planet.gravity && (
                <Table.Row>
                  <Table.Cell>{resource.planetinfo[3]}</Table.Cell>
                  <Table.Cell>{`${planet.gravity}*${
                    resource.earth
                  }`}</Table.Cell>
                </Table.Row>
              )}
              {planet.surfacePressure && (
                <Table.Row>
                  <Table.Cell>{resource.planetinfo[4]}</Table.Cell>
                  <Table.Cell>{`${planet.surfacePressure}*${
                    resource.earth
                  }`}</Table.Cell>
                </Table.Row>
              )}
              {planet.escapeVelocity && (
                <Table.Row>
                  <Table.Cell>{resource.planetinfo[5]}</Table.Cell>
                  <Table.Cell>{`${planet.escapeVelocity}*${
                    resource.earth
                  }`}</Table.Cell>
                </Table.Row>
              )}
            </Table.Body>
           
          </Table>
          <Link
                to={{
                  pathname: `../star/${planet.star.name}`,
                  state: { star: planet.star }
                }}
              >   
              <Button icon inverted basic color='grey' height="40">
              <MaterialIcon icon="wb_sunny" color="#c6d4ff" size={40} />
                {"Visit Star"} </Button>
                </Link>
              <Link
                to={{
                  pathname: `../system/${planet.star.name}`,
                  state: { star: planet.star }
                }}
              > <Button icon inverted basic color='grey' height="40">
                <MaterialIcon icon="3d_rotation" color="#c6d4ff" size={40} />
                {"Visit Solar System"}</Button>
              </Link>
        </Grid.Column>
      </Grid.Row>
    </Grid><hr />
    <Container>
           {`${resource.planetname[0]} ${planet.name} ${
             resource.planetname[1]
           }  ${planet.star.name}  ${resource.planetname[2]} ${
             resource.const[planet.star.constellation] === undefined
               ? ''
               : resource.const[planet.star.constellation]
           }`}{' '}
           {`${resource.decFormatdist[0]}${
             planet.distance !== 0 ? Math.round(planet.distance) : ''
           } ${resource.decFormatdist[1]} `}
           <p>
             {resource.massInfo[planet.massType] === undefined
               ? ''
               : resource.massInfo[planet.massType]}{' '}
          
             {resource.compInfo[planet.comp] === undefined
               ? ''
               : resource.compInfo[planet.comp]}{' '}
          
             {planet.temp != null
               ? `${resource.meantemp[0]} ${planet.temp}. `
               : ''}
             {planet.tempMax != null && planet.tempMin != null
               ? `${resource.meantemp[1]} ${planet.name} ${
                   resource.meantemp[2]
                 } ${planet.tempMax} ${resource.meantemp[3]} ${planet.tempMin}`
               : ''}
           </p>
           <p>
             {planet.period != null
               ? `${resource.orbit[0]} ${planet.period} ${resource.orbit[1]} `
               : ''}
             {planet.meanDistance != null
               ? `${resource.decMean[0]} ${planet.meanDistance} ${
                   resource.decMean[1]
                 }`
               : ''}
           </p>
           <p>{`${
             resource.hzd[planet.hzd] === undefined
               ? ''
               : resource.hzd[planet.hzd] + ' '
           }${
             resource.hza[planet.hza] === undefined
               ? ''
               : resource.hza[planet.hza] + ' '
           }${
             resource.atmosinfo[planet.atmosphere] === undefined
               ? ''
               : resource.atmosinfo[planet.atmosphere] + ' '
           }${planet.moons ? resource.moon : ''}`}</p>
           <p>{`${planet.discYear === undefined ? '' : resource.disc} ${
             planet.discYear === undefined ? '' : planet.discYear
           }.`}</p>
           <p>{`${
             resource.discinfo[planet.discMethod] == undefined
               ? ''
               : resource.discinfo[planet.discMethod]
           }`}</p>
      </Container>
      <Ad />
</Container>
         
    )
  }
}
