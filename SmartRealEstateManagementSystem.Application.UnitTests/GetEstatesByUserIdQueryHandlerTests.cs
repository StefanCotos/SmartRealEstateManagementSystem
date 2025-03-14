using Application.DTO;
using Application.Use_Cases.Queries.EstateQ;
using Application.Use_Cases.QueryHandlers.EstateQH;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class GetEstatesByUserIdQueryHandlerTests
    {
        private readonly IEstateRepository repository;
        private readonly IMapper mapper;
        private readonly GetEstatesByUserIdQueryHandler handler;

        public GetEstatesByUserIdQueryHandlerTests()
        {
            repository = Substitute.For<IEstateRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetEstatesByUserIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetEstatesByUserIdQueryHandler_When_HandleIsCalled_Then_TheEstatesShouldBeReturned()
        {
            // Arrange
            var userId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            var estates = new List<Estate>
            {
                new Estate
                {
                    Id = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"),
                    UserId = userId,
                    Name = "Estate 1",
                    Description = "Description 1",
                    Price = 100000,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    LandSize = 500,
                    Street = "Street 1",
                    City = "City 1",
                    State = "State 1",
                    ZipCode = "12345",
                    HouseSize = 200,
                    IsSold = false,
                    ListingData = DateTime.Now
                }
            };
            repository.GetAllEstatesByUserIdAsync(userId).Returns(estates);
            var query = new GetEstatesByUserIdQuery { Id = userId };
            var estateDtos = estates.Select(e => new EstateDto
            {
                Id = e.Id,
                UserId = e.UserId,
                Name = e.Name,
                Description = e.Description,
                Price = e.Price,
                Bedrooms = e.Bedrooms,
                Bathrooms = e.Bathrooms,
                LandSize = e.LandSize,
                Street = e.Street,
                City = e.City,
                State = e.State,
                ZipCode = e.ZipCode,
                HouseSize = e.HouseSize,
                IsSold = e.IsSold,
                ListingData = e.ListingData
            }).ToList();
            mapper.Map<List<EstateDto>>(estates).Returns(estateDtos);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(estates.First().Id);
        }

        [Fact]
        public async Task Given_GetEstatesByUserIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            repository.GetAllEstatesByUserIdAsync(userId).Returns(Enumerable.Empty<Estate>());
            var query = new GetEstatesByUserIdQuery { Id = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
