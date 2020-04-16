using Blazor.FileReader;
using BlazorMovies.Client.Helpers;
using BlazorMovies.Client.Repository;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorMovies.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddOptions();//Authorized sys
            //services.AddSingleton<SingletonService>();
            //services.AddSingleton<TransientService>();
            services.AddTransient<IRepositry, IRepositryInMemory>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<IMoviesRepository, MoviesRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddFileReaderService(op => op.InitializeOnFirstCall = true);
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
