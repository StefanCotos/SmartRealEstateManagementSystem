using Application.DTO;
using Application.Use_Cases.Commands.EstateC;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Net;
using System.Net.Http.Json;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class EstateControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/estates";
        public EstateControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
            });

            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GivenEstates_WhenGetAllIsCalled_ThenReturnsTheRightContentType()
        {
            // arrange
            var client = CreateAuthenticatedClient();

            // act
            var response = await client.GetAsync(BaseUrl);

            // assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GivenExistingEstates_WhenGetAllIsCalled_ThenReturnsTheRightEstates()
        {
            // arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();

            // act
            var response = await client.GetAsync(BaseUrl);

            // assert
            response.EnsureSuccessStatusCode();
            var estates = dbContext.Estates.ToList();
            estates.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenInExistingEstates_WhenGetAllIsCalled_ThenReturnsBadRequest()
        {
            // arrange
            var client = CreateAuthenticatedClient();

            // act
            var path = BaseUrl + "/fb0c0cbf-cf67-4cc8-babc-63d8b24862b7";
            var response = await client.GetAsync(path);

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenValidEstate_WhenCreatedIsCalled_Then_ShouldAddToDatabaseTheEstate()
        {
            // Arrange
            var client = CreateAuthenticatedClient();

            var user = new User
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@gmail.com",
                Password = "12345678"
            };

            var command = new CreateEstateCommand
            {
                UserId = user.Id,
                Name = "Estate Name",
                Description = "Estate Description",
                Price = 100000,
                Bedrooms = 3,
                Bathrooms = 2,
                LandSize = 100,
                Street = "Street",
                City = "City",
                State = "State",
                ZipCode = "ZipCode",
                HouseSize = 1000,
                ListingData = DateTime.Now,
            };

            // Act
            var result = await client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            result.EnsureSuccessStatusCode();
            var estate = dbContext.Estates.FirstOrDefaultAsync(b => b.Name == "Estate Name");
            estate.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenInvalidEstate_WhenCreatedIsCalled_Then_ShouldReturnBadRequest()
        {
            // Arrange
            var client = CreateAuthenticatedClient();

            var user = new User
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b9"),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@gmail.com",
                Password = "12345678"
            };

            var command = new CreateEstateCommand
            {
                UserId = user.Id,
                Name = "", // Invalid name
                Description = "Estate Description",
                Price = 100000,
                Bedrooms = 3,
                Bathrooms = 2,
                LandSize = 100,
                Street = "Street",
                City = "City",
                State = "State",
                ZipCode = "ZipCode",
                HouseSize = 1000,
                ListingData = DateTime.Now,
            };

            // Act
            var result = await client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task GivenInvalidEstateId_WhenGetEstatesByIdIsCalled_ThenReturnsBadRequest()
        {
            // arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var estateId = new Guid("4fd3f7f1-fd01-4731-8c3d-e865306e0d94");

            // act
            var response = await client.GetAsync($"{BaseUrl}/{estateId}");

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenExistingEstates_WhenGetEstatesByUserIdIsCalled_ThenReturnsTheRightEstates()
        {
            // arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var userId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7");

            // Verify that the estate is added to the database
            var addedEstate = await dbContext.Estates.FirstOrDefaultAsync(e => e.UserId == userId);
            addedEstate.Should().NotBeNull();

            // act
            var response = await client.GetAsync($"{BaseUrl}/users/{userId}");

            // assert
            response.EnsureSuccessStatusCode();
            var estates = await response.Content.ReadFromJsonAsync<List<EstateDto>>();
            estates.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenEstates_WhenGetEstatesByUserIdIsCalled_ThenReturnsTheRightContentType()
        {
            // arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var userId = "fb0c0cbf-cf67-4cc8-babc-63d8b24862b7";

            // act
            var response = await client.GetAsync($"{BaseUrl}/users/{userId}");

            // assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GivenExistingEstates_WhenGetEstatesByBuyerIdIsCalled_ThenReturnsTheRightEstates()
        {
            // arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var buyerId = new Guid("c2f59a2e-d547-4b89-bac3-5deb400f2f8b");

            // Verify that the estate is added to the database
            var addedEstate = await dbContext.Estates.FirstOrDefaultAsync(e => e.BuyerId == buyerId);
            addedEstate.Should().NotBeNull();

            // act
            var response = await client.GetAsync($"{BaseUrl}/buyers/{buyerId}");

            // assert
            response.EnsureSuccessStatusCode();
            var estates = await response.Content.ReadFromJsonAsync<List<EstateDto>>();
            estates.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenEstates_WhenGetEstatesByBuyerIdIsCalled_ThenReturnsTheRightContentType()
        {
            // arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var buyer = "c2f59a2e-d547-4b89-bac3-5deb400f2f8b";

            // act
            var response = await client.GetAsync($"{BaseUrl}/users/{buyer}");

            // assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GivenInvalidEstate_WhenUpdateIsCalled_ThenNotUpdatesTheEstateDetails()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var estateId = new Guid("4fd3f7f1-fd01-4731-8c3d-e865306e0d95");
            var updateCommand = new UpdateEstateCommand
            {
                Id = estateId,
                Name = "Updated Estate",
                Description = "Updated Description",
                Price = 2000,
                Bedrooms = 4,
                Bathrooms = 3,
                LandSize = 200,
                Street = "Updated Street",
                City = "Updated City",
                State = "Updated State",
                ZipCode = "Updated ZipCode",
                HouseSize = 2000,
                ListingData = DateTime.Now,
                PriceId = "Updated PriceId",
                ProductId = "Updated ProductId",
                IsSold = true,
                buyerId = new Guid("c2f59a2e-d547-4b89-bac3-5deb400f2f8b")
            };

            // Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{estateId}", updateCommand);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenNotExistingEstateId_WhenDeleteIsCalled_ThenNotRemovesTheEstateFromDatabase()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var estateId = new Guid("4fd3f7f1-fd01-4731-8c3d-e865306e0d95"); // Id from CreateSUT()

            // Act
            var response = await client.DeleteAsync($"{BaseUrl}/{estateId}");

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
            var estate = new Estate
            {
                Id = new Guid("4fd3f7f1-fd01-4731-8c3d-e865306e0d91"),
                UserId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                Name = "Estate 1",
                Description = "Description 1",
                Price = 1000,
                Bedrooms = 3,
                Bathrooms = 2,
                LandSize = 100,
                Street = "Street 1",
                City = "City 1",
                State = "State 1",
                ZipCode = "ZipCode 1",
                HouseSize = 1000,
                ListingData = DateTime.Now,
                PriceId = "PriceId 1",
                ProductId = "ProductId 1",
                IsSold = false,
                BuyerId = new Guid("c2f59a2e-d547-4b89-bac3-5deb400f2f8b")
            };
            dbContext.Estates.Add(estate);
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