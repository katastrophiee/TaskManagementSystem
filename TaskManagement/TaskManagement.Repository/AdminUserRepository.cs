using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;
using TaskManagement.Interface.Repository;
using TaskManagement.Repository.DBContext;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Repository;

public class AdminUserRepository(DatabaseContext dbContext) : IAdminUserRepository
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task<AdminUser?> GetById(int adminId) => await _dbContext.AdminUser.FirstOrDefaultAsync(a => a.AdminUserId == adminId);

    public async Task<AdminUser?> GetByUsername(string username) => await _dbContext.AdminUser.FirstOrDefaultAsync(a => a.Username == username);

    public async Task Update(AdminUser admin)
    {
        _dbContext.AdminUser.Update(admin);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<AdminUser>> GetListByUsernameOrEmail(string username, string email) => await _dbContext.AdminUser.Where(u => u.Username == username || u.Email == email).ToListAsync();

    public async Task Add(AdminUser user)
    {
        _dbContext.AdminUser.Add(user);
        await _dbContext.SaveChangesAsync();
    }
}
