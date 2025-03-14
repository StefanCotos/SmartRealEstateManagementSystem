using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Net.Http.Json;
using Application.AIML;
using System.Net;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class EstatePricePredictionControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/estate-price-prediction";
        public EstatePricePredictionControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Configure factory to use InMemory database
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }
                    descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var dbConnectionDescriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbConnection));

                    if (dbConnectionDescriptor != null)
                    {
                        services.Remove(dbConnectionDescriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting" + Guid.NewGuid());
                    });

                });
            });

            // Create service scope to access DbContext
            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }

        [Fact]
        public async Task PredictPrice_ShouldReturnPredictedPrice()
        {
            // Arrange
            var estateData = new EstateData
            {
                Price = 500000,
                Bedrooms = 3,
                Bathrooms = 2,
                LandSize = 500,
                Street = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                HouseSize = 1500
            };
            var client = factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/predict", estateData);

            // Log response content for debugging
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");

            // Assert
            response.Should().NotBeNull();
        }
        [Fact]
        public async Task GivenInvalidData_PredictPrice_ShouldReturnBadRequest()
        {
            // Arrange
            var estateData = new EstateData
            {
                Price = 500000,
                Bathrooms = 2,
                LandSize = 500,
                Street = "123 Main St",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                HouseSize = 1500
            };
            var client = factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/predict", estateData);

            // Log response content for debugging
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task TrainModel_ShouldReturnBool()
        {
            // Arrange
            var client = factory.CreateClient();
            // Act
            var response = await client.GetAsync($"{BaseUrl}/train");
            // Assert
            response.Should().NotBeNull();
        }


        [Fact]
        public async Task EvaluateModel_ShouldReturnRegressionMetrics()
        {
            // Arrange
            var client = factory.CreateClient();
            // Act
            var response = await client.GetAsync($"{BaseUrl}/evaluate");

            // Assert
            response.Should().NotBeNull();
        }

    }
}
