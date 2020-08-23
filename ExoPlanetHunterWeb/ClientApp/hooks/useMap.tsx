import { useState, useEffect } from 'react'
import { Map } from 'leaflet'


const useMap = (options: any) => {
    const [mapContainer, setMapContainer] = useState<any>(null)
    const [map, setMap] = useState<any>(null)

    useEffect(() => {
        setMap(createMap(mapContainer, options))
    }, [mapContainer])

    return [map, setMapContainer]
}
const createMap = (
    mapContainer: HTMLDivElement | null,
    options:any,
) => {
    if (mapContainer == null) {
        return
    }
    const L = require('Leaflet')
    const { lat, lng, zoom, minZoom, maxZoom } = options

    return L.map(mapContainer, {
        preferCanvas: true,
        center: new L.LatLng(lat, lng),
        attributionControl: false,
        zoom,
        maxZoom,
        minZoom,
  
    })
}

const createMapLayer = (
    map: L.Map | undefined,
    mapUrl: string,
    mapConfig: any = {},
) => {
    if (map == null) {
        return
    }
    const L = require('Leaflet')
    const layer = L.tileLayer(mapUrl, mapConfig)

    map.addLayer(layer)

    return layer
}

const useBaseLayer = (map: Map, url: string) => {
    const [baseLayer, setBaseLayer] = useState<any | null>(null)

    useEffect(() => {
        if (!map) {
            return
        }

        if (baseLayer) {
            setTimeout(() => {
                map.removeLayer(baseLayer)
            }, 250)
        }

        setBaseLayer(createMapLayer(map, url))
    }, [map, url])

    return [baseLayer]
}

export { useMap, useBaseLayer }
