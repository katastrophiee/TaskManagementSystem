using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;

namespace TaskManagement.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    //public DbSet<TaskUser> TaskUser { get; set; }
    //public DbSet<AdminUser> AdminUser { get; set; }
    //public DbSet<Common.Models.Task> Task { get; set; }
    //public DbSet<TaskList> TaskList { get; set; }
    //public DbSet<Group> Group { get; set; }
    //public DbSet<Roles> Roles { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.ApplyConfiguration(new TaskUserEntityTypeConfiguration());
    //    modelBuilder.ApplyConfiguration(new AdminUserEntityTypeConfiguration());
    //    modelBuilder.ApplyConfiguration(new TaskEntityTypeConfiguration());
    //    modelBuilder.ApplyConfiguration(new TaskListEntityTypeConfiguration());
    //    modelBuilder.ApplyConfiguration(new GroupEntityTypeConfiguration());
    //    modelBuilder.ApplyConfiguration(new RolesEntityTypeConfiguration());
    //}
}
