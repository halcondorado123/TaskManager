using Microsoft.EntityFrameworkCore;
using TaskManager.Infraestructure.Data;

namespace TaskManager.Testing.Test.Helpers
{
    public class DbContextHelper
    {
        public static TaskManagerDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var fakeProvider = new FakeContextDefaultProvider();

            return new TaskManagerDbContext(options, fakeProvider);
        }
    }
}
