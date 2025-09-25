using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Interfaces;

namespace minimal_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministratorsController : ControllerBase
    {
        private readonly IAdministratorServices _adminService;
        private readonly IConfiguration _configuration;

        public AdministratorsController(IAdministratorServices adminService, IConfiguration configuration)
        {
            _adminService = adminService;
            _configuration = configuration;
        }

        private string GenerateTokenJwt(Administrator administrator)
        {
            var key = _configuration.GetSection("Jwt").ToString();
            if (string.IsNullOrEmpty(key)) key = "keysecret";

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var claims = new List<System.Security.Claims.Claim>
            {
                new(System.Security.Claims.ClaimTypes.Email, administrator.Email),
                new(System.Security.Claims.ClaimTypes.Role, administrator.Profile)
            };

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var adm = _adminService.Login(loginDTO);
            if (adm == null) return Unauthorized();

            var token = GenerateTokenJwt(adm);

            return Ok(new AdministratorLogged
            {
                Email = adm.Email,
                Profile = adm.Profile,
                Token = token
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] AdministratorDTO dto)
        {
            var admin = new Administrator
            {
                Email = dto.Email,
                Password = dto.Password,
                Profile = dto.Profile.ToString()
            };

            _adminService.Include(admin);

            return Created($"/api/administrators/{admin.Id}", admin);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll([FromQuery] int? page)
        {
            var admins = _adminService.ListAdministrators(page ?? 1);
            if (!admins.Any()) return NotFound("Administrator not found.");

            return Ok(admins);
        }
    }
}
