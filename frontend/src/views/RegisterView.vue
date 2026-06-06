<template>
  <div class="login-layout">
    
    <div class="login-hero-side" :style="{ backgroundImage: `url('${imagenAutobus}')` }">
      <div class="hero-overlay">
        <div class="hero-content">
          <p class="hero-tagline">SISTEMA INTELIGENTE DE GESTIÓN CON MONITOREO DE TRANSPORTE</p>
          
          <div class="hero-footer-info">
            <span class="project-name">Transporte Frontend</span>
            <span class="project-email">transportefrontend@gmail.com</span>
          </div>
        </div>
        
        <div class="hero-arrows">
          <span class="arrow-btn">❮</span>
          <span class="arrow-btn">❯</span>
        </div>
      </div>
    </div>

    <div class="login-form-side">
      <div class="form-wrapper">
        
        <div class="form-header">
          <h2>Crear Cuenta</h2>
          <p>Regístrate para solicitar tus viajes de transporte</p>
        </div>

        <form @submit.prevent="manejarRegistro" class="custom-form">
          <div class="input-group">
            <label for="nombre">Nombre</label>
            <input type="text" id="nombre" v-model="nombre" placeholder="Ej: Juan" required />
          </div>
          <div class="input-group">
            <label for="apellido">Apellido</label>
            <input type="text" id="apellido" v-model="apellido" placeholder="Ej: Pérez" required />
          </div>

          <div class="input-group">
            <label for="email">Email</label>
            <input type="email" id="email" v-model="email" placeholder="usuario@gmail.com" required />
          </div>

          <div class="input-group">
            <label for="password">Contraseña</label>
            <input type="password" id="password" v-model="password" placeholder="••••••••••••" required />
          </div>

   
          <div class="input-group">
            <label for="confirmPassword">Confirmar Contraseña</label>
            <input type="password" id="confirmPassword" v-model="confirmPassword" placeholder="••••••••••••" required />
          </div>


          <div v-if="errorMensaje" class="alerta-error">
            {{ errorMensaje }}
          </div>
          <div v-if="exitoMensaje" class="alerta-exito">
            {{ exitoMensaje }}
          </div>


          <button type="submit" class="btn-login" :disabled="cargando">
            {{ cargando ? 'Registrando...' : 'Registrarse' }}
          </button>
        </form>

        <div class="form-footer">
          <p>¿Ya tienes una cuenta? <RouterLink to="/login" class="link-register">Inicia sesión</RouterLink></p>
        </div>

      </div>
    </div>

  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import imagenAutobus from "../assets/icons/autobus_register.png"

const router = useRouter()

const nombre = ref('')
const apellido = ref('')
const email = ref('') 
const password = ref('') 
const confirmPassword = ref('')


const cargando = ref(false)
const errorMensaje = ref('')
const exitoMensaje = ref('')

const manejarRegistro = async () => {
  cargando.value = true
  errorMensaje.value = ''
  exitoMensaje.value = ''


  if (password.value !== confirmPassword.value) {
    errorMensaje.value = 'Las contraseñas no coinciden.'
    cargando.value = false
    return
  }

  if (password.value.length < 6) {
    errorMensaje.value = 'La contraseña debe tener al menos 6 caracteres.'
    cargando.value = false
    return
  }

  const URL_API = 'https://localhost:7221/api/auth/register'

  try {
    const respuesta = await fetch(URL_API, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        Nombre: nombre.value,
        Apellido: apellido.value,
        Correo: email.value,
        Contrasena: password.value,
        RolId: 3 
      })
    })

  
    if (!respuesta.ok) {
      if (respuesta.status === 400) {

        const datosError = await respuesta.json().catch(() => null)
        throw new Error(datosError?.mensaje || 'Los datos proporcionados no son válidos o el correo ya existe.')
      }
      throw new Error('No se pudo conectar con el servidor de registro.')
    }


    exitoMensaje.value = '¡Cuenta creada con éxito! Redirigiendo al inicio de sesión...'
    

    nombre.value = ''
    apellido.value = ''
    email.value = ''
    password.value = ''
    confirmPassword.value = ''

    setTimeout(() => {
      router.push('/login')
    }, 2500)

  } catch (err) {
    console.error('Error detectado en el proceso de registro:', err)
    errorMensaje.value = err.message
  } finally {
    cargando.value = false
  }
}
</script>

<style scoped>

.login-layout {
  display: grid;
  grid-template-columns: 58fr 42fr;
  height: 100vh;
  width: 100vw;
  max-height: 100vh;
  overflow: hidden;
  font-family: system-ui, -apple-system, sans-serif;
}
.login-hero-side {
  position: relative;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  background-color: #111827;
}
.hero-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, rgba(0,0,0,0.1) 0%, rgba(0,0,0,0.7) 100%);
  padding: 60px;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
}
.hero-content { color: #ffffff; }
.hero-tagline { font-size: 0.9rem; font-weight: 700; text-transform: uppercase; margin-bottom: 40px; max-width: 80%; line-height: 1.4; letter-spacing: 0.5px; }
.hero-footer-info { display: flex; flex-direction: column; gap: 4px; }
.project-name { font-size: 0.85rem; font-weight: 600; }
.project-email { font-size: 0.85rem; opacity: 0.7; }
.hero-arrows { position: absolute; right: 60px; bottom: 60px; display: flex; gap: 20px; color: #ffffff; opacity: 0.7; }
.arrow-btn { cursor: pointer; font-size: 0.9rem; padding: 5px; user-select: none; }

.login-form-side { background-color: #ffffff; display: flex; align-items: center; justify-content: center; padding: 20px 60px; overflow-y: auto; }
.form-wrapper { width: 100%; max-width: 360px; }
.form-header { text-align: center; margin-bottom: 16px; }
.form-header h2 { font-size: 1.6rem; font-weight: 700; color: #1a1a1a; margin-bottom: 4px; }
.form-header p { font-size: 0.85rem; color: #9ca3af; }
.custom-form { display: flex; flex-direction: column; gap: 12px; }
.input-group { display: flex; flex-direction: column; gap: 4px; }
.input-group label { font-size: 0.8rem; font-weight: 600; color: #4b5563; }
.input-group input { width: 100%; padding: 8px 12px; border: 1px solid #e5e7eb; border-radius: 12px; font-size: 0.85rem; color: #1f2937; outline: none; }
.btn-login { width: 100%; background-color: #222222; color: #ffffff; padding: 12px; border-radius: 12px; font-size: 0.9rem; font-weight: 600; border: none; cursor: pointer; margin-top: 6px; }
.form-footer { text-align: center; margin-top: 16px; font-size: 0.8rem; color: #9ca3af; }
.link-register { color: #111827; font-weight: 700; text-decoration: none; }
.link-register:hover { text-decoration: underline; }

.alerta-error { background-color: #fef2f2; color: #dc2626; padding: 10px; border-radius: 12px; font-size: 0.8rem; border: 1px solid #fee2e2; text-align: center; }
.alerta-exito { background-color: #f0fdf4; color: #16a34a; padding: 10px; border-radius: 12px; font-size: 0.8rem; border: 1px solid #dcfce7; text-align: center; }
button:disabled { background-color: #9ca3af; cursor: not-allowed; }

@media (max-width: 768px) { .login-layout { grid-template-columns: 1fr; } .login-hero-side { display: none; } }
</style>