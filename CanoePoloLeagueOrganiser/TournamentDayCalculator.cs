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

            this.games = games;
        }

        //public IEnumerable<Team> Teams { get; }
        private IReadOnlyList<Game> games { get; }

        public GameOrderCalculation CalculateGameOrder()
        {
            // generate a list of all possible game orders
            var permutations = new Permupotater<Game>().GetPermutations(this.games);

            // calculate values for MaxConsecutiveMatchesByAnyTeam, which is the first factor we filter on. Filter out any permutations that are not the best
            // this line is fast
            var maxConsecutiveMatchesByAnyTeam = permutations.Select(p => new GameOrderStat(MaxConsecutiveMatchesByAnyTeam(p), p));
            // these two lines are slow
            // calc the best as we go along in the first step
            int bestMaxConsecutiveMatchesByAnyTeam = maxConsecutiveMatchesByAnyTeam.Min(s => (int) s.Stat);
            // not sure how to optimise this
            var filter1 = maxConsecutiveMatchesByAnyTeam.Where(s => s.Stat == bestMaxConsecutiveMatchesByAnyTeam).ToList();
            // the same applies to the other filters, which are slower than I would expect considering that there are a lot less permutations by then.
            // think about how many matches there normally are in a match and validate for that.
            // maybe do subsets of the data and don't get the perfect result but an acceptable one.
            // maybe even have to do a proper search algorithm
            // it looks like the garbage collector is busy so have a think about that.

            //maybe work the logic into the permutator. that way can keep running totals of the best values and not process anything sub that. Yes, this is probably the best way, it will start filtering stuff out early. Could also optimise the gamesnotplayedbetween firstandlast calc

            var occurencesOfTeamsPlayingConsecutiveMatches = filter1.Select(p => new GameOrderStat(OccurencesOfTeamsPlayingConsecutiveMatches(p.Games), p.Games));
            int bestOccurencesOfTeamsPlayingConsecutiveMatches = occurencesOfTeamsPlayingConsecutiveMatches.Min(s => (int)s.Stat);
            var filter2 = occurencesOfTeamsPlayingConsecutiveMatches.Where(s => s.Stat == bestOccurencesOfTeamsPlayingConsecutiveMatches).ToList();

            var gamesNotPlayedBetweenFirstAndLast = filter2.Select(p => new GameOrderStat(GamesNotPlayedBetweenFirstAndLast(p.Games), p.Games));
            int bestGamesNotPlayedBetweenFirstAndLast = gamesNotPlayedBetweenFirstAndLast.Min(s => (int)s.Stat);
            var filter3 = gamesNotPlayedBetweenFirstAndLast.Where(s => s.Stat == bestGamesNotPlayedBetweenFirstAndLast).ToList();

            // probably interface this out soon
            var marker = new MarkConsecutiveGames();

            // create a list of candidates
            var candidates = filter3.Select(p => new GameOrderCandidate(marker.MarkTeamsPlayingConsecutively(p.Games), OccurencesOfTeamsPlayingConsecutiveMatches(p.Games), MaxConsecutiveMatchesByAnyTeam(p.Games), GamesNotPlayedBetweenFirstAndLast(p.Games))).ToList();

            // sort by bestness and return the best one
            var orderedCandidates = candidates.OrderBy(c => c.MaxConsecutiveMatchesByAnyTeam).ThenBy(c => c.OccurencesOfTeamsPlayingConsecutiveMatches).ThenBy(c => c.GamesNotPlayedBetweenFirstAndLast).ToList();

            return new GameOrderCalculation(optimisedGameOrder: orderedCandidates.First(), originalGameOrder: new GameOrderCandidate(marker.MarkTeamsPlayingConsecutively(this.games), OccurencesOfTeamsPlayingConsecutiveMatches(this.games), MaxConsecutiveMatchesByAnyTeam(this.games), GamesNotPlayedBetweenFirstAndLast(this.games)));
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
