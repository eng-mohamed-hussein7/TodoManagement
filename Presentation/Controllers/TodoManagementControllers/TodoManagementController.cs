using Application.DTOs.TodoManagementDTOs;
using Application.IServices.ITodoManagementServices;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.TodoManagementControllers;

[Route("api/[controller]")]
[ApiController]
public class TodoManagementController : ControllerBase
{
    private readonly ITodoManagementService _todoManagementService;

    public TodoManagementController(ITodoManagementService todoManagementService)
    {
        _todoManagementService = todoManagementService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateTodoManagementDto Dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _todoManagementService.CreateAsync(Dto);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
    [HttpGet("CountAsync")]
    public async Task<IActionResult> CountAsync()
    {
        var result = await _todoManagementService.CountAsync();
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("GetById")]
    public async Task<IActionResult> GetById(Guid Id)
    {
        if (Id.ToString() is null)
        {
            return NotFound();
        }
        var result = await _todoManagementService.GetByIdAsync(Id);
        return Ok(result);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _todoManagementService.GetAllAsync();
        return Ok(result);
    }

    [HttpPut("Edit")]
    public async Task<IActionResult> Edit([FromBody] UpdateTodoManagementDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _todoManagementService.UpdateAsync(model);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(Guid Id)
    {
        if (Id.ToString() is null)
        {
            return NotFound();
        }
        var result = await _todoManagementService.DeleteAsync(Id);
        if (!result.Succeeded)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
