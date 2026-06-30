namespace DailyBloom.Models;

public class IncomeEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public string? Comments { get; set; }
}

public class ExpenseEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public string? Comments { get; set; }
}
