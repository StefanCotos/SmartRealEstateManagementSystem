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
    public class GetImagesByEstateIdQueryHandlerTests
    {
        private readonly IImageRepository repository;
        private readonly IMapper mapper;
        private readonly GetImagesByEstateIdQueryHandler handler;

        public GetImagesByEstateIdQueryHandlerTests()
        {
            repository = Substitute.For<IImageRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetImagesByEstateIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetImagesByEstateIdQueryHandler_When_HandleIsCalled_Then_TheImagesShouldBeReturned()
        {
            // Arrange
            var estateId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            var images = new List<Image>
            {
                new Image { Id = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"), EstateId = estateId, Extension = ".jpg" }
            };
            repository.GetAllImagesByEstateIdAsync(estateId).Returns(images);
            var query = new GetImagesByEstateIdQuery { Id = estateId };
            var imageDtos = images.Select(i => new ImageDto
            {
                Id = i.Id,
                EstateId = i.EstateId,
                Extension = i.Extension
            }).ToList();
            mapper.Map<List<ImageDto>>(images).Returns(imageDtos);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().EstateId.Should().Be(images.First().EstateId);
        }

        [Fact]
        public async Task Given_GetImagesByEstateIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnEmptyList()
        {
            // Arrange
            var estateId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            repository.GetAllImagesByEstateIdAsync(estateId).Returns(Enumerable.Empty<Image>());
            var query = new GetImagesByEstateIdQuery { Id = estateId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
