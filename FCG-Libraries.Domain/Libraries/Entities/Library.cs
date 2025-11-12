using FCG_Games.Domain.Games.Exceptions;
using FCG_Libraries.Domain.Libraries.Enums;
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

        private Library(Guid id, Guid userId, Guid gameId, EStatus status, decimal? pricePaid) : base(id)
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
        public EStatus Status { get; private set; }
        public decimal? PricePaid { get; private set; }
        #endregion

        #region Factory Methods

        public static Library Create(Guid userId, Guid gameId, EStatus status, decimal? pricePaid)
        {
            if (userId == Guid.Empty)
                throw new InvalidUserException(ErrorMessage.Library.UserRequired);

            //Consulta se o userId existe na base de dados de Users, se não existir, lança exceção
            //TO DO: Implementar essa verificação quando o módulo de Users estiver disponível

            if (gameId == Guid.Empty)
                throw new InvalidGameException(ErrorMessage.Library.GameRequired);

            //Consulta se o gameId existe na base de dados de Games, se não existir, lança exceção
            //TO DO: Implementar essa verificação quando o módulo de Games estiver disponível

            if (!Enum.IsDefined(typeof(EStatus), status))
                throw new InvalidStatusException(ErrorMessage.Library.InvalidStatus);

            if(pricePaid < 0)
                throw new InvalidPriceException(ErrorMessage.Library.PricePaidNegative);

            return new Library(Guid.NewGuid(), userId, gameId, status, pricePaid);
        }

        #endregion






    }
}
