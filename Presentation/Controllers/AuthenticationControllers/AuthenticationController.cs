using Application.Common;
using Application.DTOs.AuthenticationDTOs;
using Application.IServices.IAuthenticationServices;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.AuthenticationControllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    public readonly IAuthService _authenticationService;

    public AuthenticationController(IAuthService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Message = "Invalid input data",
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var result = await _authenticationService.Login(loginDto);

        if (!result.Succeeded)
        {
            return Unauthorized(new
            {
                Message = result.Message ?? "Invalid email or password.",
                Error = result.Error
            });
        }

        return Ok(new
        {
            Message = "Login successful!",
            Data = result.Data
        });
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }


        var result = await _authenticationService.Register(model);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("GetUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _authenticationService.GetAllUsers();

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpGet("GetAdmins")]
    public async Task<IActionResult> GetAllAdmins()
    {
        var result = await _authenticationService.GetAllAdmins();

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        var result = await _authenticationService.UpdateUserAsync(updateUserDto);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("Getuser/{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _authenticationService.GetUserByIdAsync(id);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> SoftDeleteUser(Guid id)
    {
        var result = await _authenticationService.SoftDeleteUserAsync(id);

        if (!result.Succeeded)
            return BadRequest(result);

        return Ok(result);
    }


    [HttpPost("Update-Password")]
    public async Task<IActionResult> UpdatePassword([FromQuery] string userId, [FromBody] UpdatePasswordDto model)
    {
        if (!ModelState.IsValid)
        {
            return Ok(Result.Failure(StatusResult.Falid, "Invalid input.", data: ModelState));
        }

        var result = await _authenticationService.UpdatePassword(userId, model);

        return Ok(result);
    }



    [HttpPost("Forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authenticationService.ForgotPassword(model);

        return Ok(result);
    }

    [HttpPost("Resetpassword")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authenticationService.ResetPassword(model);

        return Ok(result);
    }
    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO model)
    {
        if (!ModelState.IsValid)
        {
            return Ok(Result.Failure(StatusResult.Falid, "Invalid input.", data: ModelState));
        }

        var result = await _authenticationService.CreateRoleAsync(model);

        return Ok(result);
    }



    [HttpGet("GetRoles")]
    public async Task<IActionResult> GetRoles()
    {


        var result = await _authenticationService.GetRolesAsync();

        return Ok(result);
    }

    [HttpGet("GetByIdRoleAsync/{id}")]
    public async Task<IActionResult> GetByIdRoleAsync(Guid id)
    {
        var role = await _authenticationService.GetByIdRoleAsync(id);

        if (role == null)
        {
            return NotFound(new { Message = "Role not found." });
        }


        return Ok(role);
    }


    [HttpPut("UpdateRole")]
    public async Task<IActionResult> UpdateRole(UpdateRoleDTO model)
    {
        if (!ModelState.IsValid)
        {
            return Ok(Result.Failure(StatusResult.Falid, "Invalid input.", data: ModelState));
        }

        var result = await _authenticationService.UpdateRoleAsync(model);

        return Ok(result);
    }


    [HttpDelete("DeleteRole/{id}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        if (!ModelState.IsValid)
        {
            return Ok(Result.Failure(StatusResult.Falid, "Invalid input.", data: ModelState));
        }

        var result = await _authenticationService.DeleteRoleAsync(id);

        return Ok(result);
    }
}
