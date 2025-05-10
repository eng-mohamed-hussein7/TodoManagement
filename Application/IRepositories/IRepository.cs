using System.Linq.Expressions;

namespace Application.IRepositories;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> FirstOrDefaultAsync();
    Task<T> FindAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> criteria);
    Task Update(T entity);
    void Delete(T entity);
    void DeleteRange(List<T> values);

    Task<T> GetByIdWithNestedIncludeAsync<TProperty, TThenProperty>(
    Guid id,
    Expression<Func<T, IEnumerable<TProperty>>> navigation,
    Expression<Func<TProperty, TThenProperty>> thenNavigation
) where TProperty : class;

    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
    Task<T> GetByIdWithIncludeAsync(Guid id, params Expression<Func<T, object>>[] includes);

    Task<T> GetByIdWithMultipleIncludesAsync(Guid id, params Expression<Func<T, object>>[] includes);


}