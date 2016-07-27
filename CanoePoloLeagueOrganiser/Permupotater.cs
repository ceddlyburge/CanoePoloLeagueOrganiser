using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class Permupotater<T> 
        where T : class
    {
        public List<IEnumerable<T>> GetPermutations(IEnumerable<T> things)
        {
            if (things.Count() == 1) return new List<IEnumerable<T>> { things.Take(1) };

            var permutations = new List<IEnumerable<T>>();

            foreach (var thing in things)
            {
                foreach (var subPermuation in GetPermutations(things.Where(t => t != thing).ToList()))
                    permutations.Add(things.Where(t => t == thing).Concat(subPermuation).ToList());
            }

            return permutations;
        }
    }
}
