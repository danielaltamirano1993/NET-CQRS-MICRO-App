using MediatR;
using Microservicio.Items.API.App.Services;
using Microservicio.Items.API.App.Services.Contracts;
using Microservicio.Items.API.Infrastructure;
using Microservicio.Items.API.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ItemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(typeof(Program).Assembly);

builder.Services.AddScoped<IAsignacionService, AsignacionService>();
builder.Services.AddScoped<IUsuarioReferenciaRepository, UsuarioReferenciaRepository>();
builder.Services.AddScoped<IReordenamientoService, ReordenamientoService>();
builder.Services.AddScoped<ISincronizacionUsuarioService, SincronizacionUsuarioService>();

builder.Services.AddHttpClient("UsuariosApi", client =>
{
    var baseUrl = builder.Configuration["MicroserviciosConfig:UrlUsuarios"];
    if (baseUrl != null)
    {
        client.BaseAddress = new Uri(baseUrl);
    }
});

builder.Services.AddHttpClient<ISincronizacionUsuarioService, SincronizacionUsuarioService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5191/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var syncService = scope.ServiceProvider.GetRequiredService<ISincronizacionUsuarioService>();
    syncService.SincronizarUsuariosAsync().Wait();
}

await DataSeeder.EnsureUsersSynchronizedAsync(app.Services, app.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
