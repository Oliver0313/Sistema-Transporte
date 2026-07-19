<template>
  <div class="login-layout">
    
    <div class="login-hero-side" :style="{ backgroundImage: `url(${imagenesCarrusel[indiceActivo]})` }">
      <div class="hero-overlay">
        <div class="hero-content">
        </div>
        
        <div class="hero-arrows">
          <span class="arrow-btn" @click="anteriorSlide">❮</span>
          <span class="arrow-btn" @click="siguienteSlide">❯</span>
        </div>

        <div class="carrusel-indicadores">
          <span 
            v-for="(img, index) in imagenesCarrusel" 
            :key="index"
            :class="{ activo: indiceActivo === index }"
            @click="indiceActivo = index"
          ></span>
        </div>
      </div>
    </div>

    <div class="login-form-side">
      <div class="form-wrapper">
        
        <div class="form-header">
          <img
            <img src="/src/assets/icons/logo-blanco.png" class="logo-login" alt="Trayecto">
          <h2>Bienvenido</h2>
          <p>Inicia sesión con tu información</p>
        </div>

        <form @submit.prevent="manejarLogin" class="custom-form">

          <div class="input-group">
            <label for="email">Email</label>
            <input type="email" id="email" v-model="email" placeholder="admin@gmail.com" required />
          </div>
          <div class="input-group">
            <label for="password">Contraseña</label>
            <input type="password" id="password" v-model="password" placeholder="••••••••••••" required />
          </div>

          <div class="forgot-password">
            <RouterLink to="/forgot-password">
              ¿Olvidaste tu contraseña?
            </RouterLink>
          </div>

          <div v-if="errorMensaje" class="alerta-error">
            {{ errorMensaje }}
          </div>

          <button type="submit" class="btn-login" :disabled="cargando">
            {{ cargando ? 'Verificando...' : 'Iniciar sesion' }}
          </button>
        </form>

        <div class="form-footer">
          <p>¿No tienes una cuenta con nosotros? <RouterLink to="/register" class="link-register">registrarse</RouterLink></p>
        </div>

      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

import imagenAutobus from "../assets/icons/autobus_login.png"
import imagenBus2 from "../assets/icons/autobus_register.png"
import imagenBus3 from "../assets/icons/autobus_login2.png"

const router = useRouter()
const email = ref('')
const password = ref('')
const cargando = ref(false)
const errorMensaje = ref('')

const imagenesCarrusel = [imagenAutobus, imagenBus2, imagenBus3]
const indiceActivo = ref(0)
let temporizador = null

const siguienteSlide = () => {
  if (indiceActivo.value < imagenesCarrusel.length - 1) {
    indiceActivo.value++
  } else {
    indiceActivo.value = 0
  }
}

const anteriorSlide = () => {
  if (indiceActivo.value > 0) {
    indiceActivo.value--
  } else {
    indiceActivo.value = imagenesCarrusel.length - 1
  }
}

onMounted(() => {
  temporizador = setInterval(siguienteSlide, 4500)
})

onUnmounted(() => {
  if (temporizador) clearInterval(temporizador)
})

const manejarLogin = async () => {
  cargando.value = true
  errorMensaje.value = ''

  try {
    const respuesta = await fetch('https://localhost:7221/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        correo: email.value,
        contrasena: password.value
      })
    })


    if (!respuesta.ok) {
      let detalleError = 'Error al iniciar sesión.'
      try {
        const dataError = await respuesta.json()
        if (dataError?.mensaje) detalleError = dataError.mensaje
        else if (respuesta.status === 401) detalleError = 'Correo o contraseña incorrectos.'
        else if (respuesta.status === 403) detalleError = 'El usuario se encuentra deshabilitado en el sistema.'
      } catch {
        if (respuesta.status === 401) detalleError = 'Correo o contraseña incorrectos.'
        if (respuesta.status === 403) detalleError = 'El usuario se encuentra deshabilitado en el sistema.'
      }
      throw new Error(detalleError)
    }

    const data = await respuesta.json()

    if (!data?.token) {
      throw new Error('La respuesta del servidor no contiene un token válido.')
    }

    const estado = parseInt(data.estadoUsuario)

if (estado === 3) {
  throw new Error('El usuario se encuentra deshabilitado en el sistema.')
}

if (estado === 2) {
  throw new Error('Tu cuenta ha sido bloqueada. Contacta al administrador.')
}


    localStorage.setItem('token_transporte', data.token)
    localStorage.setItem('usuario_rol',      data.rol)
    localStorage.setItem('usuario_nombre',   data.nombre)
    localStorage.setItem('usuario_id',       data.id)
    localStorage.setItem('usuario_estado',   data.estadoUsuario)

    router.push('/dashboard')

  } catch (err) {
    console.error('Error en login:', err)
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
  transition: background-image 0.6s ease-in-out;
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

.hero-content {
  color: #ffffff;
}

.hero-tagline {
  font-size: 0.9rem;
  font-weight: 700;
  text-transform: uppercase;
  margin-bottom: 40px;
  max-width: 80%;
  line-height: 1.4;
  letter-spacing: 0.5px;
}

.hero-footer-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.project-name {
  font-size: 0.85rem;
  font-weight: 600;
}

.project-email {
  font-size: 0.85rem;
  opacity: 0.7;
}

.hero-arrows {
  position: absolute;
  right: 60px;
  bottom: 60px;
  display: flex;
  gap: 20px;
  color: #ffffff;
  opacity: 0.7;
  z-index: 20;
}

.arrow-btn {
  cursor: pointer;
  font-size: 0.9rem;
  padding: 5px;
  user-select: none;
  transition: opacity 0.2s;
}

.arrow-btn:hover {
  opacity: 1;
}

.carrusel-indicadores {
  position: absolute;
  bottom: 24px;
  left: 60px;
  display: flex;
  gap: 8px;
  z-index: 20;
}

.carrusel-indicadores span {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: rgba(255, 255, 255, 0.4);
  cursor: pointer;
  transition: all 0.3s ease;
}

.carrusel-indicadores span.activo {
  background-color: #ffffff;
  width: 20px;
  border-radius: 4px;
}

.login-form-side {
  background-color: #ffffff;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px 50px;
}

.form-wrapper {
  width: 100%;
  max-width: 340px;
}

.form-header {
  text-align: center;
  margin-bottom: 22px;
}

.form-header h2 {
  font-size:1.55rem;
  font-weight: 700;
  color: #1a1a1a;
  margin-bottom: 8px;
}

.form-header p {
  font-size:0.82rem;
  color: var(--text-muted, #9ca3af);
}

.custom-form {
  display: flex;
  flex-direction: column;
  gap:16px;
}

.input-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.input-group label {
  font-size:0.80rem;
  font-weight: 600;
  color: var(--text-main, #4b5563);
}

.input-group input {
  width: 100%;
  padding:10px 14px;
  border: 1px solid var(--border-color, #e5e7eb);
  border-radius: var(--radius-md, 12px); 
  font-size: 0.9rem;
  color: #1f2937;
  outline: none;
  transition: border-color 0.2s ease;
}

.input-group input:focus {
  border-color: var(--text-muted, #9ca3af);
}

.btn-login {
  width: 100%;
  background-color: #222222;
  color: #ffffff;
  padding: 12px;
  border-radius: var(--radius-md, 12px);
  font-size: 0.95rem;
  font-weight: 600;
  border: none;
  cursor: pointer;
  margin-top: 4px;
  transition: background-color 0.2s ease;
}

.btn-login:hover {
  background-color: #111111;
}

.form-footer {
  text-align: center;
  margin-top:18px;
  font-size: 0.8rem;
  color: var(--text-muted, #9ca3af);
}

.link-register {
  color: #111827;
  font-weight: 700;
  text-decoration: none;
}

.link-register:hover {
  text-decoration: underline;
}

@media (max-width: 768px) {
  .login-layout {
    grid-template-columns: 1fr;
  }
  .login-hero-side {
    display: none;
  }
}

.alerta-error {
  background-color: #fef2f2;
  color: #dc2626;
  padding: 10px 14px;
  border-radius: 8px;
  font-size: 0.85rem;
  font-weight: 500;
  border: 1px solid #fee2e2;
  text-align: center;
}

button:disabled {
  background-color: #9ca3af;
  cursor: not-allowed;
}

.forgot-password {
  text-align: left;
  margin-top:-6px;
}

.forgot-password a {
  color: #111827;
  font-size: 0.8rem;
  font-weight: 600;
  text-decoration: none;
}

.logo-login{
    width:170px;
    height: auto;
    display: block;
    margin:0 auto 15px;
}
</style>