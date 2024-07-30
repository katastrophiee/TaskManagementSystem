using TaskManagement.Common.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Interface.Repository;

public interface ITaskListRepository
{
    Task<List<TaskList>?> GetOwnedOrJoinedTaskLists(string userId, string userEmail);

    Task Add(TaskList task);

    Task<TaskList?> GetById(int taskListId);

    Task<bool> Update(TaskList task);

    Task<List<TaskList>?> GetListByGroupId(int groupId);

}
