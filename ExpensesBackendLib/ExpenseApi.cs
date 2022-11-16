using System.Text.Json.Serialization;

namespace Expenses;

public class ExpenseApi
{
    public int Id { get; set; }

    [JsonConverter(typeof(Expenses.DateOnlyJsonConverter))]
    public DateOnly Date { get; set; }

    public decimal Sum { get; set; }

    public string Currency { get; set; }

    public string? Tag { get; set; }

    public string? Notes { get; set; }

    public ExpenseApi() { }

    public ExpenseApi(Expense expense) =>
        (Id, Date, Sum, Currency, Tag, Notes) =
            (expense.Id, expense.Date, expense.Sum, expense.Currency.ToString(), expense.Tag, expense.Notes);
}
