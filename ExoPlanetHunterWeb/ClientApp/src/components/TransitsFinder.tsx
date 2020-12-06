import * as React from 'react'
import {useState, useEffect} from 'react'
import { Icon, Statistic,Modal } from 'semantic-ui-react'
import { useMap, useBaseLayer } from '../../hooks/useMap'
import sideClock from '../siderealtime/'
import {
  GetConstellationsLines,
  GetStarsMarkers
} from '../service/getConstellations'
import { GeoJsonObject } from 'geojson'
import { useHistory } from 'react-router-dom'
import { LatLngLiteral } from 'leaflet'
import TimeSeriesChart from '../chart/TimeSeriesChart'


const TransitFinder=()=> {
  let history =useHistory()
  const [longitude, setLongitude] = useState<number>(-90)
  const [latitude, setLatitude] = useState<number>(40)


  const [siderealtime, setSiderealtime] = useState<string>('')

  const [stars,setStars] = useState<Array<GeoJsonObject> | null>(null)

  const [ constlines, setConstlines] = useState<GeoJsonObject>(null)

  const [ isDownUnder, setIsDownUnder] = useState<Boolean>(true)

  const [ isGraphOpen, setIsGraphOpen] = useState<boolean>(false)

  const [radius, setRadius] = useState<number>(0)

  const [coordinates, setCoordinates ] = useState<LatLngLiteral>({ lat:0, lng:0 })

  const [map, setMap] = useMap({ lat:latitude, lng:longitude, zoom:11, minZoom:2, maxZoom:7 })

  useBaseLayer(map, '/img/map.png')

 useEffect(()=>{
 
   if (navigator.geolocation) {
     navigator.geolocation.getCurrentPosition(position =>{
      setIsDownUnder(position.coords.latitude<0)
       updatetime(position.coords.longitude)}
     )}
   GetConstellationsLines().then((p)=>setConstlines(p))
   GetStarsMarkers().then((p)=>setStars(p))

 },[])

 useEffect(()=>{

  map&& map.on('moveend', () => {
    const { lng, lat } = map.getCenter()
   setLongitude(lng)
   setLatitude(lat)
  })

},[map])

useEffect(()=>{

  stars&&constlines&&init()

},[stars,constlines])

useEffect(()=>{

  if(map===null){
    return
  }

  const L = require('Leaflet')

require('Leaflet.fullscreen')
 require('leaflet-draw')
map.addControl(L.control.fullscreen())
 var drawnItems = new L.FeatureGroup();
 map&& map.addLayer(drawnItems);
  var drawControl = new L.Control.Draw({
      edit: {
          featureGroup: drawnItems
      },
      draw: {
        rectangle:false,
        polygon:false,
        polyline:false,
        marker:false,
        circlemarker:false
    }
     
  });
  map&&map.addControl(drawControl)
 new L.Draw.Circle(map, drawControl.options.circle).enable()
 map.on(L.Draw.Event.CREATED, (event:any)=> {
  const layer = event.layer

  layer.on('dblclick', (e:any)=> { 
  
    setCoordinates(e.target._latlng)

    console.log(e.target.options.radius)

    setRadius(e.target.options.radius)
    setIsGraphOpen(true)
  
  
  })

  drawnItems.addLayer(layer)
})


},[constlines])




const init = () => {

  const lineStyle = {
    color: '#fff',
    weight: 5,
    opacity: 1
  }
  const L = require('Leaflet')

 
  L.geoJSON(constlines, {
    style: lineStyle,
    onEachFeature: onEachFeature
  } as any).addTo(map)


  const starIcon = L.icon({
    iconUrl: '/img/smarker.png',
    iconSize: [60, 60]
  })

  L.geoJSON(stars, {
    pointToLayer: (feature: any, latlng: any) => {
      return L.marker(latlng, { icon: starIcon })
        .bindTooltip(feature.properties.name, { direction: 'left' })
        .openTooltip()
        .addTo(map)
    }
  }).addTo(map)
  
    
}

const onEachFeature = (feature: any, layer: any) => {
  let options = {
    radius: 100,
    fillColor: 'white',
    color: 'white',
    weight: 0,
    opacity: 0,
    fillOpacity: 0
  }
  const coord = feature.geometry.coordinates
  const L = require('Leaflet')
  if (feature.properties.constellation != null) {
   const marker= L.circleMarker([coord[0][1], coord[0][0]], options)
      .addTo(map)
      .bindTooltip(feature.properties.constellation, {
        permanent: true,
        direction: 'left'
      })
      .openTooltip()
      marker.addEventListener('click', () =>
     history.push({
        pathname: `constellation/${feature.properties.constellationid}`,
        state: { constellation: feature.properties.constellationid }
      }))
  }
  options = {
    radius: 6,
    fillColor: 'white',
    color: 'white',
    weight: 1,
    opacity: 1,
    fillOpacity: 1
  }
  coord.map((p: any) => {
    L.circleMarker([p[1], p[0]], options).addTo(map)
  })
}


const updatetime = (position: number) => {
  setSiderealtime(sideClock(position))
  setInterval(() => {
    setSiderealtime(sideClock(position) )
  }, 60000)
}

  return (
    <> <Modal
    onClose={() => setIsGraphOpen(false)}
    open={isGraphOpen}
  
  >
  
    <Modal.Content>
      <Modal.Description>
  {radius>0&&<TimeSeriesChart  radius={radius/100000} dec={Math.round(100 * latitude) / 100} ra={Math.round(100 * (12 + (-1 * longitude) / 15)) / 100}/>}    
      </Modal.Description>
    </Modal.Content>

  </Modal>
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
    <div  ref={setMap}  style ={{
      width: '100vw',
      height: '95vh'}}/>
  </>
  
  )
}

export default TransitFinder