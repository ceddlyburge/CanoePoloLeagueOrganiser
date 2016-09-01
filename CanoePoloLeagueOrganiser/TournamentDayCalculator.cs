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
        public TournamentDayCalculator(IEnumerable<Game> games)
        {
            Contract.Requires(games != null);

            this.games = games;
        }

        //public IEnumerable<Team> Teams { get; }
        private IEnumerable<Game> games { get; }

        public GameOrderCalculation CalculateGameOrder()
        {
            // generate a list of all possible game orders
            var permutations = new Permupotater<Game>().GetPermutations(this.games);

            // probably interface this out soon
            var marker = new MarkConsecutiveGames();

            // create a list of candidates
            var candidates = permutations.Select(p => new GameOrderCandidate(marker.MarkTeamsPlayingConsecutively(p.ToList()), OccurencesOfTeamsPlayingConsecutiveMatches(p), MaxConsecutiveMatchesByAnyTeam(p), GamesNotPlayedBetweenFirstAndLast(p)));

            // sort by bestness and return the best one
            var orderedCandidates = candidates.OrderBy(c => c.MaxConsecutiveMatchesByAnyTeam).ThenBy(c => c.OccurencesOfTeamsPlayingConsecutiveMatches).ThenBy(c => c.GamesNotPlayedBetweenFirstAndLast);

            return new GameOrderCalculation(optimisedGameOrder: orderedCandidates.First(), originalGameOrder: candidates.Where(c => c.SameOrder(this.games)));
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
                gamesNotPlayedBetweenFirstAndLast += (uint)(lastgame - firstgame - games.Where(g => g.Playing(team)).Count());
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
