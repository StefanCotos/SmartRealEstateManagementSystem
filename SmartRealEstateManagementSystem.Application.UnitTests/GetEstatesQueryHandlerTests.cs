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
    public class GetEstatesQueryHandlerTests
    {
        private readonly IGenericEntityRepository<Estate> _estateRepository;
        private readonly IMapper _mapper;
        private readonly GetEstatesQueryHandler _handler;

        public GetEstatesQueryHandlerTests()
        {
            _estateRepository = Substitute.For<IGenericEntityRepository<Estate>>();
            _mapper = Substitute.For<IMapper>();
            _handler = new GetEstatesQueryHandler(_estateRepository, _mapper);
        }

        [Fact]
        public async Task Given_GetEstatesQueryHandler_When_HandleIsCalled_Then_AListOfEstatesShouldBeReturned()
        {
            // Arrange
            List<Estate> estates = GenerateEstates();
            _estateRepository.GetAllAsync().Returns(estates);
            var query = new GetEstatesQuery();
            GenerateEstatesDto(estates);
            // Act
            var result = await _handler.Handle(query, CancellationToken.None);
            // Assert
            result.Should().NotBeNull();
            Assert.Equal(2, result.Count);
            Assert.Equal(estates[0].Id, result[0].Id);
        }

        private void GenerateEstatesDto(List<Estate> estates)
        {
            _mapper.Map<List<EstateDto>>(estates).Returns(new List<EstateDto>
            {
                new EstateDto
                {
                    Id = estates[0].Id,
                    UserId = estates[0].UserId,
                    Name = estates[0].Name,
                    Description = estates[0].Description,
                    Price = estates[0].Price,
                    Bedrooms = estates[0].Bedrooms,
                    Bathrooms = estates[0].Bathrooms,
                    LandSize = estates[0].LandSize,
                    Street = estates[0].Street,
                    City = estates[0].City,
                    State = estates[0].State,
                    ZipCode = estates[0].ZipCode,
                    HouseSize = estates[0].HouseSize,
                    ListingData = estates[0].ListingData
                },
                new EstateDto
                {
                    Id = estates[1].Id,
                    UserId = estates[1].UserId,
                    Name = estates[1].Name,
                    Description = estates[1].Description,
                    Price = estates[1].Price,
                    Bedrooms = estates[1].Bedrooms,
                    Bathrooms = estates[1].Bathrooms,
                    LandSize = estates[1].LandSize,
                    Street = estates[1].Street,
                    City = estates[1].City,
                    State = estates[1].State,
                    ZipCode = estates[1].ZipCode,
                    HouseSize = estates[1].HouseSize,
                    ListingData = estates[1].ListingData
                }
            });
        }

        static private List<Estate> GenerateEstates()
        {
            return new List<Estate>
            {
                new Estate
                {
                    Id = new Guid("d2aca8c8-ea05-4303-ad6f-83b41d71f97c"),
                    UserId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                    Name = "Estate 1",
                    Description = "Description 1",
                    Price = 100000,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    LandSize = 1000,
                    Street = "Street 1",
                    City = "City 1",
                    State = "State 1",
                    ZipCode = "ZipCode 1",
                    HouseSize = 1500,
                    ListingData = DateTime.Now
                },
                new Estate
                {
                    Id = new Guid("4fd3f7f1-fd01-4731-8c3d-e865306e0d91"),
                    UserId = new Guid("fb0c0cbf-cf67-4cc8-babc-63d8b24862b7"),
                    Name = "Estate 2",
                    Description = "Description 2",
                    Price = 200000,
                    Bedrooms = 4,
                    Bathrooms = 3,
                    LandSize = 2000,
                    Street = "Street 2",
                    City = "City 2",
                    State = "State 2",
                    ZipCode = "ZipCode 2",
                    HouseSize = 2500,
                    ListingData = DateTime.Now
                }
            };
        }
    }
}
