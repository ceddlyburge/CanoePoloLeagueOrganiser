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
    }
}
