using System;
using CanoePoloLeagueOrganiser;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CanoePoloLeagueOrganiserTests
{
    public class PermupotaterTests
    {
        [Fact]
        public void OneItem()
        {
            var list = new List<String> { "0" };

            var permutations = new Permupotater<String>().GetPermutations(list);

            Assert.Equal(1, permutations.Count());
            Assert.Equal("0", permutations.First().First());
        }

        [Fact]
        public void TwoItems()
        {
            var list = new List<String> { "0", "1" };

            var permutations = new Permupotater<String>().GetPermutations(list);

            Assert.Equal(2, permutations.Count());
            Assert.Equal("0", permutations.First().First());
            Assert.Equal("1", permutations.First().Last());
            Assert.Equal("1", permutations.Last().First());
            Assert.Equal("0", permutations.Last().Last());
        }

        [Fact]
        public void ThreeItems()
        {
            var list = new List<String> { "0", "1", "2" };

            var permutations = new Permupotater<String>().GetPermutations(list);

            Assert.Equal(6, permutations.Count());
            Assert.Equal("012", permutations.Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.Equal("021", permutations.Skip(1).Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.Equal("102", permutations.Skip(2).Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.Equal("120", permutations.Skip(3).Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.Equal("201", permutations.Skip(4).Take(1).Single().Aggregate("", (s, l) => s + l));
            Assert.Equal("210", permutations.Skip(5).Take(1).Single().Aggregate("", (s, l) => s + l));
        }
    }
}
