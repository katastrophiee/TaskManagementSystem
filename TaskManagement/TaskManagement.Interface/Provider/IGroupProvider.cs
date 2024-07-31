using TaskManagement.DTO.Requests.Group;
using TaskManagement.DTO.Responses.Group;

namespace TaskManagement.Interface.Provider;

public interface IGroupProvider
{
    Task<List<GetGroupResponse>> GetOwnedOrJoinedGroups(string userId, string userEmail);

    Task<GetGroupResponse> GetById(int groupId);

    Task<bool> AddGroup(AddGroupRequest request);

    Task<bool> UpdateGroup(UpdateGroupRequest request);

    Task<string?> GetGroupOwnerId(int groupId);
}
