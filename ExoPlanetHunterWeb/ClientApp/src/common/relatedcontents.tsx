import * as React from 'react'

import { Card,  Image } from 'semantic-ui-react'

import {GetRelatedAsync , RelatedContent} from '../service/getRelated'
interface RelatedContentProps{
    name:string
}
interface RelatedContentState{
    tags:Array<RelatedContent>
}
export default class RelatedContents extends React.Component<RelatedContentProps, RelatedContentState> {
    state = { tags:[] as Array<RelatedContent>}
   async componentDidMount() {
     const {name} = this.props

     const tags = await GetRelatedAsync(name)
     this.setState({tags})
    }
  render() {
  const {tags} =this.state

    return (
      <Card.Group itemsPerRow={3}>
          {tags.map(p=>(<Card  href={p.url} >
      <Image src={p.image} />
      <Card.Content>
        <Card.Meta>Created {p.created}</Card.Meta>
        <Card.Description>{p.description}</Card.Description>
      </Card.Content>
     </Card>))}
      </Card.Group>
    )
  }
}
