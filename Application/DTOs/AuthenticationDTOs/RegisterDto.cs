namespace Application.DTOs.AuthenticationDTOs;

public class RegisterDto
{
    public string? FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Phone { get; set; }
    public string? Username { get; set; }
    public List<string> Roles { get; set; } = new List<string> { "User" };
}
