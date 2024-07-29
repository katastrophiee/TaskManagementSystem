using TaskManagement.Common.Models;

namespace TaskManagement.DTO.Responses.Group;

public class GetGroupResponse
{
    public int GroupId { get; set; }
    public string? ViewableToUserIds { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }

    public ErrorResponse Error { get; set; }

    public GetGroupResponse(ErrorResponse error)
    {
        Error = error;
    }

    public GetGroupResponse(Common.Models.Group group)
    {
        GroupId = group.GroupId;
        ViewableToUserIds = group.ViewableToUserIds;
        Name = group.Name;
        Description = group.Description;
        CreatedByUserId = group.CreatedByUserId;
        CreatedOn = group.CreatedOn;
    }
}
