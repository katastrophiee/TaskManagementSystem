using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using TaskManagement.Data;
using TaskManagement.DTO.Responses.Task;
using TaskManagement.Interface.Provider;
using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Components.Pages;

public partial class ViewTasks
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public ITaskProvider TaskProvider { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }


    public ApplicationUser? User { get; set; }
    public List<GetTaskResponse>? Tasks { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);

        var userTasks = await TaskProvider.GetTasksByUserId(User.Id);

        Tasks = userTasks.Where(t => t.TaskListId == null && t.GroupId == null).ToList();
    }
}
