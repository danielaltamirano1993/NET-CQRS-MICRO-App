using Microservicio.Items.API.Domain;

namespace Microservicio.Items.API.App.Services.Contracts
{
    public interface IAsignacionService
    {
        Task<UsuarioReferencia> AsignarUsuario(
            byte relevancia, 
            ItemTrabajo item
        );
    }
}
