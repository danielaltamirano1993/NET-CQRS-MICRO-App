
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

-- CASO 1: Usuario SATURADO para nuevos ítems Relevancia=2
INSERT INTO UsuarioReferencia (UsuarioId, NombreUsuario, ItemsPendientes, ItemsCompletados, Activo														 					
) VALUES (1, 'Usuario_Caso1', 3, 0, 1); 

-- CASO 2: Usuario con menor carga pendiente, pero tiene un ítem Pendiente.
INSERT INTO UsuarioReferencia (UsuarioId, NombreUsuario, ItemsPendientes, ItemsCompletados, Activo														 					
) VALUES (2, 'Usuario_Caso2', 1, 0, 1);

-- CASO 3: Usuario con carga media.
INSERT INTO UsuarioReferencia (UsuarioId, NombreUsuario, ItemsPendientes, ItemsCompletados, Activo														 					
) VALUES (3, 'Usuario_Caso3', 2, 1, 1); 

-- items para CASO 2: Todos pendientes
INSERT INTO ItemTrabajo (Titulo, Descripcion, FechaEntrega, Relevancia, Estado, UsuarioAsignado)
VALUES 
-- items para Usuario1 (Saturado para alta relevancia)
('Item1_Alta_Usuario1', 'Usuario1 tiene 3 ítems ALTA', DATEADD(DAY, 10, GETDATE()), 2, 'Pendiente', 1),
('Item2_Alta_Usuario1', 'Usuario1 tiene 3 ítems ALTA', DATEADD(DAY, 11, GETDATE()), 2, 'Pendiente', 1),
('Item3_Alta_Usuario1', 'Usuario1 tiene 3 ítems ALTA', DATEADD(DAY, 12, GETDATE()), 2, 'Pendiente', 1),
-- item para Usuario2 (Carga más baja)
('Item4_Alta_Usuario2', 'Usuario2 tiene 1 ítem ALTA', DATEADD(DAY, 15, GETDATE()), 2, 'Pendiente', 2),
-- item para Usuario3 (Carga media + 1 completado)
('Item5_Baja_Usuario3', 'Usuario3 tiene 2 ítems (1 completado)', DATEADD(DAY, 20, GETDATE()), 1, 'Pendiente', 3),
('Item6_Comp_Usuario3', 'Usuario3 tiene 1 completado', DATEADD(DAY, 20, GETDATE()), 1, 'Completado', 3);
GO
