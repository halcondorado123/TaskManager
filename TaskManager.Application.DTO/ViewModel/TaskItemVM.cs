namespace TaskManager.Application.DTO.ViewModel
{
    public class TaskItemVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int StatusId {  get; set; }
        public string? StatusName { get; set; }
    }
}