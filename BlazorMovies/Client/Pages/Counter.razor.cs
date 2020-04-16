using BlazorMovies.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorMovies.Client.Shared.MainLayout;

namespace BlazorMovies.Client.Pages
{
    public partial class Counter
    {
        [Inject] SingletonService singleton { get; set; }
        [Inject] TransientService transient { get; set; }
        [Inject] IJSRuntime js { get; set; }
        //[CascadingParameter(Name = "Color")] public string Color { get; set; }
        //[CascadingParameter(Name = "Size")] public string Size { get; set; }
        [CascadingParameter] public AppStyle appStyle { get; set; }

        private List<Movie> movies;

        //protected async override Task OnInitializedAsync()
        protected override void OnInitialized()
        {
            //await Task.Delay(3000);

            movies = new List<Movie>
        {
            new Movie() {Title = "Joker" }
        };
        }

        private int currentCount = 0;
        private static int currentCounterStatic = 0;

        //Invoke Static Method c# from js
        private async Task IncrementCount()
        {
            currentCount++;
            transient.Value = currentCount;
            singleton.Value = currentCount;
            currentCounterStatic++;
            await js.InvokeVoidAsync("dotnetStaticInvokation");
        }

        //Invoke Instance Method c# from js
        [JSInvokable]
        public async Task IncrementCountInstance()
        {
            currentCount++;
            transient.Value = currentCount;
            singleton.Value = currentCount;
            currentCounterStatic++;
            await js.InvokeVoidAsync("dotnetStaticInvokation");
        }

        private async Task IncrementCountJavascript()
        {
            await js.InvokeVoidAsync("dotnetInstanceInvokation",
                DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public static Task<int> GetCurrentCount()
        {
            return Task.FromResult(currentCounterStatic);
        }

    }
}
