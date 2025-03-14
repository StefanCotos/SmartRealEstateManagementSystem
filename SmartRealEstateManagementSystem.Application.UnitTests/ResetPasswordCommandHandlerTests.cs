using Application.Use_Cases.Authentification;
using Domain.Common;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class ResetPasswordCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ResetPasswordCommandHandler _handler;

        public ResetPasswordCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new ResetPasswordCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Token = "valid-token",
                NewPassword = "newpassword123"
            };

            var successResult = Result<string>.Success("Password reset successful");
            _userRepository.ResetPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(successResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Password reset successful");
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new ResetPasswordCommand
            {
                Token = "invalid-token",
                NewPassword = "newpassword123"
            };

            var failureResult = Result<string>.Failure("Invalid token");
            _userRepository.ResetPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(failureResult);

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

