using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;

namespace TaskManagement.Repository.EntityTypeConfiguration;

public sealed class TaskListEntityTypeConfiguration : IEntityTypeConfiguration<TaskList>
{
    public void Configure(EntityTypeBuilder<TaskList> builder)
    {
        builder.HasKey(e => e.TaskListId);
    }
}
