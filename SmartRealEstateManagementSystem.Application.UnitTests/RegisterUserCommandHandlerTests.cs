using Application.Use_Cases.Authentification;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class RegisterUserCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new RegisterUserCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = "john.doe@example.com",
                Password = "password123",
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe"
            };
            var userId = new Guid("087eb58a-a471-4f9c-8cc6-ae5e08db9c3f");

            var successResult = Result<Guid>.Success(userId);
            _userRepository.Register(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(successResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(userId);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new RegisterUserCommand
            {
                Email = "john.doe@example.com",
                Password = "password123",
                Username = "johndoe",
                FirstName = "John",
                LastName = "Doe"
            };

            var failureResult = Result<Guid>.Failure("Registration failed");
            _userRepository.Register(Arg.Any<User>(), Arg.Any<CancellationToken>()).Returns(failureResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().Be("Registration failed");
        }
    }
}
