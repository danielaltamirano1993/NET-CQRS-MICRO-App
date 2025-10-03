USE BDD_Items;
GO

--PRUEBAS 
SELECT *
FROM   BDD_Usuarios.dbo.Usuario
GO

--Actualizar contadores (Necesaria para verificar TotalItemTrabajo)
UPDATE UsuarioReferencia
SET 
    ItemsPendientes = (
	SELECT 
	COUNT(*) 
	FROM ItemTrabajo 
	WHERE UsuarioAsignado = UsuarioReferencia.UsuarioId 
	AND Estado = 'Pendiente'),
    ItemsCompletados = (
	SELECT 
	COUNT(*)
	FROM ItemTrabajo 
	WHERE UsuarioAsignado = UsuarioReferencia.UsuarioId 
	AND Estado = 'Completado')
WHERE UsuarioId IN (1,2,3);

SELECT *
FROM   UsuarioReferencia;
GO

SELECT COUNT(*) AS TotalItemTrabajo
FROM   ItemTrabajo;
GO

SELECT *
FROM   ItemTrabajo;
GO

--Relevancia >=2
SELECT ItemId,
	   UsuarioAsignado,
	   CASE
		  WHEN Relevancia = 1 
			THEN 'Baja'
		  ELSE 'Alta' 
       END AS Relevancia,
	   Estado
FROM   ItemTrabajo
WHERE  Relevancia >=2
ORDER BY UsuarioAsignado
GO

--Relevancia =1
SELECT ItemId,
	   UsuarioAsignado,
	   CASE 
          WHEN Relevancia = 1 
			THEN 'Baja'
          ELSE 'Alta' 
       END AS Relevancia,
	   Estado
FROM   ItemTrabajo
WHERE  Relevancia =1
ORDER BY UsuarioAsignado
GO

--Esta tabla se llena de acuerdo a las asignaciones en el micro
SELECT *
FROM   HistorialAsignacion;
GO


