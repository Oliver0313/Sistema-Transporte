<template>
  <div>
    <div class="section-header">
      <h2>Asignaciones</h2>
      <p>Gestión y consulta de asignaciones de transporte</p>
    </div>

    <div class="stats-grid">
      <div class="stat-card">
        <span>Total</span>
        <strong>{{ totalAsignaciones }}</strong>
      </div>

      <div class="stat-card">
        <span>En curso</span>
        <strong>{{ asignacionesEnCurso }}</strong>
      </div>

      <div class="stat-card">
        <span>Completas</span>
        <strong>{{ asignacionesCompletas }}</strong>
      </div>

      <div class="stat-card">
        <span>Canceladas</span>
        <strong>{{ asignacionesCanceladas }}</strong>
      </div>

      <button class="btn-primary" @click="mostrarModal = true">
        Nueva Asignación
      </button>
    </div>

    <div class="content-layout">
      <div class="main-content">
        <div class="toolbar">
          <input
            v-model="filtroBusqueda"
            type="text"
            placeholder="Buscar asignación..."
            class="search-box"
          />
        </div>

        <div class="tabs">
      <button
        class="tab"
        :class="{ active: filtroEstado === 'todas' }"
        @click="filtroEstado = 'todas'"
      >
        Todas
      </button>

      <button
        class="tab"
        :class="{ active: filtroEstado === 'activas' }"
        @click="filtroEstado = 'activas'"
      >
        Activas
      </button>

      <button
        class="tab"
        :class="{ active: filtroEstado === 'finalizadas' }"
        @click="filtroEstado = 'finalizadas'"
      >
        Finalizadas
      </button>

      <button
        class="tab"
        :class="{ active: filtroEstado === 'canceladas' }"
        @click="filtroEstado = 'canceladas'"
      >
        Canceladas
      </button>
    </div>

        <div class="table-responsive">
          <table class="asignaciones-table">
            <thead>
              <tr>
                <th>Solicitud</th>
                <th>Conductor</th>
                <th>Vehículo</th>
                <th>Fecha</th>
                <th>Usuario</th>
                <th>Estado</th>
              </tr>
            </thead>

            <tbody>
              <tr
                v-for="asignacion in asignacionesFiltradas"
                :key="asignacion.id"
                class="clickable-row"
                @click="asignacionSeleccionada = asignacion"
              >
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

      <aside class="detalle-panel">
        <h3>Detalle de Asignación</h3>

        <div v-if="asignacionSeleccionada">
          <div class="detail-card">
            <span>ID</span>
            <strong>#{{ asignacionSeleccionada.id }}</strong>
          </div>

          <div class="detail-card">
            <span>Solicitud</span>
            <strong>#{{ asignacionSeleccionada.solicitudTransporteId }}</strong>
          </div>

          <div class="detail-card">
            <span>Conductor</span>
            <strong>#{{ asignacionSeleccionada.conductorId }}</strong>
          </div>

          <div class="detail-card">
            <span>Vehículo</span>
            <strong>#{{ asignacionSeleccionada.vehiculoId }}</strong>
          </div>

          <div class="detail-card">
            <span>Usuario asignador</span>
            <strong>#{{ asignacionSeleccionada.usuarioAsignadorId }}</strong>
          </div>

          <div class="detail-card">
            <span>Estado</span>
            <strong>{{ formatearEstado(asignacionSeleccionada.estado) }}</strong>
          </div>

          <div class="detail-card">
            <span>Fecha</span>
            <strong>{{ formatearFecha(asignacionSeleccionada.fechaHoraAsignacion) }}</strong>
          </div>
        </div>

        <p v-else class="empty-text">
          Selecciona una asignación para ver el detalle.
        </p>
      </aside>
    </div>

    <div v-if="mostrarModal" class="modal-overlay">
      <div class="modal-container">
        <div class="modal-header">
          <h3>Nueva asignación</h3>

          <button class="btn-close" @click="mostrarModal = false">
            ×
          </button>
        </div>

        <form class="form-grid">
          <input
            v-model.number="formAsignacion.solicitudTransporteId"
            type="number"
            placeholder="ID Solicitud"
          />

          <input
            v-model.number="formAsignacion.conductorId"
            type="number"
            placeholder="ID Conductor"
          />

          <input
            v-model.number="formAsignacion.vehiculoId"
            type="number"
            placeholder="ID Vehículo"
          />

          <input
            v-model.number="formAsignacion.usuarioAsignadorId"
            type="number"
            placeholder="ID Usuario asignador"
          />

          <div class="modal-actions">
            <button
              type="button"
              class="btn-cancel"
              @click="mostrarModal = false"
            >
              Cancelar
            </button>

            <button
              type="button"
              class="btn-primary"
              @click="guardarAsignacion"
            >
              Guardar asignación
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const asignaciones = ref([])
const filtroBusqueda = ref('')
const asignacionSeleccionada = ref(null)

const mostrarModal = ref(false)

const formAsignacion = ref({
  solicitudTransporteId: '',
  conductorId: '',
  vehiculoId: '',
  usuarioAsignadorId: ''
})

const asignacionesFiltradas = computed(() => {
  let resultado = asignaciones.value

  if (filtroBusqueda.value) {
    resultado = resultado.filter(a =>
      a.solicitudTransporteId?.toString().includes(filtroBusqueda.value) ||
      a.conductorId?.toString().includes(filtroBusqueda.value) ||
      a.vehiculoId?.toString().includes(filtroBusqueda.value) ||
      a.usuarioAsignadorId?.toString().includes(filtroBusqueda.value)
    )
  }

  if (filtroEstado.value === 'activas') {
    resultado = resultado.filter(a => a.estado === 1)
  }

  if (filtroEstado.value === 'finalizadas') {
    resultado = resultado.filter(a => a.estado === 4)
  }

  if (filtroEstado.value === 'canceladas') {
    resultado = resultado.filter(a => a.estado === 3)
  }

  return resultado
})

const cargarAsignaciones = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch('https://localhost:7221/api/Asignaciones', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.ok) {
    asignaciones.value = await response.json()

    if (asignaciones.value.length > 0) {
      asignacionSeleccionada.value = asignaciones.value[0]
    }
  }
}

const totalAsignaciones = computed(() => asignaciones.value.length)

const asignacionesEnCurso = computed(() =>
  asignaciones.value.filter(a => a.estado === 1).length
)

const asignacionesCompletas = computed(() =>
  asignaciones.value.filter(a => a.estado === 4).length
)

const asignacionesCanceladas = computed(() =>
  asignaciones.value.filter(a => a.estado === 3).length
)

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

const guardarAsignacion = async () => {
  const token = localStorage.getItem('token_transporte')

  const payload = {
    solicitudTransporteId: Number(formAsignacion.value.solicitudTransporteId),
    conductorId: Number(formAsignacion.value.conductorId),
    vehiculoId: Number(formAsignacion.value.vehiculoId),
    usuarioAsignadorId: Number(formAsignacion.value.usuarioAsignadorId)
  }

  const response = await fetch('https://localhost:7221/api/Asignaciones', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(payload)
  })

  if (response.ok) {
    mostrarModal.value = false

    formAsignacion.value = {
      solicitudTransporteId: '',
      conductorId: '',
      vehiculoId: '',
      usuarioAsignadorId: ''
    }

    await cargarAsignaciones()
  } else {
    alert('No se pudo crear la asignación.')
  }
}

const filtroEstado = ref('todas')

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

.stats-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 18px;
  margin-bottom: 24px;
}

.stat-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 18px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.stat-card span {
  color: #6b7280;
  font-size: 0.9rem;
}

.stat-card strong {
  font-size: 2rem;
  color: #111827;
}

.btn-primary {
  background: #111827;
  color: white;
  border: none;
  border-radius: 14px;
  font-weight: 700;
  cursor: pointer;
}

.asignaciones-layout {
  display: grid;
  grid-template-columns: 3fr 1fr;
  gap: 20px;
}

.detalle-panel {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 18px;
}

.detalle-panel h3 {
  margin-top: 0;
  color: #111827;
  font-size: 1rem;
  font-weight: 800;
}

.detalle-content p {
  margin-bottom: 12px;
  color: #374151;
  font-size: 0.9rem;
}

.empty-text {
  color: #6b7280;
  font-size: 0.9rem;
}

.clickable-row {
  cursor: pointer;
}

.clickable-row:hover {
  background: #f9fafb;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal-container {
  width: 600px;
  max-width: 95%;
  background: white;
  border-radius: 18px;
  padding: 24px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.btn-close {
  border: none;
  background: transparent;
  font-size: 1.5rem;
  cursor: pointer;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px;
  margin-top: 20px;
}

.form-grid input {
  padding: 10px 12px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
}

.modal-actions {
  grid-column: 1 / -1;
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

.btn-cancel {
  padding: 10px 18px;
  border: none;
  border-radius: 10px;
  background: #e5e7eb;
  cursor: pointer;
}

.content-layout {
  display: grid;
  grid-template-columns: 3fr 1fr;
  gap: 24px;
}

.main-content {
  background: white;
  border-radius: 18px;
  padding: 20px;
  border: 1px solid #e5e7eb;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  margin-bottom: 20px;
}

.search-box {
  width: 320px;
  padding: 12px 16px;
  border: 1px solid #d1d5db;
  border-radius: 12px;
}

.tabs {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.tab {
  border: none;
  padding: 10px 16px;
  border-radius: 10px;
  cursor: pointer;
  background: #f3f4f6;
}

.tab.active {
  background: #111827;
  color: white;
}

.detalle-panel {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 20px;
}

.detail-card {
  background: #f9fafb;
  border-radius: 12px;
  padding: 12px;
  margin-bottom: 12px;
}

.detail-card span {
  display: block;
  color: #6b7280;
  font-size: .8rem;
}

.detail-card strong {
  color: #111827;
  font-size: 1rem;
}

.clickable-row:hover {
  background: #f9fafb;
}
</style>