using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrastructure.DataBase;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;
    public DatabaseContext(IConfiguration configuracaoAppSettings) => _configuracaoAppSettings = configuracaoAppSettings;

    
    public DbSet<Administrator> Administrators { get; set; } = default!;
    public DbSet<Vehicle> Vehicles { get; set; } = default!;
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator
            {
                Id = "1",
                Email = "Administrator@teste.com",
                Password = "123",
                Profile = "Adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("DefaultConnection")?.ToString();

            if (!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql(
                    stringConexao,
                    ServerVersion.AutoDetect(stringConexao)
                );
            }
        }
    }
}