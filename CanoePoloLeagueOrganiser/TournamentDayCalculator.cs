using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class TournamentDayCalculator
    {
        public TournamentDayCalculator(IReadOnlyList<Game> games)
        {
            Contract.Requires(games != null);

            this.Games = games;
        }

        private IReadOnlyList<Game> Games { get; }

        public GameOrderCalculation CalculateGameOrder()
        {
            uint lowestMaxConsecutiveMatchesByAnyTeam = uint.MaxValue;
            uint lowestOccurencesOfTeamsPlayingConsecutiveMatches = uint.MaxValue;
            uint lowestGamesNotPlayedBetweenFirstAndLast = uint.MaxValue;
            bool addCandidate;

            // probably interface this out soon
            var marker = new MarkConsecutiveGames();

            var candidates = new List<GameOrderCandidate>();

            Func<Game[], bool> callback = (Game[] games) =>
            {
                addCandidate = false;

                uint maxConsecutiveMatchesByAnyTeam = MaxConsecutiveMatchesByAnyTeam(games);
                if (maxConsecutiveMatchesByAnyTeam < lowestMaxConsecutiveMatchesByAnyTeam)
                {
                    lowestMaxConsecutiveMatchesByAnyTeam = maxConsecutiveMatchesByAnyTeam;
                    addCandidate = true;
                }
                else if (maxConsecutiveMatchesByAnyTeam > lowestMaxConsecutiveMatchesByAnyTeam)
                    return true;

                uint occurencesOfTeamsPlayingConsecutiveMatches = OccurencesOfTeamsPlayingConsecutiveMatches(games);
                if (occurencesOfTeamsPlayingConsecutiveMatches < lowestOccurencesOfTeamsPlayingConsecutiveMatches)
                {
                    lowestOccurencesOfTeamsPlayingConsecutiveMatches = occurencesOfTeamsPlayingConsecutiveMatches;
                    addCandidate = true;
                }
                else if (addCandidate == false && occurencesOfTeamsPlayingConsecutiveMatches > lowestOccurencesOfTeamsPlayingConsecutiveMatches)
                    return true;

                uint gamesNotPlayedBetweenFirstAndLast = GamesNotPlayedBetweenFirstAndLast(games);
                if (gamesNotPlayedBetweenFirstAndLast <= lowestGamesNotPlayedBetweenFirstAndLast)
                {
                    lowestGamesNotPlayedBetweenFirstAndLast = gamesNotPlayedBetweenFirstAndLast;
                    addCandidate = true;
                }
                else if (addCandidate == false && gamesNotPlayedBetweenFirstAndLast > lowestGamesNotPlayedBetweenFirstAndLast)
                    return true;

                // we have found a new candidate so add it
                // can optimise by only MarkTeamsPlayingConsecutively on the best result but do this later
                candidates.Add(new GameOrderCandidate(marker.MarkTeamsPlayingConsecutively(this.Games), occurencesOfTeamsPlayingConsecutiveMatches, maxConsecutiveMatchesByAnyTeam, gamesNotPlayedBetweenFirstAndLast));

                return true;
            };

              // generate a list of all possible game orders
            new Permutation().EnumeratePermutations<Game>(this.Games.ToArray(), callback);

            // sort by bestness and return the best one
            var orderedCandidates = candidates.OrderBy(c => c.MaxConsecutiveMatchesByAnyTeam).ThenBy(c => c.OccurencesOfTeamsPlayingConsecutiveMatches).ThenBy(c => c.GamesNotPlayedBetweenFirstAndLast).ToList();

            return new GameOrderCalculation(optimisedGameOrder: orderedCandidates.First(), originalGameOrder: new GameOrderCandidate(marker.MarkTeamsPlayingConsecutively(this.Games), OccurencesOfTeamsPlayingConsecutiveMatches(this.Games), MaxConsecutiveMatchesByAnyTeam(this.Games), GamesNotPlayedBetweenFirstAndLast(this.Games)));
        }

        private uint MaxConsecutiveMatchesByAnyTeam(IEnumerable<Game> games)
        {
            Contract.Requires(games != null);

            uint maxConsecutiveGames = 1;
            uint lastHomeTeamConsecutiveGames = 0;
            uint lastAwayTeamConsecutiveGames = 0;
            Team lastHomeTeam = null;
            Team lastAwayTeam = null;

            foreach (var game in games)
            {
                lastHomeTeamConsecutiveGames = (game.Playing(lastHomeTeam) == true) ? lastHomeTeamConsecutiveGames + 1 : 1;
                lastAwayTeamConsecutiveGames = (game.Playing(lastAwayTeam) == true) ? lastAwayTeamConsecutiveGames + 1 : 1;

                maxConsecutiveGames = (maxConsecutiveGames < lastHomeTeamConsecutiveGames) ? lastHomeTeamConsecutiveGames : maxConsecutiveGames;
                maxConsecutiveGames = (maxConsecutiveGames < lastAwayTeamConsecutiveGames) ? lastAwayTeamConsecutiveGames : maxConsecutiveGames;

                lastHomeTeam = game.HomeTeam;
                lastAwayTeam = game.AwayTeam;
            }

            return maxConsecutiveGames;
        }

        private uint GamesNotPlayedBetweenFirstAndLast(IEnumerable<Game> games)
        {
            Contract.Requires(games != null);

            var teams = games.Select(g => g.HomeTeam.Name).Concat(games.Select(g => g.AwayTeam.Name)).Distinct();
            uint gamesNotPlayedBetweenFirstAndLast = 0;

            foreach (var team in teams)
            {
                int firstgame;
                int lastgame;
                firstgame = games.TakeWhile(g => g.Playing(team) == false).Count();
                lastgame = games.Count() - games.Reverse().TakeWhile(g => g.Playing(team) == false).Count();
                gamesNotPlayedBetweenFirstAndLast += (uint)(lastgame - firstgame - games.Count(g => g.Playing(team)));
            }

            return gamesNotPlayedBetweenFirstAndLast;
        }

        private uint OccurencesOfTeamsPlayingConsecutiveMatches(IEnumerable<Game> games)
        {
            Contract.Requires(games != null);

            uint occurences = 0;
            Team lastHomeTeam = null;
            Team lastAwayTeam = null;

            foreach (var game in games)
            {
                if (game.Playing(lastHomeTeam) == true) occurences++;
                if (game.Playing(lastAwayTeam) == true) occurences++;

                lastHomeTeam = game.HomeTeam;
                lastAwayTeam = game.AwayTeam;
            }

            return occurences;
        }
    }
}
