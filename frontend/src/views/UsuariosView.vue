<template>
  <div>
    <div class="dashboard-header">
      <div>
        <h2>Gestión de Usuarios</h2>
        <p>Gestiona y administra los usuarios del sistema</p>
      </div>

      <div class="dashboard-header-badge">
        Panel administrativo
      </div>
    </div>

    <div v-if="mensajeExito" class="success-alert">
        {{ mensajeExito }}
    </div>

    <div class="stats-grid">
  <div class="stat-card">
    <div class="stat-icon">
      <Users :size="24" />
    </div>

      <div>
        <span>Usuarios totales</span>
        <strong>{{ totalUsuarios }}</strong>
        <p>Registrados en el sistema</p>
      </div>
    </div>

    <div class="stat-card">
      <div class="stat-icon">
        <UserCheck :size="24" />
      </div>

      <div>
        <span>Activos</span>
        <strong>{{ usuariosActivos }}</strong>
        <p>Con acceso habilitado</p>
      </div>
    </div>

    <div class="stat-card">
      <div class="stat-icon">
        <UserX :size="24" />
      </div>

      <div>
        <span>Inactivos</span>
        <strong>{{ usuariosInactivos }}</strong>
        <p>Acceso deshabilitado</p>
      </div>
    </div>

    <div class="stat-card">
      <div class="stat-icon">
        <ShieldCheck :size="24" />
      </div>

      <div>
        <span>Administradores</span>
        <strong>{{ administradores }}</strong>
        <p>Usuarios con permisos altos</p>
      </div>
    </div>
  </div>

    <div class="usuarios-layout">
      <section class="main-panel">
        <div class="panel-header">
          <h3>Lista de usuarios del sistema</h3>
        </div>

        <div class="toolbar">
          <input
            v-model="filtroBusqueda"
            type="text"
            placeholder="Buscar usuario..."
            class="search-box"
          />

          <select v-model="filtroRol" class="filter-select">
            <option value="todos">Todos los roles</option>
            <option value="Administrador">Administrador</option>
            <option value="Supervisor">Supervisor</option>
            <option value="Operador">Operador</option>
          </select>

          <select v-model="filtroEstado" class="filter-select">
            <option value="todos">Todos los estados</option>
            <option value="1">Activos</option>
            <option value="0">Inactivos</option>
          </select>
        </div>

        <table class="usuarios-table">
          <thead>
            <tr>
              <th>Usuario</th>
              <th>Rol</th>
              <th>Último acceso</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>

            <tbody>
            <tr v-for="usuario in usuariosFiltrados" :key="usuario.id">
                <td>
                <strong>{{ usuario.nombre }} {{ usuario.apellido }}</strong>
                <small>{{ usuario.correo }}</small>
                </td>

                <td>
                <span class="role-pill">
                    {{ usuario.rolNombre }}
                </span>
                </td>

                <td>{{ formatearFecha(usuario.ultimoAcceso) }}</td>

                <td>
                <span :class="['estado-pill', usuario.estadoUsuario === 1 ? 'activo' : 'inactivo']">
                    {{ usuario.estadoUsuario === 1 ? 'Activo' : 'Inactivo' }}
                </span>
                </td>
                <td>
                </td>
            </tr>

            <tr v-if="usuariosFiltrados.length === 0">
                <td colspan="5" class="empty-table">
                No se encontraron usuarios.
                </td>
            </tr>
            </tbody>
        </table>
      </section>

      <aside class="side-panel">
        <div class="side-card">
          <h3>Roles del sistema</h3>

          <div class="role-count">
            <span>Administrador</span>
            <strong>{{ contarRol('Administrador') }}</strong>
          </div>

          <div class="role-count">
            <span>Supervisor</span>
            <strong>{{ contarRol('Supervisor') }}</strong>
          </div>

          <div class="role-count">
            <span>Operador</span>
            <strong>{{ contarRol('Operador') }}</strong>
          </div>
        </div>

        <div class="side-card">
          <h3>Actividad reciente</h3>

          <div
            v-for="usuario in usuariosRecientes"
            :key="usuario.id"
            class="activity-item"
          >
            <strong>{{ usuario.nombre }} {{ usuario.apellido }}</strong>
            <span>{{ formatearFecha(usuario.ultimoAcceso) }}</span>
          </div>
        </div>
      </aside>
    </div>

    <div v-if="mostrarModal" class="modal-overlay">
      <div class="modal-container">
        <div class="modal-header">
          <h3>Editar usuario</h3>
          <button class="btn-close" @click="mostrarModal = false">×</button>
        </div>

        <div v-if="usuarioSeleccionado" class="modal-body">
          <p>
            <strong>{{ usuarioSeleccionado.nombre }} {{ usuarioSeleccionado.apellido }}</strong>
          </p>
          <p>{{ usuarioSeleccionado.correo }}</p>

          <label>Rol</label>
          <select v-model.number="formUsuario.rolId">
            <option value="1">Administrador</option>
            <option value="2">Supervisor</option>
            <option value="3">Operador</option>
          </select>

          <label>Estado</label>
          <select v-model.number="formUsuario.estadoUsuario">
            <option value="1">Activo</option>
            <option value="0">Inactivo</option>
          </select>

          <div class="modal-actions">
            <button class="btn-cancel" @click="mostrarModal = false">
              Cancelar
            </button>

            <button class="btn-save" @click="guardarUsuario">
              Guardar cambios
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { Users, UserCheck, UserX, ShieldCheck, Pencil } from 'lucide-vue-next'


const usuarios = ref([])
const filtroBusqueda = ref('')
const filtroRol = ref('todos')
const filtroEstado = ref('todos')
const mensajeExito = ref('')

const mostrarModal = ref(false)
const usuarioSeleccionado = ref(null)

const formUsuario = ref({
  rolId: 0,
  estadoUsuario: 1
})

const rolUsuario = (localStorage.getItem('usuario_rol') || '').trim().toLowerCase()

const esSuperAdmin = computed(() => rolUsuario === 'superadmin')
const esAdministrador = computed(() => rolUsuario === 'administrador' || rolUsuario === 'admin')

const puedeVerUsuarios = computed(() =>
  esSuperAdmin.value || esAdministrador.value
)

const puedeModificarUsuarios = computed(() =>
  esSuperAdmin.value
)

const cargarUsuarios = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch('https://localhost:7221/api/Usuarios', {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.ok) {
    usuarios.value = await response.json()
  }
}

const usuariosFiltrados = computed(() => {
  const texto = filtroBusqueda.value.toLowerCase().trim()
  let resultado = usuarios.value

  if (texto) {
    resultado = resultado.filter(u =>
      u.nombre?.toLowerCase().includes(texto) ||
      u.apellido?.toLowerCase().includes(texto) ||
      u.correo?.toLowerCase().includes(texto) ||
      u.rolNombre?.toLowerCase().includes(texto)
    )
  }

  if (filtroRol.value !== 'todos') {
    resultado = resultado.filter(u => u.rolNombre === filtroRol.value)
  }

  if (filtroEstado.value !== 'todos') {
    resultado = resultado.filter(u => u.estadoUsuario === Number(filtroEstado.value))
  }

  return resultado
})

const totalUsuarios = computed(() => usuarios.value.length)

const usuariosActivos = computed(() =>
  usuarios.value.filter(u => u.estadoUsuario === 1).length
)

const usuariosInactivos = computed(() =>
  usuarios.value.filter(u => u.estadoUsuario !== 1).length
)

const administradores = computed(() =>
  usuarios.value.filter(u => u.rolNombre === 'Administrador').length
)

const usuariosRecientes = computed(() =>
  [...usuarios.value]
    .sort((a, b) => new Date(b.ultimoAcceso) - new Date(a.ultimoAcceso))
    .slice(0, 5)
)

const contarRol = (rol) => {
  return usuarios.value.filter(u => u.rolNombre === rol).length
}

const abrirModal = (usuario) => {
  usuarioSeleccionado.value = usuario

  formUsuario.value = {
    rolId: usuario.rolId,
    estadoUsuario: usuario.estadoUsuario
  }

  mostrarModal.value = true
}

const guardarUsuario = async () => {
  const token = localStorage.getItem('token_transporte')

  const response = await fetch(`https://localhost:7221/api/Usuarios/${usuarioSeleccionado.value.id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    },
    body: JSON.stringify(formUsuario.value)
  })

  if (response.ok) {
    mostrarModal.value = false
    mensajeExito.value = 'Usuario actualizado correctamente.'

    await cargarUsuarios()

    setTimeout(() => {
      mensajeExito.value = ''
    }, 3000)
  } else {
    alert('No se pudo actualizar el usuario.')
  }
}

const formatearFecha = (fecha) => {
  if (!fecha) return '---'

  return new Date(fecha).toLocaleString('es-DO', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(() => {
  cargarUsuarios()
})
</script>

<style scoped>
.section-header { margin-bottom: 20px; }

.section-header h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: #111827;
}

.section-header p {
  margin-top: 6px;
  color: #6b7280;
  font-size: 0.9rem;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 18px;
  margin-bottom: 24px;
}

.stat-card {
  position: relative;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 18px;
  display: grid;
  grid-template-columns: 48px 1fr;
  column-gap: 14px;
  align-items: start;
  box-shadow: 0 8px 22px rgba(15, 23, 42, 0.04);
  transition: all .2s ease;
}

.stat-card::before {
  content: "";
  position: absolute;
  left: 18px;
  right: 18px;
  top: 0;
  height: 3px;
  border-radius: 999px;
  background: #111827;
  opacity: .08;
}

.stat-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 14px 30px rgba(15, 23, 42, 0.08);
}

.stat-icon {
  width: 44px;
  height: 44px;
  border-radius: 14px;
  background: #f3f4f6;
  color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
}

.stat-card span {
  display: block;
  font-size: .88rem;
  font-weight: 700;
  color: #374151;
}

.stat-card strong {
  display: block;
  font-size: 1.85rem;
  font-weight: 800;
  color: #111827;
  line-height: 1.2;
  margin-top: 4px;
}

.stat-card p {
  margin-top: 6px;
  font-size: .82rem;
  color: #6b7280;
}

.stat-card span { font-size: 0.85rem; color: #4b5563; }
.stat-card strong { display: block; margin-top: 4px; font-size: 1.4rem; color: #111827; }

.usuarios-layout {
  display: grid;
  grid-template-columns: 2fr 0.8fr;
  gap: 20px;
}

.main-panel,
.side-card {
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 16px;
  padding: 20px;
}

.panel-header h3,
.side-card h3 {
  margin: 0 0 16px;
  font-size: 1rem;
  font-weight: 700;
  color: #111827;
}

.toolbar {
  display: flex;
  gap: 12px;
  margin-bottom: 16px;
}

.search-box {
  flex: 1;
  max-width: 380px;
  height: 40px;
  padding: 0 14px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background: #ffffff;
  font-size: .9rem;
  transition: all .2s ease;
}

.search-box { flex: 1; }

.search-box:focus {
  outline: none;
  border-color: #9ca3af;
  box-shadow: 0 0 0 3px rgba(17, 24, 39, 0.05);
}

.modal-body select {
  height: 40px;
  padding: 0 12px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background: #ffffff;
}

.usuarios-table {
  width: 100%;
  border-collapse: collapse;
}

.usuarios-table th {
  background: #f9fafb;
  text-align: left;
  padding: 14px 16px;
  font-size: 0.82rem;
  font-weight: 800;
  color: #374151;
  text-transform: uppercase;
  letter-spacing: .03em;
  white-space: nowrap;
}

.usuarios-table td {
  padding: 15px 16px;
  border-top: 1px solid #e5e7eb;
  font-size: .88rem;
  color: #374151;
  vertical-align: middle;
}

.usuarios-table td strong {
  display: block;
  color: #111827;
}

.usuarios-table tbody tr {
  transition: background .2s ease;
}

.usuarios-table tbody tr:hover {
  background: #fafafa;
}

.usuarios-table td small {
  display: block;
  color: #6b7280;
  margin-top: 3px;
}

.role-pill,
.estado-pill {
  padding: 5px 10px;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 600;
}

.role-pill {
  background: #eef2ff;
  color: #3730a3;
}

.estado-pill.activo {
  background: #dcfce7;
  color: #166534;
}

.estado-pill.inactivo {
  background: #fee2e2;
  color: #991b1b;
}

.btn-action {
  width: 32px;
  height: 32px;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  background: #ffffff;
  color: #374151;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.btn-action:hover {
  background: #f3f4f6;
  color: #111827;
}

.side-panel {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.role-count,
.activity-item {
  display: flex;
  justify-content: space-between;
  padding: 12px 0;
  border-top: 1px solid #f3f4f6;
}

.activity-item {
  flex-direction: column;
  gap: 4px;
}

.activity-item span {
  color: #6b7280;
  font-size: 0.8rem;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 999;
}

.modal-container {
  width: 520px;
  max-width: 95%;
  background: white;
  border-radius: 16px;
  padding: 24px;
}

.modal-header,
.modal-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.btn-close {
  background: transparent;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
}

.modal-body {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.modal-actions {
  justify-content: flex-end;
  gap: 10px;
  margin-top: 12px;
}

.btn-cancel,
.btn-save {
  border: none;
  border-radius: 8px;
  padding: 10px 16px;
  cursor: pointer;
}

.btn-cancel { background: #e5e7eb; }
.btn-save { background: #111827; color: white; }

@media (max-width: 1200px) {
  .stats-grid,
  .usuarios-layout {
    grid-template-columns: repeat(2, 1fr);
  }

  @media (max-width: 700px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
}

.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
  margin-bottom: 24px;
  flex-wrap: wrap;
  }   
}

.success-alert {
  background: #dcfce7;
  color: #166534;
  border: 1px solid #bbf7d0;
  border-radius: 10px;
  padding: 12px 16px;
  margin-bottom: 18px;
  font-size: 0.9rem;
}

.empty-table {
  text-align: center;
  color: #6b7280;
  padding: 28px;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 22px;
}

.dashboard-header h2 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 800;
  color: #111827;
  letter-spacing: -0.02em;
}

.dashboard-header p {
  margin-top: 6px;
  color: #6b7280;
  font-size: 0.9rem;
}

.dashboard-header-badge {
  background: #ecfdf5;
  color: #166534;
  border: 1px solid #bbf7d0;
  border-radius: 999px;
  padding: 8px 14px;
  font-size: 0.82rem;
  font-weight: 700;
}
</style>