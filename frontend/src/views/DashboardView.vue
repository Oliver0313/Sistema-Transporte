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

        <div class="request-item">
          <div>
            <strong>Recursos Humanos</strong>
            <p>Transporte para capacitación</p>
          </div>
          <span class="badge pendiente">Pendiente</span>
        </div>

        <div class="request-item">
          <div>
            <strong>Finanzas</strong>
            <p>Reunión con proveedores</p>
          </div>
          <span class="badge aprobada">Aprobada</span>
        </div>

        <div class="request-item">
          <div>
            <strong>Tecnología</strong>
            <p>Soporte en sucursal</p>
          </div>
          <span class="badge viaje">En viaje</span>
        </div>
      </section>
    </div>

    <div class="dashboard-bottom">
      <section class="panel">
        <div class="panel-header">
          <h3>Próximos viajes programados</h3>
          <span>Ver agenda</span>
        </div>

        <div class="trip-item">
          <span>09:00 AM</span>
          <div>
            <strong>Capacitación del personal</strong>
            <p>Recursos Humanos</p>
          </div>
        </div>

        <div class="trip-item">
          <span>11:30 AM</span>
          <div>
            <strong>Reunión con proveedores</strong>
            <p>Finanzas</p>
          </div>
        </div>
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

onMounted(cargarDashboard)
</script>

<style scoped>
.dashboard-page {
  display: flex;
  flex-direction: column;
  gap: 26px;
}

.dashboard-header h2 {
  margin: 0;
  font-size: 1.8rem;
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
  grid-template-columns: repeat(6, 1fr);
  gap: 18px;
}

.stat-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 20px;
  min-height: 165px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-card strong {
  font-size: 2rem;
  color: #111827;
  line-height: 1;
}

.stat-card span {
  color: #111827;
  font-weight: 700;
  font-size: 0.9rem;
}

.stat-card p {
  color: #9ca3af;
  font-size: 0.8rem;
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
  border-radius: 18px;
  padding: 20px;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 18px;
}

.panel-header h3 {
  margin: 0;
  color: #111827;
  font-size: 1rem;
  font-weight: 800;
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
  font-size: 2rem;
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