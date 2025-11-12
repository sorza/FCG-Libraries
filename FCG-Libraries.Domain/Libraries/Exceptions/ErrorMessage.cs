namespace FCG_Games.Domain.Games.Exceptions
{
    public class ErrorMessage
    {
        public static LibraryErrorMessages Library { get; } = new();
    }

    public class LibraryErrorMessages
    {
        public string UserRequired { get; } = "O Id do usuário é obrigatório";
        public string InvalidUser { get; } = "O usuário informado é inválido";
        public string GameRequired { get; } = "O Id do jogo é obrigatório";
        public string InvalidGame { get; } = "O jogo informado é inválido";
        public string InvalidStatus { get; } = "O status informado é inválido";
        public string PricePaidNegative { get; } = "O valor pago não pode ser negativo";
    }
}
