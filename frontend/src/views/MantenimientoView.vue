<template>
  <div class="dashboard-header">
    <div class="dashboard-header-left">
      <h2>Mantenimiento</h2>
      <p>Administra servicios, revisiones y estado de mantenimiento de la flota.</p>
    </div>

    <div class="dashboard-header-badge">
      {{ cardPendientes }} pendientes
    </div>
  </div>

  <div class="mant-cards-row">
    <div class="mant-card">
      <div class="mant-card-icon mant-icon-hoy">
        <Wrench :size="24" />
      </div>
      <div class="mant-card-info">
        <span>De hoy</span>
        <h3>{{ cardDeHoy }}</h3>
      </div>
    </div>

    <div class="mant-card">
      <div class="mant-card-icon mant-icon-aldia">
        <CircleCheck :size="24" />
      </div>
      <div class="mant-card-info">
        <span>Al día</span>
        <h3>{{ cardAlDia }}</h3>
      </div>
    </div>

    <div class="mant-card">
      <div class="mant-card-icon mant-icon-pendiente">
        <AlertTriangle :size="24" />
      </div>
      <div class="mant-card-info">
        <span>Pendientes</span>
        <h3>{{ cardPendientes }}</h3>
      </div>
    </div>

    <div class="mant-card">
      <div class="mant-card-icon mant-icon-vencido">
        <CalendarClock :size="24" />
      </div>
      <div class="mant-card-info">
        <span>Vencidos</span>
        <h3>{{ cardVencidos }}</h3>
      </div>
    </div>

    <div class="mant-card">
      <div class="mant-card-icon mant-icon-completado">
        <CheckCheck :size="24" />
      </div>
      <div class="mant-card-info">
        <span>Completados (mes)</span>
        <h3>{{ cardCompletadosMes }}</h3>
      </div>
    </div>
  </div>

  <div class="mant-filter-container-mockup">
    <div class="mant-filter-field">
      <label class="mant-filter-label">Filtrar por Vehículo</label>
      <div class="mant-input-search-wrapper">
        <input
          type="text"
          class="mant-inline-input"
          placeholder="Buscar..."
          v-model="filtroBusqueda"
        />
        <Search :size="16" class="mant-search-inline-icon" />
      </div>
    </div>

    <div class="mant-filter-field">
      <label class="mant-filter-label">Rango de fechas</label>
      <div class="mant-date-range-wrapper">
        <input type="date" class="mant-date-inline" v-model="filtroFechaInicio" />
        <span class="mant-date-separator">-</span>
        <input type="date" class="mant-date-inline" v-model="filtroFechaFin" />
      </div>
    </div>

    <button class="btn-filter-action" @click="limpiarFiltros">Limpiar Filtros</button>

    <button
      class="mant-btn-agregar-black"
      :class="{ 'mant-btn-disabled': !puedeModificarMantenimiento }"
      :disabled="!puedeModificarMantenimiento"
      @click="abrirFormularioNuevo"
    >
      Agregar
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
              <th class="text-center" style="width: 120px;">Acciones</th>
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
                  <button
                    class="action-btn-mockup"
                    title="Ver detalle"
                    @click="verDetalleMantenimiento(mantenimiento)"
                  >
                    <Eye :size="15" />
                  </button>

                  <button
                    class="action-btn-mockup"
                    :class="{ 'btn-disabled': !puedeModificarMantenimientos }"
                    :disabled="!puedeModificarMantenimientos"
                    title="Editar mantenimiento"
                    @click="abrirEditarMantenimiento(mantenimiento)"
                  >
                    <Pencil :size="15" />
                  </button>

                  <button
                    class="action-btn-mockup"
                    :class="{ 'btn-disabled': !puedeEliminarMantenimientos }"
                    :disabled="!puedeEliminarMantenimientos"
                    title="Eliminar mantenimiento"
                    @click="eliminarMantenimiento(mantenimiento.id)"
                  >
                    <Trash2 :size="15" />
                  </button>
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


  <div class="mant-dashboard-row">


    <div class="mant-chart-card">
      <span class="mant-chart-card-title">Mantenimientos (este mes)</span>
      <div class="mant-card-body-graphic">
        <div class="mant-donut-chart" :style="{ background: donutGradient }">
          <div class="mant-donut-center"></div>
        </div>
        <div class="mant-graphic-legends">
          <div class="mant-legend-item">
            <span class="legend-color preventivo-bg"></span>
            <span class="legend-text">Preventivo ({{ conteoTipos.preventivos }})</span>
          </div>
          <div class="mant-legend-item">
            <span class="legend-color correctivo-bg"></span>
            <span class="legend-text">Correctivo ({{ conteoTipos.correctivos }})</span>
          </div>
          <div class="mant-legend-item">
            <span class="legend-color predictivo-bg"></span>
            <span class="legend-text">Predictivo ({{ conteoTipos.predictivos }})</span>
          </div>
        </div>
      </div>
    </div>


    <div class="mant-chart-card">
      <span class="mant-chart-card-title">Costos (últimos 6 meses)</span>
      <div class="mant-card-body-bars">
        <div class="mant-bars-chart-mock">
          <div class="mant-y-axis">
            <span>$20,000</span>
            <span>$10,000</span>
            <span>$5,000</span>
            <span>$0</span>
          </div>
          <div class="mant-bars-container">
            <div class="mant-bar-wrapper" v-for="(mes, idx) in costosUltimosMeses" :key="idx">
              <div class="mant-bar-pill" :style="{ height: mes.porcentaje + '%' }"></div>
              <span class="mant-bar-label">{{ mes.nombre }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>


    <div class="mant-chart-card">
      <span class="mant-chart-card-title">Próximos</span>
      <div class="mant-proximos-list-container">
        <div
          class="mant-proximo-row-item"
          v-for="prox in proximosMantenimientos"
          :key="prox.id"
        >
          <div class="mant-proximo-info">
            <span class="mant-prox-fecha">{{ formatearFechaCorta(prox.proximoMantenimiento) }} - {{ prox.descripcion || 'Revisión General' }}</span>
            <span class="mant-prox-vehiculo">{{ obtenerNombreVehiculo(prox.vehiculoId) }}</span>
          </div>
          <span class="mant-prox-tag" :class="obtenerClaseEstadoTipo(prox.tipoMantenimiento)">
            {{ formatearTipoVista(prox.tipoMantenimiento) }}
          </span>
        </div>
        <div v-if="proximosMantenimientos.length === 0" class="mant-no-data">
          No hay mantenimientos programados
        </div>
      </div>
    </div>

  </div>


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
        >Editar</button>
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


  <div v-if="mensajeAvisoFlotante" class="toast-error-moderno">
  <div class="toast-content">
    <span class="toast-title">Notificación</span>
    <p class="toast-text">{{ mensajeAvisoFlotante }}</p>
  </div>
  <button class="btn-close-toast" @click="mensajeAvisoFlotante = ''">×</button>
</div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

import {
  Wrench,
  CircleCheck,
  AlertTriangle,
  CalendarClock,
  CheckCheck,
  Search,
  Eye,
  Pencil,
  Trash2
} from 'lucide-vue-next'


const rolUsuario = ref(localStorage.getItem('usuario_rol') || 'Operador')

const puedeModificarMantenimiento = computed(() => {
  const rol = rolUsuario.value.toLowerCase()
  return rol === 'administrador' || rol === 'admin' || rol === 'superadmin'
})

const esAdmin = computed(() => puedeModificarMantenimiento.value)


const filtroBusqueda    = ref('')
const filtroEstado      = ref('')
const filtroTipo        = ref('')
const filtroVehiculo    = ref('')
const filtroFecha       = ref('')
const filtroFechaInicio = ref('')
const filtroFechaFin    = ref('')


const mantenimientos       = ref([])
const vehiculos            = ref([])
const guardando            = ref(false)
const mensajeErrorFlotante = ref('')
const mensajeAvisoFlotante = ref('')

const hoy       = new Date()
const inicioMes = new Date(hoy.getFullYear(), hoy.getMonth(), 1)


const cardDeHoy = computed(() =>
  mantenimientos.value.filter(m => new Date(m.fechaMantenimiento).toDateString() === hoy.toDateString()).length
)
const cardAlDia = computed(() =>
  mantenimientos.value.filter(m => m.estado === 4).length
)
const cardPendientes = computed(() =>
  mantenimientos.value.filter(m => m.estado === 2).length
)
const cardVencidos = computed(() =>
  mantenimientos.value.filter(m => m.estado === 5).length
)
const cardCompletadosMes = computed(() =>
  mantenimientos.value.filter(m => m.estado === 4 && new Date(m.fechaMantenimiento) >= inicioMes).length
)


const mostrarFormulario     = ref(false)
const modoEdicion           = ref(false)
const mantenimientoEditando = ref(null)

const formModelVacio = () => ({
  vehiculoId:           '',
  tipoMantenimiento:    1,
  estado:               1,
  fechaMantenimiento:   '',
  descripcion:          '',
  costo:                0,
  taller:               '',
  proximoMantenimiento: ''
})

const formModel = ref(formModelVacio())


const mostrarDetalle            = ref(false)
const mantenimientoSeleccionado = ref({})


const aDatetimeLocal = (fechaIso) => {
  if (!fechaIso) return ''
  const f   = new Date(fechaIso)
  const pad = (n) => String(n).padStart(2, '0')
  return `${f.getFullYear()}-${pad(f.getMonth()+1)}-${pad(f.getDate())}T${pad(f.getHours())}:${pad(f.getMinutes())}`
}


const abrirFormularioNuevo = () => {
  modoEdicion.value       = false
  formModel.value         = formModelVacio()
  mostrarFormulario.value = true
}

const abrirFormularioEdicion = (mantenimiento) => {
  modoEdicion.value           = true
  mantenimientoEditando.value = mantenimiento
  formModel.value = {
    vehiculoId:           mantenimiento.vehiculoId,
    tipoMantenimiento:    mantenimiento.tipoMantenimiento,
    estado:               mantenimiento.estado,
    fechaMantenimiento:   aDatetimeLocal(mantenimiento.fechaMantenimiento),
    descripcion:          mantenimiento.descripcion,
    costo:                mantenimiento.costo,
    taller:               mantenimiento.taller,
    proximoMantenimiento: aDatetimeLocal(mantenimiento.proximoMantenimiento)
  }
  mostrarFormulario.value = true
}

const cerrarFormulario = () => {
  mostrarFormulario.value     = false
  modoEdicion.value           = false
  mantenimientoEditando.value = null
  formModel.value             = formModelVacio()
}

const verDetalleMantenimiento = (mantenimiento) => {
  mantenimientoSeleccionado.value = { ...mantenimiento }
  mostrarDetalle.value = true
}

const mostrarAvisoEliminacion = () => {
  mensajeAvisoFlotante.value = 'No se pudo eliminar el registro.'
  
  setTimeout(() => { 
    mensajeAvisoFlotante.value = '' 
  }, 5000)
}


const fetchMantenimientosDeAPI = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const response = await fetch('https://localhost:7221/api/Mantenimientos', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (response.ok) mantenimientos.value = await response.json()
  } catch (error) {
    console.error('Error cargando mantenimientos:', error)
  }
}

const fetchVehiculosDeAPI = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const response = await fetch('https://localhost:7221/api/Vehiculos', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (response.ok) vehiculos.value = await response.json()
  } catch (error) {
    console.error('Error cargando vehículos:', error)
  }
}


const obtenerNombreVehiculo = (vehiculoId) => {
  const v = vehiculos.value.find(v => v.id === vehiculoId)
  if (!v) return `Vehículo #${vehiculoId}`
  return `${v.marca} ${v.modelo} - ${v.matricula}`
}

const mantenimientosFiltrados = computed(() =>
  mantenimientos.value.filter(m => {
    const vehiculo = vehiculos.value.find(v => v.id === m.vehiculoId)
    const textoVehiculo = vehiculo
      ? `${vehiculo.marca} ${vehiculo.modelo} ${vehiculo.matricula}`.toLowerCase()
      : ''

    const cumpleBusqueda = !filtroBusqueda.value ||
      textoVehiculo.includes(filtroBusqueda.value.toLowerCase()) ||
      m.descripcion?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
      m.taller?.toLowerCase().includes(filtroBusqueda.value.toLowerCase())

    let cumpleRangoFecha = true
    if (m.fechaMantenimiento) {
      const f = m.fechaMantenimiento.split('T')[0]
      if (filtroFechaInicio.value && f < filtroFechaInicio.value) cumpleRangoFecha = false
      if (filtroFechaFin.value   && f > filtroFechaFin.value)   cumpleRangoFecha = false
    }

    const cumpleEstado = !filtroEstado.value || m.estado            == filtroEstado.value
    const cumpleTipo   = !filtroTipo.value   || m.tipoMantenimiento == filtroTipo.value

    return cumpleBusqueda && cumpleRangoFecha && cumpleEstado && cumpleTipo
  })
)

const limpiarFiltros = () => {
  filtroBusqueda.value    = ''
  filtroEstado.value      = ''
  filtroTipo.value        = ''
  filtroVehiculo.value    = ''
  filtroFecha.value       = ''
  filtroFechaInicio.value = ''
  filtroFechaFin.value    = ''
}


const conteoTipos = computed(() => {
  const actuales = mantenimientos.value.filter(m => {
    const f = new Date(m.fechaMantenimiento)
    return f.getMonth() === hoy.getMonth() && f.getFullYear() === hoy.getFullYear()
  })
  const preventivos = actuales.filter(m => m.tipoMantenimiento === 1).length
  const correctivos = actuales.filter(m => m.tipoMantenimiento === 2).length
  const predictivos = actuales.filter(m => m.tipoMantenimiento === 3).length
  return { preventivos, correctivos, predictivos, total: preventivos + correctivos + predictivos }
})


const donutGradient = computed(() => {
  const { preventivos, correctivos, predictivos, total } = conteoTipos.value
  if (total === 0) return 'conic-gradient(#e5e7eb 0% 100%)'
  const p1 = (preventivos / total) * 100
  const p2 = p1 + (correctivos / total) * 100
  return `conic-gradient(#16a34a 0% ${p1}%, #ea580c ${p1}% ${p2}%, #2563eb ${p2}% 100%)`
})

const proximosMantenimientos = computed(() =>
  mantenimientos.value
    .filter(m => m.proximoMantenimiento && new Date(m.proximoMantenimiento) >= hoy)
    .sort((a, b) => new Date(a.proximoMantenimiento) - new Date(b.proximoMantenimiento))
    .slice(0, 4)
)

const costosUltimosMeses = computed(() => {
  const nombres = ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic']
  const resultado = []
  for (let i = 5; i >= 0; i--) {
    const d    = new Date(hoy.getFullYear(), hoy.getMonth() - i, 1)
    const mNum = d.getMonth()
    const anio = d.getFullYear()
    const total = mantenimientos.value
      .filter(m => { const f = new Date(m.fechaMantenimiento); return f.getMonth() === mNum && f.getFullYear() === anio })
      .reduce((sum, m) => sum + (m.costo || 0), 0)
    resultado.push({ nombre: nombres[mNum], total, porcentaje: 0 })
  }
  const TOPE = 20000
  return resultado.map(r => ({ ...r, porcentaje: Math.max((r.total / TOPE) * 100, r.total > 0 ? 6 : 2) }))
})


const formatearFechaVista = (fechaIso) => {
  if (!fechaIso) return '---'
  return new Date(fechaIso).toLocaleString('es-DO', {
    hour12: true, year: 'numeric', month: '2-digit',
    day: '2-digit', hour: '2-digit', minute: '2-digit'
  })
}

const formatearFechaCorta = (fechaIso) => {
  if (!fechaIso) return '---'
  return new Date(fechaIso).toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

const formatearCosto = (costo) => {
  if (costo === null || costo === undefined) return '---'
  return new Intl.NumberFormat('es-DO', { style: 'currency', currency: 'DOP' }).format(costo)
}

const formatearTipoVista = (tipo) =>
  ({ 1: 'Preventivo', 2: 'Correctivo', 3: 'Predictivo' }[tipo] || '---')

const formatearEstadoVista = (estado) =>
  ({ 1: 'Programado', 2: 'Pendiente', 3: 'En progreso', 4: 'Completado', 5: 'Vencido' }[estado] || 'Programado')

const obtenerClaseEstado = (estado) =>
  ({ 1: 'programado', 2: 'pendiente', 3: 'en-progreso', 4: 'completado', 5: 'vencido' }[estado] || 'programado')

const obtenerClaseEstadoTipo = (tipo) =>
  ({ 1: 'tag-preventivo', 2: 'tag-correctivo', 3: 'tag-predictivo' }[tipo] || 'tag-preventivo')


const mostrarError = (mensaje) => {
  mensajeErrorFlotante.value = mensaje
  setTimeout(() => { mensajeErrorFlotante.value = '' }, 4000)
}

const validarFormulario = () => {
  if (!formModel.value.vehiculoId) {
    mostrarError('Debe seleccionar un vehículo.')
    return false
  }
  if (formModel.value.proximoMantenimiento) {
    const fechaMant = new Date(formModel.value.fechaMantenimiento)
    const fechaProx = new Date(formModel.value.proximoMantenimiento)
    if (fechaProx <= fechaMant) {
      mostrarError('La fecha del próximo mantenimiento debe ser posterior a la fecha actual del mantenimiento.')
      return false
    }
  }
  return true
}

const crearMantenimiento = async () => {
  if (!validarFormulario()) return
  guardando.value = true
  const token   = localStorage.getItem('token_transporte')
  const fecha   = new Date(formModel.value.fechaMantenimiento)
  const proximo = formModel.value.proximoMantenimiento ? new Date(formModel.value.proximoMantenimiento) : null

  const payload = {
    vehiculoId:           formModel.value.vehiculoId,
    fechaMantenimiento:   fecha.toISOString(),
    tipoMantenimiento:    formModel.value.tipoMantenimiento,
    descripcion:          formModel.value.descripcion,
    costo:                formModel.value.costo,
    taller:               formModel.value.taller,
    proximoMantenimiento: proximo ? proximo.toISOString() : null
  }

  try {
    const response = await fetch('https://localhost:7221/api/Mantenimientos', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify(payload)
    })
    if (response.ok) {
      await fetchMantenimientosDeAPI()
      cerrarFormulario()
    } else {
      const err = await response.json()
      mostrarError('Error: ' + JSON.stringify(err.errors || err))
    }
  } catch {
    mostrarError('No se pudo conectar con el servidor.')
  } finally {
    guardando.value = false
  }
}

const actualizarMantenimiento = async () => {
  if (!validarFormulario()) return
  if (!mantenimientoEditando.value) return
  guardando.value = true
  const token   = localStorage.getItem('token_transporte')
  const proximo = formModel.value.proximoMantenimiento ? new Date(formModel.value.proximoMantenimiento) : null

  const payload = {
    estado:               formModel.value.estado,
    tipoMantenimiento:    formModel.value.tipoMantenimiento,
    descripcion:          formModel.value.descripcion,
    costo:                formModel.value.costo,
    taller:               formModel.value.taller,
    proximoMantenimiento: proximo ? proximo.toISOString() : null
  }

  try {
    const response = await fetch(`https://localhost:7221/api/Mantenimientos/${mantenimientoEditando.value.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify(payload)
    })
    if (response.ok) {
      await fetchMantenimientosDeAPI()
      cerrarFormulario()
    } else {
      const err = await response.json()
      mostrarError('Error: ' + JSON.stringify(err.errors || err))
    }
  } catch {
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

.mant-cards-row {
  display: flex;
  gap: 16px;
  margin-bottom: 24px;
  flex-wrap: wrap;
}

.mant-card {
  flex: 1;
  min-width: 140px;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 18px 20px;
  display: flex;
  align-items: center;
  gap: 14px;
}

.mant-card-icon {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  background: #f3f4f6 !important;
  color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.mant-card-icon img { width: 20px; height: 20px; object-fit: contain; }

.mant-icon-hoy        { background: #eff6ff; }
.mant-icon-aldia      { background: #dcfce7; }
.mant-icon-pendiente  { background: #fef3c7; }
.mant-icon-vencido    { background: #fee2e2; }
.mant-icon-completado { background: #f3e8ff; }

.mant-card-info { display: flex; flex-direction: column; gap: 2px; }

.mant-card-info {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.mant-card-info span {
  font-size: 0.82rem;
  color: #6b7280;
  font-weight: 500;
  text-transform: none; /* Quita las mayúsculas toscas */
  letter-spacing: normal;
}

.mant-card-info h3 {
  font-size: 1.3rem;
  font-weight: 700;
  color: #111827;
  margin: 0;
  line-height: 1.2;
}

.menu-icon {
  width: 20px;
  height: 20px;
  object-fit: contain;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 16px;
  margin-bottom: 24px;
}

.dashboard-header-left h2 {
  margin: 0;
}

.dashboard-header-left p {
  margin: 6px 0 0;
}

.dashboard-header-badge {
  flex-shrink: 0;
  background: #fef3c7;
  color: #92400e;
  border: 1px solid #fde68a;
  border-radius: 999px;
  padding: 8px 14px;
  font-size: 0.82rem;
  font-weight: 700;
  white-space: nowrap;
}

.programado  { background: #e0e7ff; color: #3730a3; }
.en-progreso { background: #dbeafe; color: #1e40af; }
.completado  { background: #dcfce7; color: #166534; }
.vencido     { background: #fee2e2; color: #991b1b; }


.toast-aviso-moderno {
  position: fixed; top: 20px; right: 20px;
  background: white; border-left: 4px solid #2563eb;
  padding: 16px; border-radius: 10px;
  box-shadow: 0 10px 20px rgba(0,0,0,.1);
  z-index: 1000; min-width: 340px; max-width: 420px;
}
.toast-aviso-moderno .toast-title { font-weight: 800; color: #1e40af; font-size: 0.85rem; }
.toast-aviso-moderno .toast-text  { color: #374151; font-size: 0.85rem; margin-top: 4px; }


.mant-filter-container-mockup {
  display: flex; align-items: center;
  background: #ffffff; border: 1px solid #d1d5db;
  border-radius: 30px; padding: 8px 24px;
  gap: 16px; width: 100%; box-sizing: border-box;
  margin-bottom: 24px; box-shadow: 0 2px 4px rgba(0,0,0,0.02);
}

.mant-filter-field { display: flex; align-items: center; gap: 12px; }

.mant-filter-label { font-size: 0.88rem; font-weight: 600; color: #111827; white-space: nowrap; }

.mant-input-search-wrapper {
  position: relative;
}

.mant-inline-input {
  border: 1px solid #cccccc; border-radius: 20px;
  padding: 4px 36px 4px 14px; font-size: 0.88rem;
  width: 130px; outline: none; height: 36px;
  box-sizing: border-box; background: #ffffff;
}
.mant-inline-input:focus { border-color: #9ca3af; }

.mant-search-inline-icon {
  position: absolute;
  right: 14px;
  top: 50%;
  transform: translateY(-50%);
  color: #9ca3af;
  pointer-events: none;
}

.mant-date-range-wrapper {
  display: flex; align-items: center;
  border: 1px solid #cccccc; border-radius: 20px;
  padding: 0 14px; background: #ffffff;
  height: 36px; box-sizing: border-box;
}

.mant-date-inline {
  border: none; outline: none; font-size: 0.88rem;
  color: #374151; background: transparent;
  font-family: inherit; cursor: pointer; width: 140px;
}

.mant-date-separator { color: #9ca3af; padding: 0 6px; font-size: 0.85rem; }

.mant-filter-container-mockup .btn-filter-action {
  margin-left: auto; height: 36px;
  display: flex; align-items: center; justify-content: center; box-sizing: border-box;
}

.mant-btn-agregar-black {
  background: #111827; color: #ffffff; border: none;
  padding: 0 18px; border-radius: 10px; cursor: pointer;
  font-size: 0.9rem; font-weight: 700; height: 36px;
  display: flex; align-items: center; justify-content: center;
  white-space: nowrap; transition: opacity 0.2s;
}
.mant-btn-agregar-black:hover:not(:disabled) { opacity: 0.88; }

.mant-btn-disabled { opacity: 0.35 !important; cursor: not-allowed !important; }

.mant-dashboard-row {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 20px;
  margin-top: 24px;
}

.mant-chart-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 20px 22px 28px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.mant-chart-card-title {
  font-size: 0.95rem;
  font-weight: 800;
  color: #111827;
}


.mant-card-body-graphic {
  display: flex;
  align-items: center;
  gap: 28px;
}


.mant-donut-chart {
  width: 120px;
  height: 120px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: background 0.4s ease;
}

.mant-donut-center {
  width: 78px;
  height: 78px;
  background: #ffffff;
  border-radius: 50%;
}

.mant-graphic-legends { display: flex; flex-direction: column; gap: 14px; }

.mant-legend-item { display: flex; align-items: center; gap: 10px; }

.legend-color { width: 14px; height: 14px; border-radius: 3px; flex-shrink: 0; }

.preventivo-bg { background: #16a34a; }
.correctivo-bg { background: #ea580c; }
.predictivo-bg { background: #2563eb; }

.legend-text { font-size: 0.85rem; font-weight: 600; color: #374151; }


.mant-card-body-bars { flex: 1; display: flex; align-items: flex-end; }

.mant-bars-chart-mock {
  display: flex; gap: 8px;
  width: 100%; align-items: flex-end; height: 170px;
}

.mant-y-axis {
  display: flex; flex-direction: column; justify-content: space-between;
  height: 130px; font-size: 0.68rem; color: #9ca3af;
  font-weight: 600; text-align: right; width: 48px;
  flex-shrink: 0; margin-bottom: 26px;
}

.mant-bars-container {
  display: flex; justify-content: space-between;
  align-items: flex-end; flex: 1;
  height: 130px;
  border-bottom: 1px solid #d1d5db;
  border-left: 1px solid #d1d5db;
  position: relative; margin-bottom: 26px; padding: 0 6px;
}

.mant-bar-wrapper {
  display: flex; flex-direction: column;
  align-items: center; justify-content: flex-end;
  flex: 1; height: 100%; position: relative;
}

.mant-bar-pill {
  width: 55%; max-width: 26px;
  background: #1f2937;
  border-radius: 3px 3px 0 0;
  min-height: 3px; transition: height 0.4s ease;
}

.mant-bar-label {
  position: absolute; bottom: -22px;
  font-size: 0.69rem; color: #6b7280;
  font-weight: 600; white-space: nowrap;
}


.mant-proximos-list-container {
  display: flex; flex-direction: column;
  overflow-y: auto; max-height: 200px;
}

.mant-proximo-row-item {
  display: flex; align-items: center;
  justify-content: space-between; gap: 10px;
  padding: 10px 0; border-bottom: 1px solid #f3f4f6;
}
.mant-proximo-row-item:last-child { border-bottom: none; }

.mant-proximo-info {
  display: flex; flex-direction: column; gap: 3px;
  min-width: 0; flex: 1;
}

.mant-prox-fecha {
  font-size: 0.8rem; font-weight: 700; color: #111827;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}

.mant-prox-vehiculo {
  font-size: 0.75rem; color: #6b7280; font-weight: 500;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}

.mant-prox-tag {
  font-size: 0.72rem; font-weight: 700;
  padding: 4px 12px; border-radius: 6px; white-space: nowrap; flex-shrink: 0;
}

.actions-cell-fixed {
  text-align: center;
  vertical-align: middle;
}

.actions-wrapper {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.action-btn-mockup {
  width: 34px;
  height: 34px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background: #ffffff;
  color: #374151;
  cursor: pointer;

  display: inline-flex;
  align-items: center;
  justify-content: center;

  padding: 0;
  line-height: 1;
  transition: all 0.2s ease;
}

.action-btn-mockup svg {
  width: 15px;
  height: 15px;
  display: block;
}

.action-btn-mockup:hover:not(:disabled) {
  background: #111827;
  color: #ffffff;
  border-color: #111827;
}

.btn-disabled {
  opacity: 0.35;
  cursor: not-allowed;
}

.tag-preventivo { background: #dcfce7; color: #166534; }
.tag-correctivo { background: #fef3c7; color: #9a3412; }
.tag-predictivo { background: #dbeafe; color: #1e40af; }

.mant-no-data { font-size: 0.83rem; color: #9ca3af; text-align: center; padding: 40px 0 20px; }
</style>