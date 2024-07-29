using TaskManagement.Common.Models;

namespace TaskManagement.Interface.Repository;

public interface ITaskListRepository
{
    Task<List<TaskList>?> GetOwnedOrJoinedTaskLists(string userId);
}
