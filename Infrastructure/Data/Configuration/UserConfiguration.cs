using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Usuario");
        builder.Property(p => p.Id)
                .IsRequired();
        builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);
        builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(200);
        builder.Property(p => p.Username)
                .IsRequired()
                .HasMaxLength(200);
        builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(200);

        builder
        .HasMany(p => p.Roles)
        .WithMany(p => p.Users)
        .UsingEntity<UsersRoles>(
            j => j
                .HasOne(pt => pt.Role)
                .WithMany(t => t.UsersRoles)
                .HasForeignKey(pt => pt.RolId),
            j => j
                .HasOne(pt => pt.User)
                .WithMany(p => p.UsersRoles)
                .HasForeignKey(pt => pt.UserId),
            j =>
            {
                j.HasKey(t => new { t.UserId, t.RolId });
            });

    }
}

