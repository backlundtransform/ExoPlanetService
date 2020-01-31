import * as React from 'react'
import {  Segment } from 'semantic-ui-react'

 const MassOrbitChart =()=> {

        return(<React.Fragment>
            <Segment inverted attached="bottom">
              <h3>{'Period mass diagram'}</h3>
              <div
                id="massorbitdiv"
                style={{
                  width: '100%',
                  maxHeight: '700px',
                  height: '69vh',
                  margin: '10px'
                }}
              />
            </Segment>
        </React.Fragment>) 
  }
export default MassOrbitChart
