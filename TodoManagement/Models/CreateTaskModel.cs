namespace TodoManagement.Models;

public class CreateTaskModel
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public int Status { get; set; }
    public int Priority { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
