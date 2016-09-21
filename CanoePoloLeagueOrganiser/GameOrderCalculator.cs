using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class GameOrderCalculator : IGameOrderCalculator
    {
        readonly IPragmatiser pragmatiser;

        public GameOrderCalculator(IPragmatiser pragmatiser)
        {
            Contract.Requires(pragmatiser != null);

            this.pragmatiser = pragmatiser;
        }

        public GameOrderCandidate CalculateOriginalGameOrder(IReadOnlyList<Game> games)
        {
            return new TournamentDayCalculator(games, pragmatiser).CalculateOriginalGameOrder();
        }

        public GameOrderCalculation OptimiseGameOrder(IReadOnlyList<Game> games)
        {
            return new TournamentDayCalculator(games, pragmatiser).CalculateGameOrder();
        }
    }
}
