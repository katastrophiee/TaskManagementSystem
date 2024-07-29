using TaskManagement.Common.Models;
using TaskManagement.DTO.Requests.Task;
using TaskManagement.DTO.Responses.Task;
using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Provider;

public class TaskProvider(ITaskRepository taskRepository) : ITaskProvider
{
    private readonly ITaskRepository _taskRepository = taskRepository;
    public async Task<GetTaskResponse> GetTaskById(int taskId)
    {
        try
        {
            var task = await _taskRepository.GetById(taskId);
            if (task is null)
            {
                return new(new ErrorResponse()
                {
                    Title = "Task not found",
                    Description = $"No task was found with the ID {taskId}",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }
            return new (task);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "An unknown error occured",
                Description = $"An unknown error occured when trying to get task {taskId}",
                StatusCode = StatusCodes.Status404NotFound,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<List<GetTaskResponse>?> GetTasksByUserId(string userId)
    {
        try
        {
            var tasks = await _taskRepository.GetListByUserUsername(userId) ?? [];

            var response = tasks.Select(t => new GetTaskResponse(t));

            return response.ToList();
        }
        catch (Exception ex)
        {
            return 
            [
                new(new ErrorResponse()
                {
                    Title = "An unknown error occured",
                    Description = $"An unknown error occured when trying to get tasks for this user",
                    StatusCode = StatusCodes.Status404NotFound,
                    AdditionalDetails = ex.Message
                })
            ];
        }
    }

    public async Task<bool> AddTask(AddTaskRequest addTaskRequest)
    {
        try
        {
            var task = new Task()
            {
                GroupId = addTaskRequest.GroupId,
                TaskListId = addTaskRequest.TaskListId,
                Name = addTaskRequest.Name,
                Description = addTaskRequest.Description,
                TaskStatus = Common.Enums.TaskStatus.New,
                CreatedByUserId = addTaskRequest.CreatedByUserId,
                CreatedOn = DateTime.Now
            };

            await _taskRepository.Add(task);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
