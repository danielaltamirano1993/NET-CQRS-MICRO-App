namespace Microservicio.Items.API.App.Services.Contracts
{
    public interface IReordenamientoService
    {     
        Task ReordenarItemsPendientesAsync(
            int userId
        );
    }
}
