using Domain.Common;
using MediatR;

namespace Application.Use_Cases.Commands.EstateC
{
    public class CreateEstateCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required float Price { get; set; }
        public required int Bedrooms { get; set; }
        public required int Bathrooms { get; set; }
        public required float LandSize { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required float HouseSize { get; set; }
        public string? PriceId { get; set; }
        public string? ProductId { get; set; }
        public DateTime ListingData { get; set; }
    }
}