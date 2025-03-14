using Application.DTO;
using Application.Use_Cases.Queries.FavoriteQ;
using Application.Use_Cases.QueryHandlers.FavoriteQH;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class GetFavoritesByUserIdQueryHandlerTests
    {
        private readonly IFavoriteRepository repository;
        private readonly IMapper mapper;
        private readonly GetFavoritesByUserIdQueryHandler handler;

        public GetFavoritesByUserIdQueryHandlerTests()
        {
            repository = Substitute.For<IFavoriteRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetFavoritesByUserIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetFavoritesByUserIdQueryHandler_When_HandleIsCalled_Then_TheFavoritesShouldBeReturned()
        {
            // Arrange
            var userId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            var favorites = new List<Favorite>
            {
                new Favorite { UserId = userId, EstateId = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2") }
            };
            repository.GetAllFavoritesByUserIdAsync(userId).Returns(favorites);
            var query = new GetFavoritesByUserIdQuery { Id = userId };
            var favoriteDtos = favorites.Select(f => new FavoriteDto
            {
                UserId = f.UserId,
                EstateId = f.EstateId
            }).ToList();
            mapper.Map<List<FavoriteDto>>(favorites).Returns(favoriteDtos);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().EstateId.Should().Be(favorites.First().EstateId);
        }

        [Fact]
        public async Task Given_GetFavoritesByUserIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnEmptyList()
        {
            // Arrange
            var userId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            repository.GetAllFavoritesByUserIdAsync(userId).Returns(Enumerable.Empty<Favorite>());
            var query = new GetFavoritesByUserIdQuery { Id = userId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
