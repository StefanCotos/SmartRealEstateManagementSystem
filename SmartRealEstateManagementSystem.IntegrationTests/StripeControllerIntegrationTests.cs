using Application.Use_Cases.Commands.StripeC;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Net;
using System.Net.Http.Json;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class StripeControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/stripe";
        public StripeControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
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
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });
                });
            });

            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GivenProducts_WhenGetAllIsCalled_ThenReturnsTheRightContentType()
        {
            // arrange
            var client = factory.CreateClient();

            // act
            var response = await client.GetAsync($"{BaseUrl}/getAllProducts");

            // assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GivenExistingProducts_WhenGetAllIsCalled_ThenReturnsTheRightProducts()
        {
            // arrange
            var client = factory.CreateClient();
            CreateSUT();

            // act
            var response = await client.GetAsync($"{BaseUrl}/getAllProducts");

            // assert
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsStringAsync();
            products.Should().Contain("Test Product");
        }

        [Fact]
        public async Task GivenValidProduct_WhenCreatedIsCalled_Then_ShouldAddToStripeTheProduct()
        {
            // Arrange
            var client = factory.CreateClient();
            CreateSUT();

            var request = new CreateProductRequestCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                UnitAmount = 1000,
            };

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/createProduct", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var productResponse = await response.Content.ReadFromJsonAsync<CreateProductRequestCommand>();
            productResponse.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenValidProduct_WhenUpdatedIsCalled_Then_ShouldUpdateInStripeTheProduct()
        {
            // Arrange
            var client = factory.CreateClient();
            var productId = await CreateTestProduct(client);

            var request = new UpdateProductRequestCommand
            {
                ProductId = productId,
                Name = "Updated Product",
                Description = "Updated Description",
                UnitAmount = 2000,
            };

            // Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/updateProduct", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var productResponse = await response.Content.ReadFromJsonAsync<UpdateProductRequestCommand>();
            productResponse.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenValidPriceId_WhenPayIsCalled_Then_ShouldCreatePaymentSession()
        {
            // Arrange
            var client = factory.CreateClient();
            var productId = await CreateTestProduct(client);
            var priceId = await GetPriceId(client, productId);

            var request = new CreatePayRequestCommand
            {
                defaultPriceId = priceId,
            };

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/pay", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var sessionUrl = await response.Content.ReadAsStringAsync();
            sessionUrl.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetProductById_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var client = factory.CreateClient();
            var invalidId = "invalid_id";

            // Act
            var response = await client.GetAsync($"{BaseUrl}/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateProduct_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var client = factory.CreateClient();
            var request = new CreateProductRequestCommand
            {
                Name = "", // Invalid data
                Description = "", // Invalid data
                UnitAmount = -1000, // Invalid data
            };

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/createProduct", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateProduct_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var client = factory.CreateClient();
            var request = new UpdateProductRequestCommand
            {
                ProductId = "invalid_id", // Invalid data
                Name = "", // Invalid data
                Description = "", // Invalid data
                UnitAmount = -2000, // Invalid data
            };

            // Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/updateProduct", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Pay_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var client = factory.CreateClient();
            var request = new CreatePayRequestCommand
            {
                defaultPriceId = "invalid_price_id", // Invalid data
            };

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/pay", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }

        static private void CreateSUT()
        {
            
        }

        private async Task<string> CreateTestProduct(HttpClient client)
        {
            var request = new CreateProductRequestCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                UnitAmount = 1000,
            };

            var response = await client.PostAsJsonAsync($"{BaseUrl}/createProduct", request);
            response.EnsureSuccessStatusCode();
            var productResponse = await response.Content.ReadFromJsonAsync<CreateProductResponse>();
            return productResponse?.ProductId ?? throw new InvalidOperationException("Product creation failed.");
        }

        public class CreateProductResponse
        {
            public string? ProductId { get; set; }
        }

        private async Task<string> GetPriceId(HttpClient client, string productId)
        {
            var response = await client.GetAsync($"{BaseUrl}/{productId}");
            response.EnsureSuccessStatusCode();
            var productResponse = await response.Content.ReadFromJsonAsync<CreatePayRequestCommand>();
            return productResponse?.defaultPriceId ?? throw new InvalidOperationException("Product response is null");
        }
    }
}

