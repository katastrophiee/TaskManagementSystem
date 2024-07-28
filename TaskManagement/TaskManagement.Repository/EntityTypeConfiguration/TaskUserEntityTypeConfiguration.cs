using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using TaskManagement.Common.Models;

namespace TaskManagement.Repository.EntityTypeConfiguration;

public sealed class TaskUserEntityTypeConfiguration : IEntityTypeConfiguration<TaskUser>
{
    public void Configure(EntityTypeBuilder<TaskUser> builder)
    {
        builder.HasKey(e => e.TaskUserId);

        builder.Property(e => e.UserRoles).HasConversion(dbIn => JsonSerializer.Serialize(dbIn, (JsonSerializerOptions)null), dbOut => JsonSerializer.Deserialize<List<Roles>>(dbOut, (JsonSerializerOptions)null));
    }
}
