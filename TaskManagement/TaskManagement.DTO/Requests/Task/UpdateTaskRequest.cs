using TaskManagement.DTO.Responses.Task;

namespace TaskManagement.DTO.Requests.Task;

public class UpdateTaskRequest
{
    public int TaskId { get; set; }
    public int? GroupId { get; set; }
    public int? TaskListId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Common.Enums.TaskStatus TaskStatus { get; set; }

    public UpdateTaskRequest()
    {
    }

    public UpdateTaskRequest(GetTaskResponse task)
    {
        TaskId = task.TaskId;
        GroupId = task.GroupId;
        TaskListId = task.TaskListId;
        Name = task.Name;
        Description = task.Description;
        TaskStatus = task.TaskStatus;
    }
}
