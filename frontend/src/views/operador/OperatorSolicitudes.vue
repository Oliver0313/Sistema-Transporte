<template>
  <div class="section-header-mockup">
    <h2>Gestión de Solicitudes</h2>
    <p>Gestiona y administra las solicitudes</p>
  </div>

  <div class="filters-bar-mockup">
    <div class="search-filter-wrapper">
      <input type="text" placeholder="Buscar solicitudes..." class="filter-search-input" v-model="filtroBusqueda" />
      <img src="../../assets/icons/search.png" class="filter-search-icon" alt="Buscar" />
    </div>

    <div class="filter-group-item">
      <label>Estado</label>
      <select class="mockup-select" v-model="filtroEstado">
        <option value="">Todos los estados</option>
        <option value="1">Pendiente</option>
        <option value="2">Aprobada</option>
        <option value="3">Rechazada</option>
        <option value="4">Cancelada</option>
        <option value="5">Finalizada</option>
      </select>
    </div>

    <div class="filter-group-item">
      <label>Area solicitante</label>
      <select class="mockup-select" v-model="filtroArea">
        <option value="">Todas las áreas</option>
        <option v-for="area in areasDisponibles" :key="area" :value="area">{{ area }}</option>
      </select>
    </div>

    <div class="filter-group-item">
      <label>Fecha del viaje</label>
      <input type="date" class="mockup-date-input" v-model="filtroFecha" />
    </div>

    <button class="btn-filter-action" @click="limpiarFiltros">Limpiar Filtros</button>

    <button class="btn-new-solicitud-trigger" @click="mostrarFormulario = true">
      Nueva solicitud
    </button>
  </div>

  <div class="solicitudes-grid full-width-table">
    <section class="grid-left-table card-panel-mockup">
      <div class="table-responsive">
        <table class="custom-table-mockup">
          <thead>
            <tr>
              <th>Id</th>
              <th>Area solicitante</th>
              <th>Cantidad de colaboradores</th>
              <th>Hora de salida</th>
              <th>Hora de regreso</th>
              <th>Destino</th>
              <th>Motivo del viaje</th>
              <th>Estado</th>
              <th>Vehiculo asignado</th>
              <th>Conductor asignado</th>
              <th class="text-center" style="width: 100px;">Acciones</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="solicitud in solicitudesFiltradas" :key="solicitud.id">
              <td>{{ solicitud.id }}</td>
              <td>{{ solicitud.areaSolicitante }}</td>
              <td>{{ solicitud.cantidadColaboradores }}</td>
              <td>{{ formatearFechaVista(solicitud.fechaHoraSalida) }}</td>
              <td>{{ formatearFechaVista(solicitud.fechaHoraRegreso) }}</td>
              <td><strong>{{ solicitud.destino }}</strong></td>
              <td>{{ solicitud.motivo }}</td>
              <td>
                <span :class="['status-pill-mockup', obtenerClaseEstado(solicitud.estado)]">
                  {{ formatearEstadoVista(solicitud.estado) }}
                </span>
              </td>
              <td>{{ solicitud.vehiculoAsignado || '---' }}</td>
              <td>{{ solicitud.conductorAsignado || '---' }}</td>
              <td class="actions-cell-fixed">
                <div class="actions-wrapper">
                  <button class="action-btn-mockup icon-view" title="Ver detalle" @click="verDetalleSolicitud(solicitud)"></button>
                  <button class="action-btn-mockup icon-edit btn-disabled" disabled title="No permitido para operador"></button>
                  <button class="action-btn-mockup icon-delete btn-disabled" disabled title="No permitido para operador"></button>
                </div>
              </td>
            </tr>

            <tr v-if="solicitudesFiltradas.length === 0">
              <td colspan="11" class="text-center text-muted" style="padding: 30px;">
                No se encontraron solicitudes con los filtros aplicados.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  </div>

  <div v-if="mostrarFormulario" class="modal-overlay-mockup" @click.self="mostrarFormulario = false">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>Registrar Nueva Solicitud</h3>
        <button class="btn-close-modal" @click="mostrarFormulario = false">×</button>
      </div>

      <form @submit.prevent="crearSolicitud" class="form-solicitud-mockup">
        <div class="form-group-mockup">
          <label>Área Solicitante</label>
          <input type="text" v-model="formModel.areaSolicitante" placeholder="Ej. RRHH" required />
        </div>

        <div class="form-group-mockup">
          <label>Cantidad de Colaboradores</label>
          <input type="number" v-model.number="formModel.cantidadColaboradores" min="1" required />
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Fecha y Hora Salida</label>
            <input type="datetime-local" v-model="formModel.fechaHoraSalida" required />
          </div>

          <div class="form-group-mockup">
            <label>Fecha y Hora Regreso</label>
            <input type="datetime-local" v-model="formModel.fechaHoraRegreso" required />
          </div>
        </div>

        <div class="form-group-mockup">
          <label>Destino</label>
          <input type="text" v-model="formModel.destino" placeholder="Ej. Bonao" required />
        </div>

        <div class="form-group-mockup">
          <label>Motivo del Viaje</label>
          <textarea v-model="formModel.motivo" placeholder="Ej. Capacitacion" rows="3" required></textarea>
        </div>

        <div class="form-actions-central">
          <button type="button" class="btn-cancel-mockup" @click="mostrarFormulario = false">Cancelar</button>
          <button type="submit" class="btn-submit-mockup" :disabled="guardando">
            {{ guardando ? 'Enviando a la API...' : 'Registrar Solicitud' }}
          </button>
        </div>
      </form>
    </div>
  </div>

  <div v-if="mostrarDetalle" class="modal-overlay-mockup" @click.self="mostrarDetalle = false">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>Detalle de Solicitud #{{ solicitudSeleccionada.id }}</h3>
        <button class="btn-close-modal" @click="mostrarDetalle = false">×</button>
      </div>

      <div class="detalle-solicitud-wrapper">
        <div class="detalle-item">
          <strong>Área Solicitante:</strong>
          <p>{{ solicitudSeleccionada.areaSolicitante }}</p>
        </div>

        <div class="detalle-item">
          <strong>Cantidad de Colaboradores:</strong>
          <p>{{ solicitudSeleccionada.cantidadColaboradores }} personas</p>
        </div>

        <div class="form-row-mockup">
          <div class="detalle-item">
            <strong>Fecha/Hora Salida:</strong>
            <p>{{ formatearFechaVista(solicitudSeleccionada.fechaHoraSalida) }}</p>
          </div>

          <div class="detalle-item">
            <strong>Fecha/Hora Regreso:</strong>
            <p>{{ formatearFechaVista(solicitudSeleccionada.fechaHoraRegreso) }}</p>
          </div>
        </div>

        <div class="detalle-item">
          <strong>Destino:</strong>
          <p>{{ solicitudSeleccionada.destino }}</p>
        </div>

        <div class="detalle-item">
          <strong>Motivo del Viaje:</strong>
          <p class="detalle-motivo-text">{{ solicitudSeleccionada.motivo }}</p>
        </div>

        <div class="form-row-mockup" style="margin-top: 10px; border-top: 1px solid #e5e7eb; padding-top: 12px;">
          <div class="detalle-item">
            <strong>Vehículo Asignado:</strong>
            <p>{{ solicitudSeleccionada.vehiculoAsignado || '---' }}</p>
          </div>

          <div class="detalle-item">
            <strong>Conductor Asignado:</strong>
            <p>{{ solicitudSeleccionada.conductorAsignado || '---' }}</p>
          </div>
        </div>
      </div>

      <div class="form-actions-central" style="margin-top: 20px;">
        <button type="button" class="btn-submit-mockup" @click="mostrarDetalle = false">Entendido</button>
      </div>
    </div>
  </div>

  <div v-if="mensajeErrorFlotante" class="toast-error-moderno">
    <div class="toast-content">
      <span class="toast-title">Error en la solicitud</span>
      <p class="toast-text">{{ mensajeErrorFlotante }}</p>
    </div>
    <button class="btn-close-toast" @click="mensajeErrorFlotante = ''">×</button>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const filtroBusqueda = ref('')
const filtroEstado = ref('')
const filtroArea = ref('')
const filtroFecha = ref('')

const mostrarFormulario = ref(false)
const guardando = ref(false)
const solicitudes = ref([])

const mensajeErrorFlotante = ref('')

const formModel = ref({
  areaSolicitante: '',
  cantidadColaboradores: 1,
  fechaHoraSalida: '',
  fechaHoraRegreso: '',
  destino: '',
  motivo: ''
})

const mostrarDetalle = ref(false)
const solicitudSeleccionada = ref({})

const verDetalleSolicitud = (solicitud) => {
  solicitudSeleccionada.value = { ...solicitud }
  mostrarDetalle.value = true
}

const fetchSolicitudesDeAPI = async () => {
  const token = localStorage.getItem('token_transporte')

  try {
    const response = await fetch('https://localhost:7221/api/solicitudestransporte', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (response.ok) {
      solicitudes.value = await response.json()
    }
  } catch (error) {
    console.error('Error cargando solicitudes de la base de datos:', error)
  }
}

const areasDisponibles = computed(() => {
  const areas = solicitudes.value.map(s => s.areaSolicitante).filter(Boolean)
  return [...new Set(areas)]
})

const solicitudesFiltradas = computed(() => {
  return solicitudes.value.filter(solicitud => {
    const cumpleBusqueda =
      !filtroBusqueda.value ||
      solicitud.destino?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
      solicitud.motivo?.toLowerCase().includes(filtroBusqueda.value.toLowerCase())

    const cumpleEstado = !filtroEstado.value || solicitud.estado == filtroEstado.value
    const cumpleArea = !filtroArea.value || solicitud.areaSolicitante === filtroArea.value
    const cumpleFecha = !filtroFecha.value || solicitud.fechaHoraSalida?.includes(filtroFecha.value)

    return cumpleBusqueda && cumpleEstado && cumpleArea && cumpleFecha
  })
})

const limpiarFiltros = () => {
  filtroBusqueda.value = ''
  filtroEstado.value = ''
  filtroArea.value = ''
  filtroFecha.value = ''
}

const formatearFechaVista = (fechaIso) => {
  if (!fechaIso) return '---'

  const fecha = new Date(fechaIso)

  return fecha.toLocaleString('es-DO', {
    hour12: true,
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatearEstadoVista = (estado) => {
  const mapeoEstados = {
    1: 'Pendiente',
    2: 'Aprobada',
    3: 'Rechazada',
    4: 'Cancelada',
    5: 'Finalizada'
  }

  return mapeoEstados[estado] || estado || 'Pendiente'
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

onMounted(() => {
  fetchSolicitudesDeAPI()
})

const crearSolicitud = async () => {
  const salida = new Date(formModel.value.fechaHoraSalida)
  const regreso = new Date(formModel.value.fechaHoraRegreso)

  if (salida.getTime() === regreso.getTime()) {
    mensajeErrorFlotante.value = 'La fecha y hora de salida no puede ser igual a la de regreso.'
    setTimeout(() => { mensajeErrorFlotante.value = '' }, 4000)
    return
  }

  if (regreso < salida) {
    mensajeErrorFlotante.value = 'La fecha de regreso no puede ser anterior a la fecha de salida.'
    setTimeout(() => { mensajeErrorFlotante.value = '' }, 4000)
    return
  }

  guardando.value = true

  const token = localStorage.getItem('token_transporte')
  const idUsuarioLogueado = parseInt(localStorage.getItem('usuario_id')) || 1

  const payload = {
    areaSolicitante: formModel.value.areaSolicitante,
    cantidadColaboradores: parseInt(formModel.value.cantidadColaboradores),
    fechaHoraSalida: formModel.value.fechaHoraSalida ? salida.toISOString() : null,
    fechaHoraRegreso: formModel.value.fechaHoraRegreso ? regreso.toISOString() : null,
    destino: formModel.value.destino,
    motivo: formModel.value.motivo,
    usuarioSolicitanteId: idUsuarioLogueado
  }

  try {
    const response = await fetch('https://localhost:7221/api/solicitudestransporte', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    })

    if (response.ok) {
      await fetchSolicitudesDeAPI()

      formModel.value = {
        areaSolicitante: '',
        cantidadColaboradores: 1,
        fechaHoraSalida: '',
        fechaHoraRegreso: '',
        destino: '',
        motivo: ''
      }

      mostrarFormulario.value = false
    } else {
      const errorResponse = await response.json()
      console.error('Error del Servidor:', errorResponse)
      alert('Error en validación de datos: ' + JSON.stringify(errorResponse.errors || errorResponse))
    }
  } catch (error) {
    console.error('Error al conectar con la API:', error)
  } finally {
    guardando.value = false
  }
}
</script>

<style>
@import '/src/assets/styles/variables.css';
@import '/src/assets/styles/reset.css';

/* Header de la página */
.section-header-mockup {
  margin-bottom: 20px;
}

.section-header-mockup h2 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
}

.section-header-mockup p {
  color: #6b7280;
  font-size: 0.9rem;
}

/* Filtros */
.filters-bar-mockup {
  display: flex;
  gap: 16px;
  align-items: end;
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  margin-bottom: 24px;
  border: 1px solid #e5e7eb;
}

.search-filter-wrapper {
  position: relative;
  flex: 1;
}

.filter-search-input {
  width: 100%;
  padding: 10px 14px 10px 40px;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
}

.filter-search-icon {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  width: 16px;
  height: 16px;
}

.filter-group-item {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.mockup-select,
.mockup-date-input {
  padding: 10px;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
}

/* Botones */
.btn-filter-action,
.btn-new-solicitud-trigger,
.btn-submit-mockup {
  background: #111827;
  color: white;
  border: none;
  padding: 10px 18px;
  border-radius: 8px;
  cursor: pointer;
}

.btn-cancel-mockup {
  background: #e5e7eb;
  border: none;
  padding: 10px 18px;
  border-radius: 8px;
  cursor: pointer;
}

/* Tabla */
.card-panel-mockup {
  background: white;
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  overflow: hidden;
}

.table-responsive {
  overflow-x: auto;
}

.custom-table-mockup {
  width: 100%;
  border-collapse: collapse;
}

.custom-table-mockup th {
  background: #f9fafb;
  padding: 14px;
  text-align: left;
  font-size: 0.85rem;
}

.custom-table-mockup td {
  padding: 14px;
  border-top: 1px solid #e5e7eb;
}

/* Estados */
.status-pill-mockup {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
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

.finalizada {
  background: #dbeafe;
  color: #1e40af;
}

/* Acciones */
.actions-wrapper {
  display: flex;
  gap: 8px;
  justify-content: center;
}

.action-btn-mockup {
  width: 30px;
  height: 30px;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  background-color: white;
  cursor: pointer;
}

/* Modal */
.modal-overlay-mockup {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.45);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-container-central {
  background: white;
  width: 650px;
  max-width: 95%;
  border-radius: 12px;
  padding: 24px;
}

.form-panel-header-central {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 18px;
}

.form-solicitud-mockup {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.form-group-mockup {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.form-group-mockup input,
.form-group-mockup textarea {
  padding: 10px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
}

.form-row-mockup {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
}

.form-actions-central {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

/* Toast */
.toast-error-moderno {
  position: fixed;
  top: 20px;
  right: 20px;
  background: white;
  border-left: 4px solid red;
  padding: 16px;
  border-radius: 10px;
  box-shadow: 0 10px 20px rgba(0,0,0,.1);
}
</style>