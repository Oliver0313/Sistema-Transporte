<template>
  <div class="section-header-mockup">
    <h2>Gestión de Solicitudes</h2>
    <p>Gestiona y administra las solicitudes</p>
  </div>

  <div class="filters-bar-mockup">
    <div class="search-filter-wrapper">
      <input type="text" placeholder="Buscar solicitudes..." class="filter-search-input" v-model="filtroBusqueda" />
      <img src="../assets/icons/search.png" class="filter-search-icon" alt="Buscar" />
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

    <button
      class="btn-new-solicitud-trigger"
      :class="{ 'btn-disabled': !puedeCrear }"
      :disabled="!puedeCrear"
      :title="!puedeCrear ? 'No permitido para tu rol' : ''"
      @click="abrirFormularioNuevo"
    >
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
              <td>{{ solicitud.vehiculo ? `${solicitud.vehiculo.marca} ${solicitud.vehiculo.modelo}` : '---' }}</td>
              <td>{{ solicitud.conductor ? `${solicitud.conductor.nombre} ${solicitud.conductor.apellido}` : '---' }}</td>
              <td class="actions-cell-fixed">
                <div class="actions-wrapper">
                  <button
                    class="action-btn-mockup icon-view"
                    title="Ver detalle"
                    @click="verDetalleSolicitud(solicitud)"
                  ></button>
                  <button
                    class="action-btn-mockup icon-edit"
                    :class="{ 'btn-disabled': !puedeEditar }"
                    :disabled="!puedeEditar"
                    :title="puedeEditar ? 'Editar / Aprobar' : 'No permitido para tu rol'"
                    @click="puedeEditar && abrirFormularioEdicion(solicitud)"
                  ></button>
                  <button
                    class="action-btn-mockup icon-delete"
                    :class="{ 'btn-disabled': !puedeEliminar }"
                    :disabled="!puedeEliminar"
                    :title="puedeEliminar ? 'Eliminar' : 'No permitido para tu rol'"
                    @click="puedeEliminar && eliminarSolicitud(solicitud)"
                  ></button>
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

  <div v-if="mostrarFormulario" class="modal-overlay-mockup" @click.self="cerrarFormulario">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>{{ modoEdicion ? 'Gestionar Solicitud #' + solicitudEditando?.id : 'Registrar Nueva Solicitud' }}</h3>
        <button class="btn-close-modal" @click="cerrarFormulario">×</button>
      </div>

      <form @submit.prevent="modoEdicion ? guardarEdicion() : crearSolicitud()" class="form-solicitud-mockup">
        
        <div class="form-group-mockup">
          <label>Área Solicitante</label>
          <input type="text" v-model="formModel.areaSolicitante" placeholder="Ej. RRHH" required :disabled="modoEdicion && !esAdmin" />
        </div>

        <div class="form-group-mockup">
          <label>Cantidad de Colaboradores</label>
          <input type="number" v-model.number="formModel.cantidadColaboradores" min="1" required :disabled="modoEdicion && !esAdmin" />
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Fecha y Hora Salida</label>
            <input type="datetime-local" v-model="formModel.fechaHoraSalida" required :disabled="modoEdicion && !esAdmin" />
          </div>
          <div class="form-group-mockup">
            <label>Fecha y Hora Regreso</label>
            <input type="datetime-local" v-model="formModel.fechaHoraRegreso" required :disabled="modoEdicion && !esAdmin" />
          </div>
        </div>

        <div class="form-group-mockup">
          <label>Destino</label>
          <input type="text" v-model="formModel.destino" placeholder="Ej. Bonao" required :disabled="modoEdicion && !esAdmin" />
        </div>

        <div class="form-group-mockup">
          <label>Motivo del Viaje</label>
          <textarea v-model="formModel.motivo" placeholder="Ej. Capacitacion" rows="3" required :disabled="modoEdicion && !esAdmin"></textarea>
        </div>

        <div class="form-row-mockup" v-if="modoEdicion">
          <div class="form-group-mockup">
            <label>Estado</label>
            <select v-model.number="formModel.estado" class="mockup-select" required>
              <option :value="1">Pendiente</option>
              <option :value="2">Aprobada</option>
              <option :value="3">Rechazada</option>
              <option :value="4">Cancelada</option>
              <option :value="5">Finalizada</option>
            </select>
          </div>

          <div class="form-group-mockup">
            <label>Vehículo Asignado</label>
            <select v-model="formModel.vehiculoId" class="mockup-select">
              <option :value="null">Sin asignar</option>
              <option v-for="v in vehiculos" :key="v.id" :value="v.id">
                {{ v.marca }} {{ v.modelo }} - {{ v.matricula }}
              </option>
            </select>
          </div>
        </div>

        <div class="form-group-mockup" v-if="modoEdicion">
          <label>Conductor Asignado</label>
          <select v-model="formModel.conductorId" class="mockup-select">
            <option :value="null">Sin asignar</option>
            <option v-for="c in conductores" :key="c.id" :value="c.id">
              {{ c.nombre }} {{ c.apellido }}
            </option>
          </select>
        </div>

        <div class="form-actions-central">
          <button type="button" class="btn-cancel-mockup" @click="cerrarFormulario">Cancelar</button>
          <button type="submit" class="btn-submit-mockup" :disabled="guardando">
            {{ guardando ? 'Guardando...' : 'Guardar Registro' }}
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
      <span class="toast-title">Notificación</span>
      <p class="toast-text">{{ mensajeErrorFlotante }}</p>
    </div>
    <button class="btn-close-toast" @click="mensajeErrorFlotante = ''">×</button>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, nextTick } from 'vue'

const rolUsuario = ref(localStorage.getItem('usuario_rol') || 'Operador')

const esAdmin = computed(() => {
  const rol = rolUsuario.value.toLowerCase()
  return rol === 'administrador' || rol === 'admin' || rol === 'superadmin'
})

const esSupervisor = computed(() => {
  return rolUsuario.value.toLowerCase() === 'supervisor'
})

const puedeCrear    = computed(() => esAdmin.value)
const puedeEditar   = computed(() => esAdmin.value || esSupervisor.value)
const puedeEliminar = computed(() => esAdmin.value)

const filtroBusqueda = ref('')
const filtroEstado   = ref('')
const filtroArea     = ref('')
const filtroFecha    = ref('')

const mostrarFormulario = ref(false)
const modoEdicion       = ref(false)
const guardando         = ref(false)
const solicitudes       = ref([])
const vehiculos         = ref([])
const conductores       = ref([])

const mensajeErrorFlotante = ref('')
const solicitudEditando    = ref(null)

const formModel = ref({
  areaSolicitante:       '',
  cantidadColaboradores: 1,
  fechaHoraSalida:       '',
  fechaHoraRegreso:       '',
  destino:               '',
  motivo:                '',
  estado:                1,
  vehiculoId:            null,
  conductorId:           null
})

const mostrarDetalle       = ref(false)
const solicitudSeleccionada = ref({})

const formModelVacio = () => ({
  areaSolicitante:       '',
  cantidadColaboradores: 1,
  fechaHoraSalida:       '',
  fechaHoraRegreso:       '',
  destino:               '',
  motivo:                '',
  estado:                1,
  vehiculoId:            null,
  conductorId:           null
})

const verDetalleSolicitud = (solicitud) => {
  solicitudSeleccionada.value = { ...solicitud }
  mostrarDetalle.value        = true
}

const abrirFormularioNuevo = () => {
  modoEdicion.value = false
  solicitudEditando.value = null
  formModel.value = formModelVacio()
  mostrarFormulario.value = true
}

const abrirFormularioEdicion = async (solicitud) => {
  solicitudEditando.value = solicitud
  modoEdicion.value = true
  mostrarFormulario.value = true

  await nextTick()

  formModel.value = {
    areaSolicitante:       solicitud.areaSolicitante,
    cantidadColaboradores: Number(solicitud.cantidadColaboradores),
    fechaHoraSalida:       solicitud.fechaHoraSalida ? solicitud.fechaHoraSalida.substring(0, 16) : '',
    fechaHoraRegreso:       solicitud.fechaHoraRegreso ? solicitud.fechaHoraRegreso.substring(0, 16) : '',
    destino:               solicitud.destino,
    motivo:                solicitud.motivo,
    estado:                Number(solicitud.estado),
    vehiculoId:            solicitud.vehiculoId ? Number(solicitud.vehiculoId) : null,
    conductorId:           solicitud.conductorId ? Number(solicitud.conductorId) : null
  }
}

const cerrarFormulario = () => {
  mostrarFormulario.value = false
  modoEdicion.value       = false
  solicitudEditando.value = null
  formModel.value         = formModelVacio()
}

// 📌 FUNCIÓN DE ELIMINACIÓN REQUERIDA
const eliminarSolicitud = (solicitud) => {
  mensajeErrorFlotante.value = 'No se pudo eliminar la solicitud.'
  setTimeout(() => { mensajeErrorFlotante.value = '' }, 5000)
}

const guardarEdicion = async () => {
  if (!solicitudEditando.value) return
  guardando.value = true
  const token = localStorage.getItem('token_transporte')
  
  try {
    const vId = formModel.value.vehiculoId !== "" && formModel.value.vehiculoId !== null 
                ? Number(formModel.value.vehiculoId) 
                : null

    const cId = formModel.value.conductorId !== "" && formModel.value.conductorId !== null 
                ? Number(formModel.value.conductorId) 
                : null

    const payload = {
      id:                    Number(solicitudEditando.value.id),
      areaSolicitante:       formModel.value.areaSolicitante,
      cantidadColaboradores: Number(formModel.value.cantidadColaboradores),
      fechaHoraSalida:       new Date(formModel.value.fechaHoraSalida).toISOString(),
      fechaHoraRegreso:       new Date(formModel.value.fechaHoraRegreso).toISOString(),
      destino:               formModel.value.destino,
      motivo:                formModel.value.motivo,
      estado:                Number(formModel.value.estado),
      vehiculoId:            vId,
      conductorId:           cId,
      usuarioSolicitanteId:  solicitudEditando.value.usuarioSolicitanteId || 1
    }

    const res = await fetch(`https://localhost:7221/api/solicitudestransporte/${solicitudEditando.value.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify(payload)
    })
    
    if (res.ok) {
      await fetchSolicitudesDeAPI()
      cerrarFormulario()
    } else {
      const err = await res.json()
      mensajeErrorFlotante.value = 'No se pudo guardar la solicitud.'
      setTimeout(() => { mensajeErrorFlotante.value = '' }, 5000)
    }
  } catch (error) {
    console.error(error)
    mensajeErrorFlotante.value = 'No se pudo conectar con el servidor.'
    setTimeout(() => { mensajeErrorFlotante.value = '' }, 4000)
  } finally {
    guardando.value = false
  }
}

const fetchSolicitudesDeAPI = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const response = await fetch('https://localhost:7221/api/solicitudestransporte', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (response.ok) {
      const datosClonados = await response.json()
      // Mapeado local para cruzar los datos mientras el backend no use Includes
      solicitudes.value = datosClonados.map(solicitud => {
        const vEncontrado = vehiculos.value.find(v => v.id === solicitud.vehiculoId)
        const cEncontrado = conductores.value.find(c => c.id === solicitud.conductorId)
        return {
          ...solicitud,
          vehiculo: solicitud.vehiculo || vEncontrado || null,
          conductor: solicitud.conductor || cEncontrado || null
        }
      })
    }
  } catch (error) { console.error('Error cargando solicitudes:', error) }
}

const fetchVehiculos = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const res = await fetch('https://localhost:7221/api/Vehiculos', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (res.ok) {
      vehiculos.value = await res.json()
      await fetchSolicitudesDeAPI() // Refresca relaciones
    }
  } catch (error) { console.error('Error cargando vehículos:', error) }
}

const fetchConductores = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const res = await fetch('https://localhost:7221/api/Conductores', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (res.ok) {
      conductores.value = await res.json()
      await fetchSolicitudesDeAPI() // Refresca relaciones
    }
  } catch (error) { console.error('Error cargando conductores:', error) }
}

const areasDisponibles = computed(() => {
  const areas = solicitudes.value.map(s => s.areaSolicitante).filter(Boolean)
  return [...new Set(areas)]
})

const solicitudesFiltradas = computed(() =>
  solicitudes.value.filter(solicitud => {
    const cumpleBusqueda =
      !filtroBusqueda.value ||
      solicitud.destino?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
      solicitud.motivo?.toLowerCase().includes(filtroBusqueda.value.toLowerCase())
    const cumpleEstado = !filtroEstado.value || solicitud.estado == filtroEstado.value
    const cumpleArea   = !filtroArea.value   || solicitud.areaSolicitante === filtroArea.value
    const cumpleFecha  = !filtroFecha.value  || solicitud.fechaHoraSalida?.includes(filtroFecha.value)
    return cumpleBusqueda && cumpleEstado && cumpleArea && cumpleFecha
  })
)

const limpiarFiltros = () => {
  filtroBusqueda.value = ''
  filtroEstado.value   = ''
  filtroArea.value     = ''
  filtroFecha.value    = ''
}

const formatearFechaVista = (fechaIso) => {
  if (!fechaIso) return '---'
  return new Date(fechaIso).toLocaleString('es-DO', {
    hour12: true, year: 'numeric', month: '2-digit',
    day: '2-digit', hour: '2-digit', minute: '2-digit'
  })
}

const formatearEstadoVista = (estado) => {
  return { 1: 'Pendiente', 2: 'Aprobada', 3: 'Rechazada', 4: 'Cancelada', 5: 'Finalizada' }[estado] || 'Pendiente'
}

const obtenerClaseEstado = (estado) => {
  return { 1: 'pendiente', 2: 'aprobada', 3: 'rechazada', 4: 'cancelada', 5: 'finalizada' }[estado] || 'pendiente'
}

const crearSolicitud = async () => {
  const salida  = new Date(formModel.value.fechaHoraSalida)
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
  const token              = localStorage.getItem('token_transporte')
  const idUsuarioLogueado  = parseInt(localStorage.getItem('usuario_id')) || 1

  const payload = {
    areaSolicitante:       formModel.value.areaSolicitante,
    cantidadColaboradores: parseInt(formModel.value.cantidadColaboradores), 
    fechaHoraSalida:       salida.toISOString(),
    fechaHoraRegreso:       regreso.toISOString(),
    destino:               formModel.value.destino,
    motivo:                formModel.value.motivo,
    usuarioSolicitanteId:   idUsuarioLogueado
  }

  try {
    const response = await fetch('https://localhost:7221/api/solicitudestransporte', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify(payload)
    })

    if (response.ok) {
      await fetchSolicitudesDeAPI()
      cerrarFormulario()
    } else {
      const errorResponse = await response.json()
      mensajeErrorFlotante.value = 'Error: ' + JSON.stringify(errorResponse.errors || errorResponse)
      setTimeout(() => { mensajeErrorFlotante.value = '' }, 4000)
    }
  } catch (error) {
    console.error('Error al conectar con la API:', error)
  } finally {
    guardando.value = false
  }
}

onMounted(() => {
  fetchVehiculos()
  fetchConductores()
})
</script>

<style>
/* Estilos se mantienen idénticos */
.section-header-mockup { margin-bottom: 26px; }
.section-header-mockup h2 { font-size: 1.8rem; font-weight: 800; color: #111827; margin: 0; }
.section-header-mockup p  { color: #6b7280; font-size: 0.95rem; margin-top: 6px; }

.filters-bar-mockup {
  display: flex;
  gap: 16px;
  align-items: end;
  background: #ffffff;
  padding: 20px;
  border-radius: 18px;
  margin-bottom: 24px;
  border: 1px solid #e5e7eb;
}

.search-filter-wrapper { position: relative; flex: 1; }

.filter-search-input {
  width: 100%;
  padding: 10px 14px 10px 40px;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
  font-size: 0.9rem;
  outline: none;
}

.filter-search-input:focus { border-color: #9ca3af; }

.filter-search-icon {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  width: 16px;
  height: 16px;
  opacity: 0.45;
}

.filter-group-item { display: flex; flex-direction: column; gap: 6px; }
.filter-group-item label { font-size: 0.85rem; font-weight: 700; color: #374151; }

.mockup-select,
.mockup-date-input {
  padding: 10px 12px;
  border-radius: 10px;
  border: 1px solid #e5e7eb;
  font-size: 0.9rem;
  background: #ffffff;
  outline: none;
}

.mockup-select:focus,
.mockup-date-input:focus { border-color: #9ca3af; }

.btn-filter-action,
.btn-new-solicitud-trigger,
.btn-submit-mockup {
  background: #111827;
  color: white;
  border: none;
  padding: 10px 18px;
  border-radius: 10px;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 700;
}

.btn-filter-action:hover,
.btn-new-solicitud-trigger:hover:not(:disabled),
.btn-submit-mockup:hover { opacity: 0.92; }

.btn-new-solicitud-trigger { margin-left: auto; }

.btn-new-solicitud-trigger.btn-disabled {
  opacity: 0.35;
  cursor: not-allowed;
}

.btn-cancel-mockup {
  background: #e5e7eb;
  color: #374151;
  border: none;
  padding: 10px 18px;
  border-radius: 10px;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 700;
}

.card-panel-mockup { background: white; border-radius: 18px; border: 1px solid #e5e7eb; overflow: hidden; }
.table-responsive   { overflow-x: auto; }
.custom-table-mockup { width: 100%; border-collapse: collapse; }

.custom-table-mockup th {
  background: #f9fafb;
  padding: 14px;
  text-align: left;
  font-size: 0.85rem;
  font-weight: 700;
  color: #374151;
  white-space: nowrap;
}

.custom-table-mockup td {
  padding: 14px;
  border-top: 1px solid #e5e7eb;
  font-size: 0.9rem;
  color: #374151;
  vertical-align: middle;
}

.status-pill-mockup {
  padding: 5px 11px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
  display: inline-block;
  white-space: nowrap;
}

.pendiente  { background: #fef3c7; color: #92400e; }
.aprobada   { background: #dcfce7; color: #166534; }
.rechazada  { background: #fee2e2; color: #991b1b; }
.cancelada  { background: #e5e7eb; color: #374151; }
.finalizada { background: #dbeafe; color: #1e40af; }

.actions-cell-fixed { text-align: center; }

.actions-wrapper {
  display: flex;
  gap: 8px;
  justify-content: center;
  align-items: center;
}

.action-btn-mockup {
  width: 30px;
  height: 30px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background-color: white;
  cursor: pointer;
  background-position: center;
  background-repeat: no-repeat;
  background-size: 14px;
}

.action-btn-mockup:hover:not(:disabled) { background-color: #f3f4f6; }

.btn-disabled {
  opacity: 0.25;
  cursor: not-allowed;
}

.icon-view   { background-image: url('../assets/icons/ver.png'); }
.icon-edit   { background-image: url('../assets/icons/editar-negro.png'); }
.icon-delete { background-image: url('../assets/icons/eliminar.png'); }

.modal-overlay-mockup {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal-container-central {
  background: white;
  width: 650px;
  max-width: 95%;
  max-height: 90vh; 
  overflow-y: auto;  
  border-radius: 18px;
  padding: 24px;
  box-shadow: 0 15px 35px rgba(0,0,0,.12);
}

.form-panel-header-central {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 18px;
  padding-bottom: 12px;
  border-bottom: 1px solid #e5e7eb;
}

.form-panel-header-central h3 { margin: 0; color: #111827; font-size: 1.1rem; font-weight: 800; }
.btn-close-modal { background: transparent; border: none; font-size: 1.5rem; color: #6b7280; cursor: pointer; }

.form-solicitud-mockup { display: flex; flex-direction: column; gap: 14px; }
.form-group-mockup     { display: flex; flex-direction: column; gap: 6px; }
.form-group-mockup label { font-size: 0.85rem; font-weight: 700; color: #374151; }

.form-group-mockup input,
.form-group-mockup textarea,
.form-group-mockup select {
  padding: 10px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  font-size: 0.9rem;
  outline: none;
}

.form-group-mockup input:focus,
.form-group-mockup textarea:focus,
.form-group-mockup select:focus { border-color: #9ca3af; }

.form-row-mockup     { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.form-actions-central { display: flex; justify-content: flex-end; gap: 10px; }

.detalle-solicitud-wrapper { display: flex; flex-direction: column; gap: 14px; }
.detalle-item strong { display: block; font-size: 0.8rem; color: #6b7280; text-transform: uppercase; margin-bottom: 4px; }
.detalle-item p      { color: #111827; font-size: 0.95rem; font-weight: 500; }
.detalle-motivo-text { background: #f9fafb; padding: 10px; border-radius: 10px; border: 1px solid #e5e7eb; }

.toast-error-moderno {
  position: fixed;
  top: 20px;
  right: 20px;
  background: white;
  border-left: 4px solid #dc2626;
  padding: 16px;
  border-radius: 10px;
  box-shadow: 0 10px 20px rgba(0,0,0,.1);
  z-index: 1000;
  min-width: 320px;
}

.toast-title { font-weight: 800; color: #111827; font-size: 0.85rem; }
.toast-text  { color: #6b7280; font-size: 0.85rem; margin-top: 4px; }

.btn-close-toast {
  background: transparent;
  border: none;
  font-size: 1.2rem;
  color: #6b7280;
  cursor: pointer;
  position: absolute;
  top: 10px;
  right: 12px;
}
</style>