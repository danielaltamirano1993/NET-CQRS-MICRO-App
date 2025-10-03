namespace Microservicio.Items.API.App.Dto
{
    public record SincronizacionResultDto(
    bool Exito,
    int UsuariosProcesados,
    int UsuariosNuevos,
    int UsuariosActualizados,
    DateTime FechaEjecucion,
    string? MensajeDetalle
);

}
