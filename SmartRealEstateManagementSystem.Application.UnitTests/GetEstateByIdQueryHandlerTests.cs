using Application.DTO;
using Application.Use_Cases.Queries.EstateQ;
using Application.Use_Cases.QueryHandlers.EstateQH;
using AutoMapper;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using NSubstitute;

namespace SmartRealEstateManagementSystem.Application.UnitTests
{
    public class GetEstateByIdQueryHandlerTests
    {
        private readonly IGenericEntityRepository<Estate> _estateRepository;
        private readonly IMapper _mapper;
        private readonly GetEstateByIdQueryHandler handler;

        public GetEstateByIdQueryHandlerTests()
        {
            _estateRepository = Substitute.For<IGenericEntityRepository<Estate>>();
            _mapper = Substitute.For<IMapper>();
            handler = new GetEstateByIdQueryHandler(_estateRepository, _mapper);
        }

        [Fact]
        public async Task Given_GetEstateByIdQueryHandler_When_HandleIsCalled_Then_TheEstateShouldBeReturned()
        { 
            // Arrange
            var estate = new Estate
            {
                Id = new Guid("d2aca8c8-ea05-4303-ad6f-83b41d71f97c"),
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
            _estateRepository.GetByIdAsync(estate.Id).Returns(estate);
            var query = new GetEstateByIdQuery { Id = estate.Id };
            var estateDto = new EstateDto
            {
                Id = estate.Id,
                Name = estate.Name,
                Description = estate.Description,
                Price = estate.Price,
                Bedrooms = estate.Bedrooms,
                Bathrooms = estate.Bathrooms,
                LandSize = estate.LandSize,
                Street = estate.Street,
                City = estate.City,
                State = estate.State,
                ZipCode = estate.ZipCode,
                HouseSize = estate.HouseSize,
            };
            _mapper.Map<EstateDto>(estate).Returns(estateDto);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(estate.Id);
            result.Name.Should().Be(estate.Name);
            result.Description.Should().Be(estate.Description);
            result.Price.Should().Be(estate.Price);
            result.Bedrooms.Should().Be(estate.Bedrooms);
            result.Bathrooms.Should().Be(estate.Bathrooms);
            result.LandSize.Should().Be(estate.LandSize);
            result.Street.Should().Be(estate.Street);
            result.City.Should().Be(estate.City);
            result.State.Should().Be(estate.State);
            result.ZipCode.Should().Be(estate.ZipCode);
            result.HouseSize.Should().Be(estate.HouseSize);
        }

        [Fact]
        public async Task Given_GetEstateByIdQueryHandler_When_HandleIsCalledWithInexistentId_Then_ShouldReturnNull()
        {
            // Arrange
            var query = new GetEstateByIdQuery { Id = new Guid("d2aca8c8-ea05-4303-ad6f-83b41d71f97c") };
            _estateRepository.GetByIdAsync(query.Id).Returns((Estate?)null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
