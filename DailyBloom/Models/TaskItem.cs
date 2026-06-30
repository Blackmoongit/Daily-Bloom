namespace DailyBloom.Models;

public enum Priority { Low, Medium, High }

public class TaskItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime EndDate { get; set; } = DateTime.Today;
    public string? Hashtags { get; set; }      // comma separated, e.g. "#work,#urgent"
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public List<SubTask> SubTasks { get; set; } = new();
    public List<TaskComment> Comments { get; set; } = new();
    public List<TaskAttachment> Attachments { get; set; } = new();
}

public class SubTask
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public string Title { get; set; } = "";
    public bool IsCompleted { get; set; }
}

public class TaskComment
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public string Text { get; set; } = "";
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}

public class TaskAttachment
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }
    public string FileName { get; set; } = "";
    public string FilePath { get; set; } = "";
}
