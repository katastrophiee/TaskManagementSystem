using TaskManagement.DTO.Responses.Group;

namespace TaskManagement.DTO.Requests.Group;

public class UpdateGroupRequest
{
    public int GroupId { get; set; }
    public string? ViewableToUserIds { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public UpdateGroupRequest()
    {
    }

    public UpdateGroupRequest(GetGroupResponse group)
    {
        GroupId = group.GroupId;
        ViewableToUserIds = group.ViewableToUserIds;
        Name = group.Name;
        Description = group.Description;
    }
}
