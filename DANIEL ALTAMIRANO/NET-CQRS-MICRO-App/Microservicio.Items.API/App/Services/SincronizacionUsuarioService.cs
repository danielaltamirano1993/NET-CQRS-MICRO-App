using Microservicio.Items.API.App.Services.Contracts;
using Microservicio.Items.API.Domain;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using Microservicio.Items.API.App.Services.Dtos;

namespace Microservicio.Items.API.App.Services
{
    public class SincronizacionUsuarioService : ISincronizacionUsuarioService
    {
        private readonly ItemDbContext _context;
        private readonly HttpClient _httpClient;

        public SincronizacionUsuarioService(ItemDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task SincronizarUsuariosAsync()
        {   
            List<UsuarioExternoDto>? usuariosExternos = null;

            try
            {
                usuariosExternos = await _httpClient.GetFromJsonAsync<List<UsuarioExternoDto>>("/api/Usuario");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(
                    $"ERROR DE CONEXIÓN CRÍTICO: " +
                    $"No se pudo conectar a la API de Usuarios. " +
                    $"Detalle: " +
                    $"{ex.Message}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"ERROR DE SINCRONIZACIÓN: " +
                    $"{ex.Message}"
                );
            }

            if (usuariosExternos == null || 
                !usuariosExternos.Any())
            {
                if (!await _context.UsuarioReferencia.AnyAsync())
                {
                    Console.WriteLine(
                        "La sincronización falló. " +
                        "Insertando usuario de Test para el arranque..."
                    );
                    var usuarioRescate = new UsuarioReferencia
                    {
                        UsuarioId = 9999,
                        NombreUsuario = "Usuario Test",
                        Activo = true,
                        ItemsPendientes = 0,
                        ItemsCompletados = 0
                    };
                    _context.UsuarioReferencia.Add(usuarioRescate);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("UsuarioTest (ID 9999) insertado.");
                    return;
                }
                Console.WriteLine("[ADVERTENCIA] " +
                                  "El microservicio de Usuarios no devolvió datos, pero existen usuarios de referencia previamente."
                );
                return;
            }

            var usuariosExistentes = await _context.UsuarioReferencia
                                                   .ToDictionaryAsync(u => u.UsuarioId);

            foreach (var usuarioExterno in usuariosExternos)
            {
                if (usuariosExistentes.TryGetValue(
                                                    usuarioExterno.UsuarioId, 
                                                    out var usuarioLocal
                                                  )
                )
                {
                    if (usuarioLocal.NombreUsuario != usuarioExterno.NombreUsuario ||
                        usuarioLocal.Activo != usuarioExterno.Activo)
                    {
                        usuarioLocal.NombreUsuario = usuarioExterno.NombreUsuario;
                        usuarioLocal.Activo = usuarioExterno.Activo;
                        _context.UsuarioReferencia.Update(usuarioLocal);
                    }
                }
                else
                {
                    var nuevoUsuario = new UsuarioReferencia
                    {
                        UsuarioId = usuarioExterno.UsuarioId,
                        NombreUsuario = usuarioExterno.NombreUsuario,
                        Activo = usuarioExterno.Activo,
                        ItemsPendientes = 0,
                        ItemsCompletados = 0
                    };
                    _context.UsuarioReferencia.Add(nuevoUsuario);
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine(
                $"[INFO] Sincronización finalizada. " +
                $"{usuariosExternos.Count} usuarios procesados."
            );
        }

        public async Task<UsuarioReferencia?> ObtenerUsuarioConMenorCargaAsync()
        {
            return await _context.UsuarioReferencia
                       .Where(u => u.Activo == true)
                       .OrderBy(u => u.ItemsPendientes)
                       .FirstOrDefaultAsync();
        }
    }
}