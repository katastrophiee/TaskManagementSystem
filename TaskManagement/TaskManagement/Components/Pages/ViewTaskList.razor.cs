using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Data;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.DTO.Responses.TaskList;
using TaskManagement.Interface.Provider;
using TaskManagement.DTO.Requests.TaskList;
using TaskManagement.Components.Pages.PageModels;
using Microsoft.AspNet.Identity;

namespace TaskManagement.Components.Pages;

public partial class ViewTaskList
{
    [Parameter]
    public int TaskListId { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public ITaskListProvider TaskListProvider { get; set; }

    [Inject]
    public IGroupProvider GroupProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public UpdateTaskListRequest UpdateTaskListRequest { get; set; } = new();

    private string OwnedByUserId = "";
    private string CurrentUserId = "";
    private ApplicationUser? User;
    private string? errorMessage;
    private string? warningMessage;
    private string GroupIdAsString;
    private string shareToUserEmail;
    private ViewableToEmail[] ViewableToUserEmails = [];
    private List<GetTaskListResponse> AvailableTaskLists = [];
    private List<GetGroupResponse> AvailableGroups = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;
        CurrentUserId = authState.User.Identity.GetUserId();

        User = await UserManager.FindByEmailAsync(userEmail);

        await GetTaskList();

        OwnedByUserId = await TaskListProvider.GetTaskListOwnerId(TaskListId) ?? "";

        AvailableGroups = await GroupProvider.GetOwnedOrJoinedGroups(User.Id, userEmail);
    }


    private async Task GetTaskList()
    {
        var taskList = await TaskListProvider.GetById(TaskListId);

        UpdateTaskListRequest = taskList is null ? new() : new UpdateTaskListRequest(taskList);

        if (UpdateTaskListRequest is not null && UpdateTaskListRequest.ViewableToUserIds is not null && UpdateTaskListRequest.ViewableToUserIds.Any())
            ViewableToUserEmails = UpdateTaskListRequest.ViewableToUserIds.Split(",").Select(e => new ViewableToEmail(e, UpdateTaskListRequest.GroupId is null)).ToArray();
        
        if (UpdateTaskListRequest.GroupId is not null)
            GroupIdAsString = UpdateTaskListRequest.GroupId.ToString() ?? "";
    }

    private async Task UpdateTaskList()
    {
        UpdateTaskListRequest.TaskListId = TaskListId;
        UpdateTaskListRequest.ViewableToUserIds = string.Join(",", ViewableToUserEmails.Select(e => e.Email));

        if (!string.IsNullOrEmpty(GroupIdAsString))
            UpdateTaskListRequest.GroupId = int.Parse(GroupIdAsString);

        var success = await TaskListProvider.UpdateTaskList(UpdateTaskListRequest);
        if (success)
        {
            NavigationManager.NavigateTo("/ViewTaskLists");
        }
        else
        {
            errorMessage = "An unknown error occured when updating task";
            return;
        }
    }

    private async Task ShareTaskListToUser()
    {
        if (shareToUserEmail == User.Email)
        {
            errorMessage = "You cannot share a task list with yourself";
            return;
        }

        if (GroupIdAsString is not null && !string.IsNullOrEmpty(GroupIdAsString))
        {
            errorMessage = "You cannot alter the shared with users if the task list is assigned to a group";
            return;
        }

        var existingSharedToUser = ViewableToUserEmails.Where(e => e.Email == shareToUserEmail).FirstOrDefault();
        if (existingSharedToUser is not null)
        {
            errorMessage = "You have already shared this task list with this user";
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

        ViewableToUserEmails = ViewableToUserEmails.Append(new ViewableToEmail(shareToUserEmail, true)).ToArray();
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

    private void RemoveSharedToUser(string email)
    {
        ViewableToUserEmails = ViewableToUserEmails.Where(e => e.Email != email).ToArray();
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
        if (!string.IsNullOrEmpty(groupIdAsString))
        {
            warningMessage = "";
            GroupIdAsString = "";
            ViewableToUserEmails = [];
            return;
        }

        var groupId = int.Parse(groupIdAsString);

        var group = await GroupProvider.GetById(groupId);
        if (!string.IsNullOrEmpty(group.ViewableToUserIds))
        {
            var groupMembers = group.ViewableToUserIds.Split(",");

            ViewableToUserEmails = groupMembers.Select(x => new ViewableToEmail(x, false)).ToArray();
        }

        GroupIdAsString = groupIdAsString;
        warningMessage = "Viewable users have been set to users within the group you assigned the task list to.";
    }
}
