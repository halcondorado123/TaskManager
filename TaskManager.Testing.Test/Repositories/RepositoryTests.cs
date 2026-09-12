using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities.Models;
using TaskManager.Infraestructure.Data;
using TaskManager.Infraestructure.Repository;
using TaskManager.Testing.Test.Helpers;

namespace TaskManager.Testing.Test.Repositories
{
    public class RepositoryTests
    {
        private TaskManagerDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var fakeProvider = new FakeContextDefaultProvider();

            return new TaskManagerDbContext(options, fakeProvider);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedEntities()
        {
            // Arrange
            var context = GetDbContext();
            var repo = new ReadRepository<TaskItemME>(context);

            // Crear 5 entidades de ejemplo
            var entities = new List<TaskItemME>
            {
                new TaskItemME { Id = 1, Title = "Task 1", DueDate = DateTime.UtcNow, StatusId = 1 },
                new TaskItemME { Id = 2, Title = "Task 2", DueDate = DateTime.UtcNow,StatusId = 1 },
                new TaskItemME { Id = 3, Title = "Task 3", DueDate = DateTime.UtcNow,StatusId = 1 },
                new TaskItemME { Id = 4, Title = "Task 4", DueDate = DateTime.UtcNow,StatusId = 1 },
                new TaskItemME { Id = 5, Title = "Task 5", DueDate = DateTime.UtcNow,StatusId = 1 }
            };

            context.AddRange(entities);
            await context.SaveChangesAsync();

            int page = 2;
            int pageSize = 2;

            // Act
            var (pagedEntities, totalCount) = await repo.GetAllAsync(page, pageSize);

            // Assert
            Assert.Equal(5, totalCount); // total de registros
            //Assert.Equal(6, totalCount); // total de registros
            Assert.Equal(pageSize, pagedEntities.Count()); // cantidad devuelta por página
            Assert.Equal("Task 3", pagedEntities.First().Title); // primer elemento de la página 2
            Assert.Equal("Task 4", pagedEntities.Last().Title);  // último elemento de la página 2
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity()
        {
            var context = GetDbContext();
            var repo = new ReadRepository<TaskItemME>(context);

            var entity = new TaskItemME { Id = 1, Title = "Task", DueDate = DateTime.UtcNow, StatusId = 1, Status = new TaskStatusME { Id = 1, Name = "Open" } };
            context.Add(entity);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Task", result.Title);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity()
        {
            var context = GetDbContext();
            var repo = new WriteRepository<TaskItemME>(context);

            var entity = new TaskItemME { Id = 1, Title = "New Task", DueDate = DateTime.UtcNow, StatusId = 1, Status = new TaskStatusME { Id = 1, Name = "Open" } };

            await repo.AddAsync(entity);

            //Assert.Equal(2, context.Set<TaskItemME>().Count());
            Assert.Single(context.Set<TaskItemME>());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyEntity()
        {
            var context = GetDbContext();
            var repo = new WriteRepository<TaskItemME>(context);

            var entity = new TaskItemME { Id = 1, Title = "Task", DueDate = DateTime.UtcNow, StatusId = 2 };
            context.Add(entity);
            await context.SaveChangesAsync();

            entity.Title = "Updated Task";
            await repo.UpdateAsync(entity);

            var updated = await context.Set<TaskItemME>().FindAsync(1);
            //Assert.Equal("Failed Task", updated.Title);
            Assert.Equal("Updated Task", updated.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            var context = GetDbContext();
            var repo = new WriteRepository<TaskItemME>(context);

            var entity = new TaskItemME { Id = 1, Title = "Task", DueDate = DateTime.UtcNow, StatusId = 1 };
            context.Add(entity);

            await repo.DeleteAsync(entity);
            await context.SaveChangesAsync();

            var deleted = await context.Set<TaskItemME>().FindAsync(1);
            //Assert.NotNull(deleted);
            Assert.Null(deleted);
        }
    }
}