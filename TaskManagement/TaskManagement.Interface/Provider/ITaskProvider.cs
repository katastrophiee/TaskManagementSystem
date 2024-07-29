using TaskManagement.DTO.Responses.Task;

namespace TaskManagement.Interface.Provider;

public interface ITaskProvider
{
    Task<GetTaskResponse?> GetTaskById(int taskId);

    Task<List<GetTaskResponse>?> GetTasksByUserId(string userId);

}
