using Core.Entities;
using System.Linq.Expressions;
namespace Core.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id, bool noTracking = true);
    Task<IEnumerable<T>> GetAllAsync(bool noTracking = true);
    IEnumerable<T> Find(Expression<Func<T, bool>> expression, bool noTracking = true);
    
    Task<(int totalRecords, IEnumerable<T> records)> GetAllAsync(int pageIndex, int pageSize, string search, bool noTracking = true);

    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    void Update(T entity);
}


