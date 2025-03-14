using Application.Use_Cases.CommandHandlers.ReportCH;
using Application.Use_Cases.Commands.ReportC;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class DeleteReportCommandHandlerTests
    {
        private readonly IGenericEntityRepository<Report> repository;
        private readonly DeleteReportCommandHandler handler;

        public DeleteReportCommandHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Report>>();
            handler = new DeleteReportCommandHandler(repository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new DeleteReportCommand(new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"));
            var report = new Report
            {
                Id = command.Id,
                BuyerId = Guid.NewGuid(),
                SellerId = Guid.NewGuid(),
                Description = "Report description"
            };

            repository.DeleteAsync(report.Id).Returns(Result<Guid>.Success(report.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(report.Id);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new DeleteReportCommand(new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"));

            repository.DeleteAsync(command.Id).Returns(Result<Guid>.Failure("Report not found"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().Be("Report not found");
        }
    }
}

