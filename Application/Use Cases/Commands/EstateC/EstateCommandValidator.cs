using Application.Utils;
using FluentValidation;

namespace Application.Use_Cases.Commands.EstateC
{
    public class EstateCommandValidator<T> : AbstractValidator<T> where T : CreateEstateCommand
    {
        public EstateCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'UserId' must be a valid Guid");
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Bedrooms).GreaterThan(0);
            RuleFor(x => x.Bathrooms).GreaterThan(0);
            RuleFor(x => x.LandSize).GreaterThan(0);
            RuleFor(x => x.Street).NotEmpty().MaximumLength(100);
            RuleFor(x => x.City).NotEmpty().MaximumLength(50);
            RuleFor(x => x.State).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(15);
            RuleFor(x => x.HouseSize).GreaterThan(0);
            RuleFor(x => x.ListingData).NotEmpty();
        }
    }
}
