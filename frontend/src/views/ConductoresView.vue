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

            <button class="btn-primary">
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
            <th>Teléfono</th>
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
              {{ conductor.telefono }}
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

onMounted(() => {
  cargarConductores()
})
</script>

<style scoped>
.section-header {
  margin-bottom: 26px;
}

.section-header h2 {
  font-size: 1.8rem;
  font-weight: 800;
  color: #111827;
  margin: 0;
}

.section-header p {
  color: #6b7280;
  font-size: 0.95rem;
  margin-top: 6px;
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
  text-align: left;
  font-size: 0.85rem;
  font-weight: 700;
  color: #374151;
}

.conductores-table td {
  padding: 14px;
  border-top: 1px solid #e5e7eb;
  font-size: 0.9rem;
  color: #374151;
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
</style>