using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Data;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.Interface.Provider;
using Microsoft.AspNetCore.Identity;
using TaskManagement.DTO.Requests.TaskList;
using Microsoft.AspNetCore.Components.Web;
using TaskManagement.Components.Pages.PageModels;
using TaskManagement.Components.Account.Pages.Manage;

namespace TaskManagement.Components.Pages;

public partial class AddTaskList
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public ITaskListProvider TaskListProvider { get; set; }

    [Inject]
    public IGroupProvider GroupProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [SupplyParameterFromForm]
    private AddTaskListRequest AddTaskListRequest { get; set; } = new();

    private string GroupIdAsString;

    private ApplicationUser? User { get; set; }

    private string? errorMessage;
    private string? warningMessage;
    private List<GetGroupResponse> AvailableGroups = [];
    private ViewableToEmail[] sharedToUserEmails = [];
    private string shareToUserEmail = "";

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);

        AvailableGroups = await GroupProvider.GetOwnedOrJoinedGroups(User.Id, userEmail);
    }

    private async Task AddNewTaskList()
    {
        //TO DO - Add add group and task list page then try and assign

        if (GroupIdAsString is not null)
            AddTaskListRequest.GroupId = int.Parse(GroupIdAsString);

        if (sharedToUserEmails.Length != 0)
            AddTaskListRequest.ViewableToUserIds = string.Join(",", sharedToUserEmails.Select(x => x.Email).ToList());

        AddTaskListRequest.CreatedByUserId = User.Id;
        var success = await TaskListProvider.AddTaskList(AddTaskListRequest);

        if (success)
        {
            NavigationManager.NavigateTo("/ViewTaskLists");
        }
        else
        {
            errorMessage = "An error occured when adding this task list. Please try again later.";
        }
    }

    //if group is selected, lock the manual field and auto set the shared to users to the group members

    private async Task ShareTaskListToUser()
    {
        if (shareToUserEmail == User.Email)
        {
           errorMessage = "You cannot share a task list with yourself";
            return;
        }

        var existingSharedToUser = sharedToUserEmails.Where(e => e.Email == shareToUserEmail).FirstOrDefault();
        if (existingSharedToUser is not null)
        {
            errorMessage = "You have already shared this task list with this user";
            return;
        }

        if (GroupIdAsString is not null && !string.IsNullOrEmpty(GroupIdAsString))
        {
            errorMessage = "You cannot alter the shared with users if the task list is assigned to a group";
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

    private async Task CorrectViewableToUsers(string groupIdAsString)
    {
        if (string.IsNullOrEmpty(groupIdAsString))
        {
            warningMessage = "";
            GroupIdAsString = "";
            sharedToUserEmails = [];
            return;
        }

        var groupId = int.Parse(groupIdAsString);

        var group = await GroupProvider.GetById(groupId);
        if (!string.IsNullOrEmpty(group.ViewableToUserIds))
        {
            var groupMembers = group.ViewableToUserIds.Split(",");

            sharedToUserEmails = groupMembers.Select(x => new ViewableToEmail(x, false)).ToArray();
        }

        GroupIdAsString = groupIdAsString;
        warningMessage = "Viewable users have been set to users within the group you assigned the task list to.";
    }

    private void RemoveSharedToUser(string email)
    {
        sharedToUserEmails = sharedToUserEmails.Where(e => e.Email != email).ToArray();
    }
}
