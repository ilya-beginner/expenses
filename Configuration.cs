namespace Expenses;

public class Configuration
{
    public string DbHost;
    public ushort DbPort;
    public string DbUser;
    public string DbPassword;
    public string DbName;

    public string[] AllowedOrigins;

    public Configuration(WebApplicationBuilder builder)
    {
        DbHost = builder.Configuration["Expenses:Database:DbHost"];
        DbPort = ushort.Parse(builder.Configuration["Expenses:Database:DbPort"]);
        DbUser = builder.Configuration["Expenses:Database:DbUser"];
        DbPassword = builder.Configuration["Expenses:Database:DbPassword"];
        DbName = builder.Configuration["Expenses:Database:DbName"];

        AllowedOrigins = builder.Configuration["Expenses:Cors:AllowedOrigins"].Split(',');
    }
}
