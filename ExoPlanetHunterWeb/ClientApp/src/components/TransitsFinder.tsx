import * as React from 'react'

import { useMap, useBaseLayer } from '../../hooks/useMap';

const TransitFinder=()=> {

  const [map, setReference] = useMap({ lat:11, lng:11, zoom:11, minZoom:2, maxZoom:7 })

 useBaseLayer(map, '/img/map.png')

  return (
    <div  ref={setReference}  style ={{
      width: '100vw',
      height: '100vh'}}/>
  )
}

export default TransitFinder