using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Common.Models;
using System.Text.Json;

namespace TaskManagement.Repository.EntityTypeConfiguration;

public sealed class AdminUserEntityTypeConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder.HasKey(e => e.AdminUserId);

        builder.Property(e => e.UserRoles).HasConversion(dbIn => JsonSerializer.Serialize(dbIn, (JsonSerializerOptions)null), dbOut => JsonSerializer.Deserialize<List<Roles>>(dbOut, (JsonSerializerOptions)null));
    }
}
