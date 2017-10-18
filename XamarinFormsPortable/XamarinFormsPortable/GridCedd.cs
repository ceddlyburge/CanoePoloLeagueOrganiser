using System.Collections.Generic;
using Xamarin.Forms;

namespace XamarinFormsPortable
{
    class GridCedd : Grid
    {
#pragma warning disable S2376 // Write-only properties should not be used
        public IReadOnlyList<IReadOnlyList<View>> SetRows
#pragma warning restore S2376 // Write-only properties should not be used
        {
            set
            {
                for (int row = 0; row < value.Count;row++)
                for (int column = 0; column < value[row].Count; column++)
                        Children.Add(value[row][column], column, row);// AddHorizontal(row);
            }

        }

#pragma warning disable S2376 // Write-only properties should not be used
        public IEnumerable<View> AddSomeRow
#pragma warning restore S2376 // Write-only properties should not be used
        {
            set
            {
                Children.AddHorizontal(value);
            }

        }
    }
}