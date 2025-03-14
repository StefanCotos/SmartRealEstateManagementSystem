using Application.Utils;
using FluentValidation;

namespace Application.Use_Cases.Commands.ReviewUserC
{
    public class CreateReviewUserCommandValidator : AbstractValidator<CreateReviewUserCommand>
    {
        public CreateReviewUserCommandValidator() 
        {
            RuleFor(x => x.BuyerId).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'UserId' must be a valid Guid");
            RuleFor(x => x.SellerId).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'ReviewId' must be a valid Guid");
            RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Rating).NotEmpty().InclusiveBetween(1, 10);
        }
    }
}
