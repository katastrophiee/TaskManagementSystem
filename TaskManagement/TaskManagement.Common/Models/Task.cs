namespace TaskManagement.Common.Models;

public sealed class Task
{
    public int TaskId { get; set; }
    public int? GroupId { get; set; }
    public int? TaskListId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }
}
