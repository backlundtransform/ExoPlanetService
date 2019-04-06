import  * as React from 'react';
import {  Segment, Button  } from 'semantic-ui-react'
interface DistanceProps{
ZoomCallback:(factor:number)=>void
max:number
distance:number
}
 const Distance =(props:DistanceProps)=> {
 
  return (<React.Fragment>
        <Segment inverted attached="bottom" >
          <h3>{'Distance diagram (light-years from Earth)'} <Button circular icon='zoom-in' disabled={props.distance<=8} onClick={()=>props.ZoomCallback(0)} /><Button circular icon='zoom-out' disabled={props.distance>= props.max} onClick={()=>props.ZoomCallback(1)} /></h3>
         
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