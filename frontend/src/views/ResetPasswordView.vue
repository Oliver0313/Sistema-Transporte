<template>
  <div class="reset-layout">
    <div class="reset-card">
      <h2>Restablecer contraseña</h2>
      <p>Ingresa tu nueva contraseña para recuperar el acceso.</p>

      <form @submit.prevent="cambiarContrasena">
        <label>Nueva contraseña</label>
        <input
          v-model="nuevaContrasena"
          type="password"
          placeholder="Nueva contraseña"
          required
        />

        <label>Confirmar contraseña</label>
        <input
          v-model="confirmarContrasena"
          type="password"
          placeholder="Confirmar contraseña"
          required
        />

        <div v-if="mensaje" class="success-alert">
          {{ mensaje }}
        </div>

        <div v-if="errorMensaje" class="error-alert">
          {{ errorMensaje }}
        </div>

        <button type="submit" :disabled="cargando">
          {{ cargando ? 'Actualizando...' : 'Cambiar contraseña' }}
        </button>
      </form>

      <RouterLink to="/login" class="back-link">
        Volver al inicio de sesión
      </RouterLink>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

const token = ref('')
const nuevaContrasena = ref('')
const confirmarContrasena = ref('')

const mensaje = ref('')
const errorMensaje = ref('')
const cargando = ref(false)

onMounted(() => {
  token.value = route.query.token || ''
})

const cambiarContrasena = async () => {
  mensaje.value = ''
  errorMensaje.value = ''

  if (nuevaContrasena.value !== confirmarContrasena.value) {
    errorMensaje.value = 'Las contraseñas no coinciden.'
    return
  }

  cargando.value = true

  try {
    const response = await fetch(
      'https://localhost:7221/api/auth/reset-password',
      {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          token: token.value,
          nuevaContrasena: nuevaContrasena.value
        })
      }
    )

    const data = await response.json()

    if (!response.ok) {
      throw new Error(data.mensaje || 'No se pudo cambiar la contraseña.')
    }

    mensaje.value = data.mensaje

    setTimeout(() => {
      router.push('/login')
    }, 2500)

  } catch (error) {
    errorMensaje.value = error.message
  } finally {
    cargando.value = false
  }
}
</script>

<style scoped>
.reset-layout {
  min-height: 100vh;
  background: #f9fafb;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}

.reset-card {
  width: 420px;
  max-width: 100%;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 32px;
}

.reset-card h2 {
  margin: 0;
  color: #111827;
  font-size: 1.6rem;
}

.reset-card p {
  color: #6b7280;
  font-size: 0.9rem;
  margin: 10px 0 24px;
}

form {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

label {
  font-size: 0.85rem;
  font-weight: 600;
  color: #374151;
}

input {
  padding: 12px 14px;
  border: 1px solid #d1d5db;
  border-radius: 12px;
  outline: none;
}

input:focus {
  border-color: #111827;
}

button {
  margin-top: 8px;
  background: #111827;
  color: white;
  border: none;
  border-radius: 12px;
  padding: 12px;
  cursor: pointer;
  font-weight: 600;
}

button:disabled {
  background: #9ca3af;
  cursor: not-allowed;
}

.success-alert {
  background: #dcfce7;
  color: #166534;
  border: 1px solid #bbf7d0;
  border-radius: 10px;
  padding: 10px 12px;
  font-size: 0.85rem;
}

.error-alert {
  background: #fee2e2;
  color: #991b1b;
  border: 1px solid #fecaca;
  border-radius: 10px;
  padding: 10px 12px;
  font-size: 0.85rem;
}

.back-link {
  display: block;
  text-align: center;
  margin-top: 22px;
  color: #111827;
  font-size: 0.85rem;
  font-weight: 600;
  text-decoration: none;
}

.back-link:hover {
  text-decoration: underline;
}
</style>