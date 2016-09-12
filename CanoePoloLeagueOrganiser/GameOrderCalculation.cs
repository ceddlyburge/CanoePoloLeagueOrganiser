using System.Diagnostics.Contracts;

namespace CanoePoloLeagueOrganiser
{
    public class GameOrderCalculation
    {
        public GameOrderCandidate OptimisedGameOrder { get; }
        public GameOrderCandidate OriginalGameOrder { get; }
        public bool PerfectOptimisation { get; }
        public string OptimisationMessage { get; }

        public GameOrderCalculation(GameOrderCandidate optimisedGameOrder, GameOrderCandidate originalGameOrder, bool perfectOptimisation, string optimisationMessage)
        {
            Contract.Requires(optimisedGameOrder != null);
            Contract.Requires(originalGameOrder!= null);

            OptimisationMessage = optimisationMessage;
            PerfectOptimisation = perfectOptimisation;
            OptimisedGameOrder = optimisedGameOrder;
            OriginalGameOrder = originalGameOrder;
        }
    }
}