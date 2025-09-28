
USE BDD_Items;
GO

IF OBJECT_ID('dbo.sp_AsignarItem', 'P') IS NOT NULL
-- Procedimiento para asignar UN item
    DROP PROCEDURE dbo.sp_AsignarItem;
GO

CREATE 
OR ALTER PROCEDURE dbo.sp_AsignarItem
    @ItemId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Relevancia TINYINT;

    SELECT @Relevancia = Relevancia 
	FROM   ItemTrabajo 
	WHERE  ItemId = @ItemId;

    IF @Relevancia IS NULL
    BEGIN
        RAISERROR(
				  'Item no existe',
				  16,
				  1
				 ); 
		RETURN;
    END

    DECLARE @UsuarioId INT;

    IF @Relevancia = 2
    BEGIN
        -- Usuarios con menos items altamente relevantes (<3) y con menor carga
        SELECT 
		TOP		 1
				 @UsuarioId = UsuarioId --SELECT *
        FROM     UsuarioReferencia
        WHERE    Activo = 1 
		AND		 ItemsCompletados < 3
        ORDER BY ItemsCompletados 
		ASC;		
    END
    ELSE
    BEGIN
        SELECT 
		TOP		 1 
				 @UsuarioId = UsuarioId  --SELECT *
        FROM	 UsuarioReferencia
        WHERE	 Activo = 1
        ORDER BY ItemsPendientes 
		ASC;
    END

    IF @UsuarioId IS NULL
    BEGIN
        RAISERROR(
				  'No hay usuario disponible para asignar',
				  16,
				  1
				 ); 
		RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE ItemTrabajo
        SET	   UsuarioAsignado = @UsuarioId,
			   Estado		   = 'En Proceso'
        WHERE  ItemId		   = @ItemId;

        INSERT INTO HistorialAsignacion (
			ItemId, 
			UsuarioId, 
			FechaAsignacion, 
			EstadoAsignacion
		)
        VALUES (
			@ItemId, 
			@UsuarioId, 
			GETDATE(), 
			'Activa'
		);

        IF @Relevancia = 2
			UPDATE UsuarioReferencia
			SET	   ItemsPendientes = ItemsPendientes + 1
			WHERE  UsuarioId	   = @UsuarioId;
		ELSE
			UPDATE UsuarioReferencia
			SET    ItemsPendientes = ItemsPendientes + 1
			WHERE  UsuarioId	   = @UsuarioId;


        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
		DECLARE @ErrMsg NVARCHAR(4000);
		SET		@ErrMsg = ERROR_MESSAGE();
        RAISERROR (
				   @ErrMsg,
				   16,
				   1
				  );
    END CATCH
END
GO