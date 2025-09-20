namespace TaskManager.Domain.Entities.Models
{
    public class TaskItemME : Entity<int>
    {
        public required string Title { get; set; } = null!;
        public string? Description { get; set; }
        public required DateTime DueDate { get; set; }
        public required int StatusId { get; set; }
        public TaskStatusME Status { get; set; } = null!;
    }
}