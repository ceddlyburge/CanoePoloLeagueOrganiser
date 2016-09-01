using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class GameOrderCandidate : IComparable
    {
        public IEnumerable<Game> GameOrder { get; }
        public uint OccurencesOfTeamsPlayingConsecutiveMatches { get; }
        public uint MaxConsecutiveMatchesByAnyTeam { get; }
        public uint GamesNotPlayedBetweenFirstAndLast { get;}

        public GameOrderCandidate(IEnumerable<Game> gameOrder, uint occurencesOfTeamsPlayingConsecutiveMatches, uint maxConsecutiveMatchesByAnyTeam, uint gamesNotPlayedBetweenFirstAndLast)
        {
            // should also check that there are no duplicates
            Contract.Requires(gameOrder != null);

            this.GameOrder = gameOrder;
            this.OccurencesOfTeamsPlayingConsecutiveMatches = occurencesOfTeamsPlayingConsecutiveMatches;
            this.MaxConsecutiveMatchesByAnyTeam = maxConsecutiveMatchesByAnyTeam;
            this.GamesNotPlayedBetweenFirstAndLast = gamesNotPlayedBetweenFirstAndLast;
        }

        public int CompareTo(object other)
        {
            Contract.Requires(other != null);
            Contract.Requires(other is GameOrderCandidate);

            return (int) this.OccurencesOfTeamsPlayingConsecutiveMatches - (int) (other as GameOrderCandidate).OccurencesOfTeamsPlayingConsecutiveMatches;
        }

        public bool GameOrderEquals(IEnumerable<Game> theirGames)
        {
            var myGames = this.GameOrder.GetEnumerator();

            foreach (var theirGame in theirGames)
            {
                myGames.MoveNext();
                if (theirGame.SameTeams(myGames.Current) == false) return false;
            }

            return true;
        }
    }
}
