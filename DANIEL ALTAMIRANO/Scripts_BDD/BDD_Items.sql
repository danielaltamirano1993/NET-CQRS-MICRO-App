
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

USE [BDD_Items]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HistorialAsignacion](
	[HistorialId]	  [int] IDENTITY(1,1) NOT NULL,
	[ItemTrabajoId]   [int]				  NOT NULL,
	[UsuarioId]		  [int]				  NOT NULL,
	[FechaAsignacion] [datetime2](7)	  NOT NULL,
	[Comentarios]     [nvarchar](255)     NOT NULL,
 CONSTRAINT [PK_HistorialAsignacion] 
 PRIMARY KEY CLUSTERED 
(
[HistorialId] ASC
)
WITH (
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
CREATE TABLE [dbo].[ItemTrabajo](
	[ItemId]		  [int] IDENTITY(1,1) NOT NULL,
	[Titulo]		  [nvarchar](200)	  NOT NULL,
	[Descripcion]     [nvarchar](max)	  NULL,
	[FechaCreacion]   [datetime2](7)	  NOT NULL,
	[FechaEntrega]    [datetime2](7)	  NOT NULL,
	[Relevancia]	  [tinyint]			  NOT NULL,
	[Orden]			  [int]				  NOT NULL,
	[Estado]		  [nvarchar](20)      NOT NULL,
	[UsuarioAsignado] [int]				  NULL,
 CONSTRAINT [PK_ItemTrabajo] 
 PRIMARY KEY CLUSTERED 
(
[ItemId] ASC
)
WITH (
PAD_INDEX = OFF, 
STATISTICS_NORECOMPUTE = OFF, 
IGNORE_DUP_KEY = OFF, 
ALLOW_ROW_LOCKS = ON, 
ALLOW_PAGE_LOCKS = ON, 
OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY]
) ON [PRIMARY] 
TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsuarioReferencia](
	[UsuarioId]			  [int]			  NOT NULL,
	[NombreUsuario]		  [nvarchar](100) NOT NULL,
	[Correo]			  [nvarchar](max) NOT NULL,
	[LimiteItems]		  [int]			  NOT NULL,
	[Activo]			  [bit]			  NOT NULL,
	[FechaRegistro]		  [datetime2](7)  NOT NULL,
	[UltimaActualizacion] [datetime2](7)  NOT NULL,
	[ItemsPendientes]	  [int]			  NOT NULL,
	[ItemsCompletados]	  [int]			  NOT NULL,
 CONSTRAINT [PK_UsuarioReferencia] 
 PRIMARY KEY CLUSTERED 
(
[UsuarioId] ASC
)
WITH (
PAD_INDEX = OFF, 
STATISTICS_NORECOMPUTE = OFF, 
IGNORE_DUP_KEY = OFF, 
ALLOW_ROW_LOCKS = ON, 
ALLOW_PAGE_LOCKS = ON, 
OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF
) ON [PRIMARY]
) ON [PRIMARY] 
TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[HistorialAsignacion]  
WITH CHECK 
ADD  CONSTRAINT [FK_HistorialAsignacion_ItemTrabajo_ItemTrabajoId] 
FOREIGN KEY([ItemTrabajoId])
REFERENCES [dbo].[ItemTrabajo] ([ItemId])
GO
ALTER TABLE [dbo].[HistorialAsignacion] 
CHECK CONSTRAINT [FK_HistorialAsignacion_ItemTrabajo_ItemTrabajoId]
GO
ALTER TABLE [dbo].[HistorialAsignacion]  
WITH CHECK 
ADD  CONSTRAINT [FK_HistorialAsignacion_UsuarioReferencia_UsuarioId] 
FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[UsuarioReferencia] ([UsuarioId])
GO
ALTER TABLE [dbo].[HistorialAsignacion] 
CHECK CONSTRAINT [FK_HistorialAsignacion_UsuarioReferencia_UsuarioId]
GO
ALTER TABLE [dbo].[ItemTrabajo]  
WITH CHECK ADD  CONSTRAINT [FK_ItemTrabajo_UsuarioReferencia_UsuarioAsignado] 
FOREIGN KEY([UsuarioAsignado])
REFERENCES [dbo].[UsuarioReferencia] ([UsuarioId])
GO
ALTER TABLE [dbo].[ItemTrabajo] 
CHECK CONSTRAINT [FK_ItemTrabajo_UsuarioReferencia_UsuarioAsignado]
GO
