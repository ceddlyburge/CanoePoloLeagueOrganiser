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

            var permutations = new Permupotater<String>().GetPermutations(list).Select(p => p.Aggregate("", (s, l) => s + l));

            Assert.Equal(6, permutations.Count());
            Assert.True(permutations.Contains("012"));
            Assert.True(permutations.Contains("021"));
            Assert.True(permutations.Contains("102"));
            Assert.True(permutations.Contains("120"));
            Assert.True(permutations.Contains("201"));
            Assert.True(permutations.Contains("210"));
        }
    }
}
