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
        configMock.Setup(config => config["Db:Host"]).Returns("localhost");
        configMock.Setup(config => config["Db:Port"]).Returns("3306");
        configMock.Setup(config => config["Db:User"]).Returns("root");
        configMock.Setup(config => config["Db:Password"]).Returns("password");
        configMock.Setup(config => config["Db:Name"]).Returns("db");
        configMock.Setup(config => config["Cors:AllowedOrigins"]).Returns("origin1,origin2");

        var resultConfig = new Expenses.Configuration(configMock.Object);

        Assert.Equal("localhost", resultConfig.DbHost);
        Assert.Equal(3306, resultConfig.DbPort);
        Assert.Equal("root", resultConfig.DbUser);
        Assert.Equal("password", resultConfig.DbPassword);
        Assert.Equal("db", resultConfig.DbName);
        Assert.Equal(new string[] { "origin1", "origin2" }, resultConfig.CorsAllowedOrigins);
    }
}
