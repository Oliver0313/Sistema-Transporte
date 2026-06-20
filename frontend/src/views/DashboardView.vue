<template>
  <div class="dashboard-page">
    <div class="dashboard-header">
      <h2>Dashboard</h2>
      <p>Resumen general del sistema de transporte</p>
    </div>

    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon">🚐</div>
        <strong>{{ resumen.vehiculosDisponibles ?? 0 }}</strong>
        <span>Vehículos disponibles</span>
        <p>Listos para asignación</p>
      </div>

      <div class="stat-card">
        <div class="stat-icon">👨‍✈️</div>
        <strong>{{ resumen.conductoresDisponibles ?? 0 }}</strong>
        <span>Conductores disponibles</span>
        <p>Disponibles para viajes</p>
      </div>

      <div class="stat-card">
        <div class="stat-icon">📋</div>
        <strong>{{ resumen.solicitudesPendientes ?? 0 }}</strong>
        <span>Solicitudes pendientes</span>
        <p>Esperando aprobación</p>
      </div>

      <div class="stat-card">
        <div class="stat-icon">🛣️</div>
        <strong>{{ resumen.viajesEnCurso ?? 0 }}</strong>
        <span>Viajes en curso</span>
        <p>Actualmente activos</p>
      </div>

      <div class="stat-card">
        <div class="stat-icon">🔧</div>
        <strong>{{ resumen.vehiculosEnMantenimiento ?? 0 }}</strong>
        <span>En mantenimiento</span>
        <p>Fuera de servicio</p>
      </div>

      <div class="stat-card">
        <div class="stat-icon">⛽</div>
        <strong>{{ resumen.consumosPendientes ?? 0 }}</strong>
        <span>Consumos pendientes</span>
        <p>Esperando validación</p>
      </div>
    </div>

    <div class="dashboard-middle">
      <section class="panel panel-map">
        <div class="panel-header">
          <h3>Viajes activos en tiempo real</h3>
          <span>Ver todos</span>
        </div>

        <div class="map-placeholder">
          Mapa próximamente
        </div>
      </section>

      <section class="panel">
        <div class="panel-header">
          <h3>Solicitudes recientes</h3>
          <span>Ver todas</span>
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
          <span>Ver agenda</span>
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
          <span>Ver todos</span>
        </div>

        <div class="vehicle-status">
          <div class="circle-status">
            <strong>{{ resumen.vehiculosDisponibles ?? 0 }}</strong>
            <span>Disponibles</span>
          </div>
        </div>
      </section>

      <section class="panel">
        <div class="panel-header">
          <h3>Consumo de combustible</h3>
          <span>Ver reporte</span>
        </div>

        <div class="fuel-box">
          <strong>{{ resumen.consumosPendientes ?? 0 }}</strong>
          <span>Registros pendientes</span>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

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
  display: flex;
  flex-direction: column;
  gap: 26px;
}

.dashboard-header h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
}

.dashboard-header p {
  margin-top: 6px;
  color: #6b7280;
  font-size: 0.9rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  gap: 18px;
}

.stat-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 16px;
  min-height: 125px;
  display: flex;
  flex-direction: column;
  gap: 8px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.04);
}

.stat-icon {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-card strong {
  font-size: 1.4rem;
  font-weight: 700;
  color: #111827;
}

.stat-card span {
  font-size: 0.85rem;
  font-weight: 600;
  color: #111827;
}

.stat-card p {
  font-size: 0.8rem;
  color: #9ca3af;
  margin: 0;
}

.dashboard-middle {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
}

.dashboard-bottom {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 20px;
}

.panel {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 20px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.04);
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 18px;
}

.panel-header h3 {
  margin: 0;
  font-size: 0.95rem;
  font-weight: 700;
  color: #111827;
}

.panel-header span {
  font-size: 0.8rem;
  color: #6b7280;
  cursor: pointer;
}

.map-placeholder {
  height: 320px;
  border-radius: 14px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #9ca3af;
}

.request-item,
.trip-item {
  display: flex;
  justify-content: space-between;
  gap: 14px;
  align-items: center;
  padding: 14px 0;
  border-bottom: 1px solid #f3f4f6;
}

.request-item strong,
.trip-item strong {
  color: #111827;
  font-size: 0.9rem;
}

.request-item p,
.trip-item p {
  color: #6b7280;
  font-size: 0.8rem;
  margin-top: 4px;
}

.trip-item > span {
  color: #6b7280;
  font-size: 0.8rem;
  min-width: 70px;
}

.badge {
  padding: 6px 12px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
}

.pendiente {
  background: #fef3c7;
  color: #92400e;
}

.aprobada {
  background: #dcfce7;
  color: #166534;
}

.rechazada {
  background: #fee2e2;
  color: #991b1b;
}

.cancelada {
  background: #e5e7eb;
  color: #374151;
}

.finalizada,
.viaje {
  background: #dbeafe;
  color: #1d4ed8;
}

.vehicle-status,
.fuel-box {
  min-height: 160px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.circle-status {
  width: 130px;
  height: 130px;
  border-radius: 50%;
  border: 18px solid #22c55e;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
}

.circle-status strong,
.fuel-box strong {
  font-size: 1.7rem;
  color: #111827;
}

.circle-status span,
.fuel-box span {
  color: #6b7280;
  font-size: 0.8rem;
}

.fuel-box {
  flex-direction: column;
  gap: 6px;
}

.empty-text {
  color: #6b7280;
  font-size: 0.9rem;
}

@media (max-width: 1200px) {
  .stats-grid {
    grid-template-columns: repeat(3, 1fr);
  }

  .dashboard-middle,
  .dashboard-bottom {
    grid-template-columns: 1fr;
  }
}
</style>