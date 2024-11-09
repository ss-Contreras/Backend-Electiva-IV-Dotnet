using Microsoft.EntityFrameworkCore;
using SonrisasBackendv01.Data;
using SonrisasBackendv01.GestorMappers;
using SonrisasBackendv01.Repositorios;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
                opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));


// Agregamos los Repositorios
builder.Services.AddScoped<IPacienteRepositorio, PacienteRepositorio>();
builder.Services.AddScoped<IOdontologoRepositorio, OdontologoRepositorio>();
builder.Services.AddScoped<IConsultorioRepositorio, ConsultorioRepositorio>();
builder.Services.AddScoped<IHistorialRepositorio, HistorialRepositorio>();
builder.Services.AddScoped<ICitaRepositorio, CitaRepositorio>();
builder.Services.AddScoped<IRadiografiaRepositorio, RadiografiaRepositorio>();


var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");

// Agregamos el AutoMapper
builder.Services.AddAutoMapper(typeof(GestorMappers));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Soporte para CORS
// Configuración de CORS más flexible para el desarrollo, permitiendo cualquier origen.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Soporte para CORS
app.UseCors("AllowAll");

app.UseStaticFiles(); // Permite que wwwroot sirva archivos estáticos

app.UseAuthorization();

app.MapControllers();

app.Run();
