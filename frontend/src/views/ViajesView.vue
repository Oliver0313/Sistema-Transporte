<template>
  <div>
    <div class="dashboard-header">
      <div class="dashboard-header-left">
        <h2>Viajes</h2>
        <p>Administra los viajes activos, finalizados y su seguimiento en tiempo real.</p>
      </div>

      <div class="dashboard-header-badge">
        {{ viajesEnCurso }} en curso
      </div>
    </div>

    <div class="stats-grid">
  <div class="stat-card">
    <div class="stat-icon">
      <Route :size="24" />
    </div>

    <div class="stat-info">
      <span>Viajes activos</span>
      <strong>{{ viajesEnCurso }}</strong>
    </div>
  </div>

  <div class="stat-card">
    <div class="stat-icon">
      <MapPinned :size="24" />
    </div>

    <div class="stat-info">
      <span>Vehículos en ruta</span>
      <strong>{{ viajesEnCurso }}</strong>
    </div>
  </div>

  <div class="stat-card">
    <div class="stat-icon">
      <CircleCheck :size="24" />
    </div>

    <div class="stat-info">
      <span>Finalizados</span>
      <strong>{{ viajesFinalizados }}</strong>
    </div>
  </div>

  <div class="stat-card">
    <div class="stat-icon">
      <ClipboardList :size="24" />
    </div>

    <div class="stat-info">
      <span>Total viajes</span>
      <strong>{{ totalViajes }}</strong>
    </div>
  </div>
</div>

<div class="filters-bar">
  <div class="search-wrapper">
    <Search :size="16" class="search-icon" />

    <input
      v-model="filtroBusqueda"
      type="text"
      placeholder="Buscar viaje..."
      class="search-box"
    />
  </div>

  <select v-model="filtroEstado" class="filter-select">
    <option value="todos">Todos</option>
    <option value="1">Programados</option>
    <option value="2">En curso</option>
    <option value="3">Finalizados</option>
    <option value="4">Cancelados</option>
  </select>
</div>

    <div class="viajes-layout">
      <section class="viajes-list">
        <div
            v-for="viaje in viajesFiltrados"
            :key="viaje.id"
            class="viaje-card"
            :class="{ selected: viajeSeleccionado?.id === viaje.id }"
            @click="seleccionarViaje(viaje)"
        >
            <div class="viaje-card-header">
            <span :class="['estado-pill', obtenerClaseEstado(viaje.estado)]">
                {{ formatearEstado(viaje.estado) }}
            </span>
            </div>

            <h4>{{ viaje.origen }} → {{ viaje.destino }}</h4>

            <p>{{ viaje.cantidadPasajeros }} pasajeros</p>

            <small>
            {{ formatearFecha(viaje.fechaHoraSalida) }}
            </small>
        </div>

        <p v-if="viajesFiltrados.length === 0" class="empty-text">
            No se encontraron viajes.
        </p>
        </section>

      <section class="map-panel">
        <MapaViaje :viaje="viajeSeleccionado" />
      </section>

            <aside class="detalle-panel">
        <h3>Información detallada</h3>

        <div
            v-if="viajeSeleccionado"
            :key="viajeSeleccionado.id"
            class="detalle-content"
            >

        <span :class="['estado-pill', obtenerClaseEstado(viajeSeleccionado.estado)]">
            {{ formatearEstado(viajeSeleccionado.estado) }}
        </span>

        <div class="detail-item">
            <span>Ruta</span>
            <strong>
            {{ viajeSeleccionado.origen }} → {{ viajeSeleccionado.destino }}
            </strong>
        </div>

        <div class="detail-item">
            <span>Pasajeros</span>
            <strong>
            {{ viajeSeleccionado.cantidadPasajeros }}
            </strong>
        </div>

        <div class="detail-item">
            <span>Salida</span>
            <strong>
            {{ formatearFecha(viajeSeleccionado.fechaHoraSalida) }}
            </strong>
        </div>

        <div class="detail-item">
            <span>Llegada</span>
            <strong>
            {{ formatearFecha(viajeSeleccionado.fechaHoraLlegada) }}
            </strong>
        </div>

        <div class="detail-item">
            <span>Distancia</span>
            <strong>
            {{ viajeSeleccionado.distanciaRecorrida }} km
            </strong>
        </div>

        <div class="detail-item">
            <span>Observaciones</span>
            <strong>
            {{ viajeSeleccionado.observaciones || 'Sin observaciones' }}
            </strong>
        </div>

        </div>

        <p v-else class="empty-text">
            Selecciona un viaje para ver el detalle.
        </p>
        </aside>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import MapaViaje from '../components/MapaViaje.vue'

import {
  Route,
  MapPinned,
  CircleCheck,
  ClipboardList,
  Search
} from 'lucide-vue-next'

const viajes = ref([])
const viajeSeleccionado = ref(null)
const filtroBusqueda = ref('')
const filtroEstado = ref('todos')

const seleccionarViaje = (viaje) => {
  viajeSeleccionado.value = viaje
}

const viajesFiltrados = computed(() => {
  const texto = filtroBusqueda.value.toLowerCase().trim()

  let resultado = viajes.value

  if (texto) {
    resultado = resultado.filter(v =>
      v.origen?.toLowerCase().includes(texto) ||
      v.destino?.toLowerCase().includes(texto) ||
      formatearEstado(v.estado).toLowerCase().includes(texto) ||
      v.cantidadPasajeros?.toString().includes(texto)
    )
  }

  if (filtroEstado.value !== 'todos') {
    resultado = resultado.filter(v => v.estado === Number(filtroEstado.value))
  }

  return resultado
})

watch(
  viajesFiltrados,
  (lista) => {
    if (!lista.length) {
      viajeSeleccionado.value = null
      return
    }

    const sigueVisible = lista.some(v => v.id === viajeSeleccionado.value?.id)

    if (!viajeSeleccionado.value || !sigueVisible) {
      viajeSeleccionado.value = lista[0]
    }
  },
  { immediate: true }
)

const totalViajes = computed(() => viajes.value.length)

const viajesEnCurso = computed(() =>
  viajes.value.filter(v => v.estado === 2).length
)

const viajesFinalizados = computed(() =>
  viajes.value.filter(v => v.estado === 3).length
)

const viajesCancelados = computed(() =>
  viajes.value.filter(v => v.estado === 4).length
)

const formatearEstado = (estado) => {
  const estados = {
    1: 'Programado',
    2: 'En curso',
    3: 'Finalizado',
    4: 'Cancelado'
  }

  return estados[estado] || 'Sin estado'
}

const obtenerClaseEstado = (estado) => {
  const clases = {
    1: 'programado',
    2: 'en-curso',
    3: 'finalizado',
    4: 'cancelado'
  }

  return clases[estado] || 'programado'
}

const formatearFecha = (fecha) => {
  if (!fecha) return '---'

  return new Date(fecha).toLocaleString('es-DO', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const cargarViajes = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch('https://localhost:7221/api/Viajes', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.ok) {
    const data = await response.json()

    viajes.value = data

    if (data.length > 0) {
      viajeSeleccionado.value = data[0]
    }
  }
}

onMounted(() => {
  cargarViajes()
})

</script>
<style scoped>
.section-header {
  margin-bottom: 20px;
}

.section-header h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
}

.section-header p {
  margin-top: 6px;
  color: #6b7280;
  font-size: 0.9rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}

.stat-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 16px;
  display: flex;
  align-items: center;
  gap: 14px;
}

.stat-icon {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.stat-card span {
  font-size: 0.85rem;
  color: #4b5563;
  font-weight: 500;
}

.stat-card strong {
  display: block;
  margin-top: 4px;
  font-size: 1.4rem;
  font-weight: 700;
  color: #111827;
}

.search-box {
  width: 100%;
  height: 40px;
  padding: 0 14px 0 42px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background: #ffffff;
  font-size: .9rem;
  box-sizing: border-box;
}

.search-icon {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  color: #9ca3af;
  pointer-events: none;
}

.search-box:focus {
  border-color: #111827;
  background: white;
}

.search-wrapper {
  position: relative;
  flex: 1;
  max-width: 500px;
}

.viajes-layout {
  display: grid;
  grid-template-columns: 0.9fr 1.7fr 1fr;
  gap: 20px;
  align-items: start;
}

.viajes-list,
.map-panel,
.detalle-panel {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 16px;
}

.viajes-list {
  max-height: 620px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.viaje-card {
  border: 1px solid #e5e7eb;
  border-radius: 14px;
  padding: 14px;
  cursor: pointer;
  transition: all 0.2s ease;
  background: white;
}

.viaje-card:hover {
  background: #f9fafb;
  transform: translateY(-2px);
}

.viaje-card.selected {
  border: 2px solid #111827;
  background: #f3f4f6;
}

.viaje-card h4 {
  margin: 8px 0 6px;
  font-size: 0.95rem;
  font-weight: 700;
  color: #111827;
}

.viaje-card p {
  margin: 0 0 4px;
  color: #4b5563;
  font-size: 0.85rem;
}

.viaje-card small {
  color: #6b7280;
  font-size: 0.75rem;
}

.estado-pill {
  display: inline-block;
  padding: 5px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 600;
  margin-bottom: 8px;
}

.estado-pill.programado {
  background: #dbeafe;
  color: #1d4ed8;
}

.estado-pill.en-curso {
  background: #dcfce7;
  color: #166534;
}

.estado-pill.finalizado {
  background: #e5e7eb;
  color: #374151;
}

.estado-pill.cancelado {
  background: #fee2e2;
  color: #991b1b;
}

.detalle-panel h3 {
  margin-top: 0;
  margin-bottom: 16px;
  font-size: 1rem;
  font-weight: 700;
  color: #111827;
}

.detalle-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.detail-item {
  background: #f9fafb;
  border-radius: 12px;
  padding: 12px;
}

.detail-item span {
  display: block;
  font-size: 0.78rem;
  color: #6b7280;
  margin-bottom: 4px;
}

.detail-item strong {
  font-size: 0.9rem;
  color: #111827;
}

.empty-text {
  color: #6b7280;
  font-size: 0.9rem;
}

@media (max-width: 1200px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .viajes-layout {
    grid-template-columns: 1fr;
  }
}

.filters-bar {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 20px;
  max-width: 860px;
}

.search-box {
  flex: 1;
  margin-bottom: 0;
}

.filter-select {
  width: 220px;
  height: 40px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background: #ffffff;
  padding: 0 12px;
  font-size: .9rem;
}

.stat-icon {
  width: 48px;
  height: 48px;
  min-width: 48px;

  display: flex;
  align-items: center;
  justify-content: center;

  background: #f3f4f6;
  border-radius: 14px;

  color: #111827;
}

.stat-icon svg {
  width: 24px;
  height: 24px;
}

.stat-icon-img {
  width: 24px;         
  height: 24px;    
  object-fit: contain; 
}
</style>