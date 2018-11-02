import * as React from 'react';
import * as L from 'leaflet';
import {GetConstellationsLines} from '../service/getConstellations'


export class Map extends  React.Component {
  _map?: L.Map
 async componentDidMount() {
  this._map=L.map('map',{ minZoom: 4,
    maxZoom: 12}) as L.Map
    const position = [51.505, -0.09] as L.LatLngExpression
    this._map.setView(position, 5)
   let constlines = await GetConstellationsLines()

    L.tileLayer('/img/tile.png', {}).addTo(this._map)

var lineStyle = {
  "color": "#fff",
  "weight": 5,
  "opacity": 1
};
L.geoJSON(constlines, {
  style: lineStyle,
  onEachFeature: this.onEachFeature
} as any).addTo(this._map);


 
  }

onEachFeature = (feature:any, layer:any)=> {


  let options = {
    radius: 100,
    fillColor: "white",
    color: "white",
    weight: 0,
    opacity: 0,
    fillOpacity: 0
  }
const coord =  feature.geometry.coordinates

if(feature.properties.constellation!=null){
const marker =  L.circleMarker([
    coord[0][1],
    coord[0][0],
  ],options)
    .addTo(this._map).bindTooltip(feature.properties.constellation,
    {permanent: true, direction:"left"}
   ).openTooltip()
   marker.addEventListener("click", ()=>console.log(feature.properties.constellation))

}
 


 options = {
    radius: 6,
    fillColor: "white",
    color: "white",
    weight: 1,
    opacity: 1,
    fillOpacity: 1
  }
   coord.map((p:any)=>{


    L.circleMarker([
      p[1],
      p[0],
    ],options)
      .addTo(this._map)
    })
  

  
 
}

  render() {
   
    return (<div id="map"></div>);
  }
}