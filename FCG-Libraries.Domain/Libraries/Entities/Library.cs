using FCG_Libraries.Domain.Libraries.Enums;
using FCG_Libraries.Domain.Libraries.Exceptions;
using FCG_Libraries.Domain.Libraries.Exceptions.Library;
using FCG_Libraries.Domain.Shared;

namespace FCG_Libraries.Domain.Libraries.Entities
{
    public class Library : Entity
    {
        #region Constructors
        private Library(Guid id) : base(id)
        {
        }

        private Library(Guid id, Guid userId, Guid gameId, EStatus status, decimal? pricePaid, EPaymentType paymentType) : base(id)
        {
            UserId = userId;
            GameId = gameId;
            Status = status;
            PricePaid = pricePaid;
            PaymentType = paymentType;
        }

        #endregion

        #region Properties

        public Guid UserId { get; private set; }
        public Guid GameId { get; private set; }
        public EStatus Status { get; private set; }
        public decimal? PricePaid { get; private set; }
        public EPaymentType PaymentType { get; private set; }
        #endregion

        #region Factory Methods

        public static Library Create(Guid userId, Guid gameId, decimal? pricePaid, EPaymentType paymentType)
        {
            if (userId == Guid.Empty)
                throw new InvalidUserException(ErrorMessage.Library.UserRequired);            

            if (gameId == Guid.Empty)
                throw new InvalidGameException(ErrorMessage.Library.GameRequired);           

            if (!Enum.IsDefined(typeof(EPaymentType), paymentType))
                throw new InvalidPaymentException(ErrorMessage.Library.InvalidPaymentType);

            if(pricePaid < 0)
                throw new InvalidPriceException(ErrorMessage.Library.PricePaidNegative);

            return new Library(Guid.NewGuid(), userId, gameId, EStatus.Requested, pricePaid, paymentType);
        }

        #endregion

        #region Methods

        public void UpdateStatus(EStatus status)
        {
            if (!Enum.IsDefined(typeof(EStatus), status))
                throw new InvalidStatusException(ErrorMessage.Library.InvalidStatus);

            UpdateLastDateChanged();
        }

        #endregion
    }
}
