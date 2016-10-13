﻿using System;
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
    public class CalculatedOptimalGameOrderFromCurtailedListTests
    {
        [Fact]
        public void OneInputGameShouldResultInThisGameBeingPlayed()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea")
             };

            var sut = new CalculatedOptimalGameOrderFromCurtailedList(games, new NoCompromisesPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder();

            Assert.Equal(1, sut.OptimisedGameOrder.GameOrder.Count());
        }

        Permupotater<Game> EnumerateAllPermutations(List<Game> games)
        {
            return new Permupotater<Game>(games.ToArray(), NoCurtailment);
        }

        bool NoCurtailment(int[] gameIndexes, int length)
        {
            return false;
        }

        [Fact]
        public void CastleShouldNotPlayTwiceInARow()
        {
            var games = new List<Game> {
                 new Game("Castle", "Battersea"),
                 new Game("Castle", "Avon"),
                 new Game("Ulu", "Letchworth")
             };

            var sut = new CalculatedOptimalGameOrderFromCurtailedList(games, new NoCompromisesPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder();

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

            var sut = new CalculatedOptimalGameOrderFromCurtailedList(games, new NoCompromisesPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder().OptimisedGameOrder;

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

            var sut = new CalculatedOptimalGameOrderFromCurtailedList(games, new NoCompromisesPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder();

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

            var sut = new CalculatedOptimalGameOrderFromCurtailedList(games, new NoCompromisesPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder();

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

            var gameOrder = new CalculatedOptimalGameOrderFromCurtailedList(games, new NoCompromisesPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder();

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

            var gameOrder = new CalculatedOptimalGameOrderFromCurtailedList(games, new TenSecondPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder();

            // allow it an extra second to finish up or whatever. It actually finished in two seconds as it finds an acceptable solution earlier.
            Assert.True(DateTime.Now.Subtract(dateStarted) < TimeSpan.FromSeconds(11));
            Assert.False(gameOrder.PerfectOptimisation);
            Assert.False(string.IsNullOrEmpty(gameOrder.OptimisationMessage));
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

            var gameOrder = new CalculatedOptimalGameOrderFromCurtailedList(games, new TenSecondPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder();

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

            new CalculatedOptimalGameOrderFromCurtailedList(games, new NoCompromisesPragmatiser(), EnumerateAllPermutations(games)).CalculateGameOrder();

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