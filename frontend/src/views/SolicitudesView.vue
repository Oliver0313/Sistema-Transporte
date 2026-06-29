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
      v-if="puedeCrear" 
      class="btn-new-solicitud-trigger" 
      @click="mostrarFormulario = true">
      Nueva solicitud
    </button>

    <button class="btn-new-solicitud-trigger" @click="mostrarHistorial = true">
  Ver historial
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
                  <button class="action-btn-mockup" title="Ver detalle" @click="verDetalleSolicitud(solicitud)">
                <Eye :size="15" />
              </button>
                  
                <button 
                  class="action-btn-mockup" 
                  :class="{ 'btn-disabled': !puedeEditarOAsignar }"
                  :disabled="!puedeEditarOAsignar" 
                  :title="puedeEditarOAsignar ? (esSupervisor ? 'Asignar Unidad' : 'Editar Solicitud') : 'No permitido para operador'"
                  @click="abrirModificarSolicitud(solicitud)"
                >
                  <Pencil :size="15" />
                </button>
                  
                <button 
                  class="action-btn-mockup" 
                  :class="{ 'btn-disabled': !puedeEliminar }"
                  :disabled="!puedeEliminar" 
                  :title="puedeEliminar ? 'Eliminar solicitud' : 'No permitido para su rol'"
                  @click="eliminarSolicitud(solicitud.id)"
                >
                  <Trash2 :size="15" />
                </button>
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
        <h3>{{ formModel.id ? (esSupervisor ? 'Asignar Transporte #' + formModel.id : 'Modificar Solicitud #' + formModel.id) : 'Registrar Nueva Solicitud' }}</h3>
        <button class="btn-close-modal" @click="cerrarFormulario">×</button>
      </div>

      <form @submit.prevent="guardarSolicitud" class="form-solicitud-mockup">
  
  <div class="form-row-mockup">
    <div class="form-group-mockup">
      <label>Área Solicitante</label>
      <input type="text" v-model="formModel.areaSolicitante" :disabled="esSupervisor" placeholder="Ej. RRHH" required />
    </div>
    <div class="form-group-mockup">
      <label>Cantidad de Colaboradores</label>
      <input type="number" v-model.number="formModel.cantidadColaboradores" :disabled="esSupervisor" min="1" required />
    </div>
  </div>

  <div class="form-row-mockup">
    <div class="form-group-mockup">
      <label>Fecha y Hora Salida</label>
      <input type="datetime-local" v-model="formModel.fechaHoraSalida" :disabled="esSupervisor" required />
    </div>
    <div class="form-group-mockup">
      <label>Fecha y Hora Regreso</label>
      <input type="datetime-local" v-model="formModel.fechaHoraRegreso" :disabled="esSupervisor" required />
    </div>
  </div>

  <div class="form-row-mockup">
    <div class="form-group-mockup">
      <label>Destino</label>
      <input type="text" v-model="formModel.destino" :disabled="esSupervisor" placeholder="Ej. Bonao" required />
    </div>
    <div v-if="formModel.id" class="form-group-mockup">
      <label>Estado Proceso</label>
      <select class="mockup-select" v-model="formModel.estado" :disabled="!esSupervisor">
        <option value="1">Pendiente</option>
        <option value="2">Aprobada</option>
        <option value="3">Rechazada</option>
        <option value="4">Cancelada</option>
        <option value="5">Finalizada</option>
      </select>
    </div>
  </div>

  <div class="form-group-mockup">
    <label>Motivo del Viaje</label>
    <textarea v-model="formModel.motivo" :disabled="esSupervisor" placeholder="Ej. Capacitacion" rows="2" required></textarea>
  </div>

  <div class="form-actions-central" style="margin-top: 10px;">
    <button type="button" class="btn-cancel-mockup" @click="cerrarFormulario">Cancelar</button>
    <button type="submit" class="btn-submit-mockup" :disabled="guardando">
      {{ guardando ? 'Enviando a la API...' : (formModel.id ? 'Confirmar Cambios' : 'Registrar Solicitud') }}
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
  <div v-if="mostrarHistorial" class="modal-overlay-mockup" @click.self="mostrarHistorial = false">
  <div class="modal-container-historial">
    <div class="form-panel-header-central">
      <h3>Historial de solicitudes</h3>
      <button class="btn-close-modal" @click="mostrarHistorial = false">×</button>
    </div>

    <div class="historial-filters">
      <input type="text" placeholder="Buscar por ID, área, destino..." v-model="histBusqueda" class="filter-search-input" style="flex:2" />
      <select class="mockup-select" v-model="histEstado">
        <option value="">Todos los estados</option>
        <option value="1">Pendiente</option>
        <option value="2">Aprobada</option>
        <option value="3">Rechazada</option>
        <option value="4">Cancelada</option>
        <option value="5">Finalizada</option>
        <option value="6">Asignada</option>
      </select>
      <input type="date" class="mockup-date-input" v-model="histDesde" />
      <input type="date" class="mockup-date-input" v-model="histHasta" />
      <button class="btn-cancel-mockup" @click="limpiarHistorial">Limpiar</button>
    </div>

    <div class="table-responsive">
      <table class="custom-table-mockup">
        <thead>
          <tr>
            <th>ID</th>
            <th>Área</th>
            <th>Personas</th>
            <th>Salida</th>
            <th>Destino</th>
            <th>Motivo</th>
            <th>Estado</th>
            <th>Vehículo</th>
            <th>Conductor</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="s in solicitudesHistorial" :key="s.id">
            <td>#{{ s.id }}</td>
            <td>{{ s.areaSolicitante }}</td>
            <td>{{ s.cantidadColaboradores }}</td>
            <td>{{ formatearFechaVista(s.fechaHoraSalida) }}</td>
            <td>{{ s.destino }}</td>
            <td>{{ s.motivo }}</td>
            <td>
              <span :class="['status-pill-mockup', obtenerClaseEstado(s.estado)]">
                {{ formatearEstadoVista(s.estado) }}
              </span>
            </td>
            <td>{{ s.vehiculoAsignado || '---' }}</td>
            <td>{{ s.conductorAsignado || '---' }}</td>
          </tr>
          <tr v-if="solicitudesHistorial.length === 0">
            <td colspan="9" style="text-align:center;padding:30px;color:#6b7280">
              No se encontraron solicitudes.
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div style="padding:10px 14px;font-size:0.8rem;color:#9ca3af;border-top:1px solid #e5e7eb">
      Mostrando {{ solicitudesHistorial.length }} de {{ solicitudes.length }} solicitudes
    </div>
  </div>
</div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { Eye, Pencil, Trash2 } from 'lucide-vue-next'

const filtroBusqueda = ref('')
const filtroEstado = ref('')
const filtroArea = ref('')
const filtroFecha = ref('')

const mostrarFormulario = ref(false)
const guardando = ref(false)
const solicitudes = ref([])
const userRole = ref('')


const vehiculos = ref([])
const conductores = ref([])

const mensajeErrorFlotante = ref('')

const mostrarHistorial = ref(false)
const histBusqueda = ref('')
const histEstado   = ref('')
const histDesde    = ref('')
const histHasta    = ref('')

const limpiarHistorial = () => {
  histBusqueda.value = ''
  histEstado.value   = ''
  histDesde.value    = ''
  histHasta.value    = ''
}

const solicitudesHistorial = computed(() => {
  const txt = histBusqueda.value.toLowerCase().trim()
  return solicitudes.value.filter(s => {
    const matchTxt = !txt || [
      String(s.id), s.areaSolicitante, s.destino, s.motivo,
      s.vehiculoAsignado, s.conductorAsignado
    ].some(v => v?.toLowerCase().includes(txt))

    const matchEst   = !histEstado.value || s.estado == histEstado.value
    const fecha      = s.fechaHoraSalida?.slice(0, 10)
    const matchDesde = !histDesde.value  || fecha >= histDesde.value
    const matchHasta = !histHasta.value  || fecha <= histHasta.value

    return matchTxt && matchEst && matchDesde && matchHasta
  })
})

const formModel = ref({
  id: null,
  areaSolicitante: '',
  cantidadColaboradores: 1,
  fechaHoraSalida: '',
  fechaHoraRegreso: '',
  destino: '',
  motivo: '',
  estado: 1,
  vehiculoId: '',   
  conductorId: ''  
})

const mostrarDetalle = ref(false)
const solicitudSeleccionada = ref({})

const obtenerRolDesdeToken = () => {
  const token = localStorage.getItem('token_transporte')
  if (!token) return

  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    )
    const decoded = JSON.parse(jsonPayload)
    userRole.value = decoded.role || decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || ''
  } catch (e) {
    console.error("Error decodificando el token de seguridad:", e)
  }
}

const esSupervisor = computed(() => {
  return userRole.value.toLowerCase() === 'supervisor'
})

const puedeCrear = computed(() => {
  const rol = userRole.value.toLowerCase()
  return rol === 'operador' || rol === 'admin' || rol === 'superadmin' || rol === 'administrador'
})

const puedeEditarOAsignar = computed(() => {
  const rol = userRole.value.toLowerCase()
  return rol === 'supervisor' || rol === 'admin' || rol === 'superadmin' || rol === 'administrador'
})

const puedeEliminar = computed(() => {
  const rol = userRole.value.toLowerCase()
  return rol === 'admin' || rol === 'superadmin' || rol === 'administrador'
})

const verDetalleSolicitud = (solicitud) => {
  solicitudSeleccionada.value = { ...solicitud }
  mostrarDetalle.value = true
}


const fetchSolicitudesDeAPI = async () => {
  const token = localStorage.getItem('token_transporte')
  const headers = { Authorization: `Bearer ${token}` }
  
  try {
    const [resV, resC, resS] = await Promise.all([
      fetch('https://localhost:7221/api/Vehiculos', { headers }),
      fetch('https://localhost:7221/api/Conductores', { headers }),
      fetch('https://localhost:7221/api/solicitudestransporte', { headers })
    ])

    if (resV.ok) vehiculos.value = await resV.json()
    if (resC.ok) conductores.value = await resC.json()
    if (resS.ok) {
      solicitudes.value = await resS.json()
    }
  } catch (error) {
    console.error('Error cargando los datos core de la API:', error)
  }
}

const areasDisponibles = computed(() => {
  const areas = solicitudes.value
    .map(s => s.areaSolicitante?.trim())
    .filter(Boolean)
  return [...new Set(areas)].sort()
})

const solicitudesFiltradas = computed(() => {
  return solicitudes.value.filter(solicitud => {
    const texto = filtroBusqueda.value.toLowerCase().trim()
    const cumpleBusqueda =
      !texto ||
      solicitud.destino?.toLowerCase().includes(texto) ||
      solicitud.motivo?.toLowerCase().includes(texto) ||
      solicitud.areaSolicitante?.toLowerCase().includes(texto) ||
      solicitud.cantidadColaboradores?.toString().includes(texto) ||
      formatearEstadoVista(solicitud.estado).toLowerCase().includes(texto) ||
      solicitud.vehiculoAsignado?.toLowerCase().includes(texto) ||
      solicitud.conductorAsignado?.toLowerCase().includes(texto) ||
      solicitud.id?.toString().includes(texto)

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
    hour12: true, year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit'
  })
}

const formatearFechaInput = (fechaIso) => {
  if (!fechaIso) return ''
  const d = new Date(fechaIso)
  const pad = (n) => n.toString().padStart(2, '0')
  return `${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`
}

const formatearEstadoVista = (estado) => {
  const mapeoEstados = { 1: 'Pendiente', 2: 'Aprobada', 3: 'Rechazada', 4: 'Cancelada', 5: 'Finalizada' }
  return mapeoEstados[estado] || estado || 'Pendiente'
}

const obtenerClaseEstado = (estado) => {
  const clases = { 1: 'pendiente', 2: 'aprobada', 3: 'rechazada', 4: 'cancelada', 5: 'finalizada' }
  return clases[estado] || 'pendiente'
}


const abrirModificarSolicitud = (solicitud) => {

  let vId = ''
  let cId = ''

  
  if (solicitud.vehiculoAsignado) {
    const vEncontrado = vehiculos.value.find(v => 
      solicitud.vehiculoAsignado.includes(v.matricula) || 
      solicitud.vehiculoAsignado.includes(v.modelo)
    )
    if (vEncontrado) vId = vEncontrado.id
  }


  if (solicitud.conductorAsignado) {
    const cEncontrado = conductores.value.find(c => 
      solicitud.conductorAsignado.includes(c.nombre)
    )
    if (cEncontrado) cId = cEncontrado.id
  }


  formModel.value = { 
    id: solicitud.id,
    areaSolicitante: solicitud.areaSolicitante,
    cantidadColaboradores: solicitud.cantidadColaboradores,
    fechaHoraSalida: formatearFechaInput(solicitud.fechaHoraSalida),
    fechaHoraRegreso: formatearFechaInput(solicitud.fechaHoraRegreso),
    destino: solicitud.destino,
    motivo: solicitud.motivo,
    estado: solicitud.estado || 1,
    

    vehiculoId: vId, 
    conductorId: cId 
  }

  if (esSupervisor.value && formModel.value.estado === 1) {
    formModel.value.estado = 2
  }
  mostrarFormulario.value = true
}


const cerrarFormulario = () => {
  mostrarFormulario.value = false
  formModel.value = {
    id: null, areaSolicitante: '', cantidadColaboradores: 1, fechaHoraSalida: '',
    fechaHoraRegreso: '', destino: '', motivo: '', estado: 1, vehiculoId: '', conductorId: ''
  }
}


const guardarSolicitud = async () => {
  console.log('=== GUARDANDO ===')
  console.log('esSupervisor:', esSupervisor.value)
  console.log('formModel:', JSON.stringify(formModel.value))

  const salida = new Date(formModel.value.fechaHoraSalida)
  const regreso = new Date(formModel.value.fechaHoraRegreso)
  console.log('salida:', salida)
  console.log('regreso:', regreso)

  if (!esSupervisor.value) {
    if (salida.getTime() === regreso.getTime()) {
      mensajeErrorFlotante.value = 'La fecha y hora de salida no puede ser igual a la de regreso.'
      return
    }
    if (regreso < salida) {
      mensajeErrorFlotante.value = 'La fecha de regreso no puede ser anterior a la fecha de salida.'
      return
    }
  }

  guardando.value = true
  const token = localStorage.getItem('token_transporte')
  const idUsuarioLogueado = parseInt(localStorage.getItem('usuario_id')) || 1


  const payload = {
    id: formModel.value.id || 0,
    areaSolicitante: formModel.value.areaSolicitante,
    cantidadColaboradores: parseInt(formModel.value.cantidadColaboradores),
    fechaHoraSalida: salida.toISOString(),
    fechaHoraRegreso: regreso.toISOString(),
    destino: formModel.value.destino,
    motivo: formModel.value.motivo,
    estado: parseInt(formModel.value.estado),
    usuarioSolicitanteId: idUsuarioLogueado,
conductorId: formModel.value.conductorId ? parseInt(formModel.value.conductorId) : null,
vehiculoId: formModel.value.vehiculoId ? parseInt(formModel.value.vehiculoId) : null
  }

  const url = formModel.value.id
    ? `https://localhost:7221/api/solicitudestransporte/${formModel.value.id}`
    : 'https://localhost:7221/api/solicitudestransporte'

  const method = formModel.value.id ? 'PUT' : 'POST'

  console.log('URL:', url)
  console.log('method:', method)
  console.log('payload:', JSON.stringify(payload))

  try {
    const response = await fetch(url, {
      method: method,
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    })

    console.log('response status:', response.status)

    if (response.ok) {
      await fetchSolicitudesDeAPI()
      cerrarFormulario()
    } else {
      const errorResponse = await response.json()
      console.error('Error del servidor:', errorResponse)
      alert('Error devuelto por el servidor: ' + JSON.stringify(errorResponse.errors || errorResponse))
    }
  } catch (error) {
    console.error('ERROR EN FETCH:', error)
    alert('Error de red: ' + error.message)
  } finally {
    guardando.value = false
  }
}

const eliminarSolicitud = async (id) => {
  mensajeErrorFlotante.value = 'Las solicitudes no pueden ser eliminadas del sistema.'
  setTimeout(() => { mensajeErrorFlotante.value = '' }, 5000)
}

onMounted(() => {
  obtenerRolDesdeToken()
  fetchSolicitudesDeAPI() 
})
</script>
<style scoped>

.solicitudes-container-page,
.solicitudes-container-page * {
  font-family: inherit !important;
}


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

.search-filter-wrapper {
  position: relative;
  flex: 1;
}

.filter-search-input {
  width: 100%;
  padding: 10px 14px 10px 40px;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
  font-size: 0.9rem;
  outline: none;
}

.filter-search-input:focus {
  border-color: #9ca3af;
}

.filter-search-icon {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  width: 16px;
  height: 16px;
  opacity: 0.45;
}

.filter-group-item {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.filter-group-item label {
  font-size: 0.85rem;
  font-weight: 700;
  color: #374151;
}

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
.mockup-date-input:focus {
  border-color: #9ca3af;
}


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
.btn-new-solicitud-trigger:hover,
.btn-submit-mockup:hover {
  opacity: 0.92;
}

.btn-new-solicitud-trigger {
  margin-left: auto;
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


.card-panel-mockup {
  background: white;
  border-radius: 18px;
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

.pendiente { background: #fef3c7; color: #92400e; }
.aprobada { background: #dcfce7; color: #166534; }
.rechazada { background: #fee2e2; color: #991b1b; }
.cancelada { background: #e5e7eb; color: #374151; }
.finalizada { background: #dbeafe; color: #1e40af; }


.actions-cell-fixed {
  text-align: center;
}

.actions-wrapper {
  display: flex;
  gap: 8px;
  justify-content: center;
  align-items: center;
}

.action-btn-mockup {
  width: 32px;
  height: 32px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background-color: white;
  color: #374151;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.action-btn-mockup:hover:not(:disabled) {
  background-color: #f3f4f6;
}

.btn-disabled {
  opacity: 0.25;
  cursor: not-allowed;
}

.icon-view { background-image: url('../assets/icons/ver.png'); }
.icon-edit { background-image: url('../assets/icons/editar-negro.png'); }
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
  width: 600px;
  max-width: 95%;
  border-radius: 18px;
  padding: 22px;
  box-shadow: 0 15px 35px rgba(0,0,0,.12);
}

.form-panel-header-central {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 14px;
  padding-bottom: 10px;
  border-bottom: 1px solid #e5e7eb;
}

.form-panel-header-central h3 {
  margin: 0;
  color: #111827;
  font-size: 1.1rem;
  font-weight: 800;
}

.btn-close-modal {
  background: transparent;
  border: none;
  font-size: 1.5rem;
  color: #6b7280;
  cursor: pointer;
}


.form-solicitud-mockup {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.form-group-mockup {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.form-group-mockup label {
  font-size: 0.82rem;
  font-weight: 700;
  color: #374151;
  margin-bottom: 2px;
}


.form-group-mockup input,
.form-group-mockup select,
.form-solicitud-mockup .mockup-select {
  height: 40px !important;
  box-sizing: border-box;
  padding: 8px 12px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  font-size: 0.9rem;
  outline: none;
  background: #ffffff;
}

.form-group-mockup input:focus,
.form-group-mockup select:focus {
  border-color: #9ca3af;
}

.form-group-mockup textarea {
  padding: 10px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  font-size: 0.9rem;
  outline: none;
  resize: none; 
}

.form-group-mockup textarea:focus {
  border-color: #9ca3af;
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
  margin-top: 6px;
}


.detalle-solicitud-wrapper {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.detalle-item strong {
  display: block;
  font-size: 0.8rem;
  color: #6b7280;
  text-transform: uppercase;
  margin-bottom: 4px;
}

.detalle-item p {
  color: #111827;
  font-size: 0.95rem;
  font-weight: 500;
}

.detalle-motivo-text {
  background: #f9fafb;
  padding: 10px;
  border-radius: 10px;
  border: 1px solid #e5e7eb;
}


.toast-error-moderno {
  position: fixed;
  top: 20px;
  right: 20px;
  background: white;
  border-left: 4px solid #10b981; 
  padding: 16px;
  border-radius: 10px;
  box-shadow: 0 10px 20px rgba(0,0,0,.1);
  z-index: 1000;
  min-width: 320px;
}

.toast-title {
  font-weight: 800;
  color: #111827;
  font-size: 0.85rem;
}

.toast-text {
  color: #6b7280;
  font-size: 0.85rem;
  margin-top: 4px;
}

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

.modal-container-historial {
  background: white;
  width: 95vw;
  max-width: 1100px;
  max-height: 88vh;
  border-radius: 18px;
  padding: 22px;
  box-shadow: 0 15px 35px rgba(0,0,0,.12);
  display: flex;
  flex-direction: column;
  gap: 16px;
  overflow: hidden;
}

.modal-container-historial .table-responsive {
  overflow-y: auto;
  flex: 1;
}

.historial-filters {
  display: flex;
  gap: 10px;
  align-items: center;
  flex-wrap: wrap;
}

.historial-filters input,
.historial-filters select {
  height: 36px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  padding: 0 10px;
  font-size: 0.88rem;
  flex: 1;
  min-width: 120px;
}
</style>