using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = "server=database-expenses.cjdnjnrorbm5.eu-west-1.rds.amazonaws.com;user=admin;password=fanxio468;database=expenses";
var serverVersion = new MariaDbServerVersion(new Version(10, 6, 8));

builder.Services.AddDbContext<ExpenseDb>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://ec2-54-246-69-31.eu-west-1.compute.amazonaws.com:8080")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                      });
});
var app = builder.Build();

app.MapPost("/expense", async (Expense expense, ExpenseDb db) =>
{
    db.expenses.Add(expense);
    await db.SaveChangesAsync();

    return Results.Created($"/expense/{expense.id}", expense);
}).RequireCors(MyAllowSpecificOrigins);

app.UseCors(MyAllowSpecificOrigins);

app.Run();

class Expense
{
    public int id { get; set; }
    [JsonConverter(typeof(SystemTextJsonSamples.DateOnlyJsonConverter))]
    public DateOnly date { get; set; }
    public float sum { get; set; }
    public string? tag { get; set; }
    public string? notes { get; set; }
}

class ExpenseDb : DbContext
{
    public ExpenseDb(DbContextOptions<ExpenseDb> options)
        : base(options) { }

    public DbSet<Expense> expenses => Set<Expense>();
}
