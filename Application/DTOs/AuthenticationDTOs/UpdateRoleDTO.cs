namespace Application.DTOs.AuthenticationDTOs;

public class UpdateRoleDTO
{
    public Guid RoleId { get; set; }
    public string NewRoleName { get; set; }
}