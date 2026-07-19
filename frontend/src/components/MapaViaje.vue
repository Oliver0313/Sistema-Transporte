<template>
  <div class="map-wrapper">
    <div ref="mapContainer" class="mapa-container"></div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue'
import mapboxgl from 'mapbox-gl'

const props = defineProps({
  viaje: Object
})

const mapContainer = ref(null)
let map = null
let origenMarker = null
let destinoMarker = null

const coordenadasCiudades = {
  'Santo Domingo': [-69.9312, 18.4861],
  'Santiago': [-70.6970, 19.4517],
  'San Pedro de Macoris': [-69.2972, 18.4539],
  'San Pedro de Macorís': [-69.2972, 18.4539],
  'La Romana': [-68.9728, 18.4273],
  'Bonao': [-70.4092, 18.9369],
  'Puerto Plata': [-70.6871, 19.7808],
  'Punta Cana': [-68.4055, 18.5601],
  'Higüey': [-68.7078, 18.6150],
  'Higuey': [-68.7078, 18.6150],
  'San Cristóbal': [-70.1090, 18.4167],
  'San Cristobal': [-70.1090, 18.4167],
  'Baní': [-70.3303, 18.2796],
  'Bani': [-70.3303, 18.2796],
  'Azua': [-70.7349, 18.4532]
}

const obtenerCoordenadas = (ciudad) => {
  return coordenadasCiudades[ciudad] || [-69.9312, 18.4861]
}

async function obtenerRuta(origen, destino) {

  const url =
    `https://api.mapbox.com/directions/v5/mapbox/driving/` +
    `${origen[0]},${origen[1]};${destino[0]},${destino[1]}` +
    `?geometries=geojson&overview=full&access_token=${mapboxgl.accessToken}`

  const response = await fetch(url)

  const data = await response.json()

  if (!data.routes.length) return null

  return data.routes[0]

}

const actualizarMapa = async () => {
  if (!map || !props.viaje?.origen || !props.viaje?.destino) return

  const origen = obtenerCoordenadas(props.viaje.origen)
  const destino = obtenerCoordenadas(props.viaje.destino)

  if (origenMarker) origenMarker.remove()
  if (destinoMarker) destinoMarker.remove()

  origenMarker = new mapboxgl.Marker({ color: '#22C55E' })
    .setLngLat(origen)
    .setPopup(new mapboxgl.Popup().setText(`Origen: ${props.viaje.origen}`))
    .addTo(map)

  destinoMarker = new mapboxgl.Marker({ color: '#EF4444' })
    .setLngLat(destino)
    .setPopup(new mapboxgl.Popup().setText(`Destino: ${props.viaje.destino}`))
    .addTo(map)
      

const datosRuta = await obtenerRuta(origen, destino)

if (!datosRuta) return

const ruta = {
  type: 'Feature',
  geometry: datosRuta.geometry
}

  if (map.getSource('ruta')) {
    map.getSource('ruta').setData(ruta)
  } else {
    map.addSource('ruta', {
      type: 'geojson',
      data: ruta
    })

    map.addLayer({
      id: 'ruta-linea',
      type: 'line',
      source: 'ruta',
      paint: {
          'line-color': '#2563EB',
          'line-width': 7,
          'line-opacity': 0.9
      }
    })
  }

const bounds = new mapboxgl.LngLatBounds()

ruta.geometry.coordinates.forEach(coord => {
    bounds.extend(coord)
})

map.fitBounds(bounds,{
    padding:70
})

}

const resizeMap = () => {
  if (map) map.resize()
}

onMounted(async () => {
  await nextTick()

  mapboxgl.accessToken = import.meta.env.VITE_MAPBOX_TOKEN

  map = new mapboxgl.Map({
    container: mapContainer.value,
    style: 'mapbox://styles/mapbox/streets-v12',
    center: [-70.25, 18.9],
    zoom: 12
  })

  map.addControl(new mapboxgl.NavigationControl(), 'top-right')

  map.on('load', () => {
    resizeMap()
    actualizarMapa()
  })

  window.addEventListener('resize', resizeMap)
})

watch(
  () => props.viaje,
  async () => {
    await nextTick()

    setTimeout(() => {
      resizeMap()

      if (!map) return

      if (map.loaded()) {
        actualizarMapa()
      } else {
        map.once('load', actualizarMapa)
      }
    }, 300)
  },
  { deep: true }
)

onUnmounted(() => {
  window.removeEventListener('resize', resizeMap)

  if (origenMarker) origenMarker.remove()
  if (destinoMarker) destinoMarker.remove()

  if (map) {
    map.remove()
    map = null
  }
})
</script>

<style scoped>
.map-wrapper {
  width: 100%;
  height: 600px;
}

.mapa-container {
  width: 100%;
  height: 100%;
  border-radius: 16px;
  overflow: hidden;
}
</style>