using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsPortable
{
    public class StackLayoutCedd : StackLayout
    {
#pragma warning disable S2376 // Write-only properties should not be used
        public IEnumerable<View> AddSomeChildren
#pragma warning restore S2376 // Write-only properties should not be used
        {
            set
            {
                foreach (var child in value)
                    Children.Add(child);
            }
        }
    }
}