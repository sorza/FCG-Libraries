using FCG.Shared.Contracts.Enums;

namespace FCG_Libraries.Application.Libraries.Responses
{
    public sealed record LibraryResponse(Guid ItemId, Guid UserId, Guid GameId, EOrderStatus Status, decimal? PricePaid);
}
