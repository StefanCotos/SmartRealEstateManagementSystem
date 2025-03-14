using Application.Use_Cases.Commands.ReportC;
using Domain.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Net.Http.Json;

namespace SmartRealEstateManagementSystem.IntegrationTests
{
    public class ReportControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext dbContext;

        private string BaseUrl = "api/reports";

        public ReportControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
        public async Task GivenExistingReport_WhenGetAllReportsIsCalled_ThenReturnsTheRightContentType()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            // Act
            var response = await client.GetAsync(BaseUrl);
            // Assert
            response.EnsureSuccessStatusCode();
            var reports = await dbContext.Reports.ToListAsync();
            reports.Should().NotBeNull();
        }

        [Fact]
        public async Task GivenExistingReport_WhenGetReportByIdIsCalled_ThenReturnsTheRightReport()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            CreateSUT();
            // Act
            var response = await client.GetAsync(BaseUrl + "/6cd6bf8e-412d-4313-a85b-f3632c8c0fb6");
            // Assert
            var reports = await dbContext.Reports.ToListAsync();
            reports.Should().NotBeNull();
            reports.Should().HaveCount(1);
            reports.First().Id.Should().Be(new Guid("6cd6bf8e-412d-4313-a85b-f3632c8c0fb6"));
        }

        [Fact]
        public async Task GivenInvalidCreateReportCommand_WhenCreateReportIsCalled_Then_ShouldReturnBadRequest()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            var command = new CreateReportCommand
            {
                BuyerId = Guid.Empty, // Invalid BuyerId
                SellerId = Guid.NewGuid(),
                Description = "Test Report"
            };

            // Act
            var response = await client.PostAsJsonAsync(BaseUrl, command);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GivenValidReport_WhenDeleteReportIsCalled_Then_ShouldReturnBadRequest()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            var reportId = new Guid("6cd6bf8e-412d-4313-a85b-f3632c8c0fb6");

            // Ensure the report exists in the database
            CreateSUT();

            // Act
            var response = await client.DeleteAsync($"{BaseUrl}/{reportId}");
            dbContext.SaveChanges();

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            // Verify that the report is not removed from the database
            var report = await dbContext.Reports.FirstOrDefaultAsync(r => r.Id == reportId);
            report.Should().NotBeNull();
        }



        [Fact]
        public async Task GivenInvalidReport_WhenDeleteReportIsCalled_Then_ShouldReturnBadRequest()
        {
            // Arrange
            var client = CreateAuthenticatedClient();
            var reportId = Guid.NewGuid();

            // Act
            var response = await client.DeleteAsync($"{BaseUrl}/{reportId}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task GivenValidReport_WhenCreateIsCalled_Then_ShouldAddToDatabaseTheEstate()
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

            var report = new CreateReportCommand
            {
                SellerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                BuyerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b6"),
                Description = "Suspicious activity",
            };

            // Act
            var response = await client.PostAsJsonAsync(BaseUrl, report);

            // Assert
            response.EnsureSuccessStatusCode();
            var reports = await dbContext.Reports.ToListAsync();
            reports.Should().NotBeNull();
        }



        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dbContext.Database.EnsureDeleted();
            dbContext.Dispose();
        }

        private void CreateSUT()
        {
            var report = new Report
            {
                Id = new Guid("6cd6bf8e-412d-4313-a85b-f3632c8c0fb6"),
                SellerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                BuyerId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b6"),
                Description = "Suspicious activity",
            };
            dbContext.Reports.Add(report);
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
