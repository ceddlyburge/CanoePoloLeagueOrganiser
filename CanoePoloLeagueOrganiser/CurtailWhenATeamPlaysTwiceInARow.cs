using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CanoePoloLeagueOrganiser
{
    internal class CurtailWhenATeamPlaysTwiceInARow
    {
        IReadOnlyList<Game> Games { get; }

        public CurtailWhenATeamPlaysTwiceInARow(IReadOnlyList<Game> games)
        {
            Contract.Requires(games != null);

            this.Games = games;
        }

        // the permupotater will call the curtailment function after every item in the permutation is fixed, so we don't need to analyse all the games in the permutation for teams playing twice in a row, and can instead just analyse the last two.
        public bool Curtail(int[] gameIndexes, int length)
        {
            // if only the first game is fixed, no teams can be playing consecutively!
            if (length < 1)
                return false;

            var currentGame = this.Games[gameIndexes[length]];
            var previousGame = this.Games[gameIndexes[length - 1]];

            return (currentGame.Playing(previousGame.HomeTeam) || currentGame.Playing(previousGame.AwayTeam));
        }

    }
}