using Application.Use_Cases.ActionsOnUser;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class ChangePasswordCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new ChangePasswordCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new ChangePasswordCommand
            {
                Email = "john@gmail.com",
                Password = "newpassword123"
            };
            var user = new User
            {
                Email = command.Email,
                Password = command.Password
            };

            var successResult = Result<bool>.Success(true);
            _userRepository.ChangePassword(Arg.Any<User>()).Returns(successResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeTrue();
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new ChangePasswordCommand
            {
                Email = "john@gmail.com",
                Password = "newpassword123"
            };
            var user = new User
            {
                Email = command.Email,
                Password = command.Password
            };

            var failureResult = Result<bool>.Failure("Error");
            _userRepository.ChangePassword(Arg.Any<User>()).Returns(failureResult);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }
}