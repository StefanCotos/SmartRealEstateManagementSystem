using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EstateRepository : IEstateRepository
    {
        private readonly ApplicationDbContext context;
        public EstateRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Guid>> AddAsync(Estate estate)
        {
            try
            {
                await context.Estates.AddAsync(estate);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(estate.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task<Result<Guid>> DeleteAsync(Guid id)
        {
            var estate = await context.Estates.FindAsync(id);
            if (estate == null)
            {
                return Result<Guid>.Failure("Estate not found");
            }
            context.Estates.Remove(estate);
            await context.SaveChangesAsync();
            return Result<Guid>.Success(id);
        }

        public async Task<IEnumerable<Estate>> GetAllAsync()
        {
            return await context.Estates.ToListAsync();
        }

        public async Task<IEnumerable<Estate>?> GetAllEstatesByUserIdAsync(Guid userId)
        {
            return await context.Estates.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Estate>?> GetAllEstatesByBuyerIdAsync(Guid buyerId)
        {
            return await context.Estates.Where(x => x.BuyerId == buyerId).ToListAsync();
        }

        public async Task<Estate?> GetByIdAsync(Guid id)
        {
            return await context.Estates.FindAsync(id);
        }

        public async Task<Result<Guid>> UpdateAsync(Estate estate)
        {
            try
            {
                context.Entry(estate).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Result<Guid>.Success(estate.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

    }
}
