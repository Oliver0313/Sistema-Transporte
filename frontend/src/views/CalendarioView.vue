<template>
    <div class="dashboard-header">
      <div class="dashboard-header-left">
        <h2>Calendario</h2>
        <p>Administra la agenda, viajes programados y disponibilidad de la flota.</p>
      </div>
    </div>

    <div class="mant-cards-row">
  <div class="mant-card">
    <div class="mant-card-icon">
      <CalendarDays :size="24" />
    </div>

    <div class="mant-card-info">
      <span>Viajes este mes</span>
      <h3>{{ totalViajesMes }}</h3>
      <small>Programados en agenda</small>
    </div>
  </div>

  <div class="mant-card">
    <div class="mant-card-icon">
      <CircleCheck :size="24" />
    </div>

    <div class="mant-card-info">
      <span>Confirmados</span>
      <h3>{{ totalConfirmados }}</h3>
      <small>Viajes aprobados</small>
    </div>
  </div>

  <div class="mant-card">
    <div class="mant-card-icon">
      <Clock :size="24" />
    </div>

    <div class="mant-card-info">
      <span>Pendientes</span>
      <h3>{{ totalPendientes }}</h3>
      <small>Esperando aprobación</small>
    </div>
  </div>

  <div class="mant-card">
    <div class="mant-card-icon">
      <UserRound :size="24" />
    </div>

    <div class="mant-card-info">
      <span>Conductores activos</span>
      <h3>{{ conductores.length }}</h3>
      <small>Disponibles en el sistema</small>
    </div>
  </div>
</div>

    <div class="calendar-filters-top">
      <select v-model="filtroConductor" class="mockup-select-modern">
        <option value="">Todos los conductores</option>
        <option v-for="c in conductores" :key="c.id" :value="c.id">
          {{ c.nombre }} {{ c.apellido }}
        </option>
      </select>

      <select v-model="filtroVehiculo" class="mockup-select-modern">
        <option value="">Todos los vehículos</option>
        <option v-for="v in vehiculos" :key="v.id" :value="v.id">
          {{ v.marca }} {{ v.modelo }} - {{ v.matricula }}
        </option>
      </select>

      <input type="date" v-model="filtroFechaDirecta" class="mockup-date-modern" />
      
      <button class="btn-today-action" @click="irAHoy">Hoy</button>
      <button class="btn-clear-filters" @click="limpiarFiltros">Limpiar Filtros</button>
    </div>

    <div class="calendar-layout-grid">
      
      <div class="main-calendar-card card-panel-mockup">
        <div class="main-calendar-navigator">
          <div class="nav-month-controls">
            <button class="btn-nav-arrow" @click="cambiarMes(-1)">◀</button>
            <span class="current-month-display">{{ nombreMesActual }} {{ anioActual }}</span>
            <button class="btn-nav-arrow" @click="cambiarMes(1)">▶</button>
          </div>
          <span class="total-indicators-text">{{ solicitudesFiltradas.length }} viajes programados</span>
          
       
        </div>

        <div class="grid-calendar-table">
          <div class="grid-weekday-header">Dom</div>
          <div class="grid-weekday-header">Lun</div>
          <div class="grid-weekday-header">Mar</div>
          <div class="grid-weekday-header">Mie</div>
          <div class="grid-weekday-header">Jue</div>
          <div class="grid-weekday-header">Vie</div>
          <div class="grid-weekday-header">Sab</div>

          <div 
            v-for="(dia, index) in matrizDiasMes" 
            :key="index" 
            :class="['grid-day-cell', { 'day-outside-bounds': !dia.esMesActual, 'day-today-highlight': dia.esHoy }]"
          >
            <span class="day-number-label">{{ dia.numero }}</span>
            
            <div class="day-events-stack">
              <span 
                v-for="viaje in obtenerViajesPorDia(dia.fechaCompleta)" 
                :key="viaje.id"
                :class="['event-badge-pill', obtenerClaseBadgeEstado(viaje.estado)]"
                @click.stop="seleccionarViajeDetalle(viaje)"
              >
                {{ generarInicialesConductor(viaje) }} - {{ viaje.destino }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <div class="side-calendar-panel card-panel-mockup">
        <div class="side-mini-navigator-box">
          <h4>Navegación rapida</h4>
          <div class="mini-month-selector">
            <button @click="cambiarMes(-1)">◀</button>
            <span>{{ nombreMesActual }} {{ anioActual }}</span>
            <button @click="cambiarMes(1)">▶</button>
          </div>

          <div class="mini-calendar-grid">
            <span v-for="d in ['D','L','M','M','J','V','S']" :key="d" class="mini-grid-th">{{ d }}</span>
            <span 
              v-for="(dia, idx) in matrizDiasMes" 
              :key="idx" 
              :class="['mini-grid-td', { 'mini-outside': !dia.esMesActual, 'mini-active-today': dia.esHoy }]"
            >
              {{ dia.numero }}
            </span>
          </div>
        </div>

        <div class="side-agenda-wrapper">
          
          <div v-if="viajeSeleccionado" class="side-detail-view">
            <div class="detail-header-row">
              <h4>Detalle del viaje</h4>
              <button class="btn-close-detail" @click="viajeSeleccionado = null">×</button>
            </div>
            <div class="detail-card-body">
              <div class="detail-item-group">
                <label>Área Solicitante</label>
                <p>{{ viajeSeleccionado.areaSolicitante }}</p>
              </div>
              <div class="detail-item-group">
                <label>Ruta / Destino</label>
                <p>📍 {{ viajeSeleccionado.destino }}</p>
              </div>
              <div class="detail-item-group">
                <label>Motivo</label>
                <p class="detail-bubble-text">{{ viajeSeleccionado.motivo }}</p>
              </div>
              <div class="detail-item-group">
                <label>Pasajeros</label>
                <p>👥 {{ viajeSeleccionado.cantidadColaboradores }} colaboradores</p>
              </div>
              <div class="detail-item-group">
                <label>Conductor</label>
                <p>🪪 {{ viajeSeleccionado.conductor ? `${viajeSeleccionado.conductor.nombre} ${viajeSeleccionado.conductor.apellido}` : 'Sin asignar' }}</p>
              </div>
              <div class="detail-item-group">
                <label>Vehículo asignado</label>
                <p>🚘 {{ viajeSeleccionado.vehiculo ? `${viajeSeleccionado.vehiculo.marca} ${viajeSeleccionado.vehiculo.modelo} (${viajeSeleccionado.vehiculo.matricula})` : 'Sin asignar' }}</p>
              </div>
              <span :class="['status-pill-mockup', obtenerClaseBadgeEstado(viajeSeleccionado.estado)]" style="margin-top:8px;">
                {{ {1:'Pendiente', 2:'Aprobada', 3:'Rechazada', 4:'Cancelada', 5:'Finalizada'}[viajeSeleccionado.estado] }}
              </span>
            </div>
          </div>

          <div v-else class="side-agenda-view">
            <h4>Viajes de hoy</h4>
            <div class="agenda-items-scroll">
              <div 
                v-for="viajeHoy in viajesDeHoyLista" 
                :key="viajeHoy.id" 
                class="agenda-timeline-item"
                @click="seleccionarViajeDetalle(viajeHoy)"
                style="cursor:pointer;"
              >
                <div class="timeline-bullet" :class="obtenerClaseEstadoCirculo(viajeHoy.estado)"></div>
                <div class="timeline-content-card">
                  <h5>{{ viajeHoy.conductor ? `${viajeHoy.conductor.nombre} ${viajeHoy.conductor.apellido}` : 'Sin Conductor' }}</h5>
                  <p class="timeline-time-sub text-muted">Hoy ~ {{ extraerHoraFormateada(viajeHoy.fechaHoraSalida) }}</p>
                  <p class="timeline-route-desc"><strong>Destino:</strong> {{ viajeHoy.destino }}</p>
                </div>
              </div>

              <div v-if="viajesDeHoyLista.length === 0" class="text-center text-muted empty-agenda-placeholder">
                No hay viajes programados para el día de hoy.
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

import {
  CalendarDays,
  CircleCheck,
  Clock,
  UserRound,
  ChevronLeft,
  ChevronRight,
  MapPin,
  Users,
  IdCard,
  CarFront,
  X
} from 'lucide-vue-next'

const rolUsuario = ref(localStorage.getItem('usuario_rol') || 'Operador')


const esAdmin = computed(() => {
  const rol = rolUsuario.value.toLowerCase()
  return rol === 'administrador' || rol === 'admin' || rol === 'superadmin'
})
const puedeCrear = computed(() => esAdmin.value)


const fechaReferencia    = ref(new Date())
const filtroConductor     = ref('')
const filtroVehiculo      = ref('')
const filtroFechaDirecta  = ref('')
const viajeSeleccionado   = ref(null)

const solicitudes = ref([])
const vehiculos   = ref([])
const conductores = ref([])

const anioActual = computed(() => fechaReferencia.value.getFullYear())
const mesActual  = computed(() => fechaReferencia.value.getMonth())

const nombreMesActual = computed(() => {
  return new Intl.DateTimeFormat('es-ES', { month: 'long' }).format(fechaReferencia.value)
    .replace(/^\w/, (c) => c.toUpperCase())
})

const totalViajesMes = computed(() => {
  return solicitudes.value.filter(s => {
    const f = new Date(s.fechaHoraSalida)
    return f.getFullYear() === anioActual.value && f.getMonth() === mesActual.value
  }).length
})

const totalConfirmados = computed(() => solicitudes.value.filter(s => s.estado === 2).length)
const totalPendientes  = computed(() => solicitudes.value.filter(s => s.estado === 1).length)

const matrizDiasMes = computed(() => {
  const primerDiaMes = new Date(anioActual.value, mesActual.value, 1)
  const ultimoDiaMes = new Date(anioActual.value, mesActual.value + 1, 0)
  
  const diasArr = []
  const desplazamientoInicial = primerDiaMes.getDay()
  
  for (let i = desplazamientoInicial - 1; i >= 0; i--) {
    const d = new Date(anioActual.value, mesActual.value, -i)
    diasArr.push({ numero: d.getDate(), esMesActual: false, esHoy: verificarEsHoy(d), fechaCompleta: d })
  }
  
  for (let i = 1; i <= ultimoDiaMes.getDate(); i++) {
    const d = new Date(anioActual.value, mesActual.value, i)
    diasArr.push({ numero: i, esMesActual: true, esHoy: verificarEsHoy(d), fechaCompleta: d })
  }
  
  const huecosRestantes = 42 - diasArr.length
  for (let i = 1; i <= huecosRestantes; i++) {
    const d = new Date(anioActual.value, mesActual.value + 1, i)
    diasArr.push({ numero: d.getDate(), esMesActual: false, esHoy: verificarEsHoy(d), fechaCompleta: d })
  }
  
  return diasArr
})


const solicitudesFiltradas = computed(() => {
  return solicitudes.value.filter(s => {
    
    const esAprobada = s.estado === 2
    
    const cumpleConductor = !filtroConductor.value || s.conductorId === Number(filtroConductor.value)
    const cumpleVehiculo  = !filtroVehiculo.value  || s.vehiculoId === Number(filtroVehiculo.value)
    
    let cumpleFecha = true
    if (filtroFechaDirecta.value) {
      cumpleFecha = s.fechaHoraSalida.substring(0, 10) === filtroFechaDirecta.value
    }
    
    return esAprobada && cumpleConductor && cumpleVehiculo && cumpleFecha
  })
})

const viajesDeHoyLista = computed(() => {
  const hoyStr = new Date().toISOString().substring(0, 10)
  return solicitudes.value.filter(s => s.estado === 2 && s.fechaHoraSalida.substring(0, 10) === hoyStr)
})

const verificarEsHoy = (fecha) => {
  const hoy = new Date()
  return fecha.getDate() === hoy.getDate() &&
         fecha.getMonth() === hoy.getMonth() &&
         fecha.getFullYear() === hoy.getFullYear()
}

const cambiarMes = (direccion) => {
  fechaReferencia.value = new Date(anioActual.value, mesActual.value + direccion, 1)
}

const irAHoy = () => {
  fechaReferencia.value = new Date()
  filtroFechaDirecta.value = ''
}


const limpiarFiltros = () => {
  filtroConductor.value = ''
  filtroVehiculo.value = ''
  filtroFechaDirecta.value = ''
  viajeSeleccionado.value = null
}

const seleccionarViajeDetalle = (viaje) => {
  viajeSeleccionado.value = viaje
}

const obtenerViajesPorDia = (fechaCelda) => {
  const celdaStr = fechaCelda.toISOString().substring(0, 10)
  return solicitudesFiltradas.value.filter(s => s.fechaHoraSalida.substring(0, 10) === celdaStr)
}

const generarInicialesConductor = (viaje) => {
  if (viaje.conductor && viaje.conductor.nombre) {
    const inicial = viaje.conductor.nombre.charAt(0).toUpperCase()
    const apellido = viaje.conductor.apellido || ''
    return `${inicial}.${apellido}`
  }
  return 'S/A'
}

const extraerHoraFormateada = (fechaIso) => {
  if (!fechaIso) return ''
  return new Date(fechaIso).toLocaleTimeString('es-DO', { hour: '2-digit', minute: '2-digit', hour12: true })
}

const obtenerClaseBadgeEstado = (estado) => {
  return { 1: 'badge-pendiente', 2: 'badge-aprobada', 3: 'badge-rechazada', 4: 'badge-cancelada', 5: 'badge-finalizada' }[estado] || 'badge-pendiente'
}

const obtenerClaseEstadoCirculo = (estado) => {
  return { 1: 'bullet-yellow', 2: 'bullet-green', 3: 'bullet-red', 4: 'bullet-gray', 5: 'bullet-blue' }[estado] || 'bullet-yellow'
}

const redireccionarNuevaSolicitud = () => {
  window.location.hash = '/solicitudes' 
}

const fetchCalendarioCoreData = async () => {
  const token = localStorage.getItem('token_transporte')
  const headers = { Authorization: `Bearer ${token}` }

  try {
    const [resS, resA, resV, resC] = await Promise.allSettled([
      fetch('https://localhost:7221/api/solicitudestransporte', { headers }),
      fetch('https://localhost:7221/api/asignaciones', { headers }),
      fetch('https://localhost:7221/api/vehiculos', { headers }),
      fetch('https://localhost:7221/api/conductores', { headers })
    ])

    let dataSolicitudes = []
    let dataAsignaciones = []
    let dataVehiculos = []
    let dataConductores = []

    if (resS.status === 'fulfilled' && resS.value.ok) {
      dataSolicitudes = await resS.value.json()
    }

    if (resA.status === 'fulfilled' && resA.value.ok) {
      dataAsignaciones = await resA.value.json()
    }

    if (resV.status === 'fulfilled' && resV.value.ok) {
      dataVehiculos = await resV.value.json()
    }

    if (resC.status === 'fulfilled' && resC.value.ok) {
      dataConductores = await resC.value.json()
    }

    vehiculos.value = dataVehiculos
    conductores.value = dataConductores

    solicitudes.value = dataSolicitudes.map(s => {
      const asignacion = dataAsignaciones.find(a => a.solicitudTransporteId === s.id)

      const vehiculo = asignacion
        ? dataVehiculos.find(v => v.id === asignacion.vehiculoId)
        : null

      const conductor = asignacion
        ? dataConductores.find(c => c.id === asignacion.conductorId)
        : null

      return {
        ...s,
        vehiculoId: asignacion?.vehiculoId || null,
        conductorId: asignacion?.conductorId || null,
        vehiculo,
        conductor
      }
    })
  } catch (error) {
    console.error('Error cargando datos del calendario:', error)
  }
}

onMounted(async () => {
  await fetchCalendarioCoreData()
})
</script>

<style scoped>

.calendar-page-container,
.calendar-page-container * {
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif !important;
}

.calendar-page-container {
  display: flex;
  flex-direction: column;
  gap: 20px;
}


.metrics-grid-mockup {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
}

.metric-card-item {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 16px;
  display: flex;
  align-items: center;
  gap: 14px;
}

.metric-icon-box {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 44px;
  height: 44px;
  border-radius: 12px;
  flex-shrink: 0;
}

.metric-png-icon {
  width: 20px;
  height: 20px;
  object-fit: contain;
}

.metric-data-box {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.metric-label {
  font-size: 0.82rem !important;
  color: #6b7280 !important;
  font-weight: 500 !important;
  text-transform: none !important;
  order: 1; 
}

.metric-value {
  font-size: 1.3rem !important;
  font-weight: 700 !important;
  color: #111827 !important;
  line-height: 1.2 !important;
  margin: 0 !important;
  order: 2; 
}


.bg-light-blue   { background: #eff6ff; }
.bg-light-green  { background: #f0fdf4; }
.bg-light-purple { background: #f5f3ff; }
.bg-light-yellow { background: #fffbeb; }

.calendar-filters-top {
  display: flex;
  gap: 12px;
  background: white;
  padding: 16px;
  border-radius: 16px;
  border: 1px solid #e5e7eb;
  align-items: center; 
}

.mockup-select-modern, 
.mockup-date-modern {
  height: 40px;
  padding: 0 14px;
  border-radius: 10px;
  border: 1px solid #e5e7eb;
  font-size: 0.88rem;
  outline: none;
  background: white;
  box-sizing: border-box;
}

.btn-today-action {
  height: 40px;
  background: #f3f4f6;
  color: #111827;
  border: 1px solid #e5e7eb;
  padding: 0 16px;
  border-radius: 10px;
  font-weight: 700;
  font-size: 0.88rem;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  box-sizing: border-box;
}

.btn-clear-filters {
  height: 40px;
  background: #111827;
  color: #ffffff;
  border: none;
  padding: 0 18px;
  border-radius: 10px;
  font-weight: 700;
  font-size: 0.9rem;
  cursor: pointer;
  transition: opacity 0.2s ease;
  display: inline-flex;
  align-items: center;
  box-sizing: border-box;
}

.btn-clear-filters:hover {
  opacity: 0.92;
}


.calendar-layout-grid {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 20px;
  align-items: start;
}

.card-panel-mockup {
  background: white;
  border-radius: 20px;
  border: 1px solid #e5e7eb;
  padding: 20px;
}

.main-calendar-navigator {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 20px;
}

.nav-month-controls {
  display: flex;
  align-items: center;
  gap: 10px;
  background: #f3f4f6;
  padding: 6px 12px;
  border-radius: 12px;
}

.btn-nav-arrow {
  background: transparent;
  border: none;
  cursor: pointer;
  font-size: 0.85rem;
}

.current-month-display {
  font-weight: 800;
  font-size: 1rem;
  color: #111827;
  min-width: 110px;
  text-align: center;
}

.total-indicators-text,
.empty-agenda-placeholder,
.rep-empty {
  font-size: 0.8rem !important;
  color: #6b7280 !important;
  font-weight: 500;
}

.btn-shortcut-new-trip {
  margin-left: auto;
  background: #111827;
  color: white;
  padding: 10px 18px;
  border-radius: 10px;
  font-size: 0.9rem;
  font-weight: 700;
  border: none;
  cursor: pointer;
  transition: opacity 0.2s ease;
}

.btn-shortcut-new-trip:hover:not(:disabled) {
  opacity: 0.92;
}

.btn-shortcut-new-trip.btn-disabled {
  opacity: 0.35;
  cursor: not-allowed;
  background: #9ca3af;
}

.grid-calendar-table {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  overflow: hidden;
}

.grid-weekday-header {
  background: #f9fafb;
  padding: 12px;
  text-align: center;
  font-size: 0.82rem;
  font-weight: 700;
  color: #4b5563;
  border-bottom: 1px solid #e5e7eb;
}

.grid-day-cell {
  min-height: 95px;
  background: white;
  border-right: 1px solid #e5e7eb;
  border-bottom: 1px solid #e5e7eb;
  padding: 6px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.grid-day-cell:nth-child(7n) {
  border-right: none;
}

.mant-cards-row {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 18px;
  margin-bottom: 24px;
}

.mant-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 18px;
  display: flex;
  align-items: center;
  gap: 14px;
  box-shadow: 0 8px 22px rgba(15, 23, 42, 0.04);
}

.mant-card-icon {
  width: 48px;
  height: 48px;
  min-width: 48px;
  border-radius: 14px;
  background: #f3f4f6;
  color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
}

.mant-card-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.mant-card-info span {
  font-size: 0.88rem;
  font-weight: 700;
  color: #374151;
}

.mant-card-info h3 {
  margin: 0;
  font-size: 1.65rem;
  font-weight: 800;
  color: #111827;
  line-height: 1.1;
}

.mant-card-info small {
  font-size: 0.78rem;
  color: #9ca3af;
}

.day-number-label {
  font-size: 0.82rem;
  font-weight: 700;
  color: #374151;
}

.day-outside-bounds {
  background: #f9fafb;
  opacity: 0.45;
}

.day-today-highlight {
  background: #eff6ff;
}

.day-today-highlight .day-number-label {
  background: #1d4ed8;
  color: white;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.day-events-stack {
  display: flex;
  flex-direction: column;
  gap: 3px;
  overflow-y: auto;
  max-height: 65px;
}


.event-badge-pill {
  padding: 3px 6px;
  border-radius: 6px;
  font-size: 0.7rem;
  font-weight: 700;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  cursor: pointer;
}

.badge-pendiente { background: #fef3c7; color: #92400e; border-left: 3px solid #d97706; }
.badge-aprobada   { background: #dcfce7; color: #166534; border-left: 3px solid #16a34a; }
.badge-rechazada  { background: #fee2e2; color: #991b1b; border-left: 3px solid #dc2626; }
.badge-cancelada  { background: #f3f4f6; color: #4b5563; border-left: 3px solid #9ca3af; }
.badge-finalizada { background: #e0f2fe; color: #0369a1; border-left: 3px solid #0284c7; }

.status-pill-mockup {
  display: inline-block;
  padding: 4px 10px;
  border-radius: 99px;
  font-size: 0.72rem;
  font-weight: 700;
}

.side-calendar-panel {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.side-mini-navigator-box h4, 
.side-agenda-wrapper h4 {
  margin: 0 0 14px 0;
  font-size: 0.95rem;
  font-weight: 700;
  color: #111827;
}

.mini-month-selector {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 0.82rem;
  font-weight: 700;
  margin-bottom: 10px;
}

.mini-month-selector button {
  background: transparent;
  border: none;
  cursor: pointer;
}

.mini-calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  text-align: center;
  gap: 4px;
}

.mini-grid-th {
  font-size: 0.72rem;
  font-weight: 700;
  color: #9ca3af;
  padding-bottom: 4px;
}

.mini-grid-td {
  font-size: 0.78rem;
  font-weight: 600;
  color: #374151;
  padding: 4px 0;
  border-radius: 6px;
}

.mini-outside {
  opacity: 0.3;
}

.mini-active-today {
  background: #1d4ed8;
  color: white !important;
  font-weight: 700;
}

.detail-header-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.btn-close-detail {
  background: transparent;
  border: none;
  font-size: 1.3rem;
  color: #9ca3af;
  cursor: pointer;
}

.detail-card-body {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  padding: 12px;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.detail-item-group label {
  font-size: 0.68rem;
  color: #6b7280;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.03em;
}

.detail-item-group p {
  margin: 2px 0 0 0;
  font-size: 0.85rem;
  color: #111827;
  font-weight: 600;
}

.detail-bubble-text {
  background: white;
  padding: 8px;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
  font-weight: 500 !important;
}


.agenda-items-scroll {
  display: flex;
  flex-direction: column;
  gap: 14px;
  max-height: 280px;
  overflow-y: auto;
  padding-right: 4px;
}

.agenda-timeline-item {
  display: flex;
  gap: 12px;
  position: relative;
}

.timeline-bullet {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-top: 5px;
  flex-shrink: 0;
}

.timeline-content-card {
  background: #f9fafb;
  padding: 10px 12px;
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  flex: 1;
}

.timeline-content-card h5 {
  margin: 0;
  font-size: 0.82rem;
  font-weight: 700;
  color: #111827;
}

.timeline-time-sub {
  font-size: 0.72rem;
  margin: 2px 0 6px 0;
  color: #6b7280;
}

.timeline-route-desc {
  font-size: 0.78rem;
  margin: 0;
  color: #4b5563;
}


.bullet-yellow { background: #fbbf24; box-shadow: 0 0 0 3px #fef3c7; }
.bullet-green  { background: #4ade80; box-shadow: 0 0 0 3px #dcfce7; }
.bullet-red    { background: #f87171; box-shadow: 0 0 0 3px #fee2e2; }
.bullet-gray   { background: #9ca3af; box-shadow: 0 0 0 3px #f3f4f6; }
.bullet-blue   { background: #60a5fa; box-shadow: 0 0 0 3px #e0f2fe; }
</style>