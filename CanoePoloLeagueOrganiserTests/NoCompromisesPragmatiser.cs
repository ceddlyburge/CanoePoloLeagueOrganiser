﻿using CanoePoloLeagueOrganiser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiserTests
{
    internal class NoCompromisesPragmatiser : IPragmatiser
    {
        public bool AcceptableSolution(TimeSpan timeElapsed, uint lowestOccurencesOfTeamsPlayingConsecutiveMatches)
        {
            return false;
        }
    }
}