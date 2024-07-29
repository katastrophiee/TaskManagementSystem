namespace TaskManagement.Common.Models;

public sealed class Group
{
    public int GroupId { get; set; }
    public string? ViewableToUserIds { get; set; }
    public string Name { get; set; } 
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }
}
