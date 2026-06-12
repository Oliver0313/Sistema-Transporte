<template>
  <div class="dashboard-page">
    <div class="dashboard-header">
      <h2>Dashboard</h2>
      <p>Resumen general del sistema</p>
    </div>

    <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon">🚐</div>
        <strong>{{ resumen.vehiculosDisponibles ?? 0 }}</strong>
        <span>Vehículos disponibles</span>
      </div>

      <div class="stat-card">
        <div class="stat-icon">👨‍✈️</div>
        <strong>{{ resumen.conductoresDisponibles ?? 0 }}</strong>
        <span>Conductores disponibles</span>
      </div>

      <div class="stat-card">
        <div class="stat-icon">📋</div>
        <strong>{{ resumen.solicitudesPendientes ?? 0 }}</strong>
        <span>Solicitudes pendientes</span>
      </div>

      <div class="stat-card">
        <div class="stat-icon">🛣️</div>
        <strong>{{ resumen.viajesEnCurso ?? 0 }}</strong>
        <span>Viajes en curso</span>
      </div>

      <div class="stat-card">
        <div class="stat-icon">🔧</div>
        <strong>{{ resumen.vehiculosEnMantenimiento ?? 0 }}</strong>
        <span>En mantenimiento</span>
      </div>

      <div class="stat-card">
        <div class="stat-icon">⛽</div>
        <strong>{{ resumen.consumosPendientes ?? 0 }}</strong>
        <span>Consumos pendientes</span>
      </div>
    </div>
  </div>

  <div class="dashboard-sections">
  <section class="panel panel-large">
    <h3>Viajes activos en tiempo real</h3>
  </section>

  <section class="panel">
    <h3>Solicitudes recientes</h3>
  </section>
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
  gap: 24px;
}

.dashboard-header h2 {
  margin: 0;
  color: #111827;
}

.dashboard-header p {
  margin-top: 4px;
  color: #6b7280;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(190px, 1fr));
  gap: 18px;
}

.stat-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 22px;
  min-height: 150px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.stat-icon {
  width: 46px;
  height: 46px;
  border-radius: 14px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-card strong {
  font-size: 2rem;
  color: #111827;
}

.stat-card span {
  color: #374151;
  font-weight: 600;
}

.dashboard-sections {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 20px;
}

.panel {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 20px;
  min-height: 260px;
}

.panel h3 {
  margin: 0;
  font-size: 1rem;
  color: #111827;
}
</style>