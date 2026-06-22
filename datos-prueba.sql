
-- Conductores de prueba

INSERT INTO Conductores
(Nombre, Apellido, Cedula, Telefono, Estado)
VALUES
('Juan', 'Pérez', '001-1234567-8', '8091234567', 1),
('María', 'Rodríguez', '001-9876543-2', '8097654321', 2);

-- Vehiculos

INSERT INTO Vehiculos
(
    Matricula,
    Marca,
    Modelo,
    Anio,
    Color,
    Capacidad,
    Estado,
    FechaUltimoMantenimiento,
    Kilometraje,
    Tipo,
    ImagenUrl
)
VALUES
('A123456', 'Toyota', 'Hiace', 2022, 'Blanco', 15, 1, '2026-05-01', 45000, 'Microbus', NULL),

('A123457', 'Hyundai', 'H1', 2021, 'Plateado', 12, 1, '2026-04-15', 52000, 'Van', NULL),

('A123458', 'Ford', 'Transit', 2020, 'Blanco', 18, 2, '2026-03-20', 70000, 'Microbus', NULL),

('A123459', 'Mercedes-Benz', 'Sprinter', 2023, 'Negro', 20, 1, '2026-05-10', 30000, 'Bus', NULL),

('A123460', 'Nissan', 'Urvan', 2019, 'Azul', 14, 3, '2026-02-15', 90000, 'Van', NULL),

('A123461', 'Chevrolet', 'Express', 2021, 'Gris', 12, 1, '2026-04-28', 48000, 'Van', NULL),

('A123462', 'Isuzu', 'NQR', 2018, 'Blanco', 25, 4, '2025-12-10', 120000, 'Camion', NULL),

('A123463', 'Toyota', 'Coaster', 2022, 'Blanco', 30, 2, '2026-05-05', 35000, 'Bus', NULL),

('A123464', 'Hyundai', 'County', 2020, 'Rojo', 28, 1, '2026-04-01', 60000, 'Bus', NULL),

('A123465', 'Kia', 'Pregio', 2019, 'Blanco', 11, 3, '2026-01-20', 85000, 'Van', NULL);

-- Conductor

INSERT INTO Conductores
(
    Nombre,
    Apellido,
    Cedula,
    Email,
    Licencia,
    TipoLicencia,
    FechaVencimientoLicencia,
    Telefono,
    Direccion,
    SupervisorId,
    Estado
)
VALUES
(
    'Juan',
    'Perez',
    '00112345678',
    'juan.perez@sistema.com',
    'LIC-1001',
    1,
    '2028-05-10',
    '8095551001',
    'Santo Domingo Este',
    1,
    1
),

(
    'Maria',
    'Rodriguez',
    '00112345679',
    'maria.rodriguez@sistema.com',
    'LIC-1002',
    2,
    '2027-08-15',
    '8095551002',
    'Santo Domingo Norte',
    1,
    1
),

(
    'Carlos',
    'Gomez',
    '00112345680',
    'carlos.gomez@sistema.com',
    'LIC-1003',
    3,
    '2029-01-20',
    '8095551003',
    'Distrito Nacional',
    1,
    2
),

(
    'Ana',
    'Martinez',
    '00112345681',
    'ana.martinez@sistema.com',
    'LIC-1004',
    2,
    '2027-11-05',
    '8095551004',
    'Santo Domingo Oeste',
    1,
    1
),

(
    'Luis',
    'Fernandez',
    '00112345682',
    'luis.fernandez@sistema.com',
    'LIC-1005',
    4,
    '2026-12-01',
    '8095551005',
    'Boca Chica',
    1,
    3
),

(
    'Pedro',
    'Ramirez',
    '00112345683',
    'pedro.ramirez@sistema.com',
    'LIC-1006',
    3,
    '2028-03-12',
    '8095551006',
    'San Cristobal',
    1,
    4
),

(
    'Laura',
    'Santos',
    '00112345684',
    'laura.santos@sistema.com',
    'LIC-1007',
    1,
    '2027-09-22',
    '8095551007',
    'Los Alcarrizos',
    1,
    1
),

(
    'Miguel',
    'Torres',
    '00112345685',
    'miguel.torres@sistema.com',
    'LIC-1008',
    4,
    '2029-06-18',
    '8095551008',
    'Haina',
    1,
    2
);

-- Solicitud Transporte

INSERT INTO SolicitudesTransporte
(
    AreaSolicitante,
    CantidadColaboradores,
    FechaHoraSalida,
    FechaHoraRegreso,
    Destino,
    Motivo,
    Estado,
    UsuarioSolicitanteId
)
VALUES
(
    'Recursos Humanos',
    5,
    '2026-07-01 08:00:00',
    '2026-07-01 17:00:00',
    'Santiago',
    'Capacitacion de personal',
    1,
    1
),

(
    'Finanzas',
    3,
    '2026-07-03 09:00:00',
    '2026-07-03 15:00:00',
    'San Pedro de Macoris',
    'Auditoria interna',
    2,
    2
),

(
    'Tecnologia',
    4,
    '2026-07-05 07:30:00',
    '2026-07-05 18:00:00',
    'La Romana',
    'Implementacion de infraestructura',
    2,
    1
),

(
    'Operaciones',
    8,
    '2026-07-08 06:00:00',
    '2026-07-08 20:00:00',
    'Puerto Plata',
    'Supervision de proyecto',
    3,
    3
),

(
    'Compras',
    2,
    '2026-07-10 08:00:00',
    '2026-07-10 14:00:00',
    'Haina',
    'Visita a proveedor',
    4,
    2
),

(
    'Gerencia',
    6,
    '2026-07-12 09:00:00',
    '2026-07-12 19:00:00',
    'Punta Cana',
    'Reunion corporativa',
    5,
    1
),

(
    'Logistica',
    10,
    '2026-07-15 05:30:00',
    '2026-07-15 21:00:00',
    'San Francisco de Macoris',
    'Distribucion de equipos',
    2,
    3
),

(
    'Marketing',
    4,
    '2026-07-18 08:00:00',
    '2026-07-18 17:00:00',
    'Bavaro',
    'Evento promocional',
    1,
    2
);

-- Asignaciones

INSERT INTO Asignaciones
(
    SolicitudTransporteId,
    ConductorId,
    VehiculoId,
    FechaHoraAsignacion,
    UsuarioAsignadorId,
    Estado
)
VALUES
(1, 1, 1, '2026-06-28 10:00:00', 1, 1),
(2, 2, 2, '2026-06-30 09:30:00', 1, 4),
(3, 3, 3, '2026-07-02 08:00:00', 1, 4),
(4, 4, 4, '2026-07-04 11:00:00', 1, 3),
(5, 5, 5, '2026-07-06 13:00:00', 1, 3),
(6, 6, 6, '2026-07-08 14:30:00', 1, 4),
(7, 7, 7, '2026-07-10 07:00:00', 1, 1),
(8, 8, 8, '2026-07-12 09:15:00', 1, 2);

-- Viajes 

INSERT INTO Viajes
(
    AsignacionId,
    FechaHoraSalida,
    FechaHoraLlegada,
    Estado,
    Observaciones,
    Origen,
    Destino,
    CantidadPasajeros,
    DistanciaRecorrida
)
VALUES
(
    1,
    '2026-07-01 08:00:00',
    NULL,
    1,
    'Viaje programado para capacitacion',
    'Santo Domingo',
    'Santiago',
    5,
    0
),

(
    2,
    '2026-07-03 09:00:00',
    '2026-07-03 15:30:00',
    3,
    'Viaje completado sin incidentes',
    'Santo Domingo',
    'San Pedro de Macoris',
    3,
    140
),

(
    3,
    '2026-07-05 07:30:00',
    '2026-07-05 18:00:00',
    3,
    'Instalacion realizada exitosamente',
    'Santo Domingo',
    'La Romana',
    4,
    230
),

(
    4,
    '2026-07-08 06:00:00',
    NULL,
    4,
    'Solicitud cancelada',
    'Santo Domingo',
    'Puerto Plata',
    8,
    0
),

(
    5,
    '2026-07-10 08:00:00',
    NULL,
    4,
    'Proveedor cancelo visita',
    'Santo Domingo',
    'Haina',
    2,
    0
),

(
    6,
    '2026-07-12 09:00:00',
    '2026-07-12 19:00:00',
    3,
    'Reunion corporativa completada',
    'Santo Domingo',
    'Punta Cana',
    6,
    420
),

(
    7,
    '2026-07-15 05:30:00',
    NULL,
    2,
    'Viaje actualmente en curso',
    'Santo Domingo',
    'San Francisco de Macoris',
    10,
    180
),

(
    8,
    '2026-07-18 08:00:00',
    NULL,
    1,
    'Evento programado',
    'Santo Domingo',
    'Bavaro',
    4,
    0
);

-- Mantenimiento 

INSERT INTO Mantenimientos
(
    VehiculoId,
    FechaMantenimiento,
    Estado,
    TipoMantenimiento,
    Descripcion,
    Costo,
    Taller,
    ProximoMantenimiento
)
VALUES
(
    1,
    '2026-06-01',
    4,
    1,
    'Cambio de aceite y filtros',
    3500,
    'Taller Central',
    '2026-12-01'
),

(
    2,
    '2026-06-10',
    4,
    2,
    'Reparacion del sistema de frenos',
    12000,
    'Auto Servicio Nacional',
    '2026-12-10'
),

(
    3,
    '2026-06-15',
    3,
    2,
    'Cambio de suspension',
    18000,
    'Mecanica Express',
    '2026-12-15'
),

(
    4,
    '2026-06-20',
    1,
    1,
    'Mantenimiento preventivo programado',
    5000,
    'Taller Central',
    '2026-12-20'
),

(
    5,
    '2026-05-05',
    5,
    1,
    'Mantenimiento pendiente vencido',
    4500,
    'Auto Servicio Nacional',
    '2025-11-05'
),

(
    6,
    '2026-06-25',
    2,
    3,
    'Revision predictiva de motor',
    6500,
    'Mecanica Express',
    '2026-12-25'
),

(
    7,
    '2026-06-28',
    4,
    1,
    'Cambio de neumaticos',
    22000,
    'Taller Central',
    '2027-01-28'
),

(
    8,
    '2026-07-01',
    1,
    3,
    'Diagnostico predictivo general',
    7000,
    'Mecanica Express',
    '2027-01-01'
);

-- Consumo Combustible 

INSERT INTO ConsumosCombustible
(
    VehiculoId,
    Fecha,
    Galones,
    Costo,
    ConductorId,
    KilometrosRecorridos,
    Estado
)
VALUES
(1, '2026-06-01', 12.5, 3500, 1, 180, 2),
(2, '2026-06-03', 10.0, 2800, 2, 150, 2),
(3, '2026-06-05', 15.3, 4300, 3, 220, 1),
(4, '2026-06-07', 18.0, 5100, 4, 260, 2),
(5, '2026-06-10', 9.5, 2600, 5, 120, 3),
(6, '2026-06-12', 11.2, 3150, 6, 175, 2),
(7, '2026-06-15', 20.0, 5700, 7, 310, 1),
(8, '2026-06-18', 13.7, 3900, 8, 200, 2),
(9, '2026-06-20', 16.4, 4600, 1, 240, 3),
(10, '2026-06-22', 14.0, 4000, 2, 210, 2);

