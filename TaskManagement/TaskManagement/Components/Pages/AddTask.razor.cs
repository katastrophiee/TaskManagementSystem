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

        if (TaskListIdAsString is not null)
            AddTaskRequest.TaskListId = int.Parse(TaskListIdAsString);

        if (GroupIdAsString is not null)
            AddTaskRequest.GroupId = int.Parse(GroupIdAsString);

        AddTaskRequest.CreatedByUserId = User.Id;
        var success = await TaskProvider.AddTask(AddTaskRequest);
    }
}
