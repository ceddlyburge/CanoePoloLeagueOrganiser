using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace CanoePoloLeagueOrganiser
{
    public class Team
    {
        public string Name { get; }

        public Team(string name)
        {
            this.Name = name;
        }

        public override bool Equals(object team)
        {
            if (team is Team)
                return (team as Team).Name == Name;

            return base.Equals(team);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
