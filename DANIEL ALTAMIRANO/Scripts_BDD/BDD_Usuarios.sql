
USE master;
GO

DROP DATABASE IF EXISTS BDD_Usuarios;
CREATE DATABASE BDD_Usuarios;
GO

USE BDD_Usuarios;
GO

DROP TABLE IF EXISTS Usuario;
DROP TABLE IF EXISTS UsuarioEstadistica;

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
								PRIMARY KEY,
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
