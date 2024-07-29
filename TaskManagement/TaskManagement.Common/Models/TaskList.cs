namespace TaskManagement.Common.Models;

public sealed class TaskList
{
    public int TaskListId { get; set; }
    public int? GroupId { get; set; }
    public string? ViewableToUserIds { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }
}
