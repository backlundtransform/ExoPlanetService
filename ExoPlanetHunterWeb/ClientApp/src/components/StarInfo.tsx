import * as React from 'react'
import { Container, Header, Table, Grid  } from 'semantic-ui-react'
import { Star } from '../service/getPlanets'
import { resource } from '../config/Resource'
import { Gradient } from '../styles/radialgradients'
import MaterialIcon from 'material-icons-react'
import Svg, { Rect, G, ClipPath, Image, Defs } from 'react-native-svg-web'
import { Link } from 'react-router-dom'
export default class StarInfo extends React.Component<any> {

  render() {
    const star = this.props.location.state.star as Star

    console.log("I made a change")

    let size = window.innerWidth / 5 > 200 ? window.innerWidth / 5 : 200

    size = size > 400 ? 400 : size

    return (
 <Container className={'post-preview'}>
 
 <Grid columns={2}   stackable>
      <Grid.Row>
        <Grid.Column>
        <Container text>
          <Header className={'post-preview'}>{star.name}</Header>
          <Svg height={star.radius * 4} width={star.radius * 3} x={0}>
          {Gradient()}
          <Rect
                x={0}
                y={-star.radius}
                width={star.radius * 3}
                height={star.radius * 3}
                fill={`url(#StarTop${
                  star.color != null ? star.color.toString() : 2
                })`}
              />
              <Rect
                x={0}
                y={2*star.radius}
                width={star.radius * 3}
                height={star.radius * 3}
                fill={`url(#Star${
                  star.color != null ? star.color.toString() : 2
                })`}
              />
         
          </Svg>
        </Container>
        </Grid.Column>
        <Grid.Column>
        <Table celled>
            {' '}
            <Table.Body>
              {(
                <Table.Row>
                  <Table.Cell>{``}</Table.Cell>
                  <Table.Cell>{``}</Table.Cell>
                </Table.Row>
              )}
              {(
                <Table.Row>
                  <Table.Cell>{``}</Table.Cell>
                  <Table.Cell>{``}</Table.Cell>
                </Table.Row>
              )}
              {(
                <Table.Row>
                  <Table.Cell>{``}</Table.Cell>
                  <Table.Cell>{``}</Table.Cell>
                </Table.Row>
              )}

              {(
                <Table.Row>
                  <Table.Cell>{``}</Table.Cell>
                  <Table.Cell>{``}</Table.Cell>
                </Table.Row>
              )}
              { (
                <Table.Row>
                  <Table.Cell>{``}</Table.Cell>
                  <Table.Cell>{``}</Table.Cell>
                </Table.Row>
              )}
              { (
                <Table.Row>
                  <Table.Cell>{``}</Table.Cell>
                  <Table.Cell>
                  {``}</Table.Cell>
                </Table.Row>
              )}
            </Table.Body>
            <Table.Footer>
              <Table.Row />
            </Table.Footer>
          </Table>
              <Link
                to={{
                  pathname: `../system/${star.name}`,
                  state: { star: star }
                }}
              >
                <MaterialIcon icon="3d_rotation" color="#c6d4ff" size={40} />
                {"Visit Solar System"}
              </Link>
        </Grid.Column>
      </Grid.Row>
    </Grid><hr />
    <Container>
        
      </Container>
</Container>
         
    )
  }
}
