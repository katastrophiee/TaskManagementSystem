using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Task = System.Threading.Tasks.Task;
using TaskManagement.Data;
using Microsoft.AspNetCore.Components.Authorization;
using TaskManagement.Interface.Provider;
using TaskManagement.DTO.Responses.Task;

namespace TaskManagement.Components.Pages;

public partial class Home
{
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    [Inject]
    public UserManager<ApplicationUser> UserManager { get; set; }

    [Inject]
    public ITaskProvider TaskProvider { get; set; }

    public ApplicationUser? User { get; set; }
    public List<GetTaskResponse>? Tasks { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.Identity.Name;

        User = await UserManager.FindByEmailAsync(userEmail);

        var userTasks = await TaskProvider.GetTasksByUserId(User.Id);

        Tasks = userTasks;
    }
}
