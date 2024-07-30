using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Data;
using TaskManagement.DTO.Responses.Task;
using TaskManagement.DTO.Responses.TaskList;
using TaskManagement.Interface.Provider;
using Microsoft.AspNetCore.Identity;
using TaskManagement.DTO.Responses.Group;

namespace TaskManagement.Components.Pages;

public partial class ViewTaskLists
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public ITaskListProvider TaskListProvider { get; set; }

    [Inject]
    public ITaskProvider TaskProvider { get; set; }

    [Inject]
    public IGroupProvider GroupProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }


    public ApplicationUser? User { get; set; }
    public List<GetTaskListResponse>? OwnedTaskLists { get; set; }
    public List<GetTaskListResponse>? SharedToMeUserTaskLists { get; set; }
    public List<GetGroupResponse> GroupsContainingReturnedTaskLists { get; set; } = [];
    public List<GetTaskResponse>? TasksInTaskLists { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);

        var userTaskLists = await TaskListProvider.GetOwnedOrJoinedTaskLists(User.Id, userEmail);

        foreach (var taskList in userTaskLists)
        {
            if (taskList.GroupId is not null)
            {
                var group = await GroupProvider.GetById(taskList.GroupId.Value);
                GroupsContainingReturnedTaskLists.Add(group);
            }
        }

        OwnedTaskLists = [.. userTaskLists.Where(t => t.CreatedByUserId == User.Id).ToList()
            .OrderBy(t => t.GroupId == null)
            .ThenBy(t => t.GroupId)];

        SharedToMeUserTaskLists = [.. userTaskLists.Where(t => (t.ViewableToUserIds ?? "").Contains(userEmail)).ToList()
            .OrderBy(t => t.GroupId == null)
            .ThenBy(t => t.GroupId)];

        foreach (var task in userTaskLists)
        {
            var tasks = await TaskProvider.GetTasksByTaskListId(task.TaskListId) ?? [];
            TasksInTaskLists!.AddRange(tasks);
        }
    }
}
