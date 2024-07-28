using TaskManagement.Common.Models;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Interface.Repository;

public interface IAdminUserRepository
{
    Task<AdminUser?> GetById(int adminId);

    Task<AdminUser?> GetByUsername(string username);

    Task Update(AdminUser admin);

    Task<List<AdminUser>> GetListByUsernameOrEmail(string username, string email);

    Task Add(AdminUser user);
}
