using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;
using TaskManagement.Data;
using TaskManagement.Repository.EntityTypeConfiguration;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Repository.DBContext;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<TaskUser> TaskUser { get; set; }
    public DbSet<AdminUser> AdminUser { get; set; }
    public DbSet<Task> Task { get; set; }
    public DbSet<TaskList> TaskList { get; set; }
    public DbSet<Group> Group { get; set; }
    public DbSet<Roles> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TaskUserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new AdminUserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TaskEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TaskListEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GroupEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RolesEntityTypeConfiguration());
    }
}
