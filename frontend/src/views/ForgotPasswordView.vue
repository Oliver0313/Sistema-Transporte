<template>
  <div class="forgot-layout">
    <div class="forgot-card">
      <h2>Recuperar contraseña</h2>
      <p>Ingresa tu correo y te enviaremos instrucciones para recuperar el acceso.</p>

      <form @submit.prevent="enviarRecuperacion">
        <label>Correo electrónico</label>

        <input
          v-model="correo"
          type="email"
          placeholder="usuario@correo.com"
          required
        />

        <div v-if="mensaje" class="success-alert">
          {{ mensaje }}
        </div>

        <div v-if="errorMensaje" class="error-alert">
          {{ errorMensaje }}
        </div>

        <div v-if="resetLink" class="reset-link-box">
          <p>Enlace de recuperación generado:</p>

          <RouterLink :to="resetLink.replace('http://localhost:5173', '')">
            Abrir enlace de recuperación
          </RouterLink>
        </div>

        <button type="submit" :disabled="cargando">
          {{ cargando ? 'Enviando...' : 'Enviar instrucciones' }}
        </button>
      </form>

      <RouterLink to="/login" class="back-link">
        Volver al inicio de sesión
      </RouterLink>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const correo = ref('')
const mensaje = ref('')
const errorMensaje = ref('')
const resetLink = ref('')
const cargando = ref(false)

const enviarRecuperacion = async () => {
  cargando.value = true
  mensaje.value = ''
  errorMensaje.value = ''
  resetLink.value = ''

  try {
    const response = await fetch('https://localhost:7221/api/auth/forgot-password', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        correo: correo.value
      })
    })

    const data = await response.json()

    if (!response.ok) {
      throw new Error(data.mensaje || 'No se pudo procesar la solicitud.')
    }

    mensaje.value = data.mensaje || 'Solicitud procesada correctamente.'
    resetLink.value = data.resetLink || ''
    correo.value = ''
  } catch (error) {
    errorMensaje.value = error.message
  } finally {
    cargando.value = false
  }
}
</script>

<style scoped>
.forgot-layout {
  min-height: 100vh;
  background: #f9fafb;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}

.forgot-card {
  width: 420px;
  max-width: 100%;
  background: #ffffff;
  border: 1px solid #e5e7eb;
  border-radius: 18px;
  padding: 32px;
}

.forgot-card h2 {
  margin: 0;
  color: #111827;
  font-size: 1.6rem;
}

.forgot-card p {
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

.success-alert {
  background: #dcfce7;
  color: #166534;
  border: 1px solid #bbf7d0;
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

.error-alert {
  background: #fee2e2;
  color: #991b1b;
  border: 1px solid #fecaca;
  border-radius: 10px;
  padding: 10px 12px;
  font-size: 0.85rem;
}

.reset-link-box {
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  padding: 12px;
  font-size: 0.85rem;
}

.reset-link-box p {
  margin: 0 0 8px;
  color: #374151;
}

.reset-link-box a {
  color: #2563eb;
  font-weight: 600;
  text-decoration: none;
}
</style>