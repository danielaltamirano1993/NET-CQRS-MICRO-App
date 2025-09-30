
USE master;
GO

--DROP DATABASE IF EXISTS BDD_Usuarios;
--CREATE DATABASE BDD_Usuarios;
--GO

USE BDD_Usuarios;
GO

DROP TABLE IF EXISTS UsuarioEstadistica;
DROP TABLE IF EXISTS Usuario;

-- Tabla principal de usuarios
CREATE TABLE Usuario (
    UsuarioId		INT IDENTITY(1,1) 
					PRIMARY KEY,
    NombreUsuario	NVARCHAR(100) NOT NULL,
    Correo			NVARCHAR(200) NOT NULL 
					UNIQUE,
    Activo			BIT NOT NULL 
					DEFAULT(1),
    FechaRegistro	DATETIME NOT NULL 
					DEFAULT(GETDATE())
);
GO

-- Tabla de estadísticas del usuario
CREATE TABLE UsuarioEstadistica (
	EstadisticaId				INT 
								IDENTITY(1,1) NOT NULL,
    UsuarioId					INT NOT NULL, 
    ItemsPendientes				INT NOT NULL 
								DEFAULT(0),
    ItemsCompletados			INT NOT NULL 
								DEFAULT(0),
    UltimaActualizacion			DATETIME NOT NULL 
								DEFAULT(GETDATE())	
    CONSTRAINT FK_UsuarioEstadistica_Usuario 
	FOREIGN KEY (UsuarioId) 
	REFERENCES Usuario(UsuarioId)
);
GO

-- CASO 1: Usuario recién creado
INSERT INTO Usuario (NombreUsuario, Correo)
VALUES ('Usuario_Caso1', 'usuario1@ejemplo.com');

-- CASO 2: Usuario con ítems asignados
INSERT INTO Usuario (NombreUsuario, Correo)
VALUES ('Usuario_Caso2', 'usuario2@ejemplo.com');

-- CASO 3: Usuario inactivo
INSERT INTO Usuario (NombreUsuario, Correo, Activo)
VALUES ('Usuario_Caso3', 'usuario3@ejemplo.com', 0);


-- Estadística para Usuario_Caso1
--INSERT INTO UsuarioEstadistica (UsuarioId, ItemsPendientes, ItemsCompletados)
--VALUES (1, 0, 0);

---- Estadística para Usuario_Caso2
--INSERT INTO UsuarioEstadistica (UsuarioId, ItemsPendientes, ItemsCompletados)
--VALUES (2, 3, 0);  -- Por ejemplo, 3 ítems pendientes y ninguno completado

---- Estadística para Usuario_Caso3 (usuario inactivo)
--INSERT INTO UsuarioEstadistica (UsuarioId, ItemsPendientes, ItemsCompletados)
--VALUES (3, 1, 2);  -- Por ejemplo, 1 pendiente y 2 completados

