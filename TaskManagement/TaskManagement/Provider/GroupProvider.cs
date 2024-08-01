using System.Threading.Tasks;
using TaskManagement.Common.Models;
using TaskManagement.DTO.Requests.Group;
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

    public async Task<bool> AddGroup(AddGroupRequest request)
    {
        try
        {
            var group = new Group()
            {
                Name = request.Name,
                Description = request.Description,
                CreatedByUserId = request.CreatedByUserId,
                CreatedOn = DateTime.Now
            };

            if (!string.IsNullOrEmpty(request.ViewableToUserIds))
            {
                group.ViewableToUserIds = request.ViewableToUserIds;
            }

            await _groupRepository.Add(group);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateGroup(UpdateGroupRequest request)
    {
        try
        {
            var group = await _groupRepository.GetById(request.GroupId);
            if (group is null)
                return false;

            if (request.Name is not null)
                group.Name = request.Name;

            if (request.Description is not null)
                group.Description = request.Description;

            if (!string.IsNullOrEmpty(request.ViewableToUserIds))
                group.ViewableToUserIds = request.ViewableToUserIds;

            await _groupRepository.Update(group);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<string?> GetGroupOwnerId(int groupId)
    {
        try
        {
            var group = await _groupRepository.GetById(groupId);
            if (group == null)
            {
                return null;
            }

            return group.CreatedByUserId;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
