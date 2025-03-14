using Application.Use_Cases.Authentification;
using Domain.Common;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class SendResetPasswordCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly SendResetPasswordCommandHandler _handler;

        public SendResetPasswordCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new SendResetPasswordCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new SendResetPasswordCommand
            {
                Email = "john.doe@example.com"
            };

            var successResult = Result<string>.Success("Reset password email sent successfully");
            _userRepository.SendResetPassword(Arg.Any<string>()).Returns(successResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Reset password email sent successfully");
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new SendResetPasswordCommand
            {
                Email = "invalid@example.com"
            };

            var failureResult = Result<string>.Failure("Email not found");
            _userRepository.SendResetPassword(Arg.Any<string>()).Returns(failureResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be("Email not found");
        }
    }
}


