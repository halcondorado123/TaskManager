using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TaskManager.Domain.Entities.Models.Identity;

namespace TaskManager.Infraestructure.Data.Configuration.Identity
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable("AspNetRoles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name).HasMaxLength(256);
            builder.Property(r => r.NormalizedName).HasMaxLength(256);
            builder.Property(r => r.ConcurrencyStamp);

            builder.HasData(
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Supervisor",
                    NormalizedName = "SUPERVISOR",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Asesor",
                    NormalizedName = "ASESOR",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Auditor",
                    NormalizedName = "AUDITOR",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    Name = "Otro",
                    NormalizedName = "OTRO",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            );
        }
    }
}
