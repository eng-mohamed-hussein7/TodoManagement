namespace Application.DTOs.AuthenticationDTOs;

public class GetUserDto
{
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public string? Username { get; set; }
    public bool? IsDeleted { get; set; }
    public List<string>? Roles { get; set; }
}
