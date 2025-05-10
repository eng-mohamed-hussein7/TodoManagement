using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.TodoManagementDTOs;

public class CreateTodoManagementDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    public string? Description { get; set; }
    public Status Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? DueDate { get; set; }
}
