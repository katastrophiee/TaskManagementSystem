using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;
using TaskManagement.Data;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Repository;

public class GroupRepository(ApplicationDbContext dbContext) : IGroupRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<Group>?> GetOwnedOrJoinedGroups(string userId, string userEmail)
    {
        return await _dbContext.Group.Where(g => g.CreatedByUserId == userId || (g.ViewableToUserIds ?? "").Contains(userEmail)).ToListAsync();
    }

    public Task<Group?> GetById(int groupId) => _dbContext.Group.FirstOrDefaultAsync(g => g.GroupId == groupId);

    public async Task<bool> Add(Group group)
    {
        _dbContext.Group.Add(group);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
