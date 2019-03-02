import * as React from 'react'
import { Container, Header, Table, Grid, Button,  Loader,
  Dimmer} from 'semantic-ui-react'
import { Star, GetPlanetAsync } from '../service/getPlanets'
import { resource } from '../config/Resource'
import { Gradient } from '../styles/radialgradients'
import MaterialIcon from 'material-icons-react'
import Svg, { Rect } from 'react-native-svg-web'
import { Link } from 'react-router-dom'
import { getSolarSystem } from '../service/getSolarSystem'


export default class StarInfo extends React.Component<any> {
  state = {
    star: {} as Star,
    loading:false
  }
  async componentDidMount() {

    const { location, match } = this.props as any
    
   
    let  star={} as Star

    if(location.state!==undefined){

     star = location.state.star
    }
    if(location.state===undefined){

      star = {planets:null, name:match.params.starId, img:null, type:null}
     }

    if (star.planets === null ) {
      const starobj = await getSolarSystem(star)

      const planet = await GetPlanetAsync(starobj.planets[0].name)

      star = planet.star
    }

    this.setState({ star, loader:true })
  }
  getPlanetText = (star: Star): string => {
    const knownhabplanets = star.noHabPlanets
    const knownplanets = star.noPlanets
    if (knownhabplanets >= 1 && knownplanets > 1) {
      if (knownhabplanets === 1) {
        return `${resource.numberplanet[1]} ${knownplanets} ${
          resource.numberplanet[2]
        } ${resource.numberplanet[0]}`
      }
      return `${resource.numberplanet[1]} ${knownplanets} ${
        resource.numberplanet[2]
      } ${knownhabplanets} ${resource.numberplanet[3]}`
    }

    if (knownhabplanets == 0 && knownplanets > 1) {
      return `${resource.numberplanet[1]} ${knownplanets} ${
        resource.numberplanet[4]
      }`
    }

    if (knownhabplanets == 0 && knownplanets == 1) {
      return `${resource.numberplanet[5]}`
    }
  }

  render() {
    const { star,loading } = this.state
    const planetext = this.getPlanetText(star)
    let size = window.innerWidth / 5 > 200 ? window.innerWidth / 5 : 200

    size = size > 400 ? 400 : size

    return (<div> {loading ? (
      <Dimmer active>
        <Loader />
      </Dimmer>
    ) : ( <Container className={'post-preview'}>
    <Grid columns={2} stackable>
      <Grid.Row>
        <Grid.Column>
          <Container text>
            {star.radius && (
              <Svg
                height={Math.round(star.radius) * 4}
                width={Math.round(star.radius) * 3}
                x={0}
              >
                {Gradient()}
                <Rect
                  x={0}
                  y={-Math.round(star.radius)}
                  width={Math.round(star.radius) * 3}
                  height={Math.round(star.radius) * 3}
                  fill={`url(#StarTop${
                    star.color != null ? star.color.toString() : 2
                  })`}
                />
                <Rect
                  x={0}
                  y={2 * Math.round(star.radius)}
                  width={Math.round(star.radius) * 3}
                  height={Math.round(star.radius) * 3}
                  fill={`url(#Star${
                    star.color != null ? star.color.toString() : 2
                  })`}
                />
              </Svg>
            )}
          </Container>
        </Grid.Column>
        <Grid.Column>
          <Header className={'post-preview'}>{star.name}</Header>
          <Table celled>
            <Table.Body>
              {star.mass ? (
                <Table.Row>
                  <Table.Cell>{`${resource.starinfo[0]}`}</Table.Cell>
                  <Table.Cell>{`${star.mass}*${
                    resource.oursun
                  }`}</Table.Cell>
                </Table.Row>
              ):''}
              {star.radiusSu!==0&&star.radiusSu ? (
                <Table.Row>
                  <Table.Cell>{`${resource.starinfo[1]}`}</Table.Cell>
                  <Table.Cell>{`${star.radiusSu}*${
                    resource.oursun
                  }`}</Table.Cell>
                </Table.Row>
              ):''}
              {star.age ? (
                <Table.Row>
                  <Table.Cell>{`${resource.starinfo[2]}`}</Table.Cell>
                  <Table.Cell>{`${star.age} ${
                   ' Gyrs'
                  }`}</Table.Cell>
                </Table.Row>
              ):''}

              {star.temp ? (
                <Table.Row>
                  <Table.Cell>{`${resource.starinfo[3]}`}</Table.Cell>
                  <Table.Cell>{`${star.temp} C`}</Table.Cell>
                </Table.Row>
              ):''}
            </Table.Body>
            <Table.Footer>
              <Table.Row />
            </Table.Footer>
          </Table>

          <Link
            to={`../system/${star.name}`}
          >
          
            <Button icon inverted basic color="grey" height="40">
              <MaterialIcon icon="3d_rotation" color="#c6d4ff" size={40} />
              {'Visit Solar System'}
            </Button>
          </Link>
          <Link
            to={{
              pathname: `../constellation/${star.constellation}`,
              state: { constellation: star.constellation }
            }}
          >
            {' '}
            <Button icon inverted basic color="grey" height="40">
              <MaterialIcon icon="scatter_plot" color="#c6d4ff" size={40} />
              {'Visit Constellation'}
            </Button>
          </Link>
        </Grid.Column>
      </Grid.Row>
    </Grid>
    <hr />
    <Container>
      <p>
        {`${resource.starname[0]} ${star.name} ${
         star.constellation&&resource.const[star.constellation]=== undefined ?'': `${resource.starname[1]}  ${resource.const[star.constellation]}.`}
        ${
          star.luminosity === 9
            ? resource.startype[0]
            : resource.startype[1] +
              ' ' +
              resource.color[star.color] +
              ' ' +
              resource.typecolor
        }${
          star.luminosity < 9
            ? ' ' +
              resource.startype[2] +
              ' ' +
              resource.lum[star.luminosity] +
              '.'
            : '.'
        } ${
          resource.mag[star.magnitude] != null
            ? resource.mag[star.magnitude]
            : ''
        }`}
      </p>

      {star.habZoneMin != null && star.habZoneMax != null
        ? `${resource.habzone[0]} ${star.habZoneMin} AU ${
            resource.habzone[1]
          } ${star.habZoneMax} AU. `
        : ''}
      {planetext == null ? <React.Fragment /> : planetext}
    </Container>
  </Container>)}</div>

     
    )
  }
}
