namespace TaskManager.Domain.Entities.Models
{
    public class TaskStatusME : Entity<int>
    {
        public required string Name { get; set; } = null!;
        public ICollection<TaskItemME> Tasks { get; set; } = new List<TaskItemME>();
    }
}