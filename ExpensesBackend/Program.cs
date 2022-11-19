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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<Expenses.ExpensesDb>();
    context.Database.Migrate();
}

app.MapPost("/expenses", async (Expenses.ExpenseApi expense, Expenses.ExpensesDb db) =>
{
    if (expense.Sum == 0.0m)
    {
        throw new System.ArgumentException("Sum cannot be zero");
    }

    db.Expenses.Add(new Expenses.Expense(expense));
    await db.SaveChangesAsync();

    return Results.Created($"/expenses/{expense.Id}", expense); // bug: returns id = 0
}).RequireCors(MyAllowSpecificOrigins);

app.MapGet("/expenses", async (DateOnly from, DateOnly to, Expenses.ExpensesDb db) =>
{
    var x = (await db.Expenses.
        Where(expense => expense.Date >= from && expense.Date <= to).
        OrderByDescending(expense => expense.Date).ThenByDescending(expense => expense.Id).
        ToListAsync()).Select(expense => new Expenses.ExpenseApi(expense)).ToList();
    return x;
}).RequireCors(MyAllowSpecificOrigins);

app.MapMethods("/expenses/{id}", new[] { "PATCH" }, async (int id, Expenses.ExpensePatch inputExpense, Expenses.ExpensesDb db) =>
{
    var expense = await db.Expenses.FindAsync(id);

    if (expense is null) return Results.NotFound();

    expense.Date = inputExpense.Date ?? expense.Date;
    expense.Sum = inputExpense.Sum ?? expense.Sum;

    var currency = expense.Currency;
    if (inputExpense.Currency is not null)
    {
        if (!Expenses.Currency.TryParse(inputExpense.Currency, out currency))
        {
            throw new System.ArgumentException("Invalid currency");
        }
    }

    expense.Currency = currency;
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
