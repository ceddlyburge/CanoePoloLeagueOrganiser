using System;
using CanoePoloLeagueOrganiser;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CanoePoloLeagueOrganiserTests
{
    // aims:
    // - The maximum consecutive games a team plays should be minimised
    // - The number of times a team plays consecutive matches should be minimised
    // - the amount of games that teams don't play between their first and last games should be minimised
    public class TournamentDayTests
    {
        const int ANY_INT = 3; // if used the value of this variable should not affect the result of a test

        [Fact]
        public void OneInputGameShouldResultInThisGameBeingPlayed()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea"),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.Equal(1, sut.OptimisedGameOrder.GameOrder.Count());
        }

        [Fact]
        public void CastleShouldNotPlayTwiceInARow()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea"),
                 new Game("Castle", "Avon"),
                 new Game("Ulu", "Letchworth"),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.False(PlayingTwiceInARow("Castle", sut.OptimisedGameOrder.GameOrder));
        }

        [Fact]
        public void CastleAndLetchworthAndAvonShouldNotPlayTwiceInARow()
        {
            var games = new List<Game> {
                 new Game("Ulu", "Letchworth"),
                 new Game("Battersea", "Letchworth"),
                 new Game("Castle", "Avon"),
                 new Game("Castle", "Avon"),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder().OptimisedGameOrder;

            Assert.False(PlayingTwiceInARow("Castle", sut.GameOrder));
            Assert.False(PlayingTwiceInARow("Letchworth", sut.GameOrder));
            Assert.False(PlayingTwiceInARow("Avon", sut.GameOrder));
        }

        [Fact]
        public void CastleShouldNotPlayThriceInARow()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea"),
                 new Game("Castle", "Letchworth"),
                 new Game("Ulu", "Castle"),
                 new Game("Battersea", "Letchworth"),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.Equal((uint)2, sut.OptimisedGameOrder.MaxConsecutiveMatchesByAnyTeam);
        }

        [Fact]
        public void NobodyShouldNotPlayThriceInARow()
        {
            var games = new List<Game> {
                 new Game("Castle", "Letchworth"),
                 new Game("Castle", "Ulu"),
                 new Game("Ulu", "Castle"),
                 new Game("Ulu", "Letchworth"),
                 new Game("Letchworth", "Castle"),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.Equal((uint)2, sut.OptimisedGameOrder.MaxConsecutiveMatchesByAnyTeam);
        }

        [Fact]
        public void EveryoneShouldGetTheirGamesOutOfTheWayAsQuicklyAsPossible()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea"),
                 new Game("Braintree", "VKC"),
                 new Game("Blackwater", "Letchworth"),
                 new Game("Castle", "Avon"),
                 new Game("Blackwater", "VKC"),
                 new Game("Battersea", "Ulu"),
                 new Game("Braintree", "Letchworth"),
                 new Game("Castle", "Ulu"),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            // hard to figure out, but this is the best order
            //new Game("Castle", Battersea), "Castle" 5, battersea 1
            //new Game(Braintree, VKC), braintree 2, vkc 1
            //new Game(Battersea, "Ulu"), "Ulu" 2
            //new Game(Blackwater, VKC), blackwater 2
            //new Game(Braintree, "Letchworth"), "Letchworth" 1
            //new Game("Castle", "Ulu"), 
            //new Game(Blackwater, "Letchworth"),
            //new Game("Castle", Avon), Avon 0
            Assert.Equal((uint)14, sut.OptimisedGameOrder.GamesNotPlayedBetweenFirstAndLast);
        }


        [Fact]
        public void SpeedTest()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea"),
                 new Game("Braintree", "VKC"),
                 new Game("Blackwater", "Letchworth"),
                 new Game("Castle", "Avon"),
                 new Game("Blackwater", "VKC"),
                 new Game("Battersea", "Ulu"),
                 new Game("Braintree", "Letchworth"),
                 new Game("Castle", "Ulu"),
                 new Game("Avon", "VKC"),
                 new Game("Braintree", "Ulu"),
             };

            new TournamentDayCalculator(games).CalculateGameOrder();

            // 10 games takes 6-7 seconds to run
        }

        private bool PlayingTwiceInARow(string team, IEnumerable<Game> gameOrder)
        {
            bool playedInLastGame = false;

            foreach (var game in gameOrder)
            {
                bool playingInThisGame = game.Playing(team);
                if (playingInThisGame && playedInLastGame) return true;
                playedInLastGame = playingInThisGame;
            }

            return false;
        }
    }
}
