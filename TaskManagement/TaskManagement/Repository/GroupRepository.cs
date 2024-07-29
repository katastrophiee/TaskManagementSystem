using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;
using TaskManagement.Data;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Repository;

public class GroupRepository(ApplicationDbContext dbContext) : IGroupRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<List<Group>?> GetOwnedOrJoinedGroups(string userId)
    {
        return await _dbContext.Group.Where(g => g.CreatedByUserId == userId || (g.ViewableToUserIds ?? "").Contains(userId)).ToListAsync();
    }
}
