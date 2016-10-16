using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class OptimalGameOrder : IOptimalGameOrder
    {
        IPragmatiser Pragmatiser { get; }

        public OptimalGameOrder(IPragmatiser pragmatiser)
        {
            Contract.Requires(pragmatiser != null);
            Pragmatiser = pragmatiser;
        }

        public GameOrderCandidate CalculateOriginalGameOrder(IReadOnlyList<Game> games)
        {
            Contract.Ensures(Contract.Result<GameOrderCandidate>() != null);
            Contract.Requires(games != null);

            return new GameOrderCandidate(
                new MarkConsecutiveGames().MarkTeamsPlayingConsecutively(games),
                new OccurencesOfTeamsPlayingConsecutiveMatches().Calculate(games.ToArray()), new MaxConsecutiveMatchesByAnyTeam().Calculate(games.ToArray()), new GamesNotPlayedBetweenFirstAndLast(games.ToArray()).Calculate(games.ToArray()));
        }

        public GameOrderCalculation OptimiseGameOrder(IReadOnlyList<Game> games)
        {
            Contract.Ensures(Contract.Result<GameOrderCalculation>() != null);
            Contract.Requires(games != null);

            var gameOrder = new OptimalGameOrderFromCurtailedList(games, Pragmatiser, new Permupotater<Game>(games.ToArray(), new CurtailWhenATeamPlaysTwiceInARow(games).Curtail)).CalculateGameOrder();

            if (gameOrder.OptimisedGameOrder == null)
                gameOrder = new OptimalGameOrderFromCurtailedList(games, Pragmatiser, new Permupotater<Game>(games.ToArray(), NoCurtailment)).CalculateGameOrder();

            return new GameOrderCalculation(gameOrder.OptimisedGameOrder, gameOrder.PerfectOptimisation, gameOrder.OptimisationMessage);
        }

        bool NoCurtailment(int[] gameIndexes, int length)
        {
            return false;
        }
    }
}
