using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Data;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.Interface.Provider;
using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Components.Pages;

public partial class ViewGroups
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public IGroupProvider GroupProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }


    public ApplicationUser? User { get; set; }
    public List<GetGroupResponse>? OwnedGroups { get; set; }
    public List<GetGroupResponse>? SharedToMeGroups { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);

        var userGroups = await GroupProvider.GetOwnedOrJoinedGroups(User.Id, userEmail);

        OwnedGroups = [.. userGroups.Where(t => t.CreatedByUserId == User.Id).ToList()
            .OrderBy(t => t.GroupId == null)
            .ThenBy(t => t.GroupId)];

        SharedToMeGroups = [.. userGroups.Where(t => (t.ViewableToUserIds ?? "").Contains(userEmail)).ToList()
            .OrderBy(t => t.GroupId == null)
            .ThenBy(t => t.GroupId)];

    }
}
