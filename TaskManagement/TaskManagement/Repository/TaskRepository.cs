using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Repository;

public class TaskRepository(ApplicationDbContext dbContext) : ITaskRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Common.Models.Task?> GetById(int taskId) => await _dbContext.Task.FirstOrDefaultAsync(u => u.TaskId == taskId);

    public async Task<List<Common.Models.Task>> GetListByUserUsername(string username) => await _dbContext.Task.Where(t => t.CreatedByUserId == username).ToListAsync();

    public async Task Update(Common.Models.Task user)
    {
        _dbContext.Task.Update(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Add(Common.Models.Task task)
    {
        _dbContext.Task.Add(task);
        await _dbContext.SaveChangesAsync();
    }
}
