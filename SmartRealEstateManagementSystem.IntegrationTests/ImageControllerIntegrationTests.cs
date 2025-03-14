using Application.Use_Cases.Commands.ImageC;
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
    public class ImageControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/images";

        public ImageControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
                        d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var dbConnectionDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbConnection));

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
                    .AddScheme<AuthenticationSchemeOptions, SmartRealEstateManagementSystem.IntegrationTests.TestAuthHandler>("Test", options => { });
                });
            });

            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GivenExistingImage_WhenGetAllImagesIsCalled_ThenReturnsTheRightContentType()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            var estateId = await SeedEstate();
            //CreateSUT(estateId);

            // Act
            var response = await client.GetAsync(BaseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GivenExistingImage_WhenGetImageByIdIsCalled_ThenReturnsTheRightImage()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();


            // Act
            var response = await client.GetAsync(BaseUrl + "/01945a67-8830-7b63-8d80-88bd08347c13");
            // Assert
            var images = await dbContext.Images.ToListAsync();
            images.Should().NotBeNull();
            images.Should().HaveCount(1);
            images.First().Id.Should().Be(new Guid("01945a67-8830-7b63-8d80-88bd08347c13"));
        }

        [Fact]
        public async Task GivenValidImage_WhenCreateIsCalled_Then_ShouldAddToDatabaseTheImage()
        {
            // Arrange
            var client = CreateAuthenticatedClient();


            var estate = new Estate
            {
                Id = new Guid("e44be7ec-fb09-4811-9636-b60f6f5ed536"),
                UserId = new Guid("e2ee50a8-fcda-44e3-9ab7-1e7e4900b99a"),
                Name = "KOA House",
                Description = "A big house with 3 rooms and 2 bathrooms, open space kitchen and a beauty view from balcony.",
                Price = 130000,
                Bedrooms = 3,
                Bathrooms = 2,
                LandSize = 2000,
                Street = "Soseaua Sararie nr.48",
                City = "Iasi",
                State = "Romania",
                ZipCode = "700452",
                HouseSize = 1000,
                ListingData = DateTime.UtcNow
            };

            var image = new CreateImageCommand
            {
                Id = new Guid("0145283c-648f-4cf0-b9c5-8226cd9f3e4f"),
                EstateId = new Guid("37ba2857-b1b4-49e9-b724-390bf0625b25"),
                Extension = ".jpg"
            };

            // Act
            var response = await client.PostAsJsonAsync(BaseUrl, image);

            // Assert
            response.EnsureSuccessStatusCode();
            var reports = await dbContext.Images.ToListAsync();
            reports.Should().NotBeNull();
        }

        

     
        [Fact]
        public async Task GetAllImages_ShouldReturnCorrectContentType()
        {
            // Arrange
            var client = CreateAuthenticatedClient();

            // Act
            var response = await client.GetAsync(BaseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType?.ToString().Should().Be("application/json; charset=utf-8");
        }

        [Fact]
        public async Task GivenInvalidExistingImge_WhenUpdateIsCalled_ThenNotUpdatesTheEstateDetails()
        {

            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var imageId = new Guid("01945a67-8830-7b63-8d80-88bd08347c13");
            var updateCommand = new UpdateImageCommand
            {
                Id = imageId,
                EstateId = new Guid("1f79d545-3309-4d12-b6dd-fe8ed5705acc"),
                Extension = ".png"
            };
            // Act
            var response = await client.PutAsJsonAsync($"{BaseUrl}/{imageId}", updateCommand);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task GivenNotExistingImageId_WhenDeleteIsCalled_ThenNotRemovesTheEstateFromDatabase()
        {
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var imageId = new Guid("01945a67-8830-7b63-8d80-88bd08347c13");
            var response = await client.DeleteAsync(BaseUrl + "/" + imageId);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }
        [Fact]
        public async Task GivenExistingImage_WhenGetImageByEstateIdIsCallend_ThenReturnsTheRightImages()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            // Act
            var response = await client.GetAsync($"{BaseUrl}/estates/1f79d545-3309-4d12-b6dd-fe8ed5705acc");
            // Assert
            response.EnsureSuccessStatusCode();
            var image = dbContext.Images.FirstOrDefaultAsync(b => b.EstateId == new Guid("1f79d545-3309-4d12-b6dd-fe8ed5705acc"));
            image.Should().NotBeNull();
        }
        [Fact]
        public async Task GivenInvalidCreateImageCommand_WhenCreateImageIsCalled_Then_ShouldReturnBadRequest()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            var command = new CreateImageCommand
            {
                Id = Guid.Empty,
                EstateId = Guid.NewGuid(),
                Extension = ""
            };

            // Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }

        private void CreateSUT()
        {

            var image = new Image
            {
                Id = new Guid("01945a67-8830-7b63-8d80-88bd08347c13"),
                EstateId = new Guid("1f79d545-3309-4d12-b6dd-fe8ed5705acc"),
                Extension = ".jpg"
            };

            dbContext.Images.Add(image);
            dbContext.SaveChanges();

        }

        private async Task<Guid> SeedEstate()
        {
      

            var estate = new Estate
            {
                Id = Guid.Parse("1f79d545-3309-4d12-b6dd-fe8ed5705acc"),
                UserId = Guid.Parse("01945721-e0a3-7996-a37a-226cc664533d"),
                Name = "KOA House",
                Description = "A big house with 3 rooms and 2 bathrooms, open space kitchen and a beauty view from balcony.",
                Price = 130000,
                Bedrooms = 3,
                Bathrooms = 2,
                LandSize = 2000,
                Street = "Soseaua Sararie nr.48",
                City = "Iasi",
                State = "Romania",
                ZipCode = "700452",
                HouseSize = 1000,
                ListingData = DateTime.UtcNow
            };
            dbContext.Estates.Add(estate);
            await dbContext.SaveChangesAsync();

            return estate.Id;
        }

        private HttpClient CreateAuthenticatedClient()
        {
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            return client;
        }
    }
}
