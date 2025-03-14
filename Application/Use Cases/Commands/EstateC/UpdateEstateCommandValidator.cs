using Application.Utils;
using FluentValidation;

namespace Application.Use_Cases.Commands.EstateC
{
    public class UpdateEstateCommandValidator : EstateCommandValidator<UpdateEstateCommand>
    {
        public UpdateEstateCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'Id' must be a valid Guid");
        }
    }
}
