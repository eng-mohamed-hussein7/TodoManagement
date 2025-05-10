namespace Application.DTOs.AuthenticationDTOs;

public class UpdatePasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
