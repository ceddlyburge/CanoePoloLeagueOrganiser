using CanoePoloLeagueOrganiserXamarin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CanoePoloLeagueOrganiserTests
{
    public class IncludeXamarinCodeInCoverage
    {
        [Fact]
        public void Dummy()
        {
            var sut = new Dummy();
            Assert.True(sut != null);
        }
    }
}
