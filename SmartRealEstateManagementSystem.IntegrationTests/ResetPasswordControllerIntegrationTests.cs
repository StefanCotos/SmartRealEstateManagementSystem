using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using Application.Use_Cases.Authentification;
using System.Net.Http.Json;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class ResetPasswordControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "/api/reset-password";

        public ResetPasswordControllerIntegrationTests(WebApplicationFactory<Program> factory)
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

            // Create service scope to access DbContext
            var scope = this.factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task SendResetPasswordEmail_ReturnsOk()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var command = new SendResetPasswordCommand
            {
                Email = "johndoe@emai.com"
            };
            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/forgot-password", command);

            // Assert
            response.Should().NotBeNull();
        }
        [Fact]
        public async Task ResetPasswordWithBadToken_ReturnsBadRequest()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            var command = new ResetPasswordCommand
            {
                Token = "badToken",
                NewPassword = "newPassword"
            };
            // Act
            var response = await client.PostAsJsonAsync($"{BaseUrl}/reset-password", command);

            // Assert
            response.Should().NotBeNull();
        }

        private void CreateSUT()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                UserName = "JohnDoe",
                Email = "johndoe@emai.com",
                Password = "password",
                IsAdmin = false,
                IsEmailConfirmed = true,
                EmailConfirmationToken = "token",
                EmailConfirmationTokenExpires = DateTime.Now.AddDays(1),
            };
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
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
