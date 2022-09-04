using System.Text.Json.Serialization;

namespace Expenses;

public class ExpenseDTO
{
    public int Id { get; set; }

    [JsonConverter(typeof(Expenses.DateOnlyJsonConverter))]
    public DateOnly? Date { get; set; }

    public decimal? Sum { get; set; }

    public string? Tag { get; set; }

    public string? Notes { get; set; }

    public ExpenseDTO() { }

    public ExpenseDTO(Expense expense) =>
    (Id, Date, Sum, Tag, Notes) = (expense.Id, expense.Date, expense.Sum, expense.Tag, expense.Notes);
}
