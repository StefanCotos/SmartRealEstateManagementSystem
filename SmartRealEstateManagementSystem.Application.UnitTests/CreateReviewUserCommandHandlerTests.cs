using Application.Use_Cases.CommandHandlers.ReviewUserCH;
using Application.Use_Cases.Commands.ReviewUserC;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class CreateReviewUserCommandHandlerTests
    {
        private readonly IGenericEntityRepository<Review> repository;
        private readonly IMapper mapper;
        private readonly CreateReviewUserCommandHandler handler;

        public CreateReviewUserCommandHandlerTests()
        {
            repository = Substitute.For<IGenericEntityRepository<Review>>();
            mapper = Substitute.For<IMapper>();
            handler = new CreateReviewUserCommandHandler(repository, mapper);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var review = new Review
            {
                Id = Guid.NewGuid(),
                BuyerId = Guid.NewGuid(),
                SellerId = Guid.NewGuid(),
                Description = "Great service",
                Rating = 5
            };
            var command = new CreateReviewUserCommand
            {
                BuyerId = review.BuyerId,
                SellerId = review.SellerId,
                Description = review.Description,
                Rating = review.Rating
            };

            mapper.Map<Review>(command).Returns(review);
            repository.AddAsync(review).Returns(Result<Guid>.Success(review.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(review.Id);
            result.ErrorMessage.Should().BeNull();
        }

        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var review = new Review
            {
                Id = Guid.NewGuid(),
                BuyerId = Guid.NewGuid(),
                SellerId = Guid.NewGuid(),
                Description = "Great service",
                Rating = 5
            };
            var command = new CreateReviewUserCommand
            {
                BuyerId = review.BuyerId,
                SellerId = review.SellerId,
                Description = review.Description,
                Rating = review.Rating
            };

            mapper.Map<Review>(command).Returns(review);
            repository.AddAsync(review).Returns(Result<Guid>.Failure("Error"));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().NotBeNullOrEmpty();
        }
    }
}

