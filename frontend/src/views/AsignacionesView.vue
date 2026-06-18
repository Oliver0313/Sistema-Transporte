<template>
  <div>
    <div class="section-header">
      <h2>Asignaciones</h2>
      <p>Gestión y consulta de asignaciones de transporte</p>
    </div>

    <div class="card-panel">
      <div class="table-responsive">
        <table class="asignaciones-table">
            <thead>
            <tr>
                <th>Solicitud</th>
                <th>Conductor</th>
                <th>Vehículo</th>
                <th>Fecha asignación</th>
                <th>Usuario asignador</th>
                <th>Estado</th>
            </tr>
            </thead>

            <tbody>
            <tr v-for="asignacion in asignaciones" :key="asignacion.id">
                <td>#{{ asignacion.solicitudTransporteId }}</td>
                <td>#{{ asignacion.conductorId }}</td>
                <td>#{{ asignacion.vehiculoId }}</td>
                <td>{{ formatearFecha(asignacion.fechaHoraAsignacion) }}</td>
                <td>#{{ asignacion.usuarioAsignadorId }}</td>
                <td>
                <span :class="['estado-badge', obtenerClaseEstado(asignacion.estado)]">
                    {{ formatearEstado(asignacion.estado) }}
                </span>
                </td>
            </tr>
            </tbody>
        </table>
        </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const asignaciones = ref([])

const cargarAsignaciones = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch('https://localhost:7221/api/Asignaciones', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.ok) {
    asignaciones.value = await response.json()
  }
}

const formatearFecha = (fecha) => {
  if (!fecha) return '---'

  return new Date(fecha).toLocaleString('es-DO', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: true
  })
}

const formatearEstado = (estado) => {
  const estados = {
    1: 'Activa',
    2: 'Reasignada',
    3: 'Cancelada',
    4: 'Finalizada'
  }

  return estados[estado] || 'Sin estado'
}

const obtenerClaseEstado = (estado) => {
  const clases = {
    1: 'activa',
    2: 'reasignada',
    3: 'cancelada',
    4: 'finalizada'
  }

  return clases[estado] || 'activa'
}

onMounted(() => {
  cargarAsignaciones()
})
</script>

<style scoped>
.section-header {
  margin-bottom: 26px;
}

.section-header h2 {
  font-size: 1.8rem;
  font-weight: 800;
  color: #111827;
  margin: 0;
}

.section-header p {
  color: #6b7280;
  font-size: 0.95rem;
  margin-top: 6px;
}

.card-panel {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 24px;
}

.table-responsive {
  overflow-x: auto;
}

.asignaciones-table {
  width: 100%;
  border-collapse: collapse;
}

.asignaciones-table th {
  background: #f9fafb;
  padding: 14px;
  text-align: left;
  font-size: 0.85rem;
  font-weight: 700;
  color: #374151;
}

.asignaciones-table td {
  padding: 14px;
  border-top: 1px solid #e5e7eb;
  font-size: 0.9rem;
  color: #374151;
}

.estado-badge {
  padding: 5px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
}

.activa {
  background: #dcfce7;
  color: #166534;
}

.reasignada {
  background: #dbeafe;
  color: #1d4ed8;
}

.cancelada {
  background: #fee2e2;
  color: #991b1b;
}

.finalizada {
  background: #e5e7eb;
  color: #374151;
}
</style>