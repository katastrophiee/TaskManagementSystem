using TaskManagement.DTO.Requests.TaskList;
using TaskManagement.DTO.Responses.TaskList;

namespace TaskManagement.Interface.Provider;

public interface ITaskListProvider
{
    Task<List<GetTaskListResponse>> GetOwnedOrJoinedTaskLists(string userId, string userEmail);

    Task<bool> AddTaskList(AddTaskListRequest request);

    Task<GetTaskListResponse> GetTaskListById(int taskListId);

    Task<bool> UpdateTaskList(UpdateTaskListRequest request);

    Task<List<GetTaskListResponse>?> GetTaskListsByGroupId(int groupId);

    Task<string?> GetTaskListOwnerId(int taskListId);
}
