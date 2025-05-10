using Application.Common;
using Application.DTOs.TodoManagementDTOs;
using Application.Interfaces;
using Application.IServices.ITodoManagementServices;
using Domain.Entities.TodoManagementEntities;

namespace Infrastructure.Services.TodoManagementServices;

public class TodoManagementService : ITodoManagementService
{
    private readonly IUnitOfWork _unitOfWork;

    public TodoManagementService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> CreateAsync(CreateTodoManagementDto model)
    {
        try
        {
            var entity = new TodoManagement
            {
                CreatedDate = DateTime.Now,
                Description = model.Description,
                DueDate = model.DueDate,
                LastModifiedDate = DateTime.Now,
                Priority = model.Priority,
                Status = model.Status,
                Title = model.Title,
            };

            await _unitOfWork.TodoManagements.AddAsync(entity);
            var result = await _unitOfWork.CompleteAsync();

            return result == 0
                ? Result.Failure(StatusResult.Falid, "Failed to add new Entity.", null, null)
                : Result.Success("Entity added successfully.", null, null);
        }
        catch (Exception ex)
        {
            return Result.Failure(StatusResult.Falid, $"Failed to retrieve data: {ex.Message}", null);
        }
    }

    public async Task<Result> DeleteAsync(Guid Id)
    {
        try
        {
            var Entity = await _unitOfWork.TodoManagements.FindAsync(m=>m.Id == Id);
            if (Entity is null)
                return Result.Failure(StatusResult.Falid, $"Invalid ID: {Id}.", null);

            _unitOfWork.TodoManagements.Delete(Entity);
            
            var result = await _unitOfWork.CompleteAsync();

            return result == 0
                ? Result.Failure(StatusResult.Falid, "Failed to delete Entity.", null)
                : Result.Success("Entity deleted successfully.", null, null);
        }
        catch (Exception ex)
        {
            return Result.Failure(StatusResult.Falid, $"Failed to retrieve data: {ex.Message}", null);
        }
    }

    public async Task<Result> GetAllAsync()
    {
        try
        {
            var models = await _unitOfWork.TodoManagements.GetAllAsync();

            var list = models
                .Select(model => new TodoManagementDto
                {
                    Id = model.Id,
                    Description = model.Description,
                    DueDate = model.DueDate?.ToString("D"),
                    Priority = model.Priority.ToString(),
                    Status = model.Status.ToString(),
                    Title = model.Title,
                    CreatedDate = model.CreatedDate.ToString("D"),
                })
                .ToList();

            return list.Count == 0
                ? Result.Success("No data found.", null, list)
                : Result.Success("Data retrieved successfully.", null, list);
        }
        catch (Exception ex)
        {
            return Result.Failure(StatusResult.Falid, $"Failed to retrieve data: {ex.Message}", null);
        }
    }

    public async Task<Result?> GetByIdAsync(Guid Id)
    {
        if (Id.ToString() is null)
            return Result.Failure(StatusResult.Falid, $"Invalid ID: {Id}.", null);

        try
        {
            var model = await _unitOfWork.TodoManagements.FindAsync(d => d.Id == Id);
            var dto = new UpdateTodoManagementDto
            {
                Id = model.Id,
                Description = model.Description,
                DueDate = model.DueDate,
                Priority = model.Priority,
                Status = model.Status,
                Title = model.Title,
            };
            return model == null
                ? Result.Failure(StatusResult.Falid, "Data not found.", null)
                : Result.Success("Data retrieved successfully.", null, dto);
        }
        catch (Exception ex)
        {
            return Result.Failure(StatusResult.Falid, $"Failed to retrieve data: {ex.Message}", null);
        }
    }

    public async Task<Result> UpdateAsync(UpdateTodoManagementDto model)
    {
        try
        {
            if (model == null)
                return Result.Failure(StatusResult.Falid, "Invalid data.", null);

            var existingModel = await _unitOfWork.TodoManagements.FindAsync(c => c.Id == model.Id);
            if (existingModel == null)
                return Result.Failure(StatusResult.Falid, "Entity not found.", null);

            existingModel.Title = model.Title;
            existingModel.Description = model.Description;
            existingModel.DueDate = model.DueDate;
            existingModel.Priority = model.Priority;
            existingModel.Status = model.Status;
            existingModel.LastModifiedDate = DateTime.Now;

            var result = await _unitOfWork.CompleteAsync();

            return result == 0
                ? Result.Failure(StatusResult.Falid, "Failed to update data.", null)
                : Result.Success("Data updated successfully.", null, existingModel);
        }
        catch (Exception ex)
        {
            return Result.Failure(StatusResult.Falid, $"Failed to retrieve data: {ex.Message}", null);
        }
    }
    
    public async Task<Result> CountAsync()
    {
        try
        {
            var task = await _unitOfWork.TodoManagements.CountAsync();
            return Result.Success($"The Total Task is {task}", null, task);
        }
        catch (Exception ex)
        {
            return Result.Failure(StatusResult.Falid, $"Failed to retrieve data: {ex.Message}", null);
        }
    }
}
