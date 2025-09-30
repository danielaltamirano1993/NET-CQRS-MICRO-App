
USE BDD_Items;
GO

CREATE OR ALTER PROCEDURE dbo.sp_AsignarItem
    @ItemId INT,
    @UsuarioAsignadoId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UsuarioId       INT;
    DECLARE @UsuarioAnterior INT;
    DECLARE @FechaEntrega    DATETIME; 
    DECLARE @EstadoAnterior  VARCHAR(20);
    DECLARE @Relevancia      TINYINT;
    DECLARE @FechaUrgente	 DATETIME = DATEADD(
											    DAY, 
												3, 
												GETDATE()
											   );

    SELECT @Relevancia      = Relevancia,
           @UsuarioAnterior = UsuarioAsignado,
           @EstadoAnterior  = Estado,
           @FechaEntrega    = FechaEntrega
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

    -- PRIORIDAD 1: ASIGNACIÓN POR URGENCIA 
	-- (Fecha de entrega próxima a vencer: < 3 días)
    IF @FechaEntrega < @FechaUrgente
    BEGIN
        SELECT 
		TOP      1 
				 @UsuarioId = UsuarioId
        FROM     UsuarioReferencia
        WHERE	 Activo = 1
        ORDER BY ItemsPendientes  ASC, 
                 ItemsCompletados ASC;
    END
    
    -- PRIORIDAD 2: ASIGNACIÓN POR RELEVANCIA 
	-- (Si no es urgente)
    ELSE 
	IF @Relevancia = 2 -- Alta 
    BEGIN
        SELECT 
		TOP		 1
				 @UsuarioId = UsuarioId
        FROM     UsuarioReferencia
        WHERE    Activo = 1
        AND		 ItemsPendientes <= 3 
        ORDER BY ItemsCompletados ASC, 
                 ItemsPendientes  ASC; 
    END

	-- Baja Relevancia (Relevancia = 1)
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

    --Actualización de contadores    
    BEGIN TRANSACTION;
		BEGIN TRY
			UPDATE ItemTrabajo
			SET	   UsuarioAsignado = @UsuarioId,
			   	   Estado	       = 'Pendiente'
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
			SET	   ItemsPendientes = ItemsPendientes + 1
			WHERE  UsuarioId	   = @UsuarioId;

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