using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class OptimalGameOrderFromCurtailedList
    {

        public OptimalGameOrderFromCurtailedList(IReadOnlyList<Game> games, IPragmatiser pragmatiser, IPermupotater<Game> permupotater)
        {
            Contract.Requires(pragmatiser != null);
            Contract.Requires(permupotater != null);
            Contract.Requires(games != null);

            Permupotater = permupotater;
            this.Games = games; 
            this.Candidates = new List<GameOrderCandidate>();
            this.Marker = new MarkConsecutiveGames();
            this.Pragmatiser = pragmatiser;
            CalculateGameOrderProperties = new CalculateGameOrderProperties();

        }

        IPermupotater<Game> Permupotater { get; }
        IReadOnlyList<Game> Games { get; }
        IPragmatiser Pragmatiser { get; }

        List<GameOrderCandidate> Candidates { get; }
        MarkConsecutiveGames Marker { get; }

        // used in Callback
        CalculateGameOrderProperties CalculateGameOrderProperties { get; }
        uint gamesNotPlayedBetweenFirstAndLast;
        uint maxConsecutiveMatchesByAnyTeam;
        uint occurencesOfTeamsPlayingConsecutiveMatches;
        uint lowestMaxConsecutiveMatchesByAnyTeam;
        uint lowestOccurencesOfTeamsPlayingConsecutiveMatches;
        uint lowestGamesNotPlayedBetweenFirstAndLast;
        bool addCandidate;
        uint permutationCount;
        DateTime timeStartedCalculation;

        bool Callback(Game[] games)
        {
            bool continuePermupotatering = (this.permutationCount++ % 1000 != 0)
                 || this.Pragmatiser.AcceptableSolution(DateTime.Now.Subtract(this.timeStartedCalculation), this.lowestOccurencesOfTeamsPlayingConsecutiveMatches) == false;

            this.addCandidate = false;

            this.maxConsecutiveMatchesByAnyTeam = CalculateGameOrderProperties.MaxConsecutiveMatchesByAnyTeam(games);
            if (this.maxConsecutiveMatchesByAnyTeam < this.lowestMaxConsecutiveMatchesByAnyTeam)
            {
                this.lowestMaxConsecutiveMatchesByAnyTeam = this.maxConsecutiveMatchesByAnyTeam;
                this.addCandidate = true;
            }
            else if (this.maxConsecutiveMatchesByAnyTeam > this.lowestMaxConsecutiveMatchesByAnyTeam)
                return continuePermupotatering;

            this.occurencesOfTeamsPlayingConsecutiveMatches = CalculateGameOrderProperties.OccurencesOfTeamsPlayingConsecutiveMatches(games);
            if (this.occurencesOfTeamsPlayingConsecutiveMatches < this.lowestOccurencesOfTeamsPlayingConsecutiveMatches)
            {
                this.lowestOccurencesOfTeamsPlayingConsecutiveMatches = this.occurencesOfTeamsPlayingConsecutiveMatches;
                this.addCandidate = true;
            }
            else if (this.addCandidate == false && this.occurencesOfTeamsPlayingConsecutiveMatches > this.lowestOccurencesOfTeamsPlayingConsecutiveMatches)
                return continuePermupotatering;

            this.gamesNotPlayedBetweenFirstAndLast = CalculateGameOrderProperties.GamesNotPlayedBetweenFirstAndLast(games);
            if (this.gamesNotPlayedBetweenFirstAndLast <= this.lowestGamesNotPlayedBetweenFirstAndLast)
            {
                this.lowestGamesNotPlayedBetweenFirstAndLast = this.gamesNotPlayedBetweenFirstAndLast;
                this.addCandidate = true;
            }
            else if (this.addCandidate == false && this.gamesNotPlayedBetweenFirstAndLast > this.lowestGamesNotPlayedBetweenFirstAndLast)
                return continuePermupotatering;

            // we have found a new candidate so add it
            // can optimise by only MarkTeamsPlayingConsecutively on the best result but do this later. Actually not many candidates get added so it may not be worth it
            this.Candidates.Add(new GameOrderCandidate(this.Marker.MarkTeamsPlayingConsecutively(games), this.occurencesOfTeamsPlayingConsecutiveMatches, this.maxConsecutiveMatchesByAnyTeam, this.gamesNotPlayedBetweenFirstAndLast));

            return continuePermupotatering;
        }

        public GameOrderCalculation CalculateGameOrder()
        {
            this.lowestMaxConsecutiveMatchesByAnyTeam = uint.MaxValue;
            this.lowestOccurencesOfTeamsPlayingConsecutiveMatches = uint.MaxValue;
            this.lowestGamesNotPlayedBetweenFirstAndLast = uint.MaxValue;
            this.Candidates.Clear();
            this.permutationCount = 0;
            this.timeStartedCalculation = DateTime.Now;

            // generate a list of all possible game orders
            var perfectOptimistaion = this.Permupotater.EnumeratePermutations(Callback);

            // sort by bestness and return the best one
            var orderedCandidates = this.Candidates.OrderBy(c => c.MaxConsecutiveMatchesByAnyTeam).ThenBy(c => c.OccurencesOfTeamsPlayingConsecutiveMatches).ThenBy(c => c.GamesNotPlayedBetweenFirstAndLast).ToList();

            return new GameOrderCalculation(optimisedGameOrder: orderedCandidates.FirstOrDefault(), perfectOptimisation: perfectOptimistaion, optimisationMessage: this.Pragmatiser.Message);
        }

    }
}
