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
        [Fact]
        public void OneInputGameShouldResultInThisGameBeingPlayed()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea")
             };

            var sut = new TournamentDayCalculator(games, new NoCompromisesPragmatiser()).CalculateGameOrder();

            Assert.Equal(1, sut.OptimisedGameOrder.GameOrder.Count());
        }

        [Fact]
        public void CastleShouldNotPlayTwiceInARow()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea"),
                 new Game("Castle", "Avon"),
                 new Game("Ulu", "Letchworth")
             };

            var sut = new TournamentDayCalculator(games, new NoCompromisesPragmatiser()).CalculateGameOrder();

            Assert.False(PlayingTwiceInARow("Castle", sut.OptimisedGameOrder.GameOrder));
        }

        [Fact]
        public void CastleAndLetchworthAndAvonShouldNotPlayTwiceInARow()
        {
            var games = new List<Game> {
                 new Game("Ulu", "Letchworth"),
                 new Game("Battersea", "Letchworth"),
                 new Game("Castle", "Avon"),
                 new Game("Castle", "Avon")
             };

            var sut = new TournamentDayCalculator(games, new NoCompromisesPragmatiser()).CalculateGameOrder().OptimisedGameOrder;

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
                 new Game("Battersea", "Letchworth")
             };

            var sut = new TournamentDayCalculator(games, new NoCompromisesPragmatiser()).CalculateGameOrder();

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
                 new Game("Letchworth", "Castle")
             };

            var sut = new TournamentDayCalculator(games, new NoCompromisesPragmatiser()).CalculateGameOrder();

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
                 new Game("Castle", "Ulu")
             };

            var gameOrder = new TournamentDayCalculator(games, new NoCompromisesPragmatiser()).CalculateGameOrder();

            // hard to figure out, but this is the best order
            //new Game("Castle", Battersea), "Castle" 5, battersea 1
            //new Game(Braintree, VKC), braintree 2, vkc 1
            //new Game(Battersea, "Ulu"), "Ulu" 2
            //new Game(Blackwater, VKC), blackwater 2
            //new Game(Braintree, "Letchworth"), "Letchworth" 1
            //new Game("Castle", "Ulu"), 
            //new Game(Blackwater, "Letchworth"),
            //new Game("Castle", Avon), Avon 0
            Assert.Equal((uint)14, gameOrder.OptimisedGameOrder.GamesNotPlayedBetweenFirstAndLast);
            Assert.True(gameOrder.PerfectOptimisation);
            Assert.True(string.IsNullOrEmpty(gameOrder.OptimisationMessage));
        }

        [Fact]
        public void RespondWithSomethingWhenPermutationsGetOutOfHand()
        {
            DateTime dateStarted = DateTime.Now;

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
                 new Game("Castle", "Battersea"),
                 new Game("Braintree", "VKC"),
                 new Game("Blackwater", "Letchworth"),
                 new Game("Castle", "Avon"),
                 new Game("Blackwater", "VKC"),
                 new Game("Battersea", "Ulu"),
                 new Game("Braintree", "Letchworth"),
                 new Game("Castle", "Ulu"),
                 new Game("Avon", "VKC")
             };

            var gameOrder = new TournamentDayCalculator(games, new TenSecondPragmatiser()).CalculateGameOrder();

            // allow it an extra second to finish up or whatever. It actually finished in two seconds as it finds an acceptable solution earlier.
            Assert.True(DateTime.Now.Subtract(dateStarted) < TimeSpan.FromSeconds(11));
            Assert.False(gameOrder.PerfectOptimisation);
            Assert.False(string.IsNullOrEmpty(gameOrder.OptimisationMessage));
        }

        // This is an actual game order that I used. The ten second pragmatiser didn't work, I think because it took more than ten seconds to return the first result. It also didn't produce a very good solution, as it got stuck analysing a gazillion permutations that all started with clapham playing three times in a row. I want it to produce a good result within 10 seconds.
        [Fact]
        public void FourteenGamesNeedsToProduceAUsefulResult()
        {
            DateTime dateStarted = DateTime.Now;

            var games = new List<Game> {
                new Game("Clapham", "Surrey"),
                new Game("Clapham", "ULU"),
                new Game("Clapham", "Meridian"),
                new Game("Blackwater", "Clapham"),
                new Game("ULU", "Blackwater"),
                new Game("Surrey", "Castle"),
                new Game("ULU", "Meridian"),
                new Game("Letchworth", "ULU"),
                new Game("Castle", "Blackwater"),
                new Game("Surrey", "Letchworth"),
                new Game("Meridian", "Castle"),
                new Game("Blackwater", "Letchworth"),
                new Game("Meridian", "Surrey"),
                new Game("Castle", "Letchworth")
             };

            var gameOrder = new TournamentDayCalculator(games, new TenSecondPragmatiser()).CalculateGameOrder();

            Assert.True(DateTime.Now.Subtract(dateStarted) < TimeSpan.FromSeconds(11));
            Assert.True(gameOrder.OptimisedGameOrder.MaxConsecutiveMatchesByAnyTeam == 0);
        }

        [Fact]
        public void RespondWithSomethingWhenPermutationsGetOutOfHandAndNoGoodSolution()
        {
            DateTime dateStarted = DateTime.Now;

            var games = new List<Game> {
                 new Game("Castle", "1"),
                 new Game("Castle", "2"),
                 new Game("Castle", "3"),
                 new Game("Castle", "4"),
                 new Game("Castle", "5"),
                 new Game("Castle", "6"),
                 new Game("Castle", "7"),
                 new Game("Castle", "8"),
                 new Game("Castle", "9"),
                 new Game("Castle", "10"),
                 new Game("Castle", "11"),
                 new Game("Castle", "12"),
                 new Game("Castle", "13"),
                 new Game("Castle", "14"),
                 new Game("Castle", "15"),
                 new Game("Castle", "16"),
                 new Game("Castle", "17"),
                 new Game("Castle", "18"),
                 new Game("Castle", "19")
             };

            var gameOrder = new TournamentDayCalculator(games, new TenSecondPragmatiser()).CalculateGameOrder();

            // allow it an extra second to finish up or whatever. This test must take 10 seconds as there are non possible good solutions
            Assert.True(DateTime.Now.Subtract(dateStarted) < TimeSpan.FromSeconds(11));
            Assert.False(gameOrder.PerfectOptimisation);
            Assert.False(string.IsNullOrEmpty(gameOrder.OptimisationMessage));
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
                 new Game("Braintree", "Ulu")
             };

            new TournamentDayCalculator(games, new NoCompromisesPragmatiser()).CalculateGameOrder();

            // 10 games takes 0.5 - 1 seconds to run, this test is just here to make analysing optimisations easier
        }

        private bool PlayingTwiceInARow(string team, IEnumerable<Game> gameOrder)
        {
            bool playedInLastGame = false;

            foreach (var game in gameOrder)
            {
                var playingInThisGame = game.Playing(team);
                if (playingInThisGame && playedInLastGame) return true;
                playedInLastGame = playingInThisGame;
            }

            return false;
        }
    }
}
