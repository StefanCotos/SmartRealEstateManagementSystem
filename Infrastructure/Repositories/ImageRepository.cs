using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext context;
        public ImageRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Guid>> AddAsync(Image image)
        {
            try
            {
                await context.Images.AddAsync(image);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(image.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task<Result<Guid>> DeleteAsync(Guid id)
        {
            var image = await context.Images.FindAsync(id);
            if (image == null)
            {
                return Result<Guid>.Failure("Image not found");
            }
            context.Images.Remove(image);
            await context.SaveChangesAsync();
            return Result<Guid>.Success(id);
        }

        public async Task<IEnumerable<Image>> GetAllAsync()
        {
            return await context.Images.ToListAsync();
        }

        public async Task<Image?> GetByIdAsync(Guid id)
        {
            return await context.Images.FindAsync(id);
        }

        public async Task<Result<Guid>> UpdateAsync(Image image)
        {
            try
            {
                context.Entry(image).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Result<Guid>.Success(image.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task<IEnumerable<Image>> GetAllImagesByEstateIdAsync(Guid estateId)
        {
            return await context.Images.Where(x => x.EstateId == estateId).ToListAsync();
        }
    }
}