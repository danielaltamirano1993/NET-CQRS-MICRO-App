using Microservicio.Items.API.Domain;

namespace Microservicio.Items.API.App.Services.Contracts
{
    public interface IUsuarioReferenciaRepository
    { 
        Task<UsuarioReferencia?> GetByIdAsync(int userId);

        Task UpdateAsync(UsuarioReferencia usuario);
    }
}
