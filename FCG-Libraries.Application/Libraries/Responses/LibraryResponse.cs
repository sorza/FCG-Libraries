using FCG_Libraries.Domain.Libraries.Enums;

namespace FCG_Libraries.Application.Libraries.Responses
{
    public sealed record LibraryResponse(Guid ItemId, Guid UserId, Guid GameId, EStatus Status, decimal? PricePaid);
}
