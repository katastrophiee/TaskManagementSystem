using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Common.Models;

namespace TaskManagement.Repository.EntityTypeConfiguration;

public sealed class RolesEntityTypeConfiguration : IEntityTypeConfiguration<Roles>
{
    public void Configure(EntityTypeBuilder<Roles> builder)
    {
        builder.HasKey(e => e.RoleId);
    }
}
