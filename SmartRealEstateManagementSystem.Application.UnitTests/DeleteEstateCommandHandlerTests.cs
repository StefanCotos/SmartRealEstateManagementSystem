using Application.Use_Cases.CommandHandlers.EstateCH;
using Application.Use_Cases.Commands.EstateC;
using Domain.Common;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class DeleteEstateCommandHandlerTests
    {
        private readonly IGenericEntityRepository<Estate> _estateRepository;
        private readonly DeleteEstateCommandHandler _handler;

        public DeleteEstateCommandHandlerTests()
        {
            _estateRepository = Substitute.For<IGenericEntityRepository<Estate>>();
            _handler = new DeleteEstateCommandHandler(_estateRepository);
        }

        [Fact]
        public async Task Given_ValidCommand_When_HandleIsCalled_Then_ShouldReturnSuccessResult()
        {
            // Arrange
            var command = new DeleteEstateCommand(new Guid("d2aca8c8-ea05-4303-ad6f-83b41d71f97c"));

            var estate = new Estate
            {
                Id = command.Id,
                Name = "Sample Estate",
                Description = "Sample Description",
                Price = 100000,
                Bedrooms = 3,
                Bathrooms = 2,
                LandSize = 1000,
                Street = "Sample Street",
                City = "Sample City",
                State = "Sample State",
                ZipCode = "12345",
                HouseSize = 2000,
            };

            _estateRepository.DeleteAsync(estate.Id).Returns(Result<Guid>.Success(estate.Id));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be(estate.Id);
            result.ErrorMessage.Should().BeNull();
        }


        [Fact]
        public async Task Given_InvalidCommand_When_HandleIsCalled_Then_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new DeleteEstateCommand(new Guid("d2aca8c8-ea05-4303-ad6f-83b41d71f97c"));

            _estateRepository.DeleteAsync(command.Id).Returns(Result<Guid>.Failure("Estate not found"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(Guid.Empty);
            result.ErrorMessage.Should().Be("Estate not found");
        }

    }
}
