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
    public class GetImagesQueryHandlerTests
    {
        private readonly IGenericEntityRepository<Image> repository;
        private readonly IMapper mapper;
        private readonly GetImagesQueryHandler handler;

        public GetImagesQueryHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Image>>();
            mapper = Substitute.For<IMapper>();
            handler = new GetImagesQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetImagesQueryHandler_When_HandleIsCalled_Then_TheImagesShouldBeReturned()
        {
            // Arrange
            var images = new List<Image>
            {
                new Image { Id = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1"), EstateId = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"), Extension = ".jpg" }
            };
            repository.GetAllAsync().Returns(images);
            var query = new GetImagesQuery();
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
            result.First().Id.Should().Be(images.First().Id);
        }

        [Fact]
        public async Task Given_GetImagesQueryHandler_When_HandleIsCalledWithNoImages_Then_ShouldReturnEmptyList()
        {
            // Arrange
            repository.GetAllAsync().Returns(Enumerable.Empty<Image>());
            var query = new GetImagesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}

