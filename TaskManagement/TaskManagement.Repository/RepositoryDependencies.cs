using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Interface.Repository;

namespace TaskManagement.Repository;

public static class RepositoryDependencies
{
    public static void InjectRepositoryDependencies(this IServiceCollection services)
    {
        services.AddScoped<ITaskUserRepository, TaskUserRepository>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
    }
}
