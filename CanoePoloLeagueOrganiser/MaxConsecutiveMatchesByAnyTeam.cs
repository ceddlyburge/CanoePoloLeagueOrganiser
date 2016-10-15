using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class MaxConsecutiveMatchesByAnyTeam
    {
        // use class variables as an optimisation, I think it will avoid a lot of allocation and deallocation of variables
        uint maxConsecutiveGames;
        uint lastHomeTeamConsecutiveGames;
        uint lastAwayTeamConsecutiveGames;
        string lastHomeTeam;
        string lastAwayTeam;

        public uint Calculate(Game[] games)
        {
            Contract.Requires(games != null);

            maxConsecutiveGames = 1;
            lastHomeTeamConsecutiveGames = 0;
            lastAwayTeamConsecutiveGames = 0;
            lastHomeTeam = null;
            lastAwayTeam = null;

            foreach (var game in games)
            {
                lastHomeTeamConsecutiveGames = (game.Playing(lastHomeTeam) == true) ? lastHomeTeamConsecutiveGames + 1 : 1;
                lastAwayTeamConsecutiveGames = (game.Playing(lastAwayTeam) == true) ? lastAwayTeamConsecutiveGames + 1 : 1;

                maxConsecutiveGames = (maxConsecutiveGames < lastHomeTeamConsecutiveGames) ? lastHomeTeamConsecutiveGames : maxConsecutiveGames;
                maxConsecutiveGames = (maxConsecutiveGames < lastAwayTeamConsecutiveGames) ? lastAwayTeamConsecutiveGames : maxConsecutiveGames;

                lastHomeTeam = game.HomeTeam.Name;
                lastAwayTeam = game.AwayTeam.Name;
            }

            return maxConsecutiveGames;
        }
    }
}
