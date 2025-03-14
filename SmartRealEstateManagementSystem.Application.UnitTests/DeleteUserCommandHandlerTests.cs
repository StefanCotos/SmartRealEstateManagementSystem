using Application.Use_Cases.CommandHandlers.UserCH;
using Application.Use_Cases.Commands.UserC;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly IGenericEntityRepository<User> repository;
        private readonly DeleteUserCommandhandler handler;

        public DeleteUserCommandHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<User>>();
            handler = new DeleteUserCommandhandler(repository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new DeleteUserCommand(new Guid("e1b6c9a7-a5d4-4326-9d85-9b67d82e0bcb"));

            var user = new User
            {
                Id = command.Id,
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@gmail.com",
                Password = "12345678"
            };

            repository.DeleteAsync(user.Id).Returns(Result<Guid>.Success(user.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(user.Id);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new DeleteUserCommand(new Guid("e1b6c9a7-a5d4-4326-9d85-9b67d82e0bcb"));

            repository.DeleteAsync(command.Id).Returns(Result<Guid>.Failure("User not found"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().Be("User not found");
        }
    }
}
