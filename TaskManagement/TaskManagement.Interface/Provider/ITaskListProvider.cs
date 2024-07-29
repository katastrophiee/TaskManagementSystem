using TaskManagement.DTO.Responses.TaskList;

namespace TaskManagement.Interface.Provider;

public interface ITaskListProvider
{
    Task<List<GetTaskListResponse>> GetOwnedOrJoinedTaskLists(string userId);
}
