using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CanoePoloLeagueOrganiser;
using System.Collections.Generic;
using System.Linq;

namespace CanoePoloLeagueOrganiserTests
{
    [TestClass]
    public class PermupotaterTests
    {
        [TestMethod]
        public void OneItem()
        {
            var list = new List<String> { "0" };

            var permutations = new Permupotater<String>().GetPermutations(list);

            Assert.AreEqual(1, permutations.Count());
            Assert.AreEqual("0", permutations.First().First());
        }

        [TestMethod]
        public void TwoItems()
        {
            var list = new List<String> { "0", "1" };

            var permutations = new Permupotater<String>().GetPermutations(list);

            Assert.AreEqual(2, permutations.Count());
            Assert.AreEqual("0", permutations.First().First());
            Assert.AreEqual("1", permutations.First().Last());
            Assert.AreEqual("1", permutations.Last().First());
            Assert.AreEqual("0", permutations.Last().Last());
        }

        [TestMethod]
        public void ThreeItems()
        {
            var list = new List<String> { "0", "1", "2" };

            var permutations = new Permupotater<String>().GetPermutations(list);

            Assert.AreEqual(6, permutations.Count());
            Assert.AreEqual("012", permutations.Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.AreEqual("021", permutations.Skip(1).Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.AreEqual("102", permutations.Skip(2).Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.AreEqual("120", permutations.Skip(3).Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.AreEqual("201", permutations.Skip(4).Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.AreEqual("210", permutations.Skip(5).Take(1).Single().Aggregate("", (s, l) => s + l));
        }
    }
}
