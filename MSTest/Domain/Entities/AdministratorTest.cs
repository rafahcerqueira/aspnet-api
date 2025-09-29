using minimal_api.Domain.Entities;

namespace MSTest.Domain.Entidades;

[TestClass]
public class AdministratorTest
{
    [TestMethod]
    public void TestingGetAndSet()
    {
        // Arrange
        var adm = new Administrator();

        // Act
        adm.Id = "1";
        adm.Email = "Administrator@gmail.com";
        adm.Password = "123";
        adm.Profile = "Adm";

        // Assert
        Assert.AreEqual("1", adm.Id);
        Assert.AreEqual("Administrator@gmail.com", adm.Email);
        Assert.AreEqual("123", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }
}
