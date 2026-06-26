<template>
  <div class="rep-page">

    <div class="dashboard-header">
      <div class="dashboard-header-left">
        <h2>Reportes</h2>
        <p>Visualiza métricas, estadísticas y exporta reportes del sistema.</p>
      </div>

      <div class="dashboard-header-badge">
        Reportes inteligentes
      </div>
    </div>

    <div class="rep-filters-bar">
      <div class="rep-filter-pill-group">
        <span class="rep-filter-label">Periodo:</span>
        <select class="rep-pill-select" v-model="filtroPeriodo">
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
        <select class="rep-pill-select" v-model="filtroModulo">
          <option value="todos">Todos</option>
          <option value="viajes">Viajes</option>
          <option value="combustible">Combustible</option>
          <option value="solicitudes">Solicitudes</option>
          <option value="conductores">Conductores</option>
        </select>
      </div>

      <div class="rep-filter-pill-group">
        <span class="rep-filter-label">Area:</span>
        <select class="rep-pill-select" v-model="filtroArea">
          <option value="todas">Todas las areas</option>
          <option v-for="area in areasDisponibles" :key="area" :value="area">{{ area }}</option>
        </select>
      </div>

      <button class="rep-btn-generar" @click="cargarReportes" :disabled="isLoading">
        {{ isLoading ? 'Cargando...' : 'Generar' }}
      </button>

      <button class="rep-btn-limpiar" @click="limpiarFiltros" :disabled="isLoading">
        Limpiar Filtros
      </button>

      <div class="rep-export-group">
        <button
            class="btn-export-mockup-file btn-excel-variant"
            @click="exportarExcel"
        >
            <FileSpreadsheet :size="16"/>
            Excel
        </button>
        <button class="btn-export-mockup-file btn-pdf-variant" @click="exportarPDF" :disabled="isLoading" title="Imprimir / PDF">
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
          <div class="rep-kpi-icon-wrap">
              <Fuel :size="24" />
          </div>
          <div class="rep-kpi-card-data">
            <div class="rep-kpi-label">Viajes realizados</div>
            <div class="rep-kpi-valor">{{ kpis.viajes.totalViajes.toLocaleString() }}</div>
          </div>
        </div>

        <div class="rep-kpi-card">
          <div class="rep-kpi-icon-wrap">
              <Fuel :size="24" />
          </div>
          <div class="rep-kpi-card-data">
            <div class="rep-kpi-label">Consumo de combustible</div>
            <div class="rep-kpi-valor">{{ kpis.combustible.totalGalones.toLocaleString() }} L</div>
          </div>
        </div>

        <div class="rep-kpi-card">
          <div class="rep-kpi-icon-wrap">
              <FileText :size="24" />
          </div>
          <div class="rep-kpi-card-data">
            <div class="rep-kpi-label">Solicitudes procesadas</div>
            <div class="rep-kpi-valor">{{ kpis.solicitudes.totalSolicitudes.toLocaleString() }}</div>
          </div>
        </div>

        <div class="rep-kpi-card">
          <div class="rep-kpi-icon-wrap">
              <Users :size="24" />
          </div>
          <div class="rep-kpi-card-data">
            <div class="rep-kpi-label">Conductores activos</div>
            <div class="rep-kpi-valor">{{ kpis.conductores.totalConductores.toLocaleString() }}</div>
          </div>
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

    <div class="tabla-impresion-pdf">
      <div class="print-header">
        <div class="print-header-left">
          <h1>SISTEMA DE GESTIÓN DE TRANSPORTE</h1>
          <h2>Reporte Consolidado de Flota — {{ filtroAnio }}</h2>
          <p class="print-meta">Área: <strong>{{ filtroArea.toUpperCase() }}</strong> | Filtro: <strong>{{ filtroModulo.toUpperCase() }}</strong></p>
        </div>
        <div class="print-header-right">
          <p>Fecha emisión: {{ new Date().toLocaleDateString('es-DO') }}</p>
          <p>Estatus: Consolidado Oficial</p>
        </div>
      </div>

      <div class="print-kpi-row">
        <div class="print-kpi-box">
          <span class="print-kpi-box-label">Viajes Completados</span>
          <strong class="print-kpi-box-val">{{ kpis.viajes.totalViajes }}</strong>
        </div>
        <div class="print-kpi-box">
          <span class="print-kpi-box-label">Consumo Combustible</span>
          <strong class="print-kpi-box-val">{{ kpis.combustible.totalGalones }} L</strong>
        </div>
        <div class="print-kpi-box">
          <span class="print-kpi-box-label">Kilómetros Totales</span>
          <strong class="print-kpi-box-val">{{ kpis.combustible.totalKilometros.toLocaleString() }} km</strong>
        </div>
        <div class="print-kpi-box">
          <span class="print-kpi-box-label">Inversión Total</span>
          <strong class="print-kpi-box-val">{{ formatearCosto(kpis.combustible.totalGastado) }}</strong>
        </div>
      </div>

      <h3 class="print-section-title">Desglose de Indicadores del Periodo</h3>
      <table class="reporte-print-table">
        <thead>
          <tr>
            <th>Dimensión / Indicador Operativo</th>
            <th class="text-right">Métrica Registrada</th>
          </tr>
        </thead>
        <tbody>
          <tr><td>Solicitudes Totales Procesadas</td><td class="text-right"><strong>{{ kpis.solicitudes.totalSolicitudes }}</strong></td></tr>
          <tr><td>Conductores Activos en Ruta</td><td class="text-right"><strong>{{ kpis.conductores.totalConductores }}</strong></td></tr>
          <tr><td>Rendimiento General de Combustible</td><td class="text-right"><strong>{{ kpis.combustible.rendimientoKmPorGalon.toFixed(2) }} km/gal</strong></td></tr>
          <tr><td>Costo Promedio Estimado por Galón</td><td class="text-right"><strong>{{ formatearCosto(kpis.combustible.costoPromedioPorGalon) }}</strong></td></tr>
          <tr class="print-divider-row"><td colspan="2">Estados del Transporte</td></tr>
          <tr><td>• Viajes Programados</td><td class="text-right">{{ kpis.viajes.programados }} ({{ obtenerPorcentaje(kpis.viajes.programados, kpis.viajes.totalViajes) }})</td></tr>
          <tr><td>• Viajes Activos / En Curso</td><td class="text-right">{{ kpis.viajes.enCurso }} ({{ obtenerPorcentaje(kpis.viajes.enCurso, kpis.viajes.totalViajes) }})</td></tr>
          <tr><td>• Viajes Finalizados</td><td class="text-right">{{ kpis.viajes.finalizados }} ({{ obtenerPorcentaje(kpis.viajes.finalizados, kpis.viajes.totalViajes) }})</td></tr>
          <tr><td>• Viajes Cancelados</td><td class="text-right">{{ kpis.viajes.cancelados }} ({{ obtenerPorcentaje(kpis.viajes.cancelados, kpis.viajes.totalViajes) }})</td></tr>
        </tbody>
      </table>

      <div class="print-footer-signatures">
        <div class="signature-block">
          <div class="signature-line"></div>
          <p>Firma Gestor de Operaciones</p>
        </div>
        <div class="signature-block">
          <div class="signature-line"></div>
          <p>Sello Auditoría Interna</p>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

import {
  Route,
  Fuel,
  FileText,
  Users,
  FileSpreadsheet,
  FileDown
} from 'lucide-vue-next'

const API_BASE = 'https://localhost:7221/api/Reportes'

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
.rep-page,
.rep-page * {
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif !important;
}

.rep-page {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.section-header { margin-bottom: 20px; }
.section-header h2 { margin: 0; font-size: 1.5rem; font-weight: 700; color: #111827; }
.section-header p { margin-top: 6px; color: #6b7280; font-size: 0.9rem; }


.rep-kpi-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }

.rep-kpi-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 16px;
  display: flex;
  align-items: center;
  gap: 14px;
}

.rep-kpi-icon-wrap{
    width:48px;
    height:48px;
    min-width:48px;

    display:flex;
    align-items:center;
    justify-content:center;

    background:#f3f4f6 !important;
    border-radius:14px;

    color:#111827;
}

.rep-kpi-icon-wrap svg{
    width:24px;
    height:24px;
}

.rep-icon-green  { background: #f0fdf4; }
.rep-icon-blue   { background: #eff6ff; }
.rep-icon-yellow { background: #fffbeb; }
.rep-icon-purple { background: #f5f3ff; }

.rep-kpi-img {
  width: 20px;
  height: 20px;
  object-fit: contain;
}

.rep-kpi-card-data {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.rep-kpi-label {
  font-size: 0.82rem !important;
  color: #6b7280 !important;
  font-weight: 500 !important;
  order: 1;
}

.rep-kpi-valor {
  font-size: 1.3rem !important;
  font-weight: 700 !important;
  color: #111827 !important;
  line-height: 1.2 !important;
  order: 2;
}


.rep-charts-row, .rep-lists-row {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 14px;
}

.rep-chart-card, .rep-list-card, .rep-metricas-panel, .rep-tabla-panel {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 16px;
}

.rep-chart-title, .rep-metricas-title {
  font-size: 0.95rem !important;
  font-weight: 700;
  color: #111827;
  line-height: 1.2;
}

.rep-chart-subtitle {
  font-size: 0.78rem !important;
  color: #6b7280 !important;
  font-weight: 500;
  margin-top: 2px;
}


.rep-bar-chart {
  display: flex;
  align-items: flex-end;
  gap: 6px;
  height: 75px;
  margin-top: 14px;
}

.rep-bar-col {
  display: flex;
  flex-direction: column;
  align-items: center;
  flex: 1;
  height: 100%;
  justify-content: flex-end;
  gap: 2px;
}

.rep-bar-wrapper {
  width: 100%;
  height: 55px;
  display: flex;
  align-items: flex-end;
}

.rep-bar-fill {
  width: 100%;
  border-radius: 2px 2px 0 0;
  transition: height 0.4s ease;
}
.dark-bar { background: #111827; }

.rep-bar-label {
  font-size: 0.72rem !important;
  color: #6b7280 !important;
  font-weight: 600 !important;
}


.rep-vehiculos-card { display: flex; flex-direction: column; }
.rep-vehiculos-list, .rep-rank-item {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-top: 10px;
}

.rep-vehiculo-row {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.rep-rank-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.rep-vehiculo-name, .rep-rank-name {
  font-size: 0.8rem !important;
  font-weight: 600;
  color: #374151;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 180px;
}

.rep-rank-num {
  width: 16px;
  height: 16px;
  background: #111827;
  color: #fff;
  border-radius: 50%;
  font-size: 0.6rem;
  font-weight: 800;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.rep-vehiculo-bar-track, .rep-tend-track {
  width: 100%;
  height: 5px;
  background: #f3f4f6;
  border-radius: 3px;
  overflow: hidden;
}

.rep-vehiculo-bar-fill, .rep-tend-fill {
  height: 100%;
  background: #111827;
  border-radius: 3px;
  transition: width 0.4s ease;
}

.rep-metricas-panel { margin-top: 0; }
.rep-metricas-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
.rep-badge-gray { font-size: 0.72rem; font-weight: 700; background: #f3f4f6; color: #6b7280; padding: 4px 12px; border-radius: 10px; border: 1px solid #e5e7eb; }
.rep-metricas-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 10px; }

.rep-metrica-item {
  background: #fff;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  padding: 10px 12px;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.rep-metrica-label {
  color: #9ca3af !important;
  font-size: 0.68rem !important;
  font-weight: 700 !important;
  text-transform: uppercase !important;
  letter-spacing: 0.05em;
}

.rep-metrica-valor { font-size: 1rem !important; font-weight: 700; color: #111827; }


.rep-tabla-panel { overflow: hidden; padding: 14px 16px; }
.custom-table-mockup { width: 100%; border-collapse: collapse; margin-top: 8px; }
.custom-table-mockup th { color: #374151; font-weight: 700; font-size: 0.78rem !important; padding: 6px 8px; text-align: left; border-bottom: 2px solid #f3f4f6; }

.rep-chart-subtitle,
.rep-rank-count,
.rep-empty,
.custom-table-mockup td {
  color: #6b7280 !important;
  font-size: 0.8rem !important;
  font-weight: 500 !important;
  text-transform: none !important;
}

.custom-table-mockup td { padding: 6px 8px !important; }
.rep-tend-track { width: 100px; }


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

.rep-filter-label { font-size: 0.88rem; font-weight: 600; color: #6b7280; white-space: nowrap; }
.rep-pill-select { border: none; background: transparent; font-size: 0.88rem; font-weight: 700; color: #111827; outline: none; cursor: pointer; padding: 0 2px; }

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

.rep-export-group {
  display: flex;
  gap: 10px;
  margin-left: auto;
}

.btn-export-mockup-file {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 0 16px;
  border-radius: 10px;
  border: 1px solid #e5e7eb;
  font-size: 0.88rem;
  font-weight: 700;
  cursor: pointer;
  height: 40px;
  box-sizing: border-box;
  transition: background-color 0.15s ease, border-color 0.15s ease;
}

.btn-excel-variant { background-color: #f0fdf4; color: #16a34a; border-color: #86efac; }
.btn-excel-variant:hover:not(:disabled) { background-color: #dcfce7; }

.btn-pdf-variant { background-color: #fef2f2; color: #dc2626; border-color: #fca5a5; }
.btn-pdf-variant:hover:not(:disabled) { background-color: #fee2e2; }

.btn-export-mockup-file:disabled {
  background-color: #f3f4f6 !important;
  color: #9ca3af !important;
  border-color: #e5e7eb !important;
  cursor: not-allowed;

}

.status-pill-mockup { padding: 2px 8px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; display: inline-block; }
.pendiente  { background: #fef3c7; color: #92400e; }
.aprobada   { background: #dcfce7; color: #166534; }
.rechazada  { background: #fee2e2; color: #991b1b; }
.finalizada { background: #dbeafe; color: #1e40af; }

.rep-loading-card { background: #f9fafb; border: 1px dashed #d1d5db; border-radius: 18px; padding: 60px; text-align: center; display: flex; flex-direction: column; align-items: center; gap: 14px; color: #6b7280; font-weight: 600; }
.rep-loading-spinner { width: 34px; height: 34px; border: 3px solid #e5e7eb; border-top-color: #111827; border-radius: 50%; animation: rep-spin 0.7s linear infinite; }

@keyframes rep-spin { to { transform: rotate(360deg); } }

@media print {
  body *, #app *, .sidebar, .navbar, .app-sidebar, .app-header, header, aside, .nav-container, .user-profile-bar {
    display: none !important;
  }
  html, body, #app, .rep-page, .tabla-impresion-pdf, .tabla-impresion-pdf * {
    display: block !important;
    visibility: visible !important;
  }
  @page { margin: 1.5cm !important; }
  .tabla-impresion-pdf {
    position: absolute !important;
    left: 0 !important;
    top: 0 !important;
    width: 100% !important;
    margin: 0 !important;
    padding: 0 !important;
    background: #ffffff !important;
  }
  .print-header {
    text-align: center !important;
    margin-bottom: 25px !important;
    border-bottom: 2px solid #111827 !important;
    padding-bottom: 12px !important;
  }
  .print-header h1 { font-size: 1.6rem !important; font-weight: 800 !important; margin: 0 !important; color: #111827 !important; text-transform: uppercase !important; }
  .print-header h2 { font-size: 1.1rem !important; font-weight: 700 !important; margin: 6px 0 !important; color: #374151 !important; }
  .print-header p { font-size: 0.85rem !important; color: #6b7280 !important; margin: 0 !important; }
  .reporte-print-table { width: 100% !important; border-collapse: collapse !important; margin-top: 15px !important; }
  .reporte-print-table th { background-color: #f3f4f6 !important; color: #111827 !important; font-weight: 700 !important; border: 1px solid #bcbfc2 !important; padding: 12px 14px !important; text-align: left !important; font-size: 0.9rem !important; print-color-adjust: exact !important; }
  .reporte-print-table td { border: 1px solid #d1d5db !important; padding: 12px 14px !important; color: #111827 !important; font-size: 0.9rem !important; }
  .reporte-print-table tr:nth-child(even) { background-color: #f9fafb !important; print-color-adjust: exact !important; }
}

@media screen {
  .tabla-impresion-pdf { display: none !important; }
}
</style>