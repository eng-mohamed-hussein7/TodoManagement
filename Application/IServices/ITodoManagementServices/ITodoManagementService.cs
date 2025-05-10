using Application.Common;
using Application.DTOs.TodoManagementDTOs;

namespace Application.IServices.ITodoManagementServices;

public interface ITodoManagementService
{
    Task<Result?> GetByIdAsync(Guid Id);
    Task<Result> GetAllAsync();
    Task<Result> CountAsync();
    Task<Result> CreateAsync(CreateTodoManagementDto model);
    Task<Result> UpdateAsync(UpdateTodoManagementDto model);
    Task<Result> DeleteAsync(Guid Id);
}
