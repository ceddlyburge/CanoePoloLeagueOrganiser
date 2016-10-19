using System.Collections.Generic;
using Xamarin.Forms;

namespace XamarinFormsPortable
{
    class GridCedd : Grid
    {
#pragma warning disable S2376 // Write-only properties should not be used
        public IEnumerable<IEnumerable<View>> AddSomeRows
#pragma warning restore S2376 // Write-only properties should not be used
        {
            set
            {
                foreach (var row in value)
                    Children.AddHorizontal(row);
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