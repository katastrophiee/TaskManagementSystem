using TaskManagement.Common.Models;
using TaskManagement.DTO.Requests.TaskList;
using TaskManagement.DTO.Responses.TaskList;
using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;
using TaskManagement.Repository;

namespace TaskManagement.Provider;

public class TaskListProvider(ITaskListRepository taskListRepository) : ITaskListProvider
{
    private readonly ITaskListRepository _taskListRepository = taskListRepository;

    public async Task<List<GetTaskListResponse>> GetOwnedOrJoinedTaskLists(string userId, string userEmail)
    {

        try
        {
            var taskLists = await _taskListRepository.GetOwnedOrJoinedTaskLists(userId, userEmail) ?? [];

            var response = taskLists.Select(t => new GetTaskListResponse(t));

            return response.ToList();
        }
        catch (Exception ex)
        {
            return
            [
                new(new ErrorResponse()
                {
                    Title = "An unknown error occured",
                    Description = $"An unknown error occured when trying to get task lists",
                    StatusCode = StatusCodes.Status404NotFound,
                    AdditionalDetails = ex.Message
                })
            ];
        }
    }

    public async Task<bool> AddTaskList(AddTaskListRequest request)
    {

        try
        {
            var taskList = new TaskList()
            {
                GroupId = request.GroupId,
                ViewableToUserIds = request.ViewableToUserIds,
                Name = request.Name,
                Description = request.Description,
                CreatedByUserId = request.CreatedByUserId,
                CreatedOn = DateTime.Now
            };

            await _taskListRepository.Add(taskList);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<GetTaskListResponse> GetTaskListById(int taskListId)
    {

        try
        {
            var taskList = await _taskListRepository.GetById(taskListId);

            if (taskList is null)
            {
                return new(new ErrorResponse()
                {
                    Title = "Task list not found",
                    Description = $"No task list was found with the ID {taskListId}",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            return new(taskList);
        }
        catch (Exception ex)
        {
            return new(new ErrorResponse()
            {
                Title = "An unknown error occured",
                Description = $"An unknown error occured when trying to get task list {taskListId}",
                StatusCode = StatusCodes.Status404NotFound,
                AdditionalDetails = ex.Message
            });
        }
    }

    public async Task<bool> UpdateTaskList(UpdateTaskListRequest updateTaskRequest)
    {
        try
        {
            var taskList = await _taskListRepository.GetById(updateTaskRequest.TaskListId);
            if (taskList is null)
                return false;

            if (updateTaskRequest.Name is not null)
                taskList.Name = updateTaskRequest.Name;

            if (updateTaskRequest.Description is not null)
                taskList.Description = updateTaskRequest.Description;

            if (updateTaskRequest.ViewableToUserIds is not null)
                taskList.ViewableToUserIds = updateTaskRequest.ViewableToUserIds;

            if (updateTaskRequest.GroupId is not null)
                taskList.GroupId = updateTaskRequest.GroupId;

            await _taskListRepository.Update(taskList);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<List<GetTaskListResponse>?> GetTaskListsByGroupId(int groupId)
    {
        try
        {
            var taskLists = await _taskListRepository.GetListByGroupId(groupId) ?? [];

            var response = taskLists.Select(t => new GetTaskListResponse(t));

            return response.ToList();
        }
        catch (Exception ex)
        {
            return
            [
                new(new ErrorResponse()
                {
                    Title = "An unknown error occured",
                    Description = $"An unknown error occured when trying to get task lists in group {groupId}",
                    StatusCode = StatusCodes.Status404NotFound,
                    AdditionalDetails = ex.Message
                })
            ];
        }
    }

    public async Task<string?> GetTaskListOwnerId(int taskListId)
    {
        try
        {
            var taskList = await _taskListRepository.GetById(taskListId);
            if (taskList == null)
            {
                return null;
            }

            return taskList.CreatedByUserId;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
