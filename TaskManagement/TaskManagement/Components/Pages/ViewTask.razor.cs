using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Data;
using TaskManagement.DTO.Requests.Task;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.DTO.Responses.TaskList;
using TaskManagement.Interface.Provider;

namespace TaskManagement.Components.Pages;

public partial class ViewTask
{
    [Parameter]
    public int TaskId { get; set; }

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

    public UpdateTaskRequest UpdateTaskRequest { get; set; } = new();

    private string OwnedByUserId = "";
    private string CurrentUserId = "";
    private ApplicationUser? User;
    private string? errorMessage;
    private string? warningMessage;
    private string TaskListIdAsString;
    private string GroupIdAsString;
    private List<GetTaskListResponse> AvailableTaskLists = [];
    private List<GetGroupResponse> AvailableGroups = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;
        CurrentUserId = authState.User.Identity.GetUserId();

        User = await UserManager.FindByEmailAsync(userEmail);

        await GetTask();

        OwnedByUserId = await TaskProvider.GetTaskOwnerId(TaskId) ?? "";

        AvailableTaskLists = await TaskListProvider.GetOwnedOrJoinedTaskLists(User.Id, userEmail);
        AvailableGroups = await GroupProvider.GetOwnedOrJoinedGroups(User.Id, userEmail);
    }

    private async Task GetTask()
    {
        var task = await TaskProvider.GetTaskById(TaskId);

        UpdateTaskRequest = new UpdateTaskRequest(task);

        if (UpdateTaskRequest.TaskListId is not null)
        {
            TaskListIdAsString = UpdateTaskRequest.TaskListId.ToString();
        }
        else         
        {
            TaskListIdAsString = "";
        }

        if (UpdateTaskRequest.GroupId is not null)
        {
            GroupIdAsString = UpdateTaskRequest.GroupId.ToString();
        }
        else
        {
            GroupIdAsString = "";
        }
    }

    private async Task UpdateTask()
    {
        UpdateTaskRequest.TaskId = TaskId;

        var response = await TaskProvider.UpdateTask(UpdateTaskRequest);

        if (response is false)
        {
            errorMessage = "Failed to update task";
            return;
        }

        await GetTask();
    }

    private async Task CorrectValuesOnGroupChange(string groupIdAsString)
    {
        if (!string.IsNullOrEmpty(groupIdAsString))
        {
            var groupId = int.Parse(groupIdAsString);

            AvailableTaskLists = await TaskListProvider.GetTaskListsByGroupId(groupId) ?? [];

            GroupIdAsString = groupIdAsString;
            warningMessage = "Visibility of this task to other users WILL be changed to the group visibility on updating";
        }
        else
        {
            AvailableTaskLists = (await TaskListProvider.GetOwnedOrJoinedTaskLists(User.Id, User.Email)).Where(t => t.GroupId is null).ToList();
        }
    }
}
