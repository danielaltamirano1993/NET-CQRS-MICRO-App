using Microservicio.Items.API.App.Services.Contracts;
using Microservicio.Items.API.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Microservicio.Items.API.Infrastructure.Repositories
{
    public class UsuarioReferenciaRepository : IUsuarioReferenciaRepository
    {
        private readonly ItemDbContext _context;

        public UsuarioReferenciaRepository(ItemDbContext context)
        {
            _context = context;
        }

        public async Task<UsuarioReferencia?> GetByIdAsync(int userId)
        {
            return await _context.UsuarioReferencia.FindAsync(userId);
        }

        public async Task UpdateAsync(UsuarioReferencia usuario)
        {
            _context.UsuarioReferencia.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
