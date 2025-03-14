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
    public class GetEstatesByBuyerIdQueryHandlerTests
    {
        private readonly IEstateRepository repository;
        private readonly IMapper mapper;
        private readonly GetEstatesByBuyerIdQueryHandler handler;

        public GetEstatesByBuyerIdQueryHandlerTests()
        {
            repository = Substitute.For<IEstateRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetEstatesByBuyerIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetEstatesByBuyerIdQueryHandler_When_HandleIsCalled_Then_TheEstatesShouldBeReturned()
        {
            // Arrange
            var buyerId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            var estates = new List<Estate>
            {
                new Estate
                {
                    Id = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"),
                    UserId = new Guid("d2aca8c8-ea05-4303-ad6f-83b41d71f97c"),
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
                    BuyerId = buyerId,
                    ListingData = DateTime.Now
                }
            };
            repository.GetAllEstatesByBuyerIdAsync(buyerId).Returns(estates);
            var query = new GetEstatesByBuyerIdQuery { Id = buyerId };
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
        public async Task Given_GetEstatesByBuyerIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnEmptyList()
        {
            // Arrange
            var buyerId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            repository.GetAllEstatesByBuyerIdAsync(buyerId).Returns(Enumerable.Empty<Estate>());
            var query = new GetEstatesByBuyerIdQuery { Id = buyerId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
