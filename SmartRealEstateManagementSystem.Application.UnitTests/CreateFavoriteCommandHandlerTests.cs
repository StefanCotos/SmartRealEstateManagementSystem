using Application.Use_Cases.CommandHandlers.FavoriteCH;
using Application.Use_Cases.Commands.FavoriteC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class CreateFavoriteCommandHandlerTests
    {
        private readonly IGenericEntityRepository<Favorite> favoriteRepository;
        private readonly IMapper mapper;
        private readonly CreateFavoriteCommandHandler handler;

        public CreateFavoriteCommandHandlerTests()
        {
            favoriteRepository = Substitute.For<IGenericEntityRepository<Favorite>>();
            mapper = Substitute.For<IMapper>();
            handler = new CreateFavoriteCommandHandler(favoriteRepository, mapper);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            //Arrange
            var favorite = new Favorite
            {
                UserId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                EstateId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7")
            };
            var command = new CreateFavoriteCommand
            {
                UserId = favorite.UserId,
                EstateId = favorite.EstateId
            };
            mapper.Map<Favorite>(command).Returns(favorite);
            favoriteRepository.AddAsync(favorite).Returns(Result<Guid>.Success(favorite.UserId));
            //Act
            var result = await handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(favorite.UserId);
        }
        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            //Arrange
            var favorite = new Favorite
            {
                UserId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                EstateId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7")
            };
            var command = new CreateFavoriteCommand
            {
                UserId = favorite.UserId,
                EstateId = favorite.EstateId
            };
            mapper.Map<Favorite>(command).Returns(favorite);
            favoriteRepository.AddAsync(favorite).Returns(Result<Guid>.Failure("Error"));
            //Act
            var result = await handler.Handle(command, CancellationToken.None);
            //Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Error");
        }
    }
}
