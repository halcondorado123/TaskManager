namespace TaskManager.Application.DTO.DTO
{
    public class TaskItemDTO
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
    }
}
