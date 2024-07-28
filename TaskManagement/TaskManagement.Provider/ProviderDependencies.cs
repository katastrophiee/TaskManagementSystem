using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Interface.Provider;

namespace TaskManagement.Provider;

public static class ProviderDependencies
{
    public static void InjectProviderDependencies(this IServiceCollection services)
    {
        services.AddScoped<ITaskUserProvider, TaskUserProvider>();
        services.AddScoped<IAuthProvider, AuthProvider>();
        services.AddScoped<IAdminUserProvider, AdminUserProvider>();
    }
}
