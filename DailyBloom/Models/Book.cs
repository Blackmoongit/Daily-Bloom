namespace DailyBloom.Models;

public class Book
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public string Name { get; set; } = "";
    public string? CoverPath { get; set; }
    public string? Reflection { get; set; }
    public int Rating { get; set; } // 0-5 stars
    public bool IsRead { get; set; } // false = "Will Read", true = "Read"
    public DateTime AddedDate { get; set; } = DateTime.Now;
}
