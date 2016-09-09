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
            string[] list = { "0" };
            var permutations = new List<String[]>();

            new Permupotater().EnumeratePermutations<String>(list, s => { permutations.Add((string[])s.Clone()); return true;  } );

            Assert.Equal(1, permutations.Count);
            Assert.Equal("0", permutations.First().First());
        }

        [Fact]
        public void TwoItems()
        {
            string[] list = { "0", "1" };
            var permutations = new List<String[]>();

            new Permupotater().EnumeratePermutations<String>(list, s => { permutations.Add((string []) s.Clone()); return true; });

            Assert.Equal(2, permutations.Count);
            Assert.Equal("0", permutations.First()[0]);
            Assert.Equal("1", permutations.First()[1]);
            Assert.Equal("1", permutations.Last()[0]);
            Assert.Equal("0", permutations.Last()[1]);
        }

        [Fact]
        public void ThreeItems()
        {
            string[] list = { "0", "1", "2" };
            var permutations = new List<String[]>();

            new Permupotater().EnumeratePermutations<String>(list, s => { permutations.Add((string[])s.Clone()); return true; });

            var flattenedPermutations = permutations.Select(p => p.Aggregate("", (s, l) => s + l)).ToList();

            Assert.Equal(6, flattenedPermutations.Count);
            Assert.True(flattenedPermutations.Contains("012"));
            Assert.True(flattenedPermutations.Contains("021"));
            Assert.True(flattenedPermutations.Contains("102"));
            Assert.True(flattenedPermutations.Contains("120"));
            Assert.True(flattenedPermutations.Contains("201"));
            Assert.True(flattenedPermutations.Contains("210"));
        }
    }
}
