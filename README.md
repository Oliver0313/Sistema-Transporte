#  SistemaTransporte - Módulo de Planificación y Control Logístico

Bienvenidos a **SmartRoute**, un Sistema de Transporte: una plataforma empresarial avanzada orientada a la administración, planificación y control del transporte institucional utilizado por los colaboradores de la empresa.

Este sistema permite gestionar de forma centralizada vehículos, conductores, solicitudes de transporte, asignaciones, rutas y el monitoreo de viajes en tiempo real. Mediante un motor de estadísticas y auditoría, la solución garantiza un estricto control operativo, optimiza el rendimiento de la flota y previene activamente el uso indebido de los recursos institucionales.

Desarrollado como un proyecto de práctica profesional, el ecosistema combina el diseño de software limpio con tecnologías modernas de desarrollo y las mejores prácticas de la industria.

---

##  Estructura de la Solución (Capa Backend)

La arquitectura del backend está desarrollada bajo los principios de **Clean Architecture** y **DDD (Domain-Driven Design)**, dividida en proyectos totalmente desacoplados mediante Inversión de Dependencias. Esto garantiza la mantenibilidad del Core del negocio y la inyección segura de dobles de prueba (Mocks):

* **`SistemaTransporte.Domain`**: El núcleo puro del sistema logístico. Contiene las reglas del negocio independientes de cualquier framework:
  * **Entities**: Modelado base del sistema (`Usuario`, `Conductor`, `Vehiculo`, `Viaje`, `Mantenimiento`, `Asignacion`, `ConsumoCombustible`, etc.).
  * **Enums**: Tipados fuertes para el control de flujos operativos (`EstadoAsignacion`, `EstadoCombustible`, `EstadoConductor`, `EstadoMantenimiento`, `EstadoSolicitud`, `EstadoUsuario`, `EstadoViaje`, etc.).

* **`SistemaTransporte.Application`**: Capa encargada de implementar la lógica de los Casos de Uso del sistema:
  * **Services**: Orquestación lógica dividida de forma estricta entre contratos (`Interfaces`) e implementaciones (`Implementations` como `UsuarioService`, `VehiculoService`, `ViajeService`, `AuthService`, `ReportesService`, `ConductorService`, `MantenimientoService`).
  * **DTOs**: Estructuras de transferencia de datos optimizadas y segmentadas por entidad para asegurar los canales de comunicación con el cliente web.

* **`SistemaTransporte.Infrastructure`**: Capa dedicada a la infraestructura, persistencia y acceso a datos físicos:
  * **Persistencia**: Centraliza el contexto transaccional de Entity Framework Core mediante `AppDbContext`, configuraciones de tablas y el mapeo de factorías (`AppDbContextFactory`).
  * **Repositories**: Implementación concreta de los repositorios de datos y del patrón de acceso a base de datos.
  * **Migrations**: Historial de control de versiones de los esquemas de la base de datos (Code-First).

* **`SistemaTransporte.API`**: Capa perimetral del servidor web. Expone los controladores RESTful de la API (`UsuarioController`, `ConductorController`, `VehiculoController`, `MantenimientoController`, etc.), los archivos de configuración de entorno (`appsettings.json`), el punto de arranque de la aplicación (`Program.cs`) y la gestión de los middlewares de seguridad.

* **`SistemaTransporte.Tests`**: Suite de calidad que contiene el desarrollo de las 42 pruebas unitarias asíncronas aisladas para auditar y certificar la estabilidad de toda la lógica del negocio.

---

##  Stack Tecnológico & Librerías

### Backend
* **Framework:** .NET 8.0 (C# 12)
* **ORM / Persistencia:** Entity Framework Core (Enfoque Code-First con Fluent API).
* **Seguridad:** Autenticación y Autorización basada en Roles mediante Tokens **JWT (JSON Web Tokens)**.

### Calidad de Código & Testing (QA)
* **Motor de Pruebas:** **xUnit v2.5.3+** (Soporte nativo para paralelismo y asincronismo).
* **Aislamiento de Dependencias:** **Moq Framework** (Utilizado para interceptar y simular el comportamiento de repositorios e inyección de configuraciones).
* **Métricas de Éxito:** **42 Escenarios de Pruebas Unitarias** automatizadas ejecutadas con una tasa de éxito del **100%**, blindando las reglas de negocio contra regresiones.

### Frontend
* **Framework:** Vue.js 3 (Composition API) con TypeScript.
* **Geolocalización:** **Mapbox GL JS (`mapbox-gl`)** integrado para el renderizado de mapas interactivos vectoriales de alto rendimiento, control de coordenadas y visualización de rutas logísticas en tiempo real.
* **Diseño e Iconografía:** Set de componentes visuales modernos optimizados para dashboards administrativos con soporte para temas claro/oscuro e iconos vectoriales personalizados.
* **Seguridad y Flujos de Acceso:**
  * Autenticación y Autorización basada en Roles mediante Tokens **JWT (JSON Web Tokens)**.
  * Flujo de **Restablecimiento seguro de contraseñas** mediante tokens criptográficos temporales de un solo uso, validados dinámicamente mediante endpoints dedicados (`/api/auth/forgot-password` y `/api/auth/reset-password`).

---

##  División de Responsabilidades del Equipo

* **Desarrollador Backend & Desarrollador Frontend (Oliver):**
  * Diseño del Core del negocio, lógica transaccional de servicios e interfaces.
  * Modelado de base de datos (Entidades, Relaciones y Enums).
  * Integración del módulo logístico con mapas interactivos, filtros avanzados de búsqueda y elementos iconográficos personalizados.

* **Desarrollador Frontend, UI/UX & QA Engineer (Karen):**
  * Maquetación de las vistas reactivas en Vue 3 y consumo asíncrono de los Endpoints de la API.
  * Desarrollo completo de la suite de 42 pruebas unitarias automatizadas con xUnit/Moq y configuración del reporte de diagnóstico vía `xunit.runner.json`.

---

##  Capturas del Sistema

A continuación se muestra un recorrido visual por los módulos principales de la plataforma en funcionamiento.

### Autenticación

| Login | Recuperar contraseña |  Registrarse |
|---|---|---|
| ![Login](frontend/assets/docs/screenshots/login.png) | ![Recuperar contraseña](frontend/assets/docs/screenshots/recuperar-password.png) |  ![Registrar](frontend/assets/docs/screenshots/registrar.png) |

### Dashboard Principal

![Dashboard](frontend/assets/docs/screenshots/dashboard.png)

### Gestión de Vehículos

| Listado de vehículos | Crear vehiculo |  Acciones |
|---|---|---|
| ![Listado de vehículos](frontend/assets/docs/screenshots/vehiculos-listado.png) | ![Detalle de vehículo](frontend/assets/docs/screenshots/vehiculos-detalle.png) | ![Editar / Detalles / Eliminar](frontend/assets/docs/screenshots/vehiculos-acciones.png) |


### Gestión de Conductores

| Listado de conductores | Crear conductor |  Acciones |
|---|---|---|
| ![Listado de conductores](frontend/assets/docs/screenshots/condutores-listado.png) | ![Detalle de conductores](frontend/assets/docs/screenshots/condutores-detalle.png) | ![Editar / Detalles / Eliminar](frontend/assets/docs/screenshots/conductores-acciones.png) |

### Viajes y Mapa en Tiempo Real

| Listado de viajes | 
|---|
| ![Listado de viajes](frontend/assets/docs/screenshots/viajes-listado.png) |

### Calendario

| Listado de calendario |
|---|
| ![Listado de calendario](frontend/assets/docs/screenshots/calendario-listado.png) | 

### Solicitudes de Transporte

| Listado de solicitudes | Crear solicitudes |  Acciones |
|---|---|---|
| ![Listado de solicitudes](frontend/assets/docs/screenshots/solicitudes-listado.png) | ![Detalle de solicitudes](frontend/assets/docs/screenshots/solicitudes-detalle.png) | ![Editar / Detalles / Eliminar](frontend/assets/docs/screenshots/solicitudes-acciones.png) |

### Gestión de Asignaciones

| Listado de asignaciones | Crear asignaciones |  Acciones |
|---|---|---|
| ![Listado de asignaciones](frontend/assets/docs/screenshots/asignaciones-listado.png) | ![Detalle de conductores](frontend/assets/docs/screenshots/asignaciones-detalle.png) | ![Editar / Detalles / Eliminar](frontend/assets/docs/screenshots/asignaciones-acciones.png) |

### Mantenimiento de Flota

| Listado de mantenimiento | Crear mantenimiento |  Acciones |
|---|---|---|
| ![Listado de mantenimiento](frontend/assets/docs/screenshots/solicitudes-listado.png) | ![Detalle de mantenimiento](frontend/assets/docs/screenshots/solicitudes-detalle.png) | ![Editar / Detalles / Eliminar](frontend/assets/docs/screenshots/solicitudes-acciones.png) |

### Módulo de Reportes

| Dashboard de métricas | Exportación a Excel/PDF |
|---|---|
| ![Reportes dashboard](frontend/assets/docs/screenshots/reportes-dashboard.png) | ![Reporte exportado](frontend/assets/docs/screenshots/reportes-export.png) |

### Combustible

| Listado de combustible | Crear combustible |  Acciones |
|---|---|---|
| ![Listado de combustible](frontend/assets/docs/screenshots/combustible-listado.png) | ![Detalle de combustible](frontend/assets/docs/screenshots/combustible-detalle.png) | ![Editar / Detalles / Eliminar](frontend/assets/docs/screenshots/combustible-acciones.png) |

### Usuarios

| Listado de usuarios |  Acciones |
|---|---|
| ![Listado de usuarios](frontend/assets/docs/screenshots/usuario-listado.png) | ![Editar / Detalles / Eliminar](frontend/assets/docs/screenshots/usuario-acciones.png) |


##  Configuración de Variables de Entorno

Antes de ejecutar el proyecto, configura el archivo `SistemaTransporte.API/appsettings.json` con tus valores locales:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=SistemaTransporte;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Secret": "TU_CLAVE_SECRETA_JWT_MINIMO_32_CARACTERES",
    "Issuer": "SistemaTransporte",
    "Audience": "SistemaTransporteUsers",
    "ExpirationHours": 8
  }
}
```

---

##  Instalación y Ejecución

###  Requisitos Previos

* **SDK de .NET 8.0** o superior.
* **Node.js (Versión LTS)** para el servidor de desarrollo del Frontend.
* **SQL Server** (local o remoto) con la base de datos creada.
* **Visual Studio 2022 (v17.12+)** o **VS Code** con las extensiones de C# Dev Kit y Volar (para Vue).

---

###  Paquetes del Backend (NuGet)

* **Persistencia y Datos (`SistemaTransporte.Infrastructure`)**:
  * `Microsoft.EntityFrameworkCore` (v8.0.x)
  * `Microsoft.EntityFrameworkCore.SqlServer` (v8.0.x)
  * `Microsoft.EntityFrameworkCore.Tools` (v8.0.x)

* **Seguridad y Tokenización (`SistemaTransporte.API` / `SistemaTransporte.Application`)**:
  * `Microsoft.AspNetCore.Authentication.JwtBearer` (v8.0.x)
  * `System.IdentityModel.Tokens.Jwt` (v8.0.x)

* **Aseguramiento de la Calidad (`SistemaTransporte.Tests`)**:
  * `xunit` (v2.5.3+)
  * `xunit.runner.visualstudio` (v2.5.3+)
  * `Moq` (v4.20.x)
  * `Microsoft.NET.Test.Sdk` (v17.8.x)

---

###  Paquetes del Frontend (npm)

* **Enrutamiento y Estado**:
  * `vue` (v3.x)
  * `vue-router` (v4.x)

* **Módulo de Geolocalización Logística**:
  * `mapbox-gl`

---

### 1. Levantar el Backend (API REST)

Abre una terminal en la raíz de la solución y ejecuta:

```bash
# Restaurar todos los paquetes NuGet
dotnet restore

# Compilar los proyectos
dotnet build

# Aplicar migraciones a la base de datos
dotnet ef database update --project SistemaTransporte.Infrastructure --startup-project SistemaTransporte.API

# Ejecutar el servidor
dotnet run --project SistemaTransporte.API/SistemaTransporte.API.csproj
```

La API estará disponible en `https://localhost:7221`.

---

### 2. Levantar el Frontend

Abre una segunda terminal en la carpeta del frontend y ejecuta:

```bash
# Instalar dependencias
npm install

# Levantar el servidor de desarrollo
npm run dev
```

---

### 3. Ejecutar las Pruebas Unitarias

```bash
# Correr la suite completa
dotnet test

# Correr y generar reporte TRX
dotnet test --logger "trx;LogFileName=resultados.trx" --results-directory ./TestResults
```

> El reporte `.trx` se generará en `SistemaTransporte.Tests/TestResults/resultados.trx`.

---

##  Endpoints Principales

| Módulo | Método | Endpoint |
|---|---|---|
| Autenticación | POST | `/api/auth/login` |
| Autenticación | POST | `/api/auth/register` |
| Autenticación | POST | `/api/auth/forgot-password` |
| Autenticación | POST | `/api/auth/reset-password` |
| Usuarios | GET/POST/PUT/DELETE | `/api/usuarios` |
| Conductores | GET/POST/PUT/DELETE | `/api/conductores` |
| Vehículos | GET/POST/PUT/DELETE | `/api/vehiculos` |
| Viajes | GET/POST/PUT/DELETE | `/api/viajes` |
| Mantenimiento | GET/POST/PUT/DELETE | `/api/mantenimiento` |

