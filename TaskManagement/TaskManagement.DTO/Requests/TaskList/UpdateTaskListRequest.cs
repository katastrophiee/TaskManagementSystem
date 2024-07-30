using TaskManagement.DTO.Responses.TaskList;

namespace TaskManagement.DTO.Requests.TaskList;

public class UpdateTaskListRequest
{
    public int TaskListId { get; set; }
    public int? GroupId { get; set; }
    public string? ViewableToUserIds { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public UpdateTaskListRequest()
    {
    }

    public UpdateTaskListRequest(GetTaskListResponse taskList)
    {
        TaskListId = taskList.TaskListId;
        GroupId = taskList.GroupId;
        ViewableToUserIds = taskList.ViewableToUserIds;
        Name = taskList.Name;
        Description = taskList.Description;
    }
}
