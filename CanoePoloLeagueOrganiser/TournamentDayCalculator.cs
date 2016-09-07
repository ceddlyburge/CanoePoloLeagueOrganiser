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
            this.Candidates = new List<GameOrderCandidate>();
            this.Marker = new MarkConsecutiveGames();

        }

        IReadOnlyList<Game> Games { get; }
        List<GameOrderCandidate> Candidates { get; }
        // probably interface this out soon
        MarkConsecutiveGames Marker { get; }

        // used in Callback
        uint gamesNotPlayedBetweenFirstAndLast;
        uint maxConsecutiveMatchesByAnyTeam;
        uint occurencesOfTeamsPlayingConsecutiveMatches;
        uint lowestMaxConsecutiveMatchesByAnyTeam;
        uint lowestOccurencesOfTeamsPlayingConsecutiveMatches;
        uint lowestGamesNotPlayedBetweenFirstAndLast;
        bool addCandidate;

        // used in MaxConsecutiveMatchesByAnyTeam
        uint maxConsecutiveGames;
        uint lastHomeTeamConsecutiveGames;
        uint lastAwayTeamConsecutiveGames;

        // used in MaxConsecutiveMatchesByAnyTeam and OccurencesOfTeamsPlayingConsecutiveMatches
        Team lastHomeTeam = null;
        Team lastAwayTeam = null;

        // used in OccurencesOfTeamsPlayingConsecutiveMatches
        uint occurences;

        bool Callback(Game[] games)
        {
            this.addCandidate = false;

            this.maxConsecutiveMatchesByAnyTeam = MaxConsecutiveMatchesByAnyTeam(games);
            if (this.maxConsecutiveMatchesByAnyTeam < this.lowestMaxConsecutiveMatchesByAnyTeam)
            {
                this.lowestMaxConsecutiveMatchesByAnyTeam = this.maxConsecutiveMatchesByAnyTeam;
                this.addCandidate = true;
            }
            else if (this.maxConsecutiveMatchesByAnyTeam > this.lowestMaxConsecutiveMatchesByAnyTeam)
                return true;

            this.occurencesOfTeamsPlayingConsecutiveMatches = OccurencesOfTeamsPlayingConsecutiveMatches(games);
            if (this.occurencesOfTeamsPlayingConsecutiveMatches < this.lowestOccurencesOfTeamsPlayingConsecutiveMatches)
            {
                this.lowestOccurencesOfTeamsPlayingConsecutiveMatches = this.occurencesOfTeamsPlayingConsecutiveMatches;
                this.addCandidate = true;
            }
            else if (this.addCandidate == false && this.occurencesOfTeamsPlayingConsecutiveMatches > this.lowestOccurencesOfTeamsPlayingConsecutiveMatches)
                return true;

            this.gamesNotPlayedBetweenFirstAndLast = GamesNotPlayedBetweenFirstAndLast(games);
            if (this.gamesNotPlayedBetweenFirstAndLast <= this.lowestGamesNotPlayedBetweenFirstAndLast)
            {
                this.lowestGamesNotPlayedBetweenFirstAndLast = this.gamesNotPlayedBetweenFirstAndLast;
                this.addCandidate = true;
            }
            else if (this.addCandidate == false && this.gamesNotPlayedBetweenFirstAndLast > this.lowestGamesNotPlayedBetweenFirstAndLast)
                return true;

            // we have found a new candidate so add it
            // can optimise by only MarkTeamsPlayingConsecutively on the best result but do this later. Actually not many candidates get added so it may not be worth it
            this.Candidates.Add(new GameOrderCandidate(this.Marker.MarkTeamsPlayingConsecutively(games), this.occurencesOfTeamsPlayingConsecutiveMatches, this.maxConsecutiveMatchesByAnyTeam, this.gamesNotPlayedBetweenFirstAndLast));

            return true;
        }

        public GameOrderCalculation CalculateGameOrder()
        {
            this.lowestMaxConsecutiveMatchesByAnyTeam = uint.MaxValue;
            this.lowestOccurencesOfTeamsPlayingConsecutiveMatches = uint.MaxValue;
            this.lowestGamesNotPlayedBetweenFirstAndLast = uint.MaxValue;
            this.Candidates.Clear();

            // generate a list of all possible game orders
            new Permutation().EnumeratePermutations<Game>(this.Games.ToArray(), Callback);

            // sort by bestness and return the best one
            var orderedCandidates = this.Candidates.OrderBy(c => c.MaxConsecutiveMatchesByAnyTeam).ThenBy(c => c.OccurencesOfTeamsPlayingConsecutiveMatches).ThenBy(c => c.GamesNotPlayedBetweenFirstAndLast).ToList();

            return new GameOrderCalculation(optimisedGameOrder: orderedCandidates.First(), originalGameOrder: new GameOrderCandidate(this.Marker.MarkTeamsPlayingConsecutively(this.Games), OccurencesOfTeamsPlayingConsecutiveMatches(this.Games), MaxConsecutiveMatchesByAnyTeam(this.Games), GamesNotPlayedBetweenFirstAndLast(this.Games)));
        }

        uint MaxConsecutiveMatchesByAnyTeam(IEnumerable<Game> games)
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

                this.lastHomeTeam = game.HomeTeam;
                this.lastAwayTeam = game.AwayTeam;
            }

            return this.maxConsecutiveGames;
        }

        uint GamesNotPlayedBetweenFirstAndLast(IEnumerable<Game> games)
        {
            Contract.Requires(games != null);

            var teams = games.Select(g => g.HomeTeam.Name).Concat(games.Select(g => g.AwayTeam.Name)).Distinct();
            this.gamesNotPlayedBetweenFirstAndLast = 0;

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

        uint OccurencesOfTeamsPlayingConsecutiveMatches(IEnumerable<Game> games)
        {
            Contract.Requires(games != null);

            occurences = 0;

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
