using FCG_Libraries.Domain.Libraries.Exceptions;
using FCG_Libraries.Domain.Libraries.Exceptions.Library;
using FCG_Libraries.Domain.Shared;
using FCG.Shared.Contracts.Enums;

namespace FCG_Libraries.Domain.Libraries.Entities
{
    public class Library : Entity
    {
        #region Constructors
        private Library(Guid id) : base(id)
        {
        }

        private Library(Guid id, Guid userId, Guid gameId, EOrderStatus status, decimal? pricePaid) : base(id)
        {
            UserId = userId;
            GameId = gameId;
            Status = status;
            PricePaid = pricePaid;
        }

        #endregion

        #region Properties

        public Guid UserId { get; private set; }
        public Guid GameId { get; private set; }
        public EOrderStatus Status { get; private set; }
        public decimal? PricePaid { get; private set; }
        #endregion

        #region Factory Methods

        public static Library Create(Guid userId, Guid gameId, decimal? pricePaid)
        {
            if (userId == Guid.Empty)
                throw new InvalidUserException(ErrorMessage.Library.UserRequired);

            if (gameId == Guid.Empty)
                throw new InvalidGameException(ErrorMessage.Library.GameRequired);       

            if(pricePaid < 0)
                throw new InvalidPriceException(ErrorMessage.Library.PricePaidNegative);

            return new Library(Guid.NewGuid(), userId, gameId, EOrderStatus.Requested, pricePaid);
        }

        #endregion

        #region Methods

        public void UpdateStatus(EOrderStatus status)
        {
            if (!Enum.IsDefined(typeof(EOrderStatus), status))
                throw new InvalidStatusException(ErrorMessage.Library.InvalidStatus);

            UpdateLastDateChanged();
        }

        #endregion
    }
}
