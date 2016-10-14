namespace CanoePoloLeagueOrganiser
{
    public class GameOrderProperties
    {
        public uint GamesNotPlayedBetweenFirstAndLast { get; }
        public uint MaxConsecutiveMatchesByAnyTeam { get; }
        public uint OccurencesOfTeamsPlayingConsecutiveMatches { get; }

        public GameOrderProperties(uint MaxConsecutiveMatchesByAnyTeam, uint OccurencesOfTeamsPlayingConsecutiveMatches, uint GamesNotPlayedBetweenFirstAndLast)
        {
            this.MaxConsecutiveMatchesByAnyTeam = MaxConsecutiveMatchesByAnyTeam;
            this.OccurencesOfTeamsPlayingConsecutiveMatches = OccurencesOfTeamsPlayingConsecutiveMatches;
            this.GamesNotPlayedBetweenFirstAndLast = GamesNotPlayedBetweenFirstAndLast;
        }
    }
}