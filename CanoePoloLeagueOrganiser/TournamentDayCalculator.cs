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
        public TournamentDayCalculator(IReadOnlyList<Game> games, IPragmatiser pragmatiser)
        {
            Contract.Requires(games != null);

            this.Games = games;
            this.Candidates = new List<GameOrderCandidate>();
            this.Marker = new MarkConsecutiveGames();
            this.Pragmatiser = pragmatiser;

            // bit naughty doing all this work in the constructor, but it does mean I can make them readonly
            this.GameCountMinus1 = games.Count() - 1;
            this.Teams = this.Games.Select(g => g.HomeTeam.Name).Concat(this.Games.Select(g => g.AwayTeam.Name)).Distinct().ToList();
            this.NumberOfGamesPlayed = this.Teams.ToDictionary(t => t, t => this.Games.Count(g => g.Playing(t)));
            this.FirstGames = new Dictionary<string, int>();
            this.LastGames = new Dictionary<string, int>();
        }

        IReadOnlyList<Game> Games { get; }
        IPragmatiser Pragmatiser { get; }

        int GameCountMinus1 { get; }
        IReadOnlyDictionary<string, int> NumberOfGamesPlayed { get; }
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
        uint permutationCount;
        DateTime timeStartedCalculation;
        // used in MaxConsecutiveMatchesByAnyTeam
        uint maxConsecutiveGames;
        uint lastHomeTeamConsecutiveGames;
        uint lastAwayTeamConsecutiveGames;

        // used in MaxConsecutiveMatchesByAnyTeam and OccurencesOfTeamsPlayingConsecutiveMatches
        string lastHomeTeam;
        string lastAwayTeam;
        uint occurences;

        // used in GamesNotPlayedBetweenFirstAndLast
        IReadOnlyList<string> Teams { get; }
        Dictionary<string, int> FirstGames { get; }
        Dictionary<string, int> LastGames { get; } 

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

            if (this.permutationCount % 1000 == 0)
            {
                return this.Pragmatiser.AcceptableSolution(DateTime.Now.Subtract(this.timeStartedCalculation), this.lowestOccurencesOfTeamsPlayingConsecutiveMatches) == false;
            }

            return true;
        }

        public GameOrderCalculation CalculateGameOrder()
        {
            this.lowestMaxConsecutiveMatchesByAnyTeam = uint.MaxValue;
            this.lowestOccurencesOfTeamsPlayingConsecutiveMatches = uint.MaxValue;
            this.lowestGamesNotPlayedBetweenFirstAndLast = uint.MaxValue;
            this.Candidates.Clear();
            permutationCount = 0;
            timeStartedCalculation = DateTime.Now;

            // generate a list of all possible game orders
            var perfectOptimistaion = new Permupotater().EnumeratePermutations<Game>(this.Games.ToArray(), Callback);

            // sort by bestness and return the best one
            var orderedCandidates = this.Candidates.OrderBy(c => c.MaxConsecutiveMatchesByAnyTeam).ThenBy(c => c.OccurencesOfTeamsPlayingConsecutiveMatches).ThenBy(c => c.GamesNotPlayedBetweenFirstAndLast).ToList();

            return new GameOrderCalculation(optimisedGameOrder: orderedCandidates.First(), originalGameOrder: CalculateOriginalGameOrder(), perfectOptimisation: perfectOptimistaion, optimisationMessage: this.Pragmatiser.Message);
        }

        private GameOrderCandidate CalculateOriginalGameOrder()
        {
            return new GameOrderCandidate(this.Marker.MarkTeamsPlayingConsecutively(this.Games), OccurencesOfTeamsPlayingConsecutiveMatches(this.Games.ToArray()), MaxConsecutiveMatchesByAnyTeam(this.Games.ToArray()), GamesNotPlayedBetweenFirstAndLast(this.Games.ToArray()));
        }

        uint MaxConsecutiveMatchesByAnyTeam(Game[] games)
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

        uint GamesNotPlayedBetweenFirstAndLast(Game[] games)
        {
            Contract.Requires(games != null);

            this.gamesNotPlayedBetweenFirstAndLast = 0;

            foreach (var team in this.Teams)
                this.FirstGames[team] = - 1;

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

        uint OccurencesOfTeamsPlayingConsecutiveMatches(Game[] games)
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
