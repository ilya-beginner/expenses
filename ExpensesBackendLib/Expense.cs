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

    public string? Tag { get; set; }

    public string? Notes { get; set; }
}
