using Application.Use_Cases.CommandHandlers.ReportCH;
using Application.Use_Cases.Commands.ReportC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class CreateReportCommandHandlerTests
    {
        private readonly IGenericEntityRepository<Report> repository;
        private readonly IMapper mapper;
        private readonly CreateReportCommandHandler handler;

        public CreateReportCommandHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Report>>();
            mapper = Substitute.For<IMapper>();
            handler = new CreateReportCommandHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var report = new Report
            {
                Id = Guid.NewGuid(),
                BuyerId = Guid.NewGuid(),
                SellerId = Guid.NewGuid(),
                Description = "Report description"
            };
            var command = new CreateReportCommand
            {
                BuyerId = report.BuyerId,
                SellerId = report.SellerId,
                Description = report.Description
            };

            mapper.Map<Report>(command).Returns(report);
            repository.AddAsync(report).Returns(Result<Guid>.Success(report.Id));

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
            var report = new Report
            {
                Id = Guid.NewGuid(),
                BuyerId = Guid.NewGuid(),
                SellerId = Guid.NewGuid(),
                Description = "Report description"
            };
            var command = new CreateReportCommand
            {
                BuyerId = report.BuyerId,
                SellerId = report.SellerId,
                Description = report.Description
            };

            mapper.Map<Report>(command).Returns(report);
            repository.AddAsync(report).Returns(Result<Guid>.Failure("Error"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }
}

