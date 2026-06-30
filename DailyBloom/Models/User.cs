namespace DailyBloom.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? ProfilePicturePath { get; set; }

    public List<TaskItem> Tasks { get; set; } = new();
    public List<IncomeEntry> Incomes { get; set; } = new();
    public List<ExpenseEntry> Expenses { get; set; } = new();
    public List<Book> Books { get; set; } = new();
}
