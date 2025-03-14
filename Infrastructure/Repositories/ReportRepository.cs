using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReportRepository : IGenericEntityRepository<Report>
    {
        private readonly ApplicationDbContext context;
        public ReportRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<Result<Guid>> AddAsync(Report report)
        {
            try
            {
                await context.Reports.AddAsync(report);
                await context.SaveChangesAsync();
                return Result<Guid>.Success(report.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure(ex.InnerException!.ToString());
            }
        }

        public async Task<Result<Guid>> DeleteAsync(Guid id)
        {
            var report = await context.Reports.FindAsync(id);
            if (report == null)
            {
                return Result<Guid>.Failure("Report not found");
            }
            context.Reports.Remove(report);
            await context.SaveChangesAsync();
            return Result<Guid>.Success(id);
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await context.Reports.ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(Guid id)
        {
            return await context.Reports.FindAsync(id);
            throw new NotImplementedException();
        }

        public Task<Result<Guid>> UpdateAsync(Report entity)
        {
            throw new NotImplementedException();
        }
    }
}
