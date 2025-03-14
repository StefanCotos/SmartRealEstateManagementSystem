using Application.DTO;
using Application.Use_Cases.Queries.Image;
using Application.Use_Cases.QueryHandlers.ImageQH;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class GetImageByIdQueryHandlerTests
    {
        private readonly IGenericEntityRepository<Image> repository;
        private readonly IMapper mapper;
        private readonly GetImageByIdQueryHandler handler;

        public GetImageByIdQueryHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Image>>();
            mapper = Substitute.For<IMapper>();
            handler = new GetImageByIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetImageByIdQueryHandler_When_HandleIsCalled_Then_TheImageShouldBeReturned()
        {
            // Arrange
            var imageId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            var image = new Image
            {
                Id = imageId,
                EstateId = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"),
                Extension = ".jpg"
            };
            repository.GetByIdAsync(imageId).Returns(image);
            var query = new GetImageByIdQuery { Id = imageId };
            var imageDto = new ImageDto
            {
                Id = image.Id,
                EstateId = image.EstateId,
                Extension = image.Extension
            };
            mapper.Map<ImageDto>(image).Returns(imageDto);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(image.Id);
        }

        [Fact]
        public async Task Given_GetImageByIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnNull()
        {
            // Arrange
            var imageId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            repository.GetByIdAsync(imageId).Returns((Image?)null);
            var query = new GetImageByIdQuery { Id = imageId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
