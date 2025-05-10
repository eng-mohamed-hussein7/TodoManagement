namespace Application.DTOs.AuthenticationDTOs;

public class TokenResponceDTo
{
    public string Email { get; set; }
    public DateTime ExpiresOn { get; set; }
    public bool? IsAuthenticated { get; set; }
    public List<string> Roles { get; set; } = new List<string> { "User" };
    public string? Token { get; set; }
    public string? Username { get; set; }
}
