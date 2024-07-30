namespace TaskManagement.DTO.Requests.Group;

public class AddGroupRequest
{
    public string? ViewableToUserIds { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }
}
