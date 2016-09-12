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

        public override bool Equals(object  obj)
        {
            if (obj is Team)
                return (obj as Team).Name == Name;

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
