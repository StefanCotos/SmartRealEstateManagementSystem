using Application.Utils;
using FluentValidation;


namespace Application.Use_Cases.Commands.ImageC
{
    public class UpdateImageCommandValidator : AbstractValidator<UpdateImageCommand>
    {
        public UpdateImageCommandValidator()
        {
            RuleFor(x => x.EstateId).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'EstateId' must be a valid Guid");
            RuleFor(x => x.Extension)
            .NotEmpty()
            .Matches("^(svg|png|jpg|jpeg|gif)$")
            .WithMessage("'Extension' must be one of the following: svg, png, jpg, jpeg, gif");
            RuleFor(x => x.Id).NotEmpty().Must(GuidValidator.BeAValidGuid).WithMessage("'Id' must be a valid Guid");
        }
    }
}

