<template>
  <div class="section-header-mockup">
    <h2>Mantenimiento</h2>
    <p>Administra y gestiona las asignaciones</p>
  </div>

  <div class="filters-bar-mockup">
    <div class="search-filter-wrapper">
      <input type="text" placeholder="Buscar mantenimientos..." class="filter-search-input" v-model="filtroBusqueda" />
      <img src="../../assets/icons/search.png" class="filter-search-icon" alt="Buscar" />
    </div>

    <div class="filter-group-item">
      <label>Estado</label>
      <select class="mockup-select" v-model="filtroEstado">
        <option value="">Todos los estados</option>
        <option value="1">Programado</option>
        <option value="2">Pendiente</option>
        <option value="3">En progreso</option>
        <option value="4">Completado</option>
        <option value="5">Vencido</option>
      </select>
    </div>

    <div class="filter-group-item">
      <label>Tipo</label>
      <select class="mockup-select" v-model="filtroTipo">
        <option value="">Todos los tipos</option>
        <option value="1">Preventivo</option>
        <option value="2">Correctivo</option>
        <option value="3">Predictivo</option>
      </select>
    </div>

    <div class="filter-group-item">
      <label>Vehículo</label>
      <select class="mockup-select" v-model="filtroVehiculo">
        <option value="">Todos los vehículos</option>
        <option v-for="vehiculo in vehiculos" :key="vehiculo.id" :value="vehiculo.id">
          {{ vehiculo.marca }} {{ vehiculo.modelo }} - {{ vehiculo.matricula }}
        </option>
      </select>
    </div>

    <div class="filter-group-item">
      <label>Fecha de mantenimiento</label>
      <input type="date" class="mockup-date-input" v-model="filtroFecha" />
    </div>

    <button class="btn-filter-action" @click="limpiarFiltros">Limpiar Filtros</button>

    <button v-if="esAdmin" class="btn-new-solicitud-trigger" @click="abrirFormularioNuevo">
      Nuevo mantenimiento
    </button>
  </div>

  <div class="solicitudes-grid full-width-table">
    <section class="grid-left-table card-panel-mockup">
      <div class="table-responsive">
        <table class="custom-table-mockup">
          <thead>
            <tr>
              <th>Id</th>
              <th>Vehículo</th>
              <th>Tipo</th>
              <th>Descripción</th>
              <th>Fecha</th>
              <th>Estado</th>
              <th>Taller</th>
              <th>Costo</th>
              <th>Próximo mantenimiento</th>
              <th class="text-center" style="width: 100px;">Acciones</th>
            </tr>
          </thead>

          <tbody>
            <tr v-for="mantenimiento in mantenimientosFiltrados" :key="mantenimiento.id">
              <td>{{ mantenimiento.id }}</td>
              <td>{{ obtenerNombreVehiculo(mantenimiento.vehiculoId) }}</td>
              <td>{{ formatearTipoVista(mantenimiento.tipoMantenimiento) }}</td>
              <td>{{ mantenimiento.descripcion }}</td>
              <td>{{ formatearFechaVista(mantenimiento.fechaMantenimiento) }}</td>
              <td>
                <span :class="['status-pill-mockup', obtenerClaseEstado(mantenimiento.estado)]">
                  {{ formatearEstadoVista(mantenimiento.estado) }}
                </span>
              </td>
              <td>{{ mantenimiento.taller || '---' }}</td>
              <td>{{ formatearCosto(mantenimiento.costo) }}</td>
              <td>{{ formatearFechaVista(mantenimiento.proximoMantenimiento) }}</td>
              <td class="actions-cell-fixed">
                <div class="actions-wrapper">
                  <button class="action-btn-mockup icon-view" title="Ver detalle" @click="verDetalleMantenimiento(mantenimiento)"></button>
                  <button
                    class="action-btn-mockup icon-edit"
                    :class="{ 'btn-disabled': !esAdmin }"
                    :disabled="!esAdmin"
                    :title="esAdmin ? 'Editar' : 'No permitido para tu rol'"
                    @click="esAdmin && abrirFormularioEdicion(mantenimiento)"
                  ></button>
                </div>
              </td>
            </tr>

            <tr v-if="mantenimientosFiltrados.length === 0">
              <td colspan="10" class="text-center text-muted" style="padding: 30px;">
                No se encontraron mantenimientos con los filtros aplicados.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  </div>

  <!-- Modal Crear / Editar -->
  <div v-if="mostrarFormulario" class="modal-overlay-mockup" @click.self="cerrarFormulario">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>{{ modoEdicion ? 'Editar Mantenimiento #' + mantenimientoEditando.id : 'Registrar Nuevo Mantenimiento' }}</h3>
        <button class="btn-close-modal" @click="cerrarFormulario">×</button>
      </div>

      <form @submit.prevent="modoEdicion ? actualizarMantenimiento() : crearMantenimiento()" class="form-solicitud-mockup">
        <div class="form-group-mockup">
          <label>Vehículo</label>
          <select v-model.number="formModel.vehiculoId" class="mockup-select" required :disabled="modoEdicion">
            <option value="" disabled>Seleccione un vehículo</option>
            <option v-for="vehiculo in vehiculos" :key="vehiculo.id" :value="vehiculo.id">
              {{ vehiculo.marca }} {{ vehiculo.modelo }} - {{ vehiculo.matricula }}
            </option>
          </select>
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Tipo de Mantenimiento</label>
            <select v-model.number="formModel.tipoMantenimiento" class="mockup-select" required>
              <option :value="1">Preventivo</option>
              <option :value="2">Correctivo</option>
              <option :value="3">Predictivo</option>
            </select>
          </div>

          <div class="form-group-mockup" v-if="modoEdicion">
            <label>Estado</label>
            <select v-model.number="formModel.estado" class="mockup-select" required>
              <option :value="1">Programado</option>
              <option :value="2">Pendiente</option>
              <option :value="3">En progreso</option>
              <option :value="4">Completado</option>
              <option :value="5">Vencido</option>
            </select>
          </div>
        </div>

        <div class="form-group-mockup">
          <label>Fecha de Mantenimiento</label>
          <input type="datetime-local" v-model="formModel.fechaMantenimiento" required />
        </div>

        <div class="form-group-mockup">
          <label>Descripción</label>
          <textarea v-model="formModel.descripcion" placeholder="Ej. Cambio de aceite y filtros" rows="3" required></textarea>
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Costo</label>
            <input type="number" v-model.number="formModel.costo" min="0" step="0.01" required />
          </div>

          <div class="form-group-mockup">
            <label>Taller</label>
            <input type="text" v-model="formModel.taller" placeholder="Ej. Taller Central" required />
          </div>
        </div>

        <div class="form-group-mockup">
          <label>Próximo Mantenimiento</label>
          <input type="datetime-local" v-model="formModel.proximoMantenimiento" />
        </div>

        <div class="form-actions-central">
          <button type="button" class="btn-cancel-mockup" @click="cerrarFormulario">Cancelar</button>
          <button type="submit" class="btn-submit-mockup" :disabled="guardando">
            {{ guardando ? 'Enviando a la API...' : (modoEdicion ? 'Guardar Cambios' : 'Registrar Mantenimiento') }}
          </button>
        </div>
      </form>
    </div>
  </div>


  <div v-if="mostrarDetalle" class="modal-overlay-mockup" @click.self="mostrarDetalle = false">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>Detalle de Mantenimiento #{{ mantenimientoSeleccionado.id }}</h3>
        <button class="btn-close-modal" @click="mostrarDetalle = false">×</button>
      </div>

      <div class="detalle-solicitud-wrapper">
        <div class="detalle-item">
          <strong>Vehículo:</strong>
          <p>{{ obtenerNombreVehiculo(mantenimientoSeleccionado.vehiculoId) }}</p>
        </div>

        <div class="form-row-mockup">
          <div class="detalle-item">
            <strong>Tipo de Mantenimiento:</strong>
            <p>{{ formatearTipoVista(mantenimientoSeleccionado.tipoMantenimiento) }}</p>
          </div>

          <div class="detalle-item">
            <strong>Estado:</strong>
            <p>
              <span :class="['status-pill-mockup', obtenerClaseEstado(mantenimientoSeleccionado.estado)]">
                {{ formatearEstadoVista(mantenimientoSeleccionado.estado) }}
              </span>
            </p>
          </div>
        </div>

        <div class="detalle-item">
          <strong>Descripción:</strong>
          <p class="detalle-motivo-text">{{ mantenimientoSeleccionado.descripcion }}</p>
        </div>

        <div class="form-row-mockup">
          <div class="detalle-item">
            <strong>Fecha de Mantenimiento:</strong>
            <p>{{ formatearFechaVista(mantenimientoSeleccionado.fechaMantenimiento) }}</p>
          </div>

          <div class="detalle-item">
            <strong>Próximo Mantenimiento:</strong>
            <p>{{ formatearFechaVista(mantenimientoSeleccionado.proximoMantenimiento) }}</p>
          </div>
        </div>

        <div class="form-row-mockup" style="margin-top: 10px; border-top: 1px solid #e5e7eb; padding-top: 12px;">
          <div class="detalle-item">
            <strong>Taller:</strong>
            <p>{{ mantenimientoSeleccionado.taller || '---' }}</p>
          </div>

          <div class="detalle-item">
            <strong>Costo:</strong>
            <p>{{ formatearCosto(mantenimientoSeleccionado.costo) }}</p>
          </div>
        </div>
      </div>

      <div class="form-actions-central" style="margin-top: 20px;">
        <button
          v-if="esAdmin"
          type="button"
          class="btn-cancel-mockup"
          @click="mostrarDetalle = false; abrirFormularioEdicion(mantenimientoSeleccionado)"
        >
          Editar
        </button>
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


const rolUsuario = ref(localStorage.getItem('rol_usuario') || 'Operador')

// Corrección para validar los strings exactos que maneja el backend
const esAdmin = computed(() => rolUsuario.value === 'SuperAdmin' || rolUsuario.value === 'Administrador' || rolUsuario.value === 'Admin')

const filtroBusqueda = ref('')
const filtroEstado = ref('')
const filtroTipo = ref('')
const filtroVehiculo = ref('')
const filtroFecha = ref('')


const mantenimientos = ref([])
const vehiculos = ref([])
const guardando = ref(false)
const mensajeErrorFlotante = ref('')


const mostrarFormulario = ref(false)
const modoEdicion = ref(false)
const mantenimientoEditando = ref(null)

const formModelVacio = () => ({
  vehiculoId: '',
  tipoMantenimiento: 1,
  estado: 1,
  fechaMantenimiento: '',
  descripcion: '',
  costo: 0,
  taller: '',
  proximoMantenimiento: ''
})

const formModel = ref(formModelVacio())


const mostrarDetalle = ref(false)
const mantenimientoSeleccionado = ref({})


const aDatetimeLocal = (fechaIso) => {
  if (!fechaIso) return ''
  const fecha = new Date(fechaIso)
  const pad = (n) => String(n).padStart(2, '0')
  return `${fecha.getFullYear()}-${pad(fecha.getMonth() + 1)}-${pad(fecha.getDate())}T${pad(fecha.getHours())}:${pad(fecha.getMinutes())}`
}


const abrirFormularioNuevo = () => {
  modoEdicion.value = false
  formModel.value = formModelVacio()
  mostrarFormulario.value = true
}

const abrirFormularioEdicion = (mantenimiento) => {
  modoEdicion.value = true
  mantenimientoEditando.value = mantenimiento
  formModel.value = {
    vehiculoId: mantenimiento.vehiculoId,
    tipoMantenimiento: mantenimiento.tipoMantenimiento,
    estado: mantenimiento.estado,
    fechaMantenimiento: aDatetimeLocal(mantenimiento.fechaMantenimiento),
    descripcion: mantenimiento.descripcion,
    costo: mantenimiento.costo,
    taller: mantenimiento.taller,
    proximoMantenimiento: aDatetimeLocal(mantenimiento.proximoMantenimiento)
  }
  mostrarFormulario.value = true
}

const cerrarFormulario = () => {
  mostrarFormulario.value = false
  modoEdicion.value = false
  mantenimientoEditando.value = null
  formModel.value = formModelVacio()
}

const verDetalleMantenimiento = (mantenimiento) => {
  mantenimientoSeleccionado.value = { ...mantenimiento }
  mostrarDetalle.value = true
}

const fetchMantenimientosDeAPI = async () => {
  const token = localStorage.getItem('token_transporte')

  try {
    const response = await fetch('https://localhost:7221/api/Mantenimientos', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (response.ok) {
      mantenimientos.value = await response.json()
    }
  } catch (error) {
    console.error('Error cargando mantenimientos de la base de datos:', error)
  }
}

const fetchVehiculosDeAPI = async () => {
  const token = localStorage.getItem('token_transporte')

  try {
    const response = await fetch('https://localhost:7221/api/Vehiculos', {
      headers: {
        Authorization: `Bearer ${token}`
      }
    })

    if (response.ok) {
      vehiculos.value = await response.json()
    }
  } catch (error) {
    console.error('Error cargando vehículos de la base de datos:', error)
  }
}


const obtenerNombreVehiculo = (vehiculoId) => {
  const vehiculo = vehiculos.value.find(v => v.id === vehiculoId)
  if (!vehiculo) return `Vehículo #${vehiculoId}`
  return `${vehiculo.marca} ${vehiculo.modelo} - ${vehiculo.matricula}`
}

const mantenimientosFiltrados = computed(() => {
  return mantenimientos.value.filter(mantenimiento => {
    const cumpleBusqueda =
      !filtroBusqueda.value ||
      mantenimiento.descripcion?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
      mantenimiento.taller?.toLowerCase().includes(filtroBusqueda.value.toLowerCase())

    const cumpleEstado = !filtroEstado.value || mantenimiento.estado == filtroEstado.value
    const cumpleTipo = !filtroTipo.value || mantenimiento.tipoMantenimiento == filtroTipo.value
    const cumpleVehiculo = !filtroVehiculo.value || mantenimiento.vehiculoId == filtroVehiculo.value
    const cumpleFecha = !filtroFecha.value || mantenimiento.fechaMantenimiento?.includes(filtroFecha.value)

    return cumpleBusqueda && cumpleEstado && cumpleTipo && cumpleVehiculo && cumpleFecha
  })
})

const limpiarFiltros = () => {
  filtroBusqueda.value = ''
  filtroEstado.value = ''
  filtroTipo.value = ''
  filtroVehiculo.value = ''
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

const formatearCosto = (costo) => {
  if (costo === null || costo === undefined) return '---'
  return new Intl.NumberFormat('es-DO', { style: 'currency', currency: 'DOP' }).format(costo)
}

const formatearTipoVista = (tipo) => {
  const mapeoTipos = {
    1: 'Preventivo',
    2: 'Correctivo',
    3: 'Predictivo'
  }

  return mapeoTipos[tipo] || tipo || '---'
}

const formatearEstadoVista = (estado) => {
  const mapeoEstados = {
    1: 'Programado',
    2: 'Pendiente',
    3: 'En progreso',
    4: 'Completado',
    5: 'Vencido'
  }

  return mapeoEstados[estado] || estado || 'Programado'
}

const obtenerClaseEstado = (estado) => {
  const clases = {
    1: 'programado',
    2: 'pendiente',
    3: 'en-progreso',
    4: 'completado',
    5: 'vencido'
  }

  return clases[estado] || 'programado'
}


const validarFormulario = () => {
  if (!formModel.value.vehiculoId) {
    mostrarError('Debe seleccionar un vehículo.')
    return false
  }

  if (formModel.value.proximoMantenimiento) {
    const fechaMantenimiento = new Date(formModel.value.fechaMantenimiento)
    const proximoMantenimiento = new Date(formModel.value.proximoMantenimiento)

    if (proximoMantenimiento <= fechaMantenimiento) {
      mostrarError('La fecha del próximo mantenimiento debe ser posterior a la fecha del mantenimiento actual.')
      return false
    }
  }

  return true
}

const mostrarError = (mensaje) => {
  mensajeErrorFlotante.value = mensaje
  setTimeout(() => { mensajeErrorFlotante.value = '' }, 4000)
}


const crearMantenimiento = async () => {
  if (!validarFormulario()) return

  guardando.value = true

  const token = localStorage.getItem('token_transporte')
  const fecha = new Date(formModel.value.fechaMantenimiento)
  const proximo = formModel.value.proximoMantenimiento ? new Date(formModel.value.proximoMantenimiento) : null

  const payload = {
    vehiculoId: formModel.value.vehiculoId,
    fechaMantenimiento: fecha.toISOString(),
    tipoMantenimiento: formModel.value.tipoMantenimiento,
    descripcion: formModel.value.descripcion,
    costo: formModel.value.costo,
    taller: formModel.value.taller,
    proximoMantenimiento: proximo ? proximo.toISOString() : null
  }

  try {
    const response = await fetch('https://localhost:7221/api/Mantenimientos', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    })

    if (response.ok) {
      await fetchMantenimientosDeAPI()
      cerrarFormulario()
    } else {
      const errorResponse = await response.json()
      console.error('Error del Servidor:', errorResponse)
      mostrarError('Error en validación de datos: ' + JSON.stringify(errorResponse.errors || errorResponse))
    }
  } catch (error) {
    console.error('Error al conectar con la API:', error)
    mostrarError('No se pudo conectar con el servidor.')
  } finally {
    guardando.value = false
  }
}


const actualizarMantenimiento = async () => {
  if (!validarFormulario()) return
  if (!mantenimientoEditando.value) return

  guardando.value = true

  const token = localStorage.getItem('token_transporte')
  const fecha = new Date(formModel.value.fechaMantenimiento)
  const proximo = formModel.value.proximoMantenimiento ? new Date(formModel.value.proximoMantenimiento) : null

  const payload = {
    estado: formModel.value.estado,
    tipoMantenimiento: formModel.value.tipoMantenimiento,
    descripcion: formModel.value.descripcion,
    costo: formModel.value.costo,
    taller: formModel.value.taller,
    proximoMantenimiento: proximo ? proximo.toISOString() : null
  }

  try {
    const response = await fetch(`https://localhost:7221/api/Mantenimientos/${mantenimientoEditando.value.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    })

    if (response.ok) {
      await fetchMantenimientosDeAPI()
      cerrarFormulario()
    } else {
      const errorResponse = await response.json()
      console.error('Error del Servidor:', errorResponse)
      mostrarError('Error en validación de datos: ' + JSON.stringify(errorResponse.errors || errorResponse))
    }
  } catch (error) {
    console.error('Error al conectar con la API:', error)
    mostrarError('No se pudo conectar con el servidor.')
  } finally {
    guardando.value = false
  }
}

onMounted(() => {
  fetchVehiculosDeAPI()
  fetchMantenimientosDeAPI()
})
</script>

<style>

.programado {
  background: #e0e7ff;
  color: #3730a3;
}

.en-progreso {
  background: #dbeafe;
  color: #1e40af;
}

.completado {
  background: #dcfce7;
  color: #166534;
}

.vencido {
  background: #fee2e2;
  color: #991b1b;
}
</style>