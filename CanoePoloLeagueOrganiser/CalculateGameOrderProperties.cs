using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class CalculateGameOrderProperties
    {
        public GameOrderProperties Calculate(Game[] games)
        {
            // bit naughty doing all this work in the constructor, but it does mean I can make them readonly
            this.GameCountMinus1 = games.Count() - 1;
            this.Teams = games.Select(g => g.HomeTeam.Name).Concat(games.Select(g => g.AwayTeam.Name)).Distinct().ToList();
            this.NumberOfGamesPlayed = this.Teams.ToDictionary(t => t, t => games.Count(g => g.Playing(t)));
            this.FirstGames = new Dictionary<string, int>();
            this.LastGames = new Dictionary<string, int>();

            return new GameOrderProperties(
                MaxConsecutiveMatchesByAnyTeam: MaxConsecutiveMatchesByAnyTeam(games),
                OccurencesOfTeamsPlayingConsecutiveMatches: OccurencesOfTeamsPlayingConsecutiveMatches(games),
                GamesNotPlayedBetweenFirstAndLast: GamesNotPlayedBetweenFirstAndLast(games)
                );
        }

        uint gamesNotPlayedBetweenFirstAndLast;
        int GameCountMinus1;
        IReadOnlyDictionary<string, int> NumberOfGamesPlayed;

        // used in MaxConsecutiveMatchesByAnyTeam
        uint maxConsecutiveGames;
        uint lastHomeTeamConsecutiveGames;
        uint lastAwayTeamConsecutiveGames;

        // used in MaxConsecutiveMatchesByAnyTeam and OccurencesOfTeamsPlayingConsecutiveMatches
        string lastHomeTeam;
        string lastAwayTeam;
        uint occurences;

        // used in GamesNotPlayedBetweenFirstAndLast
        IReadOnlyList<string> Teams;
        Dictionary<string, int> FirstGames;
        Dictionary<string, int> LastGames;

        public uint MaxConsecutiveMatchesByAnyTeam(Game[] games)
        {
            Contract.Requires(games != null);

            this.maxConsecutiveGames = 1;
            this.lastHomeTeamConsecutiveGames = 0;
            this.lastAwayTeamConsecutiveGames = 0;
            this.lastHomeTeam = null;
            this.lastAwayTeam = null;

            foreach (var game in games)
            {
                this.lastHomeTeamConsecutiveGames = (game.Playing(lastHomeTeam) == true) ? this.lastHomeTeamConsecutiveGames + 1 : 1;
                this.lastAwayTeamConsecutiveGames = (game.Playing(lastAwayTeam) == true) ? this.lastAwayTeamConsecutiveGames + 1 : 1;

                this.maxConsecutiveGames = (this.maxConsecutiveGames < this.lastHomeTeamConsecutiveGames) ? this.lastHomeTeamConsecutiveGames : this.maxConsecutiveGames;
                this.maxConsecutiveGames = (this.maxConsecutiveGames < this.lastAwayTeamConsecutiveGames) ? this.lastAwayTeamConsecutiveGames : this.maxConsecutiveGames;

                this.lastHomeTeam = game.HomeTeam.Name;
                this.lastAwayTeam = game.AwayTeam.Name;
            }

            return this.maxConsecutiveGames;
        }

        public uint GamesNotPlayedBetweenFirstAndLast(Game[] games)
        {
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

        public uint OccurencesOfTeamsPlayingConsecutiveMatches(Game[] games)
        {
            Contract.Requires(games != null);

            occurences = 0;

            foreach (var game in games)
            {
                if (game.Playing(lastHomeTeam) == true) occurences++;
                if (game.Playing(lastAwayTeam) == true) occurences++;

                lastHomeTeam = game.HomeTeam.Name;
                lastAwayTeam = game.AwayTeam.Name;
            }

            return occurences;
        }
    }
}
