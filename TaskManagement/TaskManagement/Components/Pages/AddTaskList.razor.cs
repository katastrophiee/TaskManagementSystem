using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Data;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.Interface.Provider;
using Microsoft.AspNetCore.Identity;
using TaskManagement.DTO.Requests.TaskList;
using Microsoft.AspNetCore.Components.Web;

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
    private List<GetGroupResponse> AvailableGroups = [];
    private string[] sharedToUserEmails = [];
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
            AddTaskListRequest.ViewableToUserIds = string.Join(",", sharedToUserEmails);

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

    //private async Task HandleKeyPress(KeyboardEventArgs e)
    //{
    //    if (e.Key == "Enter")
    //    {
    //        await ShareTaskListToUser();
    //    }
    //}

    //if group is selected, lock the manual field and auto set the shared to users to the group members

    private async Task ShareTaskListToUser()
    {
        if (shareToUserEmail == User.Email)
        {
           errorMessage = "You cannot share a task list with yourself";
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

        sharedToUserEmails = [.. sharedToUserEmails, shareToUserEmail];
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

    //TO DO - test this :)
    private async Task CorrectViewableToUsers()
    {
        var groupId = int.Parse(GroupIdAsString);
    }
}
