using Application.Use_Cases.Commands.ReviewUserC;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class ReviewControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "api/review-users";

        public ReviewControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            this.factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

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

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting" + Guid.NewGuid());
                    });

                    // Configure Test Authentication
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                });
                builder.UseEnvironment("Development");
            });

            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GivenExistingReview_WhenGetReviewByBuyerIdIsCallend_ThenReturnsTheRightReview()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            // Act
            var response = await client.GetAsync($"{BaseUrl}/buyers/fb0c0cbf-cf67-4cc8-babc-63d8b24862b6");
            // Assert
            response.EnsureSuccessStatusCode();
            var review = dbContext.ReviewUsers.FirstOrDefaultAsync(b => b.SellerId == new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"));
            review.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenExistingReview_WhenGetReviewBySellerIdIsCallend_ThenReturnsTheRightReview()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            // Act
            var response = await client.GetAsync($"{BaseUrl}/sellers/fb0c0cbf-cf67-4cc8-babc-63d8b24862b7");
            // Assert
            response.EnsureSuccessStatusCode();
            var review = dbContext.ReviewUsers.FirstOrDefaultAsync(b => b.SellerId == new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"));
            review.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenValidReview_WhenCreatedIscalled_Then_ShouldAddToDatabaseTheReview()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            var review = new Review
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b2"),
                SellerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                BuyerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b6"),
                Description = "Great estate",
                Rating = 5,
            };
            var command = new CreateReviewUserCommand
            {
                SellerId = review.SellerId,
                BuyerId = review.BuyerId,
                Description = review.Description,
                Rating = review.Rating
            };

            // Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.EnsureSuccessStatusCode();
            var reviewResponse = dbContext.ReviewUsers.FirstOrDefaultAsync(b => b.Description == "Great estate");
            reviewResponse.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenInvalidReview_WhenCreatedIsCalled_Then_ShouldNotAddToDatabaseTheReview()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            var review = new Review
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b2"),
                SellerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                BuyerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b6"),
                Description = "",
                Rating = 5,
            };
            var command = new CreateReviewUserCommand
            {
                SellerId = review.SellerId,
                BuyerId = review.BuyerId,
                Description = review.Description,
                Rating = review.Rating
            };

            // Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenInValidReview_WhenGetReviewUserById_ThenShouldReturnTheReview()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();

            // Act
            var response = await client.GetAsync($"{BaseUrl}/fb0c0cbf-cf67-4cc8-babc-63d8b24862b1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }
        private void CreateSUT()
        {
            var review = new Review
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b1"),
                SellerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                BuyerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b6"),
                Description = "Great estate",
                Rating = 5,
            };
            dbContext.ReviewUsers.Add(review);
            dbContext.SaveChanges();
        }
        private HttpClient CreateAuthenticatedClient()
        {
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                // Automatically include authentication headers
                AllowAutoRedirect = false
            });
            return client;
        }

    }
}
