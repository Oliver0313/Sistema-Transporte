<template>
  <div class="dashboard-header">
    <div class="dashboard-header-left">
      <h2>Gestión de Vehículos</h2>
      <p>Administra la flota, disponibilidad y estado operativo de cada unidad.</p>
    </div>

    <div class="dashboard-header-badge">
      {{ totalVehiculos }} vehículos registrados
    </div>
  </div>

  <div class="cards-kpi-container">
    <div class="card-kpi">
      <div class="card-kpi-icon total-icon">
        <CarFront :size="24" />
      </div>
      <div class="card-kpi-info">
        <span>Total vehículos</span>
        <h3>{{ totalVehiculos }}</h3>
      </div>
    </div>

    <div class="card-kpi">
    <div class="card-kpi-icon disponible-icon">
      <CircleCheck :size="24" />
    </div>
      <div class="card-kpi-info">
        <span>Disponibles</span>
        <h3>{{ disponiblesContador }}</h3>
      </div>
    </div>

    <div class="card-kpi">
      <div class="card-kpi-icon mantenimiento-icon">
        <Wrench :size="24" />
      </div>
      <div class="card-kpi-info">
        <span>En mantenimiento</span>
        <h3>{{ mantenimientoContador }}</h3>
      </div>
    </div>
    
    <div class="card-kpi">
      <div class="card-kpi-icon no-disponible-icon">
        <CircleX :size="24" />
      </div>
      <div class="card-kpi-info">
        <span>No disponibles</span>
        <h3>{{ noDisponiblesContador }}</h3>
      </div>
    </div>
  </div>

  <div class="quick-actions-bar">
    <h4>Acciones rápidas</h4>
    <div class="actions-wrapper-quick">
    <button
      class="btn-quick-action"
      :class="{ 'btn-disabled': !puedeModificarVehiculos }"
      :disabled="!puedeModificarVehiculos"
      @click="abrirFormularioModal"
    >
      <Plus :size="18" />
      <span>Registrar vehículo</span>
    </button>

      <button
        class="btn-quick-action"
        :class="{ 'btn-disabled': !puedeGestionarMantenimiento }"
        :disabled="!puedeGestionarMantenimiento"
        @click="abrirMantenimientoNuevo"
      >
        <Wrench :size="18" />
        <span>Registrar mantenimiento</span>
      </button>

      <button
        class="btn-quick-action"
        :class="{ 'btn-disabled': !puedeModificarVehiculos }"
        :disabled="!puedeModificarVehiculos"
        @click="abrirConsumoNuevo"
      >
        <Fuel :size="18" />
        <span>Registrar consumo</span>
      </button>

      <button
        class="btn-quick-action"
        @click="consultarDisponibilidadCalendario"
      >
        <CalendarDays :size="18" />
        <span>Consultar disponibilidad</span>
      </button>
    </div>
  </div>

  <div class="filters-bar-mockup">
    <div class="search-filter-wrapper">
      <input type="text" placeholder="Lista de vehículos" class="filter-search-input" v-model="filtroBusqueda" />
      <img src="../assets/icons/search.png" class="filter-search-icon" alt="Buscar" />
    </div>
    <div class="filter-group-item">
      <label>Estado:</label>
      <select class="mockup-select" v-model="filtroEstado">
        <option value="">Todos</option>
        <option value="1">Disponible</option>
        <option value="2">En Viaje</option>
        <option value="3">En Mantenimiento</option>
        <option value="4">Fuera de Servicio</option>
      </select>
    </div>
    <div class="filter-group-item">
      <label>Tipo:</label>
      <select class="mockup-select" v-model="filtroTipo">
        <option value="">Todos</option>
        <option v-for="tipo in tiposDisponibles" :key="tipo" :value="tipo">{{ tipo }}</option>
      </select>
    </div>
    <button class="btn-filter-action" @click="limpiarFiltros">Limpiar Filtros</button>
    <button class="btn-new-solicitud-trigger" @click="abrirHistorialMantenimientos">Ver historial</button>
    <button class="btn-new-solicitud-trigger" :class="{ 'btn-disabled': !puedeModificarVehiculos }" :disabled="!puedeModificarVehiculos" @click="abrirFormularioModal">Agregar</button>
  </div>

  <div class="solicitudes-grid full-width-table">
    <section class="grid-left-table card-panel-mockup">
      <div class="table-responsive">
        <table class="custom-table-mockup">
          <thead>
            <tr>
              <th>Placa</th>
              <th>Vehículo</th>
              <th>Tipo</th>
              <th>Marca</th>
              <th>Año</th>
              <th>Estado</th>
              <th>Disponibilidad</th>
              <th class="text-center" style="width: 120px;">Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="vehiculo in vehiculosFiltrados" :key="vehiculo.matricula">
              <td>{{ vehiculo.matricula }}</td>
              <td>{{ vehiculo.modelo }}</td>
              <td>{{ vehiculo.tipo }}</td>
              <td>{{ vehiculo.marca }}</td>
              <td>{{ vehiculo.anio }}</td>
              <td>
                <span :class="['status-pill-mockup', obtenerClaseEstado(vehiculo.estado)]">
                  {{ formatearEstadoVista(vehiculo.estado) }}
                </span>
              </td>
              <td>
                <span :class="['dispo-indicator', vehiculo.estado === 1 ? 'dispo-ok' : 'dispo-ko']"></span>
                {{ vehiculo.estado === 1 ? 'Disponible' : 'No disponible' }}
              </td>
              <td class="actions-cell-fixed">
                <div class="actions-wrapper">
              <button
                class="action-btn-mockup"
                title="Ver detalle"
                @click="verDetalleVehiculo(vehiculo)"
              >
                <Eye :size="15" />
              </button>

              <button
                class="action-btn-mockup"
                :class="{ 'btn-disabled': !puedeModificarVehiculos }"
                :disabled="!puedeModificarVehiculos"
                title="Editar vehículo"
                @click="abrirEditarModal(vehiculo)"
              >
                <Pencil :size="15" />
              </button>

              <button
                class="action-btn-mockup"
                :class="{ 'btn-disabled': !puedeEliminarVehiculos }"
                :disabled="!puedeEliminarVehiculos"
                title="Eliminar vehículo"
                @click="eliminarVehiculoApi(vehiculo.matricula)"
              >
                <Trash2 :size="15" />
              </button>
            </div>
              </td>
            </tr>
            <tr v-if="vehiculosFiltrados.length === 0">
              <td colspan="8" class="text-center text-muted" style="padding: 30px;">
                No se encontraron vehículos con los filtros aplicados.
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
        <h3>{{ esEdicion ? 'Editar Vehículo' : 'Registrar Nuevo Vehículo' }}</h3>
        <button class="btn-close-modal" @click="mostrarFormulario = false">×</button>
      </div>
      <form @submit.prevent="guardarVehiculo" class="form-solicitud-mockup">
        <div class="form-group-mockup">
          <label>Placa / Matrícula</label>
          <input type="text" v-model="formModel.matricula" placeholder="Ej. A123456" :disabled="esEdicion" required />
        </div>
        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Marca</label>
            <input type="text" v-model="formModel.marca" placeholder="Ej. Toyota" required />
          </div>
          <div class="form-group-mockup">
            <label>Modelo</label>
            <input type="text" v-model="formModel.modelo" placeholder="Ej. Corolla" required />
          </div>
        </div>
        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Año</label>
            <input type="number" v-model.number="formModel.anio" min="1900" max="2027" required />
          </div>
          <div class="form-group-mockup">
            <label>Color</label>
            <input type="text" v-model="formModel.color" placeholder="Ej. Blanco" required />
          </div>
        </div>
        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Tipo</label>
            <input type="text" v-model="formModel.tipo" placeholder="Ej. Autobús" required />
          </div>
          <div class="form-group-mockup">
            <label>Capacidad Pasajeros</label>
            <input type="number" v-model.number="formModel.capacidad" min="1" required />
          </div>
        </div>
        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Kilometraje</label>
            <input type="number" v-model.number="formModel.kilometraje" min="0" required />
          </div>
          <div class="form-group-mockup">
            <label>URL Imagen (Opcional)</label>
            <input type="text" v-model="formModel.imagenUrl" placeholder="http://..." />
          </div>
        </div>
        <div class="form-group-mockup" v-if="esEdicion">
          <label>Estado Operacional</label>
          <select class="mockup-select" v-model.number="formModel.estado">
            <option value="1">Disponible</option>
            <option value="2">En Viaje</option>
            <option value="3">En Mantenimiento</option>
            <option value="4">Fuera de Servicio</option>
          </select>
        </div>
        <div class="form-actions-central">
          <button type="button" class="btn-cancel-mockup" @click="mostrarFormulario = false">Cancelar</button>
          <button type="submit" class="btn-submit-mockup" :disabled="guardando">
            {{ guardando ? 'Guardando...' : 'Guardar Vehículo' }}
          </button>
        </div>
      </form>
    </div>
  </div>


  <div v-if="mostrarMantenimiento" class="modal-overlay-mockup" @click.self="cerrarMantenimiento">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>Registrar Mantenimiento de Flota</h3>
        <button class="btn-close-modal" @click="cerrarMantenimiento">×</button>
      </div>
      <form @submit.prevent="guardarMantenimientoApi" class="form-solicitud-mockup">
        
        <div class="form-group-mockup">
          <label>Seleccionar Vehículo</label>
          <select class="mockup-select" v-model.number="formMantenimiento.vehiculoId" required>
            <option value="" disabled>Seleccione una unidad...</option>
            <option v-for="v in vehiculos" :key="v.id" :value="v.id">
              {{ v.marca }} {{ v.modelo }} - {{ v.matricula }}
            </option>
          </select>
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Tipo de Mantenimiento</label>
            <select class="mockup-select" v-model.number="formMantenimiento.tipoMantenimiento" required>
              <option :value="1">Preventivo</option>
              <option :value="2">Correctivo</option>
              <option :value="3">Predictivo</option>
            </select>
          </div>
          <div class="form-group-mockup">
            <label>Costo total ($)</label>
            <input type="number" v-model.number="formMantenimiento.costo" min="0" step="0.01" placeholder="0.00" required />
          </div>
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Fecha del Servicio</label>
            <input type="datetime-local" v-model="formMantenimiento.fechaMantenimiento" required />
          </div>
          <div class="form-group-mockup">
            <label>Próxima Revisión</label>
            <input type="datetime-local" v-model="formMantenimiento.proximoMantenimiento" required />
          </div>
        </div>

        <div class="form-group-mockup">
          <label>Taller / Centro de Servicios</label>
          <input type="text" v-model="formMantenimiento.taller" placeholder="Ej. Taller Central" required />
        </div>

        <div class="form-group-mockup">
          <label>Descripción del Trabajo</label>
          <textarea v-model="formMantenimiento.descripcion" placeholder="Ej. Cambio de aceite y filtros" rows="3" required></textarea>
        </div>

        <div class="form-actions-central">
          <button type="button" class="btn-cancel-mockup" @click="cerrarMantenimiento">Cancelar</button>
          <button type="submit" class="btn-submit-mockup" :disabled="guardandoMantenimiento">
            {{ guardandoMantenimiento ? 'Enviando a la API...' : 'Registrar Mantenimiento' }}
          </button>
        </div>
      </form>
    </div>
  </div>


  <div v-if="mostrarConsumo" class="modal-overlay-mockup" @click.self="cerrarConsumo">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>Registrar Consumo de Combustible</h3>
        <button class="btn-close-modal" @click="cerrarConsumo">×</button>
      </div>
      <form @submit.prevent="guardarConsumoApi" class="form-solicitud-mockup">
        
        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Vehículo</label>
            <select v-model.number="formConsumo.vehiculoId" class="mockup-select" required>
              <option value="" disabled>Seleccione vehículo</option>
              <option v-for="v in vehiculos" :key="v.id" :value="v.id">
                {{ v.marca }} {{ v.modelo }} - {{ v.matricula }}
              </option>
            </select>
          </div>
          <div class="form-group-mockup">
            <label>Conductor</label>
            <select class="mockup-select" v-model.number="formConsumo.conductorId" required>
              <option value="" disabled>Seleccione conductor</option>
              <option v-for="c in conductores" :key="c.id" :value="c.id">
                {{ c.nombre }} {{ c.apellido || '' }}
              </option>
            </select>
          </div>
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Galones</label>
            <input type="number" v-model.number="formConsumo.galones" min="0.1" step="0.01" placeholder="0.00" required />
          </div>
          <div class="form-group-mockup">
            <label>Costo (RD$)</label>
            <input type="number" v-model.number="formConsumo.costo" min="1" step="0.01" placeholder="RD$" required />
          </div>
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Kilómetros Recorridos</label>
            <input type="number" v-model.number="formConsumo.kilometrosRecorridos" min="0" step="0.1" placeholder="0.0" required />
          </div>
          <div class="form-group-mockup">
            <label>Fecha</label>
            <input type="datetime-local" v-model="formConsumo.fecha" required />
          </div>
        </div>

        <div class="form-actions-central">
          <button type="button" class="btn-cancel-mockup" @click="cerrarConsumo">Cancelar</button>
          <button type="submit" class="btn-submit-mockup" :disabled="guardandoConsumo">
            {{ guardandoConsumo ? 'Guardando...' : 'Guardar Registro' }}
          </button>
        </div>
      </form>
    </div>
  </div>


  <div v-if="mostrarDetalle" class="modal-overlay-mockup" @click.self="mostrarDetalle = false">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>Detalle de Vehículo</h3>
        <button class="btn-close-modal" @click="mostrarDetalle = false">×</button>
      </div>
      <div class="detalle-solicitud-wrapper">
        <div class="detalle-item" style="text-align: center; margin-bottom: 15px;" v-if="vehiculoSeleccionado.imagenUrl">
          <img :src="vehiculoSeleccionado.imagenUrl" alt="Vehículo" style="max-width: 100%; max-height: 150px; border-radius: 8px; object-fit: cover;" />
        </div>
        <div class="form-row-mockup">
          <div class="detalle-item"><strong>Placa / Matrícula:</strong><p>{{ vehiculoSeleccionado.matricula }}</p></div>
          <div class="detalle-item"><strong>Modelo:</strong><p>{{ vehiculoSeleccionado.modelo }}</p></div>
        </div>
        <div class="form-row-mockup">
          <div class="detalle-item"><strong>Marca:</strong><p>{{ vehiculoSeleccionado.marca }}</p></div>
          <div class="detalle-item"><strong>Año:</strong><p>{{ vehiculoSeleccionado.anio }}</p></div>
        </div>
        <div class="form-row-mockup">
          <div class="detalle-item"><strong>Tipo:</strong><p>{{ vehiculoSeleccionado.tipo }}</p></div>
          <div class="detalle-item"><strong>Color:</strong><p>{{ vehiculoSeleccionado.color }}</p></div>
        </div>
        <div class="form-row-mockup">
          <div class="detalle-item"><strong>Capacidad Pasajeros:</strong><p>{{ vehiculoSeleccionado.capacidad }} personas</p></div>
          <div class="detalle-item"><strong>Kilometraje:</strong><p>{{ vehiculoSeleccionado.kilometraje }} KM</p></div>
        </div>
        <div class="detalle-item"><strong>Estado Actual:</strong><p>{{ formatearEstadoVista(vehiculoSeleccionado.estado) }}</p></div>
      </div>
      <div class="form-actions-central" style="margin-top: 20px;">
        <button type="button" class="btn-submit-mockup" @click="mostrarDetalle = false">Cerrar</button>
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

  <div v-if="mostrarHistorialMant" class="modal-overlay-mockup" @click.self="mostrarHistorialMant = false">
  <div class="modal-container-historial">
    <div class="form-panel-header-central">
      <h3>Historial de mantenimientos</h3>
      <button class="btn-close-modal" @click="mostrarHistorialMant = false">×</button>
    </div>

    <div class="historial-filters">
      <input
        type="text"
        placeholder="Buscar por vehículo, taller, descripción..."
        v-model="histMantBusqueda"
        class="filter-search-input"
        style="flex:2;min-width:200px"
      />
      <select class="mockup-select" v-model="histMantTipo">
        <option value="">Todos los tipos</option>
        <option value="1">Preventivo</option>
        <option value="2">Correctivo</option>
        <option value="3">Predictivo</option>
      </select>
      <select class="mockup-select" v-model="histMantEstado">
        <option value="">Todos los estados</option>
        <option value="1">Pendiente</option>
        <option value="2">En proceso</option>
        <option value="3">Completado</option>
      </select>
      <input type="date" class="mockup-select" v-model="histMantDesde" />
      <input type="date" class="mockup-select" v-model="histMantHasta" />
      <button class="btn-cancel-mockup" @click="limpiarHistorialMant">Limpiar</button>
    </div>

    <div class="table-responsive">
      <table class="custom-table-mockup">
        <thead>
          <tr>
            <th>#</th>
            <th>Fecha</th>
            <th>Vehículo</th>
            <th>Tipo</th>
            <th>Descripción</th>
            <th>Taller</th>
            <th>Kilometraje</th>
            <th>Monto (RD$)</th>
            <th>Estado</th>
            <th>Próxima revisión</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="m in mantenimientosFiltrados" :key="m.id">
            <td>#{{ m.id }}</td>
            <td>{{ formatearFechaMant(m.fechaMantenimiento) }}</td>
            <td>{{ obtenerNombreVehiculo(m.vehiculoId) }}</td>
            <td>
              <span :class="['status-pill-mockup', claseTipoMant(m.tipoMantenimiento)]">
                {{ labelTipoMant(m.tipoMantenimiento) }}
              </span>
            </td>
            <td>{{ m.descripcion || '---' }}</td>
            <td>{{ m.taller || '---' }}</td>
            <td>{{ m.kilometraje ? m.kilometraje.toLocaleString('es-DO') + ' km' : '---' }}</td>
            <td>{{ m.costo ? 'RD$ ' + Number(m.costo).toLocaleString('es-DO') : '---' }}</td>
            <td>
              <span :class="['status-pill-mockup', claseEstadoMant(m.estado)]">
                {{ labelEstadoMant(m.estado) }}
              </span>
            </td>
            <td>{{ formatearFechaMant(m.proximoMantenimiento) }}</td>
          </tr>
          <tr v-if="mantenimientosFiltrados.length === 0">
            <td colspan="10" style="text-align:center;padding:30px;color:#9ca3af">
              No se encontraron registros de mantenimiento.
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div style="padding:10px 14px;font-size:0.8rem;color:#9ca3af;border-top:1px solid #e5e7eb">
      Mostrando {{ mantenimientosFiltrados.length }} de {{ mantenimientos.length }} registros
    </div>
  </div>
</div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'


import {
  CarFront,
  CircleCheck,
  Wrench,
  CircleX,
  Plus,
  Fuel,
  CalendarDays,
  Eye,
  Pencil,
  Trash2
} from 'lucide-vue-next'

const router = useRouter()
const rolUsuario = ref('Operador')

const filtroBusqueda = ref('')
const filtroEstado = ref('')
const filtroTipo = ref('')

const vehiculos = ref([])
const mostrarFormulario = ref(false)
const mostrarDetalle = ref(false)
const esEdicion = ref(false)
const guardando = ref(false)
const mensajeNotificacion = ref('')
const vehiculoSeleccionado = ref({})
const conductores = ref([])

const mostrarHistorialMant = ref(false)
const mantenimientos        = ref([])
const histMantBusqueda      = ref('')
const histMantTipo          = ref('')
const histMantEstado        = ref('')
const histMantDesde         = ref('')
const histMantHasta         = ref('')

const limpiarHistorialMant = () => {
  histMantBusqueda.value = ''
  histMantTipo.value     = ''
  histMantEstado.value   = ''
  histMantDesde.value    = ''
  histMantHasta.value    = ''
}

const abrirHistorialMantenimientos = async () => {
  mostrarHistorialMant.value = true
  const token = localStorage.getItem('token_transporte')
  try {
    const res = await fetch('https://localhost:7221/api/Mantenimientos', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (res.ok) mantenimientos.value = await res.json()
  } catch (e) {
    console.error('Error cargando mantenimientos:', e)
  }
}

const obtenerNombreVehiculo = (id) => {
  const v = vehiculos.value.find(v => v.id === id)
  return v ? `${v.marca} ${v.modelo} • ${v.matricula}` : `#${id}`
}

const mantenimientosFiltrados = computed(() => {
  const txt = histMantBusqueda.value.toLowerCase().trim()
  return mantenimientos.value.filter(m => {
    const nombreVeh = obtenerNombreVehiculo(m.vehiculoId).toLowerCase()
    const matchTxt  = !txt || [
      String(m.id), nombreVeh, m.descripcion, m.taller
    ].some(v => v?.toLowerCase().includes(txt))
    const matchTipo  = !histMantTipo.value   || m.tipoMantenimiento == histMantTipo.value
    const matchEst   = !histMantEstado.value || m.estado == histMantEstado.value
    const fecha      = m.fechaMantenimiento?.slice(0, 10)
    const matchDesde = !histMantDesde.value  || fecha >= histMantDesde.value
    const matchHasta = !histMantHasta.value  || fecha <= histMantHasta.value
    return matchTxt && matchTipo && matchEst && matchDesde && matchHasta
  })
})

const labelTipoMant  = t => ({ 1:'Preventivo', 2:'Correctivo', 3:'Predictivo' }[t] || '---')
const claseTipoMant  = t => ({ 1:'finalizada', 2:'rechazada', 3:'aprobada' }[t] || 'pendiente')

const labelEstadoMant = e => ({ 1:'Pendiente', 2:'En proceso', 3:'Completado' }[e] || '---')
const claseEstadoMant = e => ({ 1:'pendiente', 2:'aprobada', 3:'finalizada' }[e] || 'pendiente')

const formatearFechaMant = (iso) => {
  if (!iso) return '---'
  return new Date(iso).toLocaleString('es-DO', {
    year:'numeric', month:'2-digit', day:'2-digit',
    hour:'2-digit', minute:'2-digit', hour12:true
  })
}

const mostrarMantenimiento = ref(false)
const guardandoMantenimiento = ref(false)
const formMantenimiento = ref({
  vehiculoId: '', tipoMantenimiento: '', estado: 1, fechaMantenimiento: '',
  descripcion: '', costo: 0, taller: '', proximoMantenimiento: ''
})

const mostrarConsumo = ref(false)
const guardandoConsumo = ref(false)
const formConsumo = ref({
  vehiculoId: '', conductorId: 1, fecha: '', galones: 0, costo: 0, kilometrosRecorridos: 0, estado: 1
})

const formModel = ref({
  matricula: '', marca: '', modelo: '',
  anio: new Date().getFullYear(),
  color: '', capacidad: 5, tipo: '',
  imagenUrl: '', kilometraje: 0, estado: 1
})


const esAdministradorSistema = computed(() => {
  const r = rolUsuario.value.toLowerCase()
  return r === 'administrador' || r === 'admin' || r === 'superadmin'
})

const puedeModificarVehiculos = computed(() => esAdministradorSistema.value)
const puedeGestionarMantenimiento = computed(() => esAdministradorSistema.value)
const puedeEliminarVehiculos = computed(() => esAdministradorSistema.value)


const fetchVehiculosDeAPI = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const response = await fetch('https://localhost:7221/api/Vehiculos', {
      headers: { 'Authorization': `Bearer ${token}` }
    })
    if (response.ok) vehiculos.value = await response.json()
  } catch (error) {
    console.error('Error cargando vehículos:', error)
  }
}

const guardarVehiculo = async () => {
  guardando.value = true
  const token = localStorage.getItem('token_transporte')
  const vehiculoPayload = {
    id: esEdicion.value ? parseInt(formModel.value.id) : 0,
    matricula: formModel.value.matricula.trim(),
    marca: formModel.value.marca,
    modelo: formModel.value.modelo,
    anio: parseInt(formModel.value.anio) || 2026,
    color: formModel.value.color,
    capacidad: parseInt(formModel.value.capacidad) || 5,
    tipo: formModel.value.tipo,
    imagenUrl: formModel.value.imagenUrl?.trim() || '',
    kilometraje: parseInt(formModel.value.kilometraje) || 0,
    estado: parseInt(formModel.value.estado) || 1
  }
  const url = esEdicion.value
    ? `https://localhost:7221/api/Vehiculos/${formModel.value.id}`
    : 'https://localhost:7221/api/Vehiculos'
  try {
    const response = await fetch(url, {
      method: esEdicion.value ? 'PUT' : 'POST',
      headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
      body: JSON.stringify(vehiculoPayload)
    })
    if (response.ok) {
      mostrarFormulario.value = false
      await fetchVehiculosDeAPI()
    } else {
      mensajeNotificacion.value = 'Error al guardar los cambios de la unidad.'
    }
  } catch {
    mensajeNotificacion.value = 'Error de conexión con el servidor.'
  } finally {
    guardando.value = false
  }
}

const eliminarVehiculoApi = async (matricula) => {
  if (!confirm('¿Seguro que deseas eliminar este vehículo de forma permanente?')) return
  const token = localStorage.getItem('token_transporte')
  try {
    const response = await fetch(`https://localhost:7221/api/Vehiculos/${matricula}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token}` }
    })
    if (response.ok) await fetchVehiculosDeAPI()
    else mensajeNotificacion.value = 'No se pudo eliminar el vehículo.'
  } catch (error) {
    console.error(error)
  }
}



const abrirMantenimientoNuevo = () => {
  const ahora = new Date()
  const pad = (n) => n.toString().padStart(2, '0')
  const fechaActualInput = `${ahora.getFullYear()}-${pad(ahora.getMonth()+1)}-${pad(ahora.getDate())}T${pad(ahora.getHours())}:${pad(ahora.getMinutes())}`

  formMantenimiento.value = {
    vehiculoId: '',
    tipoMantenimiento: 1, 
    estado: 1, 
    fechaMantenimiento: fechaActualInput,
    descripcion: '',
    costo: '',
    taller: '',
    proximoMantenimiento: fechaActualInput
  }
  mostrarMantenimiento.value = true
}

const cerrarMantenimiento = () => {
  mostrarMantenimiento.value = false
  guardandoMantenimiento.value = false
}

const guardarMantenimientoApi = async () => {
  guardandoMantenimiento.value = true
  const token = localStorage.getItem('token_transporte')
  
  const payload = {
    vehiculoId: parseInt(formMantenimiento.value.vehiculoId),
    tipoMantenimiento: formMantenimiento.value.tipoMantenimiento,
    estado: parseInt(formMantenimiento.value.estado),
    fechaMantenimiento: new Date(formMantenimiento.value.fechaMantenimiento).toISOString(),
    descripcion: formMantenimiento.value.descripcion,
    costo: parseFloat(formMantenimiento.value.costo) || 0,
    taller: formMantenimiento.value.taller,
    proximoMantenimiento: new Date(formMantenimiento.value.proximoMantenimiento).toISOString()
  }

  try {
    const response = await fetch('https://localhost:7221/api/Mantenimientos', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
      body: JSON.stringify(payload)
    })
    if (response.ok) {
      cerrarMantenimiento()
      mensajeNotificacion.value = 'Mantenimiento registrado y guardado con éxito.'
      await fetchVehiculosDeAPI()
    } else {
      mensajeNotificacion.value = 'La API rechazó el registro de mantenimiento.'
    }
  } catch (error) {
    console.error(error)
    mensajeNotificacion.value = 'Error al comunicar con el endpoint de mantenimientos.'
  } finally {
    guardandoMantenimiento.value = false
  }
}


const abrirConsumoNuevo = async () => {
  const ahora = new Date()
  const pad = (n) => n.toString().padStart(2, '0')
  const fechaActualInput = `${ahora.getFullYear()}-${pad(ahora.getMonth()+1)}-${pad(ahora.getDate())}T${pad(ahora.getHours())}:${pad(ahora.getMinutes())}`

  if (conductores.value.length === 0) {
    try {
      const token = localStorage.getItem('token_transporte')
      const response = await fetch('https://localhost:7221/api/Conductores', {
        headers: { 'Authorization': `Bearer ${token}` }
      })
      if (response.ok) conductores.value = await response.json()
    } catch (err) { console.error("Error cargando choferes:", err) }
  }

  formConsumo.value = {
    vehiculoId: '',
    conductorId: '',
    fecha: fechaActualInput,
    galones: '',
    costo: '',
    kilometrosRecorridos: '',
    estado: 1
  }
  mostrarConsumo.value = true
}

const cerrarConsumo = () => {
  mostrarConsumo.value = false
  guardandoConsumo.value = false
}

const guardarConsumoApi = async () => {
  guardandoConsumo.value = true
  const token = localStorage.getItem('token_transporte')

  const payload = {
    vehiculoId: parseInt(formConsumo.value.vehiculoId),
    conductorId: parseInt(formConsumo.value.conductorId),
    fecha: new Date(formConsumo.value.fecha).toISOString(),
    galones: parseFloat(formConsumo.value.galones) || 0,
    costo: parseFloat(formConsumo.value.costo) || 0,
    kilometrosRecorridos: parseInt(formConsumo.value.kilometrosRecorridos) || 0,
    estado: parseInt(formConsumo.value.estado)
  }

  try {
  
    const response = await fetch('https://localhost:7221/api/ConsumosCombustible', {
      method: 'POST',
      headers: { 
        'Content-Type': 'application/json', 
        'Authorization': `Bearer ${token}` 
      },
      body: JSON.stringify(payload)
    })
    
    if (response.ok) {
      cerrarConsumo()
      mensajeNotificacion.value = 'Consumo de combustible registrado con éxito.'
    } else {
      mensajeNotificacion.value = 'Error de validación en los datos del consumo.'
    }
  } catch (error) {
    console.error(error)
    mensajeNotificacion.value = 'No se pudo conectar con el servidor.'
  } {
    guardandoConsumo.value = false
  }
}

const consultarDisponibilidadCalendario = () => {
  router.push('/calendario')
}


const abrirFormularioModal = () => {
  esEdicion.value = false
  formModel.value = { matricula: '', marca: '', modelo: '', anio: new Date().getFullYear(), color: '', capacidad: 5, tipo: '', imagenUrl: '', kilometraje: 0, estado: 1 }
  mostrarFormulario.value = true
}

const abrirEditarModal = (vehiculo) => {
  esEdicion.value = true
  formModel.value = { ...vehiculo }
  mostrarFormulario.value = true
}

const verDetalleVehiculo = (vehiculo) => {
  vehiculoSeleccionado.value = { ...vehiculo }
  mostrarDetalle.value = true
}


const totalVehiculos = computed(() => vehiculos.value.length)
const disponiblesContador = computed(() => vehiculos.value.filter(v => v.estado === 1).length)
const mantenimientoContador = computed(() => vehiculos.value.filter(v => v.estado === 3).length)
const noDisponiblesContador = computed(() => vehiculos.value.filter(v => v.estado !== 1).length)

const tiposDisponibles = computed(() => [...new Set(vehiculos.value.map(v => v.tipo).filter(Boolean))])

const vehiculosFiltrados = computed(() =>
  vehiculos.value.filter(v => {
    const cumpleBusqueda = !filtroBusqueda.value ||
      v.matricula?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
      v.modelo?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
      v.marca?.toLowerCase().includes(filtroBusqueda.value.toLowerCase())
    return cumpleBusqueda &&
      (!filtroEstado.value || v.estado == filtroEstado.value) &&
      (!filtroTipo.value || v.tipo === filtroTipo.value)
  })
)

const limpiarFiltros = () => {
  filtroBusqueda.value = ''
  filtroEstado.value = ''
  filtroTipo.value = ''
}

const formatearEstadoVista = (estado) =>
  ({ 1: 'Disponible', 2: 'En Viaje', 3: 'En Mantenimiento', 4: 'Fuera de Servicio' }[estado] || 'Desconocido')

const obtenerClaseEstado = (estado) =>
  ({ 1: 'finalizada', 2: 'pendiente', 3: 'aprobada', 4: 'rechazada' }[estado] || 'pendiente')

onMounted(() => {
  const rolGuardado = localStorage.getItem('usuario_rol')
  if (rolGuardado) rolUsuario.value = rolGuardado
  fetchVehiculosDeAPI()
})
</script>

<style>
@import '/src/assets/styles/variables.css';
@import '/src/assets/styles/reset.css';

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

.cards-kpi-container {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 24px;
}

.card-kpi {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 18px;
  display: flex;
  align-items: flex-start;
  gap: 14px;
  box-shadow: 0 8px 22px rgba(15, 23, 42, 0.04);
}

.card-kpi-icon {
  width: 48px;
  height: 48px;
  min-width: 48px;
  border-radius: 14px;
  background-color: #f3f4f6 !important;
  color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
}

.cards-kpi-container {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 18px;
  margin-bottom: 24px;
}



.card-kpi-icon svg {
  width: 24px;
  height: 24px;
  stroke-width: 2;
}

.card-kpi-icon img { width: 20px; height: 20px; }

.total-icon { background-color: #f3e8ff; } 
.disponible-icon { background-color: #e8f5e9; }
.mantenimiento-icon { background-color: #e3f2fd; }
.no-disponible-icon { background-color: #ffeade; }

.card-kpi-info { display: flex; flex-direction: column; }
.card-kpi-info span { font-size: 0.8rem; color: #4b5563; font-weight: 500; }
.card-kpi-info h3 { font-size: 1.4rem; font-weight: 700; color: #111827; margin: 2px 0 0 0; }

.quick-actions-bar {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 20px;
  margin-bottom: 24px;
}

.quick-actions-bar h4 { margin: 0 0 12px 0; font-size: 0.85rem; font-weight: 700; color: #111827; }

.actions-wrapper-quick {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
}

.action-btn-mockup:hover:not(:disabled) {
  background: #111827;
  color: white;
  border-color: #111827;
  transform: scale(1.05);
}

.btn-quick-action {
  display: flex;
  align-items: center;
  gap: 10px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 12px 18px;
  font-size: .9rem;
  font-weight: 600;
  color: #111827;
  cursor: pointer;
  transition: all .2s ease;
}

.btn-quick-action img { width: 18px; height: 18px; object-fit: contain; flex-shrink: 0; }

.btn-quick-action:hover:not(:disabled) {
  background: #111827;
  color: white;
  border-color: #111827;
  transform: translateY(-2px);
}

.btn-quick-action svg {
  flex-shrink: 0;
}

.dispo-indicator { display: inline-block; width: 8px; height: 8px; border-radius: 50%; margin-right: 6px; }
.dispo-ok { background-color: #10b981; }
.dispo-ko { background-color: #ef4444; }

.icon-view { background-image: url('../assets/icons/ver.png'); background-size: 14px; background-repeat: no-repeat; background-position: center; }
.icon-edit { background-image: url('../assets/icons/editar-negro.png'); background-size: 14px; background-repeat: no-repeat; background-position: center; }
.icon-delete { background-image: url('../assets/icons/eliminar.png'); background-size: 14px; background-repeat: no-repeat; background-position: center; }

.btn-disabled {
  opacity: .35;
  cursor: not-allowed;
}

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

.search-filter-wrapper { position: relative; flex: 1; }

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

.filter-group-item { display: flex; flex-direction: column; gap: 6px; }

.mockup-select {
  padding: 10px;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
}

.btn-filter-action,
.btn-new-solicitud-trigger {
  background: #111827;
  color: white;
  border: none;
  padding: 10px 18px;
  border-radius: 8px;
  cursor: pointer;
}


.solicitudes-grid.full-width-table { width: 100%; }

.card-panel-mockup {
  background: white;
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  overflow: hidden;
}

.custom-table-mockup { width: 100%; border-collapse: collapse; }

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

.status-pill-mockup {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
}

.actions-cell-fixed {
  padding: 14px 12px;
  border-top: 1px solid #e5e7eb;
  vertical-align: middle;
  text-align: center;
  width: 120px;
}

.actions-wrapper {
  display: flex;
  gap: 12px;
  justify-content: center;
  align-items: center;
  min-width: 110px; 
}

.action-btn-mockup {
  width: 34px;
  height: 34px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background: #ffffff;
  color: #374151;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all .2s ease;
}

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
  border-radius: 12px;
  padding: 24px;
  max-height: 90vh;
  overflow-y: auto;
}

.form-panel-header-central {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 18px;
}

.btn-close-modal {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #6b7280;
}

.form-solicitud-mockup { display: flex; flex-direction: column; gap: 14px; }

.form-group-mockup { display: flex; flex-direction: column; gap: 6px; }

.form-group-mockup label { font-size: 0.85rem; font-weight: 500; color: #374151; }

.form-group-mockup input,
.form-group-mockup select,
.form-group-mockup textarea {
  padding: 10px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  font-size: 0.9rem;
}

.form-row-mockup { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }

.form-actions-central { display: flex; justify-content: flex-end; gap: 10px; margin-top: 8px; }

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


.detalle-solicitud-wrapper { display: flex; flex-direction: column; gap: 12px; }
.detalle-item strong { font-size: 0.82rem; color: #6b7280; }
.detalle-item p { margin: 2px 0 0 0; font-size: 0.95rem; color: #111827; }


.toast-error-moderno {
  position: fixed;
  top: 20px;
  right: 20px;
  background: white;
  border-left: 4px solid #10b981;
  padding: 16px;
  border-radius: 10px;
  box-shadow: 0 10px 20px rgba(0,0,0,.1);
  display: flex;
  align-items: flex-start;
  gap: 12px;
  z-index: 9999;
}

.toast-title { font-weight: 700; font-size: 0.85rem; color: #111827; }
.toast-text { margin: 2px 0 0 0; font-size: 0.85rem; color: #6b7280; }

.btn-close-toast {
  background: none;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
  color: #6b7280;
}

.text-center { text-align: center; }
.text-muted { color: #9ca3af; }

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.dashboard-header-left {
  display: flex;
  flex-direction: column;
}

.dashboard-header-badge {
  background: #f3f4f6;
  color: #111827;
  padding: 8px 14px;
  border-radius: 999px;
  font-size: .82rem;
  font-weight: 700;
  border: 1px solid #e5e7eb;
}

.modal-container-historial {
  background: white;
  width: 95vw;
  max-width: 1200px;
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
</style>