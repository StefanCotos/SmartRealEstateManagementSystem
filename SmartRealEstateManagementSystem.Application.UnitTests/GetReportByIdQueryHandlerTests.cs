using Application.DTO;
using Application.Use_Cases.Queries.Report;
using Application.Use_Cases.QueryHandlers.ReportQH;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class GetReportByIdQueryHandlerTests
    {
        private readonly IGenericEntityRepository<Report> repository;
        private readonly IMapper mapper;
        private readonly GetReportByIdQueryHandler handler;

        public GetReportByIdQueryHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Report>>();
            mapper = Substitute.For<IMapper>();
            handler = new GetReportByIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetReportByIdQueryHandler_When_HandleIsCalled_Then_TheReportShouldBeReturned()
        {
            // Arrange
            var reportId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            var report = new Report
            {
                Id = reportId,
                BuyerId = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"),
                SellerId = new Guid("c3c7b2a9-715d-4b99-8e1d-43ed32a6a8b3"),
                Description = "Test Report"
            };
            repository.GetByIdAsync(reportId).Returns(report);
            var query = new GetReportByIdQuery { Id = reportId };
            var reportDto = new ReportDto
            {
                Id = report.Id,
                BuyerId = report.BuyerId,
                SellerId = report.SellerId,
                Description = report.Description
            };
            mapper.Map<ReportDto>(report).Returns(reportDto);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(report.Id);
        }

        [Fact]
        public async Task Given_GetReportByIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnNull()
        {
            // Arrange
            var reportId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            repository.GetByIdAsync(reportId).Returns((Report?)null);
            var query = new GetReportByIdQuery { Id = reportId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}

