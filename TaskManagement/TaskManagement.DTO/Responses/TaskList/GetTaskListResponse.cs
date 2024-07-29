using TaskManagement.Common.Models;

namespace TaskManagement.DTO.Responses.TaskList;

public class GetTaskListResponse
{
    public int TaskListId { get; set; }
    public int? GroupId { get; set; }
    public string? ViewableToUserIds { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }
    public ErrorResponse Error { get; set; }

    public GetTaskListResponse(ErrorResponse error)
    {
        Error = error;
    }

    public GetTaskListResponse(Common.Models.TaskList taskList)
    {
        TaskListId = taskList.TaskListId;
        GroupId = taskList.GroupId;
        ViewableToUserIds = taskList.ViewableToUserIds;
        Name = taskList.Name;
        Description = taskList.Description;
        CreatedByUserId = taskList.CreatedByUserId;
        CreatedOn = taskList.CreatedOn;
    }
}
