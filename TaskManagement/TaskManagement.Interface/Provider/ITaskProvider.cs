using TaskManagement.DTO.Requests.Task;
using TaskManagement.DTO.Responses.Task;

namespace TaskManagement.Interface.Provider;

public interface ITaskProvider
{
    Task<GetTaskResponse?> GetTaskById(int taskId);

    Task<List<GetTaskResponse>?> GetTasksByUserId(string userId);

    Task<bool> AddTask(AddTaskRequest addTaskRequest);

    Task<bool> UpdateTask(UpdateTaskRequest updateTaskRequest);

    Task<List<GetTaskResponse>?> GetTasksByTaskListId(int taskListId);

    Task<string?> GetTaskOwnerId(int taskId);

    Task<List<GetTaskResponse>?> GetUnlistedTasksByGroupId(int groupId);
}
