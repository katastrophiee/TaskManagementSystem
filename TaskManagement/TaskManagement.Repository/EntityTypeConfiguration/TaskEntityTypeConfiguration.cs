using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Task = TaskManagement.Common.Models.Task;

namespace TaskManagement.Repository.EntityTypeConfiguration;

public sealed class TaskEntityTypeConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(e => e.TaskId);
    }
}
