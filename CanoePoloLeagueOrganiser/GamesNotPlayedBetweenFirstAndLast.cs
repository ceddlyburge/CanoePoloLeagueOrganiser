using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class GamesNotPlayedBetweenFirstAndLast
    {
        IReadOnlyList<string> Teams { get; }
        Dictionary<string, int> FirstGames { get; }
        Dictionary<string, int> LastGames { get; } 
        IReadOnlyDictionary<string, int> NumberOfGamesPlayed { get; }

        uint gamesNotPlayedBetweenFirstAndLast;

        /// <summary>
        /// Calculates GamesNotPlayedBetweenFirstAndLast for a passed in list of games
        /// </summary>
        /// <param name="gamesInAnyOrder">
        /// This calculator expects to be called many times with various permutations of a list of games. It does some setup at the start, for which it needs the list of games. These can be in any order.
        /// </param>
        public GamesNotPlayedBetweenFirstAndLast(Game[] gamesInAnyOrder)
        {
            Contract.Requires(gamesInAnyOrder != null);

            this.Teams = gamesInAnyOrder.Select(g => g.HomeTeam.Name).Concat(gamesInAnyOrder.Select(g => g.AwayTeam.Name)).Distinct().ToList();
            this.NumberOfGamesPlayed = this.Teams.ToDictionary(t => t, t => gamesInAnyOrder.Count(g => g.Playing(t)));
            this.FirstGames = new Dictionary<string, int>();
            this.LastGames = new Dictionary<string, int>();
        }

        public uint Calculate(Game[] games)
        {
            // Could add a contract to check that games contains the same games that were passed in to the constructor. Not doing so to keep it fast.
            Contract.Requires(games != null);

            this.gamesNotPlayedBetweenFirstAndLast = 0;

            foreach (var team in this.Teams)
                this.FirstGames[team] = -1;

            for (int i = 0; i < games.Length; i++)
            {
                if (this.FirstGames[games[i].HomeTeam.Name] == -1)
                    this.FirstGames[games[i].HomeTeam.Name] = i;

                if (this.FirstGames[games[i].AwayTeam.Name] == -1)
                    this.FirstGames[games[i].AwayTeam.Name] = i;

                this.LastGames[games[i].HomeTeam.Name] = i;
                this.LastGames[games[i].AwayTeam.Name] = i;
            }

            foreach (var team in this.Teams)
                this.gamesNotPlayedBetweenFirstAndLast += (uint)(1 + this.LastGames[team] - this.FirstGames[team] - this.NumberOfGamesPlayed[team]);

            return gamesNotPlayedBetweenFirstAndLast;
        }

    }
}
