using Microsoft.EntityFrameworkCore;

namespace Expenses;

public class ExpensesDb : DbContext
{
    public ExpensesDb(DbContextOptions<ExpensesDb> options)
        : base(options) { }

    public DbSet<Expense> Expenses => Set<Expense>();
}
