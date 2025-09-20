using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities.Constants;
using TaskManager.Domain.Entities.Models;
using TaskManager.Infraestructure.Data.EntityConfigurations.SeedConfiguration;

namespace TaskManager.Infraestructure.Data.Configuration
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItemME>
    {
        public void Configure(EntityTypeBuilder<TaskItemME> builder)
        {
            builder.ToTable("TaskItem", TaskContextSchemas.TaskDataManagment);

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
               .UseIdentityColumn(1, 1); 

            builder.Property(b => b.Title)
                .HasMaxLength(100);

            builder.Property(b => b.Description)
                .HasMaxLength(250);

            builder.Property(b => b.DueDate);

            builder.Property(b => b.StatusId);

            builder.HasOne(b => b.Status)
                .WithMany(S => S.Tasks)
                .HasForeignKey(b => b.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
               new TaskItemME
               {
                   Id = 1,
                   Title = "Configurar arquitectura base",
                   Description = "Levantar proyecto con Clean Architecture",
                   DueDate = SeedDefaults.CreationDate.AddDays(7),
                   StatusId = 1, // Pendiente
                   CreatedAt = SeedDefaults.CreationDate,
                   CreatedBy = SeedDefaults.DefaultUserId,
                   Active = true,
               },
               new TaskItemME
               {
                   Id = 2,
                   Title = "Implementar auditoría",
                   Description = "Configurar SaveChanges con ContextDefaultProvider",
                   DueDate = SeedDefaults.CreationDate.AddDays(10),
                   StatusId = 2, // En progreso
                   CreatedAt = SeedDefaults.CreationDate,
                   CreatedBy = SeedDefaults.DefaultUserId,
                   Active = true,
               },
               new TaskItemME
               {
                   Id = 3,
                   Title = "Crear UI en Blazor",
                   Description = "Pantalla para listar y gestionar tareas",
                   DueDate = SeedDefaults.CreationDate.AddDays(15),
                   StatusId = 3, // Completada
                   CreatedAt = SeedDefaults.CreationDate,
                   CreatedBy = SeedDefaults.DefaultUserId,
                   Active = true,
               }
           );

        }

    }
}
