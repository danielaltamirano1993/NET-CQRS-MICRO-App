
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

-- Tabla de usuarios de referencia (copia mínima para asignaciones)
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

-- Tabla principal de ítems
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
