<template>
  <div class="dashboard-layout">
    
    <aside class="sidebar">
      <div class="logo-container">
        <h2>Transporte</h2>
        <h3>SmartRoute</h3>
      </div>
      <nav class="menu">
        <ul>
          <li @click="router.push('/operador/dashboard')">
            <img src="../../assets/icons/dashboard.png" alt="Icono Dashboard" class="menu-icon" />
            Dashboard
          </li>
          <li>
            <img src="../../assets/icons/vehiculos.png" alt="Icono Vehículos" class="menu-icon" />
            Vehículos
          </li>
          <li>
            <img src="../../assets/icons/conductores.png" alt="Icono Conductores" class="menu-icon" />
            Conductores
          </li>
          <li>
            <img src="../../assets/icons/asignaciones.png" alt="Icono Asignaciones" class="menu-icon" />
            Asignaciones
          </li>
          <li class="active">
            <img src="../../assets/icons/solicitudes.png" alt="Icono Solicitudes" class="menu-icon" />
            Solicitudes
          </li>
          <li>
            <img src="../../assets/icons/calendario.png" alt="Icono Calendario" class="menu-icon" />
            Calendario
          </li>
          <li>
            <img src="../../assets/icons/herramientas.png" alt="Icono Mantenimiento" class="menu-icon" />
            Mantenimiento
          </li>
          <li>
            <img src="../../assets/icons/combustible.png" alt="Icono Combustible" class="menu-icon" />
            Combustible
          </li>
          <li>
            <img src="../../assets/icons/reportes.png" alt="Icono Reportes" class="menu-icon" />
            Reportes
          </li>
          <li>
            <img src="../../assets/icons/conductores.png" alt="Icono Usuarios" class="menu-icon" />
            Usuarios
          </li>
          <li>
            <img src="../../assets/icons/rutas.png" alt="Icono Viajes" class="menu-icon" />
            Viajes
          </li>
        </ul>
      </nav>

      <div class="sidebar-footer">
        <button class="btn-logout" @click="cerrarSesion">
          <img src="../../assets/icons/logout.png" alt="Icono Cerrar Sesión" class="menu-icon" />
          Cerrar Sesión
        </button>
      </div>
    </aside>

    <div class="main-content">
      
      <header class="navbar">
        <div class="navbar-left">
          <button class="btn-menu">
            <img src="../../assets/icons/menu.png" alt="Menú" class="icon-nav" />
          </button>
          <span class="navbar-title">Inicio</span>
        </div>

        <div class="navbar-center">
          <div class="search-container">
            <input type="text" placeholder="Buscar..." class="search-input" v-model="filtroBusqueda" />
            <img src="../../assets/icons/search.png" alt="Buscar" class="icon-search" />
          </div>
        </div>

        <div class="navbar-right">
          <div class="user-avatar-container">
            <img src="../../assets/icons/avatar.png" alt="Avatar" class="user-avatar" />
          </div>
          <div class="user-info">
            <span class="user-name">{{ nombreUsuario }}</span>
            <span class="user-role">{{ rolUsuario }}</span>
          </div>
        </div>
      </header>

      <main class="view-container">
        
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
      </main>

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

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const nombreUsuario = ref('Usuario')
const rolUsuario = ref('Operador')

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
      headers: { 'Authorization': `Bearer ${token}` }
    })
    if (response.ok) {
      solicitudes.value = await response.json()
    }
  } catch (error) {
    console.error("Error cargando solicitudes de la base de datos:", error)
  }
}


const areasDisponibles = computed(() => {
  const areas = solicitudes.value.map(s => s.areaSolicitante).filter(Boolean)
  return [...new Set(areas)]
})


const solicitudesFiltradas = computed(() => {
  return solicitudes.value.filter(solicitud => {
    const cumpleBusqueda = !filtroBusqueda.value || 
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
  nombreUsuario.value = localStorage.getItem('usuario_nombre') || 'Operador'
  rolUsuario.value = localStorage.getItem('usuario_rol') || 'Operador'
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
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(payload)
    })

    if (response.ok) {
      await fetchSolicitudesDeAPI() 
      formModel.value = { areaSolicitante: '', cantidadColaboradores: 1, fechaHoraSalida: '', fechaHoraRegreso: '', destino: '', motivo: '' }
      mostrarFormulario.value = false 
    } else {
      const errorResponse = await response.json()
      console.error('Error del Servidor:', errorResponse)
      alert('Error en validación de datos: ' + JSON.stringify(errorResponse.errors || errorResponse))
    }
  } catch (error) {
    console.error("Error al conectar con la API:", error)
  } finally {
    guardando.value = false
  }
}

const cerrarSesion = () => {
  localStorage.clear()
  router.push('/login')
}
</script>

<style>
@import '/src/assets/styles/variables.css';
@import '/src/assets/styles/reset.css';

.section-header-mockup { margin-bottom: 20px; }
.section-header-mockup h2 { font-size: 1.5rem; font-weight: 700; color: #111827; }
.section-header-mockup p { font-size: 0.85rem; color: #6b7280; }


.filters-bar-mockup {
  display: flex;
  align-items: flex-end;
  gap: 16px;
  background-color: #ffffff;
  padding: 16px;
  border-radius: 12px;
  margin-bottom: 24px;
  border: 1px solid var(--border-color, #e5e7eb);
}
.search-filter-wrapper { position: relative; flex: 1; }
.filter-search-input { width: 100%; padding: 10px 16px 10px 40px; border: 1px solid #e5e7eb; border-radius: 20px; font-size: 0.85rem; }
.filter-search-icon { position: absolute; left: 14px; top: 50%; transform: translateY(-50%); width: 16px; height: 16px; opacity: 0.4; }

.filter-group-item { display: flex; flex-direction: column; gap: 4px; }
.filter-group-item label { font-size: 0.8rem; font-weight: 600; color: #374151; }
.mockup-select, .mockup-date-input { padding: 8px 12px; border: 1px solid #e5e7eb; border-radius: 20px; font-size: 0.85rem; background: #ffffff; cursor: pointer; }

.btn-filter-action, .btn-new-solicitud-trigger {
  background-color: #000000; color: #ffffff; padding: 10px 20px; border-radius: 20px; font-size: 0.85rem; font-weight: 600; border: none; cursor: pointer;
}
.btn-new-solicitud-trigger { margin-left: auto; }


.solicitudes-grid.full-width-table { grid-template-columns: 1fr; width: 100%; }
.card-panel-mockup { background: #ffffff; border: 1px solid #e5e7eb; border-radius: 16px; padding: 20px; }

.table-responsive { width: 100%; overflow-x: auto; }
.custom-table-mockup { width: 100%; border-collapse: collapse; font-size: 0.82rem; }
.custom-table-mockup th { background-color: #f3f4f6; color: #1f2937; padding: 12px; font-weight: 600; border-bottom: 1px solid #e5e7eb; text-align: left; }
.custom-table-mockup td {
  padding: 14px 12px;
  border-bottom: 1px solid #e5e7eb;
  color: #374151;
  vertical-align: middle;
}
.status-pill-mockup {
  padding: 4px 12px;
  border-radius: 6px;
  font-size: 0.75rem;
  font-weight: 600;
  display: inline-block;
}
.status-pill-mockup.pendiente {
  background-color: #fef3c7;
  color: #d97706;
}
.status-pill-mockup.aprobada {
  background-color: #d1fae5;
  color: #065f46;
}
.status-pill-mockup.rechazada {
  background-color: #fee2e2;
  color: #991b1b;
}
.status-pill-mockup.cancelada {
  background-color: #e5e7eb;
  color: #374151;
}
.status-pill-mockup.finalizada {
  background-color: #e0f2fe;
  color: #0369a1;
}
.actions-cell-fixed {
  padding: 14px 12px;
  border-bottom: 1px solid #e5e7eb; 
  vertical-align: middle;
  text-align: center;
  width: 120px;
}


.actions-wrapper {
  display: flex;
  gap: 6px;
  justify-content: center;
  align-items: center;
  flex-wrap: nowrap;
}


.action-btn-mockup {
  width: 28px;
  height: 28px;
  min-width: 28px;
  border: 1px solid #e5e7eb;
  border-radius: 6px;
  background: #ffffff center no-repeat;
  background-size: 14px;
  cursor: pointer;
  flex-shrink: 0;
  padding: 0;
  transition: all 0.2s ease;

}


.action-btn-mockup:hover:not(:disabled) {
  background-color: #f3f4f6;
  transform: scale(1.05);
}


.btn-disabled { 
  opacity: 0.15; 
  cursor: not-allowed; 
  background-color: #ffffff !important;
}

.action-btn-mockup {
  width: 28px;
  height: 28px;
  min-width: 28px;
  border: 1px solid #e5e7eb;
  border-radius: 6px;

  background: #ffffff center no-repeat;
  
  background-size: 14px;
  cursor: pointer;
  flex-shrink: 0;
  display: flex;        
  align-items: center;  
  justify-content: center; 
  padding: 0; 
  transition: all 0.2s ease;
}

.action-btn-mockup:hover:not(:disabled) {
  background-color: #f3f4f6;
  transform: scale(1.05);
}


.btn-disabled { 
  opacity: 0.15; 
  cursor: not-allowed; 
  background-color: #ffffff !important;
}


.icon-view { background-image: url('../../assets/icons/ver.png'); }
.icon-edit { background-image: url('../../assets/icons/editar-negro.png'); }
.icon-delete { background-image: url('../../assets/icons/eliminar.png'); }


.modal-overlay-mockup {
  position: fixed;
  inset: 0;
  background-color: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(2px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal-container-central {
  background-color: #ffffff;
  width: 100%;
  max-width: 500px;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
  animation: scaleInMockup 0.2s ease-out;
}

.detalle-solicitud-wrapper {
  display: flex;
  flex-direction: column;
  gap: 14px;
  text-align: left;
}
.detalle-item strong {
  font-size: 0.8rem;
  color: #6b7280;
  text-transform: uppercase;
  display: block;
  margin-bottom: 2px;
}
.detalle-item p {
  font-size: 0.95rem;
  color: #111827;
  font-weight: 500;
}
.detalle-motivo-text {
  background-color: #f9fafb;
  padding: 10px;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
  font-style: italic;
}

.toast-error-moderno {
  position: fixed;
  top: 30px;
  right: 30px;
  background-color: #ffffff;
  border-left: 4px solid #dc2626; 
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.08);
  border-radius: 8px;
  padding: 16px 20px;
  display: flex;
  align-items: flex-start;
  gap: 20px;
  z-index: 9999;
  min-width: 320px;
  max-width: 420px;
  animation: slideInToast 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes slideInToast {
  from { transform: translateX(120%); opacity: 0; }
  to { transform: translateX(0); opacity: 1; }
}

.toast-content {
  display: flex;
  flex-direction: column;
  gap: 4px;
  text-align: left;
}

.toast-title {
  font-size: 0.85rem;
  font-weight: 700;
  color: #111827;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.toast-text {
  font-size: 0.85rem;
  color: #4b5563;
  line-height: 1.4;
}

.btn-close-toast {
  background: none;
  border: none;
  font-size: 1.3rem;
  color: #9ca3af;
  cursor: pointer;
  padding: 0;
  line-height: 1;
  margin-left: auto;
  transition: color 0.2s;
}

.btn-close-toast:hover {
  color: #111827;
}

@keyframes scaleInMockup {
  from { transform: scale(0.95); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}

.form-panel-header-central {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  border-bottom: 1px solid #e5e7eb;
  padding-bottom: 12px;
}
.form-panel-header-central h3 { font-size: 1.1rem; font-weight: 700; color: #111827; }
.btn-close-modal { background: none; border: none; font-size: 1.5rem; color: #9ca3af; cursor: pointer; }
.btn-close-modal:hover { color: #374151; }

.form-solicitud-mockup { display: flex; flex-direction: column; gap: 14px; }
.form-group-mockup { display: flex; flex-direction: column; gap: 4px; }
.form-group-mockup label { font-size: 0.8rem; font-weight: 600; color: #374151; }
.form-group-mockup input, .form-solicitud-mockup textarea { padding: 10px; border: 1px solid #e5e7eb; border-radius: 8px; font-size: 0.85rem; width: 100%; }
.form-row-mockup { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }

.form-actions-central {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 12px;
}
.btn-cancel-mockup { background-color: #f3f4f6; color: #374151; padding: 10px 16px; border-radius: 8px; border: none; cursor: pointer; font-weight: 600; font-size: 0.85rem; }
.btn-cancel-mockup:hover { background-color: #e5e7eb; }
.btn-submit-mockup { background: #000000; color: #fff; padding: 10px 20px; border-radius: 8px; font-weight: 600; border: none; cursor: pointer; font-size: 0.85rem; }
.btn-submit-mockup:hover { opacity: 0.9; }
</style>