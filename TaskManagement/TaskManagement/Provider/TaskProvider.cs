using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Provider;

public class TaskProvider(ITaskRepository taskRepository) : ITaskProvider
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    public async Task<Common.Models.Task> GetTaskById(int taskId)
    {
        try
        {
            var task = await _taskRepository.GetById(taskId);

            return task;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
