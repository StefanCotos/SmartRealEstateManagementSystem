using Application.Use_Cases.CommandHandlers.FavoriteCH;
using Application.Use_Cases.Commands.FavoriteC;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{

    namespace SmartRealEstateManagementSystem.Application.UnitTests
    {
        public class DeleteFavoriteCommandHandlerTests
        {
            private readonly IFavoriteRepository repository;
            private readonly DeleteFavoriteCommandHandler handler;

            public DeleteFavoriteCommandHandlerTests()
            {
                repository = Substitute.For<IFavoriteRepository>();
                handler = new DeleteFavoriteCommandHandler(repository);
            }

            [Fact]
            public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
            {
                //Arrange
                var command = new DeleteFavoriteCommand(new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"));
                var favorite = new Favorite
                {
                    UserId = command.UserId,
                    EstateId = command.EstateId
                };

                repository.DeleteAsync(favorite.UserId, command.EstateId).Returns(Result<Guid>.Success(favorite.UserId));

                //Act
                var result = await handler.Handle(command, CancellationToken.None);

                //Assert
                result.IsSuccess.Should().BeTrue();
                result.Data.Should().Be(favorite.UserId);
                result.ErrorMessage.Should().BeNull();
            }

            [Fact]
            public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
            {
                //Arrange
                var command = new DeleteFavoriteCommand(new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"), new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"));

                repository.DeleteAsync(command.UserId,command.EstateId).Returns(Result<Guid>.Failure("Favorite not found"));

                //Act
                var result = await handler.Handle(command, CancellationToken.None);

                //Assert
                result.IsSuccess.Should().BeFalse();
                result.Data.Should().Be(Guid.Empty);
                result.ErrorMessage.Should().Be("Favorite not found");
            }
        }
    }

}
