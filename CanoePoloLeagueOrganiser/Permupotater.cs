using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanoePoloLeagueOrganiser
{
    public class Permutation
    {
        public void EnumeratePermutations<T>(T[] items, Func<T[], bool> callback)
        {
            int length = items.Length;

            var work = new int[length];
            for (var i = 0; i < length; i++)
                work[i] = i;

            var result = new T[length];

            foreach (var index in GetIntPermutations(work, 0, length))
            {
                for (var i = 0; i < length; i++) result[i] = items[index[i]];
                if (callback(result) == false) break;
                //yield return result;
                //Debug.WriteLine(Concatentate(index));
            }
        }
        public IEnumerable<T[]> GetPermutations<T>(T[] items)
        {
            var work = new int[items.Length];
            for (var i = 0; i < work.Length; i++)
            {
                work[i] = i;
            }
            foreach (var index in GetIntPermutations(work, 0, work.Length))
            {
                var result = new T[index.Length];
                for (var i = 0; i < index.Length; i++) result[i] = items[index[i]];
                yield return result;
                //Debug.WriteLine(result.ToString());
            }
        }

        public IEnumerable<int[]> GetIntPermutations(int[] index, int offset, int len)
        {
            switch (len)
            {
                case 1:
                    yield return index;
                    //Debug.WriteLine(Concatentate(index));
                    break;
                case 2:
                    yield return index;
                    //Debug.WriteLine(Concatentate(index));
                    Swap(index, offset, offset + 1);
                    yield return index;
                    //Debug.WriteLine(Concatentate(index));
                    Swap(index, offset, offset + 1);
                    break;
                default:
                    foreach (var result in GetIntPermutations(index, offset + 1, len - 1))
                    {
                        yield return result;
                        //Debug.WriteLine(Concatentate(index));
                    }
                    for (var i = 1; i < len; i++)
                    {
                        Swap(index, offset, offset + i);
                        foreach (var result in GetIntPermutations(index, offset + 1, len - 1))
                        {
                            yield return result;
                            //Debug.WriteLine(Concatentate(index));
                        }
                        Swap(index, offset, offset + i);
                    }
                    break;
            }
        }

        private string Concatentate(int[] index)
        {
            return index.Aggregate("", (s, i) => s + i.ToString());
        }

        private static void Swap(IList<int> index, int offset1, int offset2)
        {
            var temp = index[offset1];
            index[offset1] = index[offset2];
            index[offset2] = temp;
        }

    }

    public class Permupotater<T>
                where T : class
    {
        public IReadOnlyList<IReadOnlyList<T>> GetPermutations(IEnumerable<T> things)
        {
            var lists = new Permutation().GetPermutations<T>(things.ToArray()).ToList();

            var perms = new List<IReadOnlyList<T>>();
            foreach (var list in lists)
                perms.Add(list.ToList());

            return perms;
        }
    }



    //public class Permupotater<T>
    //            where T : class
    //    {
    //        public List<IEnumerable<T>> GetPermutations(IEnumerable<T> things)
    //        {
    //            if (things.Count() == 1) return new List<IEnumerable<T>> { things.Take(1) };

    //            var permutations = new List<IEnumerable<T>>();

    //            foreach (var thing in things)
    //            {
    //                foreach (var subPermuation in GetPermutations(things.Where(t => !t.Equals(thing)).ToList()))
    //                    permutations.Add(things.Where(t => t.Equals(thing)).Concat(subPermuation).ToList());
    //            }

    //            return permutations;
    //        }
    //    }
}
