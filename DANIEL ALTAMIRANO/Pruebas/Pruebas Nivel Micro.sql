USE BDD_Items;
GO

--PRUEBAS 

SELECT *
FROM   BDD_Usuarios.dbo.Usuario
GO

--UPDATE UsuarioReferencia
--SET    LimiteItems = 2
--WHERE  LimiteItems = 3
--GO

SELECT *
FROM   UsuarioReferencia;
GO

SELECT COUNT(*) AS TotalItemTrabajo
FROM   ItemTrabajo;
GO

SELECT   *
FROM     ItemTrabajo
ORDER BY UsuarioAsignado;
GO

--Esta tabla se llena de acuerdo a las asignaciones en el micro
--y se ajusta dinamicamente con el endpoint ''
SELECT *
FROM   HistorialAsignacion;
GO


