import * as React from 'react'

import {
  GetConstellationsLines,
  GetStarsMarkers
} from '../service/getConstellations'
import { GeoJsonObject } from 'geojson'
import { Icon, Statistic, IconGroup } from 'semantic-ui-react'
import siderealtime from '../siderealtime/'
import celestialObject from '../celestial-functions/celestial-functions'

import { GetHabitablePlanets, Planet } from '../service/getPlanets'
interface StarMapState {
  constlines: GeoJsonObject
  stars: GeoJsonObject
  longitude: number
  latitude: number
  siderealtime: string
  planets: Array<Planet>
}
const maxzoom=12
export default class Map extends React.Component<any, StarMapState> {
  state = {
    constlines: {} as GeoJsonObject,
    stars: {} as GeoJsonObject,
    longitude: -90,
    latitude: 40,
    siderealtime: '',
    planets: [] as Array<Planet>
  }
  _map?: L.Map
  _isMounted = false
  _markers = []
  _interval: any

async componentDidMount() {
    this._isMounted = true

    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(position =>
        this.updatetime(position.coords.longitude)
      )
    }
    const { longitude, latitude } = this.state
    const L = require('Leaflet')
    this._map = L.map('map', {
      zoom: 5,
      minZoom: 4,
      maxZoom: maxzoom,
      worldCopyJump: true,
      center: [latitude, longitude] as L.LatLngExpression
    }) as L.Map
    let constlines = await GetConstellationsLines()
    let stars = await GetStarsMarkers()
    let planets = (await GetHabitablePlanets()) as Array<Planet>
    this._map.on('moveend', () => {
      const { lng, lat } = this._map.getCenter()
      this._isMounted && this.setState({ longitude: lng, latitude: lat })
    })

    L.tileLayer('/img/tile.png', {}).addTo(this._map)
    this._isMounted &&
      this.setState({ constlines, planets, stars }, () => this.init())
  }
  componentWillUnmount() {
    this._isMounted = false
    clearInterval(this._interval)
  }

  init = () => {
    const { constlines, planets, stars } = this.state

    const lineStyle = {
      color: '#fff',
      weight: 5,
      opacity: 1
    }
    const L = require('Leaflet')
    L.geoJSON(constlines, {
      style: lineStyle,
      onEachFeature: this.onEachFeature
    } as any).addTo(this._map)
    const planetIcon = L.icon({
      iconUrl: '/img/ic_launcher_web.png',
      iconSize: [60, 60]
    })
    planets.map(planet => {
      const planetmarker = L.marker(
        [
          planet.coordinate.latitude,
          planet.coordinate.longitude
        ] as L.LatLngExpression,
        { icon: planetIcon }
      )
        .bindTooltip(planet.name, { direction: 'left' })
        .openTooltip()
        .addTo(this._map)
      planetmarker.addEventListener('click', () =>
        this.props.history.push({
          pathname: `system/${planet.star.name}`,
          state: { star: planet.star },
          props: { timestamp: () => new Date().toString() }
        })
      )
    })
    this.updatemarker()
    const starIcon = L.icon({
      iconUrl: '/img/smarker.png',
      iconSize: [60, 60]
    })

    L.geoJSON(stars, {
      pointToLayer: (feature: any, latlng: any) => {
        return L.marker(latlng, { icon: starIcon })
          .bindTooltip(feature.properties.name, { direction: 'left' })
          .openTooltip()
          .addTo(this._map)
      }
    }).addTo(this._map)
    this._map.on('zoomend', (e: any) => {
      const zoom =e.target!==undefined? e.target._zoom:e

      if (zoom === maxzoom) {
        const center = e.target._lastCenter
        const nearestplanet = planets
          .map(p => {
            return {
              name: p.name,
              star: p.star,
              distance: Math.sqrt(
                Math.pow(p.coordinate.longitude - center.lng, 2) +
                  Math.pow(p.coordinate.latitude - center.lat, 2)
              )
            }
          })
          .sort((a, b) => {
            return a.distance - b.distance
          })[0]
    
        if (nearestplanet.distance < 0.5) {
          this.props.history.push({
            pathname: `system/${nearestplanet.star.name}`,
            state: { star: nearestplanet.star, scalefactor:0.4 },
            props: { timestamp: () => new Date().toString() }
          })
        }
      }
    })

if(this.props.location.state&&this.props.location.state.coord){
    const coord =this.props.location.state.coord
    this._map.setView([coord.latitude,coord.longitude], maxzoom-1)
}
   
  }

  updatemarker = () => {
    const L = require('Leaflet')
    this._markers.map(marker => this._map.removeLayer(marker))

    celestialObject.map(object => {
      const objectmarker = L.marker(object.coordinates as L.LatLngExpression, {
        icon: L.icon({
          iconUrl: object.image,
          iconSize: object.size
        })
      })
        .bindTooltip(object.name, { direction: 'left' })
        .openTooltip()
        .addTo(this._map)
      objectmarker.addEventListener('click', () =>
        this.props.history.push({
          pathname: `planet/${object.name}`
        })
      )
      this._markers.push(objectmarker)
    })
  }
  updatetime = (position: number) => {
    this.setState({ siderealtime: siderealtime(position) })
    this._interval = setInterval(() => {
      this.setState({ siderealtime: siderealtime(position) }, () =>
        this.updatemarker()
      )
    }, 60000)
  }
  onEachFeature = (feature: any, layer: any) => {
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
      const marker = L.circleMarker([coord[0][1], coord[0][0]], options)
        .addTo(this._map)
        .bindTooltip(feature.properties.constellation, {
          permanent: true,
          direction: 'left'
        })
        .openTooltip()
      marker.addEventListener('click', () =>
        this.props.history.push({
          pathname: `constellation/${feature.properties.constellationid}`,
          state: { constellation: feature.properties.constellationid }
        })
      )
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
      L.circleMarker([p[1], p[0]], options).addTo(this._map)
    })
  }

  render() {
    const { longitude, latitude, siderealtime } = this.state
    return (
      <React.Fragment>
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
        <div id="map" />
      </React.Fragment>
    )
  }
}
