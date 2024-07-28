using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;
using TaskManagement.Common.Models;
using TaskManagement.Interface.Repository;
using TaskManagement.Repository.DBContext;

namespace TaskManagement.Repository;

public class TaskUserRepository(DatabaseContext dbContext) : ITaskUserRepository
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task<TaskUser?> GetById(int userId) => await _dbContext.TaskUser.FirstOrDefaultAsync(u => u.TaskUserId == userId);

    public async Task<TaskUser?> GetByUsername(string username) => await _dbContext.TaskUser.FirstOrDefaultAsync(u => u.Username == username);

    public async Task Update(TaskUser user)
    {
        _dbContext.TaskUser.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<TaskUser>> GetListByUsernameOrEmail(string username, string email) => await _dbContext.TaskUser.Where(u => u.Username == username || u.Email == email).ToListAsync();

    public async Task Add(TaskUser user)
    {
        _dbContext.TaskUser.Add(user);
        await _dbContext.SaveChangesAsync();
    }
}
