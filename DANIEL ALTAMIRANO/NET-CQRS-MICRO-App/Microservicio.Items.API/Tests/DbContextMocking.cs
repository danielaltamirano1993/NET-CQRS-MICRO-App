using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microservicio.Items.API.App.Services;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;

namespace Microservicio.Items.API.Tests.Services
{
    public static class DbContextMocking
    {
        public static Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> entities) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());
            return mockSet;
        }
    }

    public class AsignacionUrgenteServiceTests
    {
        private readonly Mock<ItemDbContext> _mockContext;
        private readonly AsignacionUrgenteService _service;
        private readonly List<ItemTrabajo> _items;
        private readonly List<UsuarioReferencia> _usuarios;

        public AsignacionUrgenteServiceTests()
        {
            _usuarios = new List<UsuarioReferencia>
            {
                UsuarioReferencia.Crear(1, "User A", "a@test.com", 5),
                UsuarioReferencia.Crear(2, "User B", "b@test.com", 3)
            };

            var hoy = DateTime.Now.Date;
            var fechaLimite = hoy.AddDays(3);

            _items = new List<ItemTrabajo>
            {
                new ItemTrabajo { ItemId = 101, Estado = "Pendiente", UsuarioAsignado = 1, Relevancia = 1, FechaEntrega = fechaLimite.AddDays(1), FechaCreacion = hoy },
                new ItemTrabajo { ItemId = 102, Estado = "Pendiente", UsuarioAsignado = 2, Relevancia = 1, FechaEntrega = fechaLimite.AddDays(1), FechaCreacion = hoy },
                new ItemTrabajo { ItemId = 103, Estado = "Nuevo", UsuarioAsignado = 0, Relevancia = 5, FechaEntrega = fechaLimite.AddDays(-1), FechaCreacion = hoy.AddHours(-3) },
                new ItemTrabajo { ItemId = 104, Estado = "Nuevo", UsuarioAsignado = null, Relevancia = 3, FechaEntrega = fechaLimite.AddDays(-1), FechaCreacion = hoy.AddHours(-2) },
                new ItemTrabajo { ItemId = 105, Estado = "Nuevo", UsuarioAsignado = 0, Relevancia = 1, FechaEntrega = fechaLimite.AddDays(-1), FechaCreacion = hoy.AddHours(-1) },
                new ItemTrabajo { ItemId = 106, Estado = "Nuevo", UsuarioAsignado = 0, Relevancia = 5, FechaEntrega = fechaLimite.AddDays(5), FechaCreacion = hoy },
                new ItemTrabajo { ItemId = 107, Estado = "Completado", UsuarioAsignado = 1, Relevancia = 5, FechaEntrega = fechaLimite.AddDays(-1), FechaCreacion = hoy }
            };

            _mockContext = new Mock<ItemDbContext>();

            var mockItems = DbContextMocking.GetMockDbSet(_items.AsQueryable());
            var mockUsuarios = DbContextMocking.GetMockDbSet(_usuarios.AsQueryable());

            _mockContext.Setup(c => c.ItemTrabajo).Returns(mockItems.Object);
            _mockContext.Setup(c => c.UsuarioReferencia).Returns(mockUsuarios.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);

            _service = new AsignacionUrgenteService(_mockContext.Object);
        }

        [Fact]
        public async Task AsignarItemsUrgentesAsync_DebeAsignarYRotarCarga()
        {
            await _service.AsignarItemsUrgentesAsync();

            Assert.Equal(2, _items.Single(i => i.ItemId == 103).UsuarioAsignado);
            Assert.Equal(1, _items.Single(i => i.ItemId == 104).UsuarioAsignado);
            Assert.Equal(2, _items.Single(i => i.ItemId == 105).UsuarioAsignado);
            Assert.Equal(0, _items.Single(i => i.ItemId == 106).UsuarioAsignado);

            Assert.All(new[] { 103, 104, 105 }, id =>
            {
                var item = _items.Single(i => i.ItemId == id);
                Assert.Equal("Pendiente", item.Estado);
            });

            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AsignarItemsUrgentesAsync_DebeRespetarLimiteDeCarga()
        {
            var user1 = _usuarios.Single(u => u.UsuarioId == 1);
            var user2 = _usuarios.Single(u => u.UsuarioId == 2);

            user1.Actualizar(user1.NombreUsuario, user1.Correo, 2);
            user2.Actualizar(user2.NombreUsuario, user2.Correo, 2);

            _items.Add(new ItemTrabajo
            {
                ItemId = 108,
                Estado = "Nuevo",
                UsuarioAsignado = 0,
                Relevancia = 6,
                FechaEntrega = DateTime.Now.Date.AddDays(2),
                FechaCreacion = DateTime.Now.AddHours(-4)
            });

            await _service.AsignarItemsUrgentesAsync();

            Assert.Equal(2, _items.Single(i => i.ItemId == 108).UsuarioAsignado);
            Assert.Equal(1, _items.Single(i => i.ItemId == 103).UsuarioAsignado);

            Assert.Equal(0, _items.Single(i => i.ItemId == 104).UsuarioAsignado);
            Assert.Equal(0, _items.Single(i => i.ItemId == 105).UsuarioAsignado);

            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AsignarItemsUrgentesAsync_NoDebeGuardarSiNoHayItemsUrgentes()
        {
            _items.ForEach(i => i.FechaEntrega = DateTime.Now.Date.AddDays(10));

            await _service.AsignarItemsUrgentesAsync();

            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
