using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.DataBase;

namespace MSTest.Domain.Services;

[TestClass]
public class AdministratorServiceTest
{
    [TestMethod]
    public void TestingSaveAdministrator()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators");
        var adm = new Administrator();
        adm.Email = "Administrator@gmail.com";
        adm.Password = "123";
        adm.Profile = "Adm";
        var administratorService = new AdministratorServices(context);

        // Act
        administratorService.Include(adm);

        // Assert
        Assert.AreEqual(1, administratorService.ListAdministrators(1).Count());
        Assert.AreEqual("Administrator@gmail.com", adm.Email);
        Assert.AreEqual("123", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }

    private DatabaseContext CreateContextTest()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DatabaseContext(configuration);
    }
}
