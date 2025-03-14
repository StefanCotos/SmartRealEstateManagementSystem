using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    {
        public UsersDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
            optionsBuilder.UseNpgsql("host=c9tiftt16dc3eo.cluster-czz5s0kz4scl.eu-west-1.rds.amazonaws.com;port=5432;database=db46i5giqc4tft;userid=ufn4t4pukqbe1u;password=p2f82caa044b13164bdb5fd6b792155ef2efaae9858e21599e9e646127b3b65f8"); // sau UseNpgsql pentru PostgreSQL

            return new UsersDbContext(optionsBuilder.Options);
        }
    }

}