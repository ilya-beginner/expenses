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
    config.AddIniFile("config.ini", optional: true, reloadOnChange: false);
});

var config = new Expenses.Configuration(builder.Configuration);

var connectionString = @$"server={config.DbHost};
                          port={config.DbPort};
                          user={config.DbUser};
                          password={config.DbPassword};
                          database={config.DbName}";
var serverVersion = new MariaDbServerVersion(new Version(10, 6, 8));

builder.Services.AddDbContext<Expenses.ExpensesDb>(
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
                          policy.WithOrigins(config.CorsAllowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                      });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<Expenses.ExpensesDb>();
    context.Database.Migrate();
}

app.MapPost("/expenses", async (Expenses.Expense expense, Expenses.ExpensesDb db) =>
{
    db.Expenses.Add(expense);
    await db.SaveChangesAsync();

    return Results.Created($"/expenses/{expense.Id}", expense);
}).RequireCors(MyAllowSpecificOrigins);

app.MapGet("/expenses", async (DateOnly from, DateOnly to, Expenses.ExpensesDb db) =>
    await db.Expenses.Where(expense => expense.Date >= from && expense.Date <= to).ToListAsync()
).RequireCors(MyAllowSpecificOrigins);

app.MapMethods("/expenses/{id}", new[] { "PATCH" }, async (int id, Expenses.ExpenseDTO inputExpense, Expenses.ExpensesDb db) =>
{
    var expense = await db.Expenses.FindAsync(id);

    if (expense is null) return Results.NotFound();

    expense.Date = inputExpense.Date ?? expense.Date;
    expense.Sum = inputExpense.Sum ?? expense.Sum;
    expense.Tag = inputExpense.Tag ?? expense.Tag;
    expense.Notes = inputExpense.Notes ?? expense.Notes;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/expenses/{id}", async (int id, Expenses.ExpensesDb db) =>
{
    if (await db.Expenses.FindAsync(id) is Expenses.Expense expense)
    {
        db.Expenses.Remove(expense);
        await db.SaveChangesAsync();
        return Results.Ok(expense);
    }

    return Results.NotFound();
});

app.UseCors(MyAllowSpecificOrigins);

app.Run();
