using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CanoePoloLeagueOrganiser;
using System.Collections.Generic;
using System.Linq;

namespace CanoePoloLeagueOrganiserTests
{
    //3207

    // install sonar qube plugin
    // install that other plug in that I contributed to
    // install contracts plug in and work on the static analysis
    // can make the tests run on github magically?
    // can get sonar qube to run on github magically?

    // aims:
    // - The maximum consecutive games a team plays should be minimised
    // - The number of times a team plays consecutive matches should be minimised
    // - the amount of games that teams don't play between their first and last games should be minimised
    [TestClass]
    public class TournamentDayTests
    {
        private readonly Team Castle, Battersea, Ulu, Letchworth, Avon;
        private readonly Team Blackwater;
        private readonly Team Braintree;
        private readonly Team VKC;

        public TournamentDayTests()
        {
            this.Castle = new Team("Castle");
            this.Battersea = new Team("Battersea");
            this.Ulu = new Team("ULU");
            this.Letchworth = new Team("Letchworth");
            this.Avon = new Team("Avon");
            this.Blackwater = new Team("Blackwater");
            this.Braintree = new Team("Braintree");
            this.VKC = new Team("VKC");
        }

        [TestMethod]
        public void CastleAndBatterseaShouldPlay()
        {
            var games = new List<Game> {
                 new Game(Castle, Battersea),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.AreEqual(1, sut.GameOrder.Count(), "One input game should result in this game being played");
        }

        [TestMethod]
        public void CastleShouldNotPlayTwiceInARow()
        {
            var games = new List<Game> {
                 new Game(Castle, Battersea),
                 new Game(Castle, Avon),
                 new Game(Ulu, Letchworth),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.IsFalse(PlayingTwiceInARow(Castle, sut.GameOrder), "Castle should play the first and last games so that they do not play twice in a row");
        }

        [TestMethod]
        public void CastleAndLetchworthShouldNotPlayTwiceInARow()
        {
            var games = new List<Game> {
                 new Game(Ulu, Letchworth),
                 new Game(Battersea, Letchworth),
                 new Game(Castle, Avon),
                 new Game(Castle, Avon),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.IsFalse(PlayingTwiceInARow(Castle, sut.GameOrder), "Castle should play the first and third (or second and fourth) games so that they do not play twice in a row");
            Assert.IsFalse(PlayingTwiceInARow(Letchworth, sut.GameOrder), "Letchworth should play the first and third (or second and fourth) games so that they do not play twice in a row");
            Assert.IsFalse(PlayingTwiceInARow(Avon, sut.GameOrder), "Avon should play the first and third (or second and fourth) games so that they do not play twice in a row");
        }

        [TestMethod]
        public void CastleShouldNotPlayThriceInARow()
        {
            var games = new List<Game> {
                 new Game(Castle, Battersea),
                 new Game(Castle, Letchworth),
                 new Game(Ulu, Castle),
                 new Game(Battersea, Letchworth),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.AreEqual((uint) 1, sut.OccurencesOfTeamsPlayingConsecutiveMatches, "Castle have to play two in a row once");
        }

        [TestMethod]
        public void NobodyShouldNotPlayThriceInARow()
        {
            var games = new List<Game> {
                 new Game(Castle, Letchworth),
                 new Game(Castle, Ulu),
                 new Game(Ulu, Castle),
                 new Game(Ulu, Letchworth),
                 new Game(Letchworth, Castle),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            Assert.AreEqual((uint)2, sut.MaxConsecutiveMatchesByAnyTeam, "Castle should only play two in a row, at the expense of Ulu and Letchworth also playing two in a row");
        }

        [TestMethod]
        public void EveryoneShouldGetTheirGamesOutOfTheWayAsQuicklyAsPossible()
        {
            var games = new List<Game> {
                 new Game(Castle, Battersea),
                 new Game(Braintree, VKC),
                 new Game(Blackwater, Letchworth),
                 new Game(Castle, Avon),
                 new Game(Blackwater, VKC),
                 new Game(Battersea, Ulu),
                 new Game(Braintree, Letchworth),
                 new Game(Castle, Ulu),
             };

            var sut = new TournamentDayCalculator(games).CalculateGameOrder();

            // hard to figure out, but this is the best order
            //new Game(Castle, Battersea), castle 5, battersea 1
            //new Game(Braintree, VKC), braintree 2, vkc 1
            //new Game(Battersea, Ulu), ulu 2
            //new Game(Blackwater, VKC), blackwater 2
            //new Game(Braintree, Letchworth), letchworth 1
            //new Game(Castle, Ulu), 
            //new Game(Blackwater, Letchworth),
            //new Game(Castle, Avon), Avon 0

            Assert.AreEqual((uint) 14, sut.GamesNotPlayedBetweenFirstAndLast, "Teams should spend as litlle time on the sidelines as possible, as long as they don't play consecutive games");
        }

        private bool PlayingTwiceInARow(Team team, IEnumerable<Game> gameOrder)
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
