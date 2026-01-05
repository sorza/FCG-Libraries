using FCG_Libraries.Application.Libraries.Requests;
using FluentValidation;

namespace FCG_Libraries.Application.Libraries.Validators
{
    public class LibraryRequestValidator : AbstractValidator<LibraryRequest>
    {
        public LibraryRequestValidator() 
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId é obrigatório.");

            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage("GameId é obrigatório.");

        }
    }
}
