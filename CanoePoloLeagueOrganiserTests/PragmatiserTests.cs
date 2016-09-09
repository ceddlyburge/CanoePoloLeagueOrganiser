using CanoePoloLeagueOrganiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CanoePoloLeagueOrganiserTests
{
    public class PragmatiserTests
    {
        readonly TimeSpan ONE_SECOND = TimeSpan.FromSeconds(1);
        readonly TimeSpan TEN_SECONDS = TimeSpan.FromSeconds(10);   
        const uint NO_CONSECUTIVE_MATCHES_BY_ANY_TEAM = 0;
        const uint NO_TEAMS_PLAYING_CONSECUTIVE_MATCHES = 0;
        const uint A_LOT = uint.MaxValue;

        [Fact]
        public void AfterOneSecondDontWorryAboutGamesNotPlayed()
        {
            var acceptableSolution = new TenSecondPragmatiser().AcceptableSolution(ONE_SECOND, lowestOccurencesOfTeamsPlayingConsecutiveMatches: 0);

            Assert.True(acceptableSolution);
        }

        [Fact]
        public void AfterTenSecondsJustStop()
        {
            var acceptableSolution = new TenSecondPragmatiser().AcceptableSolution(TEN_SECONDS, lowestOccurencesOfTeamsPlayingConsecutiveMatches: A_LOT);

            Assert.True(acceptableSolution);
        }
    }
}
