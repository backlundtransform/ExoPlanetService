import * as React from 'react';
import{GetPlanetListAsync, storeBase64,Planet } from '../service/getPlanets'
import { Card, Icon,Grid,Rating  } from 'semantic-ui-react'
import Svg,{Circle,G,ClipPath,Image,Defs} from 'react-native-svg-web';
import MaterialIcon from 'material-icons-react';
import { Link } from 'react-router-dom'
import {Gradient} from '../styles/radialgradients'
export class Catalog extends React.Component<any,any> {
  constructor(props:any) {
    super(props);
    this.state = {loading:true, top: 100,color:'', planets:[] as Array<Planet>}

  }

async componentDidMount(){

  const planets =  await GetPlanetListAsync(null,{},100)
  let color= JSON.parse(await storeBase64())
  this.setState({color,planets,loading:false})
  }

  groupBy=(arr:Array<any>, n:number)=> {
    var group = [];
    for (var i = 0, end = arr.length / n; i < end; ++i)
      group.push(arr.slice(i * n, (i + 1) * n));
    return group;
  }
  mainPost =()=>{
    const {planets ,color} = this.state
    let posts = [] as Array<any>

     for(let item of planets as Array<Planet>){
        posts.push(<Grid.Column>
        <Card className={"post-preview"}>
        <Svg
               height="190" 
               width="180" 
         >       {Gradient(item.star)}
      <G>
      <Defs>
          <ClipPath id="clip">
              <Circle  cx="100" cy="100" r="65"   />
          </ClipPath>
      </Defs> 
      <Image
       width="180" height="190" 
       href={`${color[item.img.uri]}` }
          clipPath="url(#clip)"
      /><Circle 
      cx="100" cy="100" r="65"
          fillOpacity={0.4}
          fill={`url(#${item.img.uri}`}/></G>
          </Svg>
        <Card.Content>
          <Card.Header><Link to={{ pathname: `planet/${item.name}`, state: { planet: item}}}>{item.name}</Link></Card.Header>
          <Card.Description>
            <span>{`Discovered:${item.discYear}`}</span>
          </Card.Description>
          <Card.Description>{`${item.type&&item.type !== null?item.type:""}`} {item.distance !== 0?`${Math.round(item.distance)} lightyears from earth.`:""}

          </Card.Description>
          <Rating icon='star' defaultRating={Math.round(item.esi*10)} maxRating={10} size='large' />
    
        </Card.Content>
        <Card.Content extra>
          <a style={{margin:10}}>
            <Icon name='sun' />
       {item.star.name}
          </a><br />
          <a style={{margin:10}}>
          {`${item.star.noPlanets} Planets`}
   
          </a> <a>
          <MaterialIcon icon="3d_rotation" color='#c6d4ff'  size={25}/>
          </a>
        </Card.Content>
      </Card> 
       </Grid.Column>)
     }

    let groupedplanets = this.groupBy(posts, 3)
    let groupedposts = [] as Array<any>
    for(let planets of groupedplanets){
      groupedposts.push(this.row(planets))

    }
    return groupedposts
  
  }
 row =(post:any) => <Grid.Row centered columns={4}>{post}</Grid.Row>
  render() {
     const {loading} = this.state
      const main =this.mainPost()

return (loading?(<React.Fragment />):(<Grid stackable centered columns={2}>{main}</Grid>))

}


  

 
}