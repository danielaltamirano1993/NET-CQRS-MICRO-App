using MediatR;
using Microservicio.Items.API.App.Services;
using Microservicio.Items.API.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole(); 
//builder.Logging.AddDebug();   
//builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.Services.AddDbContext<ItemDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//            .EnableSensitiveDataLogging()  
//            .LogTo(Console.WriteLine, LogLevel.Information));
builder.Services.AddMediatR(typeof(Program).Assembly);

builder.Services.AddScoped<AsignacionService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
