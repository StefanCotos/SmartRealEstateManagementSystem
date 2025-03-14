using Domain.Entities;

namespace Domain.Repositories
{
    public interface IReviewRepository : IGenericEntityRepository<Review>
    {
        Task<IEnumerable<Review>> GetAllBuyerReviewAsync(Guid buyerId);
        Task<IEnumerable<Review>> GetAllSellerReviewAsync(Guid sellerId);
    }

}
