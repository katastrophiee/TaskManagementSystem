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
    public UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public ITaskProvider TaskProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public UpdateTaskRequest UpdateTaskRequest { get; set; } = new();


    private ApplicationUser? User;
    private string? errorMessage;
    private string TaskListIdAsString;
    private string GroupIdAsString;
    private List<GetTaskListResponse> AvailableTaskLists = [];
    private List<GetGroupResponse> AvailableGroups = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);

        await GetTask();
    }

    private async Task GetTask()
    {
        var task = await TaskProvider.GetTaskById(TaskId);

        UpdateTaskRequest = task is null ? new() : new UpdateTaskRequest(task);
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
}
