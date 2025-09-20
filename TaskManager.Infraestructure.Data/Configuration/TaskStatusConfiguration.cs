using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities.Constants;
using TaskManager.Domain.Entities.Models;
using TaskManager.Infraestructure.Data.EntityConfigurations.SeedConfiguration;

namespace TaskManager.Infraestructure.Data.Configuration
{
    public class TaskStatusConfiguration : IEntityTypeConfiguration<TaskStatusME>
    {
        public void Configure(EntityTypeBuilder<TaskStatusME> builder)
        {
            builder.ToTable("TaskStatus", TaskContextSchemas.TaskDataManagment);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            builder.Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();

            // 🔹 Seed Data
            builder.HasData(
                new TaskStatusME 
                {
                    Id = 1,
                    Name = "Pendiente",
                    CreatedAt = SeedDefaults.CreationDate,
                    CreatedBy = SeedDefaults.DefaultUserId,
                    Active = true
                },
                new TaskStatusME 
                { 
                    Id = 2, 
                    Name = "En Progreso",
                    CreatedAt = SeedDefaults.CreationDate,
                    CreatedBy = SeedDefaults.DefaultUserId,
                    Active = true
                },
                new TaskStatusME 
                {
                    Id = 3,
                    Name = "Completada",
                    CreatedAt = SeedDefaults.CreationDate,
                    CreatedBy = SeedDefaults.DefaultUserId,
                    Active = true
                }
            );
        }
    }
}