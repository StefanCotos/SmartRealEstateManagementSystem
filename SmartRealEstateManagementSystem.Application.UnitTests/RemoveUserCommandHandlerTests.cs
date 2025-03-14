using Application.Use_Cases.ActionsOnUser;
using Domain.Common;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class RemoveUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly RemoveUserCommandHandler _handler;

        public RemoveUserCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new RemoveUserCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new RemoveUserCommand(Id: Guid.NewGuid());

            _userRepository.RemoveAsync(command.Id).Returns(Result<Guid>.Success(command.Id));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(command.Id);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new RemoveUserCommand(Id: Guid.NewGuid());


            _userRepository.RemoveAsync(command.Id).Returns(Result<Guid>.Failure("Failed to remove user"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().Be("Failed to remove user");
        }
    }
}


