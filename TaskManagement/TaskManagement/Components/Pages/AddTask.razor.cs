using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Data;
using TaskManagement.DTO.Requests.Task;
using TaskManagement.DTO.Responses.Group;
using TaskManagement.DTO.Responses.TaskList;
using TaskManagement.Interface.Provider;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Components.Pages;

public partial class AddTask
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public ITaskProvider TaskProvider { get; set; }

    [Inject]
    public ITaskListProvider TaskListProvider { get; set; }

    [Inject]
    public IGroupProvider GroupProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [SupplyParameterFromForm]
    private AddTaskRequest AddTaskRequest { get; set; } = new();

    private string TaskListIdAsString;
    private string GroupIdAsString;
    private string? warningMessage;
    private ApplicationUser? User;
    private string? errorMessage;
    private List<GetTaskListResponse> AvailableTaskLists = [];
    private List<GetGroupResponse> AvailableGroups = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);

        AvailableTaskLists = await TaskListProvider.GetOwnedOrJoinedTaskLists(User.Id, userEmail);

        AvailableGroups = await GroupProvider.GetOwnedOrJoinedGroups(User.Id, userEmail);
    }

    private async Task AddNewTask()
    {
        if (!string.IsNullOrEmpty(TaskListIdAsString))
            AddTaskRequest.TaskListId = int.Parse(TaskListIdAsString);

        if (!string.IsNullOrEmpty(GroupIdAsString))
            AddTaskRequest.GroupId = int.Parse(GroupIdAsString);

        AddTaskRequest.CreatedByUserId = User.Id;
        var success = await TaskProvider.AddTask(AddTaskRequest);

        if (success)
        {
           NavigationManager.NavigateTo("/ViewTasks");
        }
        else
        {
            errorMessage = "Failed to add task";
        }
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

    private async Task CorrectSharedToUsersAndTaskListOptions(string groupIdAsString)
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
