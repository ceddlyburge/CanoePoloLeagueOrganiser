using System;

namespace CanoePoloLeagueOrganiser
{
    public class TenSecondPragmatiser : IPragmatiser
    {
        readonly TimeSpan ONE_SECOND = TimeSpan.FromSeconds(1);
        readonly TimeSpan TEN_SECONDS = TimeSpan.FromSeconds(10);

        public bool AcceptableSolution(TimeSpan timeElapsed, uint lowestOccurencesOfTeamsPlayingConsecutiveMatches)
        {
            if (timeElapsed >= TEN_SECONDS)
                return true;

            if (timeElapsed >= ONE_SECOND)
                return (lowestOccurencesOfTeamsPlayingConsecutiveMatches == 0);

            return false;
        }
    }
}