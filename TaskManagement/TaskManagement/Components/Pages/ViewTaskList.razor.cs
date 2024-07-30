using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Data;
using TaskManagement.DTO.Requests.Task;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.DTO.Responses.TaskList;
using TaskManagement.Interface.Provider;
using Microsoft.AspNetCore.Identity;
using TaskManagement.DTO.Requests.TaskList;
using TaskManagement.Provider;
using TaskManagement.Components.Pages.PageModels;
using System.Linq;

namespace TaskManagement.Components.Pages;

public partial class ViewTaskList
{
    [Parameter]
    public int TaskListId { get; set; }

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

    public UpdateTaskListRequest UpdateTaskListRequest { get; set; } = new();


    private ApplicationUser? User;
    private string? errorMessage;
    private string GroupIdAsString;
    private string shareToUserEmail;
    private ViewableToEmail[] ViewableToUserEmails = [];
    private List<GetTaskListResponse> AvailableTaskLists = [];
    private List<GetGroupResponse> AvailableGroups = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);

        await GetTaskList();
    }


    private async Task GetTaskList()
    {
        var taskList = await TaskListProvider.GetTaskListById(TaskListId);

        UpdateTaskListRequest = taskList is null ? new() : new UpdateTaskListRequest(taskList);

        if (UpdateTaskListRequest is not null && UpdateTaskListRequest.ViewableToUserIds is not null && UpdateTaskListRequest.ViewableToUserIds.Any())
            ViewableToUserEmails = UpdateTaskListRequest.ViewableToUserIds.Split(",").Select(e => new ViewableToEmail(e, UpdateTaskListRequest.GroupId is null)).ToArray();
    }

    private async Task UpdateTaskList()
    {
        UpdateTaskListRequest.TaskListId = TaskListId;
        UpdateTaskListRequest.ViewableToUserIds = string.Join(",", ViewableToUserEmails.Select(e => e));

        var response = await TaskListProvider.UpdateTaskList(UpdateTaskListRequest);

        if (response is false)
        {
            errorMessage = "Failed to update task";
            return;
        }

        await GetTaskList();
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


    //TO DO - test this :)
    private async Task CorrectViewableToUsers()
    {
        if (GroupIdAsString is not null && !string.IsNullOrEmpty(GroupIdAsString))
        {
            var groupId = int.Parse(GroupIdAsString);

            var group = await GroupProvider.GetById(groupId);
            if (group.ViewableToUserIds is not null && group.ViewableToUserIds.Any())
            {
                var groupMembers = group.ViewableToUserIds.Split(",");

                ViewableToUserEmails = groupMembers.Select(x => new ViewableToEmail(x, false)).ToArray();
            }
        }
    }

}
