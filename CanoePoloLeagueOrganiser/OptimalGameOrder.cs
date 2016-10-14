﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class OptimalGameOrder : IOptimalGameOrder
    {
        readonly IPragmatiser pragmatiser;

        public OptimalGameOrder(IPragmatiser pragmatiser)
        {
            Contract.Requires(pragmatiser != null);
            this.pragmatiser = pragmatiser;
        }

        public GameOrderCandidate CalculateOriginalGameOrder(IReadOnlyList<Game> games)
        {
            Contract.Requires(games != null);

            var gameOrderProperties = new CalculateGameOrderProperties();

            return new GameOrderCandidate(
                new MarkConsecutiveGames().MarkTeamsPlayingConsecutively(games),
                gameOrderProperties.OccurencesOfTeamsPlayingConsecutiveMatches(games.ToArray()), gameOrderProperties.MaxConsecutiveMatchesByAnyTeam(games.ToArray()), gameOrderProperties.GamesNotPlayedBetweenFirstAndLast(games.ToArray()));
        }

        public GameOrderCalculation OptimiseGameOrder(IReadOnlyList<Game> games)
        {
            Contract.Requires(games != null);

            var gameOrderResult = new OptimalGameOrderFromCurtailedList(games, pragmatiser, new Permupotater<Game>(games.ToArray(), new CurtailWhenATeamPlaysTwiceInARow(games).Curtail)).CalculateGameOrder();

            if (gameOrderResult.OptimisedGameOrder != null)
                return gameOrderResult;

            return new OptimalGameOrderFromCurtailedList(games, pragmatiser, new Permupotater<Game>(games.ToArray(), NoCurtailment)).CalculateGameOrder();
        }

        bool NoCurtailment(int[] gameIndexes, int length)
        {
            return false;
        }
    }
}
