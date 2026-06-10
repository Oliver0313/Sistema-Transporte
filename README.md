# Sistema de Gestión de Transporte

## Descripción

Sistema web para la gestión de transporte institucional desarrollado bajo una arquitectura por capas utilizando ASP.NET Core Web API y Vue.js.

El sistema permite administrar vehículos, conductores, solicitudes de transporte, viajes, mantenimientos, consumos de combustible, usuarios y reportes operativos.

---

## Tecnologías Utilizadas

### Backend

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* JWT Authentication
* Swagger

### Frontend

* Vue.js 3
* Vue Router
* JavaScript
* Bootstrap

---

## Arquitectura del Proyecto

El backend sigue una arquitectura por capas:

```text
SistemaTransporte.API
SistemaTransporte.Application
SistemaTransporte.Domain
SistemaTransporte.Infrastructure
```

### Domain

Contiene:

* Entidades
* Enumeraciones
* Reglas del dominio

### Application

Contiene:

* DTOs
* Interfaces
* Servicios

### Infrastructure

Contiene:

* DbContext
* Repositorios
* Acceso a datos

### API

Contiene:

* Controllers
* Configuración JWT
* Swagger
* CORS

---

## Funcionalidades

### Autenticación

* Registro de usuarios
* Inicio de sesión
* JWT Authentication
* Control de acceso por roles

### Gestión de Usuarios

* Consultar usuarios
* Actualizar usuarios
* Gestión de roles

### Gestión de Vehículos

* Crear vehículos
* Consultar vehículos
* Actualizar vehículos
* Eliminar vehículos

### Gestión de Conductores

* Crear conductores
* Consultar conductores
* Actualizar conductores
* Eliminar conductores

### Solicitudes de Transporte

* Registrar solicitudes
* Consultar solicitudes
* Aprobar solicitudes
* Rechazar solicitudes

### Viajes

* Gestión de viajes
* Control de estados
* Registro de pasajeros

### Mantenimientos

* Registro de mantenimientos
* Seguimiento de estados
* Control de costos

### Consumo de Combustible

* Registro de combustible
* Aprobación de consumos
* Control de gastos

### Dashboard

* Resumen general del sistema
* Métricas operativas

### Reportes

* Vehículos
* Conductores
* Solicitudes
* Viajes
* Mantenimientos
* Combustible

---

## Roles

### SuperAdmin

Acceso completo al sistema.

### Administrador

Acceso a operaciones administrativas y reportes.

### Empleado

Acceso limitado según permisos asignados.

---

## Configuración del Backend

### Clonar repositorio

```bash
git clone https://github.com/Oliver0313/Sistema-Transporte.git
```

### Configurar base de datos

Modificar la cadena de conexión en:

```json
appsettings.json
```

### Aplicar migraciones

```bash
dotnet ef database update
```

### Ejecutar API

```bash
dotnet run
```

Swagger:

```text
https://localhost:7221/swagger
```

---

## Configuración del Frontend

Instalar dependencias:

```bash
npm install
```

Ejecutar proyecto:

```bash
npm run dev
```

Frontend:

```text
http://localhost:5173
```

---

## Autenticación JWT

El sistema utiliza JWT para proteger los endpoints.

Para acceder a recursos protegidos:

```http
Authorization: Bearer TOKEN
```

---

## Endpoints Principales

### Auth

```http
POST /api/auth/login
POST /api/auth/register
```

### Dashboard

```http
GET /api/dashboard/resumen
```

### Usuarios

```http
GET /api/usuarios
GET /api/usuarios/{id}
PUT /api/usuarios/{id}
```

### Vehículos

```http
GET /api/vehiculos
POST /api/vehiculos
PUT /api/vehiculos/{id}
DELETE /api/vehiculos/{id}
```

### Conductores

```http
GET /api/conductores
POST /api/conductores
PUT /api/conductores/{id}
DELETE /api/conductores/{id}
```

### Reportes

```http
GET /api/reportes/vehiculos
GET /api/reportes/conductores
GET /api/reportes/solicitudes
GET /api/reportes/viajes
GET /api/reportes/mantenimientos
GET /api/reportes/combustible
```

---

## Seguridad

* JWT Authentication
* BCrypt Password Hashing
* Role-Based Authorization
* DTO Validation
* CORS Configuration
* Swagger JWT Integration


