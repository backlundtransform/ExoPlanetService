import * as React from 'react'
import {useState, useEffect} from 'react'
import { Icon, Statistic } from 'semantic-ui-react'
import { useMap, useBaseLayer } from '../../hooks/useMap'
import sideClock from '../siderealtime/'
import {
  GetConstellationsLines,
  GetStarsMarkers
} from '../service/getConstellations'
import { GeoJsonObject } from 'geojson'

const TransitFinder=()=> {

  const [longitude, setLongitude] = useState<number>(-90)
  const [latitude, setLatitude] = useState<number>(40)

  const [siderealtime, setSiderealtime] = useState<string>('')

  const [stars,GeoJsonObject] = useState<Array<GeoJsonObject> | null>(null)

  const [ constlines, setConstlines] = useState<GeoJsonObject>(null)

  const [ isDownUnder, setIsDownUnder] = useState<Boolean>(true)


  const [map, setReference] = useMap({ lat:latitude, lng:longitude, zoom:11, minZoom:2, maxZoom:7 })

 useBaseLayer(map, '/img/map.png')

 useEffect(()=>{


   if (navigator.geolocation) {
     navigator.geolocation.getCurrentPosition(position =>{
      setIsDownUnder(position.coords.latitude<0)
       updatetime(position.coords.longitude)}
     )}

 },[])

 useEffect(()=>{

  map&& map.on('moveend', () => {
    const { lng, lat } = map.getCenter()
   setLongitude(lng)
   setLatitude(lat)
  })

},[map])


const updatetime = (position: number) => {
  setSiderealtime(sideClock(position) )
  setInterval(() => {
    setSiderealtime(sideClock(position) )
  }, 60000)
}

  return (
    <>
    <Statistic.Group widths="three">
      <Statistic>
        <Statistic.Value>
          <Icon name="compass outline" />
          {Math.round(100 * latitude) / 100}
        </Statistic.Value>
        <Statistic.Label>{'Declination'}</Statistic.Label>
      </Statistic>
      <Statistic>
        <Statistic.Value>
          <Icon name="clock" />
          {siderealtime}
        </Statistic.Value>
        <Statistic.Label>{'Sidereal time'}</Statistic.Label>
      </Statistic>
      <Statistic>
        <Statistic.Value>
          <Icon name="compass" />
          {Math.round(100 * (12 + (-1 * longitude) / 15)) / 100}
        </Statistic.Value>
        <Statistic.Label>{'Right ascension'}</Statistic.Label>
      </Statistic>
    </Statistic.Group>
    <div  ref={setReference}  style ={{
      width: '100vw',
      height: '95vh'}}/>
  </>
  
  )
}

export default TransitFinder