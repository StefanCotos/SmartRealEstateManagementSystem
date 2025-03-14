using Application.Utils;
using FluentValidation;

namespace Application.Use_Cases.Commands.EstateC 
{
    public class CreateEstateCommandValidator : EstateCommandValidator<CreateEstateCommand>
    {
        public CreateEstateCommandValidator()
        {
        }
    }
}
