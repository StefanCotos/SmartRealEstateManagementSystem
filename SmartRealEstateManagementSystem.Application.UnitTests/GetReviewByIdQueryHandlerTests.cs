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
    public class GetReviewByIdQueryHandlerTests
    {
        private readonly IGenericEntityRepository<Review> repository;
        private readonly IMapper mapper;
        private readonly GetReviewByIdQueryHandler handler;

        public GetReviewByIdQueryHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Review>>();
            mapper = Substitute.For<IMapper>();
            handler = new GetReviewByIdQueryHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_GetReviewByIdQueryHandler_When_HandleIsCalled_Then_TheReviewShouldBeReturned()
        {
            // Arrange
            var reviewId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            var review = new Review
            {
                Id = reviewId,
                BuyerId = new Guid("b3c7b2a9-715d-4b99-8e1d-43ed32a6a8b2"),
                SellerId = new Guid("c3c7b2a9-715d-4b99-8e1d-43ed32a6a8b3"),
                Description = "Test Review",
                Rating = 5
            };
            repository.GetByIdAsync(reviewId).Returns(review);
            var query = new GetReviewByIdQuery { Id = reviewId };
            var reviewDto = new ReviewDto
            {
                Id = review.Id,
                BuyerId = review.BuyerId,
                SellerId = review.SellerId,
                Description = review.Description,
                Rating = review.Rating
            };
            mapper.Map<ReviewDto>(review).Returns(reviewDto);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(review.Id);
        }

        [Fact]
        public async Task Given_GetReviewByIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnNull()
        {
            // Arrange
            var reviewId = new Guid("a3c7b2a9-715d-4b99-8e1d-43ed32a6a8b1");
            repository.GetByIdAsync(reviewId).Returns((Review?)null);
            var query = new GetReviewByIdQuery { Id = reviewId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}


