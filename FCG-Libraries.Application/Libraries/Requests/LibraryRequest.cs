using FCG.Shared.Contracts.Enums;

namespace FCG_Libraries.Application.Libraries.Requests
{
    public sealed record LibraryRequest(Guid UserId, Guid GameId, decimal? PricePaid);

}
