using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.DataBase;
using minimal_api.Infrastructure.Interfaces;

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

builder.Services.AddScoped<IAdministratorServices, AdministratorServices>();
builder.Services.AddScoped<IVehicleService, VehicleServices>();

// Adiciona Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Minimal API",
        Description = "Simple API with ASP.NET Core",
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insert Jwt"
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

builder.Services.AddDbContext<DatabaseContext>(options =>
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

#region Admins
string GenerateTokenJwt(Administrator Administrator)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;
    
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", Administrator.Email),
        new Claim("Profile", Administrator.Profile),
        new Claim(ClaimTypes.Role, Administrator.Profile)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/Administrators/login", ([FromBody] LoginDTO loginDTO, IAdministratorServices AdministratorServices) =>
{
    var adm = AdministratorServices.Login(loginDTO);
    if (adm != null)
    {
        string token = GenerateTokenJwt(adm);
        return Results.Ok(new AdministratorLogged
        {
            Email = adm.Email,
            Profile = adm.Profile,
            Token = token
        });
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous().WithTags("Administrators");

app.MapPost("/Administrators", ([FromBody] AdministratorDTO AdministratorsDTO, IAdministratorServices AdministratorServices) =>
{
    var Administrator = new Administrator
    {
        Email = AdministratorsDTO.Email,
        Password = AdministratorsDTO.Password,
        Profile = AdministratorsDTO.Profile.ToString()
    };

    if (Administrator != null)
    {
        AdministratorServices.Include(Administrator);
        return Results.Created($"/Administrators/{Administrator.Id}", Administrator);
    }
    else
    {
        return Results.BadRequest("Error adding Administrator.");
    }
}).WithTags("Administrators");

app.MapGet("/Administrators", ([FromQuery] int? page, IAdministratorServices AdministratorServices) =>
{
    var Administrators = AdministratorServices.ListAdministrators(page: page ?? 1);
    return Administrators.Any() ? Results.Ok(Administrators) : Results.NotFound("Administrator not found.");
}).RequireAuthorization().WithTags("Administrators");
#endregion

#region Vehicles
app.MapPost("/Vehicles", ([FromBody] VehiclesDTO vehiclesDTO, IVehicleService VehicleService) =>
{
    var Vehicle = new Vehicle
    {
        Name = vehiclesDTO.Name,
        Mark = vehiclesDTO.Mark,
        Year = vehiclesDTO.Year
    };

    if (Vehicle != null)
    {
        VehicleService.IncludeVehicle(Vehicle);
        return Results.Created($"/Vehicles/{Vehicle.Id}", Vehicle);
    }
    else
    {
        return Results.BadRequest("Error adding vehicle.");
    }
}).RequireAuthorization().WithTags("Vehicles");;

app.MapGet("/Vehicles", ([FromQuery] int? page, IVehicleService VehicleService) =>
{
    var Vehicles = VehicleService.ListVehicles(page: page);
    return Vehicles.Any() ? Results.Ok(Vehicles) : Results.NotFound("Vehicle not found.");
}).RequireAuthorization().WithTags("Vehicles");

app.MapGet("/Vehicles/{id}", ([FromRoute] string id, IVehicleService VehicleService) =>
{
    var Vehicle = VehicleService.GetById(id);
    return Vehicle != null ? Results.Ok(Vehicle) : Results.NotFound("Vehicle not found.");
}).RequireAuthorization().WithTags("Vehicles");

app.MapPut("/Vehicles/{id}", ([FromRoute] string id, [FromBody] VehiclesDTO VehiclesDTO, IVehicleService VehicleService) =>
{
    var ExistingVehicle = VehicleService.GetById(id);
    if (ExistingVehicle == null)
        return Results.NotFound("Vehicle not found.");

    ExistingVehicle.Name = VehiclesDTO.Name;
    ExistingVehicle.Mark = VehiclesDTO.Mark;
    ExistingVehicle.Year = VehiclesDTO.Year;
    VehicleService.UpdateVehicle(ExistingVehicle);

    return Results.Ok(ExistingVehicle);
}).RequireAuthorization().WithTags("Vehicles");

app.MapDelete("/Vehicles/{id}", ([FromRoute] string id, IVehicleService VehicleService) =>
{
    var ExistingVehicle = VehicleService.GetById(id);
    if (ExistingVehicle == null)
        return Results.NotFound("Vehicles not found.");

    VehicleService.DeleteVehicle(id);
    return Results.Ok("Vehicles successfully deleted.");
}).RequireAuthorization().WithTags("Vehicles");
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