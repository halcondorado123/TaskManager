using System.ComponentModel.DataAnnotations;

namespace TaskManager.Application.DTO.DTO
{
    public class TaskItemDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        public string? Title { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        public DateTime? DueDate { get; set; } = DateTime.UtcNow;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido")]
        public int StatusId { get; set; } = 0;
        public string? StatusName { get; set; }
    }
}
