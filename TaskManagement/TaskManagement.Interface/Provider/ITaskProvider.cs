namespace TaskManagement.Interface.Provider;

public interface ITaskProvider
{
    Task<Common.Models.Task> GetTaskById(int taskId);
}
