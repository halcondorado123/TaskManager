using AutoMapper;
using Moq;
using System.Linq.Expressions;
using TaskManager.Application.DTO.ViewModel;
using TaskManager.Application.Service;
using TaskManager.Domain.Entities.Models;
using TaskManager.Domain.Interface;

namespace TaskManager.Testing.Test.Services
{
    public class AppServiceTests
    {
        private readonly Mock<IEntityDomain<TaskItemME>> _domainMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AppService<TaskItemME, object, TaskItemVM> _service;

        public AppServiceTests()
        {
            _domainMock = new Mock<IEntityDomain<TaskItemME>>();
            _mapperMock = new Mock<IMapper>();

            _service = new AppService<TaskItemME, object, TaskItemVM>(_domainMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedResult()
        {
            // Arrange
            int page = 1;
            int pageSize = 2;

            var entities = new List<TaskItemME>
        {
            new TaskItemME { Id = 1, Title = "Task 1", Description = "Desc 1", StatusId = 1, Status = new TaskStatusME { Name = "Pending" }, DueDate = DateTime.UtcNow },
            new TaskItemME { Id = 2, Title = "Task 2", Description = "Desc 2", StatusId = 1, Status = new TaskStatusME { Name = "Pending" }, DueDate = DateTime.UtcNow },
            new TaskItemME { Id = 3, Title = "Task 3", Description = "Desc 3", StatusId = 1, Status = new TaskStatusME { Name = "Pending" }, DueDate = DateTime.UtcNow }
        };

            int totalCount = entities.Count;

            // Mock del dominio
            _domainMock
                .Setup(d => d.GetAllAsync(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<TaskItemME, object>>[]>()
                ))
                .ReturnsAsync((int page, int pageSize, Expression<Func<TaskItemME, object>>[] includes) =>
                {
                    var pagedEntities = entities.Skip((page - 1) * pageSize).Take(pageSize);
                    return (pagedEntities, entities.Count);
                });


            // Mock del mapper
            _mapperMock
                .Setup(m => m.Map<IEnumerable<TaskItemVM>>(It.IsAny<IEnumerable<TaskItemME>>()))
                .Returns((IEnumerable<TaskItemME> source) =>
                    source.Select(t => new TaskItemVM
                    {
                        Id = t.Id,
                        Title = t.Title,
                        StatusId = t.StatusId,
                        StatusName = t.Status.Name
                    }).ToList()
                );

            // Act
            var result = await _service.GetAllAsync(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(page, result.Page);
            Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(pageSize, result.Items.Count()); // Solo los primeros "pageSize" elementos
            Assert.Equal("Task 1", result.Items.First().Title);
            Assert.Equal("Pending", result.Items.First().StatusName);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new TaskItemME { Id = 1, Title = "Test", DueDate = DateTime.UtcNow, StatusId = 1, Status = new TaskStatusME { Id = 1, Name = "Open" } };
            _domainMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<TaskItemME>(entity)).Returns(entity);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnMappedEntity_WhenAddedSuccessfully()
        {
            // Arrange
            var entity = new TaskItemME { Id = 1, Title = "New Task", DueDate = DateTime.UtcNow, StatusId = 1 };
            _domainMock.Setup(d => d.AddAsync(entity)).ReturnsAsync(entity);
            _mapperMock.Setup(m => m.Map<TaskItemME>(entity)).Returns(entity);

            // Act
            var result = await _service.CreateAsync(entity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Task", result.Title);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnMappedEntity_WhenUpdatedSuccessfully()
        {
            // Arrange
            var dto = new TaskItemME
            {
                Title = "Task",
                DueDate = DateTime.UtcNow,
                StatusId = 1
            };

            var entityFromDb = new TaskItemME
            {
                Id = 1,
                Title = "Task",
                DueDate = DateTime.UtcNow,
                StatusId = 1
            };

            var updatedEntity = new TaskItemME
            {
                Id = 1,
                Title = "Updated Task",
                DueDate = DateTime.UtcNow,
                StatusId = 1
            };

            // Mock GetByIdAsync para traer la entidad
            _domainMock
                .Setup(d => d.GetByIdAsync(1))
                .ReturnsAsync(entityFromDb);

            // Mock del mapper que mapea DTO sobre la entidad
            _mapperMock
                .Setup(m => m.Map(dto, entityFromDb));

            // Mock del dominio UpdateAsync
            _domainMock
                .Setup(d => d.UpdateAsync(entityFromDb))
                .ReturnsAsync(updatedEntity);

            // Mock del mapper que convierte a ViewModel
            _mapperMock
                .Setup(m => m.Map<TaskItemME>(It.IsAny<TaskItemME>()))
                .Returns((TaskItemME e) => new TaskItemME
                {
                    Id = e.Id,
                    Title = "Updated Task",
                    DueDate = e.DueDate,
                    StatusId = e.StatusId
                });

            // Act
            var result = await _service.UpdateAsync(dto, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Task", result.Title);
        }



        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDomainReturnsTrue()
        {
            // Arrange
            _domainMock.Setup(d => d.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }
    }
}
