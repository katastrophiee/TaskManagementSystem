using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
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

    //TO DO - add listof task lists and groups to select from when assigning a task to either one

    [SupplyParameterFromForm]
    private AddTaskRequest AddTaskRequest { get; set; } = new();

    private string TaskListIdAsString;
    private string GroupIdAsString;

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
        //TO DO - Add add group and task list page then try and assign

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

    private async Task CorrectSharedToUsersAndTaskListOptions(string groupIdAsString)
    {
        var groupId = int.Parse(groupIdAsString);

        var group = await GroupProvider.GetById(groupId);

        var taskListsInGroup = await TaskListProvider.GetTaskListsByGroupId(groupId);
        //AvailableTaskLists = taskListsInGroup ?? [];

        var selectedTaskListInAvailableOptions = taskListsInGroup.FirstOrDefault(t => t.TaskListId.ToString() == TaskListIdAsString);

        AvailableTaskLists = taskListsInGroup;

        TaskListIdAsString = selectedTaskListInAvailableOptions is null ? "" : selectedTaskListInAvailableOptions.TaskListId.ToString();

        GroupIdAsString = groupIdAsString;
    }
}
