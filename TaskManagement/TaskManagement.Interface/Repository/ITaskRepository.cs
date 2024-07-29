namespace TaskManagement.Interface.Repository;

public interface ITaskRepository
{
    Task<Common.Models.Task?> GetById(int taskId);

    Task<List<Common.Models.Task>> GetListByUserUsername(string username);

    Task Add(Common.Models.Task task);

}
