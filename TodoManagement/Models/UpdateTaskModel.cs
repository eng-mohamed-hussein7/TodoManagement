namespace TodoManagement.Models;

public class UpdateTaskModel
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public DateTime? DueDate { get; set; }
}
