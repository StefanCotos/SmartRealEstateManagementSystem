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
    public class GetReportsQueryHandlerTests
    {
        private readonly IGenericEntityRepository<Report> repository;
        private readonly IMapper mapper;
        private readonly GetReportsQueryHandler handler;

        public GetReportsQueryHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Report>>();
            mapper = Substitute.For<IMapper>();
            handler = new GetReportsQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetReportsQueryHandler_When_HandleIsCalled_Then_TheReportsShouldBeReturned()
        {
            // Arrange
            var reports = new List<Report>
            {
                new Report
                {
                    Id = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1"),
                    BuyerId = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"),
                    SellerId = new Guid("c3c7b2a9-715d-4b99-8e1d-43ed32a6a8b3"),
                    Description = "Test Report 1"
                },
                new Report
                {
                    Id = new Guid("d3c7b2a9-715d-4b99-8e1d-43ed32a6a8b4"),
                    BuyerId = new Guid("e3c7b2a9-715d-4b99-8e1d-43ed32a6a8b5"),
                    SellerId = new Guid("f3c7b2a9-715d-4b99-8e1d-43ed32a6a8b6"),
                    Description = "Test Report 2"
                }
            };
            repository.GetAllAsync().Returns(reports);
            var query = new GetReportsQuery();
            var reportDtos = reports.Select(r => new ReportDto
            {
                Id = r.Id,
                BuyerId = r.BuyerId,
                SellerId = r.SellerId,
                Description = r.Description
            }).ToList();
            mapper.Map<List<ReportDto>>(reports).Returns(reportDtos);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Id.Should().Be(reports.First().Id);
        }

        [Fact]
        public async Task Given_GetReportsQueryHandler_When_HandleIsCalledWithNoReports_Then_ShouldReturnEmptyList()
        {
            // Arrange
            repository.GetAllAsync().Returns(Enumerable.Empty<Report>());
            var query = new GetReportsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}


