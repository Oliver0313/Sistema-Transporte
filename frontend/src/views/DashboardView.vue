<template>
  <div class="dashboard-page">
    <div class="dashboard-header">
      <h2>Dashboard</h2>
      <p>Resumen general del sistema de transporte</p>
    </div>

    <div class="stats-grid">
      <div
        class="stat-card clickable-card"
        @click="router.push('/vehiculos')"
      >
      <div class="stat-icon">
        <CarFront :size="24" />
      </div>
        <strong>{{ resumen.vehiculosDisponibles ?? 0 }}</strong>
        <span>Vehículos disponibles</span>
        <p>Listos para asignación</p>
      </div>

      <div
        class="stat-card clickable-card"
        @click="router.push('/conductores')"
      >
      <div class="stat-icon">
        <UserRound :size="24" />
      </div>
        <strong>{{ resumen.conductoresDisponibles ?? 0 }}</strong>
        <span>Conductores disponibles</span>
        <p>Disponibles para viajes</p>
      </div>

      <div
        class="stat-card clickable-card"
        @click="router.push('/solicitudes')"
      >
      <div class="stat-icon">
        <ClipboardList :size="24" />
      </div>
        <strong>{{ resumen.solicitudesPendientes ?? 0 }}</strong>
        <span>Solicitudes pendientes</span>
        <p>Esperando aprobación</p>
      </div>

      <div
        class="stat-card clickable-card"
        @click="router.push('/viajes')"
      >
      <div class="stat-icon">
        <Route :size="24" />
      </div>
        <strong>{{ resumen.viajesEnCurso ?? 0 }}</strong>
        <span>Viajes en curso</span>
        <p>Actualmente activos</p>
      </div>

      <div
        v-if="puedeVerTodo"
        class="stat-card clickable-card"
        @click="router.push('/mantenimiento')"
      >
        <div class="stat-icon">
          <Wrench :size="24" />
        </div>

        <strong>{{ resumen.vehiculosEnMantenimiento ?? 0 }}</strong>
        <span>En mantenimiento</span>
        <p>Fuera de servicio</p>
      </div>

      <div
        v-if="puedeVerTodo"
        class="stat-card clickable-card"
        @click="router.push('/combustible')"
      >
        <div class="stat-icon">
          <Fuel :size="24" />
        </div>

        <strong>{{ resumen.consumosPendientes ?? 0 }}</strong>
        <span>Consumos pendientes</span>
        <p>Esperando validación</p>
      </div>
    </div>

    <div class="dashboard-middle">
      <section class="panel panel-map">
        <div class="panel-header">
          <h3>Viajes activos en tiempo real</h3>
            <span
            class="dashboard-link"
            @click="router.push('/Viajes')"
          >
            Ver todos
          </span>
        </div>

        <div class="dashboard-map">
          <MapaViaje :viaje="viajeSeleccionado" />
        </div>
      </section>

      <section class="panel">
        <div class="panel-header">
          <h3>Solicitudes recientes</h3>
          <span
            class="dashboard-link"
            @click="router.push('/solicitudes')"
          >
            Ver todos
          </span>
        </div>

        <div
          v-for="solicitud in solicitudesRecientes"
          :key="solicitud.id"
          class="request-item"
        >
          <div>
            <strong>{{ solicitud.areaSolicitante }}</strong>
            <p>{{ solicitud.motivo }}</p>
          </div>

          <span :class="['badge', obtenerClaseEstado(solicitud.estado)]">
            {{ formatearEstado(solicitud.estado) }}
          </span>
        </div>

        <p v-if="solicitudesRecientes.length === 0" class="empty-text">
          No hay solicitudes recientes.
        </p>
      </section>
    </div>

    <div class="dashboard-bottom">
      <section class="panel">
        <div class="panel-header">
          <h3>Próximos viajes programados</h3>
          <span
            class="dashboard-link"
            @click="router.push('/calendario')"
          >
            Ver todos
          </span>
        </div>

        <div
          v-for="viaje in viajesProximos"
          :key="viaje.id"
          class="trip-item"
        >
          <span>{{ formatearHora(viaje.fechaHoraSalida) }}</span>

          <div>
            <strong>{{ viaje.origen }} → {{ viaje.destino }}</strong>

            <p>
              {{ viaje.cantidadPasajeros }} pasajeros ·
              {{ formatearEstadoViaje(viaje.estado) }}
            </p>
          </div>
        </div>

        <p
          v-if="viajesProximos.length === 0"
          class="empty-text"
        >
          No hay viajes programados.
        </p>
      </section>

      <section class="panel">
        <div class="panel-header">
          <h3>Estado de vehículos</h3>
          <span
            class="dashboard-link"
            @click="router.push('/vehiculos')"
          >
            Gestionar
          </span>
        </div>

       <div class="vehicle-status-list">
          <div class="vehicle-status-row">
            <span>Disponibles</span>
            <strong>{{ resumen.vehiculosDisponibles ?? 0 }}</strong>
          </div>

          <div class="vehicle-status-row">
            <span>Mantenimiento</span>
            <strong>{{ resumen.vehiculosEnMantenimiento ?? 0 }}</strong>
          </div>

          <div class="vehicle-status-row">
            <span>Viajes activos</span>
            <strong>{{ resumen.viajesEnCurso ?? 0 }}</strong>
          </div>
        </div>
      </section>

      <section class="panel">
        <div class="panel-header">
          <h3>Consumo de combustible</h3>
          <span
            class="dashboard-link"
            @click="router.push('/combustible')"
          >
            Ver reporte
          </span>
        </div>

                <div class="fuel-box">
          <strong>{{ resumen.consumosPendientes ?? 0 }}</strong>
          <span>Registros pendientes</span>
          <small>Revisión de consumo requerida</small>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import MapaViaje from '../components/MapaViaje.vue'
import { useRouter } from 'vue-router'

import {
  CarFront,
  UserRound,
  ClipboardList,
  Route,
  Wrench,
  Fuel,
  ArrowRight
} from 'lucide-vue-next'

const router = useRouter()

const rolUsuario = (localStorage.getItem('usuario_rol') || '').trim().toLowerCase()

const esSuperAdmin = computed(() => rolUsuario === 'superadmin')
const esAdministrador = computed(() => rolUsuario === 'administrador')
const esSupervisor = computed(() => rolUsuario === 'supervisor')
const esOperador = computed(() => rolUsuario === 'operador')

const puedeVerTodo = computed(() =>
  esSuperAdmin.value || esAdministrador.value || esSupervisor.value
)

const puedeVerOperativo = computed(() =>
  puedeVerTodo.value || esOperador.value
)

const resumen = ref({})
const solicitudesRecientes = ref([])

const cargarDashboard = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch('https://localhost:7221/api/dashboard/resumen', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.ok) {
    resumen.value = await response.json()
  }
}

const viajesProximos = ref([])
const cargarViajesProximos = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch('https://localhost:7221/api/Viajes', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.ok) {
    const data = await response.json()

    viajesProximos.value = data.slice(0, 3)
  }
}

const cargarSolicitudesRecientes = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch('https://localhost:7221/api/solicitudestransporte', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.ok) {
    const data = await response.json()
    solicitudesRecientes.value = data.slice(0, 3)
  }
}

const formatearEstado = (estado) => {
  const estados = {
    1: 'Pendiente',
    2: 'Aprobada',
    3: 'Rechazada',
    4: 'Cancelada',
    5: 'Finalizada'
  }

  return estados[estado] || 'Pendiente'
}

const obtenerClaseEstado = (estado) => {
  const clases = {
    1: 'pendiente',
    2: 'aprobada',
    3: 'rechazada',
    4: 'cancelada',
    5: 'finalizada'
  }

  return clases[estado] || 'pendiente'
}

const formatearHora = (fecha) => {
  if (!fecha) return '---'

  return new Date(fecha).toLocaleTimeString('es-DO', {
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  })
}

const formatearEstadoViaje = (estado) => {
  const estados = {
    1: 'Programado',
    2: 'En curso',
    3: 'Finalizado',
    4: 'Cancelado'
  }

  return estados[estado] || 'Sin estado'
}

onMounted(() => {
  cargarDashboard()
  cargarSolicitudesRecientes()
  cargarViajesProximos()
})
</script>

<style scoped>
.dashboard-page {
  padding: 24px;
  background: #f8fafc;
  min-height: 100vh;
}

.dashboard-header {
  margin-bottom: 22px;
}

.dashboard-header h2 {
  margin: 0;
  font-size: 1.7rem;
  font-weight: 800;
  color: #111827;
}

.dashboard-header p {
  margin-top: 6px;
  color: #6b7280;
  font-size: 0.95rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 18px;
  margin-bottom: 24px;
}

.stat-card {
  position: relative;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 18px;
  min-height: 125px;
  display: grid;
  grid-template-columns: 48px 1fr;
  column-gap: 14px;
  align-items: start;
  box-shadow: 0 8px 22px rgba(15, 23, 42, 0.04);
  transition: all 0.2s ease;
}

.stat-card::before {
  content: "";
  position: absolute;
  left: 18px;
  right: 18px;
  top: 0;
  height: 3px;
  border-radius: 999px;
  background: #111827;
  opacity: 0.08;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 14px 30px rgba(15, 23, 42, 0.08);
}

.stat-icon {
  width: 44px;
  height: 44px;
  border-radius: 14px;
  background: #f3f4f6;
  color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
  grid-row: span 3;
}

.stat-card strong {
  font-size: 1.85rem;
  font-weight: 800;
  color: #111827;
  line-height: 1;
  margin-top: 2px;
}

.stat-card span {
  grid-column: 2;
  font-size: 0.88rem;
  font-weight: 700;
  color: #374151;
  order: -1;
}

.stat-card p {
  grid-column: 2;
  margin: 0;
  color: #6b7280;
  font-size: 0.82rem;
}

.dashboard-middle {
  display: grid;
  grid-template-columns: 1.6fr 1fr;
  gap: 20px;
  margin-bottom: 20px;
}

.dashboard-bottom {
  display: grid;
  grid-template-columns: 1.1fr 0.9fr 0.9fr;
  gap: 20px;
}

.panel {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 20px;
  box-shadow: 0 8px 24px rgba(15, 23, 42, 0.04);
}

.panel-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 14px;
}

.panel-header h3 {
  margin: 0;
  font-size: 1rem;
  font-weight: 800;
  color: #111827;
}

.dashboard-link {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  cursor: pointer;
  color: #374151;
  font-size: 0.85rem;
  font-weight: 700;
  transition: all .2s ease;
}

.dashboard-link:hover {
  color: #111827;
  text-decoration: underline;
}

.dashboard-map {
  height: 220px;
  overflow: hidden;
  border-radius: 16px;
  background: #f3f4f6;
}

.request-item,
.trip-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 14px;
  padding: 14px 0;
  border-top: 1px solid #f3f4f6;
}

.request-item:first-of-type,
.trip-item:first-of-type {
  border-top: none;
}

.request-item strong,
.trip-item strong {
  color: #111827;
  font-size: 0.92rem;
  font-weight: 700;
}

.request-item p,
.trip-item p {
  margin: 4px 0 0;
  color: #6b7280;
  font-size: 0.82rem;
}

.trip-item > span {
  min-width: 78px;
  background: #f3f4f6;
  color: #111827;
  border-radius: 12px;
  padding: 8px 10px;
  text-align: center;
  font-size: 0.8rem;
  font-weight: 700;
}

.vehicle-status,
.fuel-box {
  min-height: 150px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.circle-status {
  width: 135px;
  height: 135px;
  border-radius: 999px;
  border: 10px solid #e5e7eb;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
}

.circle-status strong,
.fuel-box strong {
  font-size: 2rem;
  color: #111827;
}

.circle-status span,
.fuel-box span {
  color: #6b7280;
  font-size: 0.85rem;
}

.empty-text {
  color: #9ca3af;
  font-size: 0.9rem;
  padding: 18px 0;
}

@media (max-width: 1200px) {
  .dashboard-middle,
  .dashboard-bottom {
    grid-template-columns: 1fr;
  }
}

.panel-map {
  min-height: 300px;
}

@media (max-width: 1100px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 700px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
}

.fuel-box {
  min-height: 150px;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  flex-direction: column;
  gap: 6px;
}

.fuel-box strong {
  font-size: 2rem;
  color: #111827;
}

.fuel-box span {
  color: #374151;
  font-weight: 700;
}

.fuel-box small {
  color: #6b7280;
  font-size: 0.82rem;
}

.vehicle-status-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-top: 10px;
}

.vehicle-status-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #f9fafb;
  border: 1px solid #eef2f7;
  border-radius: 14px;
  padding: 12px 14px;
}

.vehicle-status-row span {
  color: #6b7280;
  font-size: 0.9rem;
}

.vehicle-status-row strong {
  color: #111827;
  font-size: 1.15rem;
  font-weight: 700;
}

.badge {
  padding: 6px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
  white-space: nowrap;
}

.badge.pendiente {
  background: #fef3c7;
  color: #92400e;
}

.badge.aprobada {
  background: #dcfce7;
  color: #166534;
}

.badge.rechazada {
  background: #fee2e2;
  color: #991b1b;
}

.badge.cancelada {
  background: #f3f4f6;
  color: #374151;
}

.badge.finalizada {
  background: #dbeafe;
  color: #1e40af;
}
</style>