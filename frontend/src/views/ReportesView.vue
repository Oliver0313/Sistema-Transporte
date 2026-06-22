<template>
  <div class="rep-page">

    <div class="section-header">
      <h2>Gestión de reportes</h2>
      <p>Creación y exportación de reportes.</p>
    </div>

    <div class="rep-filters-bar">
      <div class="rep-filter-pill-group">
        <span class="rep-filter-label">Periodo:</span>
        <select class="rep-pill-select" v-model="filtroPeriodo" :disabled="!puedeModificarFiltros">
          <option value="1">Enero</option>
          <option value="2">Febrero</option>
          <option value="3">Marzo</option>
          <option value="4">Abril</option>
          <option value="5">Mayo</option>
          <option value="6">Junio</option>
          <option value="7">Julio</option>
          <option value="8">Agosto</option>
          <option value="9">Septiembre</option>
          <option value="10">Octubre</option>
          <option value="11">Noviembre</option>
          <option value="12">Diciembre</option>
        </select>
      </div>

      <div class="rep-filter-pill-group">
        <span class="rep-filter-label">Reporte:</span>
        <select class="rep-pill-select" v-model="filtroModulo" :disabled="!puedeModificarFiltros">
          <option value="todos">Todos</option>
          <option value="viajes">Viajes</option>
          <option value="combustible">Combustible</option>
          <option value="solicitudes">Solicitudes</option>
          <option value="conductores">Conductores</option>
        </select>
      </div>

      <div class="rep-filter-pill-group">
        <span class="rep-filter-label">Area:</span>
        <select class="rep-pill-select" v-model="filtroArea" :disabled="!puedeModificarFiltros">
          <option value="todas">Todas las areas</option>
          <option v-for="area in areasDisponibles" :key="area" :value="area">{{ area }}</option>
        </select>
      </div>

      <button v-if="puedeModificarFiltros" class="rep-btn-generar" @click="cargarReportes" :disabled="isLoading">
        {{ isLoading ? 'Cargando...' : 'Generar' }}
      </button>

      <button v-if="puedeModificarFiltros" class="rep-btn-limpiar" @click="limpiarFiltros" :disabled="isLoading">
        Limpiar Filtros
      </button>

      <div v-else class="badge-read-only">👁️ Vista de lectura</div>

      <div class="rep-export-group">
        <button class="btn-export pdf" @click="exportarExcel" :disabled="isLoading" title="Exportar Excel">
          <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><rect x="3" y="3" width="18" height="18" rx="2"/><line x1="3" y1="9" x2="21" y2="9"/><line x1="3" y1="15" x2="21" y2="15"/><line x1="9" y1="3" x2="9" y2="21"/></svg>
          Excel
        </button>
        <button class="btn-export excel" @click="exportarPDF" :disabled="isLoading" title="Imprimir / PDF">
          <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/><polyline points="14,2 14,8 20,8"/></svg>
          PDF
        </button>
      </div>
    </div>

    <div v-if="isLoading" class="rep-loading-card">
      <div class="rep-loading-spinner"></div>
      <p>Cargando métricas...</p>
    </div>

    <template v-else>
      <div class="rep-kpi-grid">
        <div class="rep-kpi-card">
          <div class="rep-kpi-icon-wrap rep-icon-green">
            <img src="../assets/icons/vehiculo-negro.png" alt="Viajes" class="rep-kpi-img" />
          </div>
          <div class="rep-kpi-valor">{{ kpis.viajes.totalViajes.toLocaleString() }}</div>
          <div class="rep-kpi-label">Viajes realizados</div>
        </div>

        <div class="rep-kpi-card">
          <div class="rep-kpi-icon-wrap rep-icon-blue">
            <img src="../assets/icons/gota-negra.png" alt="Combustible" class="rep-kpi-img" />
          </div>
          <div class="rep-kpi-valor">{{ kpis.combustible.totalGalones.toLocaleString() }} L</div>
          <div class="rep-kpi-label">Consumo de combustible</div>
        </div>

        <div class="rep-kpi-card">
          <div class="rep-kpi-icon-wrap rep-icon-yellow">
            <img src="../assets/icons/hoja.png" alt="Solicitudes" class="rep-kpi-img" />
          </div>
          <div class="rep-kpi-valor">{{ kpis.solicitudes.totalSolicitudes.toLocaleString() }}</div>
          <div class="rep-kpi-label">Solicitudes procesadas</div>
        </div>

        <div class="rep-kpi-card">
          <div class="rep-kpi-icon-wrap rep-icon-purple">
            <img src="../assets/icons/personas.png" alt="Conductores" class="rep-kpi-img" />
          </div>
          <div class="rep-kpi-valor">{{ kpis.conductores.totalConductores.toLocaleString() }}</div>
          <div class="rep-kpi-label">Conductores activos</div>
        </div>
      </div>

      <div class="rep-charts-row">
        <div class="rep-chart-card" v-if="filtroModulo === 'todos' || filtroModulo === 'viajes'">
          <div class="rep-chart-title">Viajes por mes</div>
          <div class="rep-chart-subtitle">{{ mesesLabel }} {{ filtroAnio }}</div>
          <div v-if="viajesPorMes.length === 0" class="rep-empty">Sin registros mensuales</div>
          <div v-else class="rep-bar-chart">
            <div v-for="(mes, idx) in viajesPorMes" :key="idx" class="rep-bar-col">
              <div class="rep-bar-wrapper">
                <div class="rep-bar-fill dark-bar" :style="{ height: obtenerAlturaBarra(mes.cantidad, maxViajesMes) }"></div>
              </div>
              <div class="rep-bar-label">{{ mes.mes }}</div>
            </div>
          </div>
        </div>

        <div class="rep-chart-card" v-if="filtroModulo === 'todos' || filtroModulo === 'combustible'">
          <div class="rep-chart-title">Consumo de combustible</div>
          <div class="rep-chart-subtitle">Litros por mes</div>
          <div v-if="combustiblePorMes.length === 0" class="rep-empty">Sin registros mensuales</div>
          <div v-else class="rep-bar-chart">
            <div v-for="(mes, idx) in combustiblePorMes" :key="idx" class="rep-bar-col">
              <div class="rep-bar-wrapper">
                <div class="rep-bar-fill dark-bar" :style="{ height: obtenerAlturaBarra(mes.galones, maxCombustibleMes) }"></div>
              </div>
              <div class="rep-bar-label">{{ mes.mes }}</div>
            </div>
          </div>
        </div>

        <div class="rep-chart-card rep-vehiculos-card" v-if="filtroModulo === 'todos' || filtroModulo === 'viajes'">
          <div class="rep-vehiculos-header">
            <div class="rep-chart-title">Vehiculos mas utilizados</div>
            <div class="rep-chart-subtitle">Por número de viajes</div>
          </div>
          <div v-if="vehiculosMasUsados.length === 0" class="rep-empty">Sin registros de uso</div>
          <div v-else class="rep-vehiculos-list">
            <div v-for="(item, idx) in vehiculosMasUsados" :key="idx" class="rep-vehiculo-row">
              <span class="rep-vehiculo-name">{{ item.vehiculo }}</span>
              <div class="rep-vehiculo-bar-track">
                <div class="rep-vehiculo-bar-fill" :style="{ width: obtenerPorcentaje(item.viajes, vehiculosMasUsados[0]?.viajes) }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="rep-metricas-panel" v-if="filtroModulo === 'todos' || filtroModulo === 'combustible'">
        <div class="rep-metricas-header">
          <span class="rep-metricas-title">Métricas de combustible</span>
          <span class="rep-badge-gray">Eficiencia general</span>
        </div>
        <div class="rep-metricas-grid">
          <div class="rep-metrica-item">
            <span class="rep-metrica-label">TOTAL GASTADO</span>
            <span class="rep-metrica-valor">{{ formatearCosto(kpis.combustible.totalGastado) }}</span>
          </div>
          <div class="rep-metrica-item">
            <span class="rep-metrica-label">KILÓMETROS TOTALES</span>
            <span class="rep-metrica-valor">{{ kpis.combustible.totalKilometros.toLocaleString() }} km</span>
          </div>
          <div class="rep-metrica-item">
            <span class="rep-metrica-label">COSTO PROMEDIO / GALÓN</span>
            <span class="rep-metrica-valor">{{ formatearCosto(kpis.combustible.costoPromedioPorGalon) }}</span>
          </div>
          <div class="rep-metrica-item">
            <span class="rep-metrica-label">RENDIMIENTO PROMEDIO</span>
            <span class="rep-metrica-valor">{{ kpis.combustible.rendimientoKmPorGalon.toFixed(2) }} km/gal</span>
          </div>
        </div>
      </div>

      <div class="rep-tabla-panel" v-if="filtroModulo === 'todos' || filtroModulo === 'viajes'">
        <div class="rep-metricas-header">
          <span class="rep-metricas-title">Viajes por estado</span>
        </div>
        <table class="custom-table-mockup">
          <thead>
            <tr>
              <th>Estado</th>
              <th>Cantidad</th>
              <th>Porcentaje</th>
              <th>Tendencia</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td><span class="status-pill-mockup pendiente">Programados</span></td>
              <td><strong>{{ kpis.viajes.programados }}</strong></td>
              <td>{{ obtenerPorcentaje(kpis.viajes.programados, kpis.viajes.totalViajes) }}</td>
              <td>
                <div class="rep-tend-track">
                  <div class="rep-tend-fill" style="background:#f59e0b" :style="{ width: obtenerPorcentaje(kpis.viajes.programados, kpis.viajes.totalViajes) }"></div>
                </div>
              </td>
            </tr>
            <tr>
              <td><span class="status-pill-mockup aprobada">En Curso</span></td>
              <td><strong>{{ kpis.viajes.enCurso }}</strong></td>
              <td>{{ obtenerPorcentaje(kpis.viajes.enCurso, kpis.viajes.totalViajes) }}</td>
              <td>
                <div class="rep-tend-track">
                  <div class="rep-tend-fill" style="background:#10b981" :style="{ width: obtenerPorcentaje(kpis.viajes.enCurso, kpis.viajes.totalViajes) }"></div>
                </div>
              </td>
            </tr>
            <tr>
              <td><span class="status-pill-mockup finalizada">Finalizados</span></td>
              <td><strong>{{ kpis.viajes.finalizados }}</strong></td>
              <td>{{ obtenerPorcentaje(kpis.viajes.finalizados, kpis.viajes.totalViajes) }}</td>
              <td>
                <div class="rep-tend-track">
                  <div class="rep-tend-fill" style="background:#3b82f6" :style="{ width: obtenerPorcentaje(kpis.viajes.finalizados, kpis.viajes.totalViajes) }"></div>
                </div>
              </td>
            </tr>
            <tr>
              <td><span class="status-pill-mockup rechazada">Cancelados</span></td>
              <td><strong>{{ kpis.viajes.cancelados }}</strong></td>
              <td>{{ obtenerPorcentaje(kpis.viajes.cancelados, kpis.viajes.totalViajes) }}</td>
              <td>
                <div class="rep-tend-track">
                  <div class="rep-tend-fill" style="background:#ef4444" :style="{ width: obtenerPorcentaje(kpis.viajes.cancelados, kpis.viajes.totalViajes) }"></div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="rep-lists-row">
        <div class="rep-list-card" v-if="filtroModulo === 'todos' || filtroModulo === 'solicitudes'">
          <div class="rep-chart-title" style="margin-bottom:14px">Solicitudes por área</div>
          <div v-if="solicitudesPorArea.length === 0" class="rep-empty">Sin datos en el área</div>
          <div v-for="(item, idx) in solicitudesPorArea" :key="idx" class="rep-rank-item">
            <div class="rep-rank-header">
              <span class="rep-rank-name">{{ item.area }}</span>
              <span class="rep-rank-count">{{ item.cantidad }}</span>
            </div>
            <div class="rep-vehiculo-bar-track">
              <div class="rep-vehiculo-bar-fill" style="background:#2563eb" :style="{ width: obtenerPorcentaje(item.cantidad, solicitudesPorArea[0]?.cantidad) }"></div>
            </div>
          </div>
        </div>

        <div class="rep-list-card" v-if="filtroModulo === 'todos' || filtroModulo === 'conductores'">
          <div class="rep-chart-title" style="margin-bottom:14px">Conductores destacados</div>
          <div v-if="conductoresMasViajes.length === 0" class="rep-empty">Sin datos en conductores</div>
          <div v-for="(item, idx) in conductoresMasViajes" :key="idx" class="rep-rank-item">
            <div class="rep-rank-header">
              <div style="display:flex;align-items:center;gap:8px">
                <span class="rep-rank-num">{{ idx + 1 }}</span>
                <span class="rep-rank-name">{{ item.conductor }}</span>
              </div>
              <span class="rep-rank-count">{{ item.viajes }} viajes</span>
            </div>
            <div class="rep-vehiculo-bar-track">
              <div class="rep-vehiculo-bar-fill" style="background:#16a34a" :style="{ width: obtenerPorcentaje(item.viajes, conductoresMasViajes[0]?.viajes) }"></div>
            </div>
          </div>
        </div>

        <div class="rep-list-card">
          <div class="rep-chart-title" style="margin-bottom:14px">Estado de vehículos</div>
          <div class="rep-rank-item">
            <div class="rep-rank-header"><span class="rep-rank-name">Disponibles</span><span class="rep-rank-count">{{ kpis.vehiculos.disponibles }}</span></div>
            <div class="rep-vehiculo-bar-track"><div class="rep-vehiculo-bar-fill" style="background:#10b981" :style="{ width: obtenerPorcentaje(kpis.vehiculos.disponibles, kpis.vehiculos.totalVehiculos) }"></div></div>
          </div>
          <div class="rep-rank-item">
            <div class="rep-rank-header"><span class="rep-rank-name">En Viaje</span><span class="rep-rank-count">{{ kpis.vehiculos.enViaje }}</span></div>
            <div class="rep-vehiculo-bar-track"><div class="rep-vehiculo-bar-fill" style="background:#3b82f6" :style="{ width: obtenerPorcentaje(kpis.vehiculos.enViaje, kpis.vehiculos.totalVehiculos) }"></div></div>
          </div>
          <div class="rep-rank-item">
            <div class="rep-rank-header"><span class="rep-rank-name">En Mantenimiento</span><span class="rep-rank-count">{{ kpis.vehiculos.enMantenimiento }}</span></div>
            <div class="rep-vehiculo-bar-track"><div class="rep-vehiculo-bar-fill" style="background:#f59e0b" :style="{ width: obtenerPorcentaje(kpis.vehiculos.enMantenimiento, kpis.vehiculos.totalVehiculos) }"></div></div>
          </div>
          <div class="rep-rank-item">
            <div class="rep-rank-header"><span class="rep-rank-name">Fuera de Servicio</span><span class="rep-rank-count">{{ kpis.vehiculos.fueraDeServicio }}</span></div>
            <div class="rep-vehiculo-bar-track"><div class="rep-vehiculo-bar-fill" style="background:#ef4444" :style="{ width: obtenerPorcentaje(kpis.vehiculos.fueraDeServicio, kpis.vehiculos.totalVehiculos) }"></div></div>
          </div>
        </div>
      </div>
    </template>

    <div v-if="mensajeFlotante" class="toast-error-moderno">
      <div class="toast-content">
        <span class="toast-title">Notificación</span>
        <p class="toast-text">{{ mensajeFlotante }}</p>
      </div>
      <button class="btn-close-toast" @click="mensajeFlotante = ''">×</button>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const API_BASE = 'https://localhost:7221/api/Reportes'

const rolUsuario = ref(localStorage.getItem('usuario_rol') || 'Operador')
const puedeModificarFiltros = computed(() => {
  const r = rolUsuario.value.toLowerCase()
  return r === 'superadmin' || r === 'administrador' || r === 'admin'
})

const isLoading       = ref(false)
const mensajeFlotante = ref('')
const filtroPeriodo   = ref(String(new Date().getMonth() + 1))
const filtroAnio      = ref(String(new Date().getFullYear()))
const filtroModulo    = ref('todos')
const filtroArea      = ref('todas')

const areasDisponibles = ref(['RRHH', 'Operaciones', 'Logística', 'Administración', 'TI'])
const mesesNombres = ['Enero','Febrero','Marzo','Abril','Mayo','Junio','Julio','Agosto','Septiembre','Octubre','Noviembre','Diciembre']
const mesesCortos  = ['Ene','Feb','Mar','Abr','May','Jun','Jul','Ago','Sep','Oct','Nov','Dic']

const mesesLabel = computed(() => {
  const idx = parseInt(filtroPeriodo.value) - 1
  return `${mesesCortos[0]} - ${mesesCortos[idx]}`
})

const kpis = ref({
  viajes:      { totalViajes: 0, programados: 0, enCurso: 0, finalizados: 0, cancelados: 0 },
  combustible: { totalGalones: 0, totalGastado: 0, totalKilometros: 0, costoPromedioPorGalon: 0, rendimientoKmPorGalon: 0 },
  solicitudes: { totalSolicitudes: 0 },
  conductores: { totalConductores: 0 },
  vehiculos:   { totalVehiculos: 0, disponibles: 0, enViaje: 0, enMantenimiento: 0, fueraDeServicio: 0 }
})

const viajesPorMes         = ref([])
const combustiblePorMes    = ref([])
const solicitudesPorArea   = ref([])
const vehiculosMasUsados   = ref([])
const conductoresMasViajes = ref([])

const limpiarFiltros = () => {
  filtroPeriodo.value = String(new Date().getMonth() + 1)
  filtroModulo.value = 'todos'
  filtroArea.value = 'todas'
  cargarReportes()
}

const cargarReportes = async () => {
  isLoading.value = true
  const token   = localStorage.getItem('token_transporte')
  const headers = { Authorization: `Bearer ${token}`, 'Content-Type': 'application/json' }

  try {
    const [resViajes, resCombustible, resSolicitudes, resConductores, resVehiculos] = await Promise.all([
      fetch(`${API_BASE}/viajes?mes=${filtroPeriodo.value}&anio=${filtroAnio.value}`, { headers }),
      fetch(`${API_BASE}/combustible?mes=${filtroPeriodo.value}&anio=${filtroAnio.value}`, { headers }),
      fetch(`${API_BASE}/solicitudes?mes=${filtroPeriodo.value}&anio=${filtroAnio.value}`, { headers }),
      fetch(`${API_BASE}/conductores`, { headers }),
      fetch(`${API_BASE}/vehiculos`, { headers })
    ])

    if (resViajes.ok)      kpis.value.viajes      = await resViajes.json()
    if (resCombustible.ok) kpis.value.combustible = await resCombustible.json()
    if (resSolicitudes.ok) kpis.value.solicitudes = await resSolicitudes.json()
    if (resConductores.ok) kpis.value.conductores = await resConductores.json()
    if (resVehiculos.ok)   kpis.value.vehiculos   = await resVehiculos.json()

    const resGraficos = await Promise.allSettled([
      fetch(`${API_BASE}/viajes-por-mes?anio=${filtroAnio.value}`, { headers }),
      fetch(`${API_BASE}/combustible-por-mes?anio=${filtroAnio.value}`, { headers }),
      fetch(`${API_BASE}/solicitudes-por-area`, { headers }),
      fetch(`${API_BASE}/vehiculos-mas-usados`, { headers }),
      fetch(`${API_BASE}/conductores-mas-viajes`, { headers }),
    ])

    const [rVxM, rCxM, rSxA, rVMU, rCMV] = resGraficos
    if (rVxM.status === 'fulfilled' && rVxM.value.ok)  viajesPorMes.value         = await rVxM.value.json()
    if (rCxM.status === 'fulfilled' && rCxM.value.ok)  combustiblePorMes.value    = await rCxM.value.json()
    if (rSxA.status === 'fulfilled' && rSxA.value.ok)  solicitudesPorArea.value   = await rSxA.value.json()
    if (rVMU.status === 'fulfilled' && rVMU.value.ok)  vehiculosMasUsados.value   = await rVMU.value.json()
    if (rCMV.status === 'fulfilled' && rCMV.value.ok)  conductoresMasViajes.value = await rCMV.value.json()

  } catch (e) {
    console.error('Error sincronizando datos:', e)
    mensajeFlotante.value = 'Inconveniente de comunicación con el backend .NET.'
  } finally {
    isLoading.value = false
  }
}

const maxViajesMes      = computed(() => viajesPorMes.value.length ? Math.max(...viajesPorMes.value.map(m => m.cantidad), 1) : 1)
const maxCombustibleMes = computed(() => combustiblePorMes.value.length ? Math.max(...combustiblePorMes.value.map(m => m.galones), 1) : 1)

const obtenerAlturaBarra = (valor, max) => `${Math.max((valor / max) * 100, 5)}%`

const obtenerPorcentaje = (valor, total) => {
  if (!total || total === 0) return '0%'
  return `${Math.round((valor * 100) / total)}%`
}

const formatearCosto = (c) =>
  new Intl.NumberFormat('es-DO', { style: 'currency', currency: 'DOP' }).format(c || 0)


const exportarExcel = () => {
  const nombreMes = mesesNombres[parseInt(filtroPeriodo.value) - 1].toUpperCase()
  
  const filas = [
    ['SISTEMA DE GESTIÓN DE TRANSPORTE - INFORME CONSOLIDADO GENERAL'],
    [`Periodo Evaluado: ${nombreMes} ${filtroAnio.value}`],
    [`Área de Filtro: ${filtroArea.value.toUpperCase()}`],
    [],
    ['1. INDICADORES CLAVE DE RENDIMIENTO (KPIs)'],
    ['Métrica', 'Valor Registrado'],
    ['Viajes Realizados', kpis.value.viajes.totalViajes],
    ['Consumo Total de Combustible (L)', kpis.value.combustible.totalGalones],
    ['Solicitudes Procesadas', kpis.value.solicitudes.totalSolicitudes],
    ['Conductores Activos en el Sistema', kpis.value.conductores.totalConductores],
    [],
    ['2. DISTRIBUCIÓN DE VIAJES POR ESTADO'],
    ['Estado del Viaje', 'Cantidad', 'Porcentaje'],
    ['Programados', kpis.value.viajes.programados, obtenerPorcentaje(kpis.value.viajes.programados, kpis.value.viajes.totalViajes)],
    ['En Curso', kpis.value.viajes.enCurso, obtenerPorcentaje(kpis.value.viajes.enCurso, kpis.value.viajes.totalViajes)],
    ['Finalizados', kpis.value.viajes.finalizados, obtenerPorcentaje(kpis.value.viajes.finalizados, kpis.value.viajes.totalViajes)],
    ['Cancelados', kpis.value.viajes.cancelados, obtenerPorcentaje(kpis.value.viajes.cancelados, kpis.value.viajes.totalViajes)],
    [],
    ['3. RENDIMIENTO Y MÉTRICAS DE COMBUSTIBLE'],
    ['Indicador de Eficiencia', 'Valor Computado'],
    ['Inversión / Gasto Total', formatearCosto(kpis.value.combustible.totalGastado)],
    ['Kilómetros Totales Recorridos', `${kpis.value.combustible.totalKilometros.toLocaleString()} km`],
    ['Costo Promedio por Galón', formatearCosto(kpis.value.combustible.costoPromedioPorGalon)],
    ['Rendimiento Promedio de Flota', `${kpis.value.combustible.rendimientoKmPorGalon.toFixed(2)} km/gal`],
    [],
    ['4. SOLICITUDES DE TRANSPORTE POR ÁREA DE LA EMPRESA'],
    ['Área Solicitante', 'Cantidad Solicitudes'],
    ...solicitudesPorArea.value.map(s => [s.area, s.cantidad]),
    [],
    ['5. TOP 5 - VEHÍCULOS MÁS UTILIZADOS'],
    ['Identificación del Vehículo', 'Cantidad de Viajes'],
    ...vehiculosMasUsados.value.map(v => [v.vehiculo, v.viajes]),
    [],
    ['6. RANKING - CONDUCTORES DESTACADOS'],
    ['Nombre Completo del Conductor', 'Viajes Completados'],
    ...conductoresMasViajes.value.map(c => [c.conductor, c.viajes]),
    [],
    ['7. DISPONIBILIDAD ACTUAL DE LA FLOTA AUTOMOTRIZ'],
    ['Condición / Estado de Operación', 'Unidades'],
    ['Disponibles para Asignación', kpis.value.vehiculos.disponibles],
    ['En Ruta / Viaje Activo', kpis.value.vehiculos.enViaje],
    ['En Taller / Mantenimiento', kpis.value.vehiculos.enMantenimiento],
    ['Fuera de Servicio', kpis.value.vehiculos.fueraDeServicio],
  ]

  const csv  = filas.map(f => f.map(cell => `"${String(cell).replace(/"/g, '""')}"`).join(',')).join('\n')
  const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' })
  const url  = URL.createObjectURL(blob)
  const a    = document.createElement('a')
  a.href     = url
  a.download = `Reporte_Consolidado_Flota_${filtroAnio.value}_${filtroPeriodo.value}.csv`
  a.click()
  URL.revokeObjectURL(url)
}

const exportarPDF = () => { window.print() }

onMounted(() => cargarReportes())
</script>

<style scoped>
.rep-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.section-header { margin-bottom: 20px; }
.section-header h2 { margin: 0; font-size: 1.5rem; font-weight: 700; color: #111827; }
.section-header p { margin-top: 6px; color: #6b7280; font-size: 0.9rem; }


.rep-filters-bar {
  display: flex;
  align-items: center;
  gap: 6px;
  background: #ffffff;
  padding: 14px 20px;
  border-radius: 18px;
  border: 1px solid #e5e7eb;
  flex-wrap: wrap;
}

.rep-filter-pill-group {
  display: flex;
  align-items: center;
  gap: 6px;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
  padding: 5px 10px 5px 12px;
}

.rep-filter-label { font-size: 0.82rem; font-weight: 600; color: #6b7280; white-space: nowrap; }
.rep-pill-select { border: none; background: transparent; font-size: 0.85rem; font-weight: 700; color: #111827; outline: none; cursor: pointer; padding: 0 2px; }
.rep-pill-select:disabled { cursor: not-allowed; color: #9ca3af; }

.rep-btn-generar {
  background: #111827;
  color: #fff;
  border: none;
  padding: 8px 20px;
  border-radius: 20px;
  font-size: 0.88rem;
  font-weight: 700;
  cursor: pointer;
  transition: opacity 0.15s;
  margin-left: 4px;
  height: 40px;
  box-sizing: border-box;
}
.rep-btn-generar:hover:not(:disabled) { opacity: 0.85; }

.rep-btn-limpiar {
  background: #f3f4f6;
  color: #111827;
  border: 1px solid #e5e7eb;
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 0.88rem;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.15s;
  height: 40px;
  box-sizing: border-box;
}
.rep-btn-limpiar:hover:not(:disabled) { background: #e5e7eb; }

.badge-read-only { font-size: 0.8rem; font-weight: 700; background-color: #f3f4f6; color: #4b5563; padding: 6px 14px; border-radius: 20px; border: 1px solid #e5e7eb; margin-left: 4px; }
.rep-export-group { display: flex; gap: 8px; margin-left: auto; }

.btn-export { display: flex; align-items: center; gap: 5px; padding: 6px 14px; border-radius: 20px; border: 1.5px solid; font-size: 0.8rem; font-weight: 700; cursor: pointer; height: 40px; box-sizing: border-box; }
.btn-export.pdf   { background: #f0fdf4; color: #16a34a; border-color: #86efac; }
.btn-export.excel { background: #fff7ed; color: #ea580c; border-color: #fdba74; }


.rep-kpi-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
.rep-kpi-card { background: #fff; border: 1px solid #e5e7eb; border-radius: 20px; padding: 28px 20px 20px; display: flex; flex-direction: column; align-items: flex-start; gap: 4px; }
.rep-kpi-icon-wrap { width: 52px; height: 52px; border-radius: 14px; display: flex; align-items: center; justify-content: center; margin-bottom: 12px; }
.rep-icon-green  { background: #f0fdf4; }
.rep-icon-blue   { background: #eff6ff; }
.rep-icon-yellow { background: #fffbeb; }
.rep-icon-purple { background: #f5f3ff; }
.rep-kpi-img { width: 26px; height: 26px; object-fit: contain; }
.rep-kpi-valor { font-size: 2.2rem; font-weight: 800; color: #111827; line-height: 1; }
.rep-kpi-label { font-size: 0.82rem; color: #9ca3af; font-weight: 600; margin-top: 4px; }

.rep-charts-row { display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 16px; }
.rep-chart-card { background: #fff; border: 1px solid #e5e7eb; border-radius: 18px; padding: 20px; }
.rep-chart-title { font-size: 0.95rem; font-weight: 800; color: #111827; }
.rep-chart-subtitle { font-size: 0.75rem; color: #9ca3af; font-weight: 500; margin-top: 2px; margin-bottom: 16px; }

.rep-bar-chart { display: flex; align-items: flex-end; gap: 4px; height: 100px; }
.rep-bar-col { display: flex; flex-direction: column; align-items: center; flex: 1; height: 100%; justify-content: flex-end; gap: 4px; }
.rep-bar-wrapper { width: 100%; height: 80px; display: flex; align-items: flex-end; }
.rep-bar-fill { width: 100%; border-radius: 3px 3px 0 0; transition: height 0.4s ease; }
.dark-bar { background: #111827; }
.rep-bar-label { font-size: 0.55rem; font-weight: 600; color: #9ca3af; text-transform: uppercase; text-align: center; }

.rep-vehiculos-card { display: flex; flex-direction: column; }
.rep-vehiculos-list { display: flex; flex-direction: column; gap: 10px; margin-top: 14px; }
.rep-vehiculo-row { display: flex; align-items: center; gap: 10px; }
.rep-vehiculo-name { font-size: 0.78rem; font-weight: 600; color: #374151; min-width: 140px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.rep-vehiculo-bar-track { flex: 1; height: 7px; background: #f3f4f6; border-radius: 4px; overflow: hidden; }
.rep-vehiculo-bar-fill { height: 100%; background: #111827; border-radius: 4px; transition: width 0.4s ease; }


.rep-metricas-panel { background: #fff; border: 1px solid #e5e7eb; border-radius: 18px; padding: 20px 24px; }
.rep-metricas-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; }
.rep-metricas-title { font-size: 1rem; font-weight: 800; color: #111827; }
.rep-badge-gray { font-size: 0.72rem; font-weight: 700; background: #f3f4f6; color: #6b7280; padding: 4px 12px; border-radius: 10px; border: 1px solid #e5e7eb; }
.rep-metricas-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 14px; }
.rep-metrica-item { background: #fff; border: 1px solid #e5e7eb; border-radius: 12px; padding: 14px 16px; display: flex; flex-direction: column; gap: 6px; }
.rep-metrica-label { font-size: 0.65rem; font-weight: 700; color: #9ca3af; letter-spacing: 0.5px; text-transform: uppercase; }
.rep-metrica-valor { font-size: 1.25rem; font-weight: 800; color: #111827; }


.rep-tabla-panel { background: #fff; border: 1px solid #e5e7eb; border-radius: 18px; overflow: hidden; padding: 20px 24px 0; }
.custom-table-mockup { width: 100%; border-collapse: collapse; font-size: 0.88rem; }
.custom-table-mockup th { color: #374151; font-weight: 700; padding: 10px 14px; text-align: left; border-bottom: 2px solid #f3f4f6; }
.custom-table-mockup td { padding: 14px; color: #111827; border-bottom: 1px solid #f3f4f6; vertical-align: middle; }
.rep-tend-track { width: 160px; height: 7px; background: #f3f4f6; border-radius: 4px; overflow: hidden; }
.rep-tend-fill { height: 100%; border-radius: 4px; transition: width 0.4s ease; }


.rep-lists-row { display: grid; grid-template-columns: repeat(3, 1fr); gap: 16px; }
.rep-list-card { background: #fff; border: 1px solid #e5e7eb; border-radius: 18px; padding: 20px; }
.rep-rank-item { display: flex; flex-direction: column; gap: 5px; margin-bottom: 10px; }
.rep-rank-header { display: flex; justify-content: space-between; align-items: center; }
.rep-rank-num { width: 18px; height: 18px; background: #111827; color: #fff; border-radius: 50%; font-size: 0.6rem; font-weight: 800; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.rep-rank-name { font-size: 0.82rem; font-weight: 600; color: #111827; }
.rep-rank-count { font-size: 0.78rem; font-weight: 700; color: #6b7280; white-space: nowrap; }

.status-pill-mockup { padding: 4px 12px; border-radius: 20px; font-size: 0.75rem; font-weight: 700; display: inline-block; }
.pendiente  { background: #fef3c7; color: #92400e; }
.aprobada   { background: #dcfce7; color: #166534; }
.rechazada  { background: #fee2e2; color: #991b1b; }
.finalizada { background: #dbeafe; color: #1e40af; }

.rep-loading-card { background: #f9fafb; border: 1px dashed #d1d5db; border-radius: 18px; padding: 60px; text-align: center; display: flex; flex-direction: column; align-items: center; gap: 14px; color: #6b7280; font-weight: 600; }
.rep-loading-spinner { width: 34px; height: 34px; border: 3px solid #e5e7eb; border-top-color: #111827; border-radius: 50%; animation: rep-spin 0.7s linear infinite; }
@keyframes rep-spin { to { transform: rotate(360deg); } }

.rep-empty { color: #9ca3af; font-size: 0.83rem; text-align: center; padding: 16px 0; font-weight: 500; }


@media print {
  body, .rep-page { background: #ffffff !important; color: #111827 !important; padding: 0 !important; margin: 0 !important; width: 100% !important; }
  .rep-filters-bar, .rep-export-group, .rep-btn-generar, .rep-btn-limpiar, .badge-read-only { display: none !important; }
  
  .rep-kpi-grid { display: grid !important; grid-template-columns: repeat(4, 1fr) !important; gap: 12px !important; page-break-inside: avoid !important; }
  .rep-charts-row { display: grid !important; grid-template-columns: repeat(3, 1fr) !important; gap: 12px !important; page-break-inside: avoid !important; }
  .rep-lists-row { display: grid !important; grid-template-columns: repeat(3, 1fr) !important; gap: 12px !important; page-break-inside: avoid !important; }
  
  .rep-kpi-card, .rep-chart-card, .rep-metricas-panel, .rep-tabla-panel, .rep-list-card {
    border: 1px solid #e5e7eb !important; 
    box-shadow: none !important; 
    page-break-inside: avoid !important; 
    break-inside: avoid !important;
    padding: 14px !important;
  }
  .rep-kpi-valor { font-size: 1.6rem !important; }
  .rep-bar-fill, .rep-vehiculo-bar-fill, .rep-tend-fill { -webkit-print-color-adjust: exact !important; print-color-adjust: exact !important; }
}
</style>