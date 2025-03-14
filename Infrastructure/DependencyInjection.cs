using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IEstateRepository, EstateRepository>();
            services.AddScoped<IGenericEntityRepository<Estate>, EstateRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddScoped<IGenericEntityRepository<Image>, ImageRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IGenericEntityRepository<Favorite>, FavoriteRepository>();
            services.AddScoped<IGenericEntityRepository<User>, UserRepository>();
            services.AddScoped<IGenericEntityRepository<Report>, ReportRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IGenericEntityRepository<Review>, ReviewRepository>();

            return services;
        }
    }
}
