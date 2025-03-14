using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext context;
        public ReviewRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Guid>> AddAsync(Review reviewUser)
        {
            try
            {
                await context.ReviewUsers.AddAsync(reviewUser);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(reviewUser.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public Task<Result<Guid>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await context.ReviewUsers.ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await context.ReviewUsers.FindAsync(id);
        }

        public Task<Result<Guid>> UpdateAsync(Review reviewUser)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<Review>> GetAllBuyerReviewAsync(Guid buyerId)
        {
            return await context.ReviewUsers.Where(x => x.BuyerId == buyerId).ToListAsync();
        }
        public async Task<IEnumerable<Review>> GetAllSellerReviewAsync(Guid sellerId)
        {
            return await context.ReviewUsers.Where(x => x.SellerId == sellerId).ToListAsync();
        }
    }
}
