using System;
using System.Collections.Generic;

namespace CanoePoloLeagueOrganiser
{
    public class TenSecondPragmatiser : IPragmatiser
    {
        public string Message { get; private set; }

        public TenSecondPragmatiser()
        {
            Message = "";
        }

        public bool AcceptableSolution(TimeSpan timeElapsed, uint lowestOccurencesOfTeamsPlayingConsecutiveMatches)
        {
            if (timeElapsed >= TEN_SECONDS)
            {
                Message = "There are too many teams to analyse all possible combinations, so this is the best solution found after ten seconds of number crunching";
                return true;
            }

            if (timeElapsed >= ONE_SECOND)
            {
                Message = "There are too many teams to analyse all possible combinations, so this is the best solution that has no team playing twice in a row";
                return (lowestOccurencesOfTeamsPlayingConsecutiveMatches == 0);
            }

            return false;
        }

        readonly TimeSpan ONE_SECOND = TimeSpan.FromSeconds(1);
        readonly TimeSpan TEN_SECONDS = TimeSpan.FromSeconds(10);
    }
}