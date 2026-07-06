<template>
  <div>
    <div class="section-header">
      <div>
        <h2>Conductores</h2>
        <p>Administra la información y disponibilidad de los conductores.</p>
      </div>
    </div>

    <div class="card-panel">
      <div class="table-responsive">
       <div class="toolbar">
  <div class="search-wrapper">
    <Search :size="16" class="search-icon" />
    <input
      v-model="filtroBusqueda"
      type="text"
      placeholder="Buscar conductor..."
      class="search-box"
    />
  </div>

  <div class="toolbar-actions">
    <button
      class="btn-new-solicitud-trigger"
      :class="{ 'btn-disabled-main': !puedeModificarConductores }"
      :disabled="!puedeModificarConductores"
      :title="puedeModificarConductores ? 'Registrar nuevo conductor' : 'No permitido para su rol'"
      @click="abrirFormularioNuevo"
    >
      <Plus :size="17" />
      <span>Nuevo conductor</span>
    </button>

   <button class="btn-new-solicitud-trigger" @click="mostrarHistorial = true; cargarDatosHistorial()">
  Ver historial
</button>
  </div>
</div>
        
        <table class="conductores-table">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Cédula</th>
              <th>Email</th>
              <th>Licencia</th>
              <th>Tipo licencia</th>
              <th>Vencimiento</th>
              <th>Teléfono</th>
              <th>Dirección</th>
              <th>Estado</th>
              <th class="text-center" style="width: 120px;">Acciones</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="conductor in conductoresFiltrados" :key="conductor.id">
              <td>{{ conductor.nombre }} {{ conductor.apellido }}</td>
              <td>{{ conductor.cedula }}</td>
              <td>{{ conductor.email }}</td>
              <td>{{ conductor.licencia }}</td>
              <td>{{ formatearTipoLicencia(conductor.tipoLicencia) }}</td>
              <td>{{ formatearFecha(conductor.fechaVencimientoLicencia) }}</td>
              <td>{{ conductor.telefono }}</td>
              <td>{{ conductor.direccion }}</td>
              <td>
                <span
                  class="estado-badge"
                  :class="conductor.estado === 1 ? 'activo' : 'inactivo'"
                >
                  {{ conductor.estado === 1 ? 'Disponible' : 'No disponible' }}
                </span>
              </td>
              <td class="actions-cell-fixed">
                <div class="actions-wrapper">
                  <button
                    class="action-btn-mockup"
                    title="Ver detalle"
                    @click="verDetalleConductor(conductor)"
                  >
                    <Eye :size="15" />
                  </button>

                  <button
                    class="action-btn-mockup"
                    :class="{ 'btn-disabled': !puedeModificarConductores }"
                    :disabled="!puedeModificarConductores"
                    title="Editar conductor"
                    @click="abrirFormularioEdicion(conductor)"
                  >
                    <Pencil :size="15" />
                  </button>

                  <button
                    class="action-btn-mockup"
                    :class="{ 'btn-disabled': !puedeEliminarConductores }"
                    :disabled="!puedeEliminarConductores"
                    title="Eliminar conductor"
                    @click="eliminarConductorApi(conductor.id)"
                  >
                    <Trash2 :size="15" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="dashboard-widgets-row">
      
      <div class="widget-card-panel">
        <h4>Viajes o Estados Clave</h4>
        <div class="feed-list">
          <div 
            v-for="c in conductores.slice(0, 3)" 
            :key="'viaje-' + c.id" 
            class="feed-item-row feed-clickable"
            @click="verDetalleConductor(c)"
            title="Ver ficha completa"
          >
            <span class="feed-status-dot" :class="c.estado === 1 ? 'dot-dispo' : 'dot-nodispo'"></span>
            <div class="feed-item-info">
              <h5>{{ c.nombre }} {{ c.apellido }}</h5>
              <p>Licencia: {{ c.licencia }} • {{ c.direccion || 'Sin dirección' }}</p>
            </div>
            <span class="feed-time-text">Activo</span>
          </div>
        </div>
      </div>

      <div class="widget-card-panel">
        <h4>Historial de eventos recientes</h4>
        <div class="feed-list">
          <div 
            v-for="(log, index) in ultimosEventosConductores" 
            :key="'log-' + index" 
            class="feed-item-row-complex feed-clickable"
            @click="verDetalleConductor(conductores[index])"
            title="Ver ficha completa"
          >
            <div class="feed-icon-wrapper" :class="log.claseIcono">
              <span class="icon-placeholder-text">{{ log.inicial }}</span>
            </div>
            <div class="feed-item-info">
              <h5>{{ log.titulo }}</h5>
              <p>{{ log.descripcion }}</p>
            </div>
          </div>

          <div v-if="conductores.length === 0" class="text-center text-muted" style="padding: 10px;">
            No hay actividades recientes para procesar.
          </div>
        </div>
      </div>

    </div>

  </div> <div v-if="mostrarModal" class="modal-overlay" @click.self="cerrarFormulario">
    <div class="modal-container">
      <div class="modal-header">
        <h3>{{ modoEdicion ? 'Modificar Registro de Conductor' : 'Nuevo Conductor' }}</h3>
        <button class="btn-close" @click="cerrarFormulario">×</button>
      </div>

      <div class="modal-body">
        <form @submit.prevent="guardarConductor" class="form-grid">
          <input v-model="formConductor.nombre" placeholder="Nombre" required />
          <input v-model="formConductor.apellido" placeholder="Apellido" required />
          <input v-model="formConductor.cedula" placeholder="Cédula" required />
          <input v-model="formConductor.email" type="email" placeholder="Correo electrónico" required />
          <input v-model="formConductor.licencia" placeholder="Licencia" required />

          <select v-model.number="formConductor.tipoLicencia" required>
            <option :value="1">Categoría 1</option>
            <option :value="2">Categoría 2</option>
            <option :value="3">Categoría 3</option>
            <option :value="4">Categoría 4</option>
          </select>

          <input type="date" v-model="formConductor.fechaVencimientoLicencia" required />
          <input v-model="formConductor.telefono" placeholder="Teléfono" required />
          <input v-model="formConductor.direccion" placeholder="Dirección" class="full-width" required />

          <div v-if="modoEdicion" class="form-group-full-select full-width">
            <label style="font-size: 0.85rem; font-weight:700; color:#374151; display:block; margin-bottom:4px;">Estado del Chofer</label>
            <select v-model.number="formConductor.estado" style="width: 100%;">
              <option :value="1">Disponible</option>
              <option :value="2">No disponible / En Viaje</option>
            </select>
          </div>

          <div class="modal-actions">
            <button type="button" class="btn-cancel" @click="cerrarFormulario">Cancelar</button>
            <button type="submit" class="btn-primary" :disabled="guardando">
              {{ guardando ? 'Guardando en la API...' : 'Guardar Conductor' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>

  <div v-if="mostrarDetalle" class="modal-overlay" @click.self="mostrarDetalle = false">
    <div class="modal-container" style="width: 500px;">
      <div class="modal-header">
        <h3>Ficha del Conductor</h3>
        <button class="btn-close" @click="mostrarDetalle = false">×</button>
      </div>
      <div class="detalle-solicitud-wrapper" style="margin-top: 15px; display: flex; flex-direction: column; gap: 12px;">
        <div><strong>Nombre Completo:</strong><p>{{ conductorSeleccionado.nombre }} {{ conductorSeleccionado.apellido }}</p></div>
        <div><strong>Documento Identidad (Cédula):</strong><p>{{ conductorSeleccionado.cedula }}</p></div>
        <div><strong>Correo Electrónico:</strong><p>{{ conductorSeleccionado.email }}</p></div>
        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 10px;">
          <div><strong>Núm. Licencia:</strong><p>{{ conductorSeleccionado.licencia }}</p></div>
          <div><strong>Categoría:</strong><p>{{ formatearTipoLicencia(conductorSeleccionado.tipoLicencia) }}</p></div>
        </div>
        <div><strong>Vencimiento de Licencia:</strong><p>{{ formatearFecha(conductorSeleccionado.fechaVencimientoLicencia) }}</p></div>
        <div><strong>Teléfono Contacto:</strong><p>{{ conductorSeleccionado.telefono }}</p></div>
        <div><strong>Dirección Residencial:</strong><p>{{ conductorSeleccionado.direccion }}</p></div>
      </div>
      <div class="modal-actions" style="margin-top: 20px;">
        <button type="button" class="btn-primary" @click="mostrarDetalle = false">Cerrar Ficha</button>
      </div>
    </div>
  </div>

  <div v-if="mensajeNotificacion" class="toast-error-moderno">
    <div class="toast-content">
      <span class="toast-title">Notificación</span>
      <p class="toast-text">{{ mensajeNotificacion }}</p>
    </div>
    <button class="btn-close-toast" @click="mensajeNotificacion = ''">×</button>
  </div>

  <div v-if="mostrarHistorial" class="modal-overlay" @click.self="mostrarHistorial = false">
  <div class="modal-container-historial">
    <div class="modal-header">
      <h3>Historial de viajes</h3>
      <button class="btn-close" @click="mostrarHistorial = false">×</button>
    </div>

    <div class="historial-filters">
      <input type="text" placeholder="Buscar por destino, vehículo, conductor..." v-model="histBusqueda" class="search-box" style="flex:2;min-width:200px" />
      <select class="mockup-select" v-model="histEstado">
        <option value="">Todos los estados</option>
        <option value="1">Activa</option>
        <option value="2">Reasignada</option>
        <option value="3">Cancelada</option>
        <option value="4">Finalizada</option>
      </select>
      <input type="date" class="mockup-select" v-model="histDesde" />
      <input type="date" class="mockup-select" v-model="histHasta" />
      <button class="btn-cancel" @click="limpiarHistorial">Limpiar</button>
    </div>

    <div class="table-responsive">
      <table class="conductores-table">
        <thead>
          <tr>
            <th>#</th>
            <th>Fecha</th>
            <th>Conductor</th>
            <th>Destino</th>
            <th>Vehículo</th>
            <th>Pasajeros</th>
            <th>Estado del viaje</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="v in viajesHistorial" :key="v.id">
            <td>#{{ v.id }}</td>
            <td>{{ formatearFecha(v.fecha) }}</td>
            <td>{{ v.conductorNombre }}</td>
            <td>{{ v.destino }}</td>
            <td>{{ v.vehiculo }}</td>
            <td>{{ v.pasajeros }}</td>
            <td>
              <span :class="['estado-badge', claseViaje(v.estado)]">
                {{ labelViaje(v.estado) }}
              </span>
            </td>
          </tr>
          <tr v-if="viajesHistorial.length === 0">
            <td colspan="7" style="text-align:center;padding:30px;color:#6b7280">
              No se encontraron viajes.
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div style="padding:10px 14px;font-size:0.8rem;color:#9ca3af;border-top:1px solid #e5e7eb">
      Mostrando {{ viajesHistorial.length }} de {{ asignaciones.length }} viajes registrados
    </div>
  </div>
</div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { Eye, Pencil, Trash2, Plus, Search } from 'lucide-vue-next'

const conductores = ref([])
const filtroBusqueda = ref('')
const userRole = ref('Operador')

const mostrarModal = ref(false)
const mostrarDetalle = ref(false)
const modoEdicion = ref(false)
const guardando = ref(false)

const mensajeNotificacion = ref('')
const conductorSeleccionado = ref({})

const mostrarHistorial = ref(false)
const histBusqueda    = ref('')
const histEstado      = ref('')
const histDesde       = ref('')
const histHasta       = ref('')
const asignaciones    = ref([])
const solicitudesHist = ref([])

const limpiarHistorial = () => {
  histBusqueda.value = ''
  histEstado.value   = ''
  histDesde.value    = ''
  histHasta.value    = ''
}

const viajesHistorial = computed(() => {
  const txt = histBusqueda.value.toLowerCase().trim()

  return asignaciones.value
    .map(a => {
      const solicitud = solicitudesHist.value.find(s => s.id === a.solicitudTransporteId)
      const conductor = conductores.value.find(c => c.id === a.conductorId)
      return {
        id:          a.id,
        fecha:       a.fechaHoraAsignacion,
        destino:     solicitud?.destino     || '---',
        vehiculo:    obtenerVehiculo(a.vehiculoId),
        pasajeros:   solicitud?.cantidadColaboradores ?? '---',
        estado:      a.estado,
        conductorId: a.conductorId,
        conductorNombre: conductor ? `${conductor.nombre} ${conductor.apellido}` : `#${a.conductorId}`
      }
    })
    .filter(v => {
      const matchTxt = !txt || [
        String(v.id), v.destino, v.vehiculo, v.conductorNombre
      ].some(x => x?.toLowerCase().includes(txt))
      const matchEst   = !histEstado.value || v.estado == histEstado.value
      const fecha      = v.fecha?.slice(0, 10)
      const matchDesde = !histDesde.value  || fecha >= histDesde.value
      const matchHasta = !histHasta.value  || fecha <= histHasta.value
      return matchTxt && matchEst && matchDesde && matchHasta
    })
})

const labelViaje = (estado) => {
  return { 1:'Activa', 2:'Reasignada', 3:'Cancelada', 4:'Finalizada' }[estado] || '---'
}

const claseViaje = (estado) => {
  return { 1:'activo', 2:'activo', 3:'inactivo', 4:'activo' }[estado] || 'activo'
}

const cargarDatosHistorial = async () => {
  const token = localStorage.getItem('token_transporte')
  const headers = { Authorization: `Bearer ${token}` }
  try {
    const [resA, resS, resV] = await Promise.all([
      fetch('https://localhost:7221/api/Asignaciones', { headers }),
      fetch('https://localhost:7221/api/solicitudestransporte', { headers }),
      fetch('https://localhost:7221/api/Vehiculos', { headers })
    ])
    if (resA.ok) asignaciones.value    = await resA.json()
    if (resS.ok) solicitudesHist.value = await resS.json()
    if (resV.ok) vehiculosHist.value   = await resV.json()
  } catch (e) {
    console.error('Error cargando historial:', e)
  }
}

const obtenerVehiculo = (id) => {

  const v = vehiculosHist.value?.find(v => v.id === id)
  return v ? `${v.marca} ${v.modelo} • ${v.matricula}` : `#${id}`
}

const vehiculosHist = ref([])

const formConductor = ref({
  nombre: '', apellido: '', cedula: '', email: '', licencia: '',
  tipoLicencia: 1, fechaVencimientoLicencia: '', telefono: '', direccion: '',
  supervisorId: 0, estado: 1
})


const obtenerRolDesdeToken = () => {
  const token = localStorage.getItem('token_transporte')
  if (!token) return
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const decoded = JSON.parse(atob(base64))
    userRole.value = decoded.role || decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || 'Operador'
  } catch (e) {
    console.error("Error descifrando credenciales:", e)
  }
}

const esRolAdministrativo = computed(() => {
  const r = userRole.value.toLowerCase()
  return r === 'administrador' || r === 'admin' || r === 'superadmin'
})

const puedeModificarConductores = computed(() => esRolAdministrativo.value)
const puedeEliminarConductores = computed(() => esRolAdministrativo.value)

const cargarConductores = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const response = await fetch('https://localhost:7221/api/Conductores', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (response.ok) {
      conductores.value = await response.json()
    }
  } catch (err) {
    console.error("Error de conexión al cargar la lista:", err)
  }
}

const conductoresFiltrados = computed(() => {
  return conductores.value.filter(c =>
    c.nombre?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
    c.apellido?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
    c.cedula?.includes(filtroBusqueda.value) ||
    c.email?.toLowerCase().includes(filtroBusqueda.value.toLowerCase())
  )
})


const abrirFormularioNuevo = () => {
  modoEdicion.value = false
  formConductor.value = {
    nombre: '', apellido: '', cedula: '', email: '', licencia: '',
    tipoLicencia: 1, fechaVencimientoLicencia: '', telefono: '', direccion: '',
    supervisorId: 0, estado: 1
  }
  mostrarModal.value = true
}

const abrirFormularioEdicion = (conductor) => {
  modoEdicion.value = true
  let fechaInput = ''
  if (conductor.fechaVencimientoLicencia) {
    fechaInput = conductor.fechaVencimientoLicencia.substring(0, 10)
  }
  formConductor.value = { ...conductor, fechaVencimientoLicencia: fechaInput }
  mostrarModal.value = true
}

const cerrarFormulario = () => {
  mostrarModal.value = false
  modoEdicion.value = false
}

const verDetalleConductor = (conductor) => {
  conductorSeleccionado.value = { ...conductor }
  mostrarDetalle.value = true
}

const guardarConductor = async () => {
  guardando.value = true
  const token = localStorage.getItem('token_transporte')

  const payload = {
    ...formConductor.value,
    id: modoEdicion.value ? parseInt(formConductor.value.id) : 0,
    tipoLicencia: parseInt(formConductor.value.tipoLicencia),
    estado: parseInt(formConductor.value.estado),
    fechaVencimientoLicencia: formConductor.value.fechaVencimientoLicencia
      ? new Date(formConductor.value.fechaVencimientoLicencia).toISOString()
      : null
  }

  const conductoresDisponibles = computed(() =>
  conductores.value.filter(c => c.estado === 1).length
  )

  const url = modoEdicion.value 
    ? `https://localhost:7221/api/Conductores/${formConductor.value.id}`
    : 'https://localhost:7221/api/Conductores'

  try {
    const response = await fetch(url, {
      method: modoEdicion.value ? 'PUT' : 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    })

    if (response.ok) {
      cerrarFormulario()
      await cargarConductores()
    } else {
      mensajeNotificacion.value = 'Ocurrió un error de validación en la API.'
    }
  } catch {
    mensajeNotificacion.value = 'Fallo crítico de red con el servidor.'
  } finally {
    guardando.value = false
  }
}

const eliminarConductorApi = async (id) => {
  if (!confirm(`¿Está seguro de eliminar permanentemente al conductor #${id}?`)) return
  const token = localStorage.getItem('token_transporte')
  try {
    const response = await fetch(`https://localhost:7221/api/Conductores/${id}`, {
      method: 'DELETE',
      headers: { Authorization: `Bearer ${token}` }
    })
    if (response.ok) {
      await cargarConductores()
    } else {

      mensajeNotificacion.value = 'No se pudo eliminar la solicitud.'
      setTimeout(() => { mensajeNotificacion.value = '' }, 5000)
    }
  } catch {
    mensajeNotificacion.value = 'El servidor remoto no responde.'
    setTimeout(() => { mensajeNotificacion.value = '' }, 5000)
  }
}

const formatearTipoLicencia = (tipo) => {
  return { 1: 'Categoría 1', 2: 'Categoría 2', 3: 'Categoría 3', 4: 'Categoría 4' }[tipo] || 'No definida'
}

const formatearFecha = (fecha) => {
  if (!fecha) return '---'
  return new Date(fecha).toLocaleDateString('es-DO')
}


const ultimosEventosConductores = computed(() => {
  if (!conductores.value || conductores.value.length === 0) return []


  return conductores.value.slice(0, 4).map((c, index) => {

    if (index % 2 === 0) {
      return {
        titulo: 'Conductor Registrado',
        descripcion: `${c.nombre} ${c.apellido} • Cédula: ${c.cedula}`,
        claseIcono: 'icon-bg-purple',
        inicial: '➕'
      }
    } else {
      const hoy = new Date()
      const vencimiento = new Date(c.fechaVencimientoLicencia)
      const esVencida = vencimiento < hoy

      return {
        titulo: esVencida ? 'Licencia Vencida / Alerta' : 'Verificación de Licencia',
        descripcion: `${c.nombre} - Cat: ${c.tipoLicencia} Vence: ${new Date(c.fechaVencimientoLicencia).toLocaleDateString('es-DO')}`,
        claseIcono: esVencida ? 'icon-bg-red' : 'icon-bg-yellow',
        inicial: esVencida ? '⚠️' : '🪪'
      }
    }
  })
})

onMounted(() => {
  obtenerRolDesdeToken()
  cargarConductores()
})
</script>

<style scoped>
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

.card-panel {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 24px;
}

.table-responsive {
  overflow-x: auto;
}

.conductores-table {
  width: 100%;
  border-collapse: collapse;
}

.conductores-table th {
  background: #f9fafb;
  padding: 14px;
  text-align: left;
  font-size: .85rem;
  font-weight: 700;
  color: #374151;
  white-space: nowrap;
}

.conductores-table td {
  padding: 14px;
  border-top: 1px solid #e5e7eb;
  font-size: 0.9rem;
  color: #374151;
  vertical-align: middle;
}

.estado-badge {
  padding: 5px 11px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
  display: inline-block;
  white-space: nowrap;
}

.activo { background: #dcfce7; color: #166534; }
.inactivo { background: #fee2e2; color: #991b1b; }

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  margin-bottom: 20px;
  position: sticky;
  left: 0;
  width: 100%;
  box-sizing: border-box;
}

.search-box {
  width: 420px; 
  max-width: 60%; 
  padding: 10px 16px;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
  font-size: 0.9rem;
  outline: none;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.search-box:focus {
  border-color: #9ca3af;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.02);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 24px;
}

.header-badge {
  padding: 10px 18px;
  border-radius: 999px;
  font-size: 0.9rem;
  font-weight: 600;
  white-space: nowrap;
}

.header-badge.warning {
  background: #fef3c7;
  color: #92400e;
  border: 1px solid #fcd34d;
}

.header-badge.success {
  background: #dcfce7;
  color: #166534;
  border: 1px solid #86efac;
}

.btn-submit-mockup {
  background: #111827;
  color: white;
  border: none;
  padding: 10px 18px;
  border-radius: 10px;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 700;
  flex-shrink: 0; 
  white-space: nowrap; 
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
  flex-shrink: 0;
  white-space: nowrap;
}

.btn-filter-action:hover,
.btn-new-solicitud-trigger:hover,
.btn-submit-mockup:hover {
  opacity: 0.92;
}

.toolbar-actions {
  display: flex;
  gap: 10px;
  margin-left: auto;
}

.btn-new-solicitud-trigger {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.btn-new-solicitud-trigger:disabled,
.btn-submit-mockup:disabled {
  background-color: #e5e7eb;
  color: #9ca3af;
  cursor: not-allowed;
  opacity: 1;
}

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
  width: 30px;
  height: 30px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background-color: white;
  cursor: pointer;
  background-position: center;
  background-repeat: no-repeat;
  background-size: 14px;
  transition: background-color 0.15s ease;
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

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.btn-disabled-main {
  opacity: 0.45;
  cursor: not-allowed;
}

.modal-container {
  width: 650px;
  max-width: 95%;
  background: white;
  border-radius: 18px;
  padding: 24px;
  box-shadow: 0 15px 35px rgba(0,0,0,.12);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #e5e7eb;
  padding-bottom: 12px;
}

.modal-header h3 { margin: 0; font-size: 1.1rem; font-weight: 800; color: #111827; }
.btn-close { border: none; background: transparent; font-size: 22px; color: #6b7280; cursor: pointer; }

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-top: 14px;
}

.form-grid input,
.form-grid select {
  height: 40px;
  padding: 8px 12px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  font-size: 0.9rem;
  outline: none;
  box-sizing: border-box;
}
.form-grid input:focus, .form-grid select:focus { border-color: #9ca3af; }

.full-width { grid-column: 1 / -1; }
.modal-actions {
  grid-column: 1 / -1;
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 8px;
}

.btn-cancel {
  padding: 10px 18px;
  border: none;
  border-radius: 10px;
  background: #e5e7eb;
  color: #374151;
  font-weight: 700;
  cursor: pointer;
  font-size: 0.9rem;
}

.detalle-solicitud-wrapper strong { display: block; font-size: 0.8rem; color: #6b7280; text-transform: uppercase; margin-bottom: 2px; }
.detalle-solicitud-wrapper p { color: #111827; font-size: 0.95rem; font-weight: 500; margin: 0; }

.toast-error-moderno {
  position: fixed;
  top: 20px;
  right: 20px;
  background: white;
  border-left: 4px solid #166534;
  padding: 16px;
  border-radius: 10px;
  box-shadow: 0 10px 20px rgba(0,0,0,.1);
  z-index: 1000;
  min-width: 320px;
}
.toast-title { font-weight: 800; color: #111827; font-size: 0.85rem; }
.toast-text { color: #6b7280; font-size: 0.85rem; margin-top: 4px; }
.btn-close-toast { background: transparent; border: none; font-size: 1.2rem; color: #6b7280; cursor: pointer; position: absolute; top: 10px; right: 12px; }

.dashboard-widgets-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
  margin-top: 24px;
}

.widget-card-panel {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 20px;
}

.widget-card-panel h4 {
  margin: 0 0 16px 0;
  font-size: 1.05rem;
  font-weight: 700; 
  color: #111827;
  font-family: inherit; 
}

.btn-new-solicitud-trigger {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.search-wrapper {
  position: relative;
  flex: 1;
  max-width: 550px;
  min-width: 320px;
}

.search-icon {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  color: #9ca3af;
  pointer-events: none;
}

.search-box {
  width: 100%;
  height: 40px;
  padding: 0 14px 0 42px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background: #ffffff;
  font-size: .9rem;
}

.search-box:focus {
  outline: none;
  border-color: #9ca3af;
  box-shadow: 0 0 0 3px rgba(17, 24, 39, 0.05);
}

.feed-item-info h5 {
  margin: 0;
  font-size: 0.92rem;
  font-weight: 600; 
  color: #111827;
  font-family: inherit;
}

.feed-item-info p {
  margin: 2px 0 0 0;
  font-size: 0.82rem;
  color: #6b7280;
  font-family: inherit;
  line-height: 1.4; 
}

.feed-time-text {
  font-size: 0.8rem;
  color: #9ca3af;
  white-space: nowrap;
  font-family: inherit;
}

.feed-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.feed-item-row {
  display: flex;
  align-items: center;
  gap: 12px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f3f4f6;
}

.feed-item-row:last-child,
.feed-item-row-complex:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

.feed-status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}
.dot-dispo { background-color: #22c55e; }
.dot-nodispo { background-color: #3b82f6; }

.feed-item-info {
  flex: 1;
}

.feed-item-row-complex {
  display: flex;
  align-items: center;
  gap: 14px;
  padding-bottom: 12px;
  border-bottom: 1px solid #f3f4f6;
}

.feed-icon-wrapper {
  width: 36px;
  height: 36px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.icon-bg-purple { background-color: #eedffc; }
.icon-bg-yellow { background-color: #fef08a; }
.icon-bg-red { background-color: #fee2e2; }

.icon-placeholder-text {
  font-size: 1.1rem;
}

.feed-clickable {
  cursor: pointer;
  transition: background-color 0.2s ease, transform 0.1s ease;
  padding: 8px 12px; 
  border-radius: 8px;
}

.feed-clickable:hover {
  background-color: #f9fafb; 
}

.feed-clickable:active {
  transform: scale(0.99); 
}

.modal-container-historial {
  background: white;
  width: 95vw;
  max-width: 1100px;
  max-height: 88vh;
  border-radius: 18px;
  padding: 24px;
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

.mockup-select {
  height: 40px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  padding: 0 12px;
  font-size: 0.9rem;
  background: #fff;
  outline: none;
}
</style>