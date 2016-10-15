using System.Diagnostics.Contracts;

namespace CanoePoloLeagueOrganiser
{
    public class GameOrderPossiblyNullCalculation
    {
        public GameOrderCandidate OptimisedGameOrder { get; }
        public bool PerfectOptimisation { get; }
        public string OptimisationMessage { get; }

        public GameOrderPossiblyNullCalculation(GameOrderCandidate optimisedGameOrder, bool perfectOptimisation, string optimisationMessage)
        {
            OptimisationMessage = optimisationMessage;
            PerfectOptimisation = perfectOptimisation;
            OptimisedGameOrder = optimisedGameOrder;
        }
    }

    public class GameOrderCalculation : GameOrderPossiblyNullCalculation
    {
        public GameOrderCalculation(GameOrderCandidate optimisedGameOrder, bool perfectOptimisation, string optimisationMessage) : base(optimisedGameOrder, perfectOptimisation, optimisationMessage)
        {
            Contract.Requires(optimisedGameOrder != null);
        }
    }
}