using Application.Use_Cases.CommandHandlers.ImageCH;
using Application.Use_Cases.Commands.ImageC;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class DeleteImageCommandHandlerTests
    {
        private readonly IGenericEntityRepository<Image> repository;
        private readonly DeleteImageCommandHandler handler;

        public DeleteImageCommandHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Image>>();
            handler = new DeleteImageCommandHandler(repository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new DeleteImageCommand(new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"));
            var image = new Image
            {
                Id = command.Id,
                EstateId = Guid.NewGuid(),
                Extension = ".jpg"
            };

            repository.DeleteAsync(image.Id).Returns(Result<Guid>.Success(image.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(image.Id);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new DeleteImageCommand(new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"));

            repository.DeleteAsync(command.Id).Returns(Result<Guid>.Failure("Image not found"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().Be("Image not found");
        }
    }
}

