using TaskManagement.Common.Models;

namespace TaskManagement.Interface.Repository;

public interface IGroupRepository
{
    Task<List<Group>?> GetOwnedOrJoinedGroups(string userId, string userEmail);

    Task<Group?> GetById(int groupId);

    Task<bool> Add(Group group);
}
