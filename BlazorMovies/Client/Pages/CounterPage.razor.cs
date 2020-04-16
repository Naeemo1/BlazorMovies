using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMovies.Client.Pages
{
    public partial class CounterPage
    {

        private int currentCount = 0;

        public void IncrementCount()
        {
            currentCount++;
        }

    }
}
