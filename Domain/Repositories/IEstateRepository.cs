using Domain.Entities;

namespace Domain.Repositories
{
    public interface IEstateRepository : IGenericEntityRepository<Estate>
    {
        Task<IEnumerable<Estate>?> GetAllEstatesByUserIdAsync(Guid userId);
        Task<IEnumerable<Estate>?> GetAllEstatesByBuyerIdAsync(Guid buyerId);
    }
}
