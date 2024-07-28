using TaskManagement.Common.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Interface.Repository;

public interface ITaskUserRepository
{
    Task<TaskUser?> GetById(int userId);

    Task<TaskUser?> GetByUsername(string username);

    Task Update(TaskUser user);

    Task<List<TaskUser>> GetListByUsernameOrEmail(string username, string email);

    Task Add(TaskUser user);
}
