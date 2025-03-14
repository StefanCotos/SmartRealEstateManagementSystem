using Application.Use_Cases.Authentification;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class LoginUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new LoginUserCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new LoginUserCommand
            {
                Email = "john@gmail.com",
                Password = "password123"
            };
            var user = new User
            {
                Email = command.Email,
                Password = command.Password
            };

            var successResult = Result<string>.Success("Login successful");
            _userRepository.Login(Arg.Any<User>()).Returns(successResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Login successful");
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new LoginUserCommand
            {
                Email = "john@gmail.com",
                Password = "wrongpassword"
            };
            var user = new User
            {
                Email = command.Email,
                Password = command.Password
            };

            var failureResult = Result<string>.Failure("Invalid credentials");
            _userRepository.Login(Arg.Any<User>()).Returns(failureResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be("Invalid credentials");
        }
    }
}

