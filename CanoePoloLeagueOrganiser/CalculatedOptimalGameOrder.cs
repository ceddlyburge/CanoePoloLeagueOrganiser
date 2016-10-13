using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class CalculatedOptimalGameOrder : ICalculatedOptimalGameOrder
    {
        readonly IPragmatiser pragmatiser;

        public CalculatedOptimalGameOrder(IPragmatiser pragmatiser)
        {
            Contract.Requires(pragmatiser != null);
            this.pragmatiser = pragmatiser;
        }

        public GameOrderCandidate CalculateOriginalGameOrder(IReadOnlyList<Game> games)
        {
            Contract.Requires(games != null);

            return new CalculatedOptimalGameOrderFromCurtailedList(games, pragmatiser, new Permupotater<Game>(games.ToArray(), new CurtailWhenATeamPlaysTwiceInARow(games).Curtail)).CalculateOriginalGameOrder();
        }

        public GameOrderCalculation OptimiseGameOrder(IReadOnlyList<Game> games)
        {
            Contract.Requires(games != null);

            var gameOrderResult = new CalculatedOptimalGameOrderFromCurtailedList(games, pragmatiser, new Permupotater<Game>(games.ToArray(), new CurtailWhenATeamPlaysTwiceInARow(games).Curtail)).CalculateGameOrder();

            if (gameOrderResult.OptimisedGameOrder != null)
                return gameOrderResult;

            return new CalculatedOptimalGameOrderFromCurtailedList(games, pragmatiser, new Permupotater<Game>(games.ToArray(), NoCurtailment)).CalculateGameOrder();
        }

        bool NoCurtailment(int[] gameIndexes, int length)
        {
            return false;
        }
    }
}
