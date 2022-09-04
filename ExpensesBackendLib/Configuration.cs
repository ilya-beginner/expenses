using Microsoft.Extensions.Configuration;

namespace Expenses;

public class Configuration
{
    public string DbHost;
    public ushort DbPort;
    public string DbUser;
    public string DbPassword;
    public string DbName;

    public string[] AllowedOrigins;

    public Configuration(IConfiguration config)
    {
        DbHost = config["Expenses:Database:DbHost"];
        DbPort = ushort.Parse(config["Expenses:Database:DbPort"]);
        DbUser = config["Expenses:Database:DbUser"];
        DbPassword = config["Expenses:Database:DbPassword"];
        DbName = config["Expenses:Database:DbName"];

        AllowedOrigins = config["Expenses:Cors:AllowedOrigins"].Split(',');
    }
}
