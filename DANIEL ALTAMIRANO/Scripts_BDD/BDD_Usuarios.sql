
USE master;
GO

--DROP DATABASE IF EXISTS BDD_Usuarios;
--CREATE DATABASE BDD_Usuarios;
--GO

USE [BDD_Usuarios]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Tabla principal de usuarios
CREATE TABLE [dbo].[Usuario](
	[UsuarioId]		[int] IDENTITY(1,1) NOT NULL,
	[NombreUsuario] [nvarchar](100)		NOT NULL,
	[Correo]		[nvarchar](200)		NOT NULL,
	[Activo]		[bit]				NOT NULL,
	[FechaRegistro] [datetime]			NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UsuarioId] ASC
)WITH (
PAD_INDEX = OFF, 
STATISTICS_NORECOMPUTE = OFF, 
IGNORE_DUP_KEY = OFF, 
ALLOW_ROW_LOCKS = ON, 
ALLOW_PAGE_LOCKS = ON, 
OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
[Correo] ASC
)WITH (
PAD_INDEX = OFF, 
STATISTICS_NORECOMPUTE = OFF, 
IGNORE_DUP_KEY = OFF, 
ALLOW_ROW_LOCKS = ON, 
ALLOW_PAGE_LOCKS = ON, 
OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Tabla de estadísticas del usuario
CREATE TABLE [dbo].[UsuarioEstadistica](
	[EstadisticaId]		  [int] IDENTITY(1,1) NOT NULL,
	[UsuarioId]			  [int]				  NOT NULL,
	[ItemsPendientes]	  [int]				  NOT NULL,
	[ItemsCompletados]    [int]				  NOT NULL,
	[UltimaActualizacion] [datetime]		  NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Usuario] 
ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Usuario] 
ADD  DEFAULT (GETDATE()) FOR [FechaRegistro]
GO
ALTER TABLE [dbo].[UsuarioEstadistica] 
ADD  DEFAULT ((0)) FOR [ItemsPendientes]
GO
ALTER TABLE [dbo].[UsuarioEstadistica] 
ADD  DEFAULT ((0)) FOR [ItemsCompletados]
GO
ALTER TABLE [dbo].[UsuarioEstadistica] 
ADD  DEFAULT (GETDATE()) FOR [UltimaActualizacion]
GO
ALTER TABLE [dbo].[UsuarioEstadistica]  
WITH CHECK ADD  CONSTRAINT [FK_UsuarioEstadistica_Usuario] 
FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuario] ([UsuarioId])
GO
ALTER TABLE [dbo].[UsuarioEstadistica] 
CHECK CONSTRAINT [FK_UsuarioEstadistica_Usuario]
GO

