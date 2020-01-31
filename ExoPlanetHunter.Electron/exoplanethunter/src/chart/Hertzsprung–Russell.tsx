import * as React from 'react'
import { Menu, Segment } from 'semantic-ui-react'

interface HertzsprungRussellProps {
  updateParent: () => void
}

interface HertzsprungRussellState {
  isHabitable: boolean
}
class HertzsprungRussell extends React.Component<
  HertzsprungRussellProps,
  HertzsprungRussellState
> {
  constructor(props: HertzsprungRussellProps) {
    super(props)
    this.state = { isHabitable: true }
  }

  isHabitable = (isHabitable: boolean) => {
    const { updateParent } = this.props

    updateParent()
    this.setState({ isHabitable })
  }
  render() {
    const { isHabitable } = this.state
    return (
      <React.Fragment>
        <Menu attached="top" tabular  size='mini'>
          <Menu.Item
            name="active"
            active={isHabitable}
            onClick={() => this.isHabitable(true)}
          >
            Habitable
          </Menu.Item>
          <Menu.Item
            name="2"
            active={!isHabitable}
            onClick={() => this.isHabitable(false)}
          >
            All
          </Menu.Item>
        </Menu>
        <Segment inverted attached="bottom" >
          <h3>{'Hertzsprung–Russell diagram'}</h3>
          <div
            id="bubblechartdiv"
            style={{
              width: '100%',
              maxHeight: '700px',
              height: '69vh',
              margin: '10px',
       
            }}
          />
        </Segment>
      </React.Fragment>
    )
  }
}

export default HertzsprungRussell
