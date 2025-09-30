USE BDD_Items;
GO

CREATE
OR ALTER PROCEDURE dbo.sp_AsignarItem
    @ItemId			   INT,
    @UsuarioAsignadoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Relevancia		 TINYINT;
    DECLARE @UsuarioAnterior INT;
    DECLARE @EstadoAnterior  VARCHAR(20);
    DECLARE @UsuarioId		 INT;

    SELECT @Relevancia		= Relevancia,
           @UsuarioAnterior = UsuarioAsignado,
           @EstadoAnterior  = Estado
    FROM   ItemTrabajo
    WHERE  ItemId			= @ItemId;

    IF @Relevancia IS NULL
    BEGIN
        RAISERROR(
            'Item no existe',
            16,
            1
        );
        RETURN;
    END

    IF @Relevancia = 2
    BEGIN
        SELECT 
		TOP      1 
			     @UsuarioId = UsuarioId
        FROM     UsuarioReferencia
        WHERE    Activo = 1
        AND      ItemsPendientes <= 3
        ORDER BY ItemsCompletados ASC,
                 ItemsPendientes  ASC;
    END
    ELSE
    BEGIN
        SELECT 
		TOP		 1
				 @UsuarioId = UsuarioId
        FROM	 UsuarioReferencia
        WHERE	 Activo = 1
        ORDER BY ItemsPendientes  ASC,
                 ItemsCompletados ASC;
    END

    IF @UsuarioId IS NULL
    BEGIN
        RAISERROR(
            'No hay usuario disponible para asignar. Todos saturados o inactivos.',
            16,
            1
        );
        RETURN;
    END
    
    SET @UsuarioAsignadoId = @UsuarioId;

    BEGIN TRANSACTION;
		BEGIN TRY
			UPDATE ItemTrabajo
			SET	   UsuarioAsignado = @UsuarioId,
				   Estado		   = 'Pendiente' 
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

			IF  @UsuarioAnterior IS NOT NULL
			AND @UsuarioAnterior <> @UsuarioId
			BEGIN
				IF @EstadoAnterior = 'Pendiente' 
				BEGIN
					UPDATE UsuarioReferencia
					SET ItemsPendientes =
						CASE
							WHEN ItemsPendientes > 0
								THEN ItemsPendientes - 1
							ELSE 0
						END
					WHERE UsuarioId = @UsuarioAnterior;
				END
			END

			UPDATE UsuarioReferencia
			SET    ItemsPendientes = ItemsPendientes + 1
			WHERE  UsuarioId       = @UsuarioId;

			COMMIT TRANSACTION;
		END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrMsg NVARCHAR(4000);
        SET @ErrMsg = ERROR_MESSAGE();
        RAISERROR (
            @ErrMsg,
            16,
            1
        );
    END CATCH
END
GO