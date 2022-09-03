using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    // config.Sources.Clear();

    // For dev purposes
    config.AddIniFile("config.ini", optional: true, reloadOnChange: false);

    config.AddEnvironmentVariables();
});

var config = new Expenses.Configuration(builder);

var connectionString = @$"server={config.DbHost};
                          port={config.DbPort};
                          user={config.DbUser};
                          password={config.DbPassword};
                          database={config.DbName}";
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
                          policy.WithOrigins(config.AllowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                      });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ExpenseDb>();
    context.Database.Migrate();
}

app.MapPost("/expenses", async (Expense expense, ExpenseDb db) =>
{
    db.expenses.Add(expense);
    await db.SaveChangesAsync();

    return Results.Created($"/expenses/{expense.Id}", expense);
}).RequireCors(MyAllowSpecificOrigins);

app.MapGet("/expenses", async (DateOnly from, DateOnly to, ExpenseDb db) =>
    await db.expenses.Where(expense => expense.Date >= from && expense.Date <= to).ToListAsync()
).RequireCors(MyAllowSpecificOrigins);

app.MapMethods("/expenses/{id}", new[] { "PATCH" }, async (int id, ExpenseDTO inputExpense, ExpenseDb db) =>
{
    var expense = await db.expenses.FindAsync(id);

    if (expense is null) return Results.NotFound();

    expense.Date = inputExpense.Date ?? expense.Date;
    expense.Sum = inputExpense.Sum ?? expense.Sum;
    expense.Tag = inputExpense.Tag ?? expense.Tag;
    expense.Notes = inputExpense.Notes ?? expense.Notes;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/expenses/{id}", async (int id, ExpenseDb db) =>
{
    if (await db.expenses.FindAsync(id) is Expense expense)
    {
        db.expenses.Remove(expense);
        await db.SaveChangesAsync();
        return Results.Ok(expense);
    }

    return Results.NotFound();
});

app.UseCors(MyAllowSpecificOrigins);

app.Run();

[Index(nameof(Date), nameof(Tag))]
class Expense
{
    public int Id { get; set; }

    [JsonConverter(typeof(SystemTextJsonSamples.DateOnlyJsonConverter))]
    public DateOnly Date { get; set; }

    public decimal Sum { get; set; }

    public string? Tag { get; set; }

    public string? Notes { get; set; }
}

class ExpenseDTO
{
    public int Id { get; set; }

    [JsonConverter(typeof(SystemTextJsonSamples.DateOnlyJsonConverter))]
    public DateOnly? Date { get; set; }

    public decimal? Sum { get; set; }

    public string? Tag { get; set; }

    public string? Notes { get; set; }

    public ExpenseDTO() { }

    public ExpenseDTO(Expense expense) =>
    (Id, Date, Sum, Tag, Notes) = (expense.Id, expense.Date, expense.Sum, expense.Tag, expense.Notes);
}

class ExpenseDb : DbContext
{
    public ExpenseDb(DbContextOptions<ExpenseDb> options)
        : base(options) { }

    public DbSet<Expense> expenses => Set<Expense>();
}
