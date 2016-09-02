using System.Collections.Generic;

namespace CanoePoloLeagueOrganiser
{
    internal class GameOrderStat
    {
        public uint Stat { get; }
        public IReadOnlyList<Game> Games { get; }

        public GameOrderStat(uint stat, IReadOnlyList<Game> games)
        {
            this.Stat = stat;
            this.Games = games;
        }
    }
}