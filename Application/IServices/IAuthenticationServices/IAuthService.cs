using Application.Common;
using Application.DTOs.AuthenticationDTOs;

namespace Application.IServices.IAuthenticationServices;

public interface IAuthService
{
    Task<Result> Login(LoginDto loginDto);
    Task<Result> Register(RegisterDto model);
    Task<Result> GetAllUsers();
    Task<Result> GetAllAdmins();
    Task<Result> UpdatePassword(string userId, UpdatePasswordDto model);
    Task<Result> UpdateUserAsync(UpdateUserDto updateUserDto);
    Task<Result> SoftDeleteUserAsync(Guid userId);
    Task<Result> GetUserByIdAsync(Guid userId);
    Task<Result> ForgotPassword(ForgotPasswordDto model);
    Task<Result> ResetPassword(ResetPasswordDto model);

    //==========Role============//
    Task<bool> CreateRoleAsync(CreateRoleDTO createRoleDTO);
    Task<List<RoleDetailsDTO>> GetRolesAsync();
    Task<bool> UpdateRoleAsync(UpdateRoleDTO updateRoleDTO);
    Task<bool> DeleteRoleAsync(Guid roleId);
    Task<RoleDetailsDTO> GetByIdRoleAsync(Guid roleId);
}
