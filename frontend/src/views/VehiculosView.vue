<template>
  <div class="section-header-mockup">
    <h2>Gestión de Vehículos</h2>
    <p>Administra y gestiona a los vehículos</p>
  </div>

  <div class="cards-kpi-container">
    <div class="card-kpi">
      <div class="card-kpi-icon total-icon">
        <img src="../assets/icons/vehiculo-negro.png" alt="Total" />
      </div>
      <div class="card-kpi-info">
        <span>Total vehículos</span>
        <h3>{{ totalVehiculos }}</h3>
      </div>
    </div>
    <div class="card-kpi">
      <div class="card-kpi-icon disponible-icon">
        <img src="../assets/icons/disponible-negro.png" alt="Disponibles" />
      </div>
      <div class="card-kpi-info">
        <span>Disponibles</span>
        <h3>{{ disponiblesContador }}</h3>
      </div>
    </div>
    <div class="card-kpi">
      <div class="card-kpi-icon mantenimiento-icon">
        <img src="../assets/icons/herramienta-negra.png" alt="Mantenimiento" />
      </div>
      <div class="card-kpi-info">
        <span>En mantenimiento</span>
        <h3>{{ mantenimientoContador }}</h3>
      </div>
    </div>
    <div class="card-kpi">
      <div class="card-kpi-icon no-disponible-icon">
        <img src="../assets/icons/cancelar.png" alt="No disponibles" />
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
      <button class="btn-quick-action" :class="{ 'btn-disabled': !puedeModificarVehiculos }" :disabled="!puedeModificarVehiculos" @click="abrirFormularioModal">
        <img src="../assets/icons/registrar-negro.png" alt="Registrar" /> Registrar vehículo
      </button>
      <button class="btn-quick-action" :class="{ 'btn-disabled': !puedeGestionarMantenimiento }" :disabled="!puedeGestionarMantenimiento">
        <img src="../assets/icons/herramienta-negra.png" alt="Mantenimiento" /> Registrar mantenimiento
      </button>
      <button class="btn-quick-action" :class="{ 'btn-disabled': !puedeModificarVehiculos }" :disabled="!puedeModificarVehiculos">
        <img src="../assets/icons/gasolina-negro.png" alt="Consumo" /> Registrar consumo
      </button>
      <button class="btn-quick-action">
        <img src="../assets/icons/calendario-negro.png" alt="Consultar" /> Consultar disponibilidad
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
                  <button class="action-btn-mockup icon-view" title="Ver detalle" @click="verDetalleVehiculo(vehiculo)"></button>
                  <button class="action-btn-mockup icon-edit" :class="{ 'btn-disabled': !puedeModificarVehiculos }" :disabled="!puedeModificarVehiculos" title="Editar Vehículo" @click="abrirEditarModal(vehiculo)"></button>
                  <button class="action-btn-mockup icon-delete" :class="{ 'btn-disabled': !puedeEliminarVehiculos }" :disabled="!puedeEliminarVehiculos" title="Eliminar Vehículo" @click="eliminarVehiculoApi(vehiculo.matricula)"></button>
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

  <!-- Modal Formulario -->
  <div v-if="mostrarFormulario" class="modal-overlay-mockup" @click.self="mostrarFormulario = false">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>{{ esEdicion ? 'Editar Vehículo' : 'Registrar Nuevo Vehículo' }}</h3>
        <button class="btn-close-modal" @click="mostrarFormulario = false">×</button>
      </div>
      <form @submit.prevent="guardarVehiculo" class="form-solicitud-mockup">
        <div class="form-group-mockup">
          <label>Placa / Matrícula</label>
          <input type="text" v-model="formModel.matricula" placeholder="Ej. A123456" required />
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

  <!-- Modal Detalle -->
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

  <!-- Toast -->
  <div v-if="mensajeNotificacion" class="toast-error-moderno">
    <div class="toast-content">
      <span class="toast-title">Notificación</span>
      <p class="toast-text">{{ mensajeNotificacion }}</p>
    </div>
    <button class="btn-close-toast" @click="mensajeNotificacion = ''">×</button>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

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

const formModel = ref({
  matricula: '', marca: '', modelo: '',
  anio: new Date().getFullYear(),
  color: '', capacidad: 5, tipo: '',
  imagenUrl: '', kilometraje: 0, estado: 1
})

const puedeModificarVehiculos = computed(() => rolUsuario.value.toLowerCase() === 'administrador')
const puedeGestionarMantenimiento = computed(() => ['administrador'].includes(rolUsuario.value.toLowerCase()))
const puedeEliminarVehiculos = computed(() => rolUsuario.value.toLowerCase() === 'administrador')

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
      mensajeNotificacion.value = 'Error al guardar los cambios.'
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
  border-radius: 16px;
  padding: 16px;
  display: flex;
  align-items: center;
  gap: 14px;
}

.card-kpi-icon {
  width: 44px;
  height: 44px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
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

.btn-quick-action {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 10px 16px;
  font-size: 0.88rem;
  font-weight: 500;
  color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-quick-action img { width: 18px; height: 18px; object-fit: contain; flex-shrink: 0; }

.btn-quick-action:hover:not(:disabled) {
  background-color: #f9fafb;
  border-color: #d1d5db;
}

.dispo-indicator { display: inline-block; width: 8px; height: 8px; border-radius: 50%; margin-right: 6px; }
.dispo-ok { background-color: #10b981; }
.dispo-ko { background-color: #ef4444; }

.icon-view { background-image: url('../assets/icons/ver.png'); background-size: 14px; background-repeat: no-repeat; background-position: center; }
.icon-edit { background-image: url('../assets/icons/editar-negro.png'); background-size: 14px; background-repeat: no-repeat; background-position: center; }
.icon-delete { background-image: url('../assets/icons/eliminar.png'); background-size: 14px; background-repeat: no-repeat; background-position: center; }

.btn-disabled { opacity: 0.15; cursor: not-allowed; }

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

/* Tabla */
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

.action-btn-mockup {
  width: 28px;
  height: 28px;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  background: #ffffff center no-repeat;
  background-size: 14px;
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

/* Detalle */
.detalle-solicitud-wrapper { display: flex; flex-direction: column; gap: 12px; }
.detalle-item strong { font-size: 0.82rem; color: #6b7280; }
.detalle-item p { margin: 2px 0 0 0; font-size: 0.95rem; color: #111827; }

/* Toast */
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
</style>