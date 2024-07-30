using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;
using TaskManagement.Data;
using TaskManagement.Interface.Repository;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Repository;

public class TaskListRepository(ApplicationDbContext dbContext) : ITaskListRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<TaskList>?> GetOwnedOrJoinedTaskLists(string userId, string userEmail)
    {
        return await _dbContext.TaskList.Where(g => g.CreatedByUserId == userId || (g.ViewableToUserIds ?? "").Contains(userEmail)).ToListAsync();
    }

    public async Task Add(TaskList taskList)
    {
        _dbContext.TaskList.Add(taskList);
        await _dbContext.SaveChangesAsync();
    }
}
