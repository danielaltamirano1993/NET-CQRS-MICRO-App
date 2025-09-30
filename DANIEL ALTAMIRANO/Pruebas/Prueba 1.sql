-- Prueba 1
-- Para correr las pruebas, correr BDD_Items y luego volver aqui
-- Correr 1 vez para que se cree el Item URGENTE
-- (Usuario Asignado en NULL en la 1era y 2da ejecucion como patron)

-- Esta prueba a nivel SQL es dinamica, 
-- por lo que se puede seguir ejecutando y ver el asignamiento dinamico

-- Creacion del Ítem URGENTE (FechaEntrega = 2025-10-02)
-- Debería asignarse a Usuario2 (1 pendiente) porque es el que tiene menor carga.

DECLARE @UsuarioId_P1 INT;
DECLARE @ItemId_P1    INT;

SET @ItemId_P1 = SCOPE_IDENTITY()

INSERT INTO ItemTrabajo (
	Titulo, 
	Descripcion, 
	FechaEntrega, 
	Relevancia, 
	Estado, 
	UsuarioAsignado
)
VALUES (
		'Item_Urgente_P1', 
		'Prueba de Urgencia', 
		DATEADD( DAY, 1, GETDATE() ), 
		2, 
		'Pendiente', 
		NULL
);

SELECT   *
FROM     ItemTrabajo
ORDER BY FechaCreacion
DESC

EXEC dbo.sp_AsignarItem 
	 @ItemId		    = @ItemId_P1, 
	 @UsuarioAsignadoId = @UsuarioId_P1 OUTPUT;

-- Verificación de resultados de Prueba 1
SELECT 'Prueba 1: Urgencia' AS Prueba, 
	   @UsuarioId_P1        AS UsuarioAsignado, 
	   (
		SELECT Estado 
		FROM   ItemTrabajo 
		WHERE  ItemId = @ItemId_P1
	   )					AS NuevoEstado;

SELECT UsuarioId, 
	   ItemsPendientes, 
	   ItemsCompletados 
FROM   UsuarioReferencia 
WHERE  UsuarioId IN (1, 2, 3);
GO
/*
 RESULTADO ESPERADO:
 UsuarioAsignado: 2
 Contadores: Usuario1=3/0, 
			   Usuario2=2/0, 
			   Usuario3=2/1
*/