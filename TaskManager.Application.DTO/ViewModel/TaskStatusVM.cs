namespace TaskManager.Application.DTO.ViewModel
{
    public class TaskStatusVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TaskItemVM>? Tasks { get; set; }
    }
}