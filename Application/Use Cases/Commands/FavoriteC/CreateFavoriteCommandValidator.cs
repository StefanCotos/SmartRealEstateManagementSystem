using Application.Utils;
using FluentValidation;

namespace Application.Use_Cases.Commands.FavoriteC
{
    public class CreateFavoriteCommandValidator : AbstractValidator<CreateFavoriteCommand>
    {
        public CreateFavoriteCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'UserId' must be a valid Guid");
            RuleFor(x => x.EstateId).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'EstateId' must be a valid Guid");
        }
    }
}
