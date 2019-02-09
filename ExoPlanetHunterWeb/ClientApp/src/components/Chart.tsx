import * as React from 'react'

import { Grid,  } from 'semantic-ui-react'

import HertzsprungRussell from '../chart/Hertzsprung–Russell'
import StockChart from '../chart/StockChart'

export default class Chart extends React.Component<any, any> {
  constructor(props: any) {
    super(props)
    this.state = {}
  }

  mainPost = () => {
    let posts = [] as Array<any>

    const options = [
      {
        key: 'hertz',
       
        component: <HertzsprungRussell props={this.props} />,
      
      },
      {
        key: 'planettypes',
      
        component: <StockChart props={this.props} />,
       
      }
    ]
    for (let item of options) {
      posts.push(
        <Grid.Column key={item.key}>
          
            {item.component}
          
        </Grid.Column>
      )
    }

    return posts
  }

  render() {
    const main = this.mainPost()

    return (

 
      <Grid container stackable  columns={'equal'}>
     
     {"main"}
    
      </Grid>
    )
  }
}
