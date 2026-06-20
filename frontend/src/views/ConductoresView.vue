<template>
  <div>
        <div class="section-header">
          <h2>Conductores</h2>
          <p>Gestión y consulta de conductores registrados</p>
        </div>

        <div class="card-panel">
          <div class="table-responsive">
          <div class="toolbar">
            <input
              v-model="filtroBusqueda"
              type="text"
              placeholder="Buscar conductor..."
              class="search-box"
            />

            <button
              class="btn-primary"
              @click="mostrarModal = true">
              Nuevo conductor
            </button>
          </div>
      <table class="conductores-table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Cédula</th>
            <th>Email</th>
            <th>Licencia</th>
            <th>Tipo licencia</th>
            <th>Vencimiento</th>
            <th>Teléfono</th>
            <th>Dirección</th>
            <th>Estado</th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="conductor in conductoresFiltrados"
            :key="conductor.id"
          >
            <td>
              {{ conductor.nombre }} {{ conductor.apellido }}
            </td>

            <td>
              {{ conductor.cedula }}
            </td>

            <td>
              {{ conductor.email }}
            </td>

            <td>
              {{ conductor.licencia }}
            </td>

            <td>
              {{ formatearTipoLicencia(conductor.tipoLicencia) }}
            </td>

            <td>
              {{ formatearFecha(conductor.fechaVencimientoLicencia) }}
            </td>

            <td>
              {{ conductor.telefono }}
            </td>

            <td>
              {{ conductor.direccion }}
            </td>

            <td>
              <span
                class="estado-badge"
                :class="conductor.estado === 1 ? 'activo' : 'inactivo'"
              >
                {{ conductor.estado === 1 ? 'Disponible' : 'No disponible' }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    </div>
  </div>
  <div
  v-if="mostrarModal"
  class="modal-overlay"
>
  <div class="modal-container">

    <div class="modal-header">
      <h3>Nuevo conductor</h3>

      <button
        class="btn-close"
        @click="mostrarModal = false"
      >
        ✕
      </button>
    </div>

    <div class="modal-body">
      <p>Aquí irá el formulario de conductores.</p>
    </div>

  </div>
</div>

<div v-if="mostrarModal" class="modal-overlay">
  <div class="modal-container">
    <div class="modal-header">
      <h3>Nuevo conductor</h3>

      <button class="btn-close" @click="mostrarModal = false">
        ×
      </button>
    </div>

<div class="modal-body">
  <form class="form-grid">

    <input
      v-model="formConductor.nombre"
      placeholder="Nombre"
    />

    <input
      v-model="formConductor.apellido"
      placeholder="Apellido"
    />

    <input
      v-model="formConductor.cedula"
      placeholder="Cédula"
    />

    <input
      v-model="formConductor.email"
      placeholder="Correo electrónico"
    />

    <input
      v-model="formConductor.licencia"
      placeholder="Licencia"
    />

    <select v-model.number="formConductor.tipoLicencia">
      <option :value="1">Categoría 1</option>
      <option :value="2">Categoría 2</option>
      <option :value="3">Categoría 3</option>
      <option :value="4">Categoría 4</option>
    </select>

    <input
      type="date"
      v-model="formConductor.fechaVencimientoLicencia"
    />

    <input
      v-model="formConductor.telefono"
      placeholder="Teléfono"
    />

    <input
      v-model="formConductor.direccion"
      placeholder="Dirección"
      class="full-width"
    />

        <div class="modal-actions">
          <button
            type="button"
            class="btn-cancel"
            @click="mostrarModal = false"
          >
            Cancelar
          </button>

          <button
            type="button"
            class="btn-primary"
            @click="guardarConductor"
          >
            Guardar conductor
          </button>
        </div>

      </form>
    </div>
  </div>
</div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const conductores = ref([])

const filtroBusqueda = ref('')

const conductoresFiltrados = computed(() => {
  return conductores.value.filter(c =>
    c.nombre?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
    c.apellido?.toLowerCase().includes(filtroBusqueda.value.toLowerCase()) ||
    c.cedula?.includes(filtroBusqueda.value) ||
    c.email?.toLowerCase().includes(filtroBusqueda.value.toLowerCase())
  )
})

const mostrarModal = ref(false)

const formConductor = ref({
  nombre: '',
  apellido: '',
  cedula: '',
  email: '',
  licencia: '',
  tipoLicencia: 1,
  fechaVencimientoLicencia: '',
  telefono: '',
  direccion: '',
  supervisorId: 0,
  estado: 1
})

const cargarConductores = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch('https://localhost:7221/api/Conductores', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.ok) {
    conductores.value = await response.json()
  }
}

const formatearTipoLicencia = (tipo) => {
  const tipos = {
    1: 'Categoría 1',
    2: 'Categoría 2',
    3: 'Categoría 3',
    4: 'Categoría 4'
  }

  return tipos[tipo] || 'No definida'
}

const formatearFecha = (fecha) => {
  if (!fecha) return '---'

  return new Date(fecha).toLocaleDateString('es-DO')
}

const guardarConductor = async () => {
  const token = localStorage.getItem('token_transporte')

  const payload = {
    ...formConductor.value,
    fechaVencimientoLicencia: formConductor.value.fechaVencimientoLicencia
      ? new Date(formConductor.value.fechaVencimientoLicencia).toISOString()
      : null
  }

  const response = await fetch('https://localhost:7221/api/Conductores', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(payload)
  })

  if (response.ok) {
    mostrarModal.value = false

    formConductor.value = {
      nombre: '',
      apellido: '',
      cedula: '',
      email: '',
      licencia: '',
      tipoLicencia: 1,
      fechaVencimientoLicencia: '',
      telefono: '',
      direccion: '',
      supervisorId: 0,
      estado: 1
    }

    await cargarConductores()
  } else {
    const error = await response.json()
    console.error('Error creando conductor:', error)
    alert('No se pudo crear el conductor.')
  }
}

onMounted(() => {
  cargarConductores()
})
</script>

<style scoped>
.section-header {
  margin-bottom: 26px;
}

.section-header h2 {
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
}

.section-header p {
  font-size: .9rem;
  color: #6b7280;
}

.card-panel {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 24px;
}

.table-responsive {
  overflow-x: auto;
}

.conductores-table {
  width: 100%;
  border-collapse: collapse;
}

.conductores-table th {
  background: #f9fafb;
  padding: 14px;
  font-size: .85rem;
}

.conductores-table td {
  padding: 14px;
}

.estado-badge {
  padding: 5px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 700;
}

.activo {
  background: #dcfce7;
  color: #166534;
}

.inactivo {
  background: #fee2e2;
  color: #991b1b;
}

.toolbar {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 20px;
}

.search-box {
  width: 320px;
  padding: 10px 14px;
  border: 1px solid #e5e7eb;
  border-radius: 20px;
}

.btn-primary {
  background: #111827;
  color: white;
  border: none;
  padding: 10px 18px;
  border-radius: 10px;
  font-weight: 700;
  cursor: pointer;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.45);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-container {
  width: 700px;
  max-width: 95%;
  background: white;
  border-radius: 18px;
  padding: 24px;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.btn-close {
  border: none;
  background: transparent;
  font-size: 20px;
  cursor: pointer;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 14px;
  margin-top: 20px;
}

.form-grid input,
.form-grid select {
  padding: 10px 12px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
}

.full-width {
  grid-column: 1 / -1;
}

.modal-actions {
  grid-column: 1 / -1;
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

.btn-cancel {
  padding: 10px 18px;
  border: none;
  border-radius: 10px;
  background: #e5e7eb;
  cursor: pointer;
}

.stat-card {
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 16px;
  box-shadow: 0 1px 2px rgba(0,0,0,.04);
}
</style>