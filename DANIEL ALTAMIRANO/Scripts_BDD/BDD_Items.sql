
USE master;
GO

--DROP DATABASE IF EXISTS BDD_Items;
--CREATE DATABASE BDD_Items;
--GO

USE BDD_Items;
GO

DROP TABLE IF EXISTS HistorialAsignacion;
DROP TABLE IF EXISTS ItemTrabajo;
DROP TABLE IF EXISTS UsuarioReferencia;

-- Tabla de usuarios de referencia (copia m�nima para asignaciones)
CREATE TABLE UsuarioReferencia (
    UsuarioId					INT
								PRIMARY KEY,
    NombreUsuario				NVARCHAR(100) NOT NULL,
	ItemsPendientes				INT NOT NULL 
								DEFAULT(0),
    ItemsCompletados			INT NOT NULL 
								DEFAULT(0),
	Activo					    BIT NOT NULL 
								DEFAULT(1) --SI
);
GO

-- Tabla principal de �tems
CREATE TABLE ItemTrabajo (
    ItemId			INT IDENTITY(1,1) 
					PRIMARY KEY,
    Titulo			NVARCHAR(200) NOT NULL,
    Descripcion		NVARCHAR(MAX) NULL,
    FechaCreacion	DATETIME NOT NULL 
				    DEFAULT(GETDATE()),
    FechaEntrega	DATETIME NOT NULL,
    Relevancia		TINYINT NOT NULL 
					CHECK (
							Relevancia IN (
											1, -- 1 = Baja, 
											2  -- 2 = Alta
										   )
						  ), 
    Estado			VARCHAR(20) NOT NULL 
					CHECK (
							Estado IN (
										'Pendiente',
										'Completado'
									  )
						  ),
    UsuarioAsignado INT NULL,
    CONSTRAINT FK_ItemTrabajo_UsuarioReferencia 
	FOREIGN KEY (UsuarioAsignado) 
	REFERENCES UsuarioReferencia (UsuarioId)
);
GO

--Tabla Historial de asignaciones
CREATE TABLE HistorialAsignacion (
    HistorialId			INT IDENTITY(1,1)
						PRIMARY KEY,
    ItemId				INT NOT NULL,
    UsuarioId			INT NOT NULL,
    FechaAsignacion		DATETIME NOT NULL 
						DEFAULT(GETDATE()),
    EstadoAsignacion	VARCHAR(20) NOT NULL 
						CHECK (
							   EstadoAsignacion IN (
													'Activa',
													'Reasignada',
													'Cancelada'
												   )
							  ),
    CONSTRAINT FK_Historial_Item 
	FOREIGN KEY (ItemId) 
	REFERENCES ItemTrabajo(ItemId)
);
GO

-- CASO 1: Usuario reci�n creado, sin �tems
INSERT INTO UsuarioReferencia (UsuarioId, NombreUsuario)
VALUES (1, 'Usuario_Caso1');

-- CASO 2: Usuario con �tems asignados pero ninguno completado
INSERT INTO UsuarioReferencia (UsuarioId, NombreUsuario)
VALUES (2, 'Usuario_Caso2');

-- CASO 3: Usuario con �tems asignados y algunos completados
INSERT INTO UsuarioReferencia (UsuarioId, NombreUsuario)
VALUES (3, 'Usuario_Caso3');

-- �tems para CASO 2: Todos pendientes
INSERT INTO ItemTrabajo (Titulo, Descripcion, FechaEntrega, Relevancia, Estado, UsuarioAsignado)
VALUES 
('Item_Pendiente_1', 'Descripci�n item 1', DATEADD(DAY, 5, GETDATE()), 2, 'Pendiente', 2),
('Item_Pendiente_2', 'Descripci�n item 2', DATEADD(DAY, 7, GETDATE()), 1, 'Pendiente', 2),
('Item_Pendiente_3', 'Descripci�n item 3', DATEADD(DAY, 10, GETDATE()), 2, 'Pendiente', 2);

-- �tems para CASO 3: Mezcla de pendientes y completados
INSERT INTO ItemTrabajo (Titulo, Descripcion, FechaEntrega, Relevancia, Estado, UsuarioAsignado)
VALUES 
('Item_En_Proceso_4', 'Descripci�n item 4', DATEADD(DAY, 3, GETDATE()), 2, 'Pendiente', 3),
('Item_Completado_5', 'Descripci�n item 5', DATEADD(DAY, 2, GETDATE()), 1, 'Completado', 3),
('Item_Pendiente_6', 'Descripci�n item 6', DATEADD(DAY, 7, GETDATE()), 2, 'Pendiente', 3),
('Item_Pendiente_7', 'Descripci�n item 7', DATEADD(DAY, 5, GETDATE()), 2, 'Pendiente', 1),
('Item_Pendiente_8', 'Descripci�n item 8', DATEADD(DAY, 9, GETDATE()), 2, 'Pendiente', 1);
--('Item_Pendiente_9', 'Descripci�n item 9', DATEADD(DAY, 11, GETDATE()), 2, 'Pendiente', 1)

-- Historial CASO 2
INSERT INTO HistorialAsignacion (ItemId, UsuarioId, EstadoAsignacion)
SELECT ItemId, 2, 'Activa'
FROM ItemTrabajo
WHERE UsuarioAsignado = 2;

-- Historial CASO 3
INSERT INTO HistorialAsignacion (ItemId, UsuarioId, EstadoAsignacion)
SELECT ItemId, 3, 'Activa'
FROM ItemTrabajo
WHERE UsuarioAsignado = 3;

--Actualizar contadores
UPDATE UsuarioReferencia
SET 
    ItemsPendientes = (
	SELECT 
	COUNT(*) 
	FROM ItemTrabajo 
	WHERE UsuarioAsignado = UsuarioReferencia.UsuarioId 
	AND Estado = 'Pendiente'),
    ItemsCompletados = (
	SELECT 
	COUNT(*)
	FROM ItemTrabajo 
	WHERE UsuarioAsignado = UsuarioReferencia.UsuarioId 
	AND Estado = 'Completado')
WHERE UsuarioId IN (1,2,3);

--PRUEBAS
SELECT *
FROM   UsuarioReferencia;
GO

SELECT *
FROM   ItemTrabajo;
GO

SELECT ItemId,
	   UsuarioAsignado,
	   CASE
		  WHEN Relevancia = 1 
			THEN 'Baja'
		  ELSE 'Alta' 
       END AS Relevancia,
	   Estado
FROM   ItemTrabajo
WHERE  Relevancia >=2
GO

SELECT ItemId,
	   UsuarioAsignado,
	   CASE 
          WHEN Relevancia = 1 
			THEN 'Baja'
          ELSE 'Alta' 
       END AS Relevancia,
	   Estado
FROM   ItemTrabajo
WHERE  Relevancia =1
GO

SELECT *
FROM   HistorialAsignacion;
GO


