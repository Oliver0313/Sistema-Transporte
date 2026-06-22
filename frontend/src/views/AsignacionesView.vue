<template>
  <div>
    <div class="section-header">
      <h2>Asignaciones</h2>
      <p>Gestión y consulta de asignaciones de transporte</p>
    </div>

      <div class="stats-grid">
      <div class="stat-card">
        <div class="stat-icon">📊</div>
        <div class="stat-info">
          <span>Total</span>
          <strong>{{ totalAsignaciones }}</strong>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon">✅</div>
        <div class="stat-info">
          <span>Activas</span>
          <strong>{{ asignacionesActivas }}</strong>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon">🚐</div>
        <div class="stat-info">
          <span>En curso</span>
          <strong>{{ asignacionesEnCurso }}</strong>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon">🏁</div>
        <div class="stat-info">
          <span>Completas</span>
          <strong>{{ asignacionesCompletas }}</strong>
        </div>
      </div>

      <div class="stat-card">
        <div class="stat-icon">❌</div>
        <div class="stat-info">
          <span>Canceladas</span>
          <strong>{{ asignacionesCanceladas }}</strong>
        </div>
      </div>
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

          <button
            class="btn-primary"
            @click="abrirModal"
          >
            + Nueva Asignación
          </button>
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
                <td>{{ obtenerSolicitud(asignacion.solicitudTransporteId) }}</td>
                <td>{{ obtenerConductor(asignacion.conductorId) }}</td>
                <td>{{ obtenerVehiculo(asignacion.vehiculoId) }}</td>
                <td>{{ formatearFecha(asignacion.fechaHoraAsignacion) }}</td>
                <td>#{{ asignacion.usuarioAsignadorId }}</td>

                <td>
                  <span :class="['estado-badge', obtenerClaseEstado(asignacion.estado)]">
                    {{ formatearEstado(asignacion.estado) }}
                  </span>
                </td>
              </tr>

              <tr v-if="asignacionesFiltradas.length === 0">
                <td colspan="6" class="empty-table">
                  No se encontraron asignaciones.
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
            <strong>{{ obtenerConductor(asignacionSeleccionada.conductorId) }}</strong>
          </div>

          <div class="detail-card">
            <span>Vehículo</span>
            <strong>{{ obtenerVehiculo(asignacionSeleccionada.vehiculoId) }}</strong>
          </div>

          <div class="detail-card">
            <span>Usuario asignador</span>
            <strong>{{ obtenerSolicitud(asignacionSeleccionada.solicitudTransporteId) }}</strong>
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
                  <select v-model.number="formAsignacion.solicitudTransporteId">
          <option value="">Seleccione una solicitud</option>
          <option
            v-for="solicitud in solicitudes.filter(s => s.estado === 2)"
            :key="solicitud.id"
            :value="solicitud.id"
          >
            #{{ solicitud.id }} - {{ solicitud.areaSolicitante }} - {{ solicitud.destino }}
          </option>
        </select>

        <select v-model.number="formAsignacion.conductorId">
          <option value="">Seleccione un conductor</option>
          <option
            v-for="conductor in conductores.filter(c => c.estado === 1)"
            :key="conductor.id"
            :value="conductor.id"
          >
            #{{ conductor.id }} - {{ conductor.nombre }} {{ conductor.apellido }}
          </option>
        </select>

        <select v-model.number="formAsignacion.vehiculoId">
          <option value="">Seleccione un vehículo</option>
          <option
            v-for="vehiculo in vehiculos.filter(v => v.estado === 1)"
            :key="vehiculo.id"
            :value="vehiculo.id"
          >
            #{{ vehiculo.id }} - {{ vehiculo.marca }} {{ vehiculo.modelo }} - {{ vehiculo.matricula }}
          </option>
        </select>

          <input
            v-model.number="formAsignacion.usuarioAsignadorId"
            type="number"
            placeholder="ID Usuario asignador"
          />

          <p v-if="errorModal" class="modal-error">
            {{ errorModal }}
          </p>

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
const solicitudes = ref([])
const conductores = ref([])
const vehiculos = ref([])

const filtroBusqueda = ref('')
const filtroEstado = ref('todas')
const asignacionSeleccionada = ref(null)

const mostrarModal = ref(false)
const errorModal = ref('')

const formAsignacion = ref({
  solicitudTransporteId: '',
  conductorId: '',
  vehiculoId: '',
  usuarioAsignadorId: ''
})

const asignacionesFiltradas = computed(() => {
  const texto = filtroBusqueda.value.toLowerCase().trim()

  let resultado = asignaciones.value

  if (texto) {
    resultado = resultado.filter(a => {
      const solicitud = obtenerSolicitud(a.solicitudTransporteId).toLowerCase()
      const conductor = obtenerConductor(a.conductorId).toLowerCase()
      const vehiculo = obtenerVehiculo(a.vehiculoId).toLowerCase()
      const estado = formatearEstado(a.estado).toLowerCase()
      const fecha = formatearFecha(a.fechaHoraAsignacion).toLowerCase()

      return (
        solicitud.includes(texto) ||
        conductor.includes(texto) ||
        vehiculo.includes(texto) ||
        estado.includes(texto) ||
        fecha.includes(texto) ||
        a.id?.toString().includes(texto)
      )
    })
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

const cargarDatosFormulario = async () => {
  const token = localStorage.getItem('token_transporte')

  const [solicitudesRes, conductoresRes, vehiculosRes] = await Promise.all([
    fetch('https://localhost:7221/api/solicitudestransporte', {
      headers: { Authorization: `Bearer ${token}` }
    }),
    fetch('https://localhost:7221/api/Conductores', {
      headers: { Authorization: `Bearer ${token}` }
    }),
    fetch('https://localhost:7221/api/Vehiculos', {
      headers: { Authorization: `Bearer ${token}` }
    })
  ])

  if (solicitudesRes.ok) {
    solicitudes.value = await solicitudesRes.json()
  }

  if (conductoresRes.ok) {
    conductores.value = await conductoresRes.json()
  }

  if (vehiculosRes.ok) {
    vehiculos.value = await vehiculosRes.json()
  }
}

const abrirModal = () => {
  errorModal.value = ''
  mostrarModal.value = true
  cargarDatosFormulario()
}

const guardarAsignacion = async () => {
  errorModal.value = ''

  if (
    !formAsignacion.value.solicitudTransporteId ||
    !formAsignacion.value.conductorId ||
    !formAsignacion.value.vehiculoId ||
    !formAsignacion.value.usuarioAsignadorId
  ) {
    errorModal.value = 'Completa todos los campos antes de guardar.'
    return
  }

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
    const textoError = await response.text()
    errorModal.value = textoError || 'No se pudo crear la asignación.'
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

const obtenerConductor = (id) => {
  const conductor = conductores.value.find(c => c.id === id)

  if (!conductor) return `#${id}`

  return `${conductor.nombre} ${conductor.apellido}`
}

const obtenerVehiculo = (id) => {
  const vehiculo = vehiculos.value.find(v => v.id === id)

  if (!vehiculo) return `#${id}`

  return `${vehiculo.marca} ${vehiculo.modelo} - ${vehiculo.matricula}`
}

const asignacionesActivas = computed(() =>
  asignaciones.value.filter(a => a.estado === 1).length
)

const obtenerSolicitud = (id) => {
  const solicitud = solicitudes.value.find(s => s.id === id)

  if (!solicitud) return `#${id}`

  return `${solicitud.areaSolicitante} - ${solicitud.destino}`
}

onMounted(() => {
  cargarAsignaciones()
  cargarDatosFormulario()
})
</script>

<style scoped>
.section-header {
  margin-bottom: 26px;
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
  font-size: .85rem;
}

.asignaciones-table td {
  padding: 14px;
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
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 16px;
  display: flex;
  align-items: center;
  gap: 14px;
  box-shadow: 0 1px 2px rgba(0,0,0,.04);
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
  border: 1px solid #111827;
  padding: 10px 18px;
  border-radius: 12px;
  cursor: pointer;
  font-weight: 600;
}

.btn-primary:hover {
  background: #374151;
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
  align-items: center;
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

.form-grid select {
  padding: 10px 12px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
}

.modal-error {
  grid-column: 1 / -1;
  color: #991b1b;
  background: #fee2e2;
  padding: 10px 12px;
  border-radius: 10px;
  font-size: 0.9rem;
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

.stat-info {
  display: flex;
  flex-direction: column;
}

.stat-info span {
  font-size: 0.85rem;
  color: #4b5563;
  font-weight: 500;
}

.stat-info strong {
  font-size: 1.4rem;
  font-weight: 700;
  color: #111827;
}
</style>