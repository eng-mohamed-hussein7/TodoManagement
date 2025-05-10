using Application.IRepositories;
using Domain.Entities.TodoManagementEntities;
namespace Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<TodoManagement> TodoManagements { get; }
    IRepository<T> GetRepository<T>() where T : class;  
    int Complete();
    Task<int> CompleteAsync();
}