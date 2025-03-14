using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext context;
        public FavoriteRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Guid>> AddAsync(Favorite favorite)
        {
            try
            {
                await context.Favorites.AddAsync(favorite);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(favorite.UserId);

            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task<Result<Guid>> DeleteAsync(Guid userId, Guid estateId)
        {
            var favorite = await context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.EstateId == estateId);
            if (favorite == null)
            {
                return Result<Guid>.Failure("Favorite not found");
            }
            context.Favorites.Remove(favorite);
            await context.SaveChangesAsync();
            return Result<Guid>.Success(userId);
        }

        public async Task<IEnumerable<Favorite>> GetAllFavoritesByUserIdAsync(Guid userId)
        {
            return await context.Favorites.Where(x => x.UserId == userId).ToListAsync();
        }

        public Task<IEnumerable<Favorite>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Favorite?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Guid>> UpdateAsync(Favorite favorite)
        {
            throw new NotImplementedException();
        }

        public Task<Result<Guid>> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
