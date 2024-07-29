using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;

namespace TaskManagement.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Common.Models.Task> Task { get; set; }
    public DbSet<TaskList> TaskList { get; set; }
    public DbSet<Group> Group { get; set; }
}
