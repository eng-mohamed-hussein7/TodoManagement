using Application.Common;
using Application.DTOs.AuthenticationDTOs;
using Application.IServices.IAuthenticationServices;
using Application.IServices.IEmailServices;
using Domain.Entities.ApplicationEntities;
using Infrastructure.Services.EmailServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services.AuthenticationServices;

public class AuthService: IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly JwtSettings _jwt;
    private readonly IEmailService _emailService;

    public AuthService(IEmailService emailService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IOptions<JwtSettings> jwt)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwt = jwt.Value;
        _emailService = emailService;

    }


    public async Task<Result> GetAllUsers()
    {
        var users = _userManager.Users.Where(i => i.IsDeleted == false).ToList();

        if (!users.Any())
        {
            return Result.Failure(StatusResult.Falid, "No users found.");
        }

        var userDtos = new List<GetUserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var sortedRoles = roles.OrderByDescending(r => r == "Admin").ToList();

            userDtos.Add(new GetUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.PhoneNumber,
                Username = user.UserName,
                IsDeleted = user.IsDeleted,
                Roles = sortedRoles
            });
        }

        return Result.Success("Users retrieved successfully!", null, userDtos);
    }

    public async Task<Result> GetAllAdmins()
    {
        var adminUsers = new List<GetUserDto>();

        var users = _userManager.Users.Where(i => i.IsDeleted == false).ToList();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                adminUsers.Add(new GetUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.PhoneNumber,
                    Username = user.UserName,
                    IsDeleted = user.IsDeleted,
                    Roles = roles.ToList()
                });
            }
        }

        if (!adminUsers.Any())
        {
            return Result.Failure(StatusResult.Falid, "No Admin users found.");
        }

        return Result.Success("Admin users retrieved successfully!", null, adminUsers);
    }

    public async Task<Result> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        var user = await _userManager.FindByIdAsync(updateUserDto.Id.ToString());

        if (user == null)
        {
            return Result.Failure(StatusResult.Falid, "User not found.");
        }

        user.Email = updateUserDto.Email;
        user.UserName = updateUserDto.Username;
        user.PhoneNumber = updateUserDto.Phone;
        user.FullName = updateUserDto.FullName;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            return Result.Failure(StatusResult.Falid, "Failed to update user.");
        }

        var existingRoles = await _userManager.GetRolesAsync(user);
        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);

        if (!removeRolesResult.Succeeded)
        {
            return Result.Failure(StatusResult.Falid, "Failed to remove existing roles.");
        }

        foreach (var role in updateUserDto.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return Result.Failure(StatusResult.Falid, $"Role '{role}' does not exist.");
            }
        }
        var addRolesResult = await _userManager.AddToRolesAsync(user, updateUserDto.Roles);

        if (!addRolesResult.Succeeded)
        {
            return Result.Failure(StatusResult.Falid, "Failed to update roles.");
        }

        return Result.Success("User updated successfully!");
    }

    public async Task<Result> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return Result.Failure(StatusResult.Falid, "User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userDto = new GetUserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Phone = user.PhoneNumber,
            Email = user.Email,
            Username = user.UserName,
            Roles = roles.ToList()
        };

        return Result.Success("User retrieved successfully!", null, userDto);
    }
    public async Task<Result> SoftDeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return Result.Failure(StatusResult.Falid, "User not found.");
        }

        user.IsDeleted = true;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure(StatusResult.Falid, "Failed to soft delete the user.");
        }

        return Result.Success("User soft deleted successfully!");
    }


    public async Task<Result> Login(LoginDto loginDTO)
    {
        if (string.IsNullOrEmpty(loginDTO.Username))
        {
            return Result.Failure(StatusResult.Falid, "Email is required.");
        }

        if (string.IsNullOrEmpty(loginDTO.Password))
        {
            return Result.Failure(StatusResult.Falid, "Password is required.");
        }

        var user = await _userManager.FindByEmailAsync(loginDTO.Username);
        if (user == null)
        {
            return Result.Failure(StatusResult.Falid, "Invalid email or password.");
        }

        if (user.IsDeleted)
        {
            return Result.Failure(StatusResult.Falid, "This account has been deactivated. Please contact support.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
        if (!passwordValid)
        {
            return Result.Failure(StatusResult.Falid, "Invalid email or password.");
        }

        var jwtSecurityToken = await CreateJwtToken(user);

        var rolesList = await _userManager.GetRolesAsync(user);

        var authModel = new TokenResponceDTo
        {
            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Email = user.Email,
            Username = user.UserName,
            ExpiresOn = jwtSecurityToken.ValidTo,
            Roles = rolesList.ToList()
        };

        return Result.Success("User logged in successfully!", null, authModel);
    }


    private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();

        foreach (var role in roles)
            roleClaims.Add(new Claim("roles", role));

        var claims = new[]
        {
         new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
         new Claim(JwtRegisteredClaimNames.Email, user.Email),
         new Claim("uid", user.Id.ToString())
     }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.Now.AddDays(_jwt.ExpiryInMinutes),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }

    public async Task<Result> Register(RegisterDto model)
    {
        if (string.IsNullOrEmpty(model.Email))
        {
            return Result.Failure(StatusResult.Falid, "Email is required.");
        }

        var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
        if (existingUserByEmail != null)
        {
            return Result.Failure(StatusResult.Falid, "Email is already in use.");
        }

        if (string.IsNullOrEmpty(model.FullName))
        {
            return Result.Failure(StatusResult.Falid, "Full name is required.");
        }

        var existingUserByName = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == model.FullName);
        if (existingUserByName != null)
        {
            return Result.Failure(StatusResult.Falid, "Full name is already in use.");
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            return Result.Failure(StatusResult.Falid, "Password is required.");
        }

        foreach (var role in model.Roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                return Result.Failure(StatusResult.Falid, $"Role '{role}' does not exist.");
            }
        }

        var user = new ApplicationUser
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.Phone,
            FullName = model.FullName,
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return Result.Failure(StatusResult.Falid, "User creation failed.");
        }

        foreach (var role in model.Roles)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        var jwtSecurityToken = await CreateJwtToken(user);

        var authViewModel = new TokenResponceDTo
        {
            Email = user.Email,
            ExpiresOn = jwtSecurityToken.ValidTo,
            IsAuthenticated = true,
            Roles = model.Roles.ToList(),
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Username = user.UserName
        };

        return Result.Success("User registered successfully!", null, authViewModel);
    }

    //=========================UpdatePassword=========//
    public async Task<Result> UpdatePassword(string userId, UpdatePasswordDto model)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return Result.Failure(StatusResult.Falid, "User not found or not authorized.");
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return Result.Failure(StatusResult.Falid, "Password update failed.", data: errors);
        }

        return Result.Success("Password updated successfully.");
    }
    //=========================ForgotPassword || ResetPassword=========//
    public async Task<Result> ForgotPassword(ForgotPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Result.Failure(StatusResult.Falid, "المستخدم غير موجود", null);

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://your-frontend-app.com/ResetPassword/Index?email={model.Email}&token={Uri.EscapeDataString(resetToken)}";

        var emailBody = EmailTemplates.GetResetPasswordTemplate(resetLink);

        await _emailService.SendEmailAsync(
            model.Email,
            "إعادة تعيين كلمة المرور",
            emailBody
        );

        return Result.Success("Reset email sent successfully", null);
    }
    public async Task<Result> ResetPassword(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Result.Failure(StatusResult.Falid, "User not found", null);

        var decodedToken = Uri.UnescapeDataString(model.Token);

        var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
        if (resetResult.Succeeded)
        {
            return Result.Success("Password reset successfully");
        }

        var errors = string.Join(", ", resetResult.Errors.Select(e => e.Description));
        return Result.Failure(StatusResult.Falid, errors, null);
    }


    //=============== Service Auth =================//
    public async Task<bool> CreateRoleAsync(CreateRoleDTO createRoleDTO)
    {
        if (await _roleManager.RoleExistsAsync(createRoleDTO.RoleName))
            return false;

        var role = new ApplicationRole
        {
            Name = createRoleDTO.RoleName
        };

        var result = await _roleManager.CreateAsync(role);

        return result.Succeeded;
    }
    public async Task<List<RoleDetailsDTO>> GetRolesAsync()
    {
        var roles = _roleManager.Roles
            .Where(r => !r.IsDeleted)
            .Select(role => new RoleDetailsDTO
            {
                RoleId = role.Id,
                RoleName = role.Name
            }).ToList();

        return roles;
    }
    public async Task<bool> UpdateRoleAsync(UpdateRoleDTO updateRoleDTO)
    {
        var role = await _roleManager.FindByIdAsync(updateRoleDTO.RoleId.ToString());

        if (role == null)
            return false;

        var existingRole = await _roleManager.FindByNameAsync(updateRoleDTO.NewRoleName);

        if (existingRole != null && existingRole.Id != role.Id)
            return false;

        role.Name = updateRoleDTO.NewRoleName;

        var result = await _roleManager.UpdateAsync(role);

        return result.Succeeded;
    }



    public async Task<bool> DeleteRoleAsync(Guid id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
            return false;

        role.IsDeleted = true;
        var result = await _roleManager.UpdateAsync(role);

        return result.Succeeded;
    }

    public async Task<RoleDetailsDTO> GetByIdRoleAsync(Guid roleId)
    {
        var role = await _roleManager.Roles
            .Where(r => r.Id == roleId && !r.IsDeleted)
            .Select(r => new RoleDetailsDTO
            {
                RoleId = r.Id,
                RoleName = r.Name
            })
            .FirstOrDefaultAsync();

        return role;
    }
}
