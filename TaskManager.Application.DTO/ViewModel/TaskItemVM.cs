namespace TaskManager.Application.DTO.ViewModel
{
    public class TaskItemVM
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int StatusId {  get; set; }
        public string? StatusName { get; set; }
        public bool IsEditing { get; set; } = false;
    }
}