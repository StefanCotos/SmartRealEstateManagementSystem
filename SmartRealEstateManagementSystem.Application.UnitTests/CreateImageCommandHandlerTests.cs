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
    public class CreateImageCommandHandlerTests
    {
        private readonly IGenericEntityRepository<Image> _imageRepository;
        private readonly IMapper _mapper;
        private readonly CreateImageCommandHandler _handler;

        public CreateImageCommandHandlerTests()
        {
            _imageRepository = Substitute.For<IGenericEntityRepository<Image>>();
            _mapper = Substitute.For<IMapper>();
            _handler = new CreateImageCommandHandler(_imageRepository, _mapper);
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
            var command = new CreateImageCommand
            {
                EstateId = image.EstateId,
                Extension = image.Extension
            };

            _mapper.Map<Image>(command).Returns(image);
            _imageRepository.AddAsync(image).Returns(Result<Guid>.Success(image.Id));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

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
            var command = new CreateImageCommand
            {
                EstateId = image.EstateId,
                Extension = image.Extension
            };

            _mapper.Map<Image>(command).Returns(image);
            _imageRepository.AddAsync(image).Returns(Result<Guid>.Failure("Error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }
}
