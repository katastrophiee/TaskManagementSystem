namespace TaskManagement.Interface.Repository;

public interface ITaskRepository
{
    Task<Common.Models.Task?> GetById(int taskId);

    Task<List<Common.Models.Task>> GetListByUserUsername(string username);

    Task Add(Common.Models.Task task);

    Task Update(Common.Models.Task task);

    Task<List<Common.Models.Task>> GetListByTaskListId(int taskListId);

    Task<List<Common.Models.Task>> GetListByGroupId(int groupId);

}
