using Application.Use_Cases.CommandHandlers.ImageCH;
using Application.Use_Cases.Commands.ImageC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class UpdateImageCommandHandlerTests
    {
        private readonly IGenericEntityRepository<Image> repository;
        private readonly IMapper mapper;
        private readonly UpdateImageCommandHandler handler;

        public UpdateImageCommandHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Image>>();
            mapper = Substitute.For<IMapper>();
            handler = new UpdateImageCommandHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var image = new Image
            {
                Id = Guid.NewGuid(),
                EstateId = Guid.NewGuid(),
                Extension = ".jpg"
            };
            var command = new UpdateImageCommand
            {
                Id = image.Id,
                EstateId = image.EstateId,
                Extension = image.Extension
            };

            mapper.Map<Image>(command).Returns(image);
            repository.UpdateAsync(image).Returns(Result<Guid>.Success(image.Id));

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
            var image = new Image
            {
                Id = Guid.NewGuid(),
                EstateId = Guid.NewGuid(),
                Extension = ".jpg"
            };
            var command = new UpdateImageCommand
            {
                Id = image.Id,
                EstateId = image.EstateId,
                Extension = image.Extension
            };

            mapper.Map<Image>(command).Returns(image);
            repository.UpdateAsync(image).Returns(Result<Guid>.Failure("Error"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }
}

