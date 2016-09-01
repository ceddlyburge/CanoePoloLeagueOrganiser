using System.Diagnostics.Contracts;

namespace CanoePoloLeagueOrganiser
{
    public class GameOrderCalculation
    {
        public GameOrderCandidate OptimisedGameOrder { get; }
        public GameOrderCandidate OriginalGameOrder { get; }

        public GameOrderCalculation(GameOrderCandidate optimisedGameOrder, GameOrderCandidate originalGameOrder)
        {
            Contract.Requires(optimisedGameOrder != null);
            Contract.Requires(originalGameOrder!= null);

            OptimisedGameOrder = optimisedGameOrder;
            OriginalGameOrder = originalGameOrder;
        }
    }
}