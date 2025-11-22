using FCG_Libraries.Domain.Libraries.Enums;

namespace FCG_Libraries.Application.Libraries.Requests
{
    public sealed record LibraryRequest(Guid UserId, Guid GameId, decimal? PricePaid, EPaymentType PaymentType);

}
