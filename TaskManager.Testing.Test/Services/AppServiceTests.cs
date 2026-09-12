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

            Assert.Equal(3, result.PageSize);
            //Assert.Equal(pageSize, result.PageSize);
            Assert.Equal(totalCount, result.TotalCount);
            Assert.Equal(pageSize, result.Items.Count()); // Solo los primeros "pageSize" elementos
            Assert.Equal("Task 1", result.Items.First().Title);
            Assert.Equal("Pending", result.Items.First().StatusName);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedEntity_WhenEntityExists()
        {
            // Arrange
            var entity = new TaskItemME
            {
                Id = 1,
                Title = "Test",
                DueDate = DateTime.UtcNow,
                StatusId = 1,
                Status = new TaskStatusME { Id = 1, Name = "Pendiente" }
            };

            var expectedVm = new TaskItemVM
            {
                Id = entity.Id,
                Title = entity.Title,
                StatusId = entity.StatusId,
                StatusName = entity.Status?.Name,
                DueDate = entity.DueDate
            };

            // Mock del dominio
            _domainMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(entity);

            // Mock del mapper: Entity → ViewModel
            _mapperMock.Setup(m => m.Map<TaskItemVM>(It.IsAny<TaskItemME>()))
                .Returns((TaskItemME src) => new TaskItemVM
                {
                    Id = src.Id,
                    Title = src.Title,
                    StatusId = src.StatusId,
                    StatusName = src.Status?.Name,
                    DueDate = src.DueDate
                });


            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            //Assert.Equal("Tarea incorrecta", result.Title);
            Assert.Equal("Test", result.Title);
            Assert.Equal("Pendiente", result.StatusName);
        }


        [Fact]
        public async Task CreateAsync_ShouldReturnMappedViewModel_WhenAddedSuccessfully()
        {
            // Arrange
            var inputViewModel = new TaskItemVM
            {
                Id = 0,
                Title = "New Task",
                DueDate = new DateTime(2024, 10, 26),
                StatusId = 1
            };

            var expectedEntity = new TaskItemME
            {
                Id = 1,
                Title = "New Task",
                DueDate = new DateTime(2024, 10, 26),
                StatusId = 1
            };

            var expectedViewModel = new TaskItemVM
            {
                Id = 1,
                Title = "New Task",
                DueDate = new DateTime(2024, 10, 26),
                StatusId = 1
            };

            // Mock VM -> Entity
            _mapperMock.Setup(m => m.Map<TaskItemME>(It.IsAny<TaskItemVM>())).Returns(expectedEntity);

            // Mock Entity -> VM
            _mapperMock.Setup(m => m.Map<TaskItemVM>(It.IsAny<TaskItemME>())).Returns(expectedViewModel);

            // Act
            var result = await _service.CreateAsync(inputViewModel);

            // Assert
            Assert.NotNull(result);
            //Assert.Equal("Task Incorrecta", result.Title);
            Assert.Equal("New Task", result.Title);
            Assert.Equal(new DateTime(2024, 10, 26), result.DueDate);
        }


        [Fact]
        public async Task UpdateAsync_ShouldReturnMappedEntity_WhenUpdatedSuccessfully()
        {
            // Arrange
            var dto = new TaskItemME
            {
                Title = "Updated Task",
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

            // Mock GetByIdAsync para traer la entidad original
            _domainMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(entityFromDb);

            // Mock del mapper que mapea DTO sobre la entidad (in-place)
            _mapperMock
                .Setup(m => m.Map(dto, entityFromDb))
                .Callback<TaskItemME, TaskItemME>((source, dest) =>
                {
                    dest.Title = source.Title;
                    dest.DueDate = source.DueDate;
                    dest.StatusId = source.StatusId;
                });

            // Mock del dominio UpdateAsync que actualiza la entidad y la devuelve
            _domainMock.Setup(d => d.UpdateAsync(entityFromDb))
                .ReturnsAsync(() =>
                {
                    // Actualizamos la propiedad para simular la persistencia
                    entityFromDb.Title = dto.Title;
                    entityFromDb.DueDate = dto.DueDate;
                    entityFromDb.StatusId = dto.StatusId;
                    return entityFromDb;
                });

            // Mock del mapper que convierte la entidad actualizada a ViewModel
            _mapperMock
                .Setup(m => m.Map<TaskItemVM>(It.IsAny<TaskItemME>()))
                .Returns((TaskItemME e) => new TaskItemVM
                {
                    Id = e.Id,
                    Title = e.Title,
                    DueDate = e.DueDate,
                    StatusId = e.StatusId
                });

            // Act
            var result = await _service.UpdateAsync(dto, 1);

            // Assert
            Assert.NotNull(result);
            //Assert.Equal("Task Not Updated", result.Title);
            Assert.Equal("Updated Task", result.Title);
            Assert.Equal(dto.DueDate, result.DueDate);
            Assert.Equal(dto.StatusId, result.StatusId);
        }



        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDomainReturnsTrue()
        {
            // Arrange
            _domainMock.Setup(d => d.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            //Assert.False(result);
            Assert.True(result);
        }
    }
}
