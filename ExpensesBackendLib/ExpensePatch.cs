using System.Text.Json.Serialization;

namespace Expenses;

public class ExpensePatch
{
    public int Id { get; set; }

    [JsonConverter(typeof(Expenses.DateOnlyJsonConverter))]
    public DateOnly? Date { get; set; }

    public decimal? Sum { get; set; }

    public string? Currency { get; set; }

    public string? Tag { get; set; }

    public string? Notes { get; set; }

    // public ExpensePatch() { }

    // public ExpensePatch(Expense expense) =>
    //     (Id, Date, Sum, Currency, Tag, Notes) =
    //         (expense.Id, expense.Date, expense.Sum, expense.Currency, expense.Tag, expense.Notes);
}
