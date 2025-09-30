-- Prueba 2 
-- Para correr las pruebas, correr BDD_Items y luego volver aqui
-- Correr 1 vez para que se cree el Item_Saturacion_P2

-- Esta prueba a nivel SQL es dinamica, 
-- por lo que se puede seguir ejecutando y ver el asignamiento dinamico

-- Ítem de Alta Relevancia NO urgente
-- Usuario1 (3 Pendientes) está SATURADO para Relevancia 2. 
-- El ítem debe ir a Usuario2 (2 pendientes) por desempate.

-- Una vez que se saturen, deberia arrojar
-- No hay usuario disponible para asignar. Todos saturados o inactivos.

DECLARE @UsuarioId_P2 INT;
DECLARE @ItemId_P2    INT;

SET @ItemId_P2 = SCOPE_IDENTITY();

INSERT INTO ItemTrabajo (
	Titulo, 
	Descripcion, 
	FechaEntrega, 
	Relevancia, 
	Estado, 
	UsuarioAsignado
)
VALUES (
		'Item_Saturacion_P2', 
		'Prueba de Saturación', 
		DATEADD(DAY, 5, GETDATE()), 
		2, 
		'Pendiente', 
		NULL
	   );


EXEC dbo.sp_AsignarItem 
	 @ItemId		    = @ItemId_P2, 
	 @UsuarioAsignadoId = @UsuarioId_P2 OUTPUT;

-- Verificación de resultados de Prueba 2
SELECT 'Prueba 2: Saturación' AS Prueba, 
	   @UsuarioId_P2		  AS UsuarioAsignado, 
	   (
		SELECT Estado 
		FROM   ItemTrabajo 
		WHERE  ItemId = @ItemId_P2
	   )					  AS NuevoEstado;

SELECT UsuarioId, 
	   ItemsPendientes, 
	   ItemsCompletados 
FROM   UsuarioReferencia 
WHERE  UsuarioId IN (1, 2, 3);
GO

/*
RESULTADO ESPERADO:
UsuarioAsignado: 2 por que Usuario1 saltado, 
						   Usuario2 / Usuario3 empatan en Pendientes, 
						   Usuario2 gana en Completados
Contadores: Usuario1=3/0, 
		    Usuario2=3/0, 
			Usuario3=2/1
*/