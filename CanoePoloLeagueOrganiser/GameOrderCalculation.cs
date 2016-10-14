using System.Diagnostics.Contracts;

namespace CanoePoloLeagueOrganiser
{
    public class GameOrderCalculation
    {
        public GameOrderCandidate OptimisedGameOrder { get; }
        public bool PerfectOptimisation { get; }
        public string OptimisationMessage { get; }

        public GameOrderCalculation(GameOrderCandidate optimisedGameOrder, bool perfectOptimisation, string optimisationMessage)
        {
            Contract.Requires(optimisedGameOrder != null);

            OptimisationMessage = optimisationMessage;
            PerfectOptimisation = perfectOptimisation;
            OptimisedGameOrder = optimisedGameOrder;
        }
    }
}