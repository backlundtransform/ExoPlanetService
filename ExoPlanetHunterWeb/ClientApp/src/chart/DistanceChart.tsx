import  * as React from 'react';
import {  Segment } from 'semantic-ui-react'
 const Distance =()=> {
 
  return (<React.Fragment>
        <Segment inverted attached="bottom" >
          <h3>{'Distance diagram'}</h3>
          <div
            id="polarchartdiv"
            style={{
              width: '100%',
              maxHeight: '700px',
              height: '69vh',
              margin: '10px',
       
            }}
          />
        </Segment>
      </React.Fragment>)
 }
export default Distance