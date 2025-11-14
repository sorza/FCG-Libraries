using FCG_Libraries.Domain.Libraries.Enums;

namespace FCG_Libraries.Application.Shared.Interfaces
{
    public record LibraryOrderEvent(Guid ItemId, Guid UserId, Guid GameId, EStatus Status, decimal? PricePaid);
    public record LibraryItemCreatedEvent(Guid ItemId, Guid UserId, Guid GameId, EStatus Status, decimal? PricePaid);

    public interface IEventPublisher
    {
        Task PublishAsync<T>(T evt, string subject);
    }
}
