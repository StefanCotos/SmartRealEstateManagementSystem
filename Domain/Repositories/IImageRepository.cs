using Domain.Entities;

namespace Domain.Repositories
{
    public interface IImageRepository : IGenericEntityRepository<Image>
    {
        Task<IEnumerable<Image>> GetAllImagesByEstateIdAsync(Guid estateId);
    }
}
