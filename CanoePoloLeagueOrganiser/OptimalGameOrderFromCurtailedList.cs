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
            Games = games; 
            Candidates = new List<GameOrderCandidate>();
            Marker = new MarkConsecutiveGames();
            Pragmatiser = pragmatiser;
            MaxConsecutiveMatchesByAnyTeam = new MaxConsecutiveMatchesByAnyTeam();
            GamesNotPlayedBetweenFirstAndLast = new GamesNotPlayedBetweenFirstAndLast(games.ToArray());
            OccurencesOfTeamsPlayingConsecutiveMatches = new OccurencesOfTeamsPlayingConsecutiveMatches();
        }

        IPermupotater<Game> Permupotater { get; }
        IReadOnlyList<Game> Games { get; }
        IPragmatiser Pragmatiser { get; }

        List<GameOrderCandidate> Candidates { get; }
        MarkConsecutiveGames Marker { get; }

        // used in Callback
        MaxConsecutiveMatchesByAnyTeam MaxConsecutiveMatchesByAnyTeam { get; }
        GamesNotPlayedBetweenFirstAndLast GamesNotPlayedBetweenFirstAndLast { get; }
        OccurencesOfTeamsPlayingConsecutiveMatches OccurencesOfTeamsPlayingConsecutiveMatches { get; }
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
            bool continuePermupotatering = (permutationCount++ % 1000 != 0)
                 || Pragmatiser.AcceptableSolution(DateTime.Now.Subtract(timeStartedCalculation), lowestOccurencesOfTeamsPlayingConsecutiveMatches) == false;

            addCandidate = false;

            maxConsecutiveMatchesByAnyTeam = MaxConsecutiveMatchesByAnyTeam.Calculate(games);
            if (maxConsecutiveMatchesByAnyTeam < lowestMaxConsecutiveMatchesByAnyTeam)
            {
                lowestMaxConsecutiveMatchesByAnyTeam = maxConsecutiveMatchesByAnyTeam;
                addCandidate = true;
            }
            else if (maxConsecutiveMatchesByAnyTeam > lowestMaxConsecutiveMatchesByAnyTeam)
                return continuePermupotatering;

            occurencesOfTeamsPlayingConsecutiveMatches = OccurencesOfTeamsPlayingConsecutiveMatches.Calculate(games);
            if (occurencesOfTeamsPlayingConsecutiveMatches < lowestOccurencesOfTeamsPlayingConsecutiveMatches)
            {
                lowestOccurencesOfTeamsPlayingConsecutiveMatches = occurencesOfTeamsPlayingConsecutiveMatches;
                addCandidate = true;
            }
            else if (addCandidate == false && occurencesOfTeamsPlayingConsecutiveMatches > lowestOccurencesOfTeamsPlayingConsecutiveMatches)
                return continuePermupotatering;

            gamesNotPlayedBetweenFirstAndLast = GamesNotPlayedBetweenFirstAndLast.Calculate(games);
            if (gamesNotPlayedBetweenFirstAndLast <= lowestGamesNotPlayedBetweenFirstAndLast)
            {
                lowestGamesNotPlayedBetweenFirstAndLast = gamesNotPlayedBetweenFirstAndLast;
                addCandidate = true;
            }
            else if (addCandidate == false)
                return continuePermupotatering;

            // we have found a new candidate so add it
            // can optimise by only MarkTeamsPlayingConsecutively on the best result but do this later. Actually not many candidates get added so it may not be worth it
            Candidates.Add(new GameOrderCandidate(Marker.MarkTeamsPlayingConsecutively(games), occurencesOfTeamsPlayingConsecutiveMatches, maxConsecutiveMatchesByAnyTeam, gamesNotPlayedBetweenFirstAndLast));

            return continuePermupotatering;
        }

        public GameOrderPossiblyNullCalculation CalculateGameOrder()
        {
            Contract.Ensures(Contract.Result<GameOrderPossiblyNullCalculation>() != null);

            lowestMaxConsecutiveMatchesByAnyTeam = uint.MaxValue;
            lowestOccurencesOfTeamsPlayingConsecutiveMatches = uint.MaxValue;
            lowestGamesNotPlayedBetweenFirstAndLast = uint.MaxValue;
            Candidates.Clear();
            permutationCount = 0;
            timeStartedCalculation = DateTime.Now;

            // generate a list of all possible game orders
            Permupotater.EnumeratePermutations(Callback);

            // sort by bestness and return the best one
            var orderedCandidates = Candidates.OrderBy(c => c.MaxConsecutiveMatchesByAnyTeam).ThenBy(c => c.OccurencesOfTeamsPlayingConsecutiveMatches).ThenBy(c => c.GamesNotPlayedBetweenFirstAndLast).ToList();

            return new GameOrderPossiblyNullCalculation(optimisedGameOrder: orderedCandidates.FirstOrDefault(), pragmatisationLevel: Pragmatiser.Level, optimisationMessage: Pragmatiser.Message);
        }
    }
}
