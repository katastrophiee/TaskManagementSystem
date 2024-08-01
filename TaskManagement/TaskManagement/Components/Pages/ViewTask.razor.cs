using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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

        if (!string.IsNullOrEmpty(TaskListIdAsString))
            UpdateTaskRequest.TaskListId = int.Parse(TaskListIdAsString);
        else
            UpdateTaskRequest.TaskListId = null;

        if (!string.IsNullOrEmpty(GroupIdAsString))
            UpdateTaskRequest.GroupId = int.Parse(GroupIdAsString);
        else
            UpdateTaskRequest.GroupId = null;

        var response = await TaskProvider.UpdateTask(UpdateTaskRequest);

        if (response is false)
        {
            errorMessage = "Failed to update task";
            return;
        }

        NavigationManager.NavigateTo($"/ViewTasks");

        await GetTask();
    }

    private async Task CorrectSharedToUsersAndGroupOption(string taskListIdAsString)
    {
        if (string.IsNullOrEmpty(taskListIdAsString))
        {
            TaskListIdAsString = "";
            AvailableTaskLists = await TaskListProvider.GetOwnedOrJoinedTaskLists(User.Id, User.Email);
            AvailableGroups = await GroupProvider.GetOwnedOrJoinedGroups(User.Id, User.Email);
            return;
        }

        var taskListId = int.Parse(taskListIdAsString);

        var taskList = await TaskListProvider.GetById(taskListId);

        if (taskList.GroupId is not null)
        {
            var groupContainingTaskList = await GroupProvider.GetById(taskList.GroupId.Value);

            AvailableGroups = [groupContainingTaskList];
            GroupIdAsString = taskList.GroupId.Value.ToString();
        }
        else
        {
            AvailableGroups = [];
        }

        TaskListIdAsString = taskListIdAsString;
    }


    private async Task CorrectValuesOnGroupChange(string groupIdAsString)
    {
        if (string.IsNullOrEmpty(groupIdAsString))
        {
            if (!string.IsNullOrEmpty(TaskListIdAsString))
            {
                warningMessage = "Task list is part of this group, therefore the group cannot be deselected.";
                return;
            }

            GroupIdAsString = "";
            AvailableTaskLists = await TaskListProvider.GetOwnedOrJoinedTaskLists(User.Id, User.Email);
            AvailableGroups = await GroupProvider.GetOwnedOrJoinedGroups(User.Id, User.Email);
            return;
        }

        var groupId = int.Parse(groupIdAsString);

        var group = await GroupProvider.GetById(groupId);

        var taskListsInGroup = await TaskListProvider.GetTaskListsByGroupId(groupId);

        var selectedTaskListInAvailableOptions = taskListsInGroup.FirstOrDefault(t => t.TaskListId.ToString() == TaskListIdAsString);

        AvailableTaskLists = taskListsInGroup;

        TaskListIdAsString = selectedTaskListInAvailableOptions is null ? "" : selectedTaskListInAvailableOptions.TaskListId.ToString();

        GroupIdAsString = groupIdAsString;
    }
}
