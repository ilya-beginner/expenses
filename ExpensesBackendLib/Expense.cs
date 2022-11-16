using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Expenses;

[Index(nameof(Date), nameof(Tag))]
public class Expense
{
    public int Id { get; set; }

    [JsonConverter(typeof(Expenses.DateOnlyJsonConverter))]
    public DateOnly Date { get; set; }

    public decimal Sum { get; set; }

    public Currency Currency { get; set; }

    public string? Tag { get; set; }

    public string? Notes { get; set; }

    public Expense() { }

    public Expense(ExpenseApi expense)
    {
        Expenses.Currency currency;
        if (!Expenses.Currency.TryParse(expense.Currency, out currency))
        {
            throw new System.ArgumentException("Invalid currency");
        }

        Id = expense.Id;
        Date = expense.Date;
        Sum = expense.Sum;
        Currency = currency;
        Tag = expense.Tag;
        Notes = expense.Notes;
    }
}
