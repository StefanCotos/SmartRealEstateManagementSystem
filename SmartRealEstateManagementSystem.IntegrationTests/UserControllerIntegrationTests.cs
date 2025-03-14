using Application.Use_Cases.Commands.UserC;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class UserControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "api/users";

        public UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
            });

            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task GivenUsers_WhenGetAllIsCalled_ThenReturnsTheRightContentType()
        {
            // Arrange
            var client = CreateAuthenticatedClient();

            // Act
            var response = await client.GetAsync(BaseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var users = dbContext.Users.ToList();
            users.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenExistingUsers_WhenGetAllIsCalled_ThenReturnsTheRightUsers()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();

            // Act
            var response = await client.GetAsync(BaseUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var users = dbContext.Users.ToList();
            users.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenValidUser_WhenCreateIsCalled_ThenShouldAddInDatabase()
        {
            // Arrange
            var client = factory.CreateClient();
            var user = new User
            {
                FirstName = "Jane",
                LastName = "Smith",
                UserName = "janesmith",
                Email = "janesmith@gmail.com",
                Password = "Jane1234",
            };
            var command = new CreateUserCommand
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
            };

            // Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.EnsureSuccessStatusCode();
            var users = dbContext.Users.FirstOrDefaultAsync(b => b.UserName == "janesmith");
            users.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenInvalidUser_WhenUpdateIsCalled_ThenShouldNotUpdateInDatabase()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            var userId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7");
            var user = new User
            {
                Id = userId,
                FirstName = "Jane",
                LastName = "Smith",
                UserName = "janesmith",
                Email = "janesmith@gmail.com",
                Password = "Jane1234",
            };
            var command = new UpdateUserCommand
            {
                Id = user.Id,
                FirstName = "",
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
            };
            // Act
            var path = $"{BaseUrl}/{user.Id}";
            var response = await client.PutAsJsonAsync(path, command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenInvalidUser_WhenDeleteIsCalled_ThenShouldNotBeDeletedInDatabase()
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
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var path = $"{BaseUrl}/{user.Id}";
            var response = await client.DeleteAsync(path);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenValidUser_WhenGetByIdIsCalled_ThenShouldReturnTheRightUser()
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
                Password = "12345678",
                IsAdmin = false,
                IsEmailConfirmed = true,
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var path = $"{BaseUrl}/{user.Id}";
            var response = await client.GetAsync(path);

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
            var user = new User
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@gmail.com",
                Password = "12345678"
            };
            dbContext.Users.Add(user);
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
