using TaskManagement.Common.Models;
using TaskManagement.DTO.Responses.TaskList;
using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Provider;

public class TaskListProvider(ITaskListRepository taskListRepository) : ITaskListProvider
{
    private readonly ITaskListRepository _taskListRepository = taskListRepository;

    public async Task<List<GetTaskListResponse>> GetOwnedOrJoinedTaskLists(string userId)
    {

        try
        {
            var taskLists = await _taskListRepository.GetOwnedOrJoinedTaskLists(userId) ?? [];

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
}
