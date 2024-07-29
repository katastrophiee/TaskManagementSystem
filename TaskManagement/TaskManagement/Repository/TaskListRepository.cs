using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;
using TaskManagement.Data;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Repository;

public class TaskListRepository(ApplicationDbContext dbContext) : ITaskListRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<TaskList>?> GetOwnedOrJoinedTaskLists(string userId)
    {
        return await _dbContext.TaskList.Where(g => g.CreatedByUserId == userId || (g.ViewableToUserIds ?? "").Contains(userId)).ToListAsync();
    }
}
