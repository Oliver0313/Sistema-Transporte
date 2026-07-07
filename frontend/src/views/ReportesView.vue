<template>
  <div class="rep-page">

    <div class="dashboard-header">
      <div class="dashboard-header-left">
        <h2>Reportes</h2>
        <p>Visualiza métricas, estadísticas y exporta reportes del sistema.</p>
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
  <div class="print-logo-bar">
    <h1>SISTEMA DE GESTIÓN DE TRANSPORTE</h1>
    <span class="print-badge">Reporte oficial</span>
  </div>

  <div class="print-meta-row">
    <div class="print-meta-block">
      <span class="print-meta-label">Periodo</span>
      <span class="print-meta-val">{{ mesesNombres[parseInt(filtroPeriodo) - 1] }} {{ filtroAnio }}</span>
    </div>
    <div class="print-meta-block">
      <span class="print-meta-label">Área</span>
      <span class="print-meta-val">{{ filtroArea }}</span>
    </div>
    <div class="print-meta-block">
      <span class="print-meta-label">Módulo</span>
      <span class="print-meta-val">{{ filtroModulo }}</span>
    </div>
    <div class="print-meta-block">
      <span class="print-meta-label">Fecha emisión</span>
      <span class="print-meta-val">{{ new Date().toLocaleDateString('es-DO') }}</span>
    </div>
  </div>

  <div class="print-kpi-grid">
    <div class="print-kpi-box">
      <span class="print-kpi-label">Viajes realizados</span>
      <strong>{{ kpis.viajes.totalViajes }}</strong>
    </div>
    <div class="print-kpi-box">
      <span class="print-kpi-label">Combustible (L)</span>
      <strong>{{ kpis.combustible.totalGalones }}</strong>
    </div>
    <div class="print-kpi-box">
      <span class="print-kpi-label">Solicitudes</span>
      <strong>{{ kpis.solicitudes.totalSolicitudes }}</strong>
    </div>
    <div class="print-kpi-box">
      <span class="print-kpi-label">Conductores activos</span>
      <strong>{{ kpis.conductores.totalConductores }}</strong>
    </div>
  </div>

  <h3 class="print-section-title">Distribución de viajes por estado</h3>
  <table class="reporte-print-table">
    <thead>
      <tr><th>Estado</th><th>Cantidad</th><th>Porcentaje</th></tr>
    </thead>
    <tbody>
      <tr><td>Programados</td><td>{{ kpis.viajes.programados }}</td><td>{{ obtenerPorcentaje(kpis.viajes.programados, kpis.viajes.totalViajes) }}</td></tr>
      <tr><td>En Curso</td><td>{{ kpis.viajes.enCurso }}</td><td>{{ obtenerPorcentaje(kpis.viajes.enCurso, kpis.viajes.totalViajes) }}</td></tr>
      <tr><td>Finalizados</td><td>{{ kpis.viajes.finalizados }}</td><td>{{ obtenerPorcentaje(kpis.viajes.finalizados, kpis.viajes.totalViajes) }}</td></tr>
      <tr><td>Cancelados</td><td>{{ kpis.viajes.cancelados }}</td><td>{{ obtenerPorcentaje(kpis.viajes.cancelados, kpis.viajes.totalViajes) }}</td></tr>
    </tbody>
  </table>

  <h3 class="print-section-title">Métricas de combustible</h3>
  <table class="reporte-print-table">
    <thead>
      <tr><th>Indicador</th><th>Valor</th></tr>
    </thead>
    <tbody>
      <tr><td>Total gastado</td><td>{{ formatearCosto(kpis.combustible.totalGastado) }}</td></tr>
      <tr><td>Kilómetros totales</td><td>{{ kpis.combustible.totalKilometros.toLocaleString() }} km</td></tr>
      <tr><td>Costo promedio / galón</td><td>{{ formatearCosto(kpis.combustible.costoPromedioPorGalon) }}</td></tr>
      <tr><td>Rendimiento promedio</td><td>{{ kpis.combustible.rendimientoKmPorGalon.toFixed(2) }} km/gal</td></tr>
    </tbody>
  </table>

  <h3 class="print-section-title">Estado de la flota</h3>
  <table class="reporte-print-table">
    <thead>
      <tr><th>Condición</th><th>Unidades</th></tr>
    </thead>
    <tbody>
      <tr><td>Disponibles</td><td>{{ kpis.vehiculos.disponibles }}</td></tr>
      <tr><td>En Viaje</td><td>{{ kpis.vehiculos.enViaje }}</td></tr>
      <tr><td>En Mantenimiento</td><td>{{ kpis.vehiculos.enMantenimiento }}</td></tr>
      <tr><td>Fuera de Servicio</td><td>{{ kpis.vehiculos.fueraDeServicio }}</td></tr>
    </tbody>
  </table>

  <div class="print-footer-signatures">
    <div class="signature-block">
      <div class="signature-line"></div>
      <p>Gestor de Operaciones</p>
    </div>
    <div class="signature-block">
      <div class="signature-line"></div>
      <p>Auditoría Interna</p>
    </div>
  </div>
</div>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
 import ExcelJS from 'exceljs'

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

const exportarExcel = async () => {
  const nombreMes = mesesNombres[parseInt(filtroPeriodo.value) - 1]
 

  const C_DARK   = 'FF111827'
  const C_GRAY   = 'FF6B7280'
  const C_LIGHT  = 'FFF3F4F6'
  const C_WHITE  = 'FFFFFFFF'
  const C_GREEN  = 'FF16A34A'
  const C_GREEN_BG = 'FFDCFCE7'
  const C_BLUE   = 'FF2563EB'
  const C_BLUE_BG = 'FFDBEAFE'
  const C_AMBER  = 'FFF59E0B'
  const C_AMBER_BG = 'FFFEF3C7'
  const C_RED    = 'FFEF4444'
  const C_RED_BG = 'FFFEE2E2'
  const C_BORDER = 'FFE5E7EB'
 
  const workbook = new ExcelJS.Workbook()
  workbook.creator = 'Sistema de Gestión de Transporte'
  workbook.created = new Date()
 
  const sheet = workbook.addWorksheet('Reporte', {
    views: [{ showGridLines: false }],
    pageSetup: { orientation: 'portrait', fitToPage: true }
  })
 
  sheet.columns = [
    { width: 30 },
    { width: 18 },
    { width: 16 },
    { width: 16 }
  ]
 
  let row = 1
 

  sheet.mergeCells(`A${row}:D${row}`)
  const titulo = sheet.getCell(`A${row}`)
  titulo.value = 'SISTEMA DE GESTIÓN DE TRANSPORTE'
  titulo.font = { bold: true, size: 15, color: { argb: C_WHITE } }
  titulo.alignment = { vertical: 'middle', horizontal: 'left', indent: 1 }
  titulo.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: C_DARK } }
  sheet.getRow(row).height = 30
  row++
 
  sheet.mergeCells(`A${row}:D${row}`)
  const subtitulo = sheet.getCell(`A${row}`)
  subtitulo.value =
    `Reporte de ${nombreMes} ${filtroAnio.value}   ·   Área: ${filtroArea.value}   ·   Módulo: ${filtroModulo.value}   ·   Emitido: ${new Date().toLocaleDateString('es-DO')}`
  subtitulo.font = { italic: true, size: 10, color: { argb: C_GRAY } }
  subtitulo.alignment = { vertical: 'middle', indent: 1 }
  subtitulo.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: C_LIGHT } }
  sheet.getRow(row).height = 22
  row += 2
 

  const seccion = (texto) => {
    sheet.mergeCells(row, 1, row, 4)
    const c = sheet.getCell(row, 1)
    c.value = texto.toUpperCase()
    c.font = { bold: true, size: 11, color: { argb: C_WHITE } }
    c.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: C_DARK } }
    c.alignment = { vertical: 'middle', indent: 1 }
    sheet.getRow(row).height = 20
    row++
  }
 
  const encabezados = (labels) => {
    labels.forEach((texto, i) => {
      const c = sheet.getCell(row, i + 1)
      c.value = texto
      c.font = { bold: true, size: 9, color: { argb: C_GRAY } }
      c.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: C_LIGHT } }
      c.border = { bottom: { style: 'medium', color: { argb: C_BORDER } } }
      c.alignment = { vertical: 'middle', indent: 1 }
    })
    sheet.getRow(row).height = 18
    row++
  }
 
  const dato = (valores, { bold = false, badge = null } = {}) => {
    valores.forEach((v, i) => {
      const c = sheet.getCell(row, i + 1)
      c.value = v
      c.font = { size: 10, bold, color: { argb: 'FF374151' } }
      c.border = { bottom: { style: 'thin', color: { argb: C_LIGHT } } }
      c.alignment = { vertical: 'middle', indent: 1 }
    })

    if (badge) {
      sheet.getCell(row, 1).border = {
        ...sheet.getCell(row, 1).border,
        left: { style: 'thick', color: { argb: badge } }
      }
    }
    row++
  }
 
  const espacio = () => { row++ }
 

  seccion('Indicadores clave')
  encabezados(['Indicador', 'Valor'])
  dato(['Viajes realizados', kpis.value.viajes.totalViajes], { bold: true })
  dato(['Combustible consumido (L)', kpis.value.combustible.totalGalones], { bold: true })
  dato(['Solicitudes procesadas', kpis.value.solicitudes.totalSolicitudes], { bold: true })
  dato(['Conductores activos', kpis.value.conductores.totalConductores], { bold: true })
  espacio()
 

  if (filtroModulo.value === 'todos' || filtroModulo.value === 'viajes') {
    seccion('Viajes por estado')
    encabezados(['Estado', 'Cantidad', 'Porcentaje'])
    const estados = [
      ['Programados', kpis.value.viajes.programados, C_AMBER],
      ['En Curso',    kpis.value.viajes.enCurso,      C_GREEN],
      ['Finalizados', kpis.value.viajes.finalizados,  C_BLUE],
      ['Cancelados',  kpis.value.viajes.cancelados,   C_RED],
    ]
    estados.forEach(([nombre, valor, color]) => {
      dato([nombre, valor, obtenerPorcentaje(valor, kpis.value.viajes.totalViajes)], { badge: color })
    })
    espacio()
  }
 

  if (filtroModulo.value === 'todos' || filtroModulo.value === 'combustible') {
    seccion('Métricas de combustible')
    encabezados(['Indicador', 'Valor'])
    dato(['Total gastado', formatearCosto(kpis.value.combustible.totalGastado)])
    dato(['Kilómetros totales', `${kpis.value.combustible.totalKilometros.toLocaleString()} km`])
    dato(['Costo promedio / galón', formatearCosto(kpis.value.combustible.costoPromedioPorGalon)])
    dato(['Rendimiento promedio', `${kpis.value.combustible.rendimientoKmPorGalon.toFixed(2)} km/gal`])
    espacio()
  }
 

  if (solicitudesPorArea.value.length && (filtroModulo.value === 'todos' || filtroModulo.value === 'solicitudes')) {
    seccion('Solicitudes por área')
    encabezados(['Área', 'Cantidad'])
    solicitudesPorArea.value.forEach(s => dato([s.area, s.cantidad]))
    espacio()
  }
 

  if (vehiculosMasUsados.value.length && (filtroModulo.value === 'todos' || filtroModulo.value === 'viajes')) {
    seccion('Vehículos más utilizados')
    encabezados(['Vehículo', 'Viajes'])
    vehiculosMasUsados.value.forEach(v => dato([v.vehiculo, v.viajes]))
    espacio()
  }
 

  if (conductoresMasViajes.value.length && (filtroModulo.value === 'todos' || filtroModulo.value === 'conductores')) {
    seccion('Conductores destacados')
    encabezados(['#', 'Conductor', 'Viajes'])
    conductoresMasViajes.value.forEach((c, i) => dato([i + 1, c.conductor, c.viajes]))
    espacio()
  }
 

  seccion('Estado de la flota')
  encabezados(['Condición', 'Unidades', 'Porcentaje'])
  const flota = [
    ['Disponibles',        kpis.value.vehiculos.disponibles,       C_GREEN],
    ['En Viaje',           kpis.value.vehiculos.enViaje,           C_BLUE],
    ['En Mantenimiento',   kpis.value.vehiculos.enMantenimiento,   C_AMBER],
    ['Fuera de Servicio',  kpis.value.vehiculos.fueraDeServicio,   C_RED],
  ]
  flota.forEach(([nombre, valor, color]) => {
    dato([nombre, valor, obtenerPorcentaje(valor, kpis.value.vehiculos.totalVehiculos)], { badge: color })
  })
 

  sheet.eachRow((r) => { r.alignment = { ...r.alignment, wrapText: false } })
 

  const buffer = await workbook.xlsx.writeBuffer()
  const blob = new Blob([buffer], {
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
  })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = `Reporte_${filtroModulo.value}_${nombreMes}_${filtroAnio.value}.xlsx`
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
  * { visibility: hidden !important; }
  
  .tabla-impresion-pdf,
  .tabla-impresion-pdf * { 
    visibility: visible !important; 
  }

  .tabla-impresion-pdf {
    position: fixed !important;
    top: 0 !important;
    left: 0 !important;
    width: 100% !important;
    background: white !important;
    padding: 1.5cm !important;
    box-sizing: border-box !important;
  }

  @page { margin: 0; size: A4; }

  .print-logo-bar {
    display: flex !important;
    justify-content: space-between !important;
    align-items: center !important;
    border-bottom: 3px solid #111827 !important;
    padding-bottom: 14px !important;
    margin-bottom: 20px !important;
  }

  .print-logo-bar h1 {
    font-size: 13pt !important;
    font-weight: 800 !important;
    color: #111827 !important;
    text-transform: uppercase !important;
    margin: 0 !important;
  }

  .print-badge {
    background: #111827 !important;
    color: #fff !important;
    font-size: 8pt !important;
    font-weight: 700 !important;
    padding: 4px 12px !important;
    border-radius: 20px !important;
    -webkit-print-color-adjust: exact !important;
    print-color-adjust: exact !important;
  }

  .print-meta-row {
    display: grid !important;
    grid-template-columns: repeat(4, 1fr) !important;
    gap: 8px !important;
    margin-bottom: 20px !important;
  }

  .print-meta-block {
    background: #f9fafb !important;
    border: 1px solid #e5e7eb !important;
    border-radius: 6px !important;
    padding: 8px 12px !important;
    -webkit-print-color-adjust: exact !important;
    print-color-adjust: exact !important;
  }

  .print-meta-label {
    display: block !important;
    font-size: 7pt !important;
    color: #6b7280 !important;
    text-transform: uppercase !important;
    font-weight: 700 !important;
    margin-bottom: 2px !important;
  }

  .print-meta-val {
    display: block !important;
    font-size: 10pt !important;
    font-weight: 700 !important;
    color: #111827 !important;
    text-transform: capitalize !important;
  }

  .print-kpi-grid {
    display: grid !important;
    grid-template-columns: repeat(4, 1fr) !important;
    gap: 8px !important;
    margin-bottom: 22px !important;
  }

  .print-kpi-box {
    border: 2px solid #111827 !important;
    border-radius: 8px !important;
    padding: 10px !important;
    text-align: center !important;
    -webkit-print-color-adjust: exact !important;
    print-color-adjust: exact !important;
  }

  .print-kpi-label {
    display: block !important;
    font-size: 7pt !important;
    color: #6b7280 !important;
    text-transform: uppercase !important;
    font-weight: 600 !important;
    margin-bottom: 4px !important;
  }

  .print-kpi-box strong {
    font-size: 16pt !important;
    font-weight: 800 !important;
    color: #111827 !important;
  }

  .print-section-title {
    font-size: 8pt !important;
    font-weight: 700 !important;
    text-transform: uppercase !important;
    color: #6b7280 !important;
    letter-spacing: 0.08em !important;
    margin: 18px 0 6px 0 !important;
    border-left: 3px solid #111827 !important;
    padding-left: 8px !important;
    -webkit-print-color-adjust: exact !important;
    print-color-adjust: exact !important;
  }

  .reporte-print-table {
    width: 100% !important;
    border-collapse: collapse !important;
    margin-bottom: 6px !important;
    page-break-inside: avoid !important;
  }

  .reporte-print-table th {
    background: #111827 !important;
    color: #ffffff !important;
    font-size: 8pt !important;
    font-weight: 700 !important;
    padding: 7px 10px !important;
    text-align: left !important;
    -webkit-print-color-adjust: exact !important;
    print-color-adjust: exact !important;
  }

  .reporte-print-table td {
    font-size: 9pt !important;
    padding: 6px 10px !important;
    border-bottom: 1px solid #e5e7eb !important;
    color: #374151 !important;
  }

  .reporte-print-table tr:nth-child(even) td {
    background: #f9fafb !important;
    -webkit-print-color-adjust: exact !important;
    print-color-adjust: exact !important;
  }

  .print-footer-signatures {
    display: flex !important;
    justify-content: space-around !important;
    margin-top: 40px !important;
    padding-top: 20px !important;
    border-top: 1px solid #e5e7eb !important;
  }

  .signature-block {
    text-align: center !important;
    width: 180px !important;
  }

  .signature-line {
    border-top: 1px solid #111827 !important;
    margin-bottom: 6px !important;
  }

  .signature-block p {
    font-size: 8pt !important;
    color: #6b7280 !important;
    margin: 0 !important;
  }
}

@media screen {
  .tabla-impresion-pdf { display: none !important; }
}
</style>