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

        public Game(string homeTeam, string awayTeam) : this(new Team(homeTeam), new Team(awayTeam))
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(homeTeam));
            Contract.Requires(!string.IsNullOrWhiteSpace(awayTeam));
        }

        public Team HomeTeam { get; }
        public Team AwayTeam { get; }

        public bool Playing(Team team)
        {
            return (HomeTeam.Equals(team) || AwayTeam.Equals(team));
        }

        public bool Playing(string team)
        {
            return (HomeTeam.Name == team || AwayTeam.Name == team);
        }
    }
}
