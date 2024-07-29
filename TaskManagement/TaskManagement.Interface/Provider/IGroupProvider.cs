using TaskManagement.DTO.Responses.Group;

namespace TaskManagement.Interface.Provider;

public interface IGroupProvider
{
    Task<List<GetGroupResponse>> GetOwnedOrJoinedGroups(string userId);
}
