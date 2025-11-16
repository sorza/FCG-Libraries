using FCG_Libraries.Domain.Libraries.Enums;

namespace FCG_Libraries.Application.Shared.Interfaces
{  
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T evt, string subject);
    }
}
