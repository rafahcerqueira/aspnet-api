using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.DataBase;
using minimal_api.Infraestrutura.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServicos, AdministradorServicos>();

// Adiciona Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "Minimal API",
            Description = "Uma API minimalista com ASP.NET Core",
        });
    }
);

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) =>
{
    if (administradorServicos.Login(loginDTO) != null)
        return Results.Ok("Login efetuado com sucesso.");
    else
        return Results.Unauthorized();
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API v1");
    c.RoutePrefix = string.Empty; // Acessa o Swagger UI na raiz
});


app.Run();