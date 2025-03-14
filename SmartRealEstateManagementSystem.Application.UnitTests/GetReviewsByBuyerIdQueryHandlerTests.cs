using Application.DTO;
using Application.Use_Cases.Queries.ReviewQ;
using Application.Use_Cases.QueryHandlers.ReviewQH;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class GetReviewsByBuyerIdQueryHandlerTests
    {
        private readonly IReviewRepository repository;
        private readonly IMapper mapper;
        private readonly GetReviewsByBuyerIdQueryHandler handler;

        public GetReviewsByBuyerIdQueryHandlerTests()
        {
            repository = Substitute.For<IReviewRepository>();
            mapper = Substitute.For<IMapper>();
            handler = new GetReviewsByBuyerIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetReviewsByBuyerIdQueryHandler_When_HandleIsCalled_Then_TheReviewsShouldBeReturned()
        {
            // Arrange
            var buyerId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            var reviews = new List<Review>
            {
                new Review
                {
                    Id = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"),
                    BuyerId = buyerId,
                    SellerId = new Guid("c3c7b2a9-715d-4b99-8e1d-43ed32a6a8b3"),
                    Description = "Test Review 1",
                    Rating = 5
                },
                new Review
                {
                    Id = new Guid("d3c7b2a9-715d-4b99-8e1d-43ed32a6a8b4"),
                    BuyerId = buyerId,
                    SellerId = new Guid("e3c7b2a9-715d-4b99-8e1d-43ed32a6a8b5"),
                    Description = "Test Review 2",
                    Rating = 4
                }
            };
            repository.GetAllBuyerReviewAsync(buyerId).Returns(reviews);
            var query = new GetReviewsByBuyerIdQuery { Id = buyerId };
            var reviewDtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                BuyerId = r.BuyerId,
                SellerId = r.SellerId,
                Description = r.Description,
                Rating = r.Rating
            }).ToList();
            mapper.Map<List<ReviewDto>>(reviews).Returns(reviewDtos);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Id.Should().Be(reviews.First().Id);
        }

        [Fact]
        public async Task Given_GetReviewsByBuyerIdQueryHandler_When_HandleIsCalledWithNoReviews_Then_ShouldReturnEmptyList()
        {
            // Arrange
            var buyerId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            repository.GetAllBuyerReviewAsync(buyerId).Returns(Enumerable.Empty<Review>());
            var query = new GetReviewsByBuyerIdQuery { Id = buyerId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}


