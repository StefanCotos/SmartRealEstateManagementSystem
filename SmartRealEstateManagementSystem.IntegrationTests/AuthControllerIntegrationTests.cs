using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Application.Use_Cases.Authentification;
using Domain.Entities;
using System.Net.Http.Json;
using FluentAssertions;
using System.Net;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "api/auth";

        public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
                });
            });

            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task RegisterUser_ReturnsBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var client = factory.CreateClient();
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

            var command = new RegisterUserCommand
            {
                Email = user.Email,
                Password = user.Password,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/register", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task LoginUser_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var client = factory.CreateClient();
            var user = new User
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@gmail.com",
                Password = "12345678",
            };

            var command = new LoginUserCommand
            {
                Email = user.Email,
                Password = user.Password
            };

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/login", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var users = dbContext.Users.FirstOrDefaultAsync(b => b.FirstName == "John");
            users.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenInValidEmail_WhenConfirmEmailIsCalled_ThenShouldReturnInvalidRequest()
        {
            // Arrange
            var client = factory.CreateClient();
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

            var command = new ConfirmEmailCommand
            {
                Token = "invalidToken",
            };

            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/email-confirmation-valid", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }

    }
}
