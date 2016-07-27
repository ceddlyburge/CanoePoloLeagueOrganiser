using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace CanoePoloLeagueOrganiser
{
    public class Team
    {
        public Team(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public Game Versus(Team opponent)
        {
            Contract.Requires(opponent != null);

            return new Game(this, opponent);
        }
    }
}
