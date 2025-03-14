using Domain.Common;

namespace Domain.Repositories
{
    public interface IGenericEntityRepository<T>
    {
        Task<Result<Guid>> AddAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<Result<Guid>> UpdateAsync(T entity);
        Task<Result<Guid>> DeleteAsync(Guid id);
    }
}
