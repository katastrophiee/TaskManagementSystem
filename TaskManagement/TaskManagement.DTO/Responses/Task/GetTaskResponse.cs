using TaskManagement.Common.Models;

namespace TaskManagement.DTO.Responses.Task;

public class GetTaskResponse
{
    public int TaskId { get; set; }
    public int? GroupId { get; set; }
    public int? TaskListId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Common.Enums.TaskStatus TaskStatus { get; set; }
    public string CreatedByUserId { get; set; }
    public DateTime CreatedOn { get; set; }

    public ErrorResponse? Error { get; set; }

    public GetTaskResponse(ErrorResponse error)
    {
        Error = error;
    }

    public GetTaskResponse(Common.Models.Task task)
    {
        TaskId = task.TaskId;
        GroupId = task.GroupId;
        TaskListId = task.TaskListId;
        Name = task.Name;
        Description = task.Description;
        TaskStatus = task.TaskStatus;
        CreatedByUserId = task.CreatedByUserId;
        CreatedOn = task.CreatedOn;
    }
}
