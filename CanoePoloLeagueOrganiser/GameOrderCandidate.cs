using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class GameOrderCandidate
    {
        public IReadOnlyList<Game> GameOrder { get; }
        public uint OccurencesOfTeamsPlayingConsecutiveMatches { get; }
        public uint MaxConsecutiveMatchesByAnyTeam { get; }
        public uint GamesNotPlayedBetweenFirstAndLast { get;}

        public GameOrderCandidate(IReadOnlyList<Game> gameOrder, uint occurencesOfTeamsPlayingConsecutiveMatches, uint maxConsecutiveMatchesByAnyTeam, uint gamesNotPlayedBetweenFirstAndLast)
        {
            // should also check that there are no duplicates
            Contract.Requires(gameOrder != null);

            GameOrder = gameOrder;
            OccurencesOfTeamsPlayingConsecutiveMatches = occurencesOfTeamsPlayingConsecutiveMatches;
            MaxConsecutiveMatchesByAnyTeam = maxConsecutiveMatchesByAnyTeam;
            GamesNotPlayedBetweenFirstAndLast = gamesNotPlayedBetweenFirstAndLast;
        }
    }
}
