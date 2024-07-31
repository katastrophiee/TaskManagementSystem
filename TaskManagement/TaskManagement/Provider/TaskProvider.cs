using TaskManagement.Common.Models;
using TaskManagement.DTO.Requests.Task;
using TaskManagement.DTO.Responses.Task;
using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;
using TaskManagement.Repository;
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

    public async Task<bool> UpdateTask(UpdateTaskRequest updateTaskRequest)
    {
        try
        {
            var task = await _taskRepository.GetById(updateTaskRequest.TaskId);
            if (task is null)
                return false;

            if (updateTaskRequest.Name is not null)
                task.Name = updateTaskRequest.Name;

            if (updateTaskRequest.Description is not null)
                task.Description = updateTaskRequest.Description;

            if (updateTaskRequest.TaskListId is not null)
                task.TaskListId = updateTaskRequest.TaskListId;

            if (updateTaskRequest.GroupId is not null)
                task.GroupId = updateTaskRequest.GroupId;

            task.TaskStatus = updateTaskRequest.TaskStatus;

            await _taskRepository.Update(task);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<List<GetTaskResponse>?> GetTasksByTaskListId(int taskListId)
    {
        try
        {
            var tasks = await _taskRepository.GetListByTaskListId(taskListId) ?? [];

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
                    Description = $"An unknown error occured when trying to get tasks for task list {taskListId}",
                    StatusCode = StatusCodes.Status404NotFound,
                    AdditionalDetails = ex.Message
                })
            ];
        }
    }

    public async Task<string?> GetTaskOwnerId(int taskId)
    {
        try
        {
            var task = await _taskRepository.GetById(taskId);
            if (task == null)
            {
                return null;
            }

            return task.CreatedByUserId;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}

