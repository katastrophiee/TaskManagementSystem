using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Components.Pages.PageModels;
using TaskManagement.Data;
using TaskManagement.DTO.Responses.TaskList;
using TaskManagement.Interface.Provider;
using TaskManagement.DTO.Requests.Group;
using TaskManagement.DTO.Responses.Task;
using Microsoft.AspNet.Identity;

namespace TaskManagement.Components.Pages;

public partial class ViewGroup
{
    [Parameter]
    public int GroupId { get; set; }

    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public ITaskProvider TaskProvider { get; set; }

    [Inject]
    public ITaskListProvider TaskListProvider { get; set; }

    [Inject]
    public IGroupProvider GroupProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public UpdateGroupRequest UpdateGroupRequest { get; set; } = new();

    public List<GetTaskListResponse> GroupTaskLists { get; set; } = [];

    public List<GetTaskResponse> TaskListsTasks { get; set; } = [];

    private string OwnedByUserId = "";
    private string CurrentUserId = "";
    private ApplicationUser? User;
    private string? errorMessage;
    private string? warningMessage;
    private string GroupIdAsString;
    private string shareToUserEmail;
    private ViewableToEmail[] ViewableToUserEmails = [];
    private List<GetTaskListResponse> AvailableTaskLists = [];
    private bool isEditing = false;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;
        CurrentUserId = authState.User.Identity.GetUserId();

        User = await UserManager.FindByEmailAsync(userEmail);

        await GetGroup();

        OwnedByUserId = await GroupProvider.GetGroupOwnerId(GroupId) ?? "";

        GroupTaskLists = await TaskListProvider.GetTaskListsByGroupId(GroupId) ?? [];
        foreach (var taskList in GroupTaskLists)
        {
            var tasks = await TaskProvider.GetTasksByTaskListId(taskList.TaskListId) ?? [];
            TaskListsTasks.AddRange(tasks);
        }
    }

    private async Task GetGroup()
    {
        var group = await GroupProvider.GetById(GroupId);

        UpdateGroupRequest = group is null ? new() : new UpdateGroupRequest(group);

        if (UpdateGroupRequest is not null && !string.IsNullOrEmpty(UpdateGroupRequest.ViewableToUserIds))
            ViewableToUserEmails = UpdateGroupRequest.ViewableToUserIds.Split(",").Select(e => new ViewableToEmail(e, true)).ToArray();
    }

    private async Task UpdateGroup()
    {
        UpdateGroupRequest.ViewableToUserIds = string.Join(",", ViewableToUserEmails.Select(e => e.Email));

        if (!string.IsNullOrEmpty(GroupIdAsString))
            UpdateGroupRequest.GroupId = int.Parse(GroupIdAsString);

        var response = await GroupProvider.UpdateGroup(UpdateGroupRequest);

        if (response is false)
        {
            errorMessage = "Failed to update task";
            return;
        }

        // TO DO - add success message to pages to make a lil nicer

        isEditing = false;

        await GetGroup();
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

        if (ViewableToUserEmails.Select(e => e.Email == shareToUserEmail).Any())
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
        warningMessage = "Visibility of tasks and lists within this group WILL be changed on updating";
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

    //reassign viewable to users for the task list 
    private async Task CorrectViewableToUsersForBelongingItems(string groupIdAsString)
    {
        if (!string.IsNullOrEmpty(groupIdAsString))
        {
            var groupId = int.Parse(groupIdAsString);

            var group = await GroupProvider.GetById(groupId);
            if (group.ViewableToUserIds is not null && group.ViewableToUserIds.Any())
            {
                var groupMembers = group.ViewableToUserIds.Split(",");

                ViewableToUserEmails = groupMembers.Select(x => new ViewableToEmail(x, false)).ToArray();
            }

            GroupIdAsString = groupIdAsString;
        }
        else
        {
            ViewableToUserEmails = [];
        }
    }
}
