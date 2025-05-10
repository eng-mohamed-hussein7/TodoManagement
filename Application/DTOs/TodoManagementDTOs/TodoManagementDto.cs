namespace Application.DTOs.TodoManagementDTOs;

public class TodoManagementDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public string? DueDate { get; set; }
    public string? CreatedDate { get; set; }
}
