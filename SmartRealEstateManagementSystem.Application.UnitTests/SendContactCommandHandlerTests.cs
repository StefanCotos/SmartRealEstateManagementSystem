using Application.Use_Cases.ActionsOnUser;
using Domain.Common;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class SendContactCommandHandlerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly SendContactFormCommandHandler _handler;

        public SendContactCommandHandlerTests()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _handler = new SendContactFormCommandHandler(_userRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new SendContactFormCommand
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Subject = "Inquiry",
                Message = "I have a question about your services."
            };

            _userRepository.SendContactForm(command.Name, command.Email, command.Subject, command.Message)
                .Returns(Result<string>.Success("Message sent successfully"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Message sent successfully");
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new SendContactFormCommand
            {
                Name = "John Doe",
                Email = "john.doe@example.com",
                Subject = "Inquiry",
                Message = "I have a question about your services."
            };

            _userRepository.SendContactForm(command.Name, command.Email, command.Subject, command.Message)
                .Returns(Result<string>.Failure("Failed to send message"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().Be("Failed to send message");
        }
    }
}

