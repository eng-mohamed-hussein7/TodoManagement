using Application.Interfaces;
using Application.IRepositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Domain.Entities.TodoManagementEntities;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IRepository<TodoManagement> TodoManagements { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        TodoManagements = new Repository<TodoManagement>(_context);
    }
    public IRepository<T> GetRepository<T>() where T : class
    {
        return new Repository<T>(_context);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
    public int Complete()
    {
        return _context.SaveChanges();
    }
}
