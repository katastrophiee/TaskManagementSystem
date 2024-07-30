using TaskManagement.DTO.Responses.Group;

namespace TaskManagement.Interface.Provider;

public interface IGroupProvider
{
    Task<List<GetGroupResponse>> GetOwnedOrJoinedGroups(string userId, string userEmail);

    Task<GetGroupResponse> GetById(int groupId);
}
