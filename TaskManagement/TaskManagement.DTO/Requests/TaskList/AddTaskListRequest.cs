namespace TaskManagement.DTO.Requests.TaskList;

public class AddTaskListRequest
{
    public int TaskListId { get; set; }
    public int? GroupId { get; set; }
    public string? ViewableToUserIds { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string CreatedByUserId { get; set; }
}
