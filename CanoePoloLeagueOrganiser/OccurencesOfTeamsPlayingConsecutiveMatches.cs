using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class OccurencesOfTeamsPlayingConsecutiveMatches
    {
        string lastHomeTeam;
        string lastAwayTeam;
        uint occurences;

        public uint Calculate(Game[] games)
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
