using static System.Diagnostics.Contracts.Contract;

namespace CanoePoloLeagueOrganiser
{
    public class GameOrderPossiblyNullCalculation
    {
        public GameOrderCandidate OptimisedGameOrder { get; }
        public PragmatisationLevel PragmatisationLevel { get; }
        public string OptimisationMessage { get; }

        public GameOrderPossiblyNullCalculation(GameOrderCandidate optimisedGameOrder, PragmatisationLevel pragmatisationLevel, string optimisationMessage)
        {
            OptimisationMessage = optimisationMessage;
            PragmatisationLevel = pragmatisationLevel;
            OptimisedGameOrder = optimisedGameOrder;
        }
    }

    public class GameOrderCalculation : GameOrderPossiblyNullCalculation
    {
        public GameOrderCalculation(GameOrderCandidate optimisedGameOrder, PragmatisationLevel pragmatisationLevel, string optimisationMessage) : base(optimisedGameOrder, pragmatisationLevel, optimisationMessage)
        {
            Requires(optimisedGameOrder != null);
        }
    }
}