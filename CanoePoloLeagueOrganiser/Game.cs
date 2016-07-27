using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class Game
    {
        public Game(Team homeTeam, Team awayTeam)
        {
            Contract.Requires(homeTeam != null);
            Contract.Requires(awayTeam != null);
            Contract.Requires(homeTeam != awayTeam);

            this.HomeTeam = homeTeam;
            this.AwayTeam = awayTeam;
        }

        public Team HomeTeam { get; }
        public Team AwayTeam { get; }

        public bool Playing(Team team)
        {
            return (HomeTeam == team || AwayTeam == team);
        }
    }
}
