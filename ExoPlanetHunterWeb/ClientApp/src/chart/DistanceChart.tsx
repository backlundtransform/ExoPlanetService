import  * as React from 'react';
import {  Segment, Button  } from 'semantic-ui-react'
interface DistanceProps{
ZoomCallback:()=>void

}
 const Distance =(props:DistanceProps)=> {
 
  return (<React.Fragment>
        <Segment inverted attached="bottom" >
          <h3>{'Distance diagram'} <Button circular icon='search' onClick={()=>props.ZoomCallback()} /></h3>
         
          <div
            id="polarchartdiv"
            style={{
              width: '100%',
              maxHeight: '1000px',
              height: '199vh',
              margin: '10px',
       
            }}
          /> 
        </Segment>
      </React.Fragment>)
 }
export default Distance