using Application.Use_Cases.Authentification;
using Domain.Common;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class ConfirmEmailCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ConfirmEmailCommandHandler _handler;

        public ConfirmEmailCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new ConfirmEmailCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new ConfirmEmailCommand
            {
                Token = "valid-token"
            };

            var successResult = Result<string>.Success("Email confirmed successfully");
            _userRepository.ConfirmEmail(Arg.Any<string>()).Returns(successResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Email confirmed successfully");
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new ConfirmEmailCommand
            {
                Token = "invalid-token"
            };

            var failureResult = Result<string>.Failure("Invalid token");
            _userRepository.ConfirmEmail(Arg.Any<string>()).Returns(failureResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be("Invalid token");
        }
    }
}
