import * as React from 'react'
import { ConstellationSolarSystems} from '../service/getSolarSystem'
import { Card, Button, Grid,Header,Loader,Dimmer } from 'semantic-ui-react'
import Svg, { Rect } from 'react-native-svg-web'
import {getGroupedItems} from './Catalog'
import MaterialIcon from 'material-icons-react'
import { Link } from 'react-router-dom'
import { Gradient } from '../styles/radialgradients'
import { Star} from '../service/getPlanets'
import { resource } from '../config/Resource'
import Paginate from '../common/paginate'

export default class Constellations extends React.Component<any,any> {
 constructor(props: any) {
    super(props)
    this.state = {
      loading: true,
      top:30,
     constellation:1,
      stars: [] as Array<Star>
    }
  }

  async componentDidMount() {
    const { location, match } = this.props as any
    const constellation =location.state===undefined? match.params.constellationId :location.state.constellation  as number
    const stars = await ConstellationSolarSystems(constellation,0)
    this.setState({ stars, loading: false, constellation })
  }

  handlePaginate = async (top: number) => {
   
    const {constellation} = this.state
    const stars= await ConstellationSolarSystems(constellation,(top-30)/30)

    this.setState({  top,stars})
  }

  mainPost = () => {
    const { stars } = this.state
    let posts = [] as Array<any>

    for (let item of stars as Array<Star>) {
      posts.push(
        <Grid.Column key={item.name}>
          <Card className={'post-preview'}>
          <Link
                  to={{
                    pathname: `../star/${item.name}`,
                    state: { star: item }
                  }}
                >
            <Svg height="200" width="250">
              {' '}
              {Gradient()}
              <Rect
                x={50}
                y={-50 }
                width={150}
                height={150}
                fill={`url(#StarTop${
                 item.color != null ? item.color.toString() : 2
                })`}
              />
              <Rect
                x={50}
                y={100}
                width={150}
                height={150}
                fill={`url(#Star${
                  item.color != null ? item.color.toString() : 2
                })`}
              />
          
            </Svg> </Link>
            <Card.Content>
              <Card.Header>
                <Link
                  to={{
                    pathname: `../star/${item.name}`,
                    state: { star: item }
                  }}
                >
                  {item.name}
                </Link>
              </Card.Header>
          </Card.Content>
            <Card.Content extra>
             <Link
                to={{
                  pathname: `../system/${item.name}`,
                  state: { star: item }
                }}
              ><Button icon inverted basic color='grey' height="25">
              <MaterialIcon icon="3d_rotation" color="#c6d4ff" size={25} />
               {`Visit Solarsystem`}</Button>
              </Link>
            </Card.Content>
          </Card>
        </Grid.Column>
      )
    }

    return getGroupedItems(posts)
  }

  render() {
    const { loading,top,constellation, stars } = this.state

 
    const main = this.mainPost()
 
    return loading ? (
      <Dimmer active>
      <Loader />
      </Dimmer>
    ) : (
      <React.Fragment> <Header textAlign='center'> {resource.const[constellation]}</Header>
      <Grid stackable centered columns={2}>
        {main}
      </Grid> 
      <div className={'bar'}>
        <Paginate handlePaginate={this.handlePaginate} top={top} length={stars.length} />
        </div>
      </React.Fragment> 
    )
  }
}
