
USE BDD_Items;
GO

IF OBJECT_ID('dbo.sp_ListarPendientesPorUsuario', 'P') IS NOT NULL
-- Procedimiento para listar pendientes por usuario
    DROP PROCEDURE dbo.sp_ListarPendientesPorUsuario;
GO

CREATE 
OR ALTER PROCEDURE dbo.sp_ListarPendientesPorUsuario
    @UsuarioId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT   ItemId, 
		     Titulo, 
		     Descripcion, 
			 FechaCreacion,
		     FechaEntrega, 
		     Relevancia, 
		     Estado,
			 UsuarioAsignado
    FROM     ItemTrabajo
    WHERE	 UsuarioAsignado = @UsuarioId 
	AND		 Estado <> 'Completado'
    ORDER BY FechaEntrega 
	ASC, 
	Relevancia DESC;
END
GO
