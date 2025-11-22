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

            RuleFor(x => x.PaymentType)
                .IsInEnum().WithMessage("Forma de pagamento inválida.");

            RuleFor(x => x.PricePaid)
                .GreaterThanOrEqualTo(0).When(x => x.PricePaid.HasValue).WithMessage("O preço não pode ser negativo.");

        }
    }
}
