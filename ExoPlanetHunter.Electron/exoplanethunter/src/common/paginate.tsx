import * as React from 'react'

import { Button } from 'semantic-ui-react'
interface PaginateProps{
    handlePaginate:(page:number)=>{}
    top:number
    length:number
}
export default class Paginate extends React.Component<PaginateProps> {
  render() {
    const { handlePaginate, top, length } = this.props

    return (
      <Button.Group>
        <Button
          labelPosition="left"
          icon="left chevron"
          content="Previous"
          color={top <= 30 ? 'grey' : 'black'}
          disabled={top <= 30}
          onClick={() => handlePaginate(top - 30)}
        />
        <Button
          labelPosition="right"
          icon="right chevron"
          content="Next"
          color={length <= 29 ? 'grey' : 'black'}
          disabled={length <= 29}
          onClick={() => handlePaginate(top + 30)}
        />
      </Button.Group>
    )
  }
}
