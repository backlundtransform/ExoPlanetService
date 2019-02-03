import * as React from 'react'

import { Card, Grid, Image } from 'semantic-ui-react'
import { getGroupedItems } from './Catalog'

import { Link } from 'react-router-dom'

const options = [
  {
    key: 'hertz',
    text: 'Hertzsprung russell diagram',
    image: 'Hertzsprung-russell',
    route: 'hertz'
  },
  {
    key: 'planettypes',
    text: 'Discovered exoplanets by type',
    image: 'planettypes',
    route: 'planettypes'
  }
]

export default class Chart extends React.Component<any, any> {
  constructor(props: any) {
    super(props)
    this.state = {}
  }

  mainPost = () => {
    let posts = [] as Array<any>

    for (let item of options) {
      posts.push(
        <React.Fragment key={item.key}>
          <Grid.Column key={item.text}>
            <Card style={{ padding: '25px',minHeight: '250px' }}>
              <Link
                to={{
                  pathname: `/chart/${item.route}`
                }}
              >
                <Image src={`../img/${item.image}.jpg`} bordered={true} />
                <Card.Content>
                  <Card.Header>{item.text}</Card.Header>
                </Card.Content>
              </Link>
            </Card>
          </Grid.Column>
        </React.Fragment>
      )
    }

    return getGroupedItems(posts)
  }

  render() {
    const main = this.mainPost()

    return (
      <div className={'chart'}>
        <Grid stackable centered columns={2}>
          {main}
        </Grid>
      </div>
    )
  }
}
