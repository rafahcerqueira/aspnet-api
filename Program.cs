using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.DataBase;
using minimal_api.Infraestrutura.Interfaces;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServicos, AdministradorServicos>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServicos>();

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
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) =>
{
    if (administradorServicos.Login(loginDTO) != null)
        return Results.Ok("Login efetuado com sucesso.");
    else
        return Results.Unauthorized();
}).WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradoresDTO, IAdministradorServicos administradorServicos) =>
{
    var administrador = new Administrador
    {
        Email = administradoresDTO.Email,
        Senha = administradoresDTO.Senha,
        Perfil = administradoresDTO.Perfil.ToString()
    };

    if (administrador != null)
    {
        administradorServicos.Incluir(administrador);
        return Results.Created($"/administradores/{administrador.Id}", administrador);
    }
    else
    {
        return Results.BadRequest("Erro ao adicionar administrador.");
    }
}).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServicos administradorServicos) =>
{
    var administradores = administradorServicos.ListarAdministradores(pagina: pagina ?? 1);
    return administradores.Any() ? Results.Ok(administradores) : Results.NotFound("Nenhum administrador encontrado.");
}).WithTags("Administradores");
#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    if (veiculo != null)
    {
        veiculoServico.IncluirVeiculo(veiculo);
        return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
    }
    else
    {
        return Results.BadRequest("Erro ao adicionar veículo.");
    }
}).WithTags("Veiculos");;

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.ListarVeiculos(pagina: pagina);
    return veiculos.Any() ? Results.Ok(veiculos) : Results.NotFound("Nenhum veículo encontrado.");
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] string id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    return veiculo != null ? Results.Ok(veiculo) : Results.NotFound("Veículo não encontrado.");
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] string id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculoExistente = veiculoServico.BuscaPorId(id);
    if (veiculoExistente == null)
        return Results.NotFound("Veículo não encontrado.");

    veiculoExistente.Nome = veiculoDTO.Nome;
    veiculoExistente.Marca = veiculoDTO.Marca;
    veiculoExistente.Ano = veiculoDTO.Ano;
    veiculoServico.AtualizarVeiculo(veiculoExistente);

    return Results.Ok(veiculoExistente);
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] string id, IVeiculoServico veiculoServico) =>
{
    var veiculoExistente = veiculoServico.BuscaPorId(id);
    if (veiculoExistente == null)
        return Results.NotFound("Veículo não encontrado.");

    veiculoServico.ApagarVeiculo(id);
    return Results.Ok("Veículo excluído com sucesso.");
}).WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.Run();
#endregion