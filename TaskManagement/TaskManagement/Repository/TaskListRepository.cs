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

    public async Task<TaskList?> GetById(int taskListId) => await _dbContext.TaskList.FirstOrDefaultAsync(u => u.TaskListId == taskListId);


    public async Task<bool> Update(TaskList taskList)
    {
        _dbContext.TaskList.Update(taskList);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskList>?> GetListByGroupId(int groupId)
    {
        return await _dbContext.TaskList.Where(g => g.GroupId == groupId).ToListAsync();
    }
}
