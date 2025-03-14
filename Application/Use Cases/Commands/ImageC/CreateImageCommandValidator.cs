using Application.Utils;
using FluentValidation;

namespace Application.Use_Cases.Commands.ImageC
{
    public class CreateImageCommandValidator : AbstractValidator<CreateImageCommand>
    {
        public CreateImageCommandValidator()
        {
            RuleFor(x => x.EstateId).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'EstateId' must be a valid Guid");
            RuleFor(x => x.Extension).NotEmpty().MaximumLength(10);
        }
    }
}
