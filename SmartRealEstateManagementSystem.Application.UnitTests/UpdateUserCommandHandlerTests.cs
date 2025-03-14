using Application.Use_Cases.CommandHandlers.UserCH;
using Application.Use_Cases.Commands.UserC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;


namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly IGenericEntityRepository<User> repository;
        private readonly IMapper mapper;
        private readonly UpdateUserCommandHandler handler;

        public UpdateUserCommandHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<User>>();
            mapper = Substitute.For<IMapper>();
            handler = new UpdateUserCommandHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new UpdateUserCommand
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                FirstName = "John",
                LastName = "Doe",
                UserName = "johndoe",
                Email = "john@gmail.com",
                Password = "John1234"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                Password = command.Password
            };

            mapper.Map<User>(command).Returns(user);
            repository.UpdateAsync(user).Returns(Result<Guid>.Success(user.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<Result<Guid>>();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(user.Id);
            await repository.Received(1).UpdateAsync(user);
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new UpdateUserCommand
            {
                Id = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                FirstName = "Jane",
                LastName = "Smith",
                UserName = "janesmith",
                Email = "jane@gmail.com",
                Password = "Jane1234"
            };

            var user = new User
            {
                Id = command.Id,
                FirstName = command.FirstName,
                LastName = command.LastName,
                UserName = command.UserName,
                Email = command.Email,
                Password = command.Password
            };

            mapper.Map<User>(command).Returns(user);
            repository.UpdateAsync(user).Returns(Result<Guid>.Failure("Update operation failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<Result<Guid>>();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Update operation failed.");
        }
    }
}
