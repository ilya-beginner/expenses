using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;

namespace ExpensesBackendLibTest;

public class ConfigurationTest
{
    [Fact]
    public void SuccessTest()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(config => config["Expenses:Database:DbHost"]).Returns("localhost");
        configMock.Setup(config => config["Expenses:Database:DbPort"]).Returns("3306");
        configMock.Setup(config => config["Expenses:Database:DbUser"]).Returns("root");
        configMock.Setup(config => config["Expenses:Database:DbPassword"]).Returns("password");
        configMock.Setup(config => config["Expenses:Database:DbName"]).Returns("db");
        configMock.Setup(config => config["Expenses:Cors:AllowedOrigins"]).Returns("origin1,origin2");

        var resultConfig = new Expenses.Configuration(configMock.Object);

        Assert.Equal("localhost", resultConfig.DbHost);
        Assert.Equal(3306, resultConfig.DbPort);
        Assert.Equal("root", resultConfig.DbUser);
        Assert.Equal("password", resultConfig.DbPassword);
        Assert.Equal("db", resultConfig.DbName);
        Assert.Equal(new string[] { "origin1", "origin2" }, resultConfig.AllowedOrigins);
    }
}
