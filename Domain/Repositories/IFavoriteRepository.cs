using Domain.Common;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IFavoriteRepository : IGenericEntityRepository<Favorite>
    {
        Task<Result<Guid>> DeleteAsync(Guid userId, Guid estateId);
        Task<IEnumerable<Favorite>> GetAllFavoritesByUserIdAsync(Guid userId);
    }
}
