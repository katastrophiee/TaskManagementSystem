using TaskManagement.Common.Models;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.Interface.Provider;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Provider;

public class GroupProvider(IGroupRepository groupRepository) : IGroupProvider
{
    private readonly IGroupRepository _groupRepository = groupRepository;

    public async Task<List<GetGroupResponse>> GetOwnedOrJoinedGroups(string userId)
    {

        try
        {
            var taskLists = await _groupRepository.GetOwnedOrJoinedGroups(userId) ?? [];

            var response = taskLists.Select(g => new GetGroupResponse(g));

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
}
