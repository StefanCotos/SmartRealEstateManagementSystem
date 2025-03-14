using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql("host=c7u1tn6bvvsodf.cluster-czz5s0kz4scl.eu-west-1.rds.amazonaws.com;port=5432;database=duv7cad7e8he4;userid=ua36clbn061p56;password=pcd36c81120367ce6b7dddf4e6a8903f0bac1ae0277d50f036d213612cb9edeeb"); // sau UseNpgsql pentru PostgreSQL

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }

}