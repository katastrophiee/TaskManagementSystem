using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Components.Pages.PageModels;
using TaskManagement.Data;
using TaskManagement.Interface.Provider;
using Microsoft.AspNetCore.Identity;
using TaskManagement.DTO.Requests.Group;

namespace TaskManagement.Components.Pages;

public partial class AddGroup
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public IGroupProvider GroupProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [SupplyParameterFromForm]
    private AddGroupRequest AddGroupRequest { get; set; } = new();

    private ApplicationUser? User { get; set; }

    private string? errorMessage;
    private ViewableToEmail[] sharedToUserEmails = [];
    private string shareToUserEmail = "";

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);
    }

    private async Task AddNewGroup()
    {
        if (sharedToUserEmails.Length != 0)
            AddGroupRequest.ViewableToUserIds = string.Join(",", sharedToUserEmails.Select(x => x.Email).ToList());

        AddGroupRequest.CreatedByUserId = User.Id;
        var success = await GroupProvider.AddGroup(AddGroupRequest);

        if (success)
        {
            NavigationManager.NavigateTo("/ViewGroups");
        }
        else
        {
            errorMessage = "An error occured when adding this group. Please try again later.";
        }
    }

    //TO DO - if group is selected, lock the manual field and auto set the shared to users to the group members
    //TO DO - make it so users who are not group owner cannot edit the group + task lists same
    private async Task ShareGroupToUser()
    {
        if (shareToUserEmail == User.Email)
        {
            errorMessage = "You cannot share a group with yourself";
            return;
        }

        if (sharedToUserEmails.Where(e => e.Email == shareToUserEmail).Any())
        {
            errorMessage = "You have already shared this group with this user";
            return;
        }

        if (!IsValidEmail(shareToUserEmail))
        {
            errorMessage = "Email must be formatted correctly";
            return;
        }

        var userExists = await CheckUserExists(shareToUserEmail);
        if (!userExists)
        {
            errorMessage = "No user exists with this email, please check you typed it correctly.";
            return;
        }

        sharedToUserEmails = sharedToUserEmails.Append(new ViewableToEmail(shareToUserEmail, true)).ToArray();
        shareToUserEmail = string.Empty;
        errorMessage = string.Empty;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private async Task<bool> CheckUserExists(string email)
    {
        try
        {
            var user = await UserManager.FindByEmailAsync(email);
            return user is null
                ? false
                : true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void RemoveSharedToUser(string email)
    {
        sharedToUserEmails = sharedToUserEmails.Where(e => e.Email != email).ToArray();
    }
}
