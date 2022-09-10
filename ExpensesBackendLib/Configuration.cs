using Microsoft.Extensions.Configuration;

namespace Expenses;

public class Configuration
{
    public string DbHost;
    public ushort DbPort;
    public string DbUser;
    public string DbPassword;
    public string DbName;

    public string[] CorsAllowedOrigins;

    public Configuration(IConfiguration config)
    {
        DbHost = config["Db:Host"];
        DbPort = ushort.Parse(config["Db:Port"]);
        DbUser = config["Db:User"];
        DbPassword = config["Db:Password"];
        DbName = config["Db:Name"];

        CorsAllowedOrigins = config["Cors:AllowedOrigins"].Split(',');
    }
}
