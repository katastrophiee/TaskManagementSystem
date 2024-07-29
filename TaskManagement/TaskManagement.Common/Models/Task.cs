namespace TaskManagement.Common.Models;

public sealed class Task
{
    public int TaskId { get; set; }
    public int? GroupId { get; set; }
    public int? TaskListId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Enums.TaskStatus TaskStatus { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }
}
