using System;
using System.Collections.Generic;
using static System.Diagnostics.Contracts.Contract;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class MarkConsecutiveGames
    {
        public IReadOnlyList<Game> MarkTeamsPlayingConsecutively(IReadOnlyList<Game> games)
        {
            Requires(games != null);

            var gamesWithConsecutiveMatchesMarked = new List<Game>();

            for (int i = 0; i < games.Count; i++)
            {
                gamesWithConsecutiveMatchesMarked.Add(MarkTeamsPlayingConsecutively(games[i], PreviousGame(games, i), NextGame(games, i)));
            }

            return gamesWithConsecutiveMatchesMarked;
        }

        Game MarkTeamsPlayingConsecutively(Game game, Game previousGame, Game nextGame)
        {
            bool homeTeamPlayingInPreviousGame = (previousGame != null) && previousGame.Playing(game.HomeTeam);

            bool awayTeamPlayingInPreviousGame = (previousGame != null) && previousGame.Playing(game.AwayTeam);

            bool homeTeamPlayingInNextGame = (nextGame != null) && nextGame.Playing(game.HomeTeam);

            bool awayTeamPlayingInNextGame = (nextGame != null) && nextGame.Playing(game.AwayTeam);

            return new Game(game.HomeTeam, game.AwayTeam, homeTeamPlayingInPreviousGame || homeTeamPlayingInNextGame, awayTeamPlayingInPreviousGame || awayTeamPlayingInNextGame);
        }

        Game NextGame(IReadOnlyList<Game> games, int i)
        {
            int next = i + 1;

            return (next < games.Count) ? games[next] : null;
        }

        Game PreviousGame(IReadOnlyList<Game> games, int i)
        {
            int next = i - 1;

            return (next >= 0) ? games[next] : null;
        }

    }
}
