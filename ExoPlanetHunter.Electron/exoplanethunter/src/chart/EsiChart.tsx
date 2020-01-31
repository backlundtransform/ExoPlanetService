import * as React from 'react'
import {  Segment } from 'semantic-ui-react'

 const EsiChart =()=> {

        return(<React.Fragment>
            <Segment inverted attached="bottom">
              <h3>{'ESI distance diagram'}</h3>
              <div
                id="esidiv"
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
export default EsiChart