using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.DTO;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.DataBase;
using minimal_api.Infraestrutura.Interfaces;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "keysecret";

builder.Services.AddAuthentication(option =>
{
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServicos, AdministradorServicos>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServicos>();

// Adiciona Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Minimal API",
        Description = "Uma API minimalista com ASP.NET Core",
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Inserir Jwt"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

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
string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;
    
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) =>
{
    var adm = administradorServicos.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);
        return Results.Ok(new AdministradorLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Administradores");

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
}).RequireAuthorization().WithTags("Administradores");
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
}).RequireAuthorization().WithTags("Veiculos");;

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.ListarVeiculos(pagina: pagina);
    return veiculos.Any() ? Results.Ok(veiculos) : Results.NotFound("Nenhum veículo encontrado.");
}).RequireAuthorization().WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] string id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    return veiculo != null ? Results.Ok(veiculo) : Results.NotFound("Veículo não encontrado.");
}).RequireAuthorization().WithTags("Veiculos");

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
}).RequireAuthorization().WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] string id, IVeiculoServico veiculoServico) =>
{
    var veiculoExistente = veiculoServico.BuscaPorId(id);
    if (veiculoExistente == null)
        return Results.NotFound("Veículo não encontrado.");

    veiculoServico.ApagarVeiculo(id);
    return Results.Ok("Veículo excluído com sucesso.");
}).RequireAuthorization().WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion