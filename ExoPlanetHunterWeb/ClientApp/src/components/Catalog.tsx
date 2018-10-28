import * as React from 'react';
import{GetPlanetListAsync } from '../service/getPlanets'
import { Card, Icon,Grid,Rating  } from 'semantic-ui-react'
import Svg,{Circle,G,ClipPath,Path,Rect,Image, Use,Defs} from 'react-native-svg-web';
import MaterialIcon from 'material-icons-react';
export class Catalog extends React.Component {


  async componentDidMount(){

  var planet =  await GetPlanetListAsync(null,{},100)

console.log(planet)
  }


  render() {


    return ( <Grid stackable centered columns={2}>
    <Grid.Column width={9}>
       
        </Grid.Column>
    
    <Grid.Row centered columns={4}>
      <Grid.Column>
      <Card className={"post-preview"}>
      <Svg
             height="190" 
             width="180" 
       >   
    <G>
    <Defs>
        <ClipPath id="clip">
            <Circle  cx="100" cy="100" r="65"   />
        </ClipPath>
    </Defs> 
    <Image
     width="180" height="190" 
        href={ 'https://i.imgur.com/wBKCEbc.png'  }
        clipPath="url(#clip)"
    /><Circle 
    cx="100" cy="100" r="65"
        fillOpacity={0.4}
        fill={`url(#https://i.imgur.com/wBKCEbc.png`}/></G>
        </Svg>
      <Card.Content>
        <Card.Header> <a style={{margin:10}}>Epic</a></Card.Header>
        <Card.Description>
          <span>Discovered:2018</span>
        </Card.Description>
        <Card.Description>Jovian 227 lightyears from earth.</Card.Description>
        <Rating icon='star' defaultRating={3} maxRating={4} size='large' />

      </Card.Content>
      <Card.Content extra>
        <a style={{margin:10}}>
          <Icon name='sun' />
         Epic
        </a>
        <a style={{margin:10}}>
    
        2 Planets
        </a> <a>
        <MaterialIcon icon="3d_rotation" color='#c6d4ff'  size={25}/>
        </a>
      </Card.Content>
    </Card> 
      </Grid.Column>
      <Grid.Column>
      <Card className={"post-preview"}>
      <Svg
             height="190" 
             width="180" 
       >   
    <G>
    <Defs>
        <ClipPath id="clip">
            <Circle  cx="100" cy="100" r="65"   />
        </ClipPath>
    </Defs> 
    <Image
     width="180" height="190" 
        href={ 'https://i.imgur.com/wBKCEbc.png'  }
        clipPath="url(#clip)"
    /><Circle 
    cx="100" cy="100" r="65"
        fillOpacity={0.4}
        fill={`url(#https://i.imgur.com/wBKCEbc.png`}/></G>
        </Svg>
      <Card.Content>
        <Card.Header> <a style={{margin:10}}>Epic</a></Card.Header>
        <Card.Description>
          <span>Discovered:2018</span>
        </Card.Description>
        <Card.Description>Jovian 227 lightyears from earth.</Card.Description>
        <Rating icon='star' defaultRating={3} maxRating={4} size='large' />

      </Card.Content>
      <Card.Content extra>
        <a style={{margin:10}}>
          <Icon name='sun' />
         Epic
        </a>
        <a style={{margin:10}}>
    
        2 Planets
        </a> <a>
        <MaterialIcon icon="3d_rotation" color='#c6d4ff'  size={25}/>
        </a>
      </Card.Content>    </Card> 
      </Grid.Column>
      <Grid.Column>
      <Card className={"post-preview"}>
      <Svg
             height="190" 
             width="180" 
       >   
    <G>
    <Defs>
        <ClipPath id="clip">
            <Circle  cx="100" cy="100" r="65"   />
        </ClipPath>
    </Defs> 
    <Image
     width="180" height="190" 
        href={ 'https://i.imgur.com/wBKCEbc.png'  }
        clipPath="url(#clip)"
    /><Circle 
    cx="100" cy="100" r="65"
        fillOpacity={0.4}
        fill={`url(#https://i.imgur.com/wBKCEbc.png`}/></G>
        </Svg>
      <Card.Content>
        <Card.Header> <a style={{margin:10}}>Epic</a></Card.Header>
        <Card.Description>
          <span>Discovered:2018</span>
        </Card.Description>
        <Card.Description>Jovian 227 lightyears from earth.</Card.Description>
        <Rating icon='star' defaultRating={3} maxRating={4} size='large' />

      </Card.Content>
      <Card.Content extra>
        <a style={{margin:10}}>
          <Icon name='sun' />
         Epic
        </a>
        <a style={{margin:10}}>
    
        2 Planets
        </a> <a>
        <MaterialIcon icon="3d_rotation" color='#c6d4ff'  size={25}/>
        </a>
      </Card.Content></Card> 
      </Grid.Column>
    </Grid.Row>
    <Grid.Row centered columns={4}>
      <Grid.Column>
      <Card className={"post-preview"}>
      <Svg
             height="190" 
             width="180" 
       >   
    <G>
    <Defs>
        <ClipPath id="clip">
            <Circle  cx="100" cy="100" r="65"   />
        </ClipPath>
    </Defs> 
    <Image
     width="180" height="190" 
        href={ 'https://i.imgur.com/wBKCEbc.png'  }
        clipPath="url(#clip)"
    /><Circle 
    cx="100" cy="100" r="65"
        fillOpacity={0.4}
        fill={`url(#https://i.imgur.com/wBKCEbc.png`}/></G>
        </Svg>
      <Card.Content>
        <Card.Header> <a style={{margin:10}}>Epic</a></Card.Header>
        <Card.Description>
          <span>Discovered:2018</span>
        </Card.Description>
        <Card.Description>Jovian 227 lightyears from earth.</Card.Description>
        <Rating icon='star' defaultRating={3} maxRating={4} size='large' />

      </Card.Content>
      <Card.Content extra>
        <a style={{margin:10}}>
          <Icon name='sun' />
         Epic
        </a>
        <a style={{margin:10}}>
    
        2 Planets
        </a> <a>
        <MaterialIcon icon="3d_rotation" color='#c6d4ff'  size={25}/>
        </a>
      </Card.Content></Card> 
      </Grid.Column>
      <Grid.Column>
      <Card className={"post-preview"}>
      <Svg
             height="190" 
             width="180" 
       >   
    <G>
    <Defs>
        <ClipPath id="clip">
            <Circle  cx="100" cy="100" r="65"   />
        </ClipPath>
    </Defs> 
    <Image
     width="180" height="190" 
        href={ 'https://i.imgur.com/wBKCEbc.png'  }
        clipPath="url(#clip)"
    /><Circle 
    cx="100" cy="100" r="65"
        fillOpacity={0.4}
        fill={`url(#https://i.imgur.com/wBKCEbc.png`}/></G>
        </Svg>
      <Card.Content>
        <Card.Header> <a style={{margin:10}}>Epic</a></Card.Header>
        <Card.Description>
          <span>Discovered:2018</span>
        </Card.Description>
        <Card.Description>Jovian 227 lightyears from earth.</Card.Description>
        <Rating icon='star' defaultRating={3} maxRating={4} size='large' />

      </Card.Content>
      <Card.Content extra>
        <a style={{margin:10}}>
          <Icon name='sun' />
         Epic
        </a>
        <a style={{margin:10}}>
    
        2 Planets
        </a> <a>
        <MaterialIcon icon="3d_rotation" color='#c6d4ff'  size={25}/>
        </a>
      </Card.Content></Card> 
      </Grid.Column>
      <Grid.Column>
      <Card className={"post-preview"}>
      <Svg
             height="190" 
             width="180" 
       >   
    <G>
    <Defs>
        <ClipPath id="clip">
            <Circle  cx="100" cy="100" r="65"   />
        </ClipPath>
    </Defs> 
    <Image
     width="180" height="190" 
        href={ 'https://i.imgur.com/wBKCEbc.png'  }
        clipPath="url(#clip)"
    /><Circle 
    cx="100" cy="100" r="65"
        fillOpacity={0.4}
        fill={`url(#https://i.imgur.com/wBKCEbc.png`}/></G>
        </Svg>
      <Card.Content>
        <Card.Header> <a style={{margin:10}}>Epic</a></Card.Header>
        <Card.Description>
          <span>Discovered:2018</span>
        </Card.Description>
        <Card.Description>Jovian 227 lightyears from earth.</Card.Description>
        <Rating icon='star' defaultRating={3} maxRating={4} size='large' />

      </Card.Content>
      <Card.Content extra>
        <a style={{margin:10}}>
          <Icon name='sun' />
         Epic
        </a>
        <a style={{margin:10}}>
    
        2 Planets
        </a> <a>
        <MaterialIcon icon="3d_rotation" color='#c6d4ff'  size={25}/>
        </a>
      </Card.Content>    </Card> 
      </Grid.Column>
    </Grid.Row>

</Grid>
    );
  }
}