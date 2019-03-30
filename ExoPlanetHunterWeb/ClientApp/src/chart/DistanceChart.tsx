import  * as React from 'react';
import { Menu, Segment } from 'semantic-ui-react'
 const Distance =()=> {
  const [count, setCount] =  React.useState(0);


  React.useEffect(() => {

  });

  return (<React.Fragment>
        <Segment inverted attached="bottom" >
          <h3>{'Distance diagram'}</h3>
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
      </React.Fragment>)
 }
export default Distance