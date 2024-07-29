using TaskManagement.Common.Models;

namespace TaskManagement.Interface.Repository;

public interface IGroupRepository
{
    Task<List<Group>?> GetOwnedOrJoinedGroups(string userId);
}
