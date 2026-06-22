// Guardando el archivo para que vuelva

<template>
  <div class="section-header-mockup">
    <h2>Combustible</h2>
    <p>Administra el combustible utilizado</p>
  </div>

  <div class="mant-cards-row">
    <div class="mant-card">
      <div class="mant-card-icon mant-icon-hoy">
        <img src="../assets/icons/gasolina-negro.png" alt="Consumo total" />
      </div>
      <div class="mant-card-info">
        <span class="mant-card-label">Consumo total (mes)</span>
        <span class="mant-card-valor">{{ kpis.consumoTotal }} gal</span>
      </div>
    </div>

    <div class="mant-card">
      <div class="mant-card-icon mant-icon-pendiente">
        <img src="../assets/icons/moneda.png" alt="Gasto total" />
      </div>
      <div class="mant-card-info">
        <span class="mant-card-label">Gasto total (mes)</span>
        <span class="mant-card-valor">{{ formatearCosto(kpis.gastoTotal) }}</span>
      </div>
    </div>

    <div class="mant-card">
      <div class="mant-card-icon mant-icon-aldia">
        <img src="../assets/icons/vehiculo-negro.png" alt="Rendimiento" />
      </div>
      <div class="mant-card-info">
        <span class="mant-card-label">Rendimiento promedio</span>
        <span class="mant-card-valor">{{ kpis.rendimientoPromedio }} km/gal</span>
      </div>
    </div>

    <div class="mant-card">
      <div class="mant-card-icon mant-icon-vencido">
        <img src="../assets/icons/alerta.png" alt="Elevado" />
      </div>
      <div class="mant-card-info">
        <span class="mant-card-label">Con consumo elevado</span>
        <span class="mant-card-valor">{{ kpis.consumoElevado }}</span>
      </div>
    </div>
  </div>


  <div class="comb-panels-row">


    <section class="panel">
      <div class="panel-header">
        <h3>Consumo por vehículo</h3>
        <button
          type="button"
          @click="mostrarHistorial = true"
          class="comb-ver-todos-btn"
        >
          Ver todos →
        </button>
      </div>

      <div class="comb-donut-wrapper">

        <div style="position: relative; width: 130px; height: 130px; flex-shrink: 0;">
          <div :style="{
            background: donutGradient,
            width: '130px', height: '130px',
            borderRadius: '50%',
            display: 'flex', alignItems: 'center', justifyContent: 'center'
          }">
            <div style="
              width: 80px; height: 80px;
              background: #ffffff;
              border-radius: 50%;
              display: flex;
              flex-direction: column;
              align-items: center;
              justify-content: center;
              gap: 2px;
            ">
              <span style="font-size: 15px; font-weight: 800; color: #111827; line-height: 1.1;">
                {{ kpis.consumoTotal }}
              </span>
              <span style="font-size: 9px; font-weight: 700; color: #6b7280; text-transform: uppercase; letter-spacing: 0.4px;">
                gal
              </span>
            </div>
          </div>
        </div>

        <div class="comb-donut-legends">
          <div v-if="vehiculosConConsumo.length === 0" style="color: #9ca3af; font-size: 0.8rem;">
            Sin datos de consumo
          </div>
          <div
            v-for="(item, idx) in vehiculosConConsumo.slice(0, 5)"
            :key="item.vehiculoId"
            class="comb-legend-row"
          >
            <span :style="{
              width: '10px', height: '10px', borderRadius: '50%', flexShrink: '0',
              backgroundColor: coloresDonut[idx % coloresDonut.length]
            }"></span>
            <span class="comb-legend-text">
              {{ obtenerNombreVehiculo(item.vehiculoId) }}: {{ item.totalGalones }} gal
            </span>
          </div>
        </div>
      </div>
    </section>


    <section class="panel">
      <div class="panel-header">
        <h3>Solicitudes de carga</h3>
        <span class="comb-chip-pendiente">Solo pendientes</span>
      </div>
      <div v-if="solicitudesRecientes.length === 0" style="color: #9ca3af; font-size: 0.83rem; text-align: center; padding: 20px 0;">
        Sin solicitudes pendientes
      </div>
      <div
        v-for="sol in solicitudesRecientes"
        :key="sol.id"
        class="request-item request-item-clickable"
        @click="verDetalleConsumo(sol)"
        title="Ver detalle"
      >
        <div>
          <strong>{{ obtenerNombreVehiculo(sol.vehiculoId) }}</strong>
          <p>{{ obtenerNombreConductor(sol.conductorId) }}</p>
        </div>
        <span class="badge pendiente">
          {{ sol.galones }} gal
        </span>
      </div>
    </section>

  </div>

 
  <div class="mant-filter-container-mockup">

    <div class="mant-filter-field" style="flex: 2;">
      <label class="mant-filter-label">Filtrar por Vehículo</label>
      <div class="mant-input-search-wrapper">
        <input type="text" class="mant-inline-input" placeholder="Buscar..." v-model="filtroBusqueda" />
        <img src="../assets/icons/search.png" class="mant-search-inline-icon" alt="Buscar" />
      </div>
    </div>

    <div class="mant-filter-field">
      <label class="mant-filter-label">Estado</label>
      <select class="mockup-select" v-model="filtroEstado" style="border: 1px solid #cccccc; border-radius: 20px; height: 36px; padding: 0 10px;">
        <option value="">Todos</option>
        <option value="1">Pendiente</option>
        <option value="2">Aprobado</option>
        <option value="3">Rechazado</option>
      </select>
    </div>

    <button class="btn-filter-action" @click="limpiarFiltros">Limpiar Filtros</button>

    <button
      class="mant-btn-agregar-black"
      :class="{ 'mant-btn-disabled': !esAdmin }"
      :disabled="!esAdmin"
      @click="abrirFormularioNuevo"
    >
      Registrar
    </button>
  </div>


  <section class="card-panel-mockup comb-tabla-full" ref="tablaRef">
    <div class="table-responsive">
      <table class="custom-table-mockup">
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Vehículo</th>
            <th>Conductor</th>
            <th>Galones</th>
            <th>Costo (RD$)</th>
            <th>Km recorridos</th>
            <th>Estado</th>
            <th class="text-center" style="width: 120px;">Acciones</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="consumo in consumosFiltrados" :key="consumo.id">
            <td>{{ formatearFechaCorta(consumo.fecha) }}</td>
            <td>{{ obtenerNombreVehiculo(consumo.vehiculoId) }}</td>
            <td>{{ obtenerNombreConductor(consumo.conductorId) }}</td>
            <td>{{ consumo.galones }} gal</td>
            <td>{{ formatearCosto(consumo.costo) }}</td>
            <td>{{ consumo.kilometrosRecorridos }} km</td>
            <td>
              <span :class="['status-pill-mockup', obtenerClaseEstado(consumo.estado)]">
                {{ formatearEstadoVista(consumo.estado) }}
              </span>
            </td>
            <td class="actions-cell-fixed">
              <div class="actions-wrapper" style="display: flex; gap: 6px; justify-content: center;">
                <button
                  class="action-btn-mockup icon-view"
                  title="Ver detalle"
                  @click="verDetalleConsumo(consumo)"
                ></button>
                <button
                  class="action-btn-mockup icon-edit"
                  :class="{ 'btn-disabled': !esAdmin }"
                  :disabled="!esAdmin"
                  :title="esAdmin ? 'Editar' : 'No permitido para tu rol'"
                  @click="esAdmin && abrirFormularioEdicion(consumo)"
                ></button>
                <button
                  class="action-btn-mockup icon-delete"
                  :class="{ 'btn-disabled': !esAdmin }"
                  :disabled="!esAdmin"
                  :title="esAdmin ? 'Eliminar' : 'No permitido para tu rol'"
                  @click="esAdmin && mostrarAvisoEliminacion()"
                ></button>
              </div>
            </td>
          </tr>
          <tr v-if="consumosFiltrados.length === 0">
            <td colspan="8" class="text-center text-muted" style="padding: 30px;">
              No se encontraron registros de combustible.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>


  <div v-if="mostrarFormulario" class="modal-overlay-mockup" @click.self="cerrarFormulario">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>{{ modoEdicion ? 'Editar Registro #' + consumoEditando.id : 'Registrar Consumo de Combustible' }}</h3>
        <button class="btn-close-modal" @click="cerrarFormulario">×</button>
      </div>
      <form @submit.prevent="modoEdicion ? actualizarConsumo() : crearConsumo()" class="form-solicitud-mockup">
        <div class="form-row-mockup">
         <div class="form-group-mockup">
  <label>Vehículo</label>
  <select v-model.number="formModel.vehiculoId" class="mockup-select" required :disabled="modoEdicion">
    <option value="" disabled>Seleccione vehículo</option>
    <option v-for="v in vehiculos" :key="v.id" :value="v.id">{{ v.marca }} {{ v.modelo }} - {{ v.matricula }}</option>
  </select>
</div>
          <div class="form-group-mockup">
  <label>Conductor</label>
  <select v-model.number="formModel.conductorId" class="mockup-select" required>
    <option value="" disabled>Seleccione conductor</option>
    <option v-for="c in conductores" :key="c.id" :value="c.id">
      {{ c.nombre }} {{ c.apellido }}
    </option>
  </select>
</div>
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Galones</label>
            <input type="number" v-model.number="formModel.galones" min="0.1" step="0.01" required />
          </div>
          <div class="form-group-mockup">
            <label>Costo (RD$)</label>
            <input type="number" v-model.number="formModel.costo" min="1" step="0.01" required />
          </div>
        </div>

        <div class="form-row-mockup">
          <div class="form-group-mockup">
            <label>Kilómetros Recorridos</label>
            <input type="number" v-model.number="formModel.kilometrosRecorridos" min="0" step="0.1" required />
          </div>
          <div class="form-group-mockup">
            <label>Fecha</label>
            <input type="datetime-local" v-model="formModel.fecha" required />
          </div>
        </div>

        <div class="form-group-mockup" v-if="modoEdicion">
          <label>Estado de la Carga</label>
          <select v-model.number="formModel.estado" class="mockup-select" required>
            <option :value="1">Pendiente</option>
            <option :value="2">Aprobado</option>
            <option :value="3">Rechazado</option>
          </select>
        </div>

        <div class="form-actions-central">
          <button type="button" class="btn-cancel-mockup" @click="cerrarFormulario">Cancelar</button>
          <button type="submit" class="btn-submit-mockup" :disabled="guardando">
            {{ guardando ? 'Guardando...' : 'Guardar Registro' }}
          </button>
        </div>
      </form>
    </div>
  </div>


  <div v-if="mostrarDetalle" class="modal-overlay-mockup" @click.self="mostrarDetalle = false">
    <div class="modal-container-central">
      <div class="form-panel-header-central">
        <h3>Detalle de Consumo #{{ consumoSeleccionado.id }}</h3>
        <button class="btn-close-modal" @click="mostrarDetalle = false">×</button>
      </div>
      <div class="detalle-solicitud-wrapper">
        <div class="form-row-mockup">
          <div class="detalle-item">
            <strong>Vehículo:</strong>
            <p>{{ obtenerNombreVehiculo(consumoSeleccionado.vehiculoId) }}</p>
          </div>
          <div class="detalle-item">
            <strong>Conductor:</strong>
            <p>{{ obtenerNombreConductor(consumoSeleccionado.conductorId) }}</p>
          </div>
        </div>
        <div class="form-row-mockup">
          <div class="detalle-item">
            <strong>Galones:</strong>
            <p>{{ consumoSeleccionado.galones }} gal</p>
          </div>
          <div class="detalle-item">
            <strong>Costo:</strong>
            <p>{{ formatearCosto(consumoSeleccionado.costo) }}</p>
          </div>
        </div>
        <div class="form-row-mockup">
          <div class="detalle-item">
            <strong>Km recorridos:</strong>
            <p>{{ consumoSeleccionado.kilometrosRecorridos }} km</p>
          </div>
          <div class="detalle-item">
            <strong>Fecha:</strong>
            <p>{{ formatearFechaCorta(consumoSeleccionado.fecha) }}</p>
          </div>
        </div>
        <div class="detalle-item" style="margin-top: 10px; border-top: 1px solid #e5e7eb; padding-top: 12px;">
          <strong>Estado:</strong>
          <p>
            <span :class="['status-pill-mockup', obtenerClaseEstado(consumoSeleccionado.estado)]">
              {{ formatearEstadoVista(consumoSeleccionado.estado) }}
            </span>
          </p>
        </div>
      </div>
      <div class="form-actions-central" style="margin-top: 20px;">
        <button
          v-if="esAdmin"
          type="button"
          class="btn-cancel-mockup"
          @click="mostrarDetalle = false; abrirFormularioEdicion(consumoSeleccionado)"
        >Editar</button>
        <button type="button" class="btn-submit-mockup" @click="mostrarDetalle = false">Entendido</button>
      </div>
    </div>
  </div>


  <div v-if="mostrarHistorial" class="modal-overlay-mockup" @click.self="mostrarHistorial = false">
    <div class="modal-historial-container">
      <div class="form-panel-header-central">
        <h3>Historial de Consumo de Combustible</h3>
        <button class="btn-close-modal" @click="mostrarHistorial = false">×</button>
      </div>

      <div class="historial-resumen-row">
        <div class="historial-kpi">
          <span class="historial-kpi-label">Total registros</span>
          <span class="historial-kpi-valor">{{ consumos.length }}</span>
        </div>
        <div class="historial-kpi">
          <span class="historial-kpi-label">Total galones</span>
          <span class="historial-kpi-valor">{{ consumos.reduce((s,c) => s + (c.galones||0), 0).toFixed(1) }} gal</span>
        </div>
        <div class="historial-kpi">
          <span class="historial-kpi-label">Gasto total</span>
          <span class="historial-kpi-valor">{{ formatearCosto(consumos.reduce((s,c) => s + (c.costo||0), 0)) }}</span>
        </div>
        <div class="historial-kpi">
          <span class="historial-kpi-label">Total km</span>
          <span class="historial-kpi-valor">{{ consumos.reduce((s,c) => s + (c.kilometrosRecorridos||0), 0).toFixed(0) }} km</span>
        </div>
      </div>

      <div class="historial-table-wrapper">
        <table class="custom-table-mockup">
          <thead>
            <tr>
              <th>#</th>
              <th>Fecha</th>
              <th>Vehículo</th>
              <th>Conductor</th>
              <th>Galones</th>
              <th>Costo</th>
              <th>Km recorridos</th>
              <th>Estado</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="consumo in [...consumos].sort((a,b) => new Date(b.fecha) - new Date(a.fecha))" :key="consumo.id">
              <td style="color: #9ca3af; font-size: 0.8rem;">{{ consumo.id }}</td>
              <td>{{ formatearFechaCorta(consumo.fecha) }}</td>
              <td>{{ obtenerNombreVehiculo(consumo.vehiculoId) }}</td>
              <td>{{ obtenerNombreConductor(consumo.conductorId) }}</td>
              <td><strong>{{ consumo.galones }} gal</strong></td>
              <td>{{ formatearCosto(consumo.costo) }}</td>
              <td>{{ consumo.kilometrosRecorridos }} km</td>
              <td>
                <span :class="['status-pill-mockup', obtenerClaseEstado(consumo.estado)]">
                  {{ formatearEstadoVista(consumo.estado) }}
                </span>
              </td>
            </tr>
            <tr v-if="consumos.length === 0">
              <td colspan="8" class="text-center text-muted" style="padding: 30px;">
                No hay registros de consumo.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="form-actions-central" style="margin-top: 16px;">
        <button type="button" class="btn-submit-mockup" @click="mostrarHistorial = false">Cerrar</button>
      </div>
    </div>
  </div>


  <div v-if="mensajeErrorFlotante" class="toast-error-moderno">
    <div class="toast-content">
      <span class="toast-title">Notificación</span>
      <p class="toast-text">{{ mensajeErrorFlotante }}</p>
    </div>
    <button class="btn-close-toast" @click="mensajeErrorFlotante = ''">×</button>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, nextTick } from 'vue'


const rolUsuario = ref(localStorage.getItem('usuario_rol') || 'Operador')
const esAdmin = computed(() => {
  const rol = rolUsuario.value.toLowerCase()
  return rol === 'administrador' || rol === 'admin' || rol === 'superadmin'
})


const consumos             = ref([])
const vehiculos            = ref([])
const conductores          = ref([])
const filtroBusqueda       = ref('')
const filtroEstado         = ref('')
const guardando            = ref(false)
const mensajeErrorFlotante = ref('')
const tablaRef             = ref(null)


const mostrarFormulario = ref(false)
const modoEdicion       = ref(false)
const consumoEditando   = ref(null)

const formModelVacio = () => ({
  vehiculoId:           '',
  conductorId:          '',
  fecha:                new Date().toISOString().substring(0, 16),
  galones:              0,
  costo:                0,
  kilometrosRecorridos: 0,
  estado:               1
})
const formModel = ref(formModelVacio())


const mostrarDetalle      = ref(false)
const consumoSeleccionado = ref({})


const coloresDonut = ['#16a34a', '#ea580c', '#2563eb', '#f59e0b', '#9333ea', '#14b8a6']


const hoy       = new Date()
const inicioMes = new Date(hoy.getFullYear(), hoy.getMonth(), 1)

const kpis = computed(() => {
  const delMes = consumos.value.filter(c => new Date(c.fecha) >= inicioMes)
  const totalGal  = delMes.reduce((sum, c) => sum + (c.galones || 0), 0)
  const totalCost = delMes.reduce((sum, c) => sum + (c.costo || 0), 0)
  const totalKm   = delMes.reduce((sum, c) => sum + (c.kilometrosRecorridos || 0), 0)
  return {
    consumoTotal:        totalGal.toFixed(1),
    gastoTotal:          totalCost,
    rendimientoPromedio: totalGal > 0 ? (totalKm / totalGal).toFixed(1) : '0.0',
    consumoElevado:      consumos.value.filter(c => c.galones > 25).length
  }
})

const consumosFiltrados = computed(() =>
  consumos.value.filter(c => {
    const v   = vehiculos.value.find(veh => veh.id === c.vehiculoId)
    const txt = v ? `${v.marca} ${v.modelo} ${v.matricula}`.toLowerCase() : ''
    return (
      (!filtroBusqueda.value || txt.includes(filtroBusqueda.value.toLowerCase())) &&
      (!filtroEstado.value   || c.estado == filtroEstado.value)
    )
  })
)


const solicitudesRecientes = computed(() =>
  consumos.value
    .filter(c => c.estado === 1)
    .slice()
    .reverse()
    .slice(0, 5)
)


const vehiculosConConsumo = computed(() => {
  const totales = {}
  consumos.value.forEach(c => {
    if (c.galones > 0)
      totales[c.vehiculoId] = (totales[c.vehiculoId] || 0) + c.galones
  })
  return Object.entries(totales)
    .map(([vehiculoId, totalGalones]) => ({ vehiculoId: parseInt(vehiculoId), totalGalones: totalGalones.toFixed(1) }))
    .sort((a, b) => b.totalGalones - a.totalGalones)
})

const donutGradient = computed(() => {
  if (vehiculosConConsumo.value.length === 0) return 'conic-gradient(#e5e7eb 0% 100%)'
  const total = vehiculosConConsumo.value.reduce((sum, v) => sum + parseFloat(v.totalGalones), 0)
  if (total === 0) return 'conic-gradient(#e5e7eb 0% 100%)'
  let acumulado = 0
  const stops = vehiculosConConsumo.value.map((item, idx) => {
    const pct   = (parseFloat(item.totalGalones) / total) * 100
    const color = coloresDonut[idx % coloresDonut.length]
    const inicio = acumulado
    acumulado += pct
    return `${color} ${inicio.toFixed(1)}% ${acumulado.toFixed(1)}%`
  })
  return `conic-gradient(${stops.join(', ')})`
})


const fetchConsumos = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const res = await fetch('https://localhost:7221/api/ConsumosCombustible', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (res.ok) consumos.value = await res.json()
  } catch (error) { console.error('Error cargando consumos:', error) }
}

const fetchVehiculos = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const res = await fetch('https://localhost:7221/api/Vehiculos', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (res.ok) vehiculos.value = await res.json()
  } catch (error) { console.error('Error cargando vehículos:', error) }
}

const fetchConductores = async () => {
  const token = localStorage.getItem('token_transporte')
  try {
    const res = await fetch('https://localhost:7221/api/Conductores', {
      headers: { Authorization: `Bearer ${token}` }
    })
    if (res.ok) conductores.value = await res.json()
  } catch (error) { console.error('Error cargando conductores:', error) }
}


const abrirFormularioNuevo = () => {
  modoEdicion.value     = false
  consumoEditando.value = null
  formModel.value       = formModelVacio()
  mostrarFormulario.value = true
}

const abrirFormularioEdicion = async (consumo) => {
  if (!conductores.value || conductores.value.length === 0) {
    await fetchConductores()
  }

  consumoEditando.value = consumo
  modoEdicion.value     = true
  mostrarFormulario.value = true

  await nextTick()
  await new Promise(resolve => setTimeout(resolve, 60))


  let fechaFormateada = ''
  if (consumo.fecha) {
    try {
      fechaFormateada = new Date(consumo.fecha).toISOString().substring(0, 16)
    } catch {
      fechaFormateada = new Date().toISOString().substring(0, 16)
    }
  } else {
    fechaFormateada = new Date().toISOString().substring(0, 16)
  }


  formModel.value = {
    vehiculoId:           Number(consumo.vehiculoId),
    conductorId:          Number(consumo.conductorId), 
    fecha:                fechaFormateada,
    galones:              Number(consumo.galones),
    costo:                Number(consumo.costo),
    kilometrosRecorridos: Number(consumo.kilometrosRecorridos),
    estado:               Number(consumo.estado)
  }
}

const cerrarFormulario = () => {
  mostrarFormulario.value = false
  modoEdicion.value       = false
  consumoEditando.value   = null
  formModel.value         = formModelVacio()
}

const verDetalleConsumo = (consumo) => {
  consumoSeleccionado.value = { ...consumo }
  mostrarDetalle.value = true
}


const verTodosLosConsumos = async () => {
  filtroBusqueda.value = ''
  filtroEstado.value   = ''
  await fetchConsumos()
  tablaRef.value?.scrollIntoView({ behavior: 'smooth', block: 'start' })
}

const limpiarFiltros = () => {
  filtroBusqueda.value = ''
  filtroEstado.value   = ''
}


const mostrarError = (m) => {
  mensajeErrorFlotante.value = m
  setTimeout(() => { mensajeErrorFlotante.value = '' }, 4000)
}

const crearConsumo = async () => {
  guardando.value = true
  const token = localStorage.getItem('token_transporte')
  try {
    const res = await fetch('https://localhost:7221/api/ConsumosCombustible', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify({ ...formModel.value, estado: 1 })
    })
    if (res.ok) {
      await fetchConsumos()
      cerrarFormulario()
    } else {
      const err = await res.json()
      mostrarError('Error: ' + JSON.stringify(err.errors || err))
    }
  } catch { mostrarError('No se pudo guardar el registro.') }
  finally { guardando.value = false }
}

const actualizarConsumo = async () => {
  if (!consumoEditando.value) return
  guardando.value = true
  const token = localStorage.getItem('token_transporte')
  try {
    const res = await fetch(`https://localhost:7221/api/ConsumosCombustible/${consumoEditando.value.id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', Authorization: `Bearer ${token}` },
      body: JSON.stringify({ ...formModel.value, id: consumoEditando.value.id })
    })
    if (res.ok) {
      await fetchConsumos()
      cerrarFormulario()
    } else {
      const err = await res.json()
      mostrarError('Error: ' + JSON.stringify(err.errors || err))
    }
  } catch { mostrarError('No se pudo actualizar el registro.') }
  finally { guardando.value = false }
}

const mostrarAvisoEliminacion = () => {
  mensajeErrorFlotante.value = 'No se pudo eliminar el registro de combustible.'
  setTimeout(() => { mensajeErrorFlotante.value = '' }, 4000)
}


const obtenerNombreVehiculo  = (id) => {
  const v = vehiculos.value.find(veh => veh.id === id)
  return v ? `${v.marca} ${v.modelo} - ${v.matricula}` : `Vehículo #${id}`
}

const obtenerNombreConductor = (id) => {
  const c = conductores.value.find(cond => cond.id === id)
  return c ? `${c.nombre} ${c.apellido}` : `Conductor #${id}`
}

const formatearFechaCorta  = (f) => f ? new Date(f).toLocaleDateString('es-DO', { day: '2-digit', month: '2-digit', year: 'numeric' }) : '---'
const formatearCosto       = (c) => new Intl.NumberFormat('es-DO', { style: 'currency', currency: 'DOP' }).format(c || 0)
const formatearEstadoVista = (e) => ({ 1: 'Pendiente', 2: 'Aprobado', 3: 'Rechazado' }[e] || 'Pendiente')
const obtenerClaseEstado   = (e) => ({ 1: 'pendiente', 2: 'completado', 3: 'vencido' }[e] || 'pendiente')


const mostrarHistorial = ref(false)

onMounted(() => {
  fetchVehiculos()
  fetchConductores()
  fetchConsumos()
})
</script>

<style>
.mant-cards-row {
  display: flex;
  gap: 12px;
  margin-bottom: 24px;
  flex-wrap: wrap;
  width: 100%;
}

.mant-cards-row .mant-card {
  flex: 1;
  min-width: 170px;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 10px 12px;
  display: flex;
  align-items: center;
  gap: 10px;
  box-sizing: border-box;
}

.mant-cards-row .mant-card-info {
  display: flex;
  flex-direction: column;
  gap: 1px;
  min-width: 0;
  flex: 1;
}

.mant-cards-row .mant-card-label {
  font-size: 0.62rem !important;
  color: #6b7280 !important;
  font-weight: 700 !important;
  text-transform: uppercase;
  letter-spacing: 0.3px;
  line-height: 1.2;
}

.mant-cards-row .mant-card-valor {
  font-size: 1.1rem !important;
  font-weight: 800 !important;
  color: #111827 !important;
  line-height: 1.1;
  white-space: nowrap;
}


.comb-panels-row {
  display: grid;

  grid-template-columns: 3fr 2fr;
  gap: 20px;
  margin-bottom: 20px;
}

.comb-tabla-full {
  width: 100%;
  overflow: hidden;
}

.comb-donut-wrapper {
  display: flex;
  align-items: center;
  gap: 20px;
  padding: 10px 0;
}

.comb-donut-legends {
  display: flex;
  flex-direction: column;
  gap: 8px;
  min-width: 0;
  flex: 1;
}

.comb-legend-row {
  display: flex;
  align-items: center;
  gap: 8px;
}

.comb-legend-text {
  font-size: 0.82rem;
  font-weight: 600;
  color: #374151;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.comb-ver-todos-btn {
  background: none;
  border: none;
  color: #4b5563;
  font-size: 0.8rem;
  font-weight: 600;
  cursor: pointer;
  padding: 0;
}

.comb-ver-todos-btn:hover { color: #111827; }


.comb-chip-pendiente {
  font-size: 0.7rem;
  font-weight: 700;
  background: #fef3c7;
  color: #92400e;
  padding: 2px 10px;
  border-radius: 10px;
}


.request-item-clickable {
  cursor: pointer;
  border-radius: 8px;
  transition: background 0.15s ease, padding 0.15s ease;
  padding-left: 0;
  padding-right: 0;
}

.request-item-clickable:hover {
  background: #f9fafb;
  padding-left: 6px;
  padding-right: 6px;
}

@media (max-width: 1100px) {
  .comb-panels-row { grid-template-columns: 1fr; }
}

.card-panel-mockup {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.01);
}

.panel {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 20px;
  box-sizing: border-box;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.panel-header h3 {
  margin: 0;
  color: #111827;
  font-size: 1rem;
  font-weight: 800;
}

.table-responsive { width: 100%; overflow-x: auto; }

.custom-table-mockup { width: 100%; border-collapse: collapse; font-size: 0.88rem; }

.custom-table-mockup th {
  background: #f9fafb;
  color: #374151;
  font-weight: 700;
  padding: 14px 16px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.custom-table-mockup td {
  padding: 14px 16px;
  color: #111827;
  border-bottom: 1px solid #f3f4f6;
  vertical-align: middle;
}

.custom-table-mockup tbody tr:last-child td { border-bottom: none; }

.request-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #f3f4f6;
  gap: 14px;
}

.request-item:last-child { border-bottom: none; }
.request-item strong { color: #111827; font-size: 0.88rem; display: block; }
.request-item p      { color: #6b7280; font-size: 0.78rem; margin: 2px 0 0 0; }

.status-pill-mockup,
.badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 700;
  white-space: nowrap;
  display: inline-block;
  text-align: center;
}

.pendiente  { background: #fef3c7; color: #92400e; }
.completado { background: #dcfce7; color: #166534; }
.vencido    { background: #fee2e2; color: #991b1b; }

.actions-cell-fixed { text-align: center; width: 120px; }

.action-btn-mockup {
  width: 32px;
  height: 32px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #ffffff center no-repeat;
  cursor: pointer;
  transition: all 0.2s ease;
}

.action-btn-mockup:hover:not(:disabled) {
  background-color: #f9fafb;
  border-color: #d1d5db;
}

.icon-view   { background-image: url('../assets/icons/ver.png');          background-size: 14px; }
.icon-edit   { background-image: url('../assets/icons/editar-negro.png'); background-size: 14px; }
.icon-delete { background-image: url('../assets/icons/eliminar.png');     background-size: 14px; }

.text-center { text-align: center; }
.text-muted  { color: #9ca3af; }

.btn-disabled,
.action-btn-mockup:disabled {
  opacity: 0.25 !important;
  cursor: not-allowed !important;
}

@media (max-width: 1100px) {
  .dashboard-middle { grid-template-columns: 1fr; }
}


.modal-historial-container {
  background: white;
  width: 900px;
  max-width: 96vw;
  max-height: 85vh;
  border-radius: 18px;
  padding: 24px;
  box-shadow: 0 15px 35px rgba(0,0,0,.12);
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.historial-resumen-row {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
}

.historial-kpi {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  padding: 12px 16px;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.historial-kpi-label {
  font-size: 0.72rem;
  font-weight: 700;
  color: #6b7280;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}

.historial-kpi-valor {
  font-size: 1.1rem;
  font-weight: 800;
  color: #111827;
}

.historial-table-wrapper {
  overflow-y: auto;
  max-height: 420px;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
}

.historial-table-wrapper .custom-table-mockup th {
  position: sticky;
  top: 0;
  z-index: 1;
  background: #f9fafb;
}


.mant-filter-field:first-of-type {
  flex: 1; 
  min-width: 250px; 
}


.mant-input-search-wrapper,
.mant-inline-input {
  width: 100%;
  box-sizing: border-box;
}


.mant-filter-container-mockup {
  display: flex !important;
  flex-direction: row !important;
  align-items: center !important;
  width: 100%;
  gap: 16px;
}


.mant-filter-field:first-of-type {
  display: flex !important;
  align-items: center !important;
  gap: 12px;
  flex: 1 !important; 
}


.mant-input-search-wrapper {
  position: relative;
  flex: 1 !important;
  display: flex;
  align-items: center;
}


.mant-inline-input {
  width: 100% !important;
  height: 36px;
  padding: 0 36px 0 16px !important;
  border: 1px solid #cccccc;
  border-radius: 20px;
  box-sizing: border-box;
}


.mant-search-inline-icon {
  position: absolute;
  right: 14px;
  width: 14px;
  height: 14px;
  pointer-events: none;
  opacity: 0.4;
}


.mant-filter-field:nth-of-type(2) {
  display: flex !important;
  align-items: center !important;
  gap: 8px;
  flex-shrink: 0;
}
</style>