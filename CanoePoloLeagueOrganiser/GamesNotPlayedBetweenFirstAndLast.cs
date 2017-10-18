using System;
using System.Collections.Generic;
using static System.Diagnostics.Contracts.Contract;
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

        /// <summary>
        /// Calculates GamesNotPlayedBetweenFirstAndLast for a passed in list of games
        /// </summary>
        /// <param name="gamesInAnyOrder">
        /// This calculator expects to be called many times with various permutations of a list of games. It does some setup at the start, for which it needs the list of games. These can be in any order.
        /// </param>
        public GamesNotPlayedBetweenFirstAndLast(Game[] gamesInAnyOrder)
        {
            Requires(gamesInAnyOrder != null);

            Teams = gamesInAnyOrder.Select(g => g.HomeTeam.Name).Concat(gamesInAnyOrder.Select(g => g.AwayTeam.Name)).Distinct().ToList();
            NumberOfGamesPlayed = Teams.ToDictionary(t => t, t => gamesInAnyOrder.Count(g => g.Playing(t)));
            FirstGames = new Dictionary<string, int>();
            LastGames = new Dictionary<string, int>();
        }

        public uint Calculate(Game[] games)
        {
            // Could add a contract to check that games contains the same games that were passed in to the constructor. Not doing so to keep it fast.
            Requires(games != null);

            uint gamesNotPlayedBetweenFirstAndLast = 0;

            foreach (var team in Teams)
                FirstGames[team] = -1;

            for (int i = 0; i < games.Length; i++)
            {
                if (FirstGames[games[i].HomeTeam.Name] == -1)
                    FirstGames[games[i].HomeTeam.Name] = i;

                if (FirstGames[games[i].AwayTeam.Name] == -1)
                    FirstGames[games[i].AwayTeam.Name] = i;

                LastGames[games[i].HomeTeam.Name] = i;
                LastGames[games[i].AwayTeam.Name] = i;
            }

            foreach (var team in Teams)
                gamesNotPlayedBetweenFirstAndLast += (uint)(1 + LastGames[team] - FirstGames[team] - NumberOfGamesPlayed[team]);

            return gamesNotPlayedBetweenFirstAndLast;
        }

    }
}
