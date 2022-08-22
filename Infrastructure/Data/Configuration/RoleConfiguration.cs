using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Data.Configuration;

public class RolConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Rol");
        builder.Property(p => p.Id)
                .IsRequired();
        builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
    }
}

