using TaskManagement.Common.Models;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Provider;

public class GroupProvider(IGroupRepository groupRepository) : IGroupProvider
{
    private readonly IGroupRepository _groupRepository = groupRepository;

    public async Task<List<GetGroupResponse>> GetOwnedOrJoinedGroups(string userId, string userEmail)
    {

        try
        {
            var groups = await _groupRepository.GetOwnedOrJoinedGroups(userId, userEmail) ?? [];

            var response = groups.Select(g => new GetGroupResponse(g));

            return response.ToList();
        }
        catch (Exception ex)
        {
            return
            [
                new(new ErrorResponse()
                {
                    Title = "An unknown error occured",
                    Description = $"An unknown error occured when trying to get task lists",
                    StatusCode = StatusCodes.Status404NotFound,
                    AdditionalDetails = ex.Message
                })
            ];
        }
    }

    public async Task<GetGroupResponse> GetById(int groupId)
    {

        try
        {
            var group = await _groupRepository.GetById(groupId);
            if (group == null)
            {
                return
                new(new ErrorResponse()
                {
                    Title = "Group not found",
                    Description = $"Group with id {groupId} not found",
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            return new GetGroupResponse(group);
        }
        catch (Exception ex)
        {
            return
            new(new ErrorResponse()
            {
                Title = "An unknown error occured",
                Description = $"An unknown error occured when trying to get task lists",
                StatusCode = StatusCodes.Status404NotFound,
                AdditionalDetails = ex.Message
            });
        }
    }
}
